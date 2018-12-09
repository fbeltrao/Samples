using Microsoft.Azure.Devices.Client;
using System;

namespace IoTDeviceClientActor
{
    public class GetCloudToDeviceMessageAction : DeviceAction
    {
        public TimeSpan Timeout { get; set; }
        public Message Result { get; internal set; }
    }
}
