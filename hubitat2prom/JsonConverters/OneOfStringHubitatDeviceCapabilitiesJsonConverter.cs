using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using hubitat2prom.HubitatModels;
using OneOf;

namespace hubitat2prom.JsonConverters;

public class OneOfStringHubitatDeviceCapabilitiesJsonConverter : JsonConverter<OneOf<string, HubitatDeviceCapabilities>?>
{
    public override OneOf<string, HubitatDeviceCapabilities>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString();
        }
        else if (reader.TokenType == JsonTokenType.StartObject)
        {
            var hubitatDeviceCapabilities = new HubitatDeviceCapabilities();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return hubitatDeviceCapabilities;
                }

                if (!(reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "attributes")) throw new JsonException("Device capabilities needs only an attributes property.");

                hubitatDeviceCapabilities.attributes = JsonSerializer.Deserialize<HubitatDeviceCapabilityAttributes[]>(ref reader, options);
            }
            throw new JsonException("Did not parse to end of object.");
        }
        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, OneOf<string, HubitatDeviceCapabilities>? value, JsonSerializerOptions options)
    {
        if (!value.HasValue)
        {
            writer.WriteNullValue();
        }
        else
        {
            value.Value.Switch(
                @string => writer.WriteStringValue(@string),
                capabilities =>
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("attributes");
                    writer.WriteStartArray();
                    foreach(var attribute in capabilities.attributes)
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("name");
                        writer.WriteStringValue(attribute.name);
                        writer.WritePropertyName("dataType");
                        writer.WriteStringValue(attribute.dataType);
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                }
            );
        }
    }
}