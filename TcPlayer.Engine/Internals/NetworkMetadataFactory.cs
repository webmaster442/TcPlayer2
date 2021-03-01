using ManagedBass.Tags;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TcPlayer.Engine.Models;

namespace TcPlayer.Engine.Internals
{
    internal class NetworkMetadataFactory
    {
        private static Regex StreamTitle = new Regex(@"StreamTitle=\'(.+)\';$");
        private static Regex StreamWithCover = new Regex(@"StreamTitle=\'(.+)\';StreamUrl=\'(.+)\';");

        public static Metadata CreateFromStream(string url, string? meta)
        {
            if (!string.IsNullOrEmpty(meta))
            {
                if (StreamWithCover.IsMatch(meta))
                {
                    return ParseFromStreamTitleTagWithCover(url, meta);
                }
                else if (StreamTitle.IsMatch(meta))
                {
                    return ParseFromStreamTitleTag(url, meta);
                }
                else
                {
                    return ParseAsUnknownTags(url, meta);
                }
            }
            else
            {
                return CreateDefault(url);
            }

        }

        private static Metadata CreateDefault(string url)
        {
            return new Metadata
            {
                Chapters = CreateNoChapters(),
                MediaKind = MediaKind.Stream,
                AdditionalMeta = new[]
                {
                        url,
                }
            };
        }

        private static Metadata ParseFromStreamTitleTagWithCover(string url, string meta)
        {
            var title = StreamWithCover.Split(meta).First(x => !string.IsNullOrEmpty(x));
            var coverUrl = StreamWithCover.Split(meta).Last(x => !string.IsNullOrEmpty(x));

            return new Metadata
            {
                Artist = string.Empty,
                Title = title,
                Chapters = CreateNoChapters(),
                MediaKind = MediaKind.Stream,
                CoverUrl = coverUrl,
                AdditionalMeta = new []
                {
                    url,
                }
            };
        }

        private static Metadata ParseFromStreamTitleTag(string url, string meta)
        {
            var title = StreamTitle.Split(meta).First(x => !string.IsNullOrEmpty(x));

            return new Metadata
            {
                Artist = string.Empty,
                Title = title,
                Chapters = CreateNoChapters(),
                MediaKind = MediaKind.Stream,
                AdditionalMeta = new []
                {
                    url,
                }
            };
        }

        private static Metadata ParseAsUnknownTags(string url, string meta)
        {
            return new Metadata
            {
                Chapters = CreateNoChapters(),
                MediaKind = MediaKind.Stream,
                AdditionalMeta = new []
                {
                    url,
                    meta,
                }
            };
        }

        private static IEnumerable<ChapterInfo> CreateNoChapters()
        {
            yield return new ChapterInfo
            {
                Name = "No chapters",
                TimeStamp = double.NaN,
            };
        }

        private const string TitleFormat = "%TITL";
        private const string ArtistFormat = "%ARTI";
        private const string AlbumFormat = "%ALBM";
        private const string YearFormat = "%YEAR";

        internal static Metadata CreateFromBassTags(int decodeChannel, string url)
        {
            return CreateDefault(url) with
            {
                Title = BassTags.Read(decodeChannel, TitleFormat),
                Artist = BassTags.Read(decodeChannel, ArtistFormat),
                Album = BassTags.Read(decodeChannel, AlbumFormat),
                Year = BassTags.Read(decodeChannel, YearFormat),
            };

        }
    }
}
