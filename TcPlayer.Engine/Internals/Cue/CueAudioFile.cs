// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

// Code based on CueSharp 0.5 March 24, 2007
// Original author: Wyatt O'Day wyday.com/cuesharp
// This is a heavily modified version for TCPlayer. Supports only reading

namespace TcPlayer.Engine.Internals.Cue
{
    /// <summary>
    /// This command is used to specify a data/audio file that will be written to the recorder.
    /// </summary>
    internal class CueAudioFile
    {
        public string Filename { get; init; }

        public CueAudioFile() : this(string.Empty)
        {

        }

        public CueAudioFile(string filename)
        {
            Filename = filename;
        }
    }
}