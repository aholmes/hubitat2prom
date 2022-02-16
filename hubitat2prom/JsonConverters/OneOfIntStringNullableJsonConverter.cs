using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf;

namespace hubitat2prom.JsonConverters;

public class OneOfIntStringNullableJsonConverter : JsonConverter<OneOf<int, string>?>
{
    private static OneOfIntStringJsonConverter oneOfIntStringJsonConverter = new OneOfIntStringJsonConverter();
    public override OneOf<int, string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        return oneOfIntStringJsonConverter.Read(ref reader, typeToConvert, options);
    }

    public override void Write(Utf8JsonWriter writer, OneOf<int, string>? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            oneOfIntStringJsonConverter.Write(writer, value.Value, options);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
