// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

namespace TcPlayer.Engine.Internals.Mp4Chapters
{
    internal struct TrakInfo
    {
        public uint Id { get; set; }
        public string Type { get; set; }
        public long[] Samples { get; set; }
        public uint[] Durations { get; set; }
        public uint[] FrameCount { get; set; }
        public uint[] Chaps { get; set; }
        public uint TimeUnitPerSecond { get; set; }
    }
}
