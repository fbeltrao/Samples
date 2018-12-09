using Microsoft.Azure.Devices.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace IoTDeviceClientActor
{
    public class UpstreamDeviceMessage : DeviceMessage
    {
        private readonly string body;

        public UpstreamDeviceMessage(string body)
        {
            this.body = body;
        }

        protected override async Task Execute(IoTDeviceActor device)
        {
            await device.deviceClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes(body)));

            Console.WriteLine($"[{device.Id}] Sent: {body}");
        }
    }
}
