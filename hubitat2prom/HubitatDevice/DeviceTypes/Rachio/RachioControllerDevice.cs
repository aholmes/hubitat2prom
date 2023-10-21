using hubitat2prom.PrometheusExporter.DeviceTypes;
using AttributeValue = OneOf.OneOf<string, string[], int?, double?, OneOf.OneOf<double, string>?>;

namespace hubitat2prom.HubitatDevice.DeviceTypes.Rachio;


/// <summary>
/// Rachio Controller Device. The JSON response is structured like:
///  {
///    "name": "Rachio Controller",
///    "label": "Rachio - Rachio",
///    "type": "Rachio Controller",
///    "id": "1389",
///    "date": "2023-10-21T20:20:16+0000",
///    "model": null,
///    "manufacturer": null,
///    "room": null,
///    "capabilities": [
///      "Actuator",
///      "Refresh",
///      "Polling",
///      "WaterSensor",
///      "Valve",
///      "Switch",
///      "Sensor",
///      "HealthCheck"
///    ],
///    "attributes": {
///      "rainSensorTripped": "false",
///      "dataType": "STRING",
///      "values": null,
///      "activeZoneCnt": "3",
///      "checkInterval": null,
///      "nextEventTime": "Oct 26, 2023 - 6:40:07 AM",
///      "controllerOn": "true",
///      "curZoneName": "...",
///      "curZoneRunStatus": "Status: Idle",
///      "notificationMessage": null,
///      "nextEvent": "..."
///      "water": "dry",
///      "hardwareDesc": "8-Zone (Gen 3)",
///      "nextRunStr": "Oct 26, 2023 - 6:40:07 AM",
///      "nextEventWeatherIntelligence": null,
///      "lastRun": "1697488267000",
///      "valve": "close",
///      "watering": "off",
///      "curZoneNumber": "3",
///      "lastUpdatedDt": "Oct 21, 2023 - 1:20:16 PM",
///      "rainDelay": "0",
///      "nextRun": "1698352807000",
///      "curZoneStartDate": "Not Active",
///      "rainDelayStr": "No Rain Delay",
///      "curZoneDuration": "1673",
///      "monthlyWeatherIntelligenceCount": "0",
///      "curZoneIsCycling": "False",
///      "DeviceWatch-DeviceStatus": "online",
///      "scheduleType": "Off",
///      "switch": "off",
///      "monthlyMinutesSaved": "0.0",
///      "standbyMode": "off",
///      "curZoneCycleCount": "0",
///      "nextEventThresholdInfo": null,
///      "nextEventType": "FUTURE_FEED_SCHEDULE_EVENT",
///      "monthlyMinutesUsed": "56.0",
///      "dashboard": "...",
///      "curZoneWaterTime": "10",
///      "hardwareModel": "8ZoneV3",
///      "lastRunStr": "Oct 16, 2023 - 6:31:07 AM"
///    },
///    "commands": [
///      {
///        "command": "close"
///      },
///      {
///    "command": "decZoneWaterTime"
///      },
///      {
///    "command": "decreaseRainDelay"
///      },
///      {
///    "command": "doSetRainDelay"
///      },
///      {
///    "command": "incZoneWaterTime"
///      },
///      {
///    "command": "increaseRainDelay"
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
///    "command": "pauseZoneRun"
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
///    "command": "resumeZoneRun"
///      },
///      {
///    "command": "runAllZones"
///      },
///      {
///    "command": "setDashboardColorScheme"
///      },
///      {
///    "command": "setDashboardIconScheme"
///      },
///      {
///    "command": "setRainDelay"
///      },
///      {
///    "command": "setZoneWaterTime"
///      },
///      {
///    "command": "standbyOff"
///      },
///      {
///    "command": "standbyOn"
///      },
///      {
///    "command": "stopWatering"
///      }
///    ]
///  },
/// </summary>
public class RachioControllerDevice : GenericDevice
{
    public RachioControllerDevice() { }

    public override double ExtractMetric(string attributeName, AttributeValue attributeValue)
    {
        // If the value is not a string, allow the generic parser to extract the metric.
        if (!attributeValue.IsT0) return base.ExtractMetric(attributeName, attributeValue);
        var attributeStringValue = attributeValue.AsT0;
        double value;
        // TODO Rachio has a lot of date-type fields
        // figure out their formats and add them as timestamps
        switch (attributeName)
        {
            case "curzonerunstatus":
                if (TryGetCurZoneRunStatus(attributeStringValue, out value)) return value;
                break;
            case "water":
                if (TryGetWater(attributeStringValue, out value)) return value;
                break;
            case "valve":
                if (TryGetValve(attributeStringValue, out value)) return value;
                break;
            case "devicewatch-devicestatus":
                if (TryGetDeviceWatchDeviceStatus(attributeStringValue, out value)) return value;
                break;
            case "nexteventtype":
                if (TryGetNextEventType(attributeStringValue, out value)) return value;
                break;
            default:
                System.Diagnostics.Debug.WriteLine($"Unknown attribute \"{attributeName}\" from Flume device.");
                break;
        }

        // On the off chance the string value is numeric, allow the generic _string_ parser to extract the metric.
        return base.ExtractNumericMetric(attributeName, attributeStringValue);
    }

    private static bool TryGetCurZoneRunStatus(string zoneRunStatus, out double value)
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

    private static bool TryGetWater(string water, out double value)
    {
        switch (water)
        {
            case "dry": value = 0; return true;
            case "wet": value = 1; return true;
        }

        value = -1;
        return false;
    }

    private static bool TryGetNextEventType(string water, out double value)
    {
        switch (water)
        {
            case "FUTURE_FEED_SCHEDULE_EVENT": value = 0; return true;
        }

        value = -1;
        return false;
    }
}