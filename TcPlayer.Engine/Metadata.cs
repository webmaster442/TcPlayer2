using System;
using System.Collections.Generic;
using System.Linq;

namespace TcPlayer.Engine
{
    public record Metadata
    {
        public byte[] Cover { get; init; }
        public IEnumerable<ChapterInfo> Chapters { get; init; }
        public IReadOnlyList<string> MetaIfos { get; init; }

        internal Metadata()
        {
            Chapters = Enumerable.Empty<ChapterInfo>();
            Cover = Array.Empty<byte>();
            MetaIfos = new List<string>();
        }
    }
}
