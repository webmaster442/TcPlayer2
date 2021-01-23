using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace TcPlayer.BassLibs
{
    public static class BassDllVerify
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
            Assembly? assembly = Assembly.GetAssembly(typeof(BassDllVerify));
            using (var stream = assembly?.GetManifestResourceStream("TcPlayer.BassLibs.TcPlayer.BassLibs.sha512"))
            {
                string? line;
                if (stream == null) return null;
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
    }
}
