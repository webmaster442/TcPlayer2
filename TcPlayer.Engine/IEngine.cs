// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using TcPlayer.Engine.Models;

namespace TcPlayer.Engine
{
    public interface IEngine : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a given audio output
        /// </summary>
        /// <param name="output">Audio output to use</param>
        void Initialize(SoundDeviceInfo output);
        /// <summary>
        /// Loads a file
        /// </summary>
        /// <param name="fileToPlay">A file path or a stream url</param>
        void Load(string fileToPlay);
        /// <summary>
        /// Loads a song from youtube
        /// </summary>
        /// <param name="youtubeDlResponse">YoutubeDl output</param>
        /// <returns>True, if load was succesfull, otherwise false</returns>
        bool LoadYoutube(YoutubeDlResponse youtubeDlResponse);
        /// <summary>
        /// Pause a currently playing track
        /// </summary>
        void Pause();
        /// <summary>
        /// Start playback
        /// </summary>
        void Play();
        /// <summary>
        /// Stop playback
        /// </summary>
        void Stop();
        /// <summary>
        /// Seek to a given seconds in the currently playing song
        /// </summary>
        /// <param name="position">position to seek to</param>
        void SeeekTo(double position);
        /// <summary>
        /// Returns true, if currently is seeking
        /// </summary>
        bool IsSeeking { get; }
        /// <summary>
        /// Get the current song length in seconds
        /// </summary>
        double Length { get; }
        /// <summary>
        /// Get or set the volume
        /// </summary>
        float Volume { get; set; }
        /// <summary>
        /// Current position in seconds
        /// </summary>
        double CurrentPosition { get; }
        /// <summary>
        /// Current engine state
        /// </summary>
        EngineState CurrentState { get; }
        /// <summary>
        /// Current song metadata
        /// </summary>
        Metadata Metadata { get; }
        /// <summary>
        /// Gets the available audio outputs
        /// </summary>
        IEnumerable<SoundDeviceInfo> AvailableOutputs { get; }
        /// <summary>
        /// Sets the equalizer parameters
        /// </summary>
        /// <param name="parameters">an array containing equalizer parameters</param>
        void SetEqualizerParameters(float[] parameters);
    }
}
