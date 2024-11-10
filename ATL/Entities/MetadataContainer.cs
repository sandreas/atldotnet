using System.Collections.Generic;
using ATL.AudioData.IO;

namespace ATL.Entities;

#nullable enable
public class MetadataContainer
{
    public MetaDataIO Format { get; set; }
    private Dictionary<string, object?> _fields { get; set; } = new();

    public T? GetFieldValue<T>(TagData.Field key, T? defaultValue = default)
    {
        var mappedKey = MapFieldToStringKey(key);
        if (mappedKey != "" && _fields.TryGetValue(mappedKey, out var field) && field is T value)
        {
            return value;
        }

        return defaultValue;
    }

    /// <summary>
    /// Maps TagData.Field to format specific string key (e.g. TALB)
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    public string MapFieldToStringKey(TagData.Field field)
    {
        return Format.MapField(field);
    }

    public void SetFieldValue<T>(TagData.Field key, T? value)
    {
        var mappedKey = MapFieldToStringKey(key);
        if (mappedKey != "")
        {
            _fields[mappedKey] = value;
        }
    }

    private void RemoveFieldValue<T>(TagData.Field key)
    {
        var mappedKey = MapFieldToStringKey(key);
        if (mappedKey != "" && _fields.ContainsKey(mappedKey))
        {
            _fields.Remove(mappedKey);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    public void SetCustomFieldValue<T>(string key, T? value)
    {
        if (Format.IsValidFieldKey(key))
        {
            _fields[key] = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T? GetCustomFieldValue<T>(string key) where T : class
    {
        if (key != "" && _fields.TryGetValue(key, out var field) && field is T value)
        {
            return value;
        }

        return null;
    }
}