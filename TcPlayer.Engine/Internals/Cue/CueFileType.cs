// Code based on CueSharp 0.5 March 24, 2007
// Original author: Wyatt O'Day wyday.com/cuesharp
// This is a heavily modified version for TCPlayer. Supports only reading

namespace TcPlayer.Engine.Internals.Cue
{
    /// <summary>
    /// BINARY - Intel binary file (least significant byte first)
    /// MOTOROLA - Motorola binary file (most significant byte first)
    /// AIFF - Audio AIFF file
    /// WAVE - Audio WAVE file
    /// MP3 - Audio MP3 file
    /// </summary>
    public enum CueFileType
    {
        BINARY, MOTOROLA, AIFF, WAVE, MP3
    }
}