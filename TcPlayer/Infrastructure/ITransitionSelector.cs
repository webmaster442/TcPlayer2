// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System.Windows;
using TcPlayer.Effects;

namespace TcPlayer.Infrastructure
{
    interface ITransitionSelector
    {
        Transition GetTransition(object oldContent, object newContent, DependencyObject container);
    }
}
