using System.Windows;
using System.Windows.Controls;

namespace TcPlayer.Controls
{
    public class Titlebar: Control
    {
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(Titlebar), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Titlebar), new PropertyMetadata(string.Empty));

        public Window ControlledWindow
        {
            get { return (Window)GetValue(ControlledWindowProperty); }
            set { SetValue(ControlledWindowProperty, value); }
        }

        public static readonly DependencyProperty ControlledWindowProperty =
            DependencyProperty.Register("ControlledWindow", typeof(Window), typeof(Titlebar), new PropertyMetadata(null));

        public bool IsPinned
        {
            get { return (bool)GetValue(IsPinnedProperty); }
            set { SetValue(IsPinnedProperty, value); }
        }

        public static readonly DependencyProperty IsPinnedProperty =
            DependencyProperty.Register("IsPinned", typeof(bool), typeof(Titlebar), new PropertyMetadata(false, PinnedChange));

        private static void PinnedChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Titlebar titlebar &&
                titlebar.ControlledWindow != null)
            {
                titlebar.ControlledWindow.Topmost = titlebar.IsPinned;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild("PART_BORDER") is Border border)
            {
                border.MouseDown += Border_MouseDown;
            }
            if (GetTemplateChild("PART_PIN") is Button pin)
            {
                pin.Click += Pin_Click;
            }
            if (GetTemplateChild("PART_MINIMIZE") is Button minimize)
            {
                minimize.Click += Minimize_Click;
            }
            if (GetTemplateChild("PART_CLOSE") is Button close)
            {
                close.Click += Close_Click;
            }
        }

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                ControlledWindow?.DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            ControlledWindow?.Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            if (ControlledWindow != null)
            {
                ControlledWindow.WindowState = WindowState.Minimized;
            }
        }

        private void Pin_Click(object sender, RoutedEventArgs e)
        {
            if (ControlledWindow != null)
            {
                IsPinned = !IsPinned;
            }
        }
    }
}
