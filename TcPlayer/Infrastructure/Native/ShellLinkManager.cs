// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace TcPlayer.Infrastructure.Native
{
    internal static class ShellLinkManager
    {
        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        public class ShellLink
        {
        }

        private static void CreateLink(string PathToFile,
                                  string PathToLink,
                                  string Arguments, 
                                  string Description)
        {
            var shellLink = new ShellLink() as IShellLinkW;

            Marshal.ThrowExceptionForHR(shellLink.SetDescription(Description));
            Marshal.ThrowExceptionForHR(shellLink.SetPath(PathToFile));
            Marshal.ThrowExceptionForHR(shellLink.SetArguments(Arguments));

            ((IPersistFile)shellLink).Save(PathToLink, false);

            Marshal.ReleaseComObject(shellLink);
        }

        public static void CreateLink(string file, string name, Environment.SpecialFolder folder)
        {
            string dir = Environment.GetFolderPath(folder);
            string linkFileName = Path.Combine(dir, name);
            CreateLink(file, linkFileName, string.Empty, string.Empty);
        }
    }
}
