using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Reflection;
using System.Configuration;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;
using AlbumArtDownloader.Scripts;


namespace AlbumArtDownloader.Updates
{
	class Program
	{
		static int Main(string[] args)
		{
#if DEBUG
			Console.WriteLine("Skipping Update.xml generation in Debug mode");
#else
			Console.WriteLine("Album Art Downloader XUI Updates.xml generator");
			Console.WriteLine("(this is an internal development tool)");
			Console.WriteLine();

			XElement updates = new XElement("Updates", new XAttribute("BaseURI", "http://album-art.sourceforge.net/scripts/"));

			//Get main app version
			Version appVersion = typeof(AlbumArtDownloader.App).Assembly.GetName().Version;
			string appName = String.Format("Album Art Downloader XUI v{0}.{1}" + (appVersion.Build > 0 ? ".{2}" : ""), appVersion.Major, appVersion.Minor, appVersion.Build);

			Console.WriteLine(appName);

			updates.Add(new XElement("Application", 
				new XAttribute("Version", appVersion.ToString()),
				new XAttribute("Name", appName),
				new XAttribute("URI", "https://sourceforge.net/project/platformdownload.php?group_id=187008")));

			//Write all scripts
			ProcessScripts(updates);

			string outFile = Path.Combine(String.Join(" ", args), "Updates.xml");

			Console.Write("Saving to: \"" + outFile + "\" ...");
			new XDocument(
				new XProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"Updates.xsl\""),
				updates).Save(outFile);
			Console.WriteLine("done.");
#endif
			return 0;
		}

		private static void ProcessScripts(XElement updates)
		{
			try
			{
				Console.WriteLine("Searching for scripts...");

				Dictionary<string, string> scripts = new Dictionary<string, string>();
				foreach (string scriptsPath in ScriptsPaths)
				{
					foreach (string scriptFile in Directory.GetFiles(scriptsPath, "*.boo"))
					{
						//Later files override earlier ones of the same name
						scripts[Path.GetFileName(scriptFile).ToLowerInvariant()] = scriptFile;
					}
				}

				//HACK: Remove known common files
				scripts.Remove("util.boo");
				scripts.Remove("amazon-common.boo");

				foreach (string scriptFile in scripts.Values)
				{
					try
					{
						Console.Write(String.Format("Processing {0}... ", Path.GetFileName(scriptFile)));
							
						using (StreamReader reader = File.OpenText(scriptFile))
						{
							List<string> references = new List<string>();

							string firstLine = reader.ReadLine();
							if (firstLine.StartsWith("# refs: ") && firstLine.Length > 8)
							{
								string refsText = firstLine.Substring(8);
								references.AddRange(refsText.Split(' '));
							}
							
							BooCompiler compiler = new BooCompiler();
							compiler.Parameters.Ducky = true; //Required to allow late-binding to "coverart" parameter
							compiler.Parameters.OutputType = CompilerOutputType.Library;
							compiler.Parameters.Debug = false;
							compiler.Parameters.Pipeline = new CompileToMemory();
							
							//HACK: Add known common files and references (as each script is being compiled individually
							compiler.Parameters.Input.Add(new FileInput(Path.Combine(Path.GetDirectoryName(scriptFile), "util.boo")));
							compiler.Parameters.Input.Add(new FileInput(Path.Combine(Path.GetDirectoryName(scriptFile), "amazon-common.boo")));
							references.Add("System.Core");
							references.Add("System.Web");
							references.Add("System.Web.Extensions");

							//Console.WriteLine(String.Format("Loading references: [{0}]...", string.Join(", ", references.ToArray())));
							foreach (string reference in references)
							{
								compiler.Parameters.References.Add(compiler.Parameters.LoadAssembly(reference, true));
							}

							compiler.Parameters.Input.Add(new FileInput(scriptFile));

							CompilerContext compilerContext = compiler.Run();

							bool result;
							if (compilerContext.Errors.Count > 0)
							{
								Console.WriteLine("failed.");
								result = false; //faliure
							}
							else if (compilerContext.Warnings.Count > 0)
							{
								Console.WriteLine("done, but with warnings.");
								result = true; //Allow to continue
							}
							else
							{
								Console.WriteLine("done.");
								result = true; //Success
							}

							//Report warnings and errors
							foreach (CompilerWarning warning in compilerContext.Warnings)
							{
								// Ignore warning about duplicated CoverSources namespace
								if (!(warning.Code == "BCW0008" && warning.Message.Contains("'CoverSources'")))
								{
									ReportCompilerIssue("warning", warning.LexicalInfo, warning.Code, warning.Message);
								}
							}
							foreach (CompilerError error in compilerContext.Errors)
							{
								ReportCompilerIssue("error", error.LexicalInfo, error.Code, error.Message);
							}

							if (result)
							{
								//Find the script type
								foreach (Type type in compilerContext.GeneratedAssembly.GetTypes())
								{
									try
									{
										IScript script = null;
										//Check for types implementing IScript
										if (typeof(IScript).IsAssignableFrom(type))
										{
											if (!type.IsAbstract)
											{
												script = (IScript)Activator.CreateInstance(type);
											}
										}
										//Check for static scripts (for backwards compatibility)
										else if (type.Namespace == "CoverSources")
										{
											script = new StaticScript(type);
										}

										if (script != null)
										{
											//Obtain name and version number
											var scriptXml = new XElement("Script",
																new XAttribute("Name", script.Name),
																new XAttribute("URI", Path.GetFileName(scriptFile)),
																new XAttribute("Version", script.Version));

											if (script is ICategorised)
											{
												var category = ((ICategorised)script).Category;
												if (!String.IsNullOrEmpty(category))
												{
													scriptXml.Add(new XAttribute("Category", category));
												}
											}

											//Hack: Add dependency to known dependent files
											if (script.Name.StartsWith("Amazon "))
											{
												scriptXml.Add(new XElement("Dependency", "amazon-common.boo"));
											}

											updates.Add(scriptXml);
										}
									}
									catch (Exception e)
									{
										//Skip the type. Does this need to display a user error message?
										Console.WriteLine(String.Format("Warning: Could not load script: {0}\n\n{1}", type.Name, e.Message));
									}
								}
							}
						}
					}
					catch (Exception fileReadingException)
					{
						Console.WriteLine(String.Format("Skipping unreadable file: \"{0}\"\n  {1}", scriptFile, fileReadingException.Message));
					}
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(String.Format("\nError: {0}\n", exception.Message));
			}
		}

		private static void ReportCompilerIssue(string severity, Boo.Lang.Compiler.Ast.LexicalInfo lexicalInfo, string code, string message)
		{
			Console.WriteLine(String.Format("{0}({1},{2}): {3} {4}: {5}", Path.GetFileName(lexicalInfo.FileName), lexicalInfo.Line, lexicalInfo.Column, severity, code, message));
		}

		/// <summary>
		/// Returns each path that scripts may be found in.
		/// </summary>
		private static IEnumerable<string> ScriptsPaths
		{
			get
			{
				//Scripts will be in a "scripts" subfolder of the application folder
				yield return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "scripts");

				//Ignore scripts in the Script Cache folder, for generating the Update XML.
			}
		}
	}
}
