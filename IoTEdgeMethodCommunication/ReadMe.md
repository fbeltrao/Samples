# Calling direct method from modules

Example here is outdated https://github.com/Azure/iot-edge-v1/tree/master/v2/samples/direct_method_invocation_sample

Need to use ModuleClient:

```c#
var libraryModule = "LibraryModule";
var deviceId = System.Environment.GetEnvironmentVariable("IOTEDGE_DEVICEID");
var payloadData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { payload= "ABCDEF0987654321", fport=1 }));
var methodRequest = new MethodRequest("test", payloadData);
var response = await client.InvokeMethodAsync(deviceId, libraryModule, methodRequest);
var responseText = Encoding.UTF8.GetString(response.Result);
```

## IoT edge solution used

Agent/Hub 1.0.4
Modules: TempSimulator, LibraryModule C#, SenderModule C#

Routes:
 "routes": {
          "SenderModuleToIoTHub": "FROM /messages/modules/SenderModule/outputs/* INTO $upstream",
          "sensorToSenderModule": "FROM /messages/modules/tempSensor/outputs/temperatureOutput INTO BrokeredEndpoint(\"/modules/SenderModule/inputs/input1\")"
 }

### Linux VM

First call 1500 ms\
Most calls 30-59ms\
Outliners: 60-130ms (~10%)

### Raspberry PI

First call 872 ms\
Most calls 180-375ms
