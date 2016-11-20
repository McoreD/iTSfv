using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Net;
using System.Web.Script.Serialization;

namespace AlbumArtDownloader
{
	class GenerateiTunesCountryCodes
	{
		#region Codes
		private readonly static string[] Codes = new[] {
"Algeria", "DZ",
"Angola", "AO",
"Anguilla", "AI",
"Antigua and Barbuda", "AG",
"Argentina", "AR",
"Armenia", "AM",
"Australia", "AU",
"Azerbaijan", "AZ",
"Bahamas", "BS",
"Bahrain", "BH",
"Barbados", "BB",
"Belarus", "BY",
"België/Belgique", "BE",
"Belize", "BZ",
"Bermuda", "BM",
"Bolivia", "BO",
"Botswana", "BW",
"Brazil", "BR",
"British Virgin Islands", "VG",
"Brunei Darussalam", "BN",
"Canada", "CA",
"Cayman Islands", "KY",
"Česká republika (Czech republic)", "CZ",
"Chile", "CL",
"Colombia", "CO",
"Costa Rica", "CR",
"Cyprus", "CY",
"Denmark", "DK",
"Deutschland (Germany)", "DE",
"Dominica", "DM",
"Ecuador", "EC",
"Eesti", "EE",
"Egypt", "EG",
"El Salvador", "SV",
"España (Spain)", "ES",
"France", "FR",
"Ghana", "GH",
"Greece", "GR",
"Grenada", "GD",
"Guatemala", "GT",
"Guyana", "GY",
"Honduras", "HN",
"Hong Kong", "HK",
"Hrvatska (Croatia)", "HR",
"Iceland", "IS",
"India", "IN",
"Indonesia", "ID",
"Ireland", "IE",
"Israel", "IL",
"Italia", "IT",
"Jamaica", "JM",
"Jordan", "JO",
"Kazakhstan", "KZ",
"Kenya", "KE",
"Kuwait", "KW",
"Latvija (Latvia)", "LV",
"Lebanon", "LB",
"Lietuva (Lithuania)", "LT",
"Luxembourg", "LU",
"Macau", "MO",
"Macedonia", "MK",
"Madagascar", "MG",
"Magyarország (Hungary)", "HU",
"Malaysia", "MY",
"Mali", "ML",
"Malta", "MT",
"Mauritius", "MU",
"México", "MX",
"Moldova", "MD",
"Montserrat", "MS",
"Nederland (Netherlands)", "NL",
"New Zealand", "NZ",
"Nicaragua", "NI",
"Niger", "NE",
"Nigeria", "NG",
"Norge (Norway)", "NO",
"Oman", "OM",
"Österreich (Austria)", "AT",
"Pakistan", "PK",
"Panamá", "PA",
"Paraguay", "PY",
"Perú", "PE",
"Philippines", "PH",
"Polska (Poland)", "PL",
"Portugal", "PT",
"Qatar", "QA",
"República Dominica", "DM",
"Romania", "RO",
"Saudi Arabia", "SA",
"Schweiz/Suisse (Switzerland)", "CH",
"Sénégal", "SN",
"Singapore", "SG",
"Slovakia", "SK",
"Slovenia", "SI",
"South Africa", "ZA",
"Sri Lanka", "LK",
"St. Kitts & Nevis", "KN",
"St. Lucia", "LC",
"St. Vincent & The Grenadines", "VC",
"Suomi (Finland)", "FI",
"Suriname", "SR",
"Sverige (Sweden)", "SE",
"Tanzania", "TZ",
"Thailand", "TH",
"Trinidad and Tobago", "TT",
"Tunisie", "TN",
"Türkiye (Turkey)", "TR",
"Turks & Caicos", "TC",
"Uganda", "UG",
"UK", "GB",
"United Arab Emirates", "AE",
"Uruguay", "UY",
"Uzbekistan", "UZ",
"Venezuela", "VE",
"Vietnam", "VN",
"Yemen", "YE",
"България (Bulgaria)", "BG",
"Россия (Russia)", "RU",
"China", "CN",
"Taiwan", "TW",
"Japan", "JP",
"South Korea", "KR"};
		#endregion
		
		static void Main(string[] args)
		{
			var scriptsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "scripts\\iTunes Country Overrides\\");

			Console.WriteLine("Album Art Downloader XUI iTunes Country Code Script Generator");
			Console.WriteLine("(this is an internal development tool)");
			Console.WriteLine();
			Console.WriteLine("Writing scripts to: " + scriptsPath);

			foreach (var file in Directory.GetFiles(scriptsPath))
			{
				File.Delete(file);
			}

			StringBuilder wikiText = new StringBuilder();
			var json = new JavaScriptSerializer();

			for (int i = 0; i < Codes.Length; i += 2)
			{
				string name = Codes[i];
				string code = Codes[i + 1];

				Console.WriteLine(name);

				// Check to see if there is anything at all
				var request = WebRequest.Create("http://itunes.apple.com/search?entity=album&term=_&limit=1&country=" + code);
				using(var response = request.GetResponse())
				using(var reader = new StreamReader(response.GetResponseStream()))
				{
					var content = reader.ReadToEnd();
					var result = json.Deserialize<Results>(content);
					if (result.resultCount == 0)
					{
						continue;
					}
				}

				string scriptContent = "class iTunes" + code.ToUpper() + "(iTunes):\n\toverride protected CountryName as string:\n\t\tget: return \"" + name + "\"\n\toverride protected CountryCode as string:\n\t\tget: return \"" + code + "\"\n";
				string filename = "itunes-" + code + ".boo";
				File.WriteAllText(scriptsPath + filename, scriptContent);
				wikiText.AppendLine("*[http://album-art.sourceforge.net/scripts/itunes/" + filename + " " + name + "]");
			}

			File.WriteAllText(scriptsPath + "wiki.txt", wikiText.ToString());

			Console.WriteLine("Done.");
		}

		private struct Results
		{
#pragma warning disable 0649 // Field is assigned by JavaScriptSerializer
			public int resultCount;
#pragma warning restore 0649
		}
	}
}
