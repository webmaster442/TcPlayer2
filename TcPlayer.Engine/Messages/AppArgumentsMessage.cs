using System;

namespace TcPlayer.Engine.Messages
{
    public record AppArgumentsMessage
    {
        public string[] Arguments { get; init; }

        public AppArgumentsMessage()
        {
            Arguments = Array.Empty<string>();
        }
    }
}
