using OneOf;

namespace hubitat2prom.PrometheusModels;
public class PrometheusExporterInfo
{
    public PrometheusExporterStatus status { get; set; }
    public PrometheusExporterConfig config { get; set; }
}
