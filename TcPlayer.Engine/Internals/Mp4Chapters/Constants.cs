// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

namespace TcPlayer.Engine.Internals.Mp4Chapters
{
    internal static class Constants
    {
        public static readonly byte[] Tref = new byte[] { 0x74, 0x72, 0x65, 0x66 };
        public static readonly byte[] Chap = new byte[] { 0x63, 0x68, 0x61, 0x70 };
        public static readonly byte[] Mdat = new byte[] { 0x6d, 0x64, 0x61, 0x74 };
        public static readonly byte[] Moov = new byte[] { 0x6d, 0x6f, 0x6f, 0x76 };
        public static readonly byte[] Mvhd = new byte[] { 0x6d, 0x76, 0x68, 0x64 };
        public static readonly byte[] Trak = new byte[] { 0x74, 0x72, 0x61, 0x6b };
        public static readonly byte[] Tkhd = new byte[] { 0x74, 0x6b, 0x68, 0x64 };
        public static readonly byte[] Mdia = new byte[] { 0x6d, 0x64, 0x69, 0x61 };
        public static readonly byte[] Mdhd = new byte[] { 0x6d, 0x64, 0x68, 0x64 };
        public static readonly byte[] Hdlr = new byte[] { 0x68, 0x64, 0x6c, 0x72 };
        public static readonly byte[] Minf = new byte[] { 0x6d, 0x69, 0x6e, 0x66 };
        public static readonly byte[] Stbl = new byte[] { 0x73, 0x74, 0x62, 0x6c };
        public static readonly byte[] Stco = new byte[] { 0x73, 0x74, 0x63, 0x6f };
        public static readonly byte[] Stts = new byte[] { 0x73, 0x74, 0x74, 0x73 };
        public static readonly byte[] Ftyp = new byte[] { 0x66, 0x74, 0x79, 0x70 };
        
    }
}
