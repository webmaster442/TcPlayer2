namespace TcPlayer.Engine.Internals.Mp4Chapters
{
    internal struct MoovInfo
    {
        public uint TimeUnitPerSecond { get; set; }

        public TrakInfo[] Tracks { get; set; }
    }
}
