using Microsoft.Azure.Devices.Client;

namespace IoTDeviceClientActor
{
    public class CompleteCloudToDeviceMessageAction : DeviceAction
    {
        public Message Message { get; internal set; }
    }
}
