using Microsoft.Azure.Devices.Shared;
using System;
using System.Threading.Tasks;

namespace IoTDeviceClientActor
{
    public class UpdateDesiredPropertiesDeviceMessage : DeviceMessage
    {
        private readonly TwinCollection reportedProperties;

        public UpdateDesiredPropertiesDeviceMessage(TwinCollection reportedProperties)
        {
            this.reportedProperties = reportedProperties;
        }

        protected override async Task Execute(IoTDeviceActor device)
        {
            await device.deviceClient.UpdateReportedPropertiesAsync(this.reportedProperties);

            Console.WriteLine($"[{device.Id}] Updated reported properties");

        }
    }
}
