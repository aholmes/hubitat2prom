namespace hubitat2prom.HubitatDevice;

/// <summary>
/// The list of commands for a device when querying for metrics of all devices.
/// This comes from querying for metrics at a URL like `/apps/api/712/devices/all`
/// <example>
/// {
///     "command": "on"
/// }
/// </example>
/// </summary>
public class DeviceSummaryCommands
{
    public string command { get; set; }
}