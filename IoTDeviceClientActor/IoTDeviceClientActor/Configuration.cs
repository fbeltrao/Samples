using System;
using System.Collections.Generic;
using System.Text;

namespace IoTDeviceClientActor
{
    public class Configuration
    {
        public string Device1ConnectionString { get; set; }
        public string Device2ConnectionString { get; set; }
        public string Device3ConnectionString { get; set; }
        public double RemoveDeviceTimeoutInSeconds { get; set; }
        public double DisconnectDeviceTimeoutInSeconds { get; set; }
    }
}
