using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TcPlayer.Engine.Models;
using TcPlayer.Infrastructure;
using TcPlayer.ViewModels;

namespace TcPlayer
{
    public partial class ITunesImportDialog : Window
    {
        public ITunesImportDialog()
        {
            InitializeComponent();
            SourceInitialized += ITunesImportDialog_SourceInitialized;
            DataContext = new ItunesViewModel();
            
        }

        private void ITunesImportDialog_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            var currentStyle = Native.GetWindowLong(hwnd, Native.GWL_STYLE);
            Native.SetWindowLong(hwnd, Native.GWL_STYLE, (currentStyle & ~Native.WS_MAXIMIZEBOX & ~Native.WS_MINIMIZEBOX));
        }

        private void ImportClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        internal IEnumerable<ITunesTrack> GetItems()
        {
            if (DataContext is ItunesViewModel vm)
            {
                return vm.GetItems();
            }
            return Enumerable.Empty<ITunesTrack>();
        }
    }
}
