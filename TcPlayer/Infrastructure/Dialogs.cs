using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace TcPlayer.Infrastructure
{
    internal class Dialogs : IDialogProvider
    {
        public bool TrySelectFileDialog(string filters, out string selectedFile)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = filters,
                Multiselect = false,
                CheckFileExists = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                selectedFile = openFileDialog.FileName;
                return true;
            }
            else
            {
                selectedFile = string.Empty;
                return false;
            }
        }
    }
}
