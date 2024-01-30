using System.Reflection;
using OneOf;
using System.Linq;
using System.Collections.Generic;

using OneOfDoubleString = OneOf.OneOf<double, string>;
using AttributeValue = OneOf.OneOf<string, string[], int?, double?, OneOf.OneOf<double, string>?, long?>;
using System.Dynamic;
using System;

namespace hubitat2prom.HubitatDevice;

/// <summary>
/// The values of attributes for a device when querying for metrics of all devices.
/// This comes from querying for metrics at a URL like `/apps/api/712/devices/all`
///
/// Instances of this class are iterable,
/// and will return the name and value of
/// each property in the class instance.
///
/// This class is a collection of currently known attributes and their
/// presumed types. These may be wrong on occassion, and there are additional
/// attributes that I have not added because I am not aware of them.
/// More properties can be added to collect those metrics.
/// 
/// <example>
/// {
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
[Serializable]
public class DeviceSummaryAttributes : DynamicObject
{
    /// <summary>
    /// Return the name and value of each property in this class instance.
    /// </summary>
    public IEnumerator<KeyValuePair<string, AttributeValue?>> GetEnumerator()
    {
        return properties.Select(propertyInfoEntry =>
        {
            var propertyName = propertyInfoEntry.Key;
            var propertyInfo = propertyInfoEntry.Value;
            AttributeValue? attributeValue = null;
            var value = (propertyInfo as PropertyInfo)?.GetValue(this) ?? propertyInfo;

            if (value is string @string) attributeValue = AttributeValue.FromT0(@string);
            if (value is string[] stringValue) attributeValue = AttributeValue.FromT1(stringValue);
            if (value is int @int) attributeValue = AttributeValue.FromT2(@int);
            if (value is double @double) attributeValue = AttributeValue.FromT3(@double);
            if (value is OneOfDoubleString oneOfDoubleString) attributeValue = AttributeValue.FromT4(oneOfDoubleString);
            if (value is long @long) attributeValue = AttributeValue.FromT5(@long);

            return new KeyValuePair<string, AttributeValue?>(
                propertyName,
                attributeValue
            );
        })
        .GetEnumerator();
    }

    private Lazy<Dictionary<string, PropertyInfo>> typedPropertiesLazy
        => new Lazy<Dictionary<string, PropertyInfo>>(
            () => this.GetType().GetProperties(
                  BindingFlags.Public
                | BindingFlags.Instance
                | BindingFlags.DeclaredOnly
            ).ToDictionary(propertyInfo => propertyInfo.Name)
        );
    private Dictionary<string, PropertyInfo> typedProperties => typedPropertiesLazy.Value;
    private Lazy<Dictionary<string, object>> typedPropertiesAsObjectDictionary
        => new Lazy<Dictionary<string, object>>(() => typedProperties.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value));


    private Dictionary<string, object> dynamicProperties = new Dictionary<string, object>();

    private IEnumerable<KeyValuePair<string, object>> properties
        => typedPropertiesAsObjectDictionary.Value.Concat(dynamicProperties);

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        if (typedProperties.ContainsKey(binder.Name))
        {
            result = typedProperties[binder.Name];
            return true;
        }
        else
        {
            return dynamicProperties.TryGetValue(binder.Name, out result);
        }
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        if (typedProperties.ContainsKey(binder.Name))
        {
            try
            {
                typedProperties[binder.Name].SetValue(this, value);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
        else
        {
            dynamicProperties[binder.Name] = value;
        }
        return true;
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
    {
        result = null;
        if (!dynamicProperties.ContainsKey(binder.Name) && !typedProperties.ContainsKey(binder.Name)) return false;

        var memberInfo = this.GetType().GetMember(binder.Name, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance);
        if (memberInfo == null || memberInfo.Length == 0) return false;

        var method = memberInfo[0] as MethodInfo;
        result = method.Invoke(this, args);
        return true;
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
    public string @switch { get; set; }
    public int? held { get; set; }
    public int? numberOfButtons { get; set; }
    public int? pushed { get; set; }
    public int? released { get; set; }
    public string indicatorStatus { get; set; }
    #endregion

    #region Dimmer attributes
    public double? level { get; set; }
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
    public double? battery { get; set; }
    public string supportedThermostatModes { get; set; }
    public string thermostatFanMode { get; set; }
    public string thermostatOperatingState { get; set; }
    public double? coolingSetpoint { get; set; }
    public double? thermostatSetpoint { get; set; }
    public double? humidity { get; set; }
    public double? temperature { get; set; }
    public string supportedThermostatFanModes { get; set; }
    public double? heatingSetpoint { get; set; }
    public string thermostatMode { get; set; }
    #endregion
}