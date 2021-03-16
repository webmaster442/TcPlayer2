// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TcPlayer.Engine.Settings
{
    internal sealed class SettingsProvider : ISettingProvider
    {
        private readonly Dictionary<Setting, string> _storage;
        private readonly JsonSerializerOptions _JsonOptions;

        public event EventHandler<Setting>? SettingChanged;

        public SettingsProvider(Dictionary<Setting, string> storage)
        {
            _storage = storage;
            _JsonOptions = new JsonSerializerOptions();
            _JsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        private static Setting CreateKey(Guid container, string settingKey)
        {
            return new Setting
            {
                Container = container,
                Key = settingKey
            };
        }

        private static T GetIconvertible<T>(string value) where T : IConvertible
        {
            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }

        private void SetIconvertible<T>(Setting compositeKey, T value) where T : IConvertible
        {
            string strValue = (string)Convert.ChangeType(value, typeof(string), CultureInfo.InvariantCulture);

            if (_storage.ContainsKey(compositeKey))
            {
                if (_storage[compositeKey] != strValue)
                {
                    _storage[compositeKey] = strValue;
                    SettingChanged?.Invoke(this, compositeKey);
                }
            }
            else
            {
                _storage.Add(compositeKey, strValue);
                SettingChanged?.Invoke(this, compositeKey);
            }
        }

        public bool GetBool(Guid container, string settingKey)
        {
            return GetIconvertible<bool>(_storage[CreateKey(container, settingKey)]);
        }

        public void WriteBool(Guid container, string settingKey, bool value)
        {
            SetIconvertible(CreateKey(container, settingKey), value);
        }

        public double GetDouble(Guid container, string settingKey)
        {
            return GetIconvertible<double>(_storage[CreateKey(container, settingKey)]);
        }

        public void WriteDouble(Guid container, string settingKey, double value)
        {
            SetIconvertible(CreateKey(container, settingKey), value);
        }

        public int GetInt(Guid container, string settingKey)
        {
            return GetIconvertible<int>(_storage[CreateKey(container, settingKey)]);
        }

        public void WriteInt(Guid container, string settingKey, int value)
        {
            SetIconvertible(CreateKey(container, settingKey), value);
        }

        public long GetLong(Guid container, string settingKey)
        {
            return GetIconvertible<long>(_storage[CreateKey(container, settingKey)]);
        }

        public void WriteLong(Guid container, string settingKey, long value)
        {
            SetIconvertible(CreateKey(container, settingKey), value);
        }

        public T GetType<T>(Guid container, string settingKey)
        {
            var converted = JsonSerializer.Deserialize<T>(_storage[CreateKey(container, settingKey)], _JsonOptions);
            if (converted == null)
                throw new InvalidCastException();

            return converted;
        }

        public void WriteType<T>(Guid container, string settingKey, T value)
        {
            Setting compositeKey = CreateKey(container, settingKey);
            string json = JsonSerializer.Serialize<T>(value, _JsonOptions);

            if (_storage.ContainsKey(compositeKey))
            {
                if (_storage[compositeKey] != json)
                {
                    _storage[compositeKey] = json;
                    SettingChanged?.Invoke(this, compositeKey);
                }
            }
            else
            {
                _storage.Add(compositeKey, json);
                SettingChanged?.Invoke(this, compositeKey);
            }
        }

        public bool IsExisting(Guid container, string settingKey)
        {
            Setting compositeKey = CreateKey(container, settingKey);
            return _storage.ContainsKey(compositeKey);
        }

        public void WriteList<T>(Guid container, string settingKey, IReadOnlyList<T> value)
        {
            string[] values = new string[value.Count];
            if (typeof(T).IsAssignableFrom(typeof(IConvertible)))
            {
                for (int i = 0; i < value.Count; i++)
                {
                    var converted = Convert.ChangeType(value[i], typeof(string));
                    if (converted == null)
                        throw new InvalidCastException();
                    values[i] = (string)converted;
                }
            }
            else
            {
                for (int i = 0; i < value.Count; i++)
                {
                    values[i] = JsonSerializer.Serialize(value[i], _JsonOptions);
                }
            }
            WriteType(container, settingKey, values);
        }

        public IReadOnlyList<T> GetList<T>(Guid container, string settingKey)
        {
            string[] values = GetType<string[]>(container, settingKey);
            T[] ret = new T[values.Length];
            if (typeof(T).IsAssignableFrom(typeof(IConvertible)))
            {
                for (int i = 0; i < ret.Length; i++)
                {
                    var converted = Convert.ChangeType(values[i], typeof(T));
                    if (converted == null)
                        throw new InvalidCastException();
                    ret[i] = (T)converted;
                }
            }
            else
            {
                for (int i = 0; i < ret.Length; i++)
                {
                    var converted = JsonSerializer.Deserialize<T>(values[i]);
                    if (converted == null)
                        throw new InvalidCastException();
                    ret[i] = converted;
                }
            }
            return ret;
        }
    }
}
