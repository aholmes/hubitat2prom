using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf;

namespace hubitat2prom.JsonConverters;

public class OneOfDoubleStringJsonConverter : JsonConverter<OneOf<double, string>>
{
    public override OneOf<double, string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString();
        }
        else
        {
            return reader.TryGetDouble(out double value)
                ? value
                : null;
        }
    }

    public override void Write(Utf8JsonWriter writer, OneOf<double, string> value, JsonSerializerOptions options)
    {
        value.Switch(
            @double => writer.WriteNumberValue(@double),
            @string => writer.WriteStringValue(@string)
        );
    }
}
