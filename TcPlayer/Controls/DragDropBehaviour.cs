// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace TcPlayer.Controls
{
    internal static class DragDropBehaviour
    {
        public static ICommand GetFileDraggedInCommmand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(FileDraggedInCommmandProperty);
        }

        public static void SetFileDraggedInCommmand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(FileDraggedInCommmandProperty, value);
        }


        public static readonly DependencyProperty FileDraggedInCommmandProperty =
            DependencyProperty.RegisterAttached("FileDraggedInCommmand", typeof(ICommand), typeof(DragDropBehaviour), new PropertyMetadata(null, CommandChanged));

        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Selector control)
            {
                control.AllowDrop = true;
                control.DragEnter += OnDragEnter;
                control.Drop += OnDrop;
            }
        }

        private static void OnDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private static void OnDrop(object sender, DragEventArgs e)
        {
            if (sender is DependencyObject dependencyObject)
            {
                ICommand command = GetFileDraggedInCommmand(dependencyObject);
                string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];

                if (command?.CanExecute(files) ?? false)
                {
                    command?.Execute(files);
                }
            }
        }
    }
}
