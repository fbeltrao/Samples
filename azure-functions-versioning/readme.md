# API Versioning with Azure Functions

This sample demonstrate a way to deploy an API with multiple versions using a single Azure Function App.

## API Design

The desired endpoints for the API are:

### Version 1
- Get all devices: GET http://localhost:7071/api/v1/devices
- Add new device: POST http://localhost:7071/api/v1/devices
- Get single device: GET http://localhost:7071/api/v1/devices/{deviceId}
- Update device: PUT http://localhost:7071/api/v1/devices/{deviceId}

### Version 2
- Get all devices: GET http://localhost:7071/api/v2/devices
- Add new device: POST http://localhost:7071/api/v2/devices
- Get single device: GET http://localhost:7071/api/v2/devices/{deviceId}
- Update device: PUT http://localhost:7071/api/v2/devices/{deviceId}


## Single Function App

In order to define fix routes for each HTTP triggered function we rely on the Route property of the HttpTrigger attribute.

### The code

```C#
namespace ApiFunction.v1
{
    public static class V1DevicesApi
    {
        [FunctionName(nameof(V1List))]
        public static IEnumerable<DeviceModel> V1List(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/devices")]
            HttpRequest req, 
            ILogger log)
        {
        }

		...
    }
}

namespace ApiFunction.v2
{
    public static class V2DevicesApi
    {
        [FunctionName(nameof(V2List))]
        public static IEnumerable<DeviceModel> V2List(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "v2/devices")]
            HttpRequest req, 
            ILogger log)
        {
        }

		...
    }
}

```


### Pros

This approach relies in deliverying all versions of an API as a single Function API which has the pros:

- Code simplicity when dealing with a reduced amount of versions, that don't introduce breaking changes.
- Deployment is simple since all we need is a Function App.

### Cons

- Deploying a new version updates all existing versions. Fixing a bug in a v2 can introduce a bug in v1.
- When deployed in a consumption plan auto scaling will deploy all versions as a single block. The bigger the Azure Function binaries are the longer the cold start takes.


## Alternative: a Function App for each version

An alternative is to contain each API version on its own repository, deployed into its own Function App. Joining all Function Apps into a single URL requires routing. The routing in Azure can be implemented in many ways, such as:
- [Azure Function Proxies](https://docs.microsoft.com/en-us/azure/azure-functions/functions-proxies)
- [Application Gateway](https://github.com/fbeltrao/azdeploy/tree/master/application-gateway)