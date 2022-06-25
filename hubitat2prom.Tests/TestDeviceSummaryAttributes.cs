
using System.Net.Http;
using System.Net;
using System;
using System.Text.Json;
using System.IO;
using System.Reflection;
using hubitat2prom.HubitatDevice;
using Xunit;
using Microsoft.CSharp.RuntimeBinder;
using System.Collections.Generic;
using AttributeValue = OneOf.OneOf<string, string[], int?, double?, OneOf.OneOf<double, string>?>;
using System.Dynamic;

namespace hubitat2prom.Tests;

public class TestDeviceSummaryAttributes
{
    private MockCreator _mockCreator;
    private HubitatEnv _env;

    public TestDeviceSummaryAttributes(MockCreator mockCreator)
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
    private IHttpClientFactory _getAllDeviceDetailsIHttpClientFactory() =>
        _mockCreator.GetIHttpClientFactory(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(_getAllDeviceDetailsJson())
        });

    // tests that the DynamicObject extension doesn't interfere with statically typed members
    [Fact]
    public void DeviceSummaryAttributes_Uses_Typed_Members()
    {
        var deviceSummaryAttributes = new DeviceSummaryAttributes
        {
            dataType = "123"
        };
        Assert.Equal("123", deviceSummaryAttributes.dataType);
    }

    [Fact]
    public void DeviceSummaryAttributes_Uses_Typed_Methods()
    {
        var deviceSummaryAttributes = new DeviceSummaryAttributes();
#pragma warning disable CS8974 // GetEnumerator is used correctly here so we can test its type
        Assert.IsType<Func<IEnumerator<KeyValuePair<string, AttributeValue?>>>>(deviceSummaryAttributes.GetEnumerator);
#pragma warning restore CS8974
    }

    [Fact]
    public void DeviceSummaryAttributes_Enumerates_Typed_Members()
    {
        var deviceSummaryAttributes = new DeviceSummaryAttributes
        {
            dataType = "123"
        };

        var expectedPropertyIterated = false;
        foreach (var member in deviceSummaryAttributes)
        {
            if (member.Key == "dataType")
            {
                expectedPropertyIterated = true;
                Assert.True(member.Value.HasValue);
#pragma warning disable CS8629 // Null is asserted with above line
                Assert.IsType<string>(member.Value.Value.Value);
#pragma warning restore CS8629
                Assert.Equal("123", member.Value.Value.Value);
                break;
            }
        }
        Assert.True(expectedPropertyIterated);
    }

    [Fact]
    public void DeviceSummaryAttributes_Allows_Setting_Arbitrary_Properties()
    {
        dynamic deviceSummaryAttributes = new DeviceSummaryAttributes();
        deviceSummaryAttributes.abc123 = 123;
    }

    [Fact]
    public void DeviceSummaryAttributes_Allows_Getting_Arbitrary_Properties_That_Have_Been_Set()
    {
        dynamic deviceSummaryAttributes = new DeviceSummaryAttributes();
        deviceSummaryAttributes.abc123 = "123";
        Assert.Equal("123", deviceSummaryAttributes.abc123);
    }

    [Fact]
    public void DeviceSummaryAttributes_Disallows_Getting_Arbitrary_Properties_That_Have_Not_Been_Set()
    {
        dynamic deviceSummaryAttributes = new DeviceSummaryAttributes();
        Assert.Throws<RuntimeBinderException>(() => deviceSummaryAttributes.abc123);
    }

    [Fact]
    public void DeviceSummaryAttributes_Allows_Setting_Arbitrary_Methods()
    {
        dynamic deviceSummaryAttributes = new DeviceSummaryAttributes();
        Func<int> func = () => 1;
        deviceSummaryAttributes.callabc = func;
        Assert.IsType<Func<int>>(deviceSummaryAttributes.callabc);
    }

    [Fact]
    public void DeviceSummaryAttributes_Allows_Calling_Arbitrary_Methods_That_Have_Been_Set()
    {
        dynamic deviceSummaryAttributes = new DeviceSummaryAttributes();
        Func<int> func = () => 1;
        deviceSummaryAttributes.callabc = func;
        Assert.Equal(1, deviceSummaryAttributes.callabc());
    }

    [Fact]
    public void DeviceSummaryAttributes_Disallows_Calling_Arbitrary_Methods_That_Have_Not_Been_Set()
    {
        dynamic deviceSummaryAttributes = new DeviceSummaryAttributes();
        Assert.Throws<RuntimeBinderException>(() => deviceSummaryAttributes.callabc());
    }

    [Fact]
    public void DeviceSummaryAttributes_Correctly_Maps_Typed_Properties_To_Instance_Property_Values()
    {
        var deviceSummaryAttributes = new DeviceSummaryAttributes();

        const string expectedValue = "1";

        var setMemberBinder = new TestSetMemberBinder("dataType", true);
        deviceSummaryAttributes.TrySetMember(setMemberBinder, expectedValue);

        Assert.Equal(expectedValue, deviceSummaryAttributes.dataType);
    }

    [Fact]
    public void DeviceSummaryAttributes_Correctly_Maps_Untyped_Properties_To_Instance_Property_Values()
    {
        dynamic deviceSummaryAttributes = new DeviceSummaryAttributes();

        const int expectedValue = 1;

        var setMemberBinder = new TestSetMemberBinder("abc123", true);
        deviceSummaryAttributes.TrySetMember(setMemberBinder, expectedValue);

        Assert.Equal(expectedValue, deviceSummaryAttributes.abc123);
    }


    private class TestSetMemberBinder : SetMemberBinder
    {
        public TestSetMemberBinder(string name, bool ignoreCase) : base(name, ignoreCase) { }

        public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject? errorSuggestion)
            => throw new NotImplementedException();
    }
}