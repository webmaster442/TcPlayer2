﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagLib;
using TcPlayer.Engine.Models;

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

                var artist = tags.Tag.Performers?.Length > 0 ? tags.Tag.Performers[0] : string.Empty;
                return new Metadata
                {
                    Chapters = CreateChapters(filePath, tags.Properties.Duration),
                    Cover = ExtractCover(tags.Tag.Pictures),
                    Artist = artist,
                    Title = tags.Tag.Title,
                    Year = tags.Tag.Year.ToString(),
                    Album = tags.Tag.Album,
                    AlbumArtist = tags.Tag.FirstAlbumArtist,
                    AdditionalMeta = new List<string>
                    {
                        Path.GetFileName(filePath),
                        $"{tags.Properties.AudioBitrate} kbit, {tags.Properties.AudioSampleRate} Hz",
                    }
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
            string[] mp4 = new string[] { ".mp4", ".m4a", ".m4b" };

            if (mp4.Contains(Path.GetExtension(filePath).ToLower()))
            {
                using (var extractor = new Mp4Chapters.ChapterExtractor(System.IO.File.OpenRead(filePath)))
                {
                    var chapters = extractor.ExtractChapters().ToArray();
                    if (chapters.Any())
                    {
                        return chapters;
                    }
                    else
                    {
                        return CreateTimeBasedChapters(duration);
                    }
                }
            }

            return CreateTimeBasedChapters(duration);

        }

        private static IEnumerable<ChapterInfo> CreateTimeBasedChapters(TimeSpan duration)
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
