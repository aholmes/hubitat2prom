namespace hubitat2prom.HubitatDevice;

/// <summary>
/// Detailed summary information when querying for metrics of all devices.
/// This comes from querying for metrics at a URL like `/apps/api/712/devices/all`
/// <example>
/// {
///     "name": "Kitchen Light",
///     "label": "Kitchen Light",
///     "type": "Zooz ZEN Switch Advanced",
///     "id": "130",
///     "date": "2022-03-05T14:18:00+0000",
///     "model": null,
///     "manufacturer": null,
///     "capabilities": [
///         "Configuration",
///         "ReleasableButton",
///         "Switch",
///     ],
///     "attributes": {
///         "dataType": "ENUM",
///         "values": [
///             "on",
///             "off"
///         ],
///         "numberOfButtons": "10",
///         "released": null,
///         "switch": "off"
///     },
///     "commands": [
///         {
///             "command": "configure"
///         },
///         {
///             "command": "off"
///         },
///         {
///             "command": "on"
///         },
///         {
///             "command": "release"
///         },
///     ]
/// }
/// </example>
/// </summary>
public class DeviceDetailSummary : DeviceSummary
{
    // FIXME consider making this a DateTime, not a string
    public string date { get; set; }
    // TODO what is model?
    public object model { get; set; }
    // TODO what is manufacturer
    public object manufacturer { get; set; }
    public string[] capabilities { get; set; }

    /// <summary>
    /// <example>
    /// "attributes": {
    ///     "dataType": "ENUM",
    ///     "values": [
    ///         "on",
    ///         "off"
    ///     ],
    ///     "numberOfButtons": "10",
    ///     "released": null,
    ///     "switch": "off"
    /// }
    /// </example>
    /// </summary>
    public DeviceSummaryAttributes attributes { get; set; }

    /// <summary>
    /// 
    /// <example>
    /// "commands": [
    ///     {
    ///         "command": "configure"
    ///     },
    ///     {
    ///         "command": "off"
    ///     },
    ///     {
    ///         "command": "on"
    ///     },
    ///     {
    ///         "command": "release"
    ///     }
    /// ]
    /// </example>
    /// </summary>
    public DeviceSummaryCommands[] commands { get; set; }
}