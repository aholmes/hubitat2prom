using OneOf;

namespace hubitat2prom.HubitatDevice;

public class DeviceAttributes
{
    public string name { get; set; }
    public OneOf<double, string>? currentValue { get; set; }
    public string dataType { get; set; }
    public string[] values { get; set; }
}
