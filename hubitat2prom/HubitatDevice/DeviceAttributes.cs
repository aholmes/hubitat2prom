using OneOf;

namespace hubitat2prom.HubitatDevice;

/// <summary>
/// An attribute for a single device metric.
/// This comes from querying a single device at a URL like `/apps/api/712/devices/130`
/// <example>
/// {
///     "name": "switch",
///     "currentValue": "off",
///     "dataType": "ENUM",
///     "values": [
///         "on",
///         "off"
///     ]
/// }
/// </example>
/// </summary>
public class DeviceAttributes
{
    public string name { get; set; }
    public OneOf<double, string>? currentValue { get; set; }
    public string dataType { get; set; }
    public string[] values { get; set; }
}
