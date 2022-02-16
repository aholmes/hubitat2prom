using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf;

namespace hubitat2prom.JsonConverters;

public class OneOfDoubleStringNullableJsonConverter : JsonConverter<OneOf<double, string>?>
{
    private static OneOfDoubleStringJsonConverter oneOfDoubleStringJsonConverter = new OneOfDoubleStringJsonConverter();
    public override OneOf<double, string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        return oneOfDoubleStringJsonConverter.Read(ref reader, typeToConvert, options);
    }

    public override void Write(Utf8JsonWriter writer, OneOf<double, string>? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            oneOfDoubleStringJsonConverter.Write(writer, value.Value, options);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
