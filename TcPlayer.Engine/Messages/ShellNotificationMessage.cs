using TcPlayer.Engine.Models;

namespace TcPlayer.Engine.Messages
{
    public record ShellNotificationMessage
    {
        public double Length { get; init; }
        public double Position { get; init; }
        public EngineState State { get; init; }
        public Metadata Metadata { get; init; }

        public double Percent
        {
            get => double.IsInfinity(Length) || double.IsNaN(Length) ? double.PositiveInfinity : (Position / Length);
        }
        
        public bool IsIndeterminate
        {
            get
            {
                return double.IsNaN(Percent)
                    || double.IsInfinity(Percent);
            }
        }

        public ShellNotificationMessage()
        {
            Metadata = new Metadata();
        }
    }
}
