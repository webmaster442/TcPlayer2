// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace TcPlayer.Controls
{
    /// <summary>
    /// Interaction logic for UIBlocker.xaml
    /// </summary>
    public sealed partial class UIBlocker : UserControl, IDisposable
    {
        private CancellationTokenSource _currentToken;

        public UIBlocker()
        {
            InitializeComponent();
        }

        public CancellationTokenSource Show()
        {
            ProgressBar.IsIndeterminate = true;
            Visibility = Visibility.Visible;
            Dispose();
            _currentToken = new CancellationTokenSource();
            return _currentToken;
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;
            ProgressBar.IsIndeterminate = false;
        }

        public void Dispose()
        {
            if (_currentToken != null)
            {
                _currentToken.Dispose();
                _currentToken = null;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _currentToken.Cancel();
        }
    }
}
