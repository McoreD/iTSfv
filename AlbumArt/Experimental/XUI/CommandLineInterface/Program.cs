using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using AlbumArtDownloader.Scripts;

namespace AlbumArtDownloader
{
	class Program
	{
		static int Main(string[] args)
		{
			Console.Write("Album Art Downloader XUI Command Line Interface version ");
			Console.WriteLine(Assembly.GetEntryAssembly().GetName().Version);
			Console.WriteLine();

			//valuedParameters is a list of parameters which must have values - they can not be just switches.
			string[] valuedParameters = { "artist", "ar", "album", "al", "path", "p", 
										  "sources", "s", "exclude", "es", "include", "i", "minsize", "mn",
										  "maxsize", "mx", "minaspect", "ma", "orientation", "r",
										  "covertype", "t", "sequence", "seq" };
			Arguments arguments = new Arguments(args, valuedParameters);
			if (arguments.Contains("?") || arguments.Count == 0)
			{
				ShowCommandArgs();
				WaitForExit(0);
				return 0;
			}

			if (!ScriptManager.CompileIfRequired())
			{
				WaitForExit(-1);
				return -1; //Failed
			}
			ScriptManager.LoadScripts();

			int exitCode = ProcessCommandArgs(arguments);
			WaitForExit(exitCode);
			return exitCode; //Success
		}

		private static int ProcessCommandArgs(Arguments arguments)
		{
			string artist = null, album = null, path = null;
			int? minSize = null, maxSize = null;
			AllowedCoverType? coverType = null;

			float minAspect = 0;
			int sequence = 1;
			List<String> useScripts = new List<string>();
			List<String> excludeScripts = new List<string>();
			Orientation requiredOrientation = Orientation.None;

			bool warnIfNoSearch = false; //If search-like parameters are present, warn if search terms are not.
			bool fetchAllResults = false;
			string errorMessage = null;
			foreach (Parameter parameter in arguments)
			{
				//Check un-named parameters
				if (parameter.Name == null)
				{
					//For un-named parameters, use compatibility mode: 3 args,  "<artist>" "<album>" "<path to save image>"
					switch (arguments.IndexOf(parameter))
					{
						case 0:
							artist = parameter.Value;
							break;
						case 1:
							album = parameter.Value;
							break;
						case 2:
							path = parameter.Value;
							warnIfNoSearch = true;
							break;
						default:
							errorMessage = "Only the first three parameters may be un-named";
							break;
					}
				}
				else
				{
					//Check named parameters
					switch (parameter.Name.ToLower()) //Case insensitive parameter names
					{
						case "artist":
						case "ar":
							artist = parameter.Value;
							break;
						case "album":
						case "al":
							album = parameter.Value;
							break;
						case "path":
						case "p":
							path = PathFix(parameter.Value);
							warnIfNoSearch = true;
							break;
						case "sources":
						case "s":
						case "include": //Included for compatibility with GUI command line parameters. There are no defaults here, so Include and Sources have the same effect of selecting those sources.
						case "i":
							useScripts.AddRange(parameter.Value.Split(','));
							warnIfNoSearch = true;
							break;
						case "exclude":
						case "es":
							excludeScripts.AddRange(parameter.Value.Split(','));
							warnIfNoSearch = true;
							break;
						case "minsize":
						case "mn":
							try
							{
								minSize = Int32.Parse(parameter.Value);
							}
							catch (Exception e)
							{
								errorMessage = "The /minSize parameter must be a number: " + parameter.Value + "\n  " + e.Message;
							}
							warnIfNoSearch = true;
							break;
						case "maxsize":
						case "mx":
							try
							{
								maxSize = Int32.Parse(parameter.Value);
							}
							catch (Exception e)
							{
								errorMessage = "The /maxSize parameter must be a number: " + parameter.Value + "\n  " + e.Message;
							}
							warnIfNoSearch = true;
							break;
						case "minaspect":
						case "ma":
							try
							{
								minAspect = Single.Parse(parameter.Value);
							}
							catch (Exception e)
							{
								errorMessage = "The /minAspect parameter must be a number: " + parameter.Value + "\n  " + e.Message;
							}
							if (minAspect > 1 || minAspect < 0)
							{
								errorMessage = "The /minAspect parameter must be a number between 0 and 1: " + parameter.Value;
							}
							warnIfNoSearch = true;
							break;
						case "orientation":
						case "r":
							switch (parameter.Value.ToLower())
							{
								case "portrait":
								case "p":
									requiredOrientation = Orientation.Portrait;
									break;
								case "landscape":
								case "l":
									requiredOrientation = Orientation.Landscape;
									break;
								default:
									errorMessage = "The /orientation parameter must be either 'portrait' or 'landscape': '" + parameter.Value + "'";
									break;
							}
							warnIfNoSearch = true;
							break;
						case "sequence":
						case "seq":
							if (!Int32.TryParse(parameter.Value, out sequence))
							{
								errorMessage = "The /sequence (/seq) parameter must be a number greater than 0: " + parameter.Value;
							}
							if (sequence <= 0)
							{
								errorMessage = "The /sequence (/seq) parameter must be greater than 0";
							}
							warnIfNoSearch = true;
							break;
						case "covertype":
						case "t":
							coverType = default(AllowedCoverType);
							foreach (String allowedCoverType in parameter.Value.ToLower().Split(','))
							{
								switch (allowedCoverType)
								{
									case "front":
									case "f":
										coverType |= AllowedCoverType.Front;
										break;
									case "back":
									case "b":
										coverType |= AllowedCoverType.Back;
										break;
									case "inside":
									case "i":
										coverType |= AllowedCoverType.Inside;
										break;
									case "cd":
									case "c":
										coverType |= AllowedCoverType.CD;
										break;
									case "unknown":
									case "u":
										coverType |= AllowedCoverType.Unknown;
										break;
									case "any":
									case "a":
										coverType |= AllowedCoverType.Any;
										break;
									default:
										errorMessage = "Unrecognized cover type: " + parameter.Value;
										break;
								}
							}
							warnIfNoSearch = true;
							break;
						case "listsources":
						case "l":
							ListScripts();
							//No warnIfNoSearch
							break;
						case "fetchall":
						case "fa":
							fetchAllResults = true;
							break;
						default:
							errorMessage = "Unexpected command line parameter: " + parameter.Name;
							break;
					}
				}
				if (errorMessage != null)
					break; //Stop parsing args if there was an error
			}

			if (errorMessage != null) //Problem with the command args, so display the error, and the help
			{
				Console.WriteLine(errorMessage);
				Console.WriteLine();
				ShowCommandArgs();
				return -1; //Faliure
			}

			//Check if search-like parameters have been used without search terms
			if (String.IsNullOrEmpty(artist) && String.IsNullOrEmpty(album))
			{
				if(warnIfNoSearch)
				{
					Console.WriteLine("No search terms were specified. Use /album and /artist to search for album art.");
				}
				return 0; //This is successful, if not useful.
			}

			if (String.IsNullOrEmpty(path))
			{
				//Use default path if none is specified
				path = Properties.Settings.Default.DefaultPath;
			}

			//Create list of scripts to search
			List<IScript> scripts = new List<IScript>();
			if (useScripts.Count == 0 || 
				(useScripts.Count == 1 && useScripts[0].Equals("all", StringComparison.OrdinalIgnoreCase)))
			{
				//Start by adding all the scripts, in arbitrary order
				scripts.AddRange(ScriptManager.Scripts);
			}
			else
			{
				//Add specified scripts in order
				foreach (string scriptName in useScripts)
				{
					IScript script = ScriptManager.Scripts.FirstOrDefault(s => s.Name.Equals(scriptName, StringComparison.OrdinalIgnoreCase));
					if (script != null)
					{
						scripts.Add(script);
					}
				}
			}

			//Exclude any scripts to exclude
			foreach (IScript script in new List<IScript>(scripts))
			{
				if (excludeScripts.Contains(script.Name, StringComparer.OrdinalIgnoreCase))
					scripts.Remove(script);
			}

			if (scripts.Count == 0)
			{
				Console.WriteLine("No sources to search found. Use /listSources to show available sources.");
				return -1; //Faliure
			}
			
			//perform the actual search
			try
			{
				if (Search(scripts, artist, album, path, minSize, maxSize, minAspect, requiredOrientation, coverType, sequence, fetchAllResults))
				{
					return 0; //Success
				}
				else
				{
					Console.WriteLine("\nNo matching images found.");
					return 1; //Image not found
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Unexpected faliure: " + e.Message);
				return -1; //Faliure
			}
		}

		/// <summary>
		/// Hack to fix .net args processing. Removes trailing " and replaces it by \
		/// <remarks>
		/// If the command line includes, for example /path "c:\folder\", then the last two
		/// characters are interpreted as an escaped " mark. This hack fixes that.
		/// </remarks>
		/// </summary>
		private static string PathFix(string pathParam)
		{
			if (pathParam[pathParam.Length - 1] == '\"')
			{
				return pathParam.Substring(0, pathParam.Length - 1) + "\\";
			}
			return pathParam;
		}

		/// <summary>
		/// Perform the actual search, download and save of art
		/// </summary>
		private static bool Search(IEnumerable<IScript> scripts, string artist, string album, string path, int? minSize, int? maxSize, float minAspect, Orientation requiredOrientation, AllowedCoverType? coverType, int targetSequence, bool fetchAllResults)
		{
			bool foundResult = false;

			//Replace the artist and album placeholders in the path
			path = path.Replace("%artist%", MakeSafeForPath(artist))
					   .Replace("%album%", MakeSafeForPath(album));
			int sequence = 0;
			foreach (IScript script in scripts) //Try each script in turn
			{
				ScriptResults scriptResults = new ScriptResults(script);
				Console.WriteLine("Searching {0}...", script.Name);
				try
				{
					//TODO: This could be done on a separate thread so it could be cancelled after required result is found
					script.Search(artist, album, scriptResults);
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.Fail(String.Format("Script {0} threw an exception while searching: {1}", script.Name, e.Message));
					continue; //Skip this script, try the next
				}
				foreach (ScriptResult result in scriptResults.Results)
				{
					if( CheckCoverType(result, coverType) && //Quick check of reported cover type
						CheckImageDimensions(result, minSize, maxSize, minAspect, requiredOrientation, false) && //Quick check of reported dimensions
						CheckImageDimensions(result, minSize, maxSize, minAspect, requiredOrientation, true)) //Full check of downloaded image dimensions
					{
						if (++sequence == targetSequence || fetchAllResults) //Discard sequence-1 results, unless fetching all results
						{
							if (result.Save(path, sequence))
							{
								if (!fetchAllResults)
								{
									//Unless fetching all results, once one is found, stop searching.
									return true;
								}
								foundResult = true; //Note that a result was found, to indicate success.
							}
							else
							{
								--sequence; //Result was invalid, so doesn't count towards sequence.
							}
						}
					}
				}
			}
			return foundResult;
		}

		private static bool CheckCoverType(ScriptResult result, AllowedCoverType? allowedCoverTypes)
		{
			if (allowedCoverTypes.HasValue)
			{
				var coverType = MakeAllowedCoverType(result.CoverType);
				return ((coverType & allowedCoverTypes.Value) == coverType); //Cover type is one of the allowed types
			}
			return true; //allowedCoverTypes not specified, so allow any result.
		}

		private static bool CheckImageDimensions(ScriptResult result, int? minSize, int? maxSize, float minAspect, Orientation requiredOrientation, bool forceDownload)
		{
			//Valid if there is no limit specified, or the size is within the limit. Both limits must apply if both are present
			//Check size
			if (minSize.HasValue || maxSize.HasValue)
			{
				if (!CheckImageSize(minSize, maxSize, result.GetMinImageDimension(forceDownload)))
					return false;
			}
			if (requiredOrientation != Orientation.None)
			{
				if (result.GetImageOrientation(forceDownload) != requiredOrientation)
					return false;
			}
			//Check aspect ratio
			if (minAspect > 0)
			{
				if (result.GetImageAspectRatio(forceDownload) < minAspect)
					return false;
			}
			//Passes all tests
			return true;
		}

		private static bool CheckImageSize(int? minSize, int? maxSize, int size)
		{
			return	(!minSize.HasValue || size >= minSize.Value) &&
					(!maxSize.HasValue || size <= maxSize.Value);
		}
		//MakeSafeForPath and MakeAllowedCoverType copied from Common.cs
		/// <summary>
		/// Ensures that a string is safe to be part of a file path by replacing all illegal
		/// characters with underscores.
		/// </summary>
		internal static string MakeSafeForPath(string value)
		{
			if (String.IsNullOrEmpty(value))
			{
				return String.Empty;
			}

			char[] invalid = Path.GetInvalidFileNameChars();
			char[] valueChars = value.ToCharArray();

			bool valueChanged = false;
			int invalidIndex = 0;
			while ((invalidIndex = value.IndexOfAny(invalid, invalidIndex + 1)) >= 0)
			{
				valueChars[invalidIndex] = '_';
				valueChanged = true;
			}
			if (valueChanged)
			{
				return new string(valueChars);
			}
			else //Don't perform the construction of the new string if not required
			{
				return value;
			}
		}
		internal static AllowedCoverType MakeAllowedCoverType(CoverType scriptCoverType)
		{
			switch (scriptCoverType)
			{
				case CoverType.Unknown:
					return AllowedCoverType.Unknown;
				case CoverType.Front:
					return AllowedCoverType.Front;
				case CoverType.Back:
					return AllowedCoverType.Back;
				case CoverType.Inside:
					return AllowedCoverType.Inside;
				case CoverType.CD:
					return AllowedCoverType.CD;
			}
			return AllowedCoverType.Unknown;
		}

		private static void ShowCommandArgs()
		{
			string commandArgsHelp;
			using (StreamReader textReader = new StreamReader(typeof(Program).Assembly.GetManifestResourceStream("AlbumArtDownloader.CommandArgsHelp.txt")))
				commandArgsHelp = textReader.ReadToEnd();

			Console.WriteLine(commandArgsHelp, Path.GetFileName(Assembly.GetEntryAssembly().Location));
		}

		private static void ListScripts()
		{
			//Find column widths
			int nameWidth = 0;
			int versionWidth = 0;
			int authorsWidth = 0;
			foreach (IScript script in ScriptManager.Scripts)
			{
				nameWidth = Math.Max(nameWidth, script.Name.Length);
				versionWidth = Math.Max(versionWidth, script.Version.Length);
				authorsWidth = Math.Max(authorsWidth, script.Author.Length);
			}
			//Restrict authorsWidth to remaining available size
			authorsWidth = Math.Min(authorsWidth, Console.BufferWidth - nameWidth - versionWidth - 9); //additional space for the spaces separating the columns
			if (authorsWidth < 4) //Minimum sensible author length, less than this should be omitted
				authorsWidth = 0;
			
			WriteFixedLength("Name", nameWidth);
			Console.Write("    ");
			WriteFixedLength("Ver", versionWidth);
			Console.Write("    ");
			WriteFixedLength("Author", authorsWidth);
			Console.WriteLine();
			Console.WriteLine(new String('═', nameWidth + versionWidth + authorsWidth + (authorsWidth > 0 ? 8 : 4)));

			foreach (IScript script in ScriptManager.Scripts)
			{
				WriteFixedLength(script.Name, nameWidth);
				Console.Write("    ");
				WriteFixedLength(script.Version, versionWidth);
				Console.Write("    ");
				WriteFixedLength(script.Author, authorsWidth);
				Console.WriteLine();
			}
			Console.WriteLine();
		}

		private static void WriteFixedLength(string value, int length)
		{
			if (value.Length > length) //Trim if too long
			{
				value = value.Substring(0, length);
			}
			Console.Write(value);
			Console.Write(new String(' ', length - value.Length)); //Pad if too short
		}

		[System.Diagnostics.Conditional("DEBUG")]
		private static void WaitForExit(int exitCode)
		{
			Console.WriteLine("\nDEBUG: Press Enter to Exit (exit code: {0})", exitCode);
			Console.ReadLine();
		}
	}
}
