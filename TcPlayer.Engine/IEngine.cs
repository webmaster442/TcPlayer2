using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TcPlayer.Engine
{
    public interface IEngine : IDisposable, INotifyPropertyChanged
    {
        void Initialize(SoundDeviceInfo output);
        void Load(string fileToPlay);
        void Pause();
        void Play();
        void Stop();
        void SeeekTo(float position);
        bool IsSeeking { get; set; }
        double Length { get; }
        double CurrentPosition { get; }
        EngineState CurrentState { get; }
        Metadata Metadata { get; }
        IEnumerable<SoundDeviceInfo> AvailableOutputs { get; }
    }
}
