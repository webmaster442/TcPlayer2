// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Windows;

namespace TcPlayer.Effects
{
    /// <summary>
    /// Slide in transition effect
    /// </summary>
    public class TransitionSlideIn : Transition
    {
        private const string TransitionSlideInEffect = "pack://application:,,,/TcPlayer;component/Effects/TransitionSlideInEffect.ps";

        /// <summary>
        /// Dependency property for SlideAmmount
        /// </summary>
        public static readonly DependencyProperty SlideAmountProperty =
            DependencyProperty.Register("SlideAmount", typeof(Point), typeof(TransitionSlideIn), new UIPropertyMetadata(new Point(1D, 0D), PixelShaderConstantCallback(1)));

        /// <summary>
        /// Slide direction vector
        /// </summary>
        public Point SlideAmount
        {
            get { return ((Point)(this.GetValue(SlideAmountProperty))); }
            set { this.SetValue(SlideAmountProperty, value); }
        }


        /// <summary>
        /// Creates a new instance of Slide in transition
        /// </summary>
        /// <param name="dir">Slide in direction</param>
        public TransitionSlideIn(SlideDirection dir) : base(new Uri(TransitionSlideInEffect))
        {
            switch (dir)
            {
                case SlideDirection.Left:
                    SlideAmount = new Point(1, 0);
                    break;
                case SlideDirection.Right:
                    SlideAmount = new Point(-1, 0);
                    break;
                case SlideDirection.Up:
                    SlideAmount = new Point(0, 1);
                    break;
                case SlideDirection.Down:
                    SlideAmount = new Point(0, -1);
                    break;
                case SlideDirection.LeftDown:
                    SlideAmount = new Point(1, -1);
                    break;
                case SlideDirection.LeftUp:
                    SlideAmount = new Point(-1, 1);
                    break;
                case SlideDirection.RightDown:
                    SlideAmount = new Point(-1, -1);
                    break;
                case SlideDirection.RightUp:
                    SlideAmount = new Point(1, 1);
                    break;
            }
        }

        /// <summary>
        /// Creates a new instance of Slide in transition
        /// </summary>
        public TransitionSlideIn() : this(SlideDirection.Left) { }

        /// <summary>
        /// Slide direction
        /// </summary>
        public enum SlideDirection
        {
            /// <summary>
            /// Left
            /// </summary>
            Left,
            /// <summary>
            /// Right
            /// </summary>
            Right,
            /// <summary>
            /// Up
            /// </summary>
            Up,
            /// <summary>
            /// Down
            /// </summary>
            Down,
            /// <summary>
            /// Left &amp; up
            /// </summary>
            LeftUp,
            /// <summary>
            /// Left &amp; down
            /// </summary>
            LeftDown,
            /// <summary>
            /// Right &amp; up
            /// </summary>
            RightUp,
            /// <summary>
            /// Right &amp; down
            /// </summary>
            RightDown
        }
    }
}
