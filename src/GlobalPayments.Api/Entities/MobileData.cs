using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class MobileData
    {
        public string EncodedData { get; set; }
        public string ApplicationReference { get; set; }
        public SdkInterface SdkInterface { get; set; }
        public SdkUiType[] SdkUiTypes { get; set; }
        public Utils.JsonDoc EphemeralPublicKey { get; set; }
        public int MaximumTimeout { get; set; }
        public string ReferenceNumber { get; set; }
        public string SdkTransReference { get; set; }

        public MobileData SetSdkUiTypes(SdkUiType sdkUiTypes)
        {            
           this.SdkUiTypes = new SdkUiType[] { sdkUiTypes };            
           return this;
        }
    }
}
