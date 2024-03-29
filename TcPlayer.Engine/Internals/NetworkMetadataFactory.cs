﻿// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using ManagedBass.Tags;
using System;
using System.Collections.Generic;
using TcPlayer.Engine.Models;

namespace TcPlayer.Engine.Internals
{
#pragma warning disable S1121 // Assignments should not be made from within sub-expressions
    internal static class NetworkMetadataFactory
    {

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

        internal static Metadata CreateFromStream(string url, string? meta)
        {
            if (string.IsNullOrEmpty(meta))
                return CreateDefault(url);

            var metaData = StreamTagsParser.GetTags(meta);

            return CreateDefault(url) with
            {
                Title = metaData.GetKey(StreamTagsParser.TagTitle),
                CoverUrl = metaData.GetKey(StreamTagsParser.TagUrl),
            };

        }

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

        internal static Metadata CreateFromYoutube(YoutubeDlResponse youtubeDlResponse)
        {
            return CreateDefault(youtubeDlResponse.YoutubeUrl) with
            {
                Title = youtubeDlResponse.Title,
                CoverUrl = youtubeDlResponse.Thumbnail,
            };
        }
    }
#pragma warning restore S1121 // Assignments should not be made from within sub-expressions
}
