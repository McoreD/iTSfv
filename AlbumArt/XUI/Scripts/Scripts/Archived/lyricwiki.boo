import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class LyricWiki(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "Lyric Wiki"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Lyrics"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://lyrics.wikia.com/index.php?search=" + EncodeUrl("\"${artist}\" \"${album}\""));
		
		matches = Regex("<li><a href=\"(?<url>[^\"]+)\" class=\"ResultLink\"", RegexOptions.IgnoreCase | RegexOptions.Singleline).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			// Retrive the page
			url = match.Groups["url"].Value;
			lyricPage as string = GetPage(url);

			lyricMatch = Regex("<div class='lyricbox'.+?</div>(\\s*<p>)?(?<lyrics>.+?)<!--").Match(lyricPage)

			if lyricMatch.Success:
				titleMatch = Regex("wgTitle=\"(?<title>[^\"]+)\"").Match(lyricPage)
				title = titleMatch.Groups["title"].Value;
				lyrics = System.Web.HttpUtility.HtmlDecode(lyricMatch.Groups["lyrics"].Value.Replace("<br />", "\n"))
				
				bitmap = Bitmap(400, 600);
				g = Graphics.FromImage(bitmap);
				g.Clear(SystemColors.Window);
				g.DrawString(lyrics, SystemFonts.DialogFont, SystemBrushes.WindowText, PointF.Empty);
				g.Dispose();

				results.Add(ConvertImageToStream(bitmap), title, url, -1, -1, null, CoverType.Front);

				bitmap.Dispose();


	def ConvertImageToStream(image):
	
		stream = System.IO.MemoryStream()
		image.Save(stream, ImageFormat.Png)
		
		stream.Seek(0, SeekOrigin.Begin)		
		return stream

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;

