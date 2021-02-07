using ManagedBass;
using ManagedBass.Cd;
using System.Text.RegularExpressions;

namespace TcPlayer.Engine.Internals
{
    internal static class MediaLoader
    {
        private static readonly Regex _cdRegex = new Regex(@"cd:\/\/\d{1,2}\/\d{1,2}");

        public static int LoadLocalFile(string file, BassFlags flags)
        {
            if (IsCdUrl(file))
            {
                return LoadCdTrack(file, flags);
            }
            return Bass.CreateStream(file, 0, 0, flags);
        }

        public static bool IsCdUrl(string str)
        {
            return _cdRegex.IsMatch(str);
        }

        public static bool IsStream(string file)
        {
            return file.StartsWith("http://", System.StringComparison.OrdinalIgnoreCase)
                || file.StartsWith("https://", System.StringComparison.OrdinalIgnoreCase);
        }

        public static (int drive, int track) ParseCdUrl(string cdUrl)
        {
            string[] info = cdUrl.Replace("cd://", "").Split('/');

            int drive = int.Parse(info[0]);
            int track = int.Parse(info[1]);

            return (drive, track);
        }

        private static int LoadCdTrack(string cdUrl, BassFlags flags)
        {
            var (drive, track) = ParseCdUrl(cdUrl);
            return BassCd.CreateStream(drive, track - 1, flags);
        }
    }
}
