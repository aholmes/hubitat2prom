namespace hubitat2prom.PrometheusModels;
public class InfoStatusModel
{
    public string CONNECTION { get; set; }
}
public class InfoConfigModel
{
    public string HE_URI { get; set; }
    public string HE_TOKEN { get; set; }
    public string[] HE_METRICS { get; set; }
}
public class InfoModel
{
    public InfoStatusModel status { get; set; }
    public InfoConfigModel config { get; set; }
}