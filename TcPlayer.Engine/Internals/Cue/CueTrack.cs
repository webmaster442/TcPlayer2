// Code based on CueSharp 0.5 March 24, 2007
// Original author: Wyatt O'Day wyday.com/cuesharp
// This is a heavily modified version for TCPlayer. Supports only reading

using System.Collections.Generic;

namespace TcPlayer.Engine.Internals.Cue
{
    /// <summary>
    /// Track that contains either data or audio. It can contain Indices and comment information.
    /// </summary>
    internal class CueTrack
    {
        /// <summary>
        /// Returns/Sets Index in this track.
        /// </summary>
        /// <param name="indexnumber">Index in the track.</param>
        /// <returns>Index at indexnumber.</returns>
        public CueIndex this[int indexnumber]
        {
            get => Indices[indexnumber];
            set => Indices[indexnumber] = value;
        }

        public List<string> Comments { get; set; }

        public CueAudioFile DataFile { get; set; }

        public List<CueIndex> Indices { get;  set; }

        public string ISRC { get;  set; }

        public string Performer { get;  set; }

        public CueIndex PostGap { get;  set; }

        public CueIndex PreGap { get;  set; }

        public string Songwriter { get;  set; }

        /// <summary>
        /// If the TITLE command appears before any TRACK commands, then the string will be encoded as the title of the entire disc.
        /// </summary>
        public string Title { get;  set; }

        public CueDataType TrackDataType { get;  set; }

        public List<Flags> TrackFlags { get;  set; }

        public int TrackNumber { get; set; }

        public CueTrack(int tracknumber, string datatype)
        {
            TrackNumber = tracknumber;

            switch (datatype.Trim ().ToUpper ())
            {
                case "AUDIO":
                    TrackDataType = CueDataType.AUDIO;
                    break;
                case "CDG":
                    TrackDataType = CueDataType.CDG;
                    break;
                case "MODE1/2048":
                    TrackDataType = CueDataType.MODE1_2048;
                    break;
                case "MODE1/2352":
                    TrackDataType = CueDataType.MODE1_2352;
                    break;
                case "MODE2/2336":
                    TrackDataType = CueDataType.MODE2_2336;
                    break;
                case "MODE2/2352":
                    TrackDataType = CueDataType.MODE2_2352;
                    break;
                case "CDI/2336":
                    TrackDataType = CueDataType.CDI_2336;
                    break;
                case "CDI/2352":
                    TrackDataType = CueDataType.CDI_2352;
                    break;
                default:
                    TrackDataType = CueDataType.AUDIO;
                    break;
            }

            TrackFlags = new List<Flags>();
            Songwriter = "";
            Title = "";
            ISRC = "";
            Performer = "";
            Indices = new List<CueIndex>();
            Comments = new List<string>();
            PreGap = new CueIndex(-1, 0, 0, 0);
            PostGap = new CueIndex(-1, 0, 0, 0);
            DataFile = new CueAudioFile();
        }
       
        public void AddFlag(Flags flag)
        {
            //if it's not a none tag
            //and if the tags hasn't already been added
            if (flag != Flags.NONE && !TrackFlags.Contains(flag) == true)
            {
                TrackFlags.Add(flag);
            }
        }

        public void AddFlag(string flag)
        {
            switch (flag.Trim().ToUpper())
            {
                case "DATA":
                    AddFlag(Flags.DATA);
                    break;
                case "DCP":
                    AddFlag(Flags.DCP);
                    break;
                case "4CH":
                    AddFlag(Flags.CH4);
                    break;
                case "PRE":
                    AddFlag(Flags.PRE);
                    break;
                case "SCMS":
                    AddFlag(Flags.SCMS);
                    break;
                default:
                    return;
            }
        }

        public void AddComment(string comment)
        {
            if (comment.Trim() != "")
            {
                Comments.Add(comment);
            }
        }

        public void AddIndex(int number, int minutes, int seconds, int frames)
        {
            Indices.Add(new CueIndex(number, minutes, seconds, frames));
        }
    }
}