using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlbumArtDownloader;
using AlbumArtDownloader.Scripts;
using System.IO;
using System.Threading;

namespace ScriptTester
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("Album Art Downloader XUI Script Tester");
            Console.WriteLine("(this is an internal development tool)");
            Console.WriteLine();

            if (!ScriptManager.CompileIfRequired())
            {
                WaitForExit(-1);
                return -1; //Failed
            }
            ScriptManager.LoadScripts();

            //Create a lookup of script by name
            var scripts = ScriptManager.Scripts.ToDictionary(script => script.Name);

            WritePoint status = new WritePoint();
            Console.WriteLine();
            Console.WriteLine();

            //Read through test list
            using (var testList = File.OpenText(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "TestList.txt")))
            {
                while (!testList.EndOfStream)
                {
                    string line = testList.ReadLine();
					if (line == "END")
						break; //Premature end requested

                    if (!line.StartsWith(";")) //; Used for comment marking
                    {
                        string[] fields = line.Split('\t');

                        IScript script;
                        if (scripts.TryGetValue(fields[0], out script))
                        {
                            using (status.WriteAt())
                            {
                                Console.Write(new String(' ', Console.WindowWidth - status.X));
                                Console.SetCursorPosition(status.X, status.Y);
                                Console.Write("Testing: " + script.Name);
                            }

                            TestScript(script, fields[1], fields[2]);
                        }
                        else
                        {
                            Console.WriteLine("WARNING: Could not find script: " + fields[0]);
                        }
                    }
                }
            }

            using (status.WriteAt())
            {
                Console.Write(new String(' ', Console.WindowWidth - status.X));
            }

            Console.WriteLine();
            Console.WriteLine("All tests complete");

            WaitForExit(0);
            return 0;
        }

        private static void TestScript(IScript script, string artist, string album)
        {
			ScriptResults scriptResults = new ScriptResults();
			
            Thread searchThread = new Thread(new ThreadStart(delegate
			{
				try
				{
					script.Search(artist, album, scriptResults);
				}
				catch (ThreadAbortException) 
				{
					throw;
				}
				catch (Exception e)
				{
					Console.WriteLine("FAILED: Script {0} threw an exception while searching: {1}", script.Name, e.Message);
					return; //Skip this script, try the next
				}
			}));
			searchThread.Start();
			searchThread.Join();
			if (!scriptResults.ResultFound)
			{
				Console.WriteLine("FAILED: Script {0} produced no results for {1} / {2}", script.Name, artist, album);
			}
        }

        /// <summary>
        /// Stub script results, simply records whether any result is returned
        /// </summary>
        private class ScriptResults : IScriptResults
        {
            public bool ResultFound
            {
                get;
                private set;
            }

			private void FoundResult()
			{
				ResultFound = true;
				Thread.CurrentThread.Abort(); //Only interested in whether there are *any* results at all, so stop looking now.
			}

            public void SetCountEstimate(int count) {}
            public int EstimatedCount { get; set; }

            #region Add overloads
            public void AddThumb(string thumbnailUri, string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
            {
                FoundResult();
            }

            public void AddThumb(System.IO.Stream thumbnailStream, string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
            {
                FoundResult();
            }

            public void AddThumb(System.Drawing.Image thumbnailImage, string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
            {
                FoundResult();
            }

            public void Add(object thumbnail, string name, object fullSizeImageCallback, CoverType coverType)
            {
                FoundResult();
            }

            public void Add(object thumbnail, string name, object fullSizeImageCallback)
            {
                FoundResult();
            }

            public void Add(object thumbnail, string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
            {
                FoundResult();
            }

            public void Add(object thumbnail, string name, string infoUri, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
            {
                FoundResult();
            }

            public void Add(object thumbnail, string name, string infoUri, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback, CoverType coverType)
            {
                FoundResult();
            }
            #endregion
        }


        [System.Diagnostics.Conditional("DEBUG")]
	    private static void WaitForExit(int exitCode)
	    {
		    Console.WriteLine("\nDEBUG: Press Enter to Exit (exit code: {0})", exitCode);
		    Console.ReadLine();
	    }
    }
}
