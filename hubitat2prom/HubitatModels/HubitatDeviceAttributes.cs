using OneOf;

namespace hubitat2prom.HubitatModels;

public class HubitatDeviceAttributes
{
    public string name { get; set; }
    public OneOf<double, string>? currentValue { get; set; }
    public string dataType { get; set; }
    public string[] values { get; set; }
}
