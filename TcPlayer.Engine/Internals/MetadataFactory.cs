using ManagedBass.Cd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagLib;
using TcPlayer.Engine.Internals.Cue;
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
            if (MediaLoader.IsCdUrl(filePath))
                return CreateForCd(filePath);

            using (TagLib.File tags = TagLib.File.Create(filePath))
            {

                var artist = tags.Tag.Performers?.Length > 0 ? tags.Tag.Performers[0] : string.Empty;
                return new Metadata
                {
                    Chapters = CreateChapters(filePath, tags.Properties.Duration),
                    Cover = ExtractCover(tags.Tag.Pictures),
                    Artist = artist,
                    MediaKind = MediaKind.File,
                    Title = tags.Tag.Title,
                    Year = tags.Tag.Year.ToString(),
                    Album = tags.Tag.Album,
                    AlbumArtist = tags.Tag.FirstAlbumArtist,
                    AdditionalMeta = new []
                    {
                        Path.GetFileName(filePath),
                        $"{tags.Properties.AudioBitrate} kbit, {tags.Properties.AudioSampleRate} Hz",
                    }
                };
            }
        }

        private static Metadata CreateForCd(string filePath)
        {
            var (drive, track) = MediaLoader.ParseCdUrl(filePath);

            return new Metadata
            {
                Chapters = CreateChapters(filePath, TimeSpan.FromSeconds(BassCd.GetTrackLength(drive, track))),
                Cover = Array.Empty<byte>(),
                MediaKind = MediaKind.Cd,
                Artist = AudioCd.GetPerformer(track),
                Title = AudioCd.GetTitle(track),
                Album = AudioCd.GetAlbum(),
                AlbumArtist = string.Empty,
                Year = string.Empty,
                AdditionalMeta = new []
                {
                    $"Track #{track}",
                    "1411 kbit, 44100 Hz"
                }
            };
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
            string cueFile = Path.ChangeExtension(filePath, "cue");

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
            else if (System.IO.File.Exists(cueFile))
            {
                return CreateChaptersFromCue(cueFile);
            }

            return CreateTimeBasedChapters(duration);

        }

        private static IEnumerable<ChapterInfo> CreateChaptersFromCue(string cueFile)
        {
            CueSheet cueSheet = new CueSheet(cueFile);
            foreach (var track in cueSheet.Tracks)
            {
                yield return new ChapterInfo
                {
                    Name = $"{track.Performer} -  {track.Title}",
                    TimeStamp = track.Indices.First().ToDouble()
                };
            }
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
