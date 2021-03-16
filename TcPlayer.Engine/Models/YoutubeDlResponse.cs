// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

namespace TcPlayer.Engine.Models
{
    public record YoutubeDlResponse
    {
        public string Title { get; init; }
        public string Url { get; init; }
        public string Thumbnail { get; init; }
        public string YoutubeUrl { get; init; }

        public YoutubeDlResponse()
        {
            Title = string.Empty;
            Url = string.Empty;
            Thumbnail = string.Empty;
            YoutubeUrl = string.Empty;
        }

        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(Title)
                    && string.IsNullOrEmpty(Url)
                    && string.IsNullOrEmpty(Thumbnail)
                    && string.IsNullOrEmpty(YoutubeUrl);
            }
        }
    }
}
