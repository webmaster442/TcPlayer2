// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace TcPlayer.Engine.Settings
{
    /// <summary>
    /// Provides reading posibility for settings
    /// </summary>
    public interface IReadonlySettingProvider
    {
        /// <summary>
        /// Fired, if a setting was changed or created
        /// </summary>
        event EventHandler<Setting> SettingChanged;

        /// <summary>
        /// Checks if a setting exists in a container or not.
        /// </summary>
        /// <param name="container">Storage container identifier</param>
        /// <param name="settingKey">setting identifier</param>
        /// <returns>true, if setting exists, false if not</returns>
        bool IsExisting(Guid container, string settingKey);

        /// <summary>
        /// Gets a double value setting
        /// </summary>
        /// <param name="container">Storage container identifier</param>
        /// <param name="settingKey">setting identifier</param>
        /// <returns>value in container identified by key</returns>
        double GetDouble(Guid container, string settingKey);

        /// <summary>
        /// Gets a long value setting
        /// </summary>
        /// <param name="container">Storage container identifier</param>
        /// <param name="settingKey">setting identifier</param>
        /// <returns>value in container identified by key</returns>
        long GetLong(Guid container, string settingKey);

        /// <summary>
        /// Gets a int value setting
        /// </summary>
        /// <param name="container">Storage container identifier</param>
        /// <param name="settingKey">setting identifier</param>
        /// <returns>value in container identified by key</returns>
        int GetInt(Guid container, string settingKey);

        /// <summary>
        /// Gets a bool value setting
        /// </summary>
        /// <param name="container">Storage container identifier</param>
        /// <param name="settingKey">setting identifier</param>
        /// <returns>value in container identified by key</returns>
        bool GetBool(Guid container, string settingKey);

        /// <summary>
        /// Gets a serializable value setting
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="container">Storage container identifier</param>
        /// <param name="settingKey">setting identifier</param>
        /// <returns>value in container identified by key</returns>
        T GetType<T>(Guid container, string settingKey);

        /// <summary>
        /// Gets a list of values
        /// </summary>
        /// <typeparam name="T">Type of list items</typeparam>
        /// <param name="container">Storage container identifier</param>
        /// <param name="settingKey">setting identifier</param>
        /// <returns>value in container identified by key</returns>
        IReadOnlyList<T> GetList<T>(Guid container, string settingKey);
    }
}
