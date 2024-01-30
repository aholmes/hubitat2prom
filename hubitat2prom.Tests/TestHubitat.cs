using Xunit;

using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text.Json;
using System.IO;
using System.Reflection;
using hubitat2prom.HubitatDevice;
using hubitat2prom.Controllers;
using Moq;
using Microsoft.Extensions.Logging;
using hubitat2prom.PrometheusExporter;

namespace hubitat2prom.Tests;

public class TestHubitat
{
    private MockCreator _mockCreator;
    private HubitatEnv _env;

    public TestHubitat(MockCreator mockCreator)
    {
        _mockCreator = mockCreator;
        _env = new HubitatEnv(new Uri("http://example.org"), Guid.NewGuid());
    }

    private Lazy<Assembly> _assembly = new Lazy<Assembly>(() => typeof(hubitat2prom.Tests.TestHubitat).Assembly);
    private string _readResourceJson(string resourceName)
    {
        var resourceStream = _assembly.Value.GetManifestResourceStream($"{_assembly.Value.GetName().Name}.Data.{resourceName}.json")!;
        using (var streamReader = new StreamReader(resourceStream)) return streamReader.ReadToEnd();
    }

    private string _getDeviceInfoJson() => _readResourceJson("DeviceInfo");
    private DeviceSummary[] _getDeviceInfo()
        => JsonSerializer.Deserialize<DeviceSummary[]>(_getDeviceInfoJson())!;
    private IHttpClientFactory _getDeviceInfoIHttpClientFactory()
        => _mockCreator.GetIHttpClientFactory(
            new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(_getDeviceInfoJson())
            });

    private string _getDeviceDetailsJson() => _readResourceJson("DeviceDetails");
    private DeviceDetails _getDeviceDetails()
        => JsonSerializer.Deserialize<DeviceDetails>(_getDeviceDetailsJson())!;
    private IHttpClientFactory _getDeviceDetailsIHttpClientFactory() =>
        _mockCreator.GetIHttpClientFactory(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(_getDeviceDetailsJson())
        });

    private string _getAllDeviceDetailsJson() => _readResourceJson("AllDeviceDetails");
    private DeviceDetailSummary[] _getAllDeviceDetails()
        => JsonSerializer.Deserialize<DeviceDetailSummary[]>(_getAllDeviceDetailsJson())!;
    private IHttpClientFactory _getAllDeviceDetailsIHttpClientFactory(Func<string, HttpResponseMessage>? httpResponseMessageCreator = null) =>
        _mockCreator.GetIHttpClientFactory(
            httpResponseMessageCreator?.Invoke(_getAllDeviceDetailsJson())
            ?? new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(_getAllDeviceDetailsJson())
            });

    [Fact]
    public async Task Devices_Returns_Summary()
    {

        var httpClientFactory = _getDeviceInfoIHttpClientFactory();

        var hubitat = new Hubitat(_env.HE_URI, _env.HE_TOKEN, httpClientFactory);
        var devices = await hubitat.Devices();
        Assert.NotEmpty(devices);
        foreach (var device in devices)
        {
            Assert.NotEmpty(device.id);
            Assert.NotEmpty(device.name);
            Assert.NotEmpty(device.label);
            Assert.NotEmpty(device.type);
        }
    }

    [Fact]
    public async Task DeviceDetails_Returns_Details()
    {
        var httpClientFactory = _getDeviceDetailsIHttpClientFactory();
        var hubitat = new Hubitat(_env.HE_URI, _env.HE_TOKEN, httpClientFactory);
        var device = _getDeviceInfo().First();

        var details = await hubitat.DeviceDetails(device);
        Assert.NotNull(details);
        Assert.NotEmpty(details.attributes);
        Assert.NotEmpty(details.capabilities);
    }

    [Fact]
    public async Task DeviceDetails_Attributes_All_Have_Names()
    {
        var httpClientFactory = _getDeviceDetailsIHttpClientFactory();
        var hubitat = new Hubitat(_env.HE_URI, _env.HE_TOKEN, httpClientFactory);
        var device = _getDeviceInfo().First();

        var details = await hubitat.DeviceDetails(device);
        Assert.NotNull(details);
        Assert.NotEmpty(details.attributes);

        Assert.DoesNotContain(null,
            from attribute in details.attributes
            select attribute.name
        );
    }

    [Fact]
    public async Task DeviceDetails_Attributes_All_Have_DataTypes()
    {
        var httpClientFactory = _getDeviceDetailsIHttpClientFactory();
        var hubitat = new Hubitat(_env.HE_URI, _env.HE_TOKEN, httpClientFactory);
        var device = _getDeviceInfo().First();

        var details = await hubitat.DeviceDetails(device);
        Assert.NotNull(details);
        Assert.NotEmpty(details.attributes);

        Assert.DoesNotContain(null,
            from attribute in details.attributes
            select attribute.dataType
        );
    }

    [Fact]
    public async Task DeviceDetails_Attribute_Numbers_Are_Null_Or_Numeric()
    {
        var httpClientFactory = _getDeviceDetailsIHttpClientFactory();
        var hubitat = new Hubitat(_env.HE_URI, _env.HE_TOKEN, httpClientFactory);
        var device = _getDeviceInfo().First();

        var details = await hubitat.DeviceDetails(device);
        Assert.NotNull(details);
        Assert.NotEmpty(details.attributes);

        var values = from attribute in details.attributes
                     where attribute.dataType == "NUMBER"
                     select attribute.currentValue;

        foreach (var value in values)
        {
            // NUMBER is allowed to be null, so skip it
            if (!value.HasValue) continue;

            Assert.True(value.Value.IsT0);
        }
    }

    [Fact]
    public async Task DeviceDetails_Attribute_Enums_Are_Arrays()
    {
        var httpClientFactory = _getDeviceDetailsIHttpClientFactory();
        var hubitat = new Hubitat(_env.HE_URI, _env.HE_TOKEN, httpClientFactory);
        var device = _getDeviceInfo().First();

        var details = await hubitat.DeviceDetails(device);
        Assert.NotNull(details);
        Assert.NotEmpty(details.attributes);

        var enums = from attribute in details.attributes
                    where attribute.dataType == "ENUM"
                    select attribute;

        foreach (var @enum in enums)
        {
            Assert.NotNull(@enum.values);
            Assert.IsAssignableFrom<Array>(@enum.values);
        }
    }

    [Fact]
    public async Task DeviceDetails_Capabilities_Are_Strings_Or_Objects()
    {
        var httpClientFactory = _getDeviceDetailsIHttpClientFactory();
        var hubitat = new Hubitat(_env.HE_URI, _env.HE_TOKEN, httpClientFactory);
        var device = _getDeviceInfo().First();

        var details = await hubitat.DeviceDetails(device);
        Assert.NotNull(details);
        Assert.NotEmpty(details.capabilities);

        foreach (var capability in details.capabilities)
        {
            Assert.True(capability.HasValue);

#pragma warning disable CS8629 // `capability` cannot be null because Assert.True would throw above
            capability
#pragma warning restore CS8629
                .Value
                .Switch(
                    @string => Assert.NotEmpty(@string),
                    hubitatDeviceCapabilities => Assert.NotEmpty(hubitatDeviceCapabilities.attributes)
                );
        }
    }

    [Fact]
    public async Task DeviceDetails_Returns_DetailSummary()
    {
        var httpClientFactory = _getAllDeviceDetailsIHttpClientFactory();
        var hubitat = new Hubitat(_env.HE_URI, _env.HE_TOKEN, httpClientFactory);

        var details = await hubitat.DeviceDetails();
        Assert.NotNull(details);
        Assert.NotEmpty(from detail in details select detail.attributes);
        Assert.NotEmpty(from detail in details select detail.capabilities);
        Assert.NotEmpty(from detail in details select detail.commands);
    }

    [Fact]
    public async Task DeviceDetails_Returns_All_Device_Details_When_Some_Attributes_Are_Invalid()
    {
        var httpClientFactory = _getAllDeviceDetailsIHttpClientFactory(
            (json) =>
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json.Replace(
                        "\"battery\": \"73\"",
                        "\"battery\": \"undefined\"" // a string "undefined" is invalid for a decimal
                    ))
                };
            });
        var hubitat = new Hubitat(_env.HE_URI, _env.HE_TOKEN, httpClientFactory);

        var details = await hubitat.DeviceDetails();
        Assert.NotNull(details);
        Assert.Empty(details.Where(o => o.attributes == null));
        Assert.Empty(details.Where(o => o.capabilities == null));
        Assert.Empty(details.Where(o => o.commands == null));

        var attributes = details.First(o => o.name == "Hygrometer - Living Room").attributes;
        Assert.Null(attributes.battery);
        Assert.NotNull(attributes.humidity);
        Assert.True(attributes.humidity > 0);
    }

    [Fact]
    public async Task DeviceDetails_Returns_Dynamic_Device_Attribute_Values()
    {
        var httpClientFactory = _getAllDeviceDetailsIHttpClientFactory();
        var hubitat = new Hubitat(_env.HE_URI, _env.HE_TOKEN, httpClientFactory);

        var details = await hubitat.DeviceDetails();

        var attributes = (dynamic)details.First(o => o.name == "Hub Information").attributes;
        Assert.NotNull(attributes.freeMemory);
        Assert.Equal(123d, attributes.freeMemory);
    }

    [Theory]
    [InlineData(123, 123d)]
    [InlineData(123.4, 123.4d)]
    public async Task DeviceMetricsExporter_Returns_Dynamic_Device_Attribute_Values(
        double jsonValue, double expectedValue
    )
    {
        var httpClientFactory = _getAllDeviceDetailsIHttpClientFactory(
            (json) =>
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json.Replace(
                        "\"freeMemory\": 123",
                        $"\"freeMemory\": {jsonValue}"
                    ))
                };
            });
        var hubitat = new Hubitat(_env.HE_URI, _env.HE_TOKEN, httpClientFactory);

        var details = await hubitat.DeviceDetails();
        var heMetrics = _env.HE_METRICS.Concat(["freeMemory"]).ToArray();
        var deviceAttributes = HubitatDeviceMetrics.Export(details, heMetrics).Single(o => o.MetricName == "freeMemory");
        Assert.Equal(expectedValue, deviceAttributes.MetricValue);
    }
}