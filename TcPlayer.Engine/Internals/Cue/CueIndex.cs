// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

// Code based on CueSharp 0.5 March 24, 2007
// Original author: Wyatt O'Day wyday.com/cuesharp
// This is a heavily modified version for TCPlayer. Supports only reading

using System;

namespace TcPlayer.Engine.Internals.Cue
{
    /// <summary>
    /// This command is used to specify indexes (or subindexes) within a track.
    /// Syntax:
    ///  INDEX [number] [mm:ss:ff]
    /// </summary>
    internal class CueIndex
    {
        //0-99
        int m_number;

        int _hours;
        int _minutes;
        int _seconds;

        private static void SetValue(ref int field, int value, int limit)
        {
            if (value > limit)
            {
                field = limit;
            }
            else if (value < 0)
            {
                field = 0;
            }
            else
            {
                field = value;
            }
        }

        /// <summary>
        /// Index number (0-99)
        /// </summary>
        public int Number
        {
            get => m_number;
            set => SetValue(ref m_number, value, 99);
        }

        /// <summary>
        /// Possible values: 0-99
        /// </summary>
        public int Hours
        {
            get => _hours;
            set => SetValue(ref _hours, value, 99);
        }

        /// <summary>
        /// Possible values: 0-59
        /// There are 60 seconds/minute
        /// </summary>
        public int Minutes
        {
            get => _minutes;
            set => SetValue(ref _minutes, value, 59);
        }

        /// <summary>
        /// Possible values: 0-74
        /// There are 75 frames/second
        /// </summary>
        public int Seconds
        {
            get => _seconds;
            set => SetValue(ref _seconds, value, 74);
        }

        /// <summary>
        /// The Index of a track.
        /// </summary>
        /// <param name="number">Index number 0-99</param>
        /// <param name="minutes">Minutes (0-99)</param>
        /// <param name="seconds">Seconds (0-59)</param>
        /// <param name="frames">Frames (0-74)</param>
        public CueIndex(int number, int minutes, int seconds, int frames)
        {
            m_number = number;
            _hours = minutes;
            _minutes = seconds;
            _seconds = frames;
        }

        internal double ToDouble()
        {
            return (Hours * 3600) + (Minutes * 60) + Seconds;
        }
    }
}