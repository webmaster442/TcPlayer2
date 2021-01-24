namespace TcPlayer.Engine.Internals.Mp4Chapters
{
    internal struct BoxInfo
    {
        public long Offset { get; set; }
        public long BoxOffset { get; set; }
        public bool Last { get; set; }
        public AsciiType Type { get; set; }
    }
}
