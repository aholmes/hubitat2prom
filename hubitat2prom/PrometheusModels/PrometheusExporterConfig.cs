using System;

namespace hubitat2prom.PrometheusModels;

public class PrometheusExporterConfig
{
    public Uri HE_URI { get; set; }
    public Guid HE_TOKEN { get; set; }
    public string[] HE_METRICS { get; set; }
}
