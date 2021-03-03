using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace TcPlayer.Effects
{
    /// <summary>
    /// Defines a transition effect shader that transitions from one visual to another visual
    /// using an interpolated value between 0 and 1.
    /// This class is a reimplementation of the class found in the Expression SDK
    /// </summary>
    public abstract class Transition : ShaderEffect
    {
        /// <summary>
        /// Brush-valued properties that turn into sampler-properties in the shader.
        /// Represents the image present in the final state of the transition.
        /// </summary>
        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(Transition), 0, SamplingMode.NearestNeighbor);

        /// <summary>
        /// Brush-valued properties that turn into sampler-properties in the shader.
        /// Represents the image present in the initial state of the transition.
        /// </summary>
        public static readonly DependencyProperty OldImageProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("OldImage", typeof(Transition), 1, SamplingMode.NearestNeighbor);

        /// <summary>
        /// A Dependency property as the backing store for Progress.
        /// Also used to represent the state of a transition from start to finish (range between 0 and 1).
        /// </summary>
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(Transition), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(0)));

        /// <summary>
        /// Gets or sets the Input variable within the shader.
        /// </summary>
        public Brush Input
        {
            get { return (Brush)GetValue(Transition.InputProperty); }
            set { SetValue(Transition.InputProperty, value); }
        }

        /// <summary>
        /// Gets or sets the OldImage variable within the shader.
        /// </summary>
        public Brush OldImage
        {
            get { return (Brush)GetValue(Transition.OldImageProperty); }
            set { SetValue(Transition.OldImageProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Progress variable within the shader.
        /// </summary>
        public double Progress
        {
            get { return (double)GetValue(Transition.ProgressProperty); }
            set { SetValue(Transition.ProgressProperty, value); }
        }


        /// <summary>
        /// Updates the shader's variables to the default values.
        /// </summary>
        protected Transition(Uri source)
        {
            var shader = new PixelShader();
            shader.UriSource = source;
            PixelShader = shader;

            UpdateShaderValue(Transition.InputProperty);
            UpdateShaderValue(Transition.OldImageProperty);
            UpdateShaderValue(Transition.ProgressProperty);
        }
    }
}
