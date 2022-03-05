using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using hubitat2prom.HubitatDevice;
using hubitat2prom.PrometheusModels;

namespace hubitat2prom;

public static class HubitatPrometheusMetricConverter
{
    const string INVALID_CHARACTER_REGEX = "[() -]";

    public static IEnumerable<HubitatDeviceMetricValue> GetPrometheusDeviceMetrics(IEnumerable<DeviceDetails> hubitatDeviceDetails, string[] collectedMetrics)
    {
        foreach (var deviceDetail in hubitatDeviceDetails)
            foreach (var attribute in deviceDetail.attributes)
            {
                if (!collectedMetrics.Contains(attribute.name)) continue;
                if (!attribute.currentValue.HasValue) continue;

                var metricName = Regex.Replace(attribute.name, INVALID_CHARACTER_REGEX, "_");
                var deviceName = Regex.Replace(deviceDetail.label, INVALID_CHARACTER_REGEX, "_");

                var rawMetricValue = attribute.currentValue.Value;
                var metricValue = 0d;
                var value = 0d;
                if (rawMetricValue.TryPickT0(out double rawNumericValue, out string rawStringValue))
                {
                    metricValue = rawNumericValue;
                }

                switch (metricName.ToLowerInvariant())
                {
                    case "switch":
                    case "watering":
                        metricValue = rawMetricValue.AsT1 == "on"
                            ? 1
                            : 0;
                        break;
                    case "power":
                        // the value of power is either "on," "off," or a double.
                        if (rawMetricValue.IsT1 && TryGetPower(rawStringValue, out value))
                        {
                            metricValue = value;
                        }
                        break;
                    case "thermostatoperatingstate":
                        if (TryGetThermostatOperatingState(rawMetricValue.AsT1, out value))
                        {
                            metricValue = value;
                        }
                        break;
                    case "thermostatmode":
                        if (TryGetThermostatMode(rawMetricValue.AsT1, out value))
                        {
                            metricValue = value;
                        }
                        break;
                }

                yield return new HubitatDeviceMetricValue
                {
                    DeviceName = deviceName,
                    MetricName = metricName,
                    MetricValue = metricValue,
                    MetricTimestamp = DateTime.UnixEpoch
                };
            }
    }

    public static IEnumerable<HubitatDeviceMetricValue> GetPrometheusDeviceMetrics(IEnumerable<DeviceDetailSummary> hubitatDeviceDetails, string[] collectedMetrics)
    {
        foreach (var deviceDetail in hubitatDeviceDetails)
            foreach (var attribute in deviceDetail.attributes)
            {
                var attributeName = attribute.Key;
                var attributeValue = attribute.Value;

                if (!collectedMetrics.Contains(attributeName)) continue;
                if (!attributeValue.HasValue) continue;

                var metricName = Regex.Replace(attributeName, INVALID_CHARACTER_REGEX, "_");
                var deviceName = Regex.Replace(deviceDetail.label, INVALID_CHARACTER_REGEX, "_");

                var rawMetricValue = attributeValue.Value;
                double value;
                var metricValue = rawMetricValue.Match(
                    @string =>
                    {
                        var name = attributeName.ToLowerInvariant();
                        if (name == "switch") return @string == "on" ? 1 : 0;
                        if (name == "power" && TryGetPower(@string, out value)) return value;
                        if (name == "thermostatoperatingstate" && TryGetThermostatOperatingState(@string, out value)) return value;
                        if (name == "thermostatmode" && TryGetThermostatMode(@string, out value)) return value;

                        return 0d;
                    },
                    stringArray => 0d,
                    nullableInt => nullableInt ?? 0d,
                    nullableDouble => nullableDouble ?? 0d,
                    nullableOneOfDoubleString => nullableOneOfDoubleString.HasValue
                        ? nullableOneOfDoubleString.Value.Match(
                            @double => @double,
                            @string => 0
                            )
                        : 0
                );

                yield return new HubitatDeviceMetricValue
                {
                    DeviceName = deviceName,
                    MetricName = metricName,
                    MetricValue = metricValue,
                    MetricTimestamp = DateTime.UnixEpoch
                };
            }
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