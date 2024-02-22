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
using OneOf;
using System.Dynamic;

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
    [InlineData(123d, null, 123d)]
    [InlineData(123.4d, null, 123.4d)]
    [InlineData(null, "123", 123d)]
    [InlineData(null, "123.4", 123.4d)]
    public async Task DeviceMetricsExporter_Returns_Dynamic_Device_Attribute_Values(
        double? jsonDoubleValue, string jsonStringValue, double expectedValue
    )
    {
        var expectedValueFormatted = jsonDoubleValue.HasValue ? jsonDoubleValue.ToString() : ("\"" + jsonStringValue + "\"");
        var httpClientFactory = _getAllDeviceDetailsIHttpClientFactory(
            (json) =>
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json.Replace(
                        "\"freeMemory\": 123",
                        $"\"freeMemory\": {expectedValueFormatted}"
                    ))
                };
            });
        var hubitat = new Hubitat(_env.HE_URI, _env.HE_TOKEN, httpClientFactory);

        var details = await hubitat.DeviceDetails();
        var heMetrics = _env.HE_METRICS.Concat(["freeMemory"]).ToArray();
        var deviceAttributes = HubitatDeviceMetrics.Export(details, heMetrics).Single(o => o.MetricName == "freeMemory");

        if (jsonDoubleValue != null)
        {
            Assert.Equal(expectedValue, deviceAttributes.MetricValue);
        }
        else
        {
            _ = double.TryParse(jsonStringValue, out double value);
            Assert.Equal(expectedValue, deviceAttributes.MetricValue);
        }
    }

    [Theory]
    [InlineData(123, "123.0")]
    [InlineData(123.4, "123.4")]
    public async Task GetMetrics_Returns_Dynamic_Device_Attribute_Values(
        double jsonValue, string expectedValue
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
                        $"\"freeMemory\": \"{jsonValue}\""
                    ))
                };
            });
        var env = new HubitatEnv(new Uri("http://example.org"), Guid.NewGuid(), ["freeMemory"]);
        var hubitat = new Hubitat(env.HE_URI, env.HE_TOKEN, httpClientFactory);
        var hubitatController = new HubitatController(hubitat, env, new Mock<ILogger<HubitatController>>().Object);

        var metrics = await hubitatController.GetMetrics();

        Assert.Equal($"hubitat_freememory{{device_name=\"hub_information\"}} {expectedValue}", metrics.Content?.Trim());
    }

    [Theory]
    [InlineData("switch", "on", 1)]
    [InlineData("switch", "off", 0)]
    [InlineData("switch", "", 0)]
    [InlineData("power", "on", 1)]
    [InlineData("power", "off", 0)]
    [InlineData("power", 1.2, 1.2)]
    [InlineData("power", "", 0)]
    [InlineData("thermostatoperatingstate", "heating", 0)]
    [InlineData("thermostatoperatingstate", "pending cool", 1)]
    [InlineData("thermostatoperatingstate", "pending heat", 2)]
    [InlineData("thermostatoperatingstate", "vent economizer", 3)]
    [InlineData("thermostatoperatingstate", "idle", 4)]
    [InlineData("thermostatoperatingstate", "cooling", 5)]
    [InlineData("thermostatoperatingstate", "fan only", 6)]
    [InlineData("thermostatoperatingstate", "", 0)]
    [InlineData("thermostatmode", "auto", 0)]
    [InlineData("thermostatmode", "off", 1)]
    [InlineData("thermostatmode", "heat", 2)]
    [InlineData("thermostatmode", "emergency heat", 3)]
    [InlineData("thermostatmode", "cool", 4)]
    [InlineData("thermostatmode", "", 0)]
    [InlineData("contact", "closed", 1)]
    [InlineData("contact", "not closed", 0)]
    [InlineData("contact", "", 0)]
    [InlineData("presence", "present", 1)]
    [InlineData("presence", "not present", 0)]
    [InlineData("presence", "", 0)]
    public async Task DeviceMetricsExporter_Exports_Expected_Mapped_string_Values(
        string attributeName, OneOf<string, double> jsonValue, double expectedValue
    )
    {
        var value = jsonValue.IsT0
            ? $"\"{jsonValue.AsT0}\""
            : jsonValue.AsT1.ToString();

        var httpClientFactory = _getAllDeviceDetailsIHttpClientFactory(
            (json) =>
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json.Replace(
                        "\"attributes\": {}",
                        $"\"attributes\": {{ \"{attributeName}\": \"{value}\" }}"
                    ))
                };
            });
        var env = new HubitatEnv(new Uri("http://example.org"), Guid.NewGuid(), ["freeMemory"]);
        var hubitat = new Hubitat(env.HE_URI, env.HE_TOKEN, httpClientFactory);

        var details = (await hubitat.DeviceDetails()).Single(o => o.name == "Dummy");

        var heMetrics = _env.HE_METRICS.Concat([attributeName]).ToArray();
        var deviceAttributes = HubitatDeviceMetrics.Export(new[] { details }, heMetrics).Single(o => o.MetricName == attributeName);

        Assert.Equal(expectedValue, deviceAttributes.MetricValue);
    }
}