using System;

namespace TcPlayer
{
    public static class Entrypoint
    {
        internal const string MutexName = @"Local/5C45E867-A91C-442D-A8D6-10753C6FDF55/TcPlayer2";

        [STAThread]
        public static void Main(string[] arguments)
        {
            var application = new App();
            application.SetupEngineDependencies();
            application.SetupDependencies();
            application.InitializeComponent();
            application.Run();
        }
    }
}
