﻿// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TcPlayer.Engine;

namespace TcPlayer.Controls
{
    /// <summary>
    /// Interaction logic for SongSlider.xaml
    /// </summary>
    public partial class SongSlider : UserControl
    {
        private bool externalChange;

        public SongSlider()
        {
            InitializeComponent();
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(SongSlider), new PropertyMetadata(0.0, MaximumChanged));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(SongSlider), new PropertyMetadata(0.0, ValueChanged));

        public bool IsDragged
        {
            get { return (bool)GetValue(IsDraggedProperty); }
            set { SetValue(IsDraggedProperty, value); }
        }

        public static readonly DependencyProperty IsDraggedProperty =
            DependencyProperty.Register("IsDragged", typeof(bool), typeof(SongSlider), new PropertyMetadata(false));


        public ICommand DragCompleteCommand
        {
            get { return (ICommand)GetValue(DragCompleteCommandProperty); }
            set { SetValue(DragCompleteCommandProperty, value); }
        }

        public static readonly DependencyProperty DragCompleteCommandProperty =
            DependencyProperty.Register("DragCompleteCommand", typeof(ICommand), typeof(SongSlider), new PropertyMetadata(null));



        public ICommand SongEndedCommand
        {
            get { return (ICommand)GetValue(SongEndedCommandProperty); }
            set { SetValue(SongEndedCommandProperty, value); }
        }

        public static readonly DependencyProperty SongEndedCommandProperty =
            DependencyProperty.Register("SongEndedCommand", typeof(ICommand), typeof(SongSlider), new PropertyMetadata(null));



        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SongSlider songSlider)
            {
                songSlider.externalChange = true;
                songSlider.SliderPart.Value = songSlider.Value;
                songSlider.externalChange = false;
                if (Math.Abs(songSlider.Value - songSlider.Maximum) < AudioEngine.UpdatePeriodSeconds
                    && songSlider.Maximum > 0.0)
                {
                    songSlider.SongEndedCommand?.Execute(null);
                }
            }
        }

        private static void MaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SongSlider songSlider)
            {
                if (double.IsNaN(songSlider.Maximum)
                    || double.IsInfinity(songSlider.Maximum))
                    songSlider.SliderPart.Maximum = 0;
                else
                    songSlider.SliderPart.Maximum = songSlider.Maximum;
            }
        }

        private void SliderPart_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            IsDragged = true;
        }

        private void SliderPart_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            var point = Mouse.GetPosition(this);
            var newvalue = (this.Maximum / this.ActualWidth) * point.X;

            DragCompleteCommand?.Execute(newvalue);
            IsDragged = false;
        }

        private void SliderPart_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (externalChange || IsDragged) return;
            DragCompleteCommand?.Execute(e.NewValue);
        }
    }
}
