namespace TcPlayer
{
    public static class Entrypoint
    {
        public static void Main(string[] arguments)
        {
            var application = new App();
            application.SetupEngineDependencies();
            application.SetupDependencies();
            application.Run();
        }
    }
}
