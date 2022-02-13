using System;

namespace hubitat2prom;
public class HubitatEnv
{
    public readonly Uri HE_URI;
    public readonly Guid HE_TOKEN;
    public readonly string HE_METRICS;

    public HubitatEnv()
    {
        Uri.TryCreate(Environment.GetEnvironmentVariable("HE_URI"), UriKind.Absolute, out HE_URI);
        Guid.TryParseExact(Environment.GetEnvironmentVariable("HE_TOKEN"), "D", out HE_TOKEN);

        string errors = "";
        if (HE_URI == default) errors += "\n`HE_URI` is empty";
        if (HE_TOKEN == default) errors += "\n`HE_TOKEN` is empty";
        if (errors != "") throw new Exception($"Both `HE_URI` and `HE_TOKEN` must be set in the application's environment variables.{errors}");

        HE_METRICS = Environment.GetEnvironmentVariable("HE_METRICS")
            ?? String.Join(',', new[] {
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
            });
    }
}