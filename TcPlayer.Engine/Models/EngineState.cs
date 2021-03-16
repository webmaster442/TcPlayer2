// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

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
