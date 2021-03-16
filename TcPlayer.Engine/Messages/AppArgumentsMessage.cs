// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

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
