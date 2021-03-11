using System;

namespace TcPlayer.Engine.Messages
{
    public record CoverImageChangeMessage
    {
        public byte[] CoverData { get; init; }

        public CoverImageChangeMessage()
        {
            CoverData = Array.Empty<byte>();
        }
    }
}
