namespace hubitat2prom.PrometheusModels;
public class InfoStatusModel
{
    public string CONNECTION;
}
public class InfoConfigModel
{
    public string HE_URI;
    public string HE_TOKEN;
    public string[] HE_METRICS;
}
public class InfoModel
{
    public InfoStatusModel status;
    public InfoConfigModel config;
}