using System;

namespace hubitat2prom.HubitatModels;

public class HubitatDeviceDetailSummary : HubitatDeviceSummary
{
    // FIXME this is some kind of datetime
    public string date { get; set; }
    // TODO what is model?
    public object model { get; set; }
    // TODO what is manufacturer
    public object manufacturer { get; set; }
    public string[] capabilities { get; set; }
    public HubitatDeviceSummaryAttributes attributes { get; set; }
    public HubitatDeviceSummaryCommands[] commands { get; set; }
}