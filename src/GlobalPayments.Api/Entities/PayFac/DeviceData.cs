using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.PayFac
{
    public class DeviceData {
        public List<DeviceInfo> Devices { get; set; }
        public DeviceData() { new List<DeviceInfo>(); }
    }
    public class DeviceInfo {
        /// <summary>
        /// Unique name of the device being ordered
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Number of devices ordered. Defaults to 0
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// A list of attributes for the specific device. This will be null if no attributes are set
        /// </summary>
        public List<DeviceAttributeInfo> Attributes { get; set; }
    }
    public class DeviceAttributeInfo {
        /// <summary>
        /// Name of the attribute item. For example "Heartland.AMD.OfficeKey" which is specific to Portico devices for AMD. The avlue of this item is passed to Heartland for equipment boarding
        /// AttributeName and AttributeValue are optional as a pair. But if one is specified, both must be specified.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Value of the attribute item. In the above example, the value for the attribute named "Heartland.AMD.OfficeKey"
        /// AttributeName and AttributeValue are optional as a pair. But if one is specified, both must be specified.
        /// </summary>
        public string Value { get; set; }
    }
}
