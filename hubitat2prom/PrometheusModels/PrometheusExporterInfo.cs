using System;

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