using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Resources;

namespace AlbumArtDownloader
{
	public partial class CommandArgsHelp : System.Windows.Window
	{
		public CommandArgsHelp()
		{
			InitializeComponent();
		}

		public void ShowDialog(string errorMessage)
		{
			string commandArgsHelp;
			StreamResourceInfo commandArgsHelpResource = Application.GetResourceStream(new Uri("/CommandArgsHelp.txt", UriKind.Relative));
			using(StreamReader reader = new StreamReader(commandArgsHelpResource.Stream))
			{
				commandArgsHelp = reader.ReadToEnd();
			}
			mTextDisplay.Text = String.Format(commandArgsHelp,
				Environment.CommandLine,
				Assembly.GetEntryAssembly().GetName().Version, //Version number
				String.IsNullOrEmpty(errorMessage) ? String.Empty : "\n**** " + errorMessage + " ****\n", //Message to display
				Path.GetFileName(Assembly.GetEntryAssembly().Location)); //Name of the .exe

			ShowDialog();
		}
	}
}