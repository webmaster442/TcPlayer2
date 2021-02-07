using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TcPlayer.Engine.Internals;
using TcPlayer.Engine.Models;

namespace TcPlayer.Engine
{
    public static class PlaylistItemFactory
    {
        private static TextReader? LoadFile(string file)
        {
            if (file.StartsWith("http://") || file.StartsWith("https://"))
            {
                try
                {
                    using var client = new System.Net.WebClient();
                    var response = client.DownloadString(new Uri(file));
                    return new StringReader(response);
                }
                catch (WebException)
                {
                    return null;
                }
            }
            else
            {
                return System.IO.File.OpenText(file);
            }
        }

        public static Task<IEnumerable<PlaylistItem>> LoadCd(int driveIndex, CancellationToken cancellationToken)
        {
            return AudioCd.LoadCd(driveIndex, cancellationToken);
        }

        public static Task<IEnumerable<PlaylistItem>> CreateFileInfos(IEnumerable<string> items, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var bag = new ConcurrentBag<PlaylistItem>();

                ParallelOptions po = new ParallelOptions();
                po.CancellationToken = cancellationToken;

                Parallel.ForEach(items, po, item =>
                {
                    if (MediaLoader.IsStream(item))
                    {
                        bag.Add(new PlaylistItem
                        {
                            FilePath = item,
                            Artist = string.Empty,
                            Title = item,
                            Length = double.NaN,
                        });
                    }
                    else
                    {

                        TagLib.File f = TagLib.File.Create(item);
                        var artist = f.Tag.Performers?.Length > 0 ? f.Tag.Performers[0] : string.Empty;
                        bag.Add(new PlaylistItem
                        {
                            FilePath = item,
                            Artist = artist,
                            Title = f.Tag.Title,
                            Length = f.Properties.Duration.TotalSeconds,
                        });
                    }
                });

                return bag as IEnumerable<PlaylistItem>;
            });
        }

        public async static Task<IEnumerable<PlaylistItem>> CreateFromM3UFile(string file, CancellationToken cancellationToken)
        {
            List<string> ret = new List<string>();
            string filedir = Path.GetDirectoryName(file) ?? "";
            string? line;
            using (var content = LoadFile(file))
            {
                do
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    line = content?.ReadLine();
                    if (line == null || line.StartsWith("#")) continue;
                    if (line.StartsWith("http://") || line.StartsWith("https://"))
                    {
                        ret.Add(line);
                    }
                    else if (line.Contains(":\\") || line.StartsWith("\\\\"))
                    {
                        if (!System.IO.File.Exists(line)) continue;
                        ret.Add(line);
                    }
                    else
                    {
                        string f = Path.Combine(filedir, line);
                        if (!System.IO.File.Exists(f)) continue;
                        ret.Add(f);
                    }
                }
                while (line != null);
            }
            var items = await CreateFileInfos(ret, cancellationToken);
            return items;
        }

        public async static Task<IEnumerable<PlaylistItem>> CreateFromPlsFile(string file, CancellationToken cancellationToken)
        {
            List<string> ret = new List<string>();
            string filedir = Path.GetDirectoryName(file) ?? "";
            string? line;
            string pattern = @"^(File)([0-9])+(=)";
            using (var content = LoadFile(file))
            {
                do
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    line = content?.ReadLine();
                    if (line == null) continue;
                    if (Regex.IsMatch(line, pattern)) line = Regex.Replace(line, pattern, "");
                    else continue;
                    if (line.StartsWith("http://") || line.StartsWith("https://"))
                    {
                        ret.Add(line);
                    }
                    else if (line.Contains(":\\") || line.StartsWith("\\\\"))
                    {
                        if (!System.IO.File.Exists(line)) continue;
                        ret.Add(line);
                    }
                    else
                    {
                        string f = Path.Combine(filedir, line);
                        if (!System.IO.File.Exists(f)) continue;
                        ret.Add(f);
                    }
                }
                while (line != null);
            }
            var items = await CreateFileInfos(ret, cancellationToken);
            return items;
        }

        public async static Task<IEnumerable<PlaylistItem>> CreateFromAsxFile(string file, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var content = LoadFile(file);
            List<string> ret = new List<string>();
            if (content != null)
            {
                var doc = XDocument.Load(content).Descendants("asx").Elements("entry").Elements("ref").ToList();
                foreach (var media in doc)
                {

                    cancellationToken.ThrowIfCancellationRequested();
                    var src = media.Attribute("href")?.Value ?? string.Empty;
                    ret.Add(src);
                }
                var items = await CreateFileInfos(ret, cancellationToken);
                return items;
            }
            return Enumerable.Empty<PlaylistItem>();
        }

        public async static Task<IEnumerable<PlaylistItem>> CreateFromWplFile(string file, CancellationToken cancellationToken)
        {
            using var content = LoadFile(file);
            if (content != null)
            {
                var doc = XDocument.Load(content).Descendants("body").Elements("seq").Elements("media").ToList();
                List<string> ret = new List<string>(doc.Count);
                foreach (var media in doc)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var src = media.Attribute("src")?.Value ?? string.Empty;
                    ret.Add(src);
                }
                var items = await CreateFileInfos(ret, cancellationToken);
                return items;
            }
            return Enumerable.Empty<PlaylistItem>();
        }

        public static IEnumerable<PlaylistItem> CreateFromITunesTracks(IEnumerable<ITunesTrack> tunesTracks)
        {
            return tunesTracks.AsParallel().Select(x => new PlaylistItem
            {
                Artist = x.Artist,
                FilePath = x.FilePath,
                Length = x.PlayingTime,
                Title = x.Name
            });
        }
    }
}
