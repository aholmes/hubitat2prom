using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using hubitat2prom.HubitatModels;

namespace hubitat2prom;
public class Hubitat
{
    private HubitatEnv _env;
    private Uri _baseUri;
    private HttpClient _httpClient;

    public Hubitat(HubitatEnv env, Uri baseUri, HttpClient httpClient)
    {
        _env = env;
        _baseUri = baseUri;
        _httpClient = httpClient;
    }

    private UriBuilder _authenticatedUriBuilder()
    {
        var uriBuilder = new UriBuilder(_baseUri);
        uriBuilder.Query = $"access_token={_env.HE_URI}";
        return uriBuilder;
    }

    public async Task<DeviceSummaryModel[]> Devices()
    {
        var uriBuilder = _authenticatedUriBuilder();

        var response = await _httpClient.GetAsync(uriBuilder.ToString());
        return await response.Content.ReadFromJsonAsync<DeviceSummaryModel[]>();
    }

    public async Task<dynamic> DeviceDetails(int deviceId)
    {
        var uriBuilder = _authenticatedUriBuilder();
        uriBuilder.PathAppend(deviceId.ToString());

        var response = await _httpClient.GetAsync(uriBuilder.ToString());
        var data = await response.Content.ReadFromJsonAsync<dynamic>();
        return data;
    }
}

public static class UriBuilderExtensions
{
    /**
    * Changes the UriBuilder.Path to include additional path
    * segments at the end of the existing Path.
    */
    public static void PathAppend(this UriBuilder uriBuilder, params string[] segments)
    {
        uriBuilder.Path = string.Join('/', new[] { uriBuilder.Path.TrimEnd('/') }
            .Concat(segments.Select(s => s.Trim('/'))));
    }
}