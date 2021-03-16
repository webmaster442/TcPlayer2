// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

namespace TcPlayer.Network
{
    public sealed record DlnaItem
    {
        public DlnaItem()
        {
            Name = string.Empty;
            Locaction = string.Empty;
            Id = string.Empty;
        }

        public string Name { get; init; }
        public string Locaction { get; init; }
        public bool IsServer { get; init; }
        public bool IsBrowsable { get; init; }
        public string Id { get; init; }
    }
}
