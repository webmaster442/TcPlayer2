using System.Collections.Generic;
using System.Threading;
using TcPlayer.Engine.Models;

namespace TcPlayer.Infrastructure
{
    public enum YoutubeDlState
    {
        NotInstalled,
        Outdated,
    }

    internal interface IDialogProvider
    {
        bool TrySelectFileDialog(string filters, out string selectedFile);
        bool TrySelectFilesDialog(string filters, out string[] selectedFiles);
        bool TrySaveFileDialog(string filter, out string selectedFile);

        void SetMainTab(MainTab tab);

        CancellationTokenSource ShowUiBlocker();
        void HideUiBlocker();
        bool TryImportITunes(out IEnumerable<ITunesTrack> items);
        bool TryImportUrl(out string url);
        bool TryImportDlna(out IEnumerable<string> urls);
        void ShowYoutubeDlDialog(YoutubeDlState state);
    }
}
