using System;

namespace hubitat2prom;
public record HubitatEnv
{
    public Uri HE_URI { get; init; }
    public Guid HE_TOKEN { get; init; }
    public string[] HE_METRICS { get; init; }

    public static Lazy<HubitatEnv> _instance = new Lazy<HubitatEnv>();
    public static HubitatEnv Instance => _instance.Value;

    private HubitatEnv()
        : this(
            Environment.GetEnvironmentVariable("HE_URI"),
            Environment.GetEnvironmentVariable("HE_TOKEN"),
            Environment.GetEnvironmentVariable("HE_METRICS")?.Split(',')
        )
    { }

    public HubitatEnv(string he_uri, string he_token, string[] metrics = default)
        : this(
            _parseHeUri(he_uri),
            _parseHeToken(he_token),
            metrics
        )
    { }

    public HubitatEnv(Uri he_uri, Guid he_token, string[] metrics = default)
    {
        string errors = "";
        if (he_uri == default) errors += "\n`HE_URI` is empty";
        if (he_token == default) errors += "\n`HE_TOKEN` is empty";
        if (errors != "") throw new Exception($"Both `HE_URI` and `HE_TOKEN` must be set in the application's environment variables, or as non-empty parameters to HubitatEnv.{errors}");

        var he_metrics = metrics
            ?? new[] {
                    "battery",
                    "humidity",
                    "illuminance",
                    "level",
                    "switch",
                    "temperature",
                    "heatingSetpoint",
                    "thermostatSetpoint",
                    "thermostatFanMode",
                    "thermostatOperatingState",
                    "thermostatMode",
                    "coolingSetpoint",
                    "power",
                    "energy",
                    "current",
                    "voltage"
            };

        HE_URI = he_uri;
        HE_TOKEN = he_token;
        HE_METRICS = he_metrics;
    }

    private static Uri _parseHeUri(string he_uri)
    {
        Uri.TryCreate(he_uri, UriKind.Absolute, out Uri _he_uri);
        return _he_uri;
    }

    private static Guid _parseHeToken(string he_token)
    {
        Guid.TryParseExact(he_token, "D", out Guid _he_token);
        return _he_token;
    }
}