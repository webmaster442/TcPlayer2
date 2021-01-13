using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcPlayer.Engine.Internals
{
    internal static class MetadataFactory
    {
        public static Metadata CreateEmpty()
        {
            return new Metadata
            {
                Album = string.Empty,
                Artist = string.Empty,
                Chapters = Enumerable.Empty<ChapterInfo>(),
                Cover = Array.Empty<byte>(),
                Title = string.Empty,
                Year = string.Empty
            };
        }
    }
}
