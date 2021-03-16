// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using TcPlayer.Engine.Models;

namespace TcPlayer.Engine
{
    /// <summary>
    /// Interface for ITunesXmlDb
    /// </summary>
    public interface IItunesXmlDb
    {
        /// <summary>
        /// Gets All Tracks from the Database
        /// </summary>
        /// <seealso cref="Track"/>
        IEnumerable<ITunesTrack> Tracks { get; }
        /// <summary>
        /// Gets All Album names from the Database
        /// </summary>
        IEnumerable<string> Albums { get; }
        /// <summary>
        /// Gets All Artist names from the Database
        /// </summary>
        IEnumerable<string> Artists { get; }
        /// <summary>
        /// Gets All Genres from the Database
        /// </summary>
        IEnumerable<string> Genres { get; }
        /// <summary>
        /// Gets All years from the Database
        /// </summary>
        IEnumerable<string> Years { get; }
        /// <summary>
        /// Gets All playlists from the Database
        /// </summary>
        IEnumerable<string> Playlists { get; }
        /// <summary>
        /// Filter the Tracks by a criteria
        /// </summary>
        /// <param name="kind">Specifies filter kind</param>
        /// <param name="param">Specifies Filter string</param>
        /// <returns>Tracks maching the filter kind and string</returns>
        /// <seealso cref="Track"/>
        IEnumerable<ITunesTrack> Filter(ITunesFilterKind kind, string param);
        /// <summary>
        /// Gets a Playlists contents
        /// </summary>
        /// <param name="id">Plalist id</param>
        /// <returns>Tracks in the specified playlist</returns>
        /// <seealso cref="Track"/>
        IEnumerable<ITunesTrack> ReadPlaylist(string id);
    }
}
