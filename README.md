# hubitat2prom
Hubitat Prometheus exporter

C# port of the Python [hubitat2prom](https://github.com/BudgetSmartHome/hubitat2prom).

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
    aholmes0/hubitat2prom:v1.0.1\
    -e "HE_URI=your-uri"\
    -e "HE_TOKEN=your-token"
```

## Build the Docker Image

`docker build -t hubitat2prom .`

## Run the Docker Image

Use a command similar to the one used for [Using Docker](#using-docker), but use the local Docker image name:

```bash
docker run\
    -p 8080:80\
    hubitat2prom\
    -e "HE_URI=your-uri"\
    -e "HE_TOKEN=your-token"
```