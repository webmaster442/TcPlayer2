using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace TcPlayer.BassLibs
{
    public static class BassLibs
    {
        private static string ComputeHash(string file)
        {
            using (var sha = SHA512.Create())
            {
                using (var fs = File.OpenRead(file))
                {
                    var result = sha.ComputeHash(fs);
                    return BitConverter.ToString(result).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        private static Dictionary<string, string>? LoadStoredHashes()
        {
            Dictionary<string, string> storedHashes = new Dictionary<string, string>();

            var currentdir = AppDomain.CurrentDomain.BaseDirectory;
            Assembly? assembly = Assembly.GetAssembly(typeof(BassLibs));
            using (var stream = assembly?.GetManifestResourceStream("TcPlayer.BassLibs.TcPlayer.BassLibs.sha512"))
            {
                string? line;
                if (stream == null) return new Dictionary<string, string>();
                using (var streamreader = new StreamReader(stream))
                {
                    while ((line = streamreader.ReadLine()) != null)
                    {
                        string[] parts = line?.Split(' ') ?? new string[1] { string.Empty };
                        var fullpath = Path.Combine(currentdir + parts[1].Replace("*", ""));
                        storedHashes.Add(fullpath, parts[0]);
                    }
                }
            }

            return storedHashes;
        }

        public static bool VerifyDllFiles()
        {
            var stored = LoadStoredHashes();
            if (stored == null) return false;
            foreach (var hash in stored)
            {
                if (!File.Exists(hash.Key)) return false;

                var expected = ComputeHash(hash.Key);

                if (string.Compare(expected, hash.Value, false) != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static IEnumerable<string> Plugins
        {
            get
            {
                var currentdir = AppDomain.CurrentDomain.BaseDirectory;
                yield return Path.Combine(currentdir, "bass_aac.dll");
                yield return Path.Combine(currentdir, "bass_ac3.dll");
                yield return Path.Combine(currentdir, "bass_ape.dll");
                yield return Path.Combine(currentdir, "bass_spx.dll");
                yield return Path.Combine(currentdir, "bassalac.dll");
                yield return Path.Combine(currentdir, "bassflac.dll");
                yield return Path.Combine(currentdir, "basswebm.dll");
                yield return Path.Combine(currentdir, "basswma.dll");
                yield return Path.Combine(currentdir, "basswv.dll");
            }
        }
    }
}
