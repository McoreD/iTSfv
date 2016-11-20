using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace AlbumArtDownloader
{
	public partial class OverwriteExistingWarning : Window, INotifyPropertyChanged
	{
		public static string Show(string filename, SaveFileDialog saveAsDialog, Window owner)
		{
			var dialog = new OverwriteExistingWarning
			{
				Filename = filename,
				SaveAsDialog = saveAsDialog,
				Owner = owner
			};
			dialog.InitializeComponent();
			dialog.LoadSettings();
			dialog.mDefaultButton.Focus();

			ThreadPool.QueueUserWorkItem(dialog.GetExistingFileDetails);

			dialog.StartFilenameSuggestionWorker(); // Do this after LoadSettings, so the mSuggestedFilenamePattern.PathPattern is already present

			if (dialog.ShowDialog().GetValueOrDefault(false))
			{
				return dialog.Filename;
			}

			return null;
		}

		private Thread mFilenameSuggestionWorker;
		private AutoResetEvent mFilenameSuggestionTrigger;
		
		private void StartFilenameSuggestionWorker()
		{
			System.Diagnostics.Debug.Assert(mFilenameSuggestionWorker == null);

			mFilenameSuggestionWorker = new Thread(FilenameSuggestionWorker) { Name = "FilenameSuggestionWorker" };
			mFilenameSuggestionTrigger = new AutoResetEvent(true); // Start signalled to generate a first suggestion

			mFilenameSuggestionWorker.Start();
		}

		private void FilenameSuggestionWorker()
		{
			var numericPatternPartParser = new Regex(@"%(?<n>n+)(?<d>\d*)%");
			do
			{
				string originalFilename = null;
				string suggestionPattern = null;
				int startNumber = 1;
				bool hasNumericPart = false;

				// Wait to be signalled that the suggestion pattern has changed
				mFilenameSuggestionTrigger.WaitOne();
				Dispatcher.Invoke(DispatcherPriority.Send, new ThreadStart(delegate
				{
					// Get the current path pattern and reset the trigger. If it changes again, the trigger will be set again so a new suggestion is generated.
					// As this takes place in a single dispatched block, the pattern can't change between being read and the trigger being reset (as pattern changing is also dispatched).
					originalFilename = Filename;
					suggestionPattern = mSuggestedFilenamePattern.PathPattern;
					mFilenameSuggestionTrigger.Reset();
				}));

				suggestionPattern = Common.MakeSafeForPath(suggestionPattern);

				// Substitute the %filename% and %extension% parameters with {0} and {2} for string format.
				suggestionPattern = suggestionPattern.Replace("%filename%", "{0}").Replace("%extension%", "{2}");
				
				// Parse the numeric part of the suggestion, and replace with string formatters
				suggestionPattern = numericPatternPartParser.Replace(suggestionPattern, new MatchEvaluator(delegate(Match match)
				{
					hasNumericPart = true;

					if (match.Groups["d"].Length > 0)
					{
						Int32.TryParse(match.Groups["d"].Value, out startNumber);
					}
					return "{1:" + new String('0', match.Groups["n"].Length) + "}";
				}));

				string extension = Path.GetExtension(originalFilename);
				string originalFilenameNoExtension = originalFilename.Substring(0, originalFilename.Length - extension.Length);
				if (extension.Length > 1)
				{
					extension = extension.Substring(1); // Remove the .
				}
				
				// Find the next filename that can match
				int i = startNumber;
				string suggestedFilename;
				do
				{
					suggestedFilename = String.Format(System.Globalization.CultureInfo.CurrentUICulture, suggestionPattern, originalFilenameNoExtension, i++, extension);
				} while (hasNumericPart && File.Exists(suggestedFilename)); //If there's no numeric part, we can't vary to suggest other filenames anyway, so don't look further.

				// Found a filename, so present it
				Dispatcher.Invoke(DispatcherPriority.DataBind, new ThreadStart(delegate
				{
					SuggestedFilename = suggestedFilename;
					NotifyPropertyChanged("SuggestedFilename");
				}));
			} while (true);
		}

		private void mSuggestedFilenamePattern_PathPatternChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (mFilenameSuggestionTrigger != null)
			{
				// Trigger the suggestion generator to search for new suggestions.
				mFilenameSuggestionTrigger.Set();
			}
		}

		public string Filename { get; private set; }
		public long FileSize { get; private set; }
		public int FileWidth { get; private set; }
		public int FileHeight { get; private set; }

		public string SuggestedFilename { get; private set; }
		public SaveFileDialog SaveAsDialog { get; private set; }

		private void GetExistingFileDetails(object state)
		{
			//Attempt to get the image dimesions
			try
			{
				using (var fileStream = File.OpenRead(Filename))
				{
					var bitmapDecoder = BitmapDecoder.Create(fileStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
					FileWidth = bitmapDecoder.Frames[0].PixelWidth;
					FileHeight = bitmapDecoder.Frames[0].PixelHeight;
					FileSize = fileStream.Length;
				}

				NotifyPropertyChanged("FileWidth");
				NotifyPropertyChanged("FileHeight");
				NotifyPropertyChanged("FileSize");
			}
			catch (Exception)
			{
				//Ignore exceptions when reading the details, they aren't important
				FileSize = FileWidth = FileHeight = 0;
			}
		}

		private void Overwrite_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void UseSuggestion_Click(object sender, RoutedEventArgs e)
		{
			if (String.IsNullOrEmpty(SuggestedFilename))
			{
				//Wait for a suggested filename to present itself
				this.Cursor = Cursors.Wait;
				this.IsEnabled = false;
				PropertyChanged += WaitForSuggestedFilename;
				return;
			}

			Filename = SuggestedFilename;

			if (File.Exists(Filename))
			{
				// Reshow the warning dialog for the new filename, which also needs overwrite confirmation
				Filename = OverwriteExistingWarning.Show(Filename, SaveAsDialog, Owner);
			}

			DialogResult = true;
		}

		private void WaitForSuggestedFilename(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "SuggestedFilename")
			{
				PropertyChanged -= WaitForSuggestedFilename;
				UseSuggestion_Click(null, null);
			}
		}

		private void SaveAs_Click(object sender, RoutedEventArgs e)
		{
			SaveAsDialog.FileName = Filename;
			
			if (SaveAsDialog.ShowDialog(Owner).GetValueOrDefault(false))
			{
				Filename = SaveAsDialog.FileName;
				if (!SaveAsDialog.CheckFileExists && File.Exists(Filename))
				{
					// Reshow the warning dialog for the new filename, which also needs overwrite confirmation
					Filename = OverwriteExistingWarning.Show(Filename, SaveAsDialog, Owner);
				}
				
				DialogResult = true;
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			if (mFilenameSuggestionWorker != null)
			{
				mFilenameSuggestionWorker.Abort();
				mFilenameSuggestionWorker = null;
			}

			if (DialogResult.GetValueOrDefault(false))
			{
				SaveSettings();
			}
			base.OnClosed(e);
		}

		private void LoadSettings()
		{
			mSuggestedFilenamePattern.History.Clear();
			foreach (string historyItem in Properties.Settings.Default.OverwriteSuggestionHistory)
			{
				mSuggestedFilenamePattern.History.Add(historyItem);
			}
			mSuggestedFilenamePattern.PathPattern = Properties.Settings.Default.OverwriteSuggestion;
		}

		private void SaveSettings()
		{
			mSuggestedFilenamePattern.AddPatternToHistory();
			Properties.Settings.Default.OverwriteSuggestionHistory.Clear();
			foreach (string historyItem in mSuggestedFilenamePattern.History)
			{
				Properties.Settings.Default.OverwriteSuggestionHistory.Add(historyItem);
			}
			Properties.Settings.Default.OverwriteSuggestion = mSuggestedFilenamePattern.PathPattern;
		}

		#region Property Notification
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler temp = PropertyChanged;
			if (temp != null)
			{
				temp(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}
}
