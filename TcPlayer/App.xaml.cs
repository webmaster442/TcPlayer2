using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using TcPlayer.Engine;
using TcPlayer.Infrastructure;
using TcPlayer.ViewModels;

namespace TcPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Current.MainWindow = new MainWindow();
            var engine = new AudioEngine();
            var model = new MainViewModel(engine, new Dialogs());
            Current.MainWindow.DataContext = model;

            Current.MainWindow.Show();
        }
    }
}
