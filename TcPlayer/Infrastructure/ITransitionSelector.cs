using System.Windows;
using TcPlayer.Effects;

namespace TcPlayer.Infrastructure
{
    interface ITransitionSelector
    {
        Transition GetTransition(object oldContent, object newContent, DependencyObject container);
    }
}
