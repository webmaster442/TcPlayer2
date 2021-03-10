using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TcPlayer.Dialogs;
using TcPlayer.Engine.Models;
using TcPlayer.Infrastructure;
using TcPlayer.ViewModels;

namespace TcPlayer
{
    internal class DialogProvider : IDialogProvider
    {
        private static OpenFileDialog CreateSelectDialog(string filters, bool multiselect = false)
        {
            return new OpenFileDialog
            {
                Filter = filters,
                Multiselect = multiselect,
                CheckFileExists = true
            };
        }

        public void ShowYoutubeDlDialog(YoutubeDlState state)
        {
            var dialog = new NoYoutubeDlFoundDialog(state);
            dialog.ShowDialog();
        }

        public bool TryImportDlna(out IEnumerable<string> urls)
        {
            var dialog = new DlnaImportDialog();
            var vm = new DlnaViewModel();
            dialog.DataContext = vm;
            if (dialog.ShowDialog() == true)
            {
                urls = vm.GetUrls();
                return true;
            }
            else
            {
                urls = Enumerable.Empty<string>();
                return false;
            }
        }

        public bool TryImportITunes(out IEnumerable<ITunesTrack> items)
        {
            var dialog = new ITunesImportDialog();
            if (dialog.ShowDialog() == true)
            {
                items = dialog.GetItems();
                return true;
            }
            else
            {
                items = Enumerable.Empty<ITunesTrack>();
                return false;
            }
        }

        public bool TryImportUrl(out string url)
        {
            var dialog = new ImportUrlDialog();
            if (dialog.ShowDialog() == true)
            {
                url = dialog.Url;
                return true;
            }
            else
            {
                url = string.Empty;
                return false;
            }
        }

        public bool TrySaveFileDialog(string filter, out string selectedFile)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                AddExtension = true,
                Filter = filter
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                selectedFile = saveFileDialog.FileName;
                return true;
            }
            else
            {
                selectedFile = string.Empty;
                return false;
            }
        }

        public bool TrySelectFileDialog(string filters, out string selectedFile)
        {
            OpenFileDialog openFileDialog = CreateSelectDialog(filters);
            if (openFileDialog.ShowDialog() == true)
            {
                selectedFile = openFileDialog.FileName;
                return true;
            }
            else
            {
                selectedFile = string.Empty;
                return false;
            }
        }

        public bool TrySelectFilesDialog(string filters, out string[] selectedFiles)
        {
            OpenFileDialog openFileDialog = CreateSelectDialog(filters, true);
            if (openFileDialog.ShowDialog() == true)
            {
                selectedFiles = openFileDialog.FileNames;
                return true;
            }
            else
            {
                selectedFiles = Array.Empty<string>();
                return false;
            }
        }
    }
}
