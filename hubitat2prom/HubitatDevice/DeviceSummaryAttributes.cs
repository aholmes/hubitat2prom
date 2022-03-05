using System.Collections;
using System.Reflection;
using OneOf;
using System.Linq;
using System.Collections.Generic;

using OneOfDoubleString = OneOf.OneOf<double, string>;
using AttributeValue = OneOf.OneOf<string, string[], int?, double?, OneOf.OneOf<double, string>?>;

namespace hubitat2prom.HubitatDevice;

public class DeviceSummaryAttributes
{
    public IEnumerator<KeyValuePair<string, AttributeValue?>> GetEnumerator()
    {
        return this.GetType().GetProperties(
              BindingFlags.Public
            | BindingFlags.Instance
            | BindingFlags.DeclaredOnly
        )
        .Select(propertyInfo =>
        {
            AttributeValue? attributeValue = null;
            var value = propertyInfo.GetValue(this);
            if (value is string @string) attributeValue = AttributeValue.FromT0(@string);
            if (value is string[] stringValue) attributeValue = AttributeValue.FromT1(stringValue);
            if (value is int @int) attributeValue = AttributeValue.FromT2(@int);
            if (value is double @double) attributeValue = AttributeValue.FromT3(@double);
            if (value is OneOfDoubleString oneOfDoubleString) attributeValue = AttributeValue.FromT4(oneOfDoubleString);

            return new KeyValuePair<string, AttributeValue?>(
                propertyInfo.Name,
                attributeValue
            );
        })
        .GetEnumerator();
    }

    public string dataType { get; set; }
    public string[] values { get; set; }
    public string syncStatus { get; set; }
    public string powerSource { get; set; }
    // is there an assocDNI1?
    public string assocDNI2 { get; set; }
    public string assocDNI3 { get; set; }
    #region Group Dimmer attributes
    public string groupState { get; set; }
    #endregion

    #region Switch attributes
    //[JsonPropertyName("switch")]
    public string @switch { get; set; }
    public int? held { get; set; }
    public int? numberOfButtons { get; set; }
    public int? pushed { get; set; }
    public int? released { get; set; }
    public string indicatorStatus { get; set; }
    #endregion

    #region Dimmer attributes
    public int? level { get; set; }
    #endregion

    #region Virtual Button attributes
    public int? doubleTapped { get; set; }
    #endregion

    #region Power attributes
    public double? current { get; set; }

    public double? currentH { get; set; }
    public double? currentL { get; set; }
    public double? energy { get; set; }
    public string energyDuration { get; set; }
    public double? frequency { get; set; }
    public OneOf<double, string>? power { get; set; }
    public double? powerH { get; set; }
    public double? powerL { get; set; }
    public double? voltage { get; set; }
    public double? voltageH { get; set; }
    public double? voltageL { get; set; }
    #endregion

    #region Roku TV attributes
    public string channel { get; set; }
    public string mute { get; set; }
    public string volume { get; set; }
    public string mediaInputSource { get; set; }
    public string sound { get; set; }
    public string transportStatus { get; set; }
    public string supportedInputs { get; set; }
    public string application { get; set; }
    public string picture { get; set; }
    public string movieMode { get; set; }
    #endregion

    #region Rachio attributes
    #region Rachio Controller attributes
    public int? curZoneId { get; set; }
    public string scheduleType { get; set; }
    public int? curZoneNumber { get; set; }
    public int? curZoneWaterTime { get; set; }
    public int? duration { get; set; }
    public string hardwareDesc { get; set; }
    public int? lastWateredDuration { get; set; }
    public string curZoneStartDate { get; set; }
    public int? curZoneDuration { get; set; }
    public string rainDelayStr { get; set; }
    public string lastWateredDt { get; set; }
    public string standbyMode { get; set; }
    public string valve { get; set; }
    public string lastUpdatedDt { get; set; }
    public int? activeZoneCnt { get; set; }
    public string totalCycleCount { get; set; }
    public string hardwareModel { get; set; }
    public int? rainDelay { get; set; }
    public string curZoneName { get; set; }
    public string watering { get; set; }
    public int? curZoneCycleCount { get; set; }
    public string startDate { get; set; }
    public string curZoneRunStatus { get; set; }
    public string controllerOn { get; set; }
    public string lastWateredDesc { get; set; }
    public string controllerRunStatus { get; set; }
    public string durationNoCycle { get; set; }
    public string curZoneIsCycling { get; set; }
    #endregion

    #region Rachio Zone attributes
    public string zoneDuration { get; set; }
    public string shadeName { get; set; }
    public string inStandby { get; set; }
    public string soilCategory { get; set; }
    public double? efficiency { get; set; }
    public string zoneStartDate { get; set; }
    public int? zoneTotalDuration { get; set; }
    public int? rootZoneDepth { get; set; }
    public double? saturatedDepthOfWater { get; set; }
    public string selectedZone { get; set; }
    public int? zoneWaterTime { get; set; }
    public double? availableWater { get; set; }
    public string nozzleCategory { get; set; }
    public string nozzleName { get; set; }
    public double? depthOfWater { get; set; }
    public string zoneRunElapsed { get; set; }
    public int? zoneNumber { get; set; }
    public int? cycleCount { get; set; }
    public string scheduleTypeBtnDesc { get; set; }
    public int? maxRuntime { get; set; }
    public int? zoneId { get; set; }
    public string zoneName { get; set; }
    public string slopeName { get; set; }
    public string soilName { get; set; }
    public int? zoneSquareFeet { get; set; }
    public string cropName { get; set; }
    #endregion
    #endregion

    #region Lutron Telnet attributes
    public string networkStatus { get; set; }
    #endregion

    #region Thermostat attributes
    public int? battery { get; set; }
    public string supportedThermostatModes { get; set; }
    public string thermostatFanMode { get; set; }
    public string thermostatOperatingState { get; set; }
    public double? coolingSetpoint { get; set; }
    public double? thermostatSetpoint { get; set; }
    public int? humidity { get; set; }
    public double? temperature { get; set; }
    public string supportedThermostatFanModes { get; set; }
    public double? heatingSetpoint { get; set; }
    public string thermostatMode { get; set; }
    #endregion
}