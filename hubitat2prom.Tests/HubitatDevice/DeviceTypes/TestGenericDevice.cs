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
    // an arbitrary invalid attribute value used to trigger errors in GenericDevice
    private const string INVALID_DEVICE_NAME = "INVALID DEVICE NAME";
    private const string INVALID_ATTRIBUTE_VALUE = "INVALID ATTRIBUTE VALUE";
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
        /*
         * string
         * string[]
         * int?
         * double?
         * OneOf<double, string>?
         */
    }
}
