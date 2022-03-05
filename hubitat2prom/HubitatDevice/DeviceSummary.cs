namespace hubitat2prom.HubitatDevice;

/// <summary>
/// A device from the list of devices on a Hubitat hub.
/// This comes from querying for devices at a URL like `/apps/api/712/devices`
/// <example>
/// {
///     "id": "130",
///     "name": "Kitchen Light",
///     "label": "Kitchen Light",
///     "type": "Zooz ZEN Switch Advanced"
/// }
/// </example>
/// </summary>
public class DeviceSummary
{
    public string id { get; set; }
    public string name { get; set; }
    public string label { get; set; }
    public string type { get; set; }
}