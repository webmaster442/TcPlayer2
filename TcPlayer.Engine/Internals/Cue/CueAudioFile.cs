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

        /// <summary>
        /// BINARY - Intel binary file (least significant byte first)
        /// MOTOROLA - Motorola binary file (most significant byte first)
        /// AIFF - Audio AIFF file
        /// WAVE - Audio WAVE file
        /// MP3 - Audio MP3 file
        /// </summary>
        public CueFileType Filetype { get; init; }

        public CueAudioFile() : this(string.Empty, string.Empty)
        {

        }

        public CueAudioFile(string filename, string filetype)
        {
            Filename = filename;

            Filetype = (filetype.Trim().ToUpper()) switch
            {
                "BINARY" => CueFileType.BINARY,
                "MOTOROLA" => CueFileType.MOTOROLA,
                "AIFF" => CueFileType.AIFF,
                "WAVE" => CueFileType.WAVE,
                "MP3" => CueFileType.MP3,
                _ => CueFileType.BINARY,
            };
        }

        public CueAudioFile(string filename, CueFileType filetype)
        {
            Filename = filename;
            Filetype = filetype;
        }
    }
}