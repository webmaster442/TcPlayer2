using System.Threading;

namespace TcPlayer.Infrastructure
{
    internal interface IDialogProvider
    {
        bool TrySelectFileDialog(string filters, out string selectedFile);
        CancellationTokenSource ShowUiBlocker();
        void HideUiBlocker();
    }
}
