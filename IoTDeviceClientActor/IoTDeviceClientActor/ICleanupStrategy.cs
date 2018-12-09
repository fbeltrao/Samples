using System;
using System.Collections.Generic;
using System.Text;

namespace IoTDeviceClientActor
{
    public enum DeviceCleanupDecision
    {
        // Makes no changes
        NoChanges,

        // Keep instance alive, but disconnect client
        DisconnectClient,

        // Disconnect and Remove from cache
        DisconnectAndRemove,

    }

    public interface IDeviceCleanupStrategy
    {
        DeviceCleanupDecision ShouldCleanup(IoTDeviceActor value);
    }
}
