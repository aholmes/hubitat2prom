using OneOf;

namespace hubitat2prom.HubitatDevice;

/// <summary>
/// A device from a single device metric.
/// This comes from querying a single device at a URL like `/apps/api/712/devices/130`
/// <example>
/// {
///     "id": "130",
///     "name": "Kitchen Light",
///     "label": "Kitchen Light",
///     "type": "Zooz ZEN Switch Advanced",
///     "attributes": [
///         {
///             "name": "switch",
///             "currentValue": "off",
///             "dataType": "ENUM",
///             "values": [
///                 "on",
///                 "off"
///             ]
///         }
///     ],
///     "capabilities": [
///         "Configuration",
///         "Switch",
///             {
///                 "attributes": [
///                     {
///                         "name": "switch",
///                         "dataType": null
///                     }
///                 ]
///             }
///     ],
///     "commands": [
///         "configure",
///         "off",
///         "on",
///     ]
/// }
/// </example>
/// </summary>
public class DeviceDetails : DeviceSummary
{
    /// <summary>
    /// <example>
    /// "attributes": [
    ///     {
    ///         "name": "switch",
    ///         "currentValue": "off",
    ///         "dataType": "ENUM",
    ///         "values": [
    ///             "on",
    ///             "off"
    ///         ]
    ///     }
    /// ]
    /// </example>
    /// </summary>
    public DeviceAttributes[] attributes { get; set; }

    /// <summary>
    /// <example>
    /// "capabilities": [
    ///     "Configuration",
    ///     "Switch",
    ///         {
    ///             "attributes": [
    ///                 {
    ///                     "name": "switch",
    ///                     "dataType": null
    ///                 }
    ///             ]
    ///         }
    /// ]
    /// </example>
    /// </summary>
    public OneOf<string, DeviceCapabilities>?[] capabilities { get; set; }
    public string[] commands { get; set; }
}