// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace TcPlayer.Engine.Models
{
    public record Metadata
    {
        public byte[] Cover { get; init; }
        public IEnumerable<ChapterInfo> Chapters { get; init; }
        public string[] AdditionalMeta { get; init; }

        public MediaKind MediaKind { get; init; }

        public Metadata()
        {
            Chapters = Enumerable.Empty<ChapterInfo>();
            Cover = Array.Empty<byte>();
            AdditionalMeta = Array.Empty<string>();
            Artist = string.Empty;
            Title = string.Empty;
            Year = string.Empty;
            Album = string.Empty;
            AlbumArtist = string.Empty;
            MediaKind = MediaKind.File;
            CoverUrl = string.Empty;
        }

        public string Artist { get; init; }
        public string Title { get; init; }
        public string Year { get; init; }
        public string Album { get; init; }

        public string CoverUrl { get; init; }
        public string AlbumArtist { get; init; }

        public IEnumerable<string> MetaIfos
        {
            get
            {
                if (string.IsNullOrEmpty(Artist) && string.IsNullOrEmpty(Title))
                    yield return string.Empty;
                else if (string.IsNullOrEmpty(Artist))
                    yield return Title;
                else
                    yield return $"{Artist} - {Title}";

                yield return Album;
                yield return Year;
                foreach (var additional in AdditionalMeta)
                {
                    yield return additional;
                }
            }
        }
    }
}
