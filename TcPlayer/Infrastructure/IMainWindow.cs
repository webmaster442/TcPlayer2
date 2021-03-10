using System.Threading;

namespace TcPlayer.Infrastructure
{
    internal interface IMainWindow
    {
        void HideUiBlocker();
        CancellationTokenSource ShowUiBlocker();
        void SetMainTab(MainTab tab);
    }
}
