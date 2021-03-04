using System.Windows;
using TcPlayer.ViewModels;

namespace TcPlayer.Dialogs
{
    /// <summary>
    /// Interaction logic for DlnaImport.xaml
    /// </summary>
    public partial class DlnaImportDialog : Window
    {
        public DlnaImportDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is DlnaViewModel viewModel)
            {
                viewModel.ServersCommand.Execute(null);
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ImportClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
