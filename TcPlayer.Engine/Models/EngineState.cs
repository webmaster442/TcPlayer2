namespace TcPlayer.Engine.Models
{
    /// <summary>
    /// State transition:
    /// NoFile -> ReadyToPlay -> Playing -> NoFile
    /// </summary>
    public enum EngineState
    {
        NoFile,
        ReadyToPlay,
        Playing,
        Paused,
        Seeking,
    }
}
