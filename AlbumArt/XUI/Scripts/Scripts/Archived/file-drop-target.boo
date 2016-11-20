# refs: PresentationFramework PresentationCore

import System
import System.Windows
import System.Windows.Media.Animation
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts

class FileDropTarget(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "File Drop Target"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		try:
			// Create a suitable STA thread for the window
			thread = System.Threading.Thread(WindowShower);
			thread.SetApartmentState(System.Threading.ApartmentState.STA);
			thread.Start(results);
		
			// Wait for that thread to complete
			thread.Join();
		except e as System.Threading.ThreadAbortException:
			// Abort the child thread too
			thread.Abort();

	def WindowShower(results):
		try:
			// Create the drop target window
			dropWindow = DropWindow();
			dropWindow.FileDropped += def(filename):
				results.Add(filename, System.IO.Path.GetFileNameWithoutExtension(filename), null);
		
			// Launch the window
			dropWindow.ShowDialog();
		except e as System.Threading.ThreadAbortException:
			dropWindow.Close();

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return null;

class DropWindow(Window):
	mAnimation as ThicknessAnimation
	
	def constructor():
		Title = "Album Art Downloader Drop Target";
		Content = Controls.TextBlock(TextWrapping:TextWrapping.Wrap, Text:"Drag and drop image files here to add them to the results. Close this window when done.");
		WindowStartupLocation = WindowStartupLocation.CenterScreen
		Width = 200;
		Height = 200;
		Topmost = true;
		WindowStyle = WindowStyle.ToolWindow
		AllowDrop = true;

		BorderBrush = System.Windows.Media.Brushes.Blue;
		mAnimation = ThicknessAnimation()
		mAnimation.From = Thickness(0)
		mAnimation.To = Thickness(2)
		mAnimation.Duration = Duration(TimeSpan.FromMilliseconds(50));
		mAnimation.AutoReverse = true

	callable FileDropEvent(filename as string)

	event FileDropped as FileDropEvent

	protected override def OnDragEnter(e as DragEventArgs):
		e.Effects = DragDropEffects.None;
		e.Handled = true;

	protected override def OnDragOver(e as DragEventArgs):
		e.Effects = DragDropEffects.None;
		e.Handled = true;

		// Allow drop of image files
		if e.Data.GetDataPresent(DataFormats.FileDrop):
			e.Effects = DragDropEffects.Copy;
	
	protected override def OnDrop(e as DragEventArgs):
		if e.Data.GetDataPresent(DataFormats.FileDrop):
			files = e.Data.GetData(DataFormats.FileDrop) as (string);
			if files != null:
				BeginAnimation(BorderThicknessProperty, mAnimation, HandoffBehavior.SnapshotAndReplace);
				for file in files:
					FileDropped(file)