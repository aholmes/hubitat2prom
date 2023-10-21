using AttributeValue = OneOf.OneOf<string, string[], int?, double?, OneOf.OneOf<double, string>?>;

namespace hubitat2prom.PrometheusExporter.DeviceTypes;


/// <summary>
/// Flume Device. The JSON response is structured like:
///   {
///    "name": "Flume",
///    "label": "Flume",
///    "type": "Flume Device",
///    "id": "1319",
///    "date": "2023-10-21T20:09:39+0000",
///    "model": null,
///    "manufacturer": null,
///    "room": "Outside",
///    "capabilities": [
///      "WaterSensor",
///      "Configuration",
///      "Refresh",
///      "Initialize",
///      "PresenceSensor"
///    ],
///    "attributes": {
///      "usageLastDay": "111.02",
///      "dataType": "ENUM",
///      "values": [
///        "present",
///        "not present"
///      ],
///      "usageLastHour": "0.0",
///      "notificationStream": "[2023-09-30T01:55:00.000Z]: High Flow Alert triggered at 4243 Coolidge. Water has been running for 15 minutes averaging 9.06 gallons every minute.",
///      "water": "dry",
///      "usageLastMonth": "2594.13",
///      "usageLastWeek": "548.59",
///      "flowStatus": "stopped",
///      "commStatus": "good",
///      "flowDurationMin": "0",
///      "usageLastMinute": "0.0",
///      "presence": "present"
///    },
///    "commands": [
///      {
///        "command": "clearAwayMode"
///      },
///      {
///    "command": "clearWetStatus"
///      },
///      {
///    "command": "configure"
///      },
///      {
///    "command": "initialize"
///      },
///      {
///    "command": "refresh"
///      },
///      {
///    "command": "setAwayMode"
///      },
///      {
///    "command": "testWetStatus"
///      }
///    ]
///  },
/// </summary>
public class FlumeDevice: GenericDevice
{
    public FlumeDevice() { }

    public override double ExtractMetric(string attributeName, AttributeValue attributeValue)
    {
        // If the value is not a string, allow the generic parser to extract the metric.
        if (!attributeValue.IsT0) return base.ExtractMetric(attributeName, attributeValue);
        var attributeStringValue = attributeValue.AsT0;
        double value;
        switch (attributeName)
        {
            case "commstatus":
                if (TryGetCommStatus(attributeStringValue, out value)) return value;
                break;
            case "flowstatus":
                if (TryGetFlowStatus(attributeStringValue, out value)) return value;
                break;
            case "presence":
                if (TryGetPresence(attributeStringValue, out value)) return value;
                break;
            case "water":
                if (TryGetWater(attributeStringValue, out value)) return value;
                break;
            default:
                System.Diagnostics.Debug.WriteLine($"Unknown attribute \"{attributeName}\" from Flume device.");
                break;
        }

        // On the off chance the string value is numeric, allow the generic _string_ parser to extract the metric.
        return base.ExtractNumericMetric(attributeName, attributeStringValue);
    }
    
    private static bool TryGetCommStatus(string commStatus, out double value)
    {
        switch (commStatus)
        {
            case "good": value = 0; return true;
            case "unknown": value = 1; return true;
            case "error": value = 2; return true;
        }

        value = -1;
        return false;
    }
    
    private static bool TryGetFlowStatus(string flowStatus, out double value)
    {
        switch (flowStatus)
        {
            case "stopped": value = 0; return true;
            case "running": value = 1; return true;
            case "monitoring": value = 2; return true;
        }

        value = -1;
        return false;
    }

    private static bool TryGetPresence(string presence, out double value)
    {
        switch (presence)
        {
            case "not present": value = 0; return true;
            case "present": value = 1; return true;
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
}