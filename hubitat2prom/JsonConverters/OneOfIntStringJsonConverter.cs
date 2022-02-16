using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf;

namespace hubitat2prom.JsonConverters;

public class OneOfIntStringJsonConverter : JsonConverter<OneOf<int, string>?>
{
    public override OneOf<int, string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString();
        }
        else
        {
            return reader.TryGetInt32(out int value)
                ? value
                : null;
        }
    }

    public override void Write(Utf8JsonWriter writer, OneOf<int, string>? value, JsonSerializerOptions options)
    {
        if (!value.HasValue)
        {
            writer.WriteNullValue();
        }
        else
        {
            value.Value.Switch(
                @int => writer.WriteNumberValue(@int),
                @string => writer.WriteStringValue(@string)
            );
        }
    }
}
