using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE117_WIC_Data_Field_EA {
        public string UPCData { get; set; }
        public string ItemDesciption { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryDesciption { get; set; }
        public string SubCategoryCode { get; set; }
        public string SubCategoryDesciption { get; set; }
        public string UnitOfMeasure { get; set; }
        public string PackageSize { get; set; }
        public string BenefitQuantity { get; set; }
        public string BenefitUnitDescription { get; set; }
        public string UPCDataLength { get; set; }
    }
}
