using AttributeValue = OneOf.OneOf<string, string[], int?, double?, OneOf.OneOf<double, string>?>;
using hubitat2prom.HubitatDevice;
using Rachio = hubitat2prom.HubitatDevice.DeviceTypes.Rachio;

namespace hubitat2prom.PrometheusExporter.DeviceTypes;

public abstract class DeviceType
{
    protected const double MISSING_VALUE_DEFAULT = 0d;

    protected DeviceType() { }
    public abstract double ExtractMetric(string attributeName, AttributeValue attributeValue);
    
    public static DeviceType CreateDeviceType(DeviceSummary deviceSummary)
    {
        switch(deviceSummary.type)
        {
            case "Flume Device":
                return new FlumeDevice();
            case "Rachio Controller":
                return new Rachio.RachioControllerDevice();
            default:
                return new GenericDevice();
        }
    }
}
