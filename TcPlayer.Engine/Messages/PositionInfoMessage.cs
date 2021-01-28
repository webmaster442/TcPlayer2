﻿using TcPlayer.Engine.Models;

namespace TcPlayer.Engine.Messages
{
    public record PositionInfoMessage
    {
        public double Percent { get; init; }
        public EngineState State { get; init; }
    }
}