using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using hubitat2prom.HubitatDevice;
using hubitat2prom.PrometheusExporter.DeviceTypes;
using AttributeValue = OneOf.OneOf<string, string[], int?, double?, OneOf.OneOf<double, string>?>;

namespace hubitat2prom.PrometheusExporter;

public static class HubitatDeviceMetrics
{
    private const string INVALID_CHARACTER_REGEX = "[() -]";
    private static Regex _invalidCharacterRegex = new Regex(INVALID_CHARACTER_REGEX, RegexOptions.Compiled);

    /// <summary>
    /// Convert the metrics of Hubitat device details from a single device query,
    /// e.g., `/apps/api/712/devices/130`, into Prometheus exporter metrics.
    /// </summary>
    /// <param name="hubitatDeviceDetails"></param>
    /// <param name="collectedMetrics"></param>
    /// <returns></returns>
    public static IEnumerable<ExporterHubitatDeviceMetric> Export(IEnumerable<DeviceDetails> hubitatDeviceDetails, string[] collectedMetrics)
    {
        return hubitatDeviceDetails.SelectMany(deviceDetail =>
        {
            var firstAttribute = deviceDetail.attributes.First();
            var deviceSummaryAttributes = new DeviceSummaryAttributes
            {
                dataType = firstAttribute.dataType,
                values = firstAttribute.values
            };
            var deviceSummaryAttributesDict = (IDictionary<string, AttributeValue?>)deviceSummaryAttributes;
            foreach (var deviceAttribute in deviceDetail.attributes)
            {
                if (!deviceAttribute.currentValue.HasValue) continue;

                deviceSummaryAttributesDict.Add(deviceAttribute.name, deviceAttribute.currentValue);
            }

            var deviceDetailSummary = new DeviceDetailSummary
            {
                id = deviceDetail.id,
                label = deviceDetail.label,
                name = deviceDetail.name,
                @type = deviceDetail.type,
                attributes = deviceSummaryAttributes,
                capabilities = deviceDetail.capabilities.SelectMany(capability
                    => capability.HasValue
                        ? capability.Value.Match(
                            @string => new[] { @string },
                            deviceCapabilities => deviceCapabilities.attributes.Select(
                                capabilityAttribute => capabilityAttribute.name
                            )
                        )
                        : new string[0]
                    ).ToArray(),
                    commands = deviceDetail.commands.Select(command => new DeviceSummaryCommands
                    {
                        command = command
                    }).ToArray()
            };
            var deviceType = DeviceType.CreateDeviceType(deviceDetail);
            return ExtractAttributeMetrics(deviceType, deviceDetailSummary, collectedMetrics);
        });
    }

    /// <summary>
    /// Convert the metrics of Hubitat device details from a query for all devices,
    /// e.g., `/apps/api/712/devices/all`, into Prometheus exporter metrics.
    /// </summary>
    /// <param name="hubitatDeviceDetails"></param>
    /// <param name="collectedMetrics"></param>
    /// <returns></returns>
    public static IEnumerable<ExporterHubitatDeviceMetric> Export(IEnumerable<DeviceDetailSummary> hubitatDeviceDetails, string[] collectedMetrics)
    {
        return hubitatDeviceDetails.SelectMany(deviceDetail =>
        {
            var deviceType = DeviceType.CreateDeviceType(deviceDetail);
            return ExtractAttributeMetrics(deviceType, deviceDetail, collectedMetrics);
        });
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
    
    private static IEnumerable<ExporterHubitatDeviceMetric> ExtractAttributeMetrics(DeviceType deviceType, DeviceDetailSummary deviceDetail, string[] collectedMetrics)
    {
        foreach (var attribute in deviceDetail.attributes)
        {
            var attributeName = attribute.Key;
            var attributeValue = attribute.Value;

            if (!collectedMetrics.Contains(attributeName)) continue;
            if (!attributeValue.HasValue) continue;

            var metricName = Regex.Replace(attributeName, INVALID_CHARACTER_REGEX, "_");
            var deviceName = Regex.Replace(deviceDetail.label, INVALID_CHARACTER_REGEX, "_");

            var metricValue = deviceType.ExtractMetric(attributeName, attributeValue.Value);

            yield return new ExporterHubitatDeviceMetric
            {
                DeviceName = deviceName,
                MetricName = metricName,
                MetricValue = metricValue,
                MetricTimestamp = DateTime.UnixEpoch
            };
        }
    }
}
