using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE117_WIC_Data_Field_PS {
        public string UPCData { get; set; }
        public string CategoryCode { get; set; }
        public string SubCategoryCode { get; set; }
        public string Units { get; set; }
        public string ItemPrice { get; set; }
        public string PurchaseQuantity { get; set; }
        public string ItemActionCode { get; set; }
        public string OriginalItemPrice { get; set; }
        public string OriginalPurchaseQuantity { get; set; }
        public string UPCDataLength { get; set; }
    }
}
