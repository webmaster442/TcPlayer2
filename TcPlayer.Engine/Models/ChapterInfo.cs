// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

namespace TcPlayer.Engine.Models
{
    public record ChapterInfo
    {
        public string Name { get; init; }
        public double TimeStamp { get; init; }

        public ChapterInfo()
        {
            Name = string.Empty;
        }
    }
}
