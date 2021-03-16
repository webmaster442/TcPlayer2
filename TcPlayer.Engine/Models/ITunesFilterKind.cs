// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

namespace TcPlayer.Engine.Models
{
    public enum ITunesFilterKind
    {
        /// <summary>
        /// No filtering
        /// </summary>
        None,
        /// <summary>
        /// Filter string is an album name
        /// </summary>
        Album,
        /// <summary>
        /// Filter string is an artist name
        /// </summary>
        Artist,
        /// <summary>
        /// Filter string is a Genre
        /// </summary>
        Genre,
        /// <summary>
        /// Filter string represents a year
        /// </summary>
        Year
    }
}
