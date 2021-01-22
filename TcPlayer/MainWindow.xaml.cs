﻿using Microsoft.Win32;
using System;
using System.Threading;
using System.Windows;
using TcPlayer.Engine;
using TcPlayer.Engine.Messages;
using TcPlayer.Engine.Models;
using TcPlayer.Engine.Ui;
using TcPlayer.Infrastructure;

namespace TcPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window, IMessageClient<PositionInfo>, IDialogProvider, IDisposable
    {
        private readonly IMessenger _messenger;

        public MainWindow(IMessenger messenger)
        {
            InitializeComponent();
            _messenger = messenger;
            _messenger.SubScribe(this);
        }

        public Guid MessageReciverID => Guid.NewGuid();

        public void HandleMessage(PositionInfo message)
        {
            switch (message.State)
            {
                case EngineState.ReadyToPlay:
                case EngineState.NoFile:
                    TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
                    break;
                case EngineState.Playing:
                    TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
                    break;
                case EngineState.Paused:
                    TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Paused;
                    break;
            }
            TaskbarItemInfo.ProgressValue = message.Percent;
        }

        public bool TrySelectFileDialog(string filters, out string selectedFile)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = filters,
                Multiselect = false,
                CheckFileExists = true
            };
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
    }
}
