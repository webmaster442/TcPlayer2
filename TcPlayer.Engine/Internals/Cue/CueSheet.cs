// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

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
        /// <summary>
        /// Returns/Sets track in this cuefile.
        /// </summary>
        /// <param name="tracknumber">The track in this cuefile.</param>
        /// <returns>Track at the tracknumber.</returns>
        public CueTrack this[int tracknumber]
        {
            get => Tracks[tracknumber];
            set => Tracks[tracknumber] = value;
        }

        /// <summary>
        /// The array of tracks on the cuesheet.
        /// </summary>
        public List<CueTrack> Tracks;

        /// <summary>
        /// Create a cue sheet from scratch.
        /// </summary>
        public CueSheet()
        {
            Tracks = new List<CueTrack>();
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
            var cueLines = tr.ReadToEnd().Split(delimiters).ToList();

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
                    case "FILE":
                        currentFile = ParseFile(file[i]);
                        break;
                    case "INDEX":
                        ParseIndex(file[i], trackOn);
                        break;
                    case "PERFORMER":
                    case "TITLE":
                        ParseString(file[i], trackOn);
                        break;
                    case "TRACK":
                        trackOn++;
                        ParseTrack(file[i]);
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

        private CueAudioFile ParseFile(string line)
        {
            line = line[line.IndexOf(' ')..].Trim();

            line = line.Substring(0, line.LastIndexOf(' ')).Trim();

            //if quotes around it, remove them.
            if (line[0] == '"')
            {
                line = line[1..line.LastIndexOf('"')];
            }

            return new CueAudioFile(line);
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

            tempString = line[line.IndexOf(' ')..].Trim();

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
        }

        private void ParseString(string line, int trackOn)
        {
            string category = line.Substring(0, line.IndexOf(' ')).ToUpper();

            line = line.Substring(line.IndexOf(' '), line.Length - line.IndexOf(' ')).Trim();

            //get rid of the quotes
            if (line[0] == '"')
            {
                line = line[1..line.LastIndexOf('"')];
            }

            switch (category)
            {
                case "PERFORMER":
                    if (trackOn != -1)
                    {
                        Tracks[trackOn].Performer = line;
                    }
                    break;
                case "TITLE":
                    if (trackOn != -1)
                    {
                        Tracks[trackOn].Title = line;
                    }
                    break;
            }
        }

        /// <summary>
        /// Parses the TRACK command. 
        /// </summary>
        /// <param name="line">The line in the cue file that contains the TRACK command.</param>
        /// <param name="trackOn">The track currently processing.</param>
        private void ParseTrack(string line)
        {
            string tempString;
            int trackNumber;

            tempString = line[line.IndexOf(' ')..].Trim();
            trackNumber = Convert.ToInt32(tempString.Substring(0, tempString.IndexOf(' ')));

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