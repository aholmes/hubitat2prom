using System;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf;

namespace hubitat2prom.HubitatModels;

public class HubitatDeviceAttributes
{
    public string name { get; set; }
    public OneOf<int, string>? currentValue { get; set; }
    public string dataType { get; set; }
    public string[] values { get; set; }
}

public class HubitatDeviceCapabilityAttributes
{
    public string name { get; set; }
    public string dataType { get; set; }
}

public class HubitatDeviceCapabilities
{
    public HubitatDeviceCapabilityAttributes[] attributes { get; set; }
}

public class HubitatDeviceDetails
{
    public string id { get; set; }
    public string name { get; set; }
    public string label { get; set; }
    public string type { get; set; }
    public HubitatDeviceAttributes[] attributes { get; set; }
    /**
    * array of strings or objects, e.g.
    [
      "Configuration",
      "Actuator",
      "Refresh",
      "HoldableButton",
      {
         "attributes":[
            {
               "name":"held",
               "dataType":null
            }
         ]
      },
      "Switch",
      {
         "attributes":[
            {
               "name":"switch",
               "dataType":null
            }
         ]
      }
    ]
    */
    //public dynamic capabilities { get; set; }
    public OneOf<string, HubitatDeviceCapabilities>?[] capabilities { get; set; }
    public string[] commands { get; set; }
}