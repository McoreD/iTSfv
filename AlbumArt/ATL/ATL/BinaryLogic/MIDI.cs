/****************************************************************************
Software: Midi Class
Version:  1.5
Date:     2005/04/25
Author:   Valentin Schmidt
Contact:  fluxus@freenet.de
License:  Freeware

You may use and modify this software as you wish.

Translated to C# by Zeugma 440

Last Changes: - some fixes by Michael Mlivoncic (MM):
							  + PrCh added as shortened form (repetition)
				+ exception-handling for PHP 5: raise exception on corrupt MIDI-files
				+ download now sends gzip-Encoded (if supported  by browser)
				+ parser correctly reads field-length > 127 (several event types)
				+ fixed problem with fopen ("rb")
				+ PitchBend: correct values (writing back negative nums lead to corrupt files)
				+ parser now accepts unknown meta-events

****************************************************************************/

/*
==== SOME NOTES CONCERNING LENGTH CALCULATION - Z440 ====

- The current method to calculate a MIDI file's length does not takes tempo changes into account -- it
only computes total time and uses the value of the last tempo tag of the song to calculate the actual length.
==> Improvement : dynamic length calculation _during parsing_, when detecting a tempo change, or when reaching
the eof

- The calculated length is not "intelligent", i.e. the whole MIDI file is taken as musical material, even if there
is only one note followed by three minutes of TimeSig's and tempo changes. It may be a choice of policy, but
it appears that Winamp uses "intelligent" parsing, which ends the song whith the last note event. That may be
interesting as well...
*/

using System;
using System.IO;
using System.Collections;
using ATL.Logging;

namespace ATL.AudioReaders.BinaryLogic
{

	class Midi : AudioDataReader
	{

		//Private properties
		private ArrayList tracks;    //array of tracks, where each track is array of message strings
		private int timebase;        //timebase = ticks per frame (quarter note)
		private int tempo;           //tempo as integer (0 for unknown)
		private int tempoMsgNum;     //position of tempo event in track 0
		private int type;
    
		private String[] instrumentList = new String[128] { "Piano",
															  "Bright Piano",
															  "Electric Grand",
															  "Honky Tonk Piano",
															  "Electric Piano 1",
															  "Electric Piano 2",
															  "Harpsichord",
															  "Clavinet",
															  "Celesta",
															  "Glockenspiel",
															  "Music Box",
															  "Vibraphone",
															  "Marimba",
															  "Xylophone",
															  "Tubular Bell",
															  "Dulcimer",
															  "Hammond Organ",
															  "Perc Organ",
															  "Rock Organ",
															  "Church Organ",
															  "Reed Organ",
															  "Accordion",
															  "Harmonica",
															  "Tango Accordion",
															  "Nylon Str Guitar",
															  "Steel String Guitar",
															  "Jazz Electric Gtr",
															  "Clean Guitar",
															  "Muted Guitar",
															  "Overdrive Guitar",
															  "Distortion Guitar",
															  "Guitar Harmonics",
															  "Acoustic Bass",
															  "Fingered Bass",
															  "Picked Bass",
															  "Fretless Bass",
															  "Slap Bass 1",
															  "Slap Bass 2",
															  "Syn Bass 1",
															  "Syn Bass 2",
															  "Violin",
															  "Viola",
															  "Cello",
															  "Contrabass",
															  "Tremolo Strings",
															  "Pizzicato Strings",
															  "Orchestral Harp",
															  "Timpani",
															  "Ensemble Strings",
															  "Slow Strings",
															  "Synth Strings 1",
															  "Synth Strings 2",
															  "Choir Aahs",
															  "Voice Oohs",
															  "Syn Choir",
															  "Orchestra Hit",
															  "Trumpet",
															  "Trombone",
															  "Tuba",
															  "Muted Trumpet",
															  "French Horn",
															  "Brass Ensemble",
															  "Syn Brass 1",
															  "Syn Brass 2",
															  "Soprano Sax",
															  "Alto Sax",
															  "Tenor Sax",
															  "Baritone Sax",
															  "Oboe",
															  "English Horn",
															  "Bassoon",
															  "Clarinet",
															  "Piccolo",
															  "Flute",
															  "Recorder",
															  "Pan Flute",
															  "Bottle Blow",
															  "Shakuhachi",
															  "Whistle",
															  "Ocarina",
															  "Syn Square Wave",
															  "Syn Saw Wave",
															  "Syn Calliope",
															  "Syn Chiff",
															  "Syn Charang",
															  "Syn Voice",
															  "Syn Fifths Saw",
															  "Syn Brass and Lead",
															  "Fantasia",
															  "Warm Pad",
															  "Polysynth",
															  "Space Vox",
															  "Bowed Glass",
															  "Metal Pad",
															  "Halo Pad",
															  "Sweep Pad",
															  "Ice Rain",
															  "Soundtrack",
															  "Crystal",
															  "Atmosphere",
															  "Brightness",
															  "Goblins",
															  "Echo Drops",
															  "Sci Fi",
															  "Sitar",
															  "Banjo",
															  "Shamisen",
															  "Koto",
															  "Kalimba",
															  "Bag Pipe",
															  "Fiddle",
															  "Shanai",
															  "Tinkle Bell",
															  "Agogo",
															  "Steel Drums",
															  "Woodblock",
															  "Taiko Drum",
															  "Melodic Tom",
															  "Syn Drum",
															  "Reverse Cymbal",
															  "Guitar Fret Noise",
															  "Breath Noise",
															  "Seashore",
															  "Bird",
															  "Telephone",
															  "Helicopter",
															  "Applause",
															  "Gunshot"};

		public double BitRate
		{
			get { return 0; }
		}		
		public double Duration
		{
			get
			{
				int maxTime=0;
				String msgStr;
				int time;
				int index;
				int prevIndex;

				foreach (ArrayList aTrack in tracks)
				{
					msgStr = (String)aTrack[aTrack.Count-1];

					prevIndex = -1;
					index = msgStr.IndexOf(' ',prevIndex+1);
					time = Int32.Parse( msgStr.Substring(prevIndex+1,index-prevIndex) );
					maxTime = Math.Max(maxTime,time);
				}
				return maxTime * (double)getTempo() / getTimebase() / 1000000;
			 }
		}
		public bool IsVBR
		{
			get { return false; }
		}
		public int CodecFamily
		{
			get { return AudioReaderFactory.CF_SEQ; }
		}
		public BinaryLogic.TID3v1 ID3v1
		{
			get { return new TID3v1(); }
		}
		public BinaryLogic.TID3v2 ID3v2
		{
			get { return new TID3v2(); }
		}
		public BinaryLogic.TAPEtag APEtag
		{
			get { return new TAPEtag(); }
		}

		/****************************************************************************
		*                                                                           *
		*                              Public methods                               *
		*                                                                           *
		****************************************************************************/

		//---------------------------------------------------------------
		// creates (or resets to) new empty MIDI song
		//---------------------------------------------------------------
		public void open()
		{
			open(480);
		}

		public void open(int timebase)
		{
			this.tempo = 0;//125000 = 120 bpm
			this.timebase = timebase;
			this.tracks = new ArrayList();
			this.tempoMsgNum = -1;
		}

		//---------------------------------------------------------------
		// sets tempo by replacing set tempo msg in track 0 (or adding new track 0)
		//---------------------------------------------------------------
		public void setTempo(int iTempo)
		{
            this.tempo = (int)Math.Round((double)iTempo);
			if ( this.tempoMsgNum != -1 ) 
			{    
				((ArrayList)this.tracks[0])[this.tempoMsgNum] = "0 Tempo "+iTempo;
			}
			else
			{
				ArrayList tempoTrack = new ArrayList();
				tempoTrack.Add("0 TimeSig 4/4 24 8");
				tempoTrack.Add("0 Tempo "+iTempo);
				tempoTrack.Add("0 Meta TrkEnd");
				this.tracks.Add(tempoTrack);
				this.tempoMsgNum = 1;
			}
		}

		//---------------------------------------------------------------
		// returns tempo (0 if not set)
		//---------------------------------------------------------------
		public int getTempo()
		{
			return this.tempo;
		}

		//---------------------------------------------------------------
		// sets tempo corresponding to given bpm
		//---------------------------------------------------------------
		public void setBpm(int bpm)
		{
			int tempo = 0;
			if (bpm != 0)
			{
				tempo = (int)Math.Round((double)60000000/bpm);
			}
			this.setTempo(tempo);
		}

		//---------------------------------------------------------------
		// returns bpm corresponding to tempo
		//---------------------------------------------------------------
		public int getBpm()
		{
			if (this.tempo != 0)
			{
				return (int)(60000000/this.tempo);
			}
			else
			{
				return 0;
			}
		}

		//---------------------------------------------------------------
		// sets timebase
		//---------------------------------------------------------------
		public void setTimebase(int tb)
		{
			this.timebase = tb;
		}

		//---------------------------------------------------------------
		// returns timebase
		//---------------------------------------------------------------
		public int getTimebase()
		{
			return this.timebase;
		}
		//---------------------------------------------------------------
		// adds new track, returns new track count
		//---------------------------------------------------------------
		public int newTrack()
		{
			this.tracks.Add(new ArrayList());	
			return this.tracks.Count;
		}

		//---------------------------------------------------------------
		// returns track $tn as array of msg strings
		//---------------------------------------------------------------
		public ArrayList getTrack(int tn)
		{
			return (ArrayList)this.tracks[tn];
		}

		//---------------------------------------------------------------
		// returns number of messages of track $tn
		//---------------------------------------------------------------
		public int getMsgCount(int tn)
		{
			return ((ArrayList)(this.tracks[tn])).Count;
		}

		//---------------------------------------------------------------
		// adds message to end of track $tn
		//---------------------------------------------------------------
		/*
		public void addMsg(int tn, String msgStr)
		{
			addMsg(tn, msgStr, 0);
		}

		public void addMsg(int tn, String msgStr, int ttype) //0:absolute, 1:delta
		{ 
			ArrayList track = (ArrayList)this.tracks[tn];
			int last;
			int dt;
			ArrayList msg;

    
			if (1 == ttype)
			{
				last = this->_getTime(track[track.count-1]);
				msg = explode(' ',$msgStr);
				dt = (int) $msg[0];
				msg[0] = $last + $dt;
				msgStr = implode(' ',$msg);
			}

			$track[] = $msgStr;
			$this->tracks[$tn] = $track;
		}
		*/
		//---------------------------------------------------------------
		// adds message at adequate position of track $n (slower than addMsg)
		//---------------------------------------------------------------
		/*
		function insertMsg($tn,$msgStr){
			$time = $this->_getTime($msgStr);
			$track = $this->tracks[$tn];
			$mc = count($track);
			for ($i=0;$i<$mc;$i++){
				$t = $this->_getTime($track[$i]);
				if ($t>=$time) break;
			}
			array_splice($this->tracks[$tn], $i, 0, $msgStr);
		}
		*/
		//---------------------------------------------------------------
		// returns message number $mn of track $tn
		//---------------------------------------------------------------
		public String getMsg(int tn, int mn)
		{
			return (String)((ArrayList)this.tracks[tn])[mn];
		}

		//---------------------------------------------------------------
		// deletes message number $mn of track $tn
		//---------------------------------------------------------------
		/*
		function deleteMsg($tn,$mn)
		{
			array_splice($this->tracks[$tn], $mn, 1);
		}
		*/
		//---------------------------------------------------------------
		// deletes track $tn
		//---------------------------------------------------------------
		/*
		function deleteTrack($tn){
			array_splice($this->tracks, $tn, 1);
			return count($this->tracks);
		}
		*/
		//---------------------------------------------------------------
		// returns number of tracks
		//---------------------------------------------------------------
		public int getTrackCount()
		{
			return this.tracks.Count;
		}

		//---------------------------------------------------------------
		// deletes all tracks except track $tn (and $track 0 which contains tempo info)
		//---------------------------------------------------------------
		/*
		function soloTrack($tn)
		{
			if ($tn==0) $this->tracks = array($this->tracks[0]);
			else $this->tracks = array($this->tracks[0],$this->tracks[$tn]);
		}
		*/
		//---------------------------------------------------------------
		// transposes song by $dn half tone steps
		//---------------------------------------------------------------
		/*
		function transpose($dn){
			$tc = count($this->tracks);
			for ($i=0;$i<$tc;$i++) $this->transposeTrack($i,$dn);
		}
		*/
		//---------------------------------------------------------------
		// transposes track $tn by $dn half tone steps
		//---------------------------------------------------------------
		/*
		function transposeTrack($tn, $dn){
			$track = $this->tracks[$tn];
		  $mc = count($track);
		  for ($i=0;$i<$mc;$i++){
			$msg = explode(' ',$track[$i]);
				if ($msg[1] == 'On' || $msg[1] == 'Off'){
					eval("\$".$msg[3].';'); // $n
					$n = max(0,min(127,$n+$dn));
					$msg[3] = "n=$n";
					$track[$i] = join(' ',$msg);
				}
			}
			$this->tracks[$tn] = $track;
		}
		*/
		//---------------------------------------------------------------
		// import whole MIDI song as text (mf2t-format)
		//---------------------------------------------------------------
		/*
		function importTxt($txt){
			$txt = trim($txt);
			// make unix text format
			if (strpos($txt,"\r")!==false && strpos($txt,"\n")===false) // MAC
				$txt = str_replace("\r","\n",$txt);
			else // PC?
				$txt = str_replace("\r",'',$txt);
			$txt = $txt."\n";// makes things easier

			$headerStr = strtok($txt,"\n");
			$header = explode(' ',$headerStr); //"MFile $type $tc $timebase";
			$this->type = $header[1];
			$this->timebase = $header[3];
			$this->tempo = 0;

			$trackStrings = explode("MTrk\n",$txt);
			array_shift($trackStrings);
			$tracks = array();
			foreach ($trackStrings as $trackStr){
				$track = explode("\n",$trackStr);
				array_pop($track);
				array_pop($track);

				if ($track[0]=="TimestampType=Delta"){//delta
					array_shift($track);
					$track = _delta2Absolute($track);
				}

				$tracks[] = $track;
			}
			$this->tracks = $tracks;
			$this->_findTempo();
		}
		*/
		//---------------------------------------------------------------
		// imports track as text (mf2t-format)
		//---------------------------------------------------------------
		/*
		function importTrackTxt($txt, $tn){
			$txt = trim($txt);
			// make unix text format
			if (strpos($txt,"\r")!==false && strpos($txt,"\n")===false) // MAC
				$txt = str_replace("\r","\n",$txt);
			else // maybe PC, 0D 0A?
				$txt = str_replace("\r",'',$txt);

			$track = explode("\n",$txt);

			if ($track[0]=='MTrk') array_shift($track);
			if ($track[count($track)-1]=='TrkEnd') array_pop($track);

			if ($track[0]=="TimestampType=Delta"){//delta
				array_shift($track);
				$track = _delta2Absolute($track);
			}

			$tn = isset($tn)?$tn:count($this.tracks);
			$this->tracks[$tn] = $track;
			if ($tn==0) $this->_findTempo();
		}
		*/
		//---------------------------------------------------------------
		// returns MIDI song as text
		//---------------------------------------------------------------
		/*
		function getTxt($ttype=0){ //0:absolute, 1:delta
			$timebase = $this->timebase;
			$tracks = $this->tracks;
			$tc = count($tracks);
			$type = ($tc>1)?1:0;
			$str =  "MFile $type $tc $timebase\n";
			for ($i=0;$i<$tc;$i++) $str .= $this->getTrackTxt($i,$ttype);
			return $str;
		}
		*/
		//---------------------------------------------------------------
		// returns track as text
		//---------------------------------------------------------------
		/*
		function getTrackTxt($tn,$ttype=0){ //0:absolute, 1:delta
			$track = $this->tracks[$tn];
			$str = "MTrk\n";
			if ($ttype==1) {//time as delta
				$str .= "TimestampType=Delta\n";
				$last = 0;
				foreach ($track as $msgStr){
					$msg = explode (' ', $msgStr);
					$t = (int) $msg[0];
					$msg[0] = $t - $last;
					$str .= implode(' ',$msg)."\n";
					$last = $t;
				}
			}else foreach ($track as $msg) $str .= $msg."\n";
			$str .= "TrkEnd\n";
			return $str;
		}
		*/
		//---------------------------------------------------------------
		// returns MIDI XML representation (v0.9, http://www.musicxml.org/dtds/midixml.dtd)
		//---------------------------------------------------------------
		/*
		function getXml($ttype=0){ //0:absolute, 1:delta
			$tracks = $this->tracks;
			$tc = count($tracks);
			$type = ($tc>1)?1:0;
			$timebase = $this->timebase;

			$xml = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>
		<!DOCTYPE MIDIFile PUBLIC
		  \"-//Recordare//DTD MusicXML 0.9 MIDI//EN\"
		  \"http://www.musicxml.org/dtds/midixml.dtd\">
		<MIDIFile>
		<Format>$type</Format>
		<TrackCount>$tc</TrackCount>
		<TicksPerBeat>$timebase</TicksPerBeat>
		<TimestampType>".($ttype==1?'Delta':'Absolute')."</TimestampType>\n";

		  for ($i=0;$i<$tc;$i++){
			$xml .= "<Track Number=\"$i\">\n";
			$track = $tracks[$i];
			$mc = count($track);
			$last = 0;
			for ($j=0;$j<$mc;$j++){
				$msg = explode(' ',$track[$j]);
				$t = (int) $msg[0];
				if ($ttype==1){//delta
					$dt = $t - $last;
					$last = $t;
				}
				$xml .= "  <Event>\n";
				$xml .= ($ttype==1)?"    <Delta>$dt</Delta>\n":"    <Absolute>$t</Absolute>\n";
				$xml .= '    ';

					switch($msg[1]){
						case 'PrCh':
							eval("\$".$msg[2].';'); // $ch
							eval("\$".$msg[3].';'); // $p
						$xml .= "<ProgramChange Channel=\"$ch\" Number=\"$p\"/>\n";
							break;

						case 'On':
						case 'Off':
							eval("\$".$msg[2].';'); // $ch
							eval("\$".$msg[3].';'); // $n
							eval("\$".$msg[4].';'); // $v
						$xml .= "<Note{$msg[1]} Channel=\"$ch\" Note=\"$n\" Velocity=\"$v\"/>\n";
							break;

						case 'PoPr':
							eval("\$".$msg[2].';'); // $ch
							eval("\$".$msg[3].';'); // $n
							eval("\$".$msg[4].';'); // $v
							$xml .= "<PolyKeyPressure Channel=\"$ch\" Note=\"$n\" Pressure=\"$v\"/>\n";
							break;

						case 'Par':
							eval("\$".$msg[2].';'); // ch
							eval("\$".$msg[3].';'); // c
							eval("\$".$msg[4].';'); // v
							$xml .= "<ControlChange Channel=\"$ch\" Control=\"$c\" Value=\"$v\"/>\n";
							break;

						case 'ChPr':
							eval("\$".$msg[2].';'); // ch
							eval("\$".$msg[3].';'); // v
							$xml .= "<ChannelKeyPressure Channel=\"$ch\" Pressure=\"$v\"/>\n";
							break;

						case 'Pb':
							eval("\$".$msg[2].';'); // ch
							eval("\$".$msg[3].';'); // v
							$xml .= "<PitchBendChange Channel=\"$ch\" Value=\"$v\"/>\n";
							break;

						case 'Seqnr':
							$xml .= "<SequenceNumber Value=\"{$msg[2]}\"/>\n";
							break;

						case 'Meta':
							$txttypes = array('Text','Copyright','TrkName','InstrName','Lyric','Marker','Cue');
							$mtype = $msg[2];

							$pos = array_search($mtype, $txttypes);
							if ($pos !== FALSE){
								$tags = array('TextEvent','CopyrightNotice','TrackName','InstrumentName','Lyric','Marker','CuePoint');
								$tag = $tags[$pos];
								$line = $track[$j];
								$start = strpos($line,'"')+1;
								$end = strrpos($line,'"');
								$txt = substr($line,$start,$end-$start);
								$xml .= "<$tag>".htmlspecialchars($txt)."</$tag>\n";
							}else{
								if ($mtype == 'TrkEnd')
								$xml .= "<EndOfTrack/>\n";
								elseif ($mtype == '0x20' || $mtype == '0x21') // ChannelPrefix ???
								$xml .= "<MIDIChannelPrefix Value=\"{$msg[3]}\"/>\n";
							}
							break;

						case 'Tempo':
							$xml .= "<SetTempo Value=\"{$msg[2]}\"/>\n";
							break;

						case 'SMPTE':
							$xml .= "<SMPTEOffset TimeCodeType=\"1\" Hour=\"{$msg[2]}\" Minute=\"{$msg[3]}\" Second=\"{$msg[4]}\" Frame=\"{$msg[5]}\" FractionalFrame=\"{$msg[6]}\"/>\n"; //TimeCodeType???
							break;

						case 'TimeSig': // LogDenum???
							$ts = explode('/',$msg[2]);
						$xml .= "<TimeSignature Numerator=\"{$ts[0]}\" LogDenominator=\"".log($ts[1])/log(2)."\" MIDIClocksPerMetronomeClick=\"{$msg[3]}\" ThirtySecondsPer24Clocks=\"{$msg[4]}\"/>\n";
							break;

						case 'KeySig':
							$mode = ($msg[3]=='major')?0:1;
							$xml .= "<KeySignature Fifths=\"{$msg[2]}\" Mode=\"$mode\"/>\n"; // ???
							break;

						case 'SeqSpec':
							$line = $track[$j];
							$start = strpos($line,'SeqSpec')+8;
							$data = substr($line,$start);
							$xml .= "<SequencerSpecific>$data</SequencerSpecific>\n";
							break;

						case 'SysEx':
							$str = '';
							for ($k=3;$k<count($msg);$k++) $str .= $msg[$k].' ';
							$str = trim(strtoupper($str));
							$xml .= "<SystemExclusive>$str</SystemExclusive>\n";
							break;
		*/                                        
		/* TODO:
		<AllSoundOff Channel="9"/>
		<ResetAllControllers Channel="9"/>
		<LocalControl Channel="9" Value="on"/>
		<AllNotesOff Channel="9"/>
		<OmniOff Channel="9"/>
		<OmniOn Channel="9"/>
		<MonoMode Channel="9" Value="5"/>
		<PolyMode Channel="9"/>
		*/
		/*
						default:
							_err('unknown event: '.$msg[1]);
							exit();
					}
					$xml .= "  </Event>\n";
			}
			$xml .= "</Track>\n";
		  }
		  $xml .= "</MIDIFile>";
			return $xml;
		}
		*/
		//---------------------------------------------------------------
		// import MIDI XML representation
		// (so far only a subset of http://www.musicxml.org/dtds/midixml.dtd (v0.8), see documentation)
		//---------------------------------------------------------------
		/*
		function importXml($xmlStr){
			$this->evt = array();
			$this->atr = array();
			$this->dat = '';
			$this->open();

			$this->xml_parser = xml_parser_create("ISO-8859-1");
			xml_set_object($this->xml_parser, &$this);
			xml_set_element_handler($this->xml_parser, "_startElement", "_endElement");
			xml_set_character_data_handler($this->xml_parser,"_chData");
			if (!xml_parse($this->xml_parser, $xmlStr, TRUE))
				die(sprintf("XML error: %s at line %d", xml_error_string(xml_get_error_code($this->xml_parser)),xml_get_current_line_number($this->xml_parser)));
			xml_parser_free($this->xml_parser);
		}
		*/


		//---------------------------------------------------------------
		// imports Standard MIDI File (typ 0 or 1) (and RMID)
		// (if optional parameter $tn set, only track $tn is imported)
		//---------------------------------------------------------------
//		public void importMid(String iPath)
		public bool ReadFromFile(String iPath)
		{
			FileStream fs = null;
			BinaryReader r = null;
			char[] header;

			char type;
			char readChar;
			int timeBase;

			int tsc;
			String track;
			ArrayList trackStrings = new ArrayList();
			ArrayList tracks = new ArrayList();
    
			try
			{
				fs = new FileStream(iPath, FileMode.Open, FileAccess.Read);
				r = new BinaryReader(fs);
				String trigger = "";
				long fileLength = fs.Length;
        
				// Ignores everything (comments) written before the MIDI header
				while( trigger != "MThd")
				{
					trigger += Utils.GetStringFromCharArray( Utils.ReadTrueChars(r,1) );
					if (trigger.Length > 4) trigger = trigger.Remove(0,1);
				}

				// Ready to read header data...
				header = Utils.ReadTrueChars(r,10);
				if ( (header[0] != '\0') ||
					 (header[1] != '\0') ||
					 (header[2] != '\0') ||
					 (header[3] != (char)6)					
					)
				{
					System.Console.WriteLine("Wrong MIDI-header");
				}
				type = header[5];
    
				if ( type > 1)
				{
					System.Console.WriteLine("only SMF Type 0 and 1 supported");
				}
    
				//$trackCnt = (byte)($header[10])*256 + (byte)($header[11]); //ignore
				timeBase = ((byte)header[8]*256)+(byte)header[9];

				this.type = type;
				this.timebase = timeBase;
				this.tempo = 0; // maybe (hopefully!) overwritten by _parseTrack

				// Ready to read track data...
				while( fs.Position < fileLength )
				{
					track = "";
					trigger = "";
					while( (trigger != "MTrk") && (fs.Position < fileLength) )
					{
						readChar = Utils.ReadTrueChar(r);
						track += readChar;
						trigger += readChar;
						if (trigger.Length > 4) trigger = trigger.Remove(0,1);
					}
					// new track found 
					track.Remove(track.Length-4,4);
					trackStrings.Add(track);
				}
			} 
			catch (Exception e) 
			{
				System.Console.WriteLine(e.StackTrace);
				LogDelegator.GetLogDelegate()(Log.LV_ERROR,e.Message);
			}
    
			if (r != null)
			{
				r.Close();
			}
    

			/*
			header = data.Substring(0,14);

			if ( "MThd"+'\0'+'\0'+'\0'+(char)6 != header.Substring(0,8) )
			{
				System.Console.WriteLine("Wrong MIDI-header");
			}
			type = header[9];
    
			if ( type > 1)
			{
				System.Console.WriteLine("only SMF Type 0 and 1 supported");
			}
    
			//$trackCnt = (byte)($header[10])*256 + (byte)($header[11]); //ignore
			timeBase = ((byte)header[12]*256)+(byte)header[13];

			this.type = type;
			this.timebase = timeBase;
			this.tempo = 0; // maybe (hopefully!) overwritten by _parseTrack
			
			
			predIndex = -1;
			index = data.IndexOf("MTrk",predIndex+1);
			//#
			while(index != -1)
			{
				trackStrings.Add(data.Substring(predIndex+4,index-predIndex-3));
				predIndex = index;
				index = data.IndexOf("MTrk",predIndex+1);
			}
			trackStrings.Add(data.Substring(predIndex+4,data.Length-predIndex-4));
			*/
			//$trackStrings = explode('MTrk',$song);
    
			// Deletes the first element of the array (should be empty)
			//array_shift($trackStrings);
			trackStrings.RemoveAt(0);
    
			tsc = trackStrings.Count;
    
			/* This behaviour is not useful to Ethos (yet)
			if ( func_num_args() > 1 )
			{
				$tn =  func_get_arg(1);
				if ($tn>=$tsc) _err('SMF has less tracks than $tn');
				$tracks[] = $this->_parseTrack($trackStrings[$tn],$tn);
			} else
			{*/
			// Parsing track data
			for (int i=0;i<tsc;i++)  
			{   
				tracks.Add(_parseTrack((String)trackStrings[i],i));
				//$tracks[] = $this->_parseTrack($trackStrings[$i],$i);
			}
			//}
			this.tracks = tracks;
			return true;
		}

		//---------------------------------------------------------------
		// returns binary MIDI string
		//---------------------------------------------------------------
		/*
		function getMid(){
			$tracks = $this->tracks;
			$tc = count($tracks);
			$type = ($tc > 1)?1:0;
			$midStr = "MThd\0\0\0\6\0".chr($type)._getBytes($tc,2)._getBytes($this->timebase,2);
			for ($i=0;$i<$tc;$i++){
				$track = $tracks[$i];
				$mc = count($track);
				$time = 0;
				$midStr .= "MTrk";
				$trackStart = strlen($midStr);

				$last = '';

				for ($j=0;$j<$mc;$j++){
					$line = $track[$j];
					$t = $this->_getTime($line);
					$dt = $t - $time;
					$time = $t;
					$midStr .= _writeVarLen($dt);

					// repetition, same event, same channel, omit first byte (smaller file size)
					// TODO: same for PoPr,ChPr,Par,Pb
					$str = $this->_getMsgStr($line);
					$start = (byte)($str[0]);
					if ($start>=0x80 && $start<=0x9F && $start==$last) $str = substr($str, 1);
					$last = $start;

					$midStr .= $str;
				}
				$trackLen = strlen($midStr) - $trackStart;
				$midStr = substr($midStr,0,$trackStart)._getBytes($trackLen,4).substr($midStr,$trackStart);
			}
			return $midStr;
		}

		//---------------------------------------------------------------
		// saves MIDI song as Standard MIDI File
		//---------------------------------------------------------------
		function saveMidFile($mid_path){
			if (count($this->tracks)<1) _err('MIDI song has no tracks');
			$SMF = fopen($mid_path, "wb"); // SMF
			fwrite($SMF,$this->getMid());
			fclose($SMF);
		}

		//---------------------------------------------------------------
		// embeds Standard MIDI File
		//---------------------------------------------------------------
		function playMidFile($file,$visible=1,$auto=1,$loop=1,$plug=''){

			switch($plug){
				case 'qt':
		?>
		<!-- QT -->
		<OBJECT CLASSID="clsid:02BF25D5-8C17-4B23-BC80-D3488ABDDC6B" WIDTH="<?php echo ($visible==0)?2:160?>" HEIGHT="<?php echo ($visible==0)?2:16?>" CODEBASE="http://www.apple.com/qtactivex/qtplugin.cab">
		<PARAM NAME="SRC" VALUE="<?php echo $file?>">
		<PARAM NAME="AUTOPLAY" VALUE="<?php echo $auto?'true':'false'?>">
		<PARAM NAME="LOOP" VALUE="<?php echo $loop?'true':'false'?>">
		<PARAM NAME="CONTROLLER" VALUE="<?php echo ($visible==0)?'false':'true'?>">
		<?php echo ($visible==0)?'<PARAM NAME="HIDDEN" VALUE="true">':''?>
		<EMBED TYPE="video/quicktime" SRC="<?php echo $file?>" WIDTH="<?php echo ($visible==0)?2:160?>" HEIGHT="<?php echo ($visible==0)?2:16?>" AUTOPLAY="<?php echo $auto?'true':'false'?>" LOOP="<?php echo $loop?'true':'false'?>" CONTROLLER="<?php echo ($visible==0)?'false':'true'?>" <?php echo ($visible==0)?'HIDDEN="true" ':''?>PLUGINSPAGE="http://www.apple.com/quicktime/download/">
		</OBJECT>
		<?php
				break;
				case 'wm':
		?>
		<!-- WMP -->
		<OBJECT CLASSID="CLSID:22D6f312-B0F6-11D0-94AB-0080C74C7E95" CODEBASE="http://www.microsoft.com/ntserver/netshow/download/en/nsmp2inf.cab#Version=5,1,51,415" type="application/x-oleobject" width=<?php echo ($visible==0)?0:300?> height=<?php echo ($visible==0)?0:44?>>
		<PARAM NAME="AutoStart" VALUE="<?php echo $auto?'true':'false'?>">
		<PARAM NAME="FileName" VALUE="<?php echo $file?>">
		<PARAM NAME="ControlType" VALUE="1">
		<PARAM NAME="Loop" VALUE="<?php echo $loop?'true':'false'?>">
		<PARAM NAME="ShowControls" VALUE="<?php echo ($visible==0)?'false':'true'?>">
		<EMBED TYPE="video/x-ms-asf-plugin" PLUGINSPAGE="http://www.microsoft.com/windows/mediaplayer/download/default.asp" SRC="<?php echo $file?>" AutoStart="<?php echo $auto?1:0?>" ShowControls="<?php echo ($visible==0)?0:1?>" Loop="<?php echo $loop?>" width=<?php echo ($visible==0)?0:300?> height=<?php echo ($visible==0)?0:44?>>
		</OBJECT>
		<?php
				break;
				case 'bk':
		?>
		<OBJECT CLASSID="CLSID:B384F118-18EE-11D1-95C8-00A024330339" CODEBASE="http://dasdeck.de/beatnik/beatnik.cab" WIDTH=<?php echo ($visible==0)?2:144?> HEIGHT=<?php echo ($visible==0)?0:15?>>
		   <PARAM NAME="SRC" VALUE="<?php echo $file?>">
		   <PARAM NAME="TYPE" VALUE="audio/midi">
		   <PARAM NAME="WIDTH" VALUE="<?php echo ($visible==0)?2:144?>">
		   <PARAM NAME="HEIGHT" VALUE="<?php echo ($visible==0)?2:15?>">
		   <PARAM NAME="DISPLAY" VALUE="song">
		   <PARAM NAME="AUTOSTART" VALUE="<?php echo $auto?'true':'false'?>">
		   <PARAM NAME="LOOP" VALUE="<?php echo $loop?'true':'false'?>">
		   <?php echo ($visible==0)?'<PARAM NAME="HIDDEN" VALUE="true">':''?>
		   <EMBED TYPE="audio/rmf" SRC="<?php echo $file?>" WIDTH="<?php echo ($visible==0)?2:144?>" HEIGHT="<?php echo ($visible==0)?2:15?>" DISPLAY="SONG" AUTOSTART="<?php echo $auto?'true':'false'?>" LOOP="<?php echo $loop?'true':'false'?>" PLUGINSPAGE="http://www.beatnik.com/to/?player"<?php echo ($visible==0)?' HIDDEN="true"':''?>
		   >
		</OBJECT>
		<?php
				break;
				case 'jv':
		?>
		<OBJECT classid="clsid:CAFEEFAC-0014-0002-0000-ABCDEFFEDCBA" WIDTH="<?php echo ($visible==0)?0:32?>" HEIGHT="<?php echo ($visible==0)?0:32?>" CODEBASE="http://java.sun.com/products/plugin/autodl/jinstall-1_4_2-windows-i586.cab#Version=1,4,2,0">
		<PARAM NAME="code" VALUE="MidiApplet.class">
		<PARAM NAME="type" VALUE="application/x-java-applet;jpi-version=1.4.2">
		<PARAM NAME="src" VALUE="<?php echo $file?>">
		<PARAM NAME="autostart" VALUE="<?php echo $auto?1:0?>">
		<PARAM NAME="loop" VALUE="<?php echo $loop?1:0?>">
		<COMMENT>
		<EMBED TYPE="application/x-java-applet;jpi-version=1.4.2" CODE="MidiApplet.class" WIDTH="<?php echo ($visible==0)?0:32?>" HEIGHT="<?php echo ($visible==0)?0:32?>" pluginspage="http://java.sun.com/products/plugin/index.html#download" autostart="<?php echo $auto?1:0?>" loop="<?php echo $loop?1:0?>"></EMBED>
		</COMMENT>
		</OBJECT>
		<?php
				break;
				default:
		?>
				<EMBED SRC="<?php echo $file?>" TYPE="audio/midi" AUTOSTART="<?php echo $auto?'TRUE':'FALSE'?>" LOOP="<?php echo $loop?'TRUE':'FALSE'?>"<?php echo ($visible==0)?' HIDDEN="true"':''?>>
		<?php
			}
		}

		//---------------------------------------------------------------
		// starts download of Standard MIDI File
		//---------------------------------------------------------------
		function downloadMidFile($file,$output){
		// MM: added for compressed output...
			ob_start("ob_gzhandler");

			//$mime_type = 'audio/midi';
			$mime_type = 'application/octetstream'; // force download

			header('Content-Type: '.$mime_type);
			header('Expires: '.gmdate('D, d M Y H:i:s').' GMT');
			header('Content-Disposition: attachment; filename="'.$output.'"');
			header('Pragma: no-cache');

			$d=fopen($file,"rb");
			fpassthru($d);
			@fclose($d);
			exit();
		}
		*/
		//***************************************************************
		// PUBLIC UTILITIES
		//***************************************************************

		//---------------------------------------------------------------
		// returns list of drumset instrument names
		//---------------------------------------------------------------
		/* ## TO DO
		function getDrumset(){
			return array(
			35=>'Acoustic Bass Drum',
			36=>'Bass Drum 1',
			37=>'Side Stick',
			38=>'Acoustic Snare',
			39=>'Hand Clap',
			40=>'Electric Snare',
			41=>'Low Floor Tom',
			42=>'Closed Hi-Hat',
			43=>'High Floor Tom',
			44=>'Pedal Hi-Hat',
			45=>'Low Tom',
			46=>'Open Hi-Hat',
			47=>'Low Mid Tom',
			48=>'High Mid Tom',
			49=>'Crash Cymbal 1',
			50=>'High Tom',
			51=>'Ride Cymbal 1',
			52=>'Chinese Cymbal',
			53=>'Ride Bell',
			54=>'Tambourine',
			55=>'Splash Cymbal',
			56=>'Cowbell',
			57=>'Crash Cymbal 2',
			58=>'Vibraslap',
			59=>'Ride Cymbal 2',
			60=>'High Bongo',
			61=>'Low Bongo',
			62=>'Mute High Conga',
			63=>'Open High Conga',
			64=>'Low Conga',
			65=>'High Timbale',
			66=>'Low Timbale',
			//35..66
			67=>'High Agogo',
			68=>'Low Agogo',
			69=>'Cabase',
			70=>'Maracas',
			71=>'Short Whistle',
			72=>'Long Whistle',
			73=>'Short Guiro',
			74=>'Long Guiro',
			75=>'Claves',
			76=>'High Wood Block',
			77=>'Low Wood Block',
			78=>'Mute Cuica',
			79=>'Open Cuica',
			80=>'Mute Triangle',
			81=>'Open Triangle');
		}
		*/

		//---------------------------------------------------------------
		// returns list of standard drum kit names
		//---------------------------------------------------------------
		/* ## TO DO 
		function getDrumkitList(){
			return array(
				1   => 'Dry',
				9   => 'Room',
				19  => 'Power',
				25  => 'Electronic',
				33  => 'Jazz',
				41  => 'Brush',
				57  => 'SFX',
				128 => 'Default'
			);
		}
		*/

		//---------------------------------------------------------------
		// returns list of note names
		//---------------------------------------------------------------
		/*
		function getNoteList(){
		  //note 69 (A6) = A440
		  //note 60 (C6) = Middle C
			return array(
			//Do          Re           Mi    Fa           So           La           Ti
			'C0', 'Cs0', 'D0', 'Ds0', 'E0', 'F0', 'Fs0', 'G0', 'Gs0', 'A0', 'As0', 'B0',
			'C1', 'Cs1', 'D1', 'Ds1', 'E1', 'F1', 'Fs1', 'G1', 'Gs1', 'A1', 'As1', 'B1',
			'C2', 'Cs2', 'D2', 'Ds2', 'E2', 'F2', 'Fs2', 'G2', 'Gs2', 'A2', 'As2', 'B2',
			'C3', 'Cs3', 'D3', 'Ds3', 'E3', 'F3', 'Fs3', 'G3', 'Gs3', 'A3', 'As3', 'B3',
			'C4', 'Cs4', 'D4', 'Ds4', 'E4', 'F4', 'Fs4', 'G4', 'Gs4', 'A4', 'As4', 'B4',
			'C5', 'Cs5', 'D5', 'Ds5', 'E5', 'F5', 'Fs5', 'G5', 'Gs5', 'A5', 'As5', 'B5',
			'C6', 'Cs6', 'D6', 'Ds6', 'E6', 'F6', 'Fs6', 'G6', 'Gs6', 'A6', 'As6', 'B6',
			'C7', 'Cs7', 'D7', 'Ds7', 'E7', 'F7', 'Fs7', 'G7', 'Gs7', 'A7', 'As7', 'B7',
			'C8', 'Cs8', 'D8', 'Ds8', 'E8', 'F8', 'Fs8', 'G8', 'Gs8', 'A8', 'As8', 'B8',
			'C9', 'Cs9', 'D9', 'Ds9', 'E9', 'F9', 'Fs9', 'G9', 'Gs9', 'A9', 'As9', 'B9',
			'C10','Cs10','D10','Ds10','E10','F10','Fs10','G10');
		}
		*/

		/****************************************************************************
		*                                                                           *
		*                              Private methods                              *
		*                                                                           *
		****************************************************************************/

		//---------------------------------------------------------------
		// returns time code of message string
		//---------------------------------------------------------------
		private int _getTime(String message)
		{
			return Int32.Parse( message.Substring( 0, message.IndexOf(' ') ) );
			//strtok($msgStr,' ');
		}

		//---------------------------------------------------------------
		// returns binary code for message string
		//---------------------------------------------------------------
		/* NOT USED SO FAR
		function _getMsgStr($line){
			$msg = explode(' ',$line);
			switch($msg[1]){
				case 'PrCh': // 0x0C
					eval("\$".$msg[2].';'); // chan
					eval("\$".$msg[3].';'); // prog
					return chr(0xC0+$ch-1).chr($p);
					break;
				case 'On': // 0x09
					eval("\$".$msg[2].';'); // chan
					eval("\$".$msg[3].';'); // note
					eval("\$".$msg[4].';'); // vel
					return chr(0x90+$ch-1).chr($n).chr($v);
					break;
				case 'Off': // 0x08
					eval("\$".$msg[2].';'); // chan
					eval("\$".$msg[3].';'); // note
					eval("\$".$msg[4].';'); // vel
					return chr(0x80+$ch-1).chr($n).chr($v);
					break;
				case 'PoPr': // 0x0A = PolyPressure
					eval("\$".$msg[2].';'); // chan
					eval("\$".$msg[3].';'); // note
					eval("\$".$msg[4].';'); // val
					return chr(0xA0+$ch-1).chr($n).chr($v);
					break;
				case 'Par': // 0x0B = ControllerChange
					eval("\$".$msg[2].';'); // chan
					eval("\$".$msg[3].';'); // controller
					eval("\$".$msg[4].';'); // val
					return chr(0xB0+$ch-1).chr($c).chr($v);
					break;
				case 'ChPr': // 0x0D = ChannelPressure
					eval("\$".$msg[2].';'); // chan
					eval("\$".$msg[3].';'); // val
					return chr(0xD0+$ch-1).chr($v);
					break;
				case 'Pb': // 0x0E = PitchBend
					eval("\$".$msg[2].';'); // chan
					eval("\$".$msg[3].';'); // val (2 Bytes!)
					$a = $v & 0x7f; // Bits 0..6
					$b = ($v >> 7) & 0x7f; // Bits 7..13
					return chr(0xE0+$ch-1).chr($a).chr($b);
					break;
				// META EVENTS
				case 'Seqnr': // 0x00 = sequence_number
					$num = chr($msg[2]);
					if ($msg[2]>255) _err("code broken around Seqnr event");
					return "\xFF\x00\x02\x00$num";
					break;
				case 'Meta':
					$type = $msg[2];
					switch ($type){
						case 'Text': //0x01: // Meta Text
						case 'Copyright': //0x02: // Meta Copyright
						case 'TrkName': //0x03: // Meta TrackName ???SeqName???
						case 'InstrName': //0x04: // Meta InstrumentName
						case 'Lyric': //0x05: // Meta Lyrics
						case 'Marker': //0x06: // Meta Marker
						case 'Cue': //0x07: // Meta Cue
							$texttypes = array('Text','Copyright','TrkName','InstrName','Lyric','Marker','Cue');
							$byte = chr(array_search($type,$texttypes)+1);
							$start = strpos($line,'"')+1;
							$end = strrpos($line,'"');
							$txt = substr($line,$start,$end-$start);
		// MM: Todo: Len could also be more than one Byte (variable length; see. "Sequence/Track name specification)
							$len = chr(strlen($txt));
							if ($len>127) _err("code broken (write varLen-Meta)");
							return "\xFF$byte$len$txt";
							break;
						case 'TrkEnd': //0x2F
							return "\xFF\x2F\x00";
							break;
						case '0x20': // 0x20 = ChannelPrefix
							$v = chr($msg[3]);
							return "\xFF\x20\x01$v";
							break;
						case '0x21': // 0x21 = ChannelPrefixOrPort
							$v = chr($msg[3]);
							return "\xFF\x21\x01$v";
							break;
						default:
							_err("unknown meta event: $type");
							exit();
					}
					break;
				case 'Tempo': // 0x51
					$tempo = _getBytes((int)$msg[2],3);
					return "\xFF\x51\x03$tempo";
					break;
				case 'SMPTE': // 0x54 = SMPTE offset
					$h = chr($msg[2]);
					$m = chr($msg[3]);
					$s = chr($msg[4]);
					$f = chr($msg[5]);
					$fh = chr($msg[6]);
					return "\xFF\x54\x05$h$m$s$f$fh";
					break;
				case 'TimeSig': // 0x58
					$zt = explode('/',$msg[2]);
					$z = chr($zt[0]);
					$t = chr(log($zt[1])/log(2));
					$mc = chr($msg[3]);
					$c = chr($msg[4]);
					return "\xFF\x58\x04$z$t$mc$c";
					break;
				case 'KeySig': // 0x59
					$vz = chr($msg[2]);
					$g = chr(($msg[3]=='major')?0:1);
					return "\xFF\x59\x02$vz$g";
					break;
				case 'SeqSpec': // 0x7F = Sequencer specific data (Bs: 0 SeqSpec 00 00 41)
					$cnt = count($msg)-2;
					$data = '';
					for ($i=0;$i<$cnt;$i++)
						$data.=_hex2bin($msg[$i+2]);
		// MM: ToDo: Len >127 has to be variable length-encoded !!!
					$len = chr(strlen($data));
					if ($len>127) _err('code broken (write varLen-Meta)');
					return "\xFF\x7F$len$data";
					break;
				case 'SysEx': // 0xF0 = SysEx
					$start = strpos($line,'f0');
					$end = strrpos($line,'f7');
					$data = substr($line,$start+3,$end-$start-1);
					$data = _hex2bin(str_replace(' ','',$data));
					$len = chr(strlen($data));
					return "\xF0$len".$data;
					break;

				default:
					_err('unknown event: '.$msg[1]);
					exit();
			}
		}
		*/
		//---------------------------------------------------------------
		// converts binary track string to track (list of msg strings)
		//---------------------------------------------------------------
		private ArrayList _parseTrack(String binStr, int tn)
		{
			//$trackLen2 =  ((( (( ((byte)($binStr[0]) << 8) | (byte)($binStr[1]))<<8) | (byte)($binStr[2]) ) << 8 ) |  (byte)($binStr[3]) );
			//$trackLen2 += 4;
			int trackLen = binStr.Length;
			ArrayList track = new ArrayList();
			// MM: ToDo: Warn if trackLen and trackLen2 are different!!!
			// if ($trackLen != $trackLen2) { echo "Warning: TrackLength is corrupt ($trackLen != $trackLen2)! \n"; }
    
			//# convert explicitly $xxx in strings to their actual values !
			int p=4;
			int time = 0;
			int dt;
			byte tempByte;
			byte high;
			byte low;
			byte chan=0;
			byte prog;
			byte note;
			byte meta;
			byte vel;
			byte val;
			int num;
			int len;
			byte tmp;
			byte c;
			String txt;
			String last="";
    
			while ( p < trackLen )
			{
				// timedelta
				dt = _readVarLen(binStr,ref p);
				time += dt;

				tempByte = (byte)(binStr[p]);//#
				high = (byte)(tempByte >> 4);
				low = (byte)(tempByte - high*16);
				switch(high)
				{
					case 0x0C: //PrCh = ProgramChange
						chan = (byte)(low+1);
						prog = (byte)(binStr[p+1]);
						last = "PrCh";
						track.Add(time+"PrCh ch="+chan+" p="+prog);
						p+=2;
						break;
					case 0x09: //On
						chan = (byte)(low+1);
						note = (byte)(binStr[p+1]);
						vel = (byte)(binStr[p+2]);
						last = "On";
						track.Add(""+time+" On ch="+chan+" n="+note+" v="+vel);
						p+=3;
						break;
					case 0x08: //Off
						chan = (byte)(low+1);
						note = (byte)(binStr[p+1]);
						vel = (byte)(binStr[p+2]);
						last = "Off";
						track.Add(""+time+" Off ch="+chan+" n="+note+" v="+vel);
						p+=3;
						break;
					case 0x0A: //PoPr = PolyPressure
						chan = (byte)(low+1);
						note = (byte)(binStr[p+1]);
						val = (byte)(binStr[p+2]);
						last = "PoPr";
						track.Add(""+time+" PoPr ch="+chan+" n="+note+" v="+val);
						p+=3;
						break;
					case 0x0B: //Par = ControllerChange
						chan = (byte)(low+1);
						c = (byte)(binStr[p+1]);
						val = (byte)(binStr[p+2]);
						last = "Par";
						track.Add(""+time+" Par ch="+chan+" c="+c+" v="+val);
						p+=3;
						break;
					case 0x0D: //ChPr = ChannelPressure
						chan = (byte)(low+1);
						val = (byte)(binStr[p+1]);
						last = "ChPr";
						track.Add(""+time+" ChPr ch="+chan+" v="+val);
						p+=2;
						break;
					case 0x0E: //Pb = PitchBend
						chan = (byte)(low+1);
						val = (byte)( ((byte)(binStr[p+1]) & 0x7F) | ( (((byte)(binStr[p+2])) & 0x7F) << 7 ) );
						last = "Pb";
						track.Add(""+time+" Pb ch="+chan+" v="+val);
						p+=3;
						break;
					default:
						switch(tempByte)
						{
							case 0xFF: // Meta
								meta = (byte)(binStr[p+1]);
							switch (meta)
							{
								case 0x00: // sequence_number
									tmp = (byte)(binStr[p+2]);
									if (tmp==0x00) { num = tn; p+=3;}
									else { num= 1; p+=5; }
									track.Add(""+time+" Seqnr "+num);
									break;

								case 0x01: // Meta Text
								case 0x02: // Meta Copyright
								case 0x03: // Meta TrackName ???sequence_name???
								case 0x04: // Meta InstrumentName
								case 0x05: // Meta Lyrics
								case 0x06: // Meta Marker
								case 0x07: // Meta Cue
									String[] texttypes = new String[7] {"Text","Copyright","TrkName","InstrName","Lyric","Marker","Cue"};
									String type = texttypes[meta-1];
									p +=2;
									len = _readVarLen(binStr,ref p);
									if ( (len+p) > trackLen ) throw new Exception("Meta "+type+" has corrupt variable length field ("+len+") [track: "+tn+" dt: "+dt+"]");
									txt = binStr.Substring(p,len);
									track.Add(""+time+" Meta "+type+" \""+txt+"\"");
									p+=len;
									break;
								case 0x20: // ChannelPrefix
									chan = (byte)(binStr[p+3]);
									//if (chan<10) chan = "0"+chanStr;//???
									track.Add(""+time+" Meta 0x20 "+chan);
									p+=4;
									break;
								case 0x21: // ChannelPrefixOrPort
									chan = (byte)(binStr[p+3]);
									//if (chan<10) chan = "0"+chan.ToString();//???
									track.Add(""+time+" Meta 0x21 "+chan);
									p+=4;
									break;
								case 0x2F: // Meta TrkEnd
									track.Add(""+time+" Meta TrkEnd");
									return track;//ignore rest
									//break;
								case 0x51: // Tempo
									tempo = (byte)(binStr[p+3])*256*256 + (byte)(binStr[p+4])*256 + (byte)(binStr[p+5]);
									track.Add(""+time+" Tempo "+tempo);
									if (tn==0 && time==0) 
									{
										this.tempoMsgNum = track.Count - 1;
									}
									p+=6;
									break;
								case 0x54: // SMPTE offset
									byte h = (byte)(binStr[p+3]);
									byte m = (byte)(binStr[p+4]);
									byte s = (byte)(binStr[p+5]);
									byte f = (byte)(binStr[p+6]);
									byte fh = (byte)(binStr[p+7]);
									track.Add(""+time+" SMPTE "+h+" "+m+" "+s+" "+f+" "+fh);
									p+=8;
									break;
								case 0x58: // TimeSig
									byte z = (byte)(binStr[p+3]);
									int t = 2^(byte)(binStr[p+4]);
									byte mc = (byte)(binStr[p+5]);
									c = (byte)(binStr[p+6]);
									track.Add(""+time+" TimeSig "+z+"/"+t+" "+mc+" "+c);
									p+=7;
									break;
								case 0x59: // KeySig
									byte vz = (byte)(binStr[p+3]);
									String g = (byte)(binStr[p+4])==0?"major":"minor";
									track.Add(""+time+" KeySig "+vz+" "+g);
									p+=5;
									break;
								case 0x7F: // Sequencer specific data (string or hexString???)
									p +=2;
									len = _readVarLen(binStr,ref p);
									if ((len+p) > trackLen) throw new Exception("SeqSpec has corrupt variable length field ("+len+") [track: "+tn+" dt: "+dt+"]");
									p-=3;
									String data="";
									//# some formatting to do here
									for (int i=0;i<len;i++) data+=' '+(byte)(binStr[p+3+i]); //data.=' '.sprintf("%02x",(byte)($binStr[$p+3+$i]));
									track.Add(""+time+" SeqSpec"+data);
									p+=len+3;
									break;

								default:
									// MM added: accept "unknown" Meta-Events
									//# here too
									byte metacode = (byte)(binStr[p+1]);//sprintf("%02x", (byte)($binStr[$p+1]) );
									p +=2;
									len = _readVarLen(binStr,ref p);
									if ((len+p) > trackLen) throw new Exception("Meta "+metacode+" has corrupt variable length field ("+len+") [track: "+tn+" dt: "+dt+"]");
									p -=3;
									data="";
									//# here as well
									for (int i=0;i<len;i++) data+=" "+(byte)(binStr[p+3+i]);//sprintf("%02x",(byte)($binStr[$p+3+$i]));
									track.Add(""+time+" Meta 0x"+metacode+" "+data);
									p+=len+3;
									break;
							} // switch ($meta)
								break; // Ende Meta

							case 0xF0: // SysEx
								p +=1;
								len = _readVarLen(binStr,ref p);
								if ((len+p) > trackLen) throw new Exception("SysEx has corrupt variable length field ("+len+") [track: "+tn+" dt: "+dt+" p: "+p+"]");
								String str = "f0";
								for (int i=0;i<len;i++) str+=' '+(byte)(binStr[p+2+i]); //str+=' '.sprintf("%02x",(byte)(binStr[p+2+i]));
								track.Add(""+time+" SysEx "+str);
								p+=len;
								break;
							default: // Repetition of last event?
								if ((last == "On") || (last == "Off"))
								{
									note = (byte)(binStr[p]);
									vel = (byte)(binStr[p+1]);
									track.Add(""+time+" "+last+" ch="+chan+" n="+note+" v="+vel);
									p+=2;
								}
								if (last == "PrCh")
								{
									prog = (byte)(binStr[p]);
									track.Add(""+time+" PrCh ch="+chan+" p="+prog);
									p+=1;
								}
								if (last == "PoPr")
								{
									note = (byte)(binStr[p+1]);
									val = (byte)(binStr[p+2]);
									track.Add(""+time+" PoPr ch="+chan+" n="+note+" v="+val);
									p+=2;
								}
								if(last == "ChPr")
								{
									val = (byte)(binStr[p]);
									track.Add(""+time+" ChPr ch="+chan+" v="+val);
									p+=1;
								}
								if(last == "Par")
								{
									c = (byte)(binStr[p]);
									val = (byte)(binStr[p+1]);
									track.Add(""+time+" Par ch="+chan+" c="+c+" v="+val);
									p+=2;
								}
								if (last == "Pb")
								{
									val = (byte)( ((byte)(binStr[p])  & 0x7F) | (( (byte)(binStr[p+1]) & 0x7F)<<7) );
									track.Add(""+time+" Pb ch="+chan+" v="+val);
									p+=2;
								}
								//default:
								// MM: ToDo: Repetition of SysEx and META-events? with <last>?? \n";
								//  _err("unknown repetition: $last");
								break;
						} // switch (tempByte)
						break;
				} // switch ($high)
			} // while
			return track;
		}

		//---------------------------------------------------------------
		// search track 0 for set tempo msg
		//---------------------------------------------------------------
		/* NOT USED SO FAR
		function _findTempo(){
			$track = $this->tracks[0];
			$mc = count($track);
			for ($i=0;$i<$mc;$i++){
				$msg = explode(' ',$track[$i]);
				if ((int) $msg[0]>0) break;
				if ($msg[1]=='Tempo'){
					$this->tempo = $msg[2];
					$this->tempoMsgNum = $i;
					break;
				}
			}
		}
		*/
		//---------------------------------------------------------------
		// XML PARSING CALLBACKS
		//---------------------------------------------------------------
		/* NOT USED SO FAR
		function _startElement($parser, $name, $attrs) {

			switch ($name){
				case 'MIDIFILE':
				case 'FORMAT':
				case 'TRACKCOUNT':
				case 'TICKSPERBEAT':
				case 'TIMESTAMPTYPE':
				case 'DELTA':
				case 'ABSOLUTE':
					break;
				case 'TRACK':
					$this->newTrack();
					if ($this->ttype=='Delta') $this->t = 0;
					break;

				case 'EVENT':
					$this->evt = array();
					$this->atr = array();
					$this->dat = '';
					break;

				default:
					$this->atr = $attrs;
			}
		}

		function _endElement($parser, $name){
			switch ($name){
				case 'MIDIFILE':
					break;
				case 'FORMAT':
					$this->type = (int) $this->dat;
					break;
				case 'TRACKCOUNT':
					break;
				case 'TICKSPERBEAT':
					$this->timebase = (int) $this->dat;
					break;
				case 'TIMESTAMPTYPE':
					$this->ttype = $this->dat;//DELTA or ABSOLUTE
					break;
				case 'TRACK':
					break;

				case 'DELTA':
					$this->t = $this->t + (int) $this->dat;
					$this->evt['t'] = $this->t;
					break;
				case 'ABSOLUTE':
					$this->evt['t'] = (int) $this->dat;
					break;

				case 'EVENT':
					$time = $this->evt['t'];
					$a = $this->evt['atr'];
					$typ = $this->evt['typ'];
					$dat = $this->evt['dat'];
					$tn = count($this->tracks)-1;

					switch ($typ){
						case 'PROGRAMCHANGE':
							$msg = "$time PrCh ch={$a['CHANNEL']} p={$a['NUMBER']}";
							break;
						case 'NOTEON':
							$msg = "$time On ch={$a['CHANNEL']} n={$a['NOTE']} v={$a['VELOCITY']}";
							break;
						case 'NOTEOFF':
							$msg = "$time Off ch={$a['CHANNEL']} n={$a['NOTE']} v={$a['VELOCITY']}";
							break;
						case 'POLYKEYPRESSURE':
							$msg = "$time PoPr ch={$a['CHANNEL']} n={$a['NOTE']} v={$a['PRESSURE']}";
							break;
						case 'CONTROLCHANGE':
							$msg = "$time Par ch={$a['CHANNEL']} c={$a['CONTROL']} v={$a['VALUE']}";
							break;
						case 'CHANNELKEYPRESSURE':
							$msg = "$time ChPr ch={$a['CHANNEL']} v={$a['PRESSURE']}";
							break;
						case 'PITCHBENDCHANGE':
							$msg = "$time Pb ch={$a['CHANNEL']} v={$a['VALUE']}";
							break;

						case 'SEQUENCENUMBER':
							$msg = "$time Seqnr {$a['VALUE']}";
							break;

						case 'TEXTEVENT':
						case 'COPYRIGHTNOTICE':
						case 'TRACKNAME':
						case 'INSTRUMENTNAME':
						case 'LYRIC':
						case 'MARKER':
						case 'CUEPOINT':
							$tags = array('TEXTEVENT','COPYRIGHTNOTICE','TRACKNAME','INSTRUMENTNAME','LYRIC','MARKER','CUEPOINT');
							$txttypes = array('Text','Copyright','TrkName','InstrName','Lyric','Marker','Cue');
							$type = $txttypes[array_search($typ, $tags)];
							$msg = "$time Meta $type \"$dat\"";
							break;

						case 'ENDOFTRACK':
							$msg = "$time Meta TrkEnd";
							break;

						case 'MIDICHANNELPREFIX'://???
							if ((int) $dat<10) $dat='0'.$dat;
							$msg = "$time Meta 0x20 $dat";
							break;

						case 'SETTEMPO':
							$tempo = (int) $a['VALUE'];
							$msg = "$time Tempo $tempo";
							if ($tn==0 && $time==0) {//???
								$this->tempo = $tempo;
								$this->tempoMsgNum = count($this->tracks[$tn]);//???
							}
							break;

						case 'SMPTEOFFSET'://???
							$msg = "$time SMPTE {$a['HOUR']} {$a['MINUTE']} {$a['SECOND']} {$a['FRAME']} {$a['FRACTIONALFRAME']}";
							break;

						case 'TIMESIGNATURE':
							$z = (int) $a['NUMERATOR'];
							$t = pow(2,(int) $a['LOGDENOMINATOR']);
							$msg = "$time TimeSig $z/$t {$a['MIDICLOCKSPERMETRONOMECLICK']} {$a['THIRTYSECONDSPER24CLOCKS']}";
							break;

						case 'KEYSIGNATURE':
							$g = ($a['MODE']==0)?'major':'minor';
							$msg = "$time KeySig {$a['FIFTHS']} $g";
							break;

						case 'SEQUENCERSPECIFIC':
							$msg = "$time SeqSpec $dat";
							break;

						case 'SYSTEMEXCLUSIVE'://???
							$dat = strtolower($dat);
							$msg = "$time SysEx f0 $dat";
							break;

						default:return;//ignore
					}

					$this->addMsg(count($this->tracks)-1,$msg);
					$evt = 0;
					break;

				default://within event!?
					$this->evt['typ'] = $name;
					$this->evt['atr'] = $this->atr;
					$this->evt['dat'] = $this->dat;
			}
		}
		*/
		/* NOT USED SO FAR
		function _chData($parser, $data){
			$this->dat = (trim($data)=='')?'':$data;//???
		}
		*/



		//***************************************************************
		// UTILITIES
		//***************************************************************

		//---------------------------------------------------------------
		// hexstr to binstr
		//---------------------------------------------------------------
		/* NOT USED SO FAR
		function _hex2bin($hex_str) {
			$bin_str='';
		  for ($i = 0; $i < strlen($hex_str); $i += 2) {
			$bin_str .= chr(hexdec(substr($hex_str, $i, 2)));
		  }
		  return $bin_str;
		}
		*/
		//---------------------------------------------------------------
		// int to bytes (length $len)
		//---------------------------------------------------------------
		/* NOT USED SO FAR
		function _getBytes($n,$len){
			$str='';
			for ($i=$len-1;$i>=0;$i--){
				$str.=chr(floor($n/pow(256,$i)));
			}
			return $str;
		}
		*/
		//---------------------------------------------------------------
		// variable length string to int (+repositioning)
		//---------------------------------------------------------------
		//# TO LOOK AFTER CAREFULLY <.<
		private int _readVarLen(String str,ref int pos)
		{
			int value;
			int c;
    
			value = (byte)(str[pos++]);
			if ( ( value & 0x80 ) != 0)
			{
				value &= 0x7F;
				do 
				{
					c = (byte)(str[pos++]);
					value = (value << 7) + (c & 0x7F);
				} while ((c & 0x80) != 0);
			}
			return(value);
		}

		//---------------------------------------------------------------
		// int to variable length string
		//---------------------------------------------------------------
		/* NOT USED SO FAR
		function _writeVarLen($value){
			$buf = $value & 0x7F;
			$str='';
			while (($value >>= 7)){
			  $buf <<= 8;
			  $buf |= (($value & 0x7F) | 0x80);
			}
			while (TRUE){
				$str.=chr($buf%256);
				if ($buf & 0x80) $buf >>= 8;
				else break;
			}
			return $str;
		}
		*/
		//---------------------------------------------------------------
		// converts all delta times in track to absolute times
		//---------------------------------------------------------------
		/* NOT USED SO FAR
		function _delta2Absolute($track){
			$mc = count($track);
			$last = 0;
			for ($i=0;$i<$mc;$i++){
				$msg = explode(' ',$track[$i]);
				$t = $last + (int) $msg[0];
				$msg[0] = $t;
				$track[$i] = implode(' ',$msg);
				$last = $t;
			}
			return $track;
		}
		*/
	} // END OF CLASS
}