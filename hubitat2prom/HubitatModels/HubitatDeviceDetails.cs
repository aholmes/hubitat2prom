using OneOf;

namespace hubitat2prom.HubitatModels;

public class HubitatDeviceDetails
{
    public string id { get; set; }
    public string name { get; set; }
    public string label { get; set; }
    public string type { get; set; }
    public HubitatDeviceAttributes[] attributes { get; set; }
    public OneOf<string, HubitatDeviceCapabilities>?[] capabilities { get; set; }
    public string[] commands { get; set; }
}