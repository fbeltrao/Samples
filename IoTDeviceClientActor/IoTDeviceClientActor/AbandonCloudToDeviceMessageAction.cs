using Microsoft.Azure.Devices.Client;

namespace IoTDeviceClientActor
{
    public class AbandonCloudToDeviceMessageAction : DeviceAction
    {
        public Message Message { get; set; }
    }
}
