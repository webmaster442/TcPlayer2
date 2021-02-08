using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TcPlayer.Engine.Models;

namespace TcPlayer.Engine.Internals
{
    internal class YoutubeDlInterop
    {
        private const string TitleAndDurationCommand = " -f 251 --get-title --get-duration";
        private const string URlCommand = "-f 251 --get url";
        private readonly string _youtubeDlPath;

        public bool IsInstalled { get; }

        public static bool IsYoutubeUrl(string url)
        {
            Regex fullPattern = new Regex(@"http(s){0,1}\:\/\/(www\.){0,1}youtube\.com\/watch\?v\=([a-zA-Z0-9]){11}");
            Regex shortPattern = new Regex(@"https\:\/\/youtu\.be\/([a-zA-Z0-9]){11}");

            return fullPattern.IsMatch(url)
                || shortPattern.IsMatch(url);
        }

        public YoutubeDlInterop()
        {
            string? pathVariable = Environment.GetEnvironmentVariable("Path");
            List<string> directories = pathVariable?.Split(';').ToList() ?? new List<string>();
            directories.Add(AppContext.BaseDirectory);

            _youtubeDlPath = string.Empty;
            IsInstalled = false;

            foreach (var directory in directories)
            {
                var file = Path.Combine(directory, "youtube-dl.exe");
                if (File.Exists(file))
                {
                    _youtubeDlPath = file;
                    IsInstalled = true;
                    break;
                }
            }
        }

        public async Task<PlaylistItem> ExtractTitleAndDuration(string url)
        {
            var processOutput = await AsyncProcess.ExecuteShellCommand(_youtubeDlPath, $"{TitleAndDurationCommand} {url}");
            if (processOutput.Completed && processOutput.ExitCode > -1)
            {
                var lines = processOutput.Output.Split('\n');
                if (lines.Length > 0)
                {
                    return new PlaylistItem
                    {
                        FilePath = url,
                        Artist = string.Empty,
                        Title = lines[0],
                        Length = TimeSpan.Parse(lines[1]).TotalSeconds,
                    };
                }
            }
            return new PlaylistItem
            {
                FilePath = url,
                Title = url,
                Artist = string.Empty,
                Length = double.PositiveInfinity,
            };
        }

        public async Task<string> GetAudioUrl(string url)
        {
            var processOutput = await AsyncProcess.ExecuteShellCommand(_youtubeDlPath, $"{URlCommand} {url}");
            if (processOutput.Completed && processOutput.ExitCode > -1)
            {
                return processOutput.Output;
            }
            return string.Empty;
        }
    }
}
