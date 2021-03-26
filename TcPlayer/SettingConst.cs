// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;

namespace TcPlayer
{
    internal static class SettingConst
    {
        public static readonly Guid AppSettings = Guid.Parse("FD832D4F-09B9-4BC0-9D8D-7C5331BCC5FC");
        public static readonly Guid AudioSettings = Guid.Parse("14BB670C-301D-4E69-81A2-0460A632DECF");
        public const string NotifySongChange = "NotifySongChange";
        public const string AudioOutput = "AudioOutput";
        public const string Equalizer = "Equalizer";
        public const string Installed = "Installed";
    }
}