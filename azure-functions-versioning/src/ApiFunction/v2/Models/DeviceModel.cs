using System;
using System.Collections.Generic;
using System.Text;

namespace ApiFunction.v2.Models
{
    public class DeviceModel
    {
        public string DeviceId { get; set; }

        public string Location { get; set; }

        public DateTime CreationDate { get; set; }

        /// <summary>
        /// This property was added in v2
        /// </summary>
        public string Department { get; set; }

        public string UniqueId => string.Concat(Department, "\\", Location, "\\", DeviceId);
    }
}
