﻿// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TcPlayer.Engine.Internals;
using TcPlayer.Engine.Models;

namespace TcPlayer.Engine
{
    /// <summary>
    /// A class for Interacting iTunes xml database 
    /// </summary>
    public class ItunesXmlDb: IItunesXmlDb
    {
        private readonly XDocument _xml;
        private List<ITunesTrack> _tracks;
        private readonly ItunesXmlDbOptions _options;

        #region ctor
        /// <summary>
        /// Load an iTunes XML File database
        /// </summary>
        /// <param name="fileLocation">full path of iTunes Music Library.xml</param>
        /// <param name="options">Parser options. If not specified default options will be used.</param>
        /// <seealso cref="ItunesXmlDbOptions"/>
        public ItunesXmlDb(string fileLocation, ItunesXmlDbOptions options)
        {
            _xml = XDocument.Load(fileLocation);
            _options = options;
            _tracks = new List<ITunesTrack>();
        }
        #endregion

        private IEnumerable<XElement> LoadTrackElements()
        {
            return from x in _xml.Descendants("dict")
                   .Descendants("dict")
                   .Descendants("dict")
                   where x.Descendants("key").Count() > 1
                   select x;
        }

        private IEnumerable<XElement> LoadPlaylists()
        {
            return from x in _xml.Descendants("dict")
                   .Descendants("array")
                   .Descendants("dict")
                   where x.Descendants("key").Count() > 1
                   select x;
        }

        /// <inheritdoc/>
        public IEnumerable<ITunesTrack> Tracks
        {
            get
            {
                if (_tracks.Count == 0)
                {
                    var query = from trackElement in LoadTrackElements()
                                select Parser.CreateTrack(trackElement, _options.ExcludeNonExistingFiles);

                    if (_options.ParalelParsingEnabled)
                    {
                        _tracks = query.Where(x => x != null).AsParallel().ToList();
                    }
                    else
                    {
                        _tracks = query.Where(x => x != null).ToList();
                    }
                }
                return _tracks;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Albums
        {
            get { return Tracks.Select(t => t.Album.Trim()).OrderBy(t => t).Distinct(); }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Artists
        {
            get { return Tracks.Select(t => t.Artist.Trim()).OrderBy(t => t).Distinct(); }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Genres
        {
            get { return Tracks.Select(t => t.Genre.Trim()).OrderBy(t => t).Distinct(); }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Years
        {
            get { return Tracks.Select(t => t.Year.ToString() ?? string.Empty).OrderBy(t => t).Distinct(); }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Playlists
        {
            get
            {
                var playlistNodes = LoadPlaylists();
                foreach (var item in playlistNodes)
                {
                    var parent = item.ParseStringValue("Parent Persistent ID");
                    if (!string.IsNullOrEmpty(parent))
                    {
                        var parentitem = playlistNodes.FirstOrDefault(i => i.ParseStringValue("Playlist Persistent ID") == parent);
                        if (parentitem != null)
                        {
                            yield return $"{parentitem.ParseStringValue("Name")} \\ {item.ParseStringValue("Name")}";
                        }
                    }
                    else
                        yield return item.ParseStringValue("Name");
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerable<ITunesTrack> Filter(ITunesFilterKind kind, string param)
        {
            switch (kind)
            {
                case ITunesFilterKind.Album:
                    return Tracks.Where(t => t.Album.Trim() == param);
                case ITunesFilterKind.Artist:
                    return Tracks.Where(t => t.Artist.Trim() == param);
                case ITunesFilterKind.Genre:
                    return Tracks.Where(t => t.Genre.Trim() == param);
                case ITunesFilterKind.Year:
                    return Tracks.Where(t => t.Year == int.Parse(param));
                case ITunesFilterKind.None:
                default:
                    return Tracks;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<ITunesTrack> ReadPlaylist(string id)
        {
            if (id.Contains(" \\ "))
            {
                var parts = id.Split('\\');
                id = parts[parts.Length - 1].Trim();
            }

            var playlistNodes = LoadPlaylists();

            var query = from node in playlistNodes
                        where node.ParseStringValue("Name") == id
                        select node.Descendants("array").Descendants("dict");

            foreach (var item in query)
            {
                foreach (var subitem in item)
                {
                    var trackid = int.Parse(subitem.ParseStringValue("Track ID"));
                    var track = Tracks.FirstOrDefault(t => t?.TrackId == trackid);
                    if (track != null)
                    {
                        yield return track;
                    }
                }
            }
        }

        #region static Helpers
        /// <summary>
        /// Return the default user specific path for iTunes Music Library.xml
        /// </summary>
        public static string UserItunesDbPath
        {
            get
            {
                var musicfolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                return System.IO.Path.Combine(musicfolder, @"iTunes\iTunes Music Library.xml");
            }
        }

        /// <summary>
        /// Returns true, if the user has a iTunes Music Library.xml at the default location
        /// </summary>
        public static bool UserHasItunesDb
        {
            get { return System.IO.File.Exists(UserItunesDbPath); }
        }
        #endregion
    }
}
