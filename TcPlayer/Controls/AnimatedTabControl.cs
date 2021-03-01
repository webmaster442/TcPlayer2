using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TcPlayer.Effects;
using TcPlayer.Infrastructure;

namespace TcPlayer.Controls
{
    public class AnimatedTabControl : TabControl, ITransitionSelector
    {
        static AnimatedTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedTabControl), new FrameworkPropertyMetadata(typeof(AnimatedTabControl)));
        }

        public Transition GetTransition(object oldContent, object newContent, DependencyObject container)
        {
            int oldIndex = -1;
            int newIndex = -1;
            int i = -1;
            foreach (TabItem item in Items)
            {
                ++i;
                if (item.Content.GetHashCode() == oldContent.GetHashCode())
                {
                    oldIndex = i;
                }
                else if (item.Content.GetHashCode() == newContent.GetHashCode())
                {
                    newIndex = i;
                }
            }

            if (oldIndex < newIndex)
            {
                return new TransitionSlideIn(TransitionSlideIn.SlideDirection.Up);
            }
            return new TransitionSlideIn(TransitionSlideIn.SlideDirection.Down);
        }

        public override void OnApplyTemplate()
        {
            if (GetTemplateChild("PART_SelectedContentHost") is TransitionControl transitionControl)
            {
                transitionControl.ContentTransitionSelector = this;
            }
        }
    }
}
