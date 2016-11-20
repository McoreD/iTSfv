using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;

namespace AlbumArtDownloader
{
	public partial class Splashscreen : System.Windows.Window, INotifyPropertyChanged
	{

		public static class Commands
		{
			public static RoutedUICommand Retry = new RoutedUICommand("Retry", "Retry", typeof(Commands));
		}

		/// <summary>
		/// Shows the splashscreen modally, if it is required to compile scripts.
		/// </summary>
		/// <returns>True to continue with the application, false to exit</returns>
		public static bool ShowIfRequired()
		{
			//Check whether boo script cache is up to date or not
			try
			{
				FileInfo cacheFileInfo = new FileInfo(App.BooScriptsCacheFile);
				if (cacheFileInfo.Exists)
				{
					//There is a cache file, so check if it is up to date or not
					bool rebuildCache = false;
							
					foreach (string scriptsPath in App.ScriptsPaths)
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
						return true; //No need to rebuild the cache, so just continue execution.
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.Fail("Exception while checking script file last update times:\n\n" + e.Message);
				//Ignore the exception, and try to recompile. If that doesn't fix it, then errors will be reported by the recompilation process.
			}
			
			return new Splashscreen().ShowDialog().GetValueOrDefault(true);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private BackgroundWorker mBackgroundWorker;

		public Splashscreen()
		{
			InitializeComponent();
			mBackgroundWorker = new BackgroundWorker();
			mBackgroundWorker.WorkerReportsProgress = true;
			mBackgroundWorker.DoWork += new DoWorkEventHandler(PerformScriptCompilation);
			mBackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(OnScriptCompilationProgressChanged);
			mBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnScriptCompilationComplete);

			CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, new ExecutedRoutedEventHandler(CloseExec)));
			CommandBinding retryBinding = new CommandBinding(Commands.Retry, new ExecutedRoutedEventHandler(RetryExec));
			retryBinding.CanExecute += new CanExecuteRoutedEventHandler(OnCanRetry);
			CommandBindings.Add(retryBinding);

			mDetails.SizeChanged += new SizeChangedEventHandler(OnDetailsSizeChanged);

			Loaded += new RoutedEventHandler(OnLoaded);
		}

		private void OnDetailsSizeChanged(object sender, SizeChangedEventArgs e)
		{
			mDetails.ScrollToEnd();
		}

		private void OnCanRetry(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !mBackgroundWorker.IsBusy;
		}
		private void RetryExec(object sender, ExecutedRoutedEventArgs e)
		{
			mDetails.Clear();
			CurrentTask = String.Empty;
			ProgressPercentage = 0;

			mBackgroundWorker.RunWorkerAsync();
		}

		private void CloseExec(object sender, ExecutedRoutedEventArgs e)
		{
			DialogResult = false;
		}

		#region Background script compilation
		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			mBackgroundWorker.RunWorkerAsync();
		}

		/// <summary>
		/// The asynchronous scriptcompilation thread, called by the background worker
		/// </summary>
		private void PerformScriptCompilation(object sender, DoWorkEventArgs e)
		{
			try
			{
				FileInfo outputFile = new FileInfo(App.BooScriptsCacheFile);
				if(!outputFile.Directory.Exists)
					outputFile.Directory.Create(); //Enusre that the output file can be created.

				mBackgroundWorker.ReportProgress(0, "Searching for scripts...");

				Dictionary<string, string> scripts = new Dictionary<string,string>();
				foreach (string scriptsPath in App.ScriptsPaths)
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
						mBackgroundWorker.ReportProgress(-1, String.Format("Skipping unreadable file: \"{0}\"\n  {1}", scriptFile, fileReadingException.Message));
					}
				}

				mBackgroundWorker.ReportProgress(-1, string.Format("Found {0} files: [{1}]...", readableFiles.Count, FormatFileListForDisplay(readableFiles)));
				mBackgroundWorker.ReportProgress(-1, string.Format("Loading references: [{0}]...", string.Join(", ", references.ToArray())));

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
				mBackgroundWorker.ReportProgress(0, "Compiling scripts...");

				//Report compilation progress
				mTotalCompilerSteps = compiler.Parameters.Pipeline.Count;
				mCurrentCompilerStep = 0;
				compiler.Parameters.Pipeline.BeforeStep += new CompilerStepEventHandler(OnBeforeCompilerStep);

				CompilerContext compilerContext = compiler.Run();

				foreach (CompilerWarning warning in compilerContext.Warnings)
				{
					ReportCompilerIssue("warning", warning.LexicalInfo, warning.Code, warning.Message);
					//TODO: Not auto-close if warnings are present?
				}
				foreach (CompilerError error in compilerContext.Errors)
				{
					ReportCompilerIssue("error", error.LexicalInfo, error.Code, error.Message);
					e.Cancel = true;
				}
				if (!e.Cancel)
					mBackgroundWorker.ReportProgress(-1, "Complete");
			}
			catch (Exception exception)
			{
				mBackgroundWorker.ReportProgress(-1, string.Format("Error: {0}", exception.Message));
				e.Cancel = true;
			}
		}

		private void ReportCompilerIssue(string severity, Boo.Lang.Compiler.Ast.LexicalInfo lexicalInfo, string code, string message)
		{
			mBackgroundWorker.ReportProgress(-1, String.Format("{0}({1},{2}): {3} {4}: {5}", Path.GetFileName(lexicalInfo.FileName), lexicalInfo.Line, lexicalInfo.Column, severity, code, message));
		}

		private int mTotalCompilerSteps;
		private int mCurrentCompilerStep;
		private void OnBeforeCompilerStep(object sender, CompilerStepEventArgs args)
		{
			mBackgroundWorker.ReportProgress(++mCurrentCompilerStep * 100 / mTotalCompilerSteps, null);
		}

		private string FormatFileListForDisplay(IEnumerable<string> files)
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

		private void OnScriptCompilationProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			string report = (string)e.UserState;
			if (e.ProgressPercentage >= 0)
			{
				ProgressPercentage = e.ProgressPercentage;
				if(report != null)
					CurrentTask = report;
			}
			if (report != null)
				DetailPrint(report);
		}
		private void OnScriptCompilationComplete(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
				CurrentTask = "Script Compilation Failed!";
				ProgressPercentage = 100;
				ShowDetails = true;
				ShowInTaskbar = true;
				CommandManager.InvalidateRequerySuggested(); //Enables the retry button, as its CanExecute will now return true.
				Activate();
			}
			else
			{
				DialogResult = true;
			}
		}
		#endregion

		private void DetailPrint(string text)
		{
			mDetails.AppendText(text + "\n");
			mDetails.ScrollToEnd();
		}

		private string mCurrentTask;
		public string CurrentTask
		{
			get { return mCurrentTask; }
			private set
			{
				if (mCurrentTask != value)
				{
					mCurrentTask = value;
					NotifyPropertyChanged("CurrentTask");
				}
			}
		}

		private int mProgressPercentage;
		public int ProgressPercentage
		{
			get { return mProgressPercentage; }
			private set
			{
				if (mProgressPercentage != value)
				{
					mProgressPercentage = value;
					NotifyPropertyChanged("ProgressPercentage");
				}
			}
		}

		private bool mShowDetails;
		public bool ShowDetails
		{
			get
			{
				return mShowDetails;
			}
			set
			{
				if (value != mShowDetails)
				{
					mShowDetails = value;
					NotifyPropertyChanged("ShowDetails");
				}
			}
		}

		private void NotifyPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler temp = PropertyChanged;
			if (temp != null)
			{
				temp(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}