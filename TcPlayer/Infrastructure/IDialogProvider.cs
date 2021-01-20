using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcPlayer.Infrastructure
{
    internal interface IDialogProvider
    {
        bool TrySelectFileDialog(string filters, out string selectedFile);
    }
}
