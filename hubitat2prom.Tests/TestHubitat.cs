using Xunit;

using hubitat2prom;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

namespace hubitat2prom.Tests;

public class TestHubitat
{
    private IHttpClientFactory _httpClientFactory;
    public TestHubitat(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [Fact]
    public async Task Devices_Returns_Summary()
    {
        var env = new HubitatEnv();
        var hubitat = new Hubitat(env.HE_URI, env.HE_TOKEN, _httpClientFactory);
        var devices = await hubitat.Devices();
        Assert.NotEmpty(devices);
        foreach(var device in devices)
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
        var env = new HubitatEnv();
        var hubitat = new Hubitat(env.HE_URI, env.HE_TOKEN, _httpClientFactory);
        var devices = await hubitat.Devices();

        var device = devices.First();

        var details = await hubitat.DeviceDetails(device);
        Assert.NotNull(details);
        Assert.NotEmpty(details.attributes);
        Assert.NotEmpty(details.capabilities);
    }

    [Fact]
    public async Task DeviceDetails_Attributes_All_Have_Names()
    {
        var env = new HubitatEnv();
        var hubitat = new Hubitat(env.HE_URI, env.HE_TOKEN, _httpClientFactory);
        var devices = await hubitat.Devices();

        var device = devices.First();

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
        var env = new HubitatEnv();
        var hubitat = new Hubitat(env.HE_URI, env.HE_TOKEN, _httpClientFactory);
        var devices = await hubitat.Devices();

        var device = devices.First();

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
        var env = new HubitatEnv();
        var hubitat = new Hubitat(env.HE_URI, env.HE_TOKEN, _httpClientFactory);
        var devices = await hubitat.Devices();

        var device = devices.First();

        var details = await hubitat.DeviceDetails(device);
        Assert.NotNull(details);
        Assert.NotEmpty(details.attributes);

        var values = from attribute in details.attributes
                     where attribute.dataType == "NUMBER"
                     select attribute.currentValue;

        foreach(var value in values)
        {
            // NUMBER is allowed to be null, so skip it
            if (!value.HasValue) continue;

            Assert.True(value.Value.IsT0);
        }
    }

    [Fact]
    public async Task DeviceDetails_Attribute_Enums_Are_Arrays()
    {
        var env = new HubitatEnv();
        var hubitat = new Hubitat(env.HE_URI, env.HE_TOKEN, _httpClientFactory);
        var devices = await hubitat.Devices();

        var device = devices.First();

        var details = await hubitat.DeviceDetails(device);
        Assert.NotNull(details);
        Assert.NotEmpty(details.attributes);

        var enums = from attribute in details.attributes
                     where attribute.dataType == "ENUM"
                     select attribute;

        foreach(var @enum in enums)
        {
            Assert.NotNull(@enum.values);
            Assert.IsAssignableFrom(typeof(Array), @enum.values);
        }
    }

    [Fact]
    public async Task DeviceDetails_Capabilities_Are_Strings_Or_Objects()
    {
        var env = new HubitatEnv();
        var hubitat = new Hubitat(env.HE_URI, env.HE_TOKEN, _httpClientFactory);
        var devices = await hubitat.Devices();

        var device = devices.First();

        var details = await hubitat.DeviceDetails(device);
        Assert.NotNull(details);
        Assert.NotEmpty(details.capabilities);

        foreach(var capability in details.capabilities)
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
}