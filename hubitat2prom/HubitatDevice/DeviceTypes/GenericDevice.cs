using AttributeValue = OneOf.OneOf<string, string[], int?, double?, OneOf.OneOf<double, string>?>;

namespace hubitat2prom.PrometheusExporter.DeviceTypes;

public class GenericDevice: DeviceType
{
    public GenericDevice() { }

    public override double ExtractMetric(string attributeName, AttributeValue attributeValue)
    {
        double value;
        var metricValue = attributeValue.Match(
            @string =>
            {
                var name = attributeName.ToLowerInvariant();
                if (name == "switch") return @string == "on" ? 1 : 0;
                if (name == "power" && TryGetPower(@string, out value)) return value;
                if (name == "thermostatoperatingstate" && TryGetThermostatOperatingState(@string, out value)) return value;
                if (name == "thermostatmode" && TryGetThermostatMode(@string, out value)) return value;
                if (name == "contact") return @string == "closed" ? 1 : 0;
                
                return MISSING_VALUE_DEFAULT;
            },
            stringArray => MISSING_VALUE_DEFAULT,
            nullableInt => nullableInt ?? MISSING_VALUE_DEFAULT,
            nullableDouble => nullableDouble ?? MISSING_VALUE_DEFAULT,
            nullableOneOfDoubleString => nullableOneOfDoubleString.HasValue
                ? nullableOneOfDoubleString.Value.Match(
                    @double => @double,
                    @string => MISSING_VALUE_DEFAULT
                    )
                : MISSING_VALUE_DEFAULT
        );

        return metricValue;
    }

    private static bool TryGetPower(string power, out double value)
    {
        if (power == "off")
        {
            value = 0;
            return true;
        }

        if (power == "on")
        {
            value = 1;
            return true;
        }

        value = -1;
        return false;
    }

    private static bool TryGetThermostatOperatingState(string thermostatOperatingState, out double value)
    {
        switch (thermostatOperatingState)
        {
            case "heating": value = 0; return true;
            case "pending cool": value = 1; return true;
            case "pending heat": value = 2; return true;
            case "vent economizer": value = 3; return true;
            case "idle": value = 4; return true;
            case "cooling": value = 5; return true;
            case "fan only": value = 6; return true;
        }

        value = -1;
        return false;
    }

    private static bool TryGetThermostatMode(string thermostatMode, out double value)
    {
        switch (thermostatMode)
        {
            case "auto": value = 0; return true;
            case "off": value = 1; return true;
            case "heat": value = 2; return true;
            case "emergency heat": value = 3; return true;
            case "cool": value = 4; return true;
        }

        value = -1;
        return false;
    }
}