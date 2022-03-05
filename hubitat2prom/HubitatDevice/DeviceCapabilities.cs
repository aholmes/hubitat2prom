namespace hubitat2prom.HubitatDevice;

/// <summary>
/// Attributes listed for a single device capability object.
/// This comes from querying a single device at a URL like `/apps/api/712/devices/130`
/// <example>
/// "attributes": [
///     {
///         "name": "switch",
///         "dataType": null
///     }
/// ]
/// </example>
/// </summary>
public class DeviceCapabilities
{
    /// <summary>
    /// <example>
    /// "attributes": [
    ///     {
    ///         "name": "switch",
    ///         "dataType": null
    ///     }
    /// ]
    /// </example>
    /// </summary>
    public DeviceCapabilityAttributes[] attributes { get; set; }
}
