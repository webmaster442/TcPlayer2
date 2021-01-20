using System;
using System.Collections.Generic;
using System.Linq;
using TagLib;

namespace TcPlayer.Engine.Internals
{
    internal static class MetadataFactory
    {
        public static Metadata CreateEmpty()
        {
            return new Metadata();
        }

        public static Metadata CreateFromFile(string filePath)
        {
            using (TagLib.File tags = TagLib.File.Create(filePath))
            {
                return new Metadata
                {
                    Year = tags.Tag.Year.ToString(),
                    Artist = tags.Tag.Performers?.Length > 0 ? tags.Tag.Performers[0] : string.Empty,
                    Album = tags.Tag.Album,
                    Title = tags.Tag.Title,
                    Chapters = CreateChapters(filePath, tags.Properties.Duration),
                    Cover = ExtractCover(tags.Tag.Pictures),
                };
            }
        }

        private static byte[] ExtractCover(IPicture[] pictures)
        {
            if (pictures.Length < 1)
                return Array.Empty<byte>();

            return pictures[0].Data.ToArray();
        }

        private static IEnumerable<ChapterInfo> CreateChapters(string filePath, TimeSpan duration)
        {
            int chapters = duration.TotalMinutes < 5 ? 10 : 20;

            for (int i = 0; i < chapters; i++)
            {
                yield return new ChapterInfo
                {
                    Name = $"Chapter {i + 1}",
                    TimeStamp = (duration.TotalSeconds / chapters) * (i + 1),
                };
            }

        }
    }
}
