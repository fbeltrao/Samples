using System;
using System.Collections.Generic;
using System.Text;

namespace ApiFunction.v1.Models
{
    public class DeviceModel
    {
        public string DeviceId { get; set; }

        public string Location { get; set; }

        public DateTime CreationDate { get; set; }

        public string UniqueId => string.Concat(Location, "\\", DeviceId);
    }
}
