using ManagedBass;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using TcPlayer.Dialogs;
using TcPlayer.Engine;
using TcPlayer.Engine.Messages;
using TcPlayer.Engine.Models;
using TcPlayer.Engine.Ui;
using TcPlayer.Infrastructure;
using TcPlayer.ViewModels;

namespace TcPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window, IMessageClient<PositionInfoMessage>, IDialogProvider, IDisposable
    {
        private readonly IMessenger _messenger;

        public MainWindow(IMessenger messenger)
        {
            InitializeComponent();
            _messenger = messenger;
            _messenger.SubScribe(this);
        }

        public Guid MessageReciverID => Guid.NewGuid();

        public void HandleMessage(PositionInfoMessage message)
        {
            Dispatcher.Invoke(() =>
            {
                switch (message.State)
                {
                    case EngineState.ReadyToPlay:
                    case EngineState.NoFile:
                        TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
                        TaskbarItemInfo.ProgressValue = 0;
                        break;
                    case EngineState.Playing:

                        if (message.IsIndeterminate)
                            TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;
                        else
                            TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
                        break;
                    case EngineState.Paused:
                        TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Paused;
                        break;
                }

                if (!message.IsIndeterminate)
                    TaskbarItemInfo.ProgressValue = message.Percent;

            });
        }

        private static OpenFileDialog CreateSelectDialog(string filters, bool multiselect = false)
        {
            return new OpenFileDialog
            {
                Filter = filters,
                Multiselect = multiselect,
                CheckFileExists = true
            };
        }

        private void RemoteButtonClick(object sender, RoutedEventArgs e)
        {
            var remote = new RemoteServerDialog(_messenger);
            remote.ShowDialog();
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

        public void HideUiBlocker()
        {
            Blocker.Hide();
        }

        public CancellationTokenSource ShowUiBlocker()
        {
            return Blocker.Show();
        }

        public void Dispose()
        {
            if (Blocker != null)
            {
                Blocker.Dispose();
                Blocker = null;
            }
            if (DataContext is IDisposable datacontext)
            {
                datacontext.Dispose();
                DataContext = null;
            }
        }

        private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Bass.PluginFree(0);
            Dispose();
        }

        public void SetMainTab(MainTab tab)
        {
            Dispatcher.Invoke(() =>
            {
                MainTabs.SelectedIndex = (int)tab;
            });
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
    }
}
