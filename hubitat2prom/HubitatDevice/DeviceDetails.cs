using OneOf;

namespace hubitat2prom.HubitatDevice;

public class DeviceDetails
{
    public string id { get; set; }
    public string name { get; set; }
    public string label { get; set; }
    public string type { get; set; }
    public DeviceAttributes[] attributes { get; set; }
    public OneOf<string, DeviceCapabilities>?[] capabilities { get; set; }
    public string[] commands { get; set; }
}