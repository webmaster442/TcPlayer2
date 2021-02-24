using System;

namespace TcPlayer.Engine.Settings
{
    /// <summary>
    /// Represents a container an setting key
    /// </summary>
    public sealed class Setting : IEquatable<Setting?>
    {
        public Setting()
        {
            Container = Guid.Empty;
            Key = string.Empty;
        }

        /// <summary>
        /// Container id
        /// </summary>
        public Guid Container { get; init; }

        /// <summary>
        /// Setting key
        /// </summary>
        public string Key { get; init; }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as Setting);
        }

        /// <inheritdoc/>
        public bool Equals(Setting? other)
        {
            return other != null &&
                   Container.Equals(other.Container) &&
                   Key == other.Key;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Container, Key);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Key}\n{Container}";
        }
    }
}
