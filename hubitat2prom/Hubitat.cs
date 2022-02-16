using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using hubitat2prom.HubitatModels;
using OneOf;

namespace hubitat2prom;
public class Hubitat
{
    private Uri _baseUri;
    private string _authToken;
    private IHttpClientFactory _httpClientFactory;

    public Hubitat(Uri baseUri, Guid authToken, IHttpClientFactory httpClientFactory)
    {
        _baseUri = baseUri;
        _authToken = authToken.ToString("D");
        _httpClientFactory = httpClientFactory;
    }

    private UriBuilder _authenticatedUriBuilder()
    {
        var uriBuilder = new UriBuilder(_baseUri);
        uriBuilder.Query = $"access_token={_authToken}";
        return uriBuilder;
    }

    public async Task<HubitatDeviceSummary[]> Devices()
    {
        var uriBuilder = _authenticatedUriBuilder();

        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync(uriBuilder.ToString());
        var result = await response.Content.ReadFromJsonAsync<HubitatDeviceSummary[]>();
        return result;
    }

    public async Task<HubitatDeviceDetails> DeviceDetails(HubitatDeviceSummary device)
    {
        var uriBuilder = _authenticatedUriBuilder();
        uriBuilder.PathAppend(device.id.ToString());

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString
                | JsonNumberHandling.AllowNamedFloatingPointLiterals
            
        };
        jsonSerializerOptions.Converters.Add(new OneOfIntStringJsonConverter());
        jsonSerializerOptions.Converters.Add(new OneOfStringHubitatDeviceCapabilitiesJsonConverter());
        jsonSerializerOptions.UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode;

        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync(uriBuilder.ToString());
        var result = await response.Content.ReadFromJsonAsync<HubitatDeviceDetails>(jsonSerializerOptions);
        return result;
    }
}

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