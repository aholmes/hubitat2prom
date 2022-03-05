using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using hubitat2prom.HubitatDevice;
using hubitat2prom.JsonConverters;

namespace hubitat2prom;
public class Hubitat
{
    private Uri _baseUri;
    private string _authToken;
    private IHttpClientFactory _httpClientFactory;
    private HttpClient _httpClient => _httpClientFactory.CreateClient();

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

    public async Task<HttpStatusCode> StatusCheck()
    {
        var uriBuilder = _authenticatedUriBuilder();

        var response = await _httpClient.GetAsync(uriBuilder.ToString());
        return response.StatusCode;
    }

    public async Task<DeviceSummary[]> Devices()
    {
        var uriBuilder = _authenticatedUriBuilder();

        var result = await _httpClient.GetFromJsonAsync<DeviceSummary[]>(uriBuilder.ToString());
        return result;
    }

    public Task<DeviceDetails> DeviceDetails(DeviceSummary device) => DeviceDetails(int.Parse(device.id));
    public async Task<DeviceDetails> DeviceDetails(int deviceId)
    {
        var uriBuilder = _authenticatedUriBuilder();
        uriBuilder.PathAppend(deviceId.ToString());

        var jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.Converters.Add(new OneOfDoubleStringNullableJsonConverter());
        jsonSerializerOptions.Converters.Add(new OneOfStringHubitatDeviceCapabilitiesJsonConverter());

        var result = await _httpClient.GetFromJsonAsync<DeviceDetails>(uriBuilder.ToString(), jsonSerializerOptions);
        return result;
    }

    public async Task<DeviceDetailSummary[]> DeviceDetails()
    {
        var uriBuilder = _authenticatedUriBuilder();
        uriBuilder.PathAppend("all");

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
        jsonSerializerOptions.Converters.Add(new OneOfDoubleStringNullableJsonConverter());
        jsonSerializerOptions.Converters.Add(new OneOfStringHubitatDeviceCapabilitiesJsonConverter());

        var result = await _httpClient.GetFromJsonAsync<DeviceDetailSummary[]>(uriBuilder.ToString(), jsonSerializerOptions);
        return result;
    }
}