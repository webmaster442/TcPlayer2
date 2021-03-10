using System.Threading;
using System.Windows.Shell;

namespace TcPlayer.Infrastructure
{
    internal interface IMainWindow
    {
        void HideUiBlocker();
        CancellationTokenSource ShowUiBlocker();
        void SetMainTab(MainTab tab);
        TaskbarItemInfo TaskbarItemInfo { get; }
    }
}
