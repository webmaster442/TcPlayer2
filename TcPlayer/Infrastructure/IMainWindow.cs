// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

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
