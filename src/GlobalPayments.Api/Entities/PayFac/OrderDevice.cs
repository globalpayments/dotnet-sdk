namespace GlobalPayments.Api.Entities.PayFac {
    public class OrderDevice {
        public int AccountNum { get; set; }
        public string ShipTo { get; set; }
        public string ShipToContact { get; set; }
        public string ShipToAddress { get; set; }
        public string ShipToAddress2 { get; set; }
        public string ShipToCity { get; set; }
        public string ShipToState { get; set; }
        public string ShipToZip { get; set; }
        public string ShipToPhone { get; set; }
        public string CardholderName { get; set; }
        public string CcNum { get; set; }
        public string ExpDate { get; set; }
        public string CVV2 { get; set; }
        public string BillingZip { get; set; }
        public string Name { get; set; }
        public string Quantity { get; set; }
    }
}
