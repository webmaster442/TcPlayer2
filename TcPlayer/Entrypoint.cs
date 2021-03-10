using System;

namespace TcPlayer
{
    public static class Entrypoint
    {
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
