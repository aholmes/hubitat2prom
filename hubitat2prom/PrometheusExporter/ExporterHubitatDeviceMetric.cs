using System;

namespace hubitat2prom.PrometheusExporter;

/// <summary>
/// Contains the information Prometheus is able to digest.
/// Serializes to a Prometheus metric line.
/// </summary>
public class ExporterHubitatDeviceMetric
{
    /// <summary>
    /// The name of the device configured on a Hubitat,
    /// e.g., "bathroom_switch."
    /// </summary>
    public string DeviceName { get; set; }

    /// <summary>
    /// The type of metric for the device,
    /// e.g., "switch" for whether a switch is on or off,
    /// "level" for a dimmer level, etc.
    /// </summary>
    public string MetricName { get; set; }

    /// <summary>
    /// The value for the device metric.
    /// </summary>
    public double MetricValue { get; set; }

    /// <summary>
    /// When the metric value was measured.
    /// </summary>
    public DateTime MetricTimestamp { get; set; }

    // Show at least one, and up to three decimals
    private string _metricValue => (Math.Truncate(MetricValue * 1000) / 1000).ToString("0.0##");

    /// <summary>
    /// Formats the line Prometheus needs to ingest a metric.
    /// </summary>
    /// <returns>
    /// This looks like, e.g., `switch{device_name="light_switch"} 1.0`
    /// </returns>

    public override string ToString() => $"hubitat_{MetricName}{{device_name=\"{DeviceName}\"}} {_metricValue}".ToLowerInvariant();
}
