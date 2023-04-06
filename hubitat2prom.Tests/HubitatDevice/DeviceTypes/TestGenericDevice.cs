using hubitat2prom.PrometheusExporter.DeviceTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace hubitat2prom.Tests.HubitatDevice.DeviceTypes;

public class TestGenericDevice: TestBase
{
    // arbitrary device and attribute values used to trigger errors in GenericDevice in these tests
    private const string INVALID_DEVICE_NAME = "INVALID DEVICE NAME";
    private const string INVALID_ATTRIBUTE_VALUE = "INVALID ATTRIBUTE VALUE";
    // arbitrary device name to represent any kind of device in these tests
    private const string ANY_DEVICE_NAME = "ANY DEVICE";
    private const double MISSING_VALUE_DEFAULT = 0d;

    public TestGenericDevice(MockCreator mockCreator)
        : base(mockCreator)
    {
    }

    [Theory]
    [InlineData(INVALID_DEVICE_NAME, INVALID_ATTRIBUTE_VALUE, MISSING_VALUE_DEFAULT)]
    [InlineData("switch", "on", 1)]
    [InlineData("switch", "off", 0)]
    [InlineData("switch", INVALID_ATTRIBUTE_VALUE, MISSING_VALUE_DEFAULT)]
    [InlineData("power", "on", 1)]
    [InlineData("power", "off", 0)]
    [InlineData("power", INVALID_ATTRIBUTE_VALUE, MISSING_VALUE_DEFAULT)]
    [InlineData("thermostatOperatingState", "heating", 0)]
    [InlineData("thermostatOperatingState", "pending cool", 1)]
    [InlineData("thermostatOperatingState", "pending heat", 2)]
    [InlineData("thermostatOperatingState", "vent economizer", 3)]
    [InlineData("thermostatOperatingState", "idle", 4)]
    [InlineData("thermostatOperatingState", "cooling", 5)]
    [InlineData("thermostatOperatingState", "fan only", 6)]
    [InlineData("thermostatOperatingState", INVALID_ATTRIBUTE_VALUE, MISSING_VALUE_DEFAULT)]
    [InlineData("thermostatMode", "auto", 0)]
    [InlineData("thermostatMode", "off", 1)]
    [InlineData("thermostatMode", "heat", 2)]
    [InlineData("thermostatMode", "emergency heat", 3)]
    [InlineData("thermostatMode", "cool", 4)]
    [InlineData("thermostatMode", INVALID_ATTRIBUTE_VALUE, MISSING_VALUE_DEFAULT)]
    [InlineData("contact", "open", 0)]
    [InlineData("contact", "closed", 1)]
    [InlineData("contact", INVALID_ATTRIBUTE_VALUE, MISSING_VALUE_DEFAULT)]
    public void ExtractMetric_Returns_Correct_String_Attribute_State_Value(
        string attributeName,
        string attributeValue,
        double expectedValue)
    {
        var genericDevice = new GenericDevice();
        Assert.Equal(expectedValue, genericDevice.ExtractMetric(attributeName, attributeValue));
    }

    [Fact]
    public void ExtractMetric_Returns_Correct_String_Array_State_Value()
    {
        var genericDevice = new GenericDevice();
        Assert.Equal(MISSING_VALUE_DEFAULT, genericDevice.ExtractMetric("any device", new string[] { }));
    }

    [Theory]
    [InlineData(null, MISSING_VALUE_DEFAULT)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(int.MinValue, int.MinValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public void ExtractMetric_Returns_Correct_Nullable_Int_State_Value(
        int? attributeValue,
        double expectedValue)
    {
        // is there a different was to make an `int?` with literals?
        // this test needs to trigger the nullable int code path.
        if (attributeValue == null)
        {
            attributeValue = new int?();
        }

        var genericDevice = new GenericDevice();
        Assert.Equal(expectedValue, genericDevice.ExtractMetric("any device", attributeValue));
    }

    [Theory]
    [InlineData(null, MISSING_VALUE_DEFAULT)]
    [InlineData(0d, 0d)]
    [InlineData(1d, 1d)]
    [InlineData(double.MinValue, double.MinValue)]
    [InlineData(double.MaxValue, double.MaxValue)]
    public void ExtractMetric_Returns_Correct_Nullable_Double_State_Value(
        double? attributeValue,
        double expectedValue)
    {
        if (attributeValue == null)
        {
            attributeValue = new double?();
        }

        var genericDevice = new GenericDevice();
        Assert.Equal(expectedValue, genericDevice.ExtractMetric("any device", attributeValue));
    }
    /*
     * string
     * string[]
     * int?
     * double?
     * OneOf<double, string>?
     */
}
