using System.Windows;
using System.Windows.Controls;

namespace TcPlayer.Controls
{
    /// <summary>
    /// Interaction logic for EqualizerControl.xaml
    /// </summary>
    public partial class EqualizerControl : UserControl
    {
        public EqualizerControl()
        {
            InitializeComponent();
        }

        private void ResetClick(object sender, RoutedEventArgs e)
        {
            Slider0.Value = 0;
            Slider1.Value = 0;
            Slider2.Value = 0;
            Slider3.Value = 0;
            Slider4.Value = 0;
        }

        public float[] Values
        {
            get
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
            set
            {
                if (value != null 
                    && value.Length == 5)
                {
                    Slider0.Value = value[0];
                    Slider1.Value = value[1];
                    Slider2.Value = value[2];
                    Slider3.Value = value[3];
                    Slider4.Value = value[4];
                }
            }
        }
    }
}
