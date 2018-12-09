using System;
using System.Collections.Generic;
using System.Text;

namespace IoTDeviceClientActor
{
    public class DefaultDeviceCleanupStrategy : IDeviceCleanupStrategy
    {
        public TimeSpan DisconnectTimeout { get; }
        public TimeSpan RemoveTimeout { get; }

        public DefaultDeviceCleanupStrategy(TimeSpan disconnectTimeout, TimeSpan removeTimeout)
        {
            DisconnectTimeout = disconnectTimeout;
            RemoveTimeout = removeTimeout;
        }

        public DeviceCleanupDecision ShouldCleanup(IoTDeviceActor device)
        {
            if (!device.HasWork())
            {
                var timeSpanSinceLastActity = DateTime.UtcNow.Subtract(device.LastActivityUtc);
                if (timeSpanSinceLastActity >= this.RemoveTimeout)
                    return DeviceCleanupDecision.DisconnectAndRemove;

                if (device.IsConnected() && timeSpanSinceLastActity >= this.DisconnectTimeout)
                    return DeviceCleanupDecision.DisconnectClient;

            }

            return DeviceCleanupDecision.NoChanges;
        }
    }
}
