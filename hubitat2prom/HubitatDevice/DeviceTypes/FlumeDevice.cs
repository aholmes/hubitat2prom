using AttributeValue = OneOf.OneOf<string, string[], int?, double?, OneOf.OneOf<double, string>?>;

namespace hubitat2prom.PrometheusExporter.DeviceTypes;


public class FlumeDevice: GenericDevice
{
    public FlumeDevice() { }

    public override double ExtractMetric(string attributeName, AttributeValue attributeValue)
    {
        // We can return if the value type is not T0 (string)
        // because that is the only type this method extracts
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

        return MISSING_VALUE_DEFAULT;
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