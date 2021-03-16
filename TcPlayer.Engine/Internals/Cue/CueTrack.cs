// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

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

        public CueAudioFile DataFile { get; set; }

        public List<CueIndex> Indices { get;  set; }

        public string Performer { get;  set; }

        public string Title { get;  set; }

        public int TrackNumber { get; set; }

        public CueTrack(int tracknumber, string datatype)
        {
            TrackNumber = tracknumber;
            Title = "";
            Performer = "";
            Indices = new List<CueIndex>();
            DataFile = new CueAudioFile();
        }

        public void AddIndex(int number, int minutes, int seconds, int frames)
        {
            Indices.Add(new CueIndex(number, minutes, seconds, frames));
        }
    }
}