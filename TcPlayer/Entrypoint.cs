// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Windows.Threading;
using TcPlayer.Dialogs;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace TcPlayer
{
    public static class Entrypoint
    {
        internal const string MutexName = @"Local/5C45E867-A91C-442D-A8D6-10753C6FDF55/TcPlayer2";

        internal static IEnumerable<string> KnownArguments 
        {
            get
            {
                yield return "/install";
            }
        }

        [STAThread]
        public static void Main(string[] arguments)
        {
            var application = new App();
            application.SetupEngineDependencies();
            application.SetupDependencies();
            application.DispatcherUnhandledException += OnUnhandledException;
            if (!application.IsInstalled || arguments.Contains("/install"))
            {
                application.ShouldShowMainWindow = false;
                application.InitializeComponent();
                var dialog = new InstallDialog();
                dialog.ShowDialog();
                application.MainWindow.Show();
                application.IsInstalled = true;
                application.Run();
            }
            else
            {
                application.ShouldShowMainWindow = true;
                application.InitializeComponent();
                application.Run();
            }
        }

        private static void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string crashlog = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TcPlayerCrashLog.txt");
            using (var file = File.CreateText(crashlog))
            {
                file.WriteLine("TC Player has crashed. Create a bug report detailing what you did and attach this file to the report.");
                file.WriteLine("To report a bug visit: https://github.com/webmaster442/TcPlayer2/issues");
                file.WriteLine("-----------------------------------------------------------------------");
                file.WriteLine(e.Exception.Message);
                file.WriteLine();
                file.WriteLine(e.Exception.StackTrace);
            }
        }
    }
}
