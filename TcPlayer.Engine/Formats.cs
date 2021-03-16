// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TcPlayer.Engine
{
    public static class Formats
    {
        public const string NativePlaylistFilterString = "TcPlayler Playlist|*.tpls";

        public static string PlaylistFilterString
        {
            get => CreateFilterString(PlaylsitExtensions, "All playlist formats");
        }

        public static string AudioFormatFilterString
        {
            get => CreateFilterString(AudioFormats, "All audio files");
        }

        public static bool IsPLaylist(string file)
        {
            string extension = System.IO.Path.GetExtension(file);
            return
                !string.IsNullOrEmpty(extension)
                && PlaylsitExtensions.Contains(extension);
        }

        public static bool IsAudioFile(string file)
        {
            string extension = System.IO.Path.GetExtension(file);
            return
                !string.IsNullOrEmpty(extension)
                && AudioFormats.Contains(extension);
        }

        public static IEnumerable<string> PlaylsitExtensions
        {
            get
            {
                yield return ".pls";
                yield return ".m3u";
                yield return ".wpl";
                yield return ".asx";
                yield return ".tpls";
            }
        }

        public  static IEnumerable<string> AudioFormats
        {
            get
            {
                yield return ".mp1";
                yield return ".mp2";
                yield return ".mp3";
                yield return ".aiff";
                yield return ".wav";
                yield return ".ogg";
                yield return ".aac";
                yield return ".mp4";
                yield return ".m4a";
                yield return ".m4b";
                yield return ".ac3";
                yield return ".ape";
                yield return ".spx";
                yield return ".flac";
                yield return ".webm";
                yield return ".wma";
                yield return ".wv";
            }
        }

        private static string CreateFilterString(IEnumerable<string> items, string allName)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0}|{1}", allName, string.Join(";*", items));
            foreach (var item in items)
            {
                builder.AppendFormat("|*{0}|*{0};", item);
            }
            return builder.ToString();
        }
    }
}
