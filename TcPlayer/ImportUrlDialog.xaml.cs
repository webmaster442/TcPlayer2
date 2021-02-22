using System.Windows;

namespace TcPlayer
{
    /// <summary>
    /// Interaction logic for OpenUrlDialog.xaml
    /// </summary>
    public partial class ImportUrlDialog : Window
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
