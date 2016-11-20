namespace CoverSources
import System
import System.Text
import System.Text.RegularExpressions
import util

class ArtistsTrivialbeing:
	static SourceName as string:
		get: return "Artists.Trivialbeing"
	static SourceCreator as string:
		get: return "Alex Vallat"
	static SourceVersion as decimal:
		get: return 0.1
	static def GetThumbs(coverart,artist,album):
		coverart.AddThumb(String.Format("http://artists.trivialbeing.org/?a={0}&outputmode=img", artist), artist, -1, -1, String.Format("http://artists.trivialbeing.org/?a={0}&outputmode=img&imgs=large", artist))
	static def GetResult(param):
		return param
