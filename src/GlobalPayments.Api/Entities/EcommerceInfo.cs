using System;

namespace GlobalPayments.Api.Entities {
    public class EcommerceInfo {
        public string Cavv { get; set; }
        public EcommerceChannel Channel { get; set; }
        public string Eci { get; set; }
        public string PaymentDataSource { get; set; }
        public string PaymentDataType { get; set; }
        public int ShipDay { get; set; }
        public int ShipMonth { get; set; }
        public string Xid { get; set; }

        public EcommerceInfo() {
            Channel = EcommerceChannel.ECOM;
            ShipDay = DateTime.Now.AddDays(1).Day;
            ShipMonth = DateTime.Now.AddDays(1).Month;
            PaymentDataType = "3DSecure";
        }
    }

    public enum EcommerceChannel {
        ECOM,
        MOTO
    }
}
