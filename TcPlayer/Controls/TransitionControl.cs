using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TcPlayer.Effects;
using TcPlayer.Infrastructure;

namespace TcPlayer.Controls
{
    internal class TransitionControl : ContentControl
    {
        private ContentPresenter _contentPresenter;
        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration",
                                                                                                 typeof(TimeSpan),
                                                                                                 typeof(TransitionControl),
                                                                                                 new UIPropertyMetadata(TimeSpan.FromSeconds(1)));

        public bool EnableTransitions
        {
            get { return (bool)GetValue(EnableTransitionsProperty); }
            set { SetValue(EnableTransitionsProperty, value); }
        }

        public static readonly DependencyProperty EnableTransitionsProperty = DependencyProperty.Register("EnableTransitions",
                                                                                                          typeof(bool),
                                                                                                          typeof(TransitionControl),
                                                                                                          new UIPropertyMetadata(true));


        public ITransitionSelector ContentTransitionSelector
        {
            get { return (ITransitionSelector)GetValue(ContentTransitionSelectorProperty); }
            set { SetValue(ContentTransitionSelectorProperty, value); }
        }

        public static readonly DependencyProperty ContentTransitionSelectorProperty = DependencyProperty.Register("ContentTransitionSelector",
                                                                                                                   typeof(ITransitionSelector),
                                                                                                                   typeof(TransitionControl),
                                                                                                                   new UIPropertyMetadata(null));

        static TransitionControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TransitionControl), new FrameworkPropertyMetadata(typeof(TransitionControl)));

            ContentProperty.OverrideMetadata(
                typeof(TransitionControl), new FrameworkPropertyMetadata(OnContentPropertyChanged));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _contentPresenter = (ContentPresenter)Template.FindName("PART_ContentHost", this);
        }

        private static void OnContentPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs args)
        {
            var oldContent = args.OldValue;
            var newContent = args.NewValue;

            if (oldContent != null && newContent != null)
            {
                var transitionControl = (TransitionControl)dp;
                transitionControl.AnimateContent(oldContent, newContent);
            }
            else
            {
                var transitionControl = (TransitionControl)dp;
                transitionControl.AnimateContent(newContent);
            }
        }

        private static void ExecuteOnLoaded(FrameworkElement fe, Action action)
        {
            if (fe.IsLoaded)
            {
                action?.Invoke();
            }
            else
            {
                RoutedEventHandler handler = null;
                handler = delegate
                {
                    fe.Loaded -= handler;
                    action?.Invoke();
                };

                fe.Loaded += handler;
            }
        }


        private void AnimateContent(object content)
        {
            ExecuteOnLoaded(this, () =>
            {
                _contentPresenter.Content = content;
            });
        }

        private void AnimateContent(object oldContent, object newContent)
        {
            var oldContentVisual = GetVisualChild();
            var tier = (RenderCapability.Tier >> 16);

            // if we dont have a selector, or the visual content is not a FE, do not animate
            if (!EnableTransitions || ContentTransitionSelector == null || oldContentVisual == null || tier < 2)
            {
                SetNonVisualChild(newContent);
                return;
            }

            // create the transition
            Transition transitionEffect = ContentTransitionSelector.GetTransition(oldContent, newContent, this);
            if (transitionEffect == null)
            {
                throw new InvalidOperationException("Returned transition effect is null.");
            }

            // create the animation
            DoubleAnimation da = new DoubleAnimation(0.0, 1.0, new Duration(Duration), FillBehavior.HoldEnd);
            da.Completed += delegate
            {
                ApplyEffect(null);
            };
            da.AccelerationRatio = 0.5;
            da.DecelerationRatio = 0.5;
            transitionEffect.BeginAnimation(Transition.ProgressProperty, da);

            VisualBrush oldVisualBrush = new VisualBrush(oldContentVisual);
            transitionEffect.OldImage = oldVisualBrush;

            SetNonVisualChild(newContent);
            ApplyEffect(transitionEffect);
        }


        private FrameworkElement GetVisualChild()
        {
            try
            {
                var visualChild = VisualTreeHelper.GetChild(_contentPresenter, 0) as FrameworkElement;
                return visualChild;
            }
            catch (ArgumentOutOfRangeException) { return null; }
        }

        private void SetNonVisualChild(object content)
        {
            _contentPresenter.Content = content;
        }

        private void ApplyEffect(Transition effect)
        {
            _contentPresenter.Effect = effect;
        }
    }
}
