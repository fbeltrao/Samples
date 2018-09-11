using System;
using System.Collections.Generic;
using System.Text;

namespace ApiFunction.Data
{
    public class DeviceRepository
    {
        static DeviceRepository singleton;

        static DeviceRepository()
        {
            singleton = new DeviceRepository();
        }

        public static DeviceRepository Get() => singleton;

        private readonly Dictionary<string, Device> devices;

        DeviceRepository()
        {
            devices = new Dictionary<string, Device>
            {
                { "0001", new Device { DeviceId = "0001", CreationDate = new DateTime(2000, 12, 1, 08, 0, 0, DateTimeKind.Utc), Department = "Sales", Location = "Zurich" } },
                { "0002", new Device { DeviceId = "0002", CreationDate = new DateTime(2005, 08, 1, 07, 0, 0, DateTimeKind.Utc), Department = "HR", Location = "London" } },
                { "0003", new Device { DeviceId = "0003", CreationDate = new DateTime(2010, 07, 1, 06, 0, 0, DateTimeKind.Utc), Department = "HR", Location = "Zurich" } },
                { "0004", new Device { DeviceId = "0004", CreationDate = new DateTime(2015, 11, 1, 08, 0, 0, DateTimeKind.Utc), Department = "IT", Location = "Amsterdam" } },
            };
        }

        public IReadOnlyCollection<Device> GetAll() => this.devices.Values;

        public Device Get(string id)
        {
            this.devices.TryGetValue(id, out var device);
            return device;
        }

        public void Create(Device device)
        {
            if (devices.ContainsKey(device.DeviceId))
                throw new DuplicateDeviceException();

            devices.Add(device.DeviceId, device);
        }

        public void Update(string deviceId, Device device)
        {
            if (!devices.TryGetValue(deviceId, out var existingDevice))
                throw new DeviceNotFound();

            // handle v1 missing property department
            device.Department = device.Department ?? existingDevice.Department;
            
            devices[device.DeviceId] = device;
        }
    }
}
