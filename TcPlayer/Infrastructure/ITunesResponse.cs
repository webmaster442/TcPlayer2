﻿// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TcPlayer.Infrastructure
{
    public record Result
    {
        [JsonPropertyName("wrapperType")]
        public string WrapperType { get; set; }

        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("artistId")]
        public int ArtistId { get; set; }

        [JsonPropertyName("collectionId")]
        public int CollectionId { get; set; }
        
        [JsonPropertyName("trackId")]
        public int TrackId { get; set; }

        [JsonPropertyName("artistName")]
        public string ArtistName { get; set; }

        [JsonPropertyName("collectionName")]
        public string CollectionName { get; set; }
        
        [JsonPropertyName("trackName")]
        public string TrackName { get; set; }

        [JsonPropertyName("collectionCensoredName")]
        public string CollectionCensoredName { get; set; }

        [JsonPropertyName("trackCensoredName")]
        public string TrackCensoredName { get; set; }

        [JsonPropertyName("artistViewUrl")]
        public string ArtistViewUrl { get; set; }

        [JsonPropertyName("collectionViewUrl")]
        public string CollectionViewUrl { get; set; }

        [JsonPropertyName("trackViewUrl")]
        public string TrackViewUrl { get; set; }

        [JsonPropertyName("previewUrl")]
        public string PreviewUrl { get; set; }

        [JsonPropertyName("artworkUrl30")]
        public string ArtworkUrl30 { get; set; }

        [JsonPropertyName("artworkUrl60")]
        public string ArtworkUrl60 { get; set; }

        [JsonPropertyName("artworkUrl100")]
        public string ArtworkUrl100 { get; set; }

        [JsonPropertyName("collectionPrice")]
        public double CollectionPrice { get; set; }

        [JsonPropertyName("trackPrice")]
        public double TrackPrice { get; set; }

        [JsonPropertyName("releaseDate")]
        public DateTime ReleaseDate { get; set; }

        [JsonPropertyName("collectionExplicitness")]
        public string CollectionExplicitness { get; set; }

        [JsonPropertyName("trackExplicitness")]
        public string TrackExplicitness { get; set; }

        [JsonPropertyName("discCount")]
        public int DiscCount { get; set; }

        [JsonPropertyName("discNumber")]
        public int DiscNumber { get; set; }

        [JsonPropertyName("trackCount")]
        public int TrackCount { get; set; }

        [JsonPropertyName("trackNumber")]
        public int TrackNumber { get; set; }

        [JsonPropertyName("trackTimeMillis")]
        public int TrackTimeMillis { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("primaryGenreName")]
        public string PrimaryGenreName { get; set; }

        [JsonPropertyName("isStreamable")]
        public bool IsStreamable { get; set; }
    }

    public record RootObject
    {
        [JsonPropertyName("resultCount")]
        public int ResultCount { get; set; }

        [JsonPropertyName("results")]
        public List<Result> Results { get; set; }
    }
}
