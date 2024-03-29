﻿// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace TcPlayer.Engine
{
    public abstract class NotifyObject : DispatcherObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly DispatcherTimer _timer;
        private int _timerCalls;

        public bool TimerEnabled
        {
            get => _timer.IsEnabled;
            set => _timer.IsEnabled = value;
        }

        public const double UpdatePeriodSeconds = 0.1d;

        protected NotifyObject()
        {
            _timer = new DispatcherTimer(DispatcherPriority.Normal);
            _timer.Tick += OnTimerTick;
            _timer.Interval = TimeSpan.FromMilliseconds(100);
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            _timerCalls++;
            if (_timerCalls > 9)
            {
                UpdateTimer();
                UpdateTimerLong();
                _timerCalls = 0;
            }
            else
            {
                UpdateTimer();
            }
        }

        protected virtual void UpdateTimerLong()
        {
            //empty in base class
        }

        protected virtual void UpdateTimer()
        {
            //empty in base class
        }

        public bool SetProperty<T>(ref T backfield, T value, [CallerMemberName]string? propName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(backfield, value))
            {
                backfield = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
                return true;
            }
            return false;
        }

        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
