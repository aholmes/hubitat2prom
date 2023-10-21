using hubitat2prom.PrometheusExporter.DeviceTypes;
using OneOf;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace hubitat2prom.Tests.HubitatDevice.DeviceTypes;

public class TestFlumeDevice: TestBase
{
    // arbitrary device and attribute values used to trigger errors in FlumeDevice in these tests
    private const string INVALID_DEVICE_NAME = "INVALID DEVICE NAME";
    private const string INVALID_ATTRIBUTE_VALUE = "INVALID ATTRIBUTE VALUE";
    private const double MISSING_VALUE_DEFAULT = 0d;

    public TestFlumeDevice(MockCreator mockCreator)
        : base(mockCreator)
    {
    }

    [Theory]
    [InlineData(INVALID_DEVICE_NAME, INVALID_ATTRIBUTE_VALUE, MISSING_VALUE_DEFAULT)]
    [InlineData("usagelastday", "111.02", 111.02)]
    [InlineData("usagelasthour", "0.0", 0d)]
    [InlineData("usagelastmonth", "2594.13", 2594.13)]
    [InlineData("usagelastweek", "548.59", 548.59)]
    [InlineData("usagelastminute", "0.0", 0d)]
    [InlineData("flowdurationmin", "0", 0d)]
    [InlineData("water", "dry", 0d)]
    [InlineData("water", "wet", 1d)]
    [InlineData("flowstatus", "stopped", 0d)]
    [InlineData("flowstatus", "running", 1d)]
    [InlineData("flowstatus", "monitoring", 2d)]
    [InlineData("commstatus", "good", 0d)]
    [InlineData("commstatus", "unknown", 1d)]
    [InlineData("commstatus", "error", 2d)]
    [InlineData("presence", "not present", 0d)]
    [InlineData("presence", "present", 1d)]
    public void ExtractMetric_Returns_Correct_String_Attribute_State_Value(
        string attributeName,
        string attributeValue,
        double expectedValue)
    {
        var genericDevice = new FlumeDevice();
        Assert.Equal(expectedValue, genericDevice.ExtractMetric(attributeName, attributeValue));
    }
}
