// Code based on CueSharp 0.5 March 24, 2007
// Original author: Wyatt O'Day wyday.com/cuesharp
// This is a heavily modified version for TCPlayer. Supports only reading

using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace TcPlayer.Engine.Internals.Cue
{
    /// <summary>
    /// A CueSheet class used to create, open, edit, and save cuesheets.
    /// </summary>
    internal class CueSheet
    {
        private List<string> cueLines;

        // strings that don't belong or were mistyped in the global part of the cue
        private List<string> m_Garbage;

        /// <summary>
        /// Returns/Sets track in this cuefile.
        /// </summary>
        /// <param name="tracknumber">The track in this cuefile.</param>
        /// <returns>Track at the tracknumber.</returns>
        public CueTrack this[int tracknumber]
        {
            get
            {
                return Tracks[tracknumber];
            }
            set
            {
                Tracks[tracknumber] = value;
            }
        }


        /// <summary>
        /// The catalog number must be 13 digits long and is encoded according to UPC/EAN rules.
        /// Example: CATALOG 1234567890123
        /// </summary>
        public string Catalog { get; set; } = "";

        /// <summary>
        /// This command is used to specify the name of the file that contains the encoded CD-TEXT information for the disc. This command is only used with files that were either created with the graphical CD-TEXT editor or generated automatically by the software when copying a CD-TEXT enhanced disc.
        /// </summary>
        public string CDTextFile { get; set; } = "";

        /// <summary>
        /// This command is used to put comments in your CUE SHEET file.
        /// </summary>
        public List<string> Comments { get; set; }

        /// <summary>
        /// Lines in the cue file that don't belong or have other general syntax errors.
        /// </summary>
        public IReadOnlyList<string> Garbage
        {
            get { return m_Garbage; }
        }

        /// <summary>
        /// This command is used to specify the name of a perfomer for a CD-TEXT enhanced disc.
        /// </summary>
        public string Performer { get; set; } = "";

        /// <summary>
        /// This command is used to specify the name of a songwriter for a CD-TEXT enhanced disc.
        /// </summary>
        public string Songwriter { get; set; } = "";

        /// <summary>
        /// The title of the entire disc as a whole.
        /// </summary>
        public string Title { get; set; } = "";

        /// <summary>
        /// The array of tracks on the cuesheet.
        /// </summary>
        public List<CueTrack> Tracks;

        /// <summary>
        /// Create a cue sheet from scratch.
        /// </summary>
        public CueSheet()
        {
            cueLines = new List<string>();
            m_Garbage = new List<string>();
            Tracks = new List<CueTrack>();
            Comments = new List<string>();
        }

        /// <summary>
        /// Parse a cue sheet string.
        /// </summary>
        /// <param name="cueString">A string containing the cue sheet data.</param>
        /// <param name="lineDelims">Line delimeters; set to "(char[])null" for default delimeters.</param>
        public CueSheet(string cueString, char[] lineDelims): this()
        {
            if (lineDelims == null)
            {
                lineDelims = new char[] { '\n' };
            }

            cueLines = cueString.Split(lineDelims).ToList();
            RemoveEmptyLines(ref cueLines);
            ParseCue(cueLines);
        }

        /// <summary>
        /// Parses a cue sheet file.
        /// </summary>
        /// <param name="cuefilename">The filename for the cue sheet to open.</param>
        public CueSheet(string cuefilename): this()
        {
            ReadCueSheet(cuefilename, Encoding.Default);
        }

        /// <summary>
        /// Parses a cue sheet file.
        /// </summary>
        /// <param name="cuefilename">The filename for the cue sheet to open.</param>
        /// <param name="encoding">The encoding used to open the file.</param>
        public CueSheet(string cuefilename, Encoding encoding) : this()
        {
            ReadCueSheet(cuefilename, encoding);
        }

        private void ReadCueSheet(string filename, Encoding encoding)
        {
            // array of delimiters to split the sentence with
            char[] delimiters = new char[] { '\n' };
            
            // read in the full cue file
            TextReader tr = new StreamReader(filename, encoding);
            //read in file
            cueLines = tr.ReadToEnd().Split(delimiters).ToList();

            // close the stream
            tr.Close();

            RemoveEmptyLines(ref cueLines);

            ParseCue(cueLines);
        }

        /// <summary>
        /// Removes any empty lines, elimating possible trouble.
        /// </summary>
        /// <param name="file"></param>
        private void RemoveEmptyLines(ref List<string> file)
        {
            file = file.Where(x => !string.IsNullOrEmpty(x.Trim())).ToList();
        }

        private void ParseCue(List<string> file)
        {
            //-1 means still global, 
            //all others are track specific
            int trackOn = -1;
            CueAudioFile currentFile = new CueAudioFile();

            for (int i = 0; i < file.Count; i++)
            {
                file[i] = file[i].Trim();

                switch (file[i].Substring(0, file[i].IndexOf(' ')).ToUpper())
                {
                    case "CATALOG":
                        ParseString(file[i], trackOn);
                        break;
                    case "CDTEXTFILE":
                        ParseString(file[i], trackOn);
                        break;
                    case "FILE":
                        currentFile = ParseFile(file[i], trackOn);
                        break;
                    case "FLAGS":
                        ParseFlags(file[i], trackOn);
                        break;
                    case "INDEX":
                        ParseIndex(file[i], trackOn);
                        break;
                    case "ISRC":
                        ParseString(file[i], trackOn);
                        break;
                    case "PERFORMER":
                        ParseString(file[i], trackOn);
                        break;
                    case "POSTGAP":
                        ParseIndex(file[i], trackOn);
                        break;
                    case "PREGAP":
                        ParseIndex(file[i], trackOn);
                        break;
                    case "REM":
                        ParseComment(file[i], trackOn);
                        break;
                    case "SONGWRITER":
                        ParseString(file[i], trackOn);
                        break;
                    case "TITLE":
                        ParseString(file[i], trackOn);
                        break;
                    case "TRACK":
                        trackOn++;
                        ParseTrack(file[i], trackOn);
                        if (currentFile.Filename != "") //if there's a file
                        {
                            Tracks[trackOn].DataFile = currentFile;
                            currentFile = new CueAudioFile();
                        }
                        break;
                    default:
                        break;
                }
            }

        }

        private void ParseComment(string line, int trackOn)
        {
            //remove "REM" (we know the line has already been .Trim()'ed)
            line = line.Substring(line.IndexOf(' '), line.Length - line.IndexOf(' ')).Trim();

            if (trackOn == -1)
            {
                if (line.Trim() != "")
                {
                    Comments.Add(line);
                }
            }
            else
            {
                Tracks[trackOn].AddComment(line);
            }
        }

        private CueAudioFile ParseFile(string line, int trackOn)
        {
            string fileType;

            line = line.Substring(line.IndexOf(' '), line.Length - line.IndexOf(' ')).Trim();

            fileType = line.Substring(line.LastIndexOf(' '), line.Length - line.LastIndexOf(' ')).Trim();

            line = line.Substring(0, line.LastIndexOf(' ')).Trim();

            //if quotes around it, remove them.
            if (line[0] == '"')
            {
                line = line.Substring(1, line.LastIndexOf('"') - 1);
            }

            return new CueAudioFile(line, fileType);
        }

        private void ParseFlags(string line, int trackOn)
        {
            string temp;

            if (trackOn != -1)
            {
                line = line.Trim();
                if (line != "")
                {
                    try
                    {
                        temp = line.Substring(0, line.IndexOf(' ')).ToUpper();
                    }
                    catch (Exception)
                    {
                        temp = line.ToUpper();
                        
                    }

                    switch (temp)
                    {
                        case "FLAGS":
                            Tracks[trackOn].AddFlag(temp);
                            break;
                        case "DATA":
                            Tracks[trackOn].AddFlag(temp);
                            break;
                        case "DCP":
                            Tracks[trackOn].AddFlag(temp);
                            break;
                        case "4CH":
                            Tracks[trackOn].AddFlag(temp);
                            break;
                        case "PRE":
                            Tracks[trackOn].AddFlag(temp);
                            break;
                        case "SCMS":
                            Tracks[trackOn].AddFlag(temp);
                            break;
                        default:
                            break;
                    }

                    //processing for a case when there isn't any more spaces
                    //i.e. avoiding the "index cannot be less than zero" error
                    //when calling line.IndexOf(' ')
                    try
                    {
                        temp = line.Substring(line.IndexOf(' '), line.Length - line.IndexOf(' '));
                    }
                    catch (Exception)
                    {
                        temp = line.Substring(0, line.Length);
                    }

                    //if the flag hasn't already been processed
                    if (temp.ToUpper().Trim() != line.ToUpper().Trim())
                    {
                        ParseFlags(temp, trackOn);
                    }
                }
            }
        }

        private void ParseIndex(string line, int trackOn)
        {
            string indexType;
            string tempString;

            int number = 0;
            int minutes;
            int seconds;
            int frames;

            indexType = line.Substring(0, line.IndexOf(' ')).ToUpper();

            tempString = line.Substring(line.IndexOf(' '), line.Length - line.IndexOf(' ')).Trim();

            if (indexType == "INDEX")
            {
                //read the index number
                number = Convert.ToInt32(tempString.Substring(0, tempString.IndexOf(' ')));
                tempString = tempString.Substring(tempString.IndexOf(' '), tempString.Length - tempString.IndexOf(' ')).Trim();
            }

            //extract the minutes, seconds, and frames
            minutes = Convert.ToInt32(tempString.Substring(0, tempString.IndexOf(':')));
            seconds = Convert.ToInt32(tempString.Substring(tempString.IndexOf(':') + 1, tempString.LastIndexOf(':') - tempString.IndexOf(':') - 1));
            frames = Convert.ToInt32(tempString.Substring(tempString.LastIndexOf(':') + 1, tempString.Length - tempString.LastIndexOf(':') - 1));

            if (indexType == "INDEX")
            {
                Tracks[trackOn].AddIndex(number, minutes, seconds, frames);
            }
            else if (indexType == "PREGAP")
            {
                Tracks[trackOn].PreGap = new CueIndex(0, minutes, seconds, frames);
            }
            else if (indexType == "POSTGAP")
            {
                Tracks[trackOn].PostGap = new CueIndex(0, minutes, seconds, frames);
            }
        }

        private void ParseString(string line, int trackOn)
        {
            string category = line.Substring(0, line.IndexOf(' ')).ToUpper();

            line = line.Substring(line.IndexOf(' '), line.Length - line.IndexOf(' ')).Trim();

            //get rid of the quotes
            if (line[0] == '"')
            {
                line = line.Substring(1, line.LastIndexOf('"') - 1);
            }

            switch (category)
            {
                case "CATALOG":
                    if (trackOn == -1)
                    {
                        this.Catalog = line;
                    }
                    break;
                case "CDTEXTFILE":
                    if (trackOn == -1)
                    {
                        this.CDTextFile = line;
                    }
                    break;
                case "ISRC":
                    if (trackOn != -1)
                    {
                        Tracks[trackOn].ISRC = line;
                    }
                    break;
                case "PERFORMER":
                    if (trackOn == -1)
                    {
                        this.Performer = line;
                    }
                    else
                    {
                        Tracks[trackOn].Performer = line;
                    }
                    break;
               case "SONGWRITER":
                   if (trackOn == -1)
                   {
                       this.Songwriter = line;
                   }
                   else
                   {
                       Tracks[trackOn].Songwriter = line;
                   }
                    break;
                case "TITLE":
                    if (trackOn == -1)
                    {
                        this.Title = line;
                    }
                    else
                    {
                        Tracks[trackOn].Title = line;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Parses the TRACK command. 
        /// </summary>
        /// <param name="line">The line in the cue file that contains the TRACK command.</param>
        /// <param name="trackOn">The track currently processing.</param>
        private void ParseTrack(string line, int trackOn)
        {
            string tempString;
            int trackNumber;

            tempString = line.Substring(line.IndexOf(' '), line.Length - line.IndexOf(' ')).Trim();

            try
            {
                trackNumber = Convert.ToInt32(tempString.Substring(0, tempString.IndexOf(' ')));
            }
            catch (Exception)
            { throw; }

            //find the data type.
            tempString = tempString.Substring(tempString.IndexOf(' '), tempString.Length - tempString.IndexOf(' ')).Trim();

            AddTrack(trackNumber, tempString);
        }

        /// <summary>
        /// Add a track to the current cuesheet.
        /// </summary>
        /// <param name="tracknumber">The number of the said track.</param>
        /// <param name="datatype">The datatype of the track.</param>
        private void AddTrack(int tracknumber, string datatype)
        {
            Tracks.Add(new CueTrack(tracknumber, datatype));
        }
    }   
}