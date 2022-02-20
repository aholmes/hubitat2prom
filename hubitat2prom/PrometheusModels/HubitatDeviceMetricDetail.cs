using System;

namespace hubitat2prom.PrometheusModels;

public class HubitatDeviceMetricDetail
{
    public string DeviceName { get; set; }
    public string MetricName { get; set; }
    public double MetricValue { get; set; }
    public DateTime MetricTimestamp { get; set; }
    private string _metricValue => (Math.Truncate(MetricValue * 1000) / 1000).ToString("0.0##");

    public override string ToString() => $"{MetricName}{{device_name=\"{DeviceName}\"}} {_metricValue}".ToLowerInvariant();
}