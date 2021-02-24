using System;
using System.Collections.Generic;

namespace TcPlayer.Engine.Settings
{
    /// <summary>
    /// Provides writing posibility for settings
    /// </summary>
    public interface ISettingProvider : IReadonlySettingProvider
    {
        /// <summary>
        /// Write a double value setting
        /// </summary>
        /// <param name="container">Storage container identifier</param>
        /// <param name="settingKey">setting identifier</param>
        /// <param name="value">value to write</param>
        void WriteDouble(Guid container, string settingKey, double value);

        /// <summary>
        /// Write a long value setting
        /// </summary>
        /// <param name="container">Storage container identifier</param>
        /// <param name="settingKey">setting identifier</param>
        /// <param name="value">value to write</param>
        void WriteLong(Guid container, string settingKey, long value);

        /// <summary>
        /// Write an int value setting
        /// </summary>
        /// <param name="container">Storage container identifier</param>
        /// <param name="settingKey">setting identifier</param>
        /// <param name="value">value to write</param>
        void WriteInt(Guid container, string settingKey, int value);

        /// <summary>
        /// Write a bool value setting
        /// </summary>
        /// <param name="container">Storage container identifier</param>
        /// <param name="settingKey">setting identifier</param>
        /// <param name="value">value to write</param>
        void WriteBool(Guid container, string settingKey, bool value);

        /// <summary>
        /// Write a Serializable type value setting
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="container">Storage container identifier</param>
        /// <param name="settingKey">setting identifier</param>
        /// <param name="value">value to write</param>
        void WriteType<T>(Guid container, string settingKey, T value);

        /// <summary>
        /// Write a list of value settings
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="container">Storage container identifier</param>
        /// <param name="settingKey">setting identifier</param>
        /// <param name="value">value to write</param>
        void WriteList<T>(Guid container, string settingKey, IReadOnlyList<T> value);
    }
}
