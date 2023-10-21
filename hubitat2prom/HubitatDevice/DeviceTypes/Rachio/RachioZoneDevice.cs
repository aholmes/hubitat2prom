using hubitat2prom.PrometheusExporter.DeviceTypes;
using AttributeValue = OneOf.OneOf<string, string[], int?, double?, OneOf.OneOf<double, string>?>;

namespace hubitat2prom.HubitatDevice.DeviceTypes.Rachio;


/// <summary>
/// Rachio Zone Device. The JSON response is structured like:
///  {
///    "name": "Rachio Zone",
///    "label": "Rachio - Front (Left) And Side",
///    "type": "Rachio Zone",
///    "id": "1390",
///    "date": "2023-10-21T20:20:16+0000",
///    "model": null,
///    "manufacturer": null,
///    "room": null,
///    "capabilities": [
///      "Actuator",
///      "Refresh",
///      "Polling",
///      "Valve",
///      "Switch",
///      "Sensor",
///      "HealthCheck"
///    ],
///    "attributes": {
///      "inStandby": "false",
///      "dataType": "NUMBER",
///      "values": null,
///      "isCycling": "False",
///      "zoneSquareFeet": "500",
///      "nozzleName": "Bubbler",
///      "watering": "off",
///      "zoneWaterTime": "10",
///      "availableWater": "0.15000000596046448",
///      "nextRunStr": "Oct 29, 2023 - 6:04:00 PM",
///      "cropName": "Xeriscape",
///      "lastRunStr": "Oct 14, 2023 - 1:07:36 PM",
///      "efficiency": "0.8999999761581421",
///      "checkInterval": null,
///      "zoneCycleCount": "0",
///      "zoneTotalDuration": "3179",
///      "DeviceWatch-DeviceStatus": "online",
///      "lastRunEndTime": "1697339262000",
///      "shadeName": "Some Shade",
///      "depthOfWater": "0.83",
///      "zoneRunStatus": "Status: Idle",
///      "slopeName": "Zero Three",
///      "rootZoneDepth": "11",
///      "lastRun": "1697339256000",
///      "lastUpdatedDt": "Oct 21, 2023 - 1:20:16 PM",
///      "nextRun": "1698653040000",
///      "valve": "closed",
///      "notificationMessage": null,
///      "zoneDuration": "null",
///      "zoneNumber": "1",
///      "zoneElapsed": "null",
///      "soilName": "Clay",
///      "zoneStartDate": "Not Active",
///      "saturatedDepthOfWater": "0.91",
///      "zoneName": "Front (Left) And Side",
///      "switch": "off",
///      "scheduleType": "Off",
///      "maxRuntime": "10800"
///    },
///    "commands": [
///      {
///        "command": "close"
///      },
///      {
///    "command": "decZoneWaterTime"
///      },
///      {
///    "command": "incZoneWaterTime"
///      },
///      {
///    "command": "off"
///      },
///      {
///    "command": "on"
///      },
///      {
///    "command": "open"
///      },
///      {
///    "command": "pause"
///      },
///      {
///    "command": "ping"
///      },
///      {
///    "command": "poll"
///      },
///      {
///    "command": "refresh"
///      },
///      {
///    "command": "setZoneWaterTime"
///      },
///      {
///    "command": "startZone"
///      },
///      {
///    "command": "stopWatering"
///      }
///    ]
///  }
/// </summary>
public class RachioZoneDevice : GenericDevice
{
    public RachioZoneDevice() { }

    public override double ExtractMetric(string attributeName, AttributeValue attributeValue)
    {
        // If the value is not a string, allow the generic parser to extract the metric.
        if (!attributeValue.IsT0) return base.ExtractMetric(attributeName, attributeValue);
        var attributeStringValue = attributeValue.AsT0;
        double value;
        switch (attributeName)
        {
            case "zonerunstatus":
                if (TryGetZoneRunStatus(attributeStringValue, out value)) return value;
                break;
            case "valve":
                if (TryGetValve(attributeStringValue, out value)) return value;
                break;
            case "devicewatch-devicestatus":
                if (TryGetDeviceWatchDeviceStatus(attributeStringValue, out value)) return value;
                break;
            default:
                System.Diagnostics.Debug.WriteLine($"Unknown attribute \"{attributeName}\" from Flume device.");
                break;
        }

        // On the off chance the string value is numeric, allow the generic _string_ parser to extract the metric.
        return base.ExtractNumericMetric(attributeName, attributeStringValue);
    }

    private static bool TryGetZoneRunStatus(string zoneRunStatus, out double value)
    {
        switch (zoneRunStatus)
        {
            case "Status: Idle": value = 0; return true;
            // TODO are these correct?
            case "Device is Offline": value = 1; return true;
            case "Device in Standby Mode": value = 2; return true;
        }

        value = -1;
        return false;
    }

    private static bool TryGetValve(string valve, out double value)
    {
        switch (valve)
        {
            case "close": value = 0; return true;
            case "open": value = 1; return true;
        }

        value = -1;
        return false;
    }

    private static bool TryGetDeviceWatchDeviceStatus(string deviceWatchDeviceStatus, out double value)
    {
        switch (deviceWatchDeviceStatus)
        {
            case "offline": value = 0; return true;
            case "online": value = 1; return true;
        }

        value = -1;
        return false;
    }
}