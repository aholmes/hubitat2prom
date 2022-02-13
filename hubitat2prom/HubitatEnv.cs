using System;

namespace hubitat2prom;
public class HubitatEnv
{
    public readonly string HE_URI;
    public readonly string HE_TOKEN;
    public readonly string HE_METRICS;

    public HubitatEnv()
    {
        HE_URI = Environment.GetEnvironmentVariable("HE_URI");
        HE_TOKEN = Environment.GetEnvironmentVariable("HE_TOKEN");

        string errors = "";
        if (HE_URI == null) errors += "\n`HE_URI` is null";
        if (HE_TOKEN == null) errors += "\n`HE_TOKEN` is null";
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