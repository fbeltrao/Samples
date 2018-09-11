# API Versioning with Azure Functions

This sample demonstrate a way to deploy an API with multiple versions using a single Azure Function App.

## API Design

The desired endpoints for the API are:

|Request|Http Method|URL|
|-|-|-|
|Get all devices|GET|/api/{version}/devices`|
|Add new device|POST|/api/{version}/devices`|
|Get single device|GET|/api/{version}/devices/{deviceId}`|
|Update device|PUT|`/api/{version}/devices/{deviceId}`|



## Single Function App

This approach relies in deploying all versions of an API as a single Function App. Custom routes (to include the version) are defined using the HttpTrigger Route property.

### Pros

- Code simplicity when dealing with a reduced amount of versions, that don't introduce breaking changes
- Deployment is simple since all we need is a Function App

### Cons

- Deploying a new version updates all existing versions. Fixing a bug in a v2 can introduce a bug in v1
- When deployed in a consumption plan auto scaling will deploy all versions as a single block. The bigger the Azure Function binaries are the longer the cold start takes


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



## Alternative: a Function App for each version

An alternative is to contain each API version on its own repository, deployed into its own Function App. Joining all Function Apps into a single URL requires routing. The routing in Azure can be implemented in many ways, such as:
- [Azure Function Proxies](https://docs.microsoft.com/en-us/azure/azure-functions/functions-proxies)
- [Application Gateway](https://github.com/fbeltrao/azdeploy/tree/master/application-gateway)


### Pros

- Version isolation, ensuring that changes to a single version won't affect others
- In a consumption plan, each version scales independently

### Cons

- Fixing a bug that affects multiple versions requires making changes to multiple repositories 
- Slightly more complicated deployment, requiring a router to control traffic based on the API version.