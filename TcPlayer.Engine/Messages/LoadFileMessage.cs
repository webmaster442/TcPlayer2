// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

namespace TcPlayer.Engine.Messages
{
    public record LoadFileMessage
    {
        public string File { get; init; }

        public LoadFileMessage()
        {
            File = string.Empty;
        }
    }
}
