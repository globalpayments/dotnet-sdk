using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class EWICData {
        private DE117_WIC_Data_Fields ewicData;
        public EWICData() {
            ewicData = new DE117_WIC_Data_Fields();
        }

        public void Add(DE117_WIC_Data_Field_EA eaData) {
            //ewicData.DataSetIdentifier = "EA";
            ewicData.EAData = eaData;
        }

        public void Add(DE117_WIC_Data_Field_PS psData) {
            //ewicData.DataSetIdentifier = "PS";
            ewicData.PSData = psData;
        }

        public DE117_WIC_Data_Fields ToDataElement() {
            return ewicData;
        }
    }
}
