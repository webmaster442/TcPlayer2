namespace TcPlayer.Engine.Models
{
    internal class AsyncProcesssResult
    {
        public bool Completed { get; init; }
        public int? ExitCode { get; init; }
        public string Output { get; init; }

        public AsyncProcesssResult()
        {
            Output = string.Empty;
        }
    }
}
