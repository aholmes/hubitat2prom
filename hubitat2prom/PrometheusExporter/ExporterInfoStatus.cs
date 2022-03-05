namespace hubitat2prom.PrometheusExporter;

/// <summary>
/// Whether the Hubitat hub is online. This is set to either `ONLINE` or `OFFLINE`.
/// </summary>
public class ExporterInfoStatus
{
    public string CONNECTION { get; set; }
}
