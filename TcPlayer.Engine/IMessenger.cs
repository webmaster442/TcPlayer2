// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using TcPlayer.Engine.Ui;

namespace TcPlayer.Engine
{
    public interface IMessenger
    {
        void SubScribe(IMessageClient subscriber);
        void UnSubscribe(IMessageClient subscriber);
        bool SendMessage(Guid target, object message);
        bool SendMessage(Type target, Guid id, object message);
        bool SendMessage(Type targettype, object message);
        bool SendMessage(object message);
    }
}
