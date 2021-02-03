using System;

namespace TcPlayer.Engine.Models
{
    public sealed record ITunesTrack
    {
        /// <summary>
        /// Track Id
        /// </summary>
        public int TrackId { get; init; }
        /// <summary>
        /// Track Title
        /// </summary>
        public string Name { get; init; }
        /// <summary>
        /// Track Artist
        /// </summary>
        public string Artist { get; init; }
        /// <summary>
        /// Track Album Artist
        /// </summary>
        public string AlbumArtist { get; init; }
        /// <summary>
        /// Track Composer
        /// </summary>
        public string Composer { get; init; }
        /// <summary>
        /// Track Album
        /// </summary>
        public string Album { get; init; }
        /// <summary>
        /// Track Genre
        /// </summary>
        public string Genre { get; init; }
        /// <summary>
        /// Track Kind
        /// </summary>
        public string Kind { get; init; }
        /// <summary>
        /// Track size in bytes
        /// </summary>
        public long Size { get; init; }
        /// <summary>
        /// Track length
        /// </summary>
        public string PlayingTime { get; init; }
        /// <summary>
        /// Track number
        /// </summary>
        public int? TrackNumber { get; init; }
        /// <summary>
        /// Track year
        /// </summary>
        public int? Year { get; init; }
        /// <summary>
        /// Last modification date
        /// </summary>
        public DateTime? DateModified { get; init; }
        /// <summary>
        /// Date added
        /// </summary>
        public DateTime? DateAdded { get; init; }
        /// <summary>
        /// Track bitrate
        /// </summary>
        public int? BitRate { get; init; }
        /// <summary>
        /// Track sample rate
        /// </summary>
        public int? SampleRate { get; init; }
        /// <summary>
        /// Play count
        /// </summary>
        public int? PlayCount { get; init; }
        /// <summary>
        /// Last play date
        /// </summary>
        public DateTime? PlayDate { get; init; }
        /// <summary>
        /// Part of compilation flag
        /// </summary>
        public bool PartOfCompilation { get; init; }
        /// <summary>
        /// File Path
        /// </summary>
        public string FilePath { get; init; }

        public ITunesTrack()
        {
            FilePath = string.Empty;
            Name = string.Empty;
            Artist = string.Empty;
            AlbumArtist = string.Empty;
            Composer = string.Empty;
            Album = string.Empty;
            Genre = string.Empty;
            Kind = string.Empty;
            PlayingTime = string.Empty;
        }
    }
}
