using System;

namespace TcPlayer.Dlna
{
    public sealed class DlnaItem : IEquatable<DlnaItem>
    {
        public string Name { get; init; }
        public string Locaction { get; init; }
        public bool IsServer { get; init; }
        public bool IsBrowsable { get; init; }
        public string Id { get; init; }

        public override bool Equals(object obj)
        {
            return Equals(obj as DlnaItem);
        }

        public bool Equals(DlnaItem other)
        {
            return other != null &&
                   Name == other.Name &&
                   Locaction == other.Locaction &&
                   IsServer == other.IsServer &&
                   IsBrowsable == other.IsBrowsable;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Locaction, IsServer, IsBrowsable);
        }
    }
}
