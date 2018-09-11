using System;
using System.Collections.Generic;
using System.Text;

namespace ApiFunction.Data
{
    public class Device
    {
        public string DeviceId { get; set; }

        public string Location { get; set; }

        public DateTime CreationDate { get; set; }

        /// <summary>
        /// This property was added in v2
        /// </summary>
        public string Department { get; set; }
    }
}
