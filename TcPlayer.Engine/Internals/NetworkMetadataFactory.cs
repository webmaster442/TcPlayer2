using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TcPlayer.Engine.Models;

namespace TcPlayer.Engine.Internals
{
    internal class NetworkMetadataFactory
    {
        private static Regex StreamTitle = new Regex(@"StreamTitle=\'(.+)\'");

        public static Metadata CreateFromStream(string url, string? meta)
        {
            if (!string.IsNullOrEmpty(meta))
            {
                if (StreamTitle.IsMatch(meta))
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
                return new Metadata
                {
                    Chapters = CreateNoChapters(),
                    MediaKind = MediaKind.Stream,
                    AdditionalMeta = new List<string>
                    {
                        url,
                    }
                };
            }

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
                AdditionalMeta = new List<string>
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
                AdditionalMeta = new List<string>
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
    }
}
