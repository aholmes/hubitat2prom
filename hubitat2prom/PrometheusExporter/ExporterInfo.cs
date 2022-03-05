namespace hubitat2prom.PrometheusExporter;

/// <summary>
/// The Prometheus exporter info.
/// </summary>
public class ExporterInfo
{
    public ExporterInfoStatus status { get; set; }
    public HubitatEnv config { get; set; }
}
