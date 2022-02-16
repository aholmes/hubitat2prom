using System;

namespace hubitat2prom.PrometheusModels;

public class HubitatDeviceMetricDetail
{
    public string DeviceName { get; set; }
    public string MetricName { get; set; }
    public double MetricValue { get; set; }
    public DateTime MetricTimestamp { get; set; }
    
    public override string ToString() => $"{MetricName}{{device_name=\"{DeviceName}\"}} {MetricValue.ToString("0.0##")}".ToLowerInvariant();
}