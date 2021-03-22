// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System.Windows;

namespace TcPlayer.Dialogs
{
    /// <summary>
    /// Interaction logic for OpenUrlDialog.xaml
    /// </summary>
    public partial class ImportUrlDialog
    {
        public ImportUrlDialog()
        {
            InitializeComponent();
        }

        public string Url
        {
            get => UrlBox.Text;
            set => UrlBox.Text = value;
        }

        private void ImportClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
