using System;
using OneOf;

namespace hubitat2prom.PrometheusModels;
public class PrometheusExporterStatus
{
    public string CONNECTION { get; set; }
}
public class PrometheusExporterConfig
{
    public Uri HE_URI { get; set; }
    public Guid HE_TOKEN { get; set; }
    public string[] HE_METRICS { get; set; }
}
public class PrometheusExporterInfo
{
    public PrometheusExporterStatus status { get; set; }
    public PrometheusExporterConfig config { get; set; }
}

public class PrometheusHubitatDeviceDetail
{
    public string DeviceName { get; set; }
    public string MetricName { get; set; }
    public double MetricValue { get; set; }
    public DateTime MetricTimestamp { get; set; }
    
    public override string ToString() => $"{MetricName}{{device_name=\"{DeviceName}\"}} {MetricValue.ToString("0.0##")}".ToLowerInvariant();
}