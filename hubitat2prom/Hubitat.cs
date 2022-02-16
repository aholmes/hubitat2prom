using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using hubitat2prom.HubitatModels;
using hubitat2prom.JsonConverters;

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

    public async Task<HttpStatusCode> StatusCheck()
    {
        var uriBuilder = _authenticatedUriBuilder();

        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync(uriBuilder.ToString());
        return response.StatusCode;
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

        var jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.Converters.Add(new OneOfIntStringJsonConverter());
        jsonSerializerOptions.Converters.Add(new OneOfStringHubitatDeviceCapabilitiesJsonConverter());

        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync(uriBuilder.ToString());
        var result = await response.Content.ReadFromJsonAsync<HubitatDeviceDetails>(jsonSerializerOptions);
        return result;
    }
}
