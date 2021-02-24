using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TcPlayer.Controls
{
    /// <summary>
    /// Interaction logic for EqualizerControl.xaml
    /// </summary>
    public partial class EqualizerControl : UserControl
    {
        public ICommand EqualizerChangeCommand
        {
            get { return (ICommand)GetValue(EqualizerChangeCommandProperty); }
            set { SetValue(EqualizerChangeCommandProperty, value); }
        }

        public static readonly DependencyProperty EqualizerChangeCommandProperty =
            DependencyProperty.Register("EqualizerChangeCommand", typeof(ICommand), typeof(EqualizerControl), new PropertyMetadata(null));
       
        private bool _lockChange;

        public EqualizerControl()
        {
            InitializeComponent();
        }

        private void ResetClick(object sender, RoutedEventArgs e)
        {
            _lockChange = true;
            Slider0.Value = 0;
            Slider1.Value = 0;
            Slider2.Value = 0;
            Slider3.Value = 0;
            Slider4.Value = 0;
            _lockChange = false;
            EqualizerChangeCommand?.Execute(GetValues());
        }

        private float[] GetValues()
        {
            return new float[]
            {
                (float)Slider0.Value,
                (float)Slider1.Value,
                (float)Slider2.Value,
                (float)Slider3.Value,
                (float)Slider4.Value,
            };
        }

        private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_lockChange) return;
            EqualizerChangeCommand?.Execute(GetValues());
        }
    }
}
