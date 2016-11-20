using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Reflection;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Pipelines;
using Boo.Lang.Compiler.IO;
using AlbumArtDownloader.Scripts;

namespace AlbumArtDownloader
{
	public static class ScriptManager
	{
		/// <summary>
		/// Compiles the script files, if the cache is out of date
		/// </summary>
		/// <returns>True if successful</returns>
		public static bool CompileIfRequired()
		{
			//Compile if required
			if (!CheckScriptCache())
			{
				Console.WriteLine("Script cache out of date, rebuilding:\n");
				return CompileScripts();
			}
			return true;
		}

		/// <summary>
		/// Load all the scripts from dlls in the Scripts folder.
		/// <remarks>Boo scripts should previously have been compiled into
		/// a dll (boo script cache.dll)</remarks> 
		/// </summary>
		public static void LoadScripts()
		{
			sScripts = new List<IScript>();
			foreach (string scriptsPath in ScriptsPaths)
			{
				foreach (string dllFile in Directory.GetFiles(scriptsPath, "*.dll"))
				{
					try
					{
						Assembly assembly = Assembly.LoadFile(dllFile);
						foreach (Type type in assembly.GetTypes())
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
									sScripts.Add(script);
							}
							catch (Exception e)
							{
								//Skip the type. Does this need to display a user error message?
								System.Diagnostics.Debug.Fail(String.Format("Could not load script: {0}\n\n{1}", type.Name, e.Message));
							}
						}
					}
					catch (Exception e)
					{
						//Skip the assembly
						System.Diagnostics.Debug.Fail(String.Format("Could not load assembly: {0}\n\n{1}", dllFile, e.Message));
					}
				}
			}
		}

		private static List<IScript> sScripts;
		public static IEnumerable<IScript> Scripts
		{
			get
			{
				if (sScripts == null)
					throw new InvalidOperationException("Must call LoadScripts() before using this property");

				return sScripts;
			}
		}

		private static readonly string sBooScriptCacheDll = "boo script cache.dll";
		private static string mBooScriptsCacheFile;
		private static string BooScriptsCacheFile
		{
			get
			{
				if (mBooScriptsCacheFile == null)
					mBooScriptsCacheFile = Path.Combine(Path.Combine(Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath), "Scripts"), sBooScriptCacheDll);

				return mBooScriptsCacheFile;
			}
		}

		/// <summary>
		/// Returns each path that scripts may be found in.
		/// </summary>
		private static IEnumerable<string> ScriptsPaths
		{
			get
			{
				//Scripts may be in a "scripts" subfolder of the application folder
				yield return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Scripts");

				//They may also be in the scripts cache file path.
				yield return Path.GetDirectoryName(BooScriptsCacheFile);
			}
		}


		/// <summary>
		/// Check to see if the script cache is up to date.
		/// </summary>
		/// <returns>True if the script cache is up to date, false if it needs to be recompiled</returns>
		private static bool CheckScriptCache()
		{
			//Check whether boo script cache is up to date or not
			try
			{
				FileInfo cacheFileInfo = new FileInfo(BooScriptsCacheFile);
				if (cacheFileInfo.Exists)
				{
					//There is a cache file, so check if it is up to date or not
					bool rebuildCache = false;

					foreach (string scriptsPath in ScriptsPaths)
					{
						DirectoryInfo scriptFolder = new DirectoryInfo(scriptsPath);

						if (scriptFolder.Exists)
						{
							if (scriptFolder.LastWriteTimeUtc > cacheFileInfo.LastWriteTimeUtc)
							{
								//This folder has been modified (file added or removed, so rebuild)
								rebuildCache = true;
								break;
							}

							//Check to see if any boo script files within it have been modified since that time (as this doesn't seem to trigger the LastWriteTime of the folder reliably)
							foreach (FileInfo fileInfo in scriptFolder.GetFiles("*.boo"))
							{
								if (fileInfo.LastWriteTimeUtc > cacheFileInfo.LastWriteTimeUtc)
								{
									rebuildCache = true;
									break;
								}
							}
						}

						if (rebuildCache)
							break; //No need to check the others, we already need to rebuild
					}

					if (!rebuildCache)
						return true; //No need to rebuild the cache
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.Fail("Exception while checking script file last update times:\n\n" + e.Message);
				//Ignore the exception, and try to recompile. If that doesn't fix it, then errors will be reported by the recompilation process.
				return false;
			}

			return false;
		}

		/// <summary>
		/// Compiles the boo scripts.
		/// </summary>
		/// <returns>True if successful</returns>
		private static bool CompileScripts()
		{
			try
			{
				FileInfo outputFile = new FileInfo(BooScriptsCacheFile);
				if (!outputFile.Directory.Exists)
					outputFile.Directory.Create(); //Enusre that the output file can be created.

				Console.WriteLine("Searching for scripts...");

				Dictionary<string, string> scripts = new Dictionary<string,string>();
				foreach (string scriptsPath in ScriptsPaths)
				{
					foreach (string scriptFile in Directory.GetFiles(scriptsPath, "*.boo"))
					{
						//Later files override earlier ones of the same name
						scripts[Path.GetFileName(scriptFile).ToLowerInvariant()] = scriptFile;
					}
				}

				List<string> readableFiles = new List<string>();
				List<string> references = new List<string>();

				foreach (string scriptFile in scripts.Values)
				{
					try
					{
						using (StreamReader reader = File.OpenText(scriptFile))
						{
							string firstLine = reader.ReadLine();
							if (firstLine.StartsWith("# refs: ") && firstLine.Length > 8)
							{
								string refsText = firstLine.Substring(8);
								references.AddRange(refsText.Split(' '));
							}
							readableFiles.Add(scriptFile);
						}
					}
					catch (Exception fileReadingException)
					{
						Console.WriteLine(String.Format("Skipping unreadable file: \"{0}\"\n  {1}", scriptFile, fileReadingException.Message));
					}
				}

				Console.WriteLine(String.Format("Found {0} files: [{1}]...", readableFiles.Count, FormatFileListForDisplay(readableFiles)));
				Console.WriteLine(String.Format("Loading references: [{0}]...", string.Join(", ", references.ToArray())));

				BooCompiler compiler = new BooCompiler();
				compiler.Parameters.Ducky = true; //Required to allow late-binding to "coverart" parameter
				compiler.Parameters.OutputType = CompilerOutputType.Library;
#if DEBUG
				compiler.Parameters.Debug = true;
#else
				compiler.Parameters.Debug = false;
#endif
				compiler.Parameters.Pipeline = new CompileToFile();
				compiler.Parameters.OutputAssembly = outputFile.FullName;
				foreach (string reference in references)
				{
					compiler.Parameters.References.Add(compiler.Parameters.LoadAssembly(reference, true));
				}
				foreach (string scriptFile in readableFiles)
				{
					compiler.Parameters.Input.Add(new FileInput(scriptFile));
				}
				Console.Write("\nCompiling scripts... ");
				
				bool origCursorVisible = Console.CursorVisible; //To restore it to its previous state
				Console.CursorVisible = false;
				try
				{
					//Report compilation progress
					//Ensure the progress bar is not larger than the remaining width
					int progressBarWidth = Math.Min(30, Console.WindowWidth - Console.CursorLeft - 1);
					//Align the progress bar to the right
					ProgressBar progressBar = new ProgressBar(new WritePoint(Console.WindowWidth - progressBarWidth - 1, Console.CursorTop), progressBarWidth);
					progressBar.Maximum = compiler.Parameters.Pipeline.Count;
					compiler.Parameters.Pipeline.BeforeStep += new CompilerStepEventHandler(
						delegate(object sender, CompilerStepEventArgs args)
						{
							progressBar.Value++;
						}
					);
					CompilerContext compilerContext = compiler.Run();

					progressBar.Clear();

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
						ReportCompilerIssue("warning", warning.LexicalInfo, warning.Code, warning.Message);
					}
					foreach (CompilerError error in compilerContext.Errors)
					{
						ReportCompilerIssue("error", error.LexicalInfo, error.Code, error.Message);
					}

					Console.WriteLine();
					return result;
				}
				finally
				{
					//Restore cursor visibility
					Console.CursorVisible = origCursorVisible;
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(String.Format("\nError: {0}\n", exception.Message));
				return false;
			}
		}

		private static string FormatFileListForDisplay(IEnumerable<string> files)
		{
			StringBuilder builder = new StringBuilder();
			bool first = true;
			foreach (string file in files)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					builder.Append(", ");
				}
				builder.Append(Path.GetFileName(file));
			}
			return builder.ToString();
		}

		private static void ReportCompilerIssue(string severity, Boo.Lang.Compiler.Ast.LexicalInfo lexicalInfo, string code, string message)
		{
			Console.WriteLine(String.Format("{0}({1},{2}): {3} {4}: {5}", Path.GetFileName(lexicalInfo.FileName), lexicalInfo.Line, lexicalInfo.Column, severity, code, message));
		}
	}
}
