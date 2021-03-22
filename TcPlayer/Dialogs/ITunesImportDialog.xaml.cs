// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TcPlayer.Engine.Models;
using TcPlayer.ViewModels;

namespace TcPlayer.Dialogs
{
    public partial class ITunesImportDialog
    {
        public ITunesImportDialog()
        {
            InitializeComponent();
            DataContext = new ItunesViewModel();
            
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
