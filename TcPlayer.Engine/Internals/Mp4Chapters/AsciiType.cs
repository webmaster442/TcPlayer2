// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System.Text;

namespace TcPlayer.Engine.Internals.Mp4Chapters
{
    internal struct AsciiType
    {
        public byte[] Type { get; set; }

        public bool Check(byte[] refType)
        {
            return Type[0] == refType[0] &&
                   Type[1] == refType[1] &&
                   Type[2] == refType[2] &&
                   Type[3] == refType[3];
        }

        public override string ToString()
        {
            var enc = Encoding.ASCII;
            var c = new char[4];
            enc.GetDecoder().GetChars(Type, 0, 4, c, 0);
            return new string(c);
        }
    }
}
