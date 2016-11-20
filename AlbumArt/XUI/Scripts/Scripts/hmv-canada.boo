import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class HMVCanada(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "HMV Canada"
	Version as string:
		get: return "0.3"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Canadian"

	def Search(artist as string, album as string, results as IScriptResults):
		query = EncodeUrl("${artist} ${album}");
		resultsInfoJson = GetPage("https://hmv.ca/Search/LoadMore?filter=CD&query=${query}&sort=Bestselling&page=1&limit=20&isFrenchLocale=False&type=1");

		json = JavaScriptSerializer();
		
		resultsInfo = json.Deserialize[of Result](resultsInfoJson);
		
		results.EstimatedCount = resultsInfo.Count;
		
		for product in resultsInfo.Products:
			if not product.SmallImageUrl.EndsWith("cd_noimage_small.jpg"):
				url = "http://hmv.ca/en/Search/Details?sku=" + product.Sku;
				thumbnail = "http://hmv.ca" + product.SmallImageUrl;
				image = "http://hmv.ca" + product.LargeImageUrl;
				title = product.Artist + " - " + product.Name;
				results.Add(thumbnail, title, url, -1, -1, image, CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter

	class Result:
		public Count as int;
		public Products as (Product);

	class Product:
		public Artist as string;
		public Name as string;
		public LargeImageUrl as string;
		public SmallImageUrl as string;
		public Sku as string;
