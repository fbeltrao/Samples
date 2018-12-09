using Microsoft.Azure.Devices.Shared;
using System.Threading.Tasks;

namespace IoTDeviceClientActor
{
    public class UpdateDesiredPropertiesDeviceAction : DeviceAction
    {
        private readonly TwinCollection reportedProperties;
        public TwinCollection ReportedProperties => reportedProperties;

        public UpdateDesiredPropertiesDeviceAction(TwinCollection reportedProperties)
        {
            this.reportedProperties = reportedProperties;
        }
    }
}
