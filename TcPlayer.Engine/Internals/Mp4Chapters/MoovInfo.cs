// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

namespace TcPlayer.Engine.Internals.Mp4Chapters
{
    internal struct MoovInfo
    {
        public uint TimeUnitPerSecond { get; set; }

        public TrakInfo[] Tracks { get; set; }
    }
}
