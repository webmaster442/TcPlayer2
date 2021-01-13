using System.Collections.Generic;

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
    }
}
