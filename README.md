# hubitat2prom
Hubitat Prometheus exporter

C# alternative to the Python [hubitat2prom](https://github.com/BudgetSmartHome/hubitat2prom).

This application supports a wider set of devices and device attributes, and provides type safety gaurantees.

# Prerequisites

## Install Maker API

The Maker API app must be installed on your Hubitat. Follow the [Hubitat documentation](https://docs.hubitat.com/index.php?title=Maker_API).

<a name="gather-maker-api-information" id="gather-maker-api-information"></a>
## Gather Maker API Information

`hubitat2prom` needs to know where to get Hubitat device information, and how to authenticate with your Hubitat. The values for these are stored in two environment variables: HE_URI, and HE_TOKEN.

On the Maker API app page:

1. `HE_URI`: look for **Get All Devices** under **Local URLS**. Copy the URL, and remove `?access_token=...`. Mine looks like `http://192.168.50.22/apps/api/712/devices`.
2. `HE_TOKEN`: Copy the GUID that is `access_token` from the previous step.

# Run hubitat2prom

Set the `HE_URI` and `HE_TOKEN` environment variables. Using Bash, this command will do the trick. Replace the values with the ones from [Gather Maker API Information](#gather-maker-api-information):

`HE_URI=your-url HE_TOKEN=your-token dotnet run`

## Optional Configuration

### Configurable Metrics
`hubitat2prom` supports the environment variable `HE_METRICS`. This is a comma-separated list of Hubitat device metrics that should be collected by `hubitat2prom`. If not provided, `hubitat2prom` will use a hard-coded [list of defaults](hubitat2prom/HubitatEnv.cs#L35-L50).

`HE_METRICS` is used like any other environment variable, and follows the same patterns as both `HE_URI` and `HE_TOKEN`. Use the variable like this: `HE_METRICS=power,switch,humidity,battery`.

# Test hubitat2prom
`dotnet test`

<a name="using-docker" id="using-docker"></a>
# Using Docker

The Docker image can be pulled directly from Dockerhub.

You will need to provide the same environment variables, but the syntax is different with Docker.

```bash
docker run\
    -p 8080:80\
    -e "HE_URI=your-uri"\
    -e "HE_TOKEN=your-token"\
    aholmes0/hubitat2prom:latest
```

## Build the Docker Image

`docker build -t hubitat2prom .`

## Run the Docker Image

Use a command similar to the one used for [Using Docker](#using-docker), but use the local Docker image name:

```bash
docker run\
    -p 8080:80\
    -e "HE_URI=your-uri"\
    -e "HE_TOKEN=your-token"\
    hubitat2prom
```

# Internals

## Mapped values

Because Prometheus scrapes only scalar values, some enum/string "attribute" values coming from Hubitat devices are mapped to corresponding scalar values. The values chosen represent the same order that the Maker API reports them. In cases where an unknown attribute name is parsed, the value `0` is returned.


If you would like to chart these values in an order other than the default, a PromQL query similar to the following will map one scalar value to another.

```
((thermostatoperatingstate == 0) + 4) # map heating to 4
or ((thermostatoperatingstate == 1) + 0) # map pending cool to 1
or ((thermostatoperatingstate == 2) + 1) # map pending heat to 3
or ((thermostatoperatingstate == 3) + 3) # map vent economizer to 6
or ((thermostatoperatingstate == 4) - 4) # map idle to 0
or ((thermostatoperatingstate == 5) - 3) # map cooling to 2
or ((thermostatoperatingstate == 6) - 1) # map fan only to 5
```

### Power and Switch

The [`power`](https://github.com/aholmes/hubitat2prom/blob/6aab4b3b01621fa7c8d5e906883b1a0eb0bce733/hubitat2prom/PrometheusExporter/HubitatDeviceMetrics.cs#L134-L150) and [`switch`](https://github.com/aholmes/hubitat2prom/blob/6aab4b3b01621fa7c8d5e906883b1a0eb0bce733/hubitat2prom/PrometheusExporter/HubitatDeviceMetrics.cs#L106) attributes.

```
"off" = 0
"on" = 1
```

### Thermostat mode

The [`thermostatMode`](https://github.com/aholmes/hubitat2prom/blob/6aab4b3b01621fa7c8d5e906883b1a0eb0bce733/hubitat2prom/PrometheusExporter/HubitatDeviceMetrics.cs#L169-L182) attribute.

```
"auto" = 0
"off" = 1
"heat" = 2
"emergency heat" = 3
"cool" = 4
```

### Thermostat operating state

The [`thermostatOperatingState`](https://github.com/aholmes/hubitat2prom/blob/6aab4b3b01621fa7c8d5e906883b1a0eb0bce733/hubitat2prom/PrometheusExporter/HubitatDeviceMetrics.cs#L152-L167) attribute.

```
"heating" = 0
"pending cool" = 1
"pending heat" = 2
"vent economizer" = 3
"idle" = 4
"cooling" = 5
"fan only" = 6
```
