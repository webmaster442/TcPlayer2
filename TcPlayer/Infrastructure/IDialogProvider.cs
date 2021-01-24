using System.Threading;

namespace TcPlayer.Infrastructure
{
    internal interface IDialogProvider
    {
        bool TrySelectFileDialog(string filters, out string selectedFile);
        bool TrySelectFilesDialog(string filters, out string[] selectedFiles);
        bool TrySaveFileDialog(string filter, out string selectedFile);

        CancellationTokenSource ShowUiBlocker();
        void HideUiBlocker();
    }
}
