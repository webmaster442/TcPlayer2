// Code based on CueSharp 0.5 March 24, 2007
// Original author: Wyatt O'Day wyday.com/cuesharp
// This is a heavily modified version for TCPlayer. Supports only reading

namespace TcPlayer.Engine.Internals.Cue
{
    /// <summary>
    ///DCP - Digital copy permitted
    ///4CH - Four channel audio
    ///PRE - Pre-emphasis enabled (audio tracks only)
    ///SCMS - Serial copy management system (not supported by all recorders)
    ///There is a fourth subcode flag called "DATA" which is set for all non-audio tracks. This flag is set automatically based on the datatype of the track.
    /// </summary>
    public enum Flags
    {
        DCP, CH4, PRE, SCMS, DATA, NONE
    }
}