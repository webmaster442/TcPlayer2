// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System.Xml.Serialization;

namespace TcPlayer.Engine.Models
{
    public record PlaylistItem
    {
        public string FilePath { get; init; }
        [XmlAttribute]
        public string Artist { get; init; }
        [XmlAttribute]
        public string Title { get; init; }
        [XmlAttribute]
        public double Length { get; init; }

        public PlaylistItem()
        {
            FilePath = string.Empty;
            Artist = string.Empty;
            Title = string.Empty;
        }
    }
}
