namespace hubitat2prom.HubitatDevice;

/// <summary>
/// An attribute for a single device capability object.
/// This comes from querying a single device at a URL like `/apps/api/712/devices/130`
/// <example>
/// {
///     "name": "switch",
///     "dataType": null
/// }
/// </example>
/// </summary>
public class DeviceCapabilityAttributes
{
    public string name { get; set; }
    public string dataType { get; set; }
}
