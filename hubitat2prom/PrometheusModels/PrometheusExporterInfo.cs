namespace hubitat2prom.PrometheusModels;
public class PrometheusExporterStatus
{
    public string CONNECTION { get; set; }
}
public class PrometheusExporterConfig
{
    public string HE_URI { get; set; }
    public string HE_TOKEN { get; set; }
    public string[] HE_METRICS { get; set; }
}
public class PrometheusExporterInfo
{
    public PrometheusExporterStatus status { get; set; }
    public PrometheusExporterConfig config { get; set; }
}