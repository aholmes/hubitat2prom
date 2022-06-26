using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using hubitat2prom.HubitatDevice;

namespace hubitat2prom.JsonConverters;

public class DeviceSummaryAttributesDynamicJsonConverter : JsonConverter<DeviceSummaryAttributes>
{
    private class SerializerSetMemberBinder : SetMemberBinder
    {
        public SerializerSetMemberBinder(string name, bool ignoreCase) : base(name, ignoreCase) { }

        public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject? errorSuggestion)
            => throw new NotImplementedException();
    }

    public override DeviceSummaryAttributes Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            var hubitatDeviceCapabilities = new DeviceSummaryAttributes();
            var type = hubitatDeviceCapabilities.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToDictionary(propertyInfo => propertyInfo.Name);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return hubitatDeviceCapabilities;
                }

                var propertyName = reader.GetString();
                if (properties.ContainsKey(propertyName))
                {
                    var property = type.GetProperty(propertyName);
                    var value = JsonSerializer.Deserialize(ref reader, property.PropertyType, options);
                    property.SetValue(hubitatDeviceCapabilities, value);
                }
                else
                {
                    var setMemberBinder = new SerializerSetMemberBinder(reader.GetString(), true);
                    hubitatDeviceCapabilities.TrySetMember(
                        setMemberBinder,
                        JsonSerializer.Deserialize<dynamic>(ref reader, options)
                    );
                }
            }
            throw new JsonException("Did not parse to end of object.");
        }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, DeviceSummaryAttributes value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}


/// <summary>
/// https://github.com/dotnet/runtime/issues/29690#issuecomment-571969037
/// We use this to get actual types in the deserialized object instead of JsonValueKind objects.
/// This lets the serialization for exporting dynamic properties on DeviceSummaryAttributes work.
/// </summary>
public class DynamicJsonConverter : JsonConverter<dynamic>
{
    public override dynamic Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.True) return true;

        if (reader.TokenType == JsonTokenType.False) return false;

        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt64(out long l)) return l;

            return reader.GetDouble();
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            if (reader.TryGetDateTime(out DateTime datetime)) return datetime;

            return reader.GetString();
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using JsonDocument documentV = JsonDocument.ParseValue(ref reader);
            return ReadObject(documentV.RootElement);
        }
        // Use JsonElement as fallback.
        // Newtonsoft uses JArray or JObject.
        JsonDocument document = JsonDocument.ParseValue(ref reader);
        return document.RootElement.Clone();
    }

    private object ReadObject(JsonElement jsonElement)
    {
        IDictionary<string, object> expandoObject = new ExpandoObject();
        foreach (var obj in jsonElement.EnumerateObject())
        {
            var k = obj.Name;
            var value = ReadValue(obj.Value);
            expandoObject[k] = value;
        }
        return expandoObject;
    }
    private object? ReadValue(JsonElement jsonElement)
    {
        object? result = null;
        switch (jsonElement.ValueKind)
        {
            case JsonValueKind.Object:
                result = ReadObject(jsonElement);
                break;
            case JsonValueKind.Array:
                result = ReadList(jsonElement);
                break;
            case JsonValueKind.String:
                //TODO: Missing Datetime&Bytes Convert
                result = jsonElement.GetString();
                break;
            case JsonValueKind.Number:
                //TODO: more num type
                result = 0;
                if (jsonElement.TryGetInt64(out long l))
                {
                    result = l;
                }
                break;
            case JsonValueKind.True:
                result = true;
                break;
            case JsonValueKind.False:
                result = false;
                break;
            case JsonValueKind.Undefined:
            case JsonValueKind.Null:
                result = null;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return result;
    }

    private object? ReadList(JsonElement jsonElement)
    {
        IList<object?> list = new List<object?>();
        foreach (var item in jsonElement.EnumerateArray())
        {
            list.Add(ReadValue(item));
        }
        return list.Count == 0 ? null : list;
    }
    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        => throw new NotImplementedException();
}