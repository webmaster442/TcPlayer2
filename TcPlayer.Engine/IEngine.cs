using System;
using System.Collections.Generic;
using System.ComponentModel;
using TcPlayer.Engine.Models;

namespace TcPlayer.Engine
{
    public interface IEngine : IDisposable, INotifyPropertyChanged
    {
        void Initialize(SoundDeviceInfo output);
        void Load(string fileToPlay);
        bool LoadYoutube(YoutubeDlResponse youtubeDlResponse);
        void Pause();
        void Play();
        void Stop();
        void SeeekTo(double position);
        bool IsSeeking { get; set; }
        bool IsVolumeSeeking { get; set; }
        double Length { get; }
        float Volume { get; set; }
        double CurrentPosition { get; }
        EngineState CurrentState { get; }
        Metadata Metadata { get; }
        IEnumerable<SoundDeviceInfo> AvailableOutputs { get; }
        void SetEqualizerParameters(float[] parameters);
    }
}
