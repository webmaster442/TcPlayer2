using System;
using System.Collections.Generic;
using System.Linq;

namespace TcPlayer.Engine
{
    public record Metadata
    {
        public string Artist { get; init; }
        public string Title { get; init; }
        public string Album { get; init; }
        public string Year { get; init; }
        public byte[] Cover { get; init; }
        public IEnumerable<ChapterInfo> Chapters { get; init; }

        internal Metadata()
        {
            Album = string.Empty;
            Artist = string.Empty;
            Chapters = Enumerable.Empty<ChapterInfo>();
            Cover = Array.Empty<byte>();
            Title = string.Empty;
            Year = string.Empty;
        }
    }
}
