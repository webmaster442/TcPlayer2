// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using TcPlayer.Infrastructure.Native;

namespace TcPlayer.Dialogs
{
    /// <summary>
    /// Interaction logic for Install.xaml
    /// </summary>
    public partial class InstallDialog
    {
        private const string _name = "Tc Player";
        private readonly string _programPath;
        private const string _website = "https://github.com/webmaster442/TcPlayer2";

        public InstallDialog()
        {
            InitializeComponent();
            using (var process = Process.GetCurrentProcess())
            {
                _programPath = process.MainModule.FileName;
            }
        }

        private void OnPlaceShortcutsAndStart(object sender, RoutedEventArgs e)
        {
            if (ShortcutDesktop.IsChecked == true)
            {
                ShellLinkManager.CreateLink(_programPath, _name, Environment.SpecialFolder.Desktop);
            }
            if (ShortcutStartMenu.IsChecked == true)
            {
                ShellLinkManager.CreateLink(_programPath, _name, Environment.SpecialFolder.StartMenu);
            }
            DialogResult = true;
        }

        private void OnViewWebsite(object sender, RoutedEventArgs e)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = _website;
                process.StartInfo.UseShellExecute = true;
                process.Start();
            }
        }
    }
}
