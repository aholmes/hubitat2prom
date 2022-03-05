namespace hubitat2prom.HubitatDevice;

public class DeviceDetailSummary : DeviceSummary
{
    // FIXME this is some kind of datetime
    public string date { get; set; }
    // TODO what is model?
    public object model { get; set; }
    // TODO what is manufacturer
    public object manufacturer { get; set; }
    public string[] capabilities { get; set; }
    public DeviceSummaryAttributes attributes { get; set; }
    public DeviceSummaryCommands[] commands { get; set; }
}