using Microsoft.Azure.Devices.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace IoTDeviceClientActor
{
    public class UpstreamDeviceAction : DeviceAction
    {
        private readonly string body;
        public string Body => body;

        public UpstreamDeviceAction(string body)
        {
            this.body = body;
        }
 
    }
}
