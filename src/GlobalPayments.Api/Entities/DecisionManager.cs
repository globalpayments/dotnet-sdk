namespace GlobalPayments.Api.Entities {
    public class DecisionManager {
        public string BillToHostName { get; set; }
        public bool BillToHttpBrowserCookiesAccepted { get; set; }
        public string BillToHttpBrowserEmail { get; set; }
        public string BillToHttpBrowserType { get; set; }
        public string BillToIpNetworkAddress { get; set; }
        public string BusinessRulesCoreThreshold { get; set; }
        public string BillToPersonalId { get; set; }
        public string DecisionManagerProfile { get; set; }
        public string InvoiceHeaderTenderType { get; set; }
        public Risk ItemHostHedge { get; set; }
        public Risk ItemNonsensicalHedge { get; set; }
        public Risk ItemObscenitiesHedge { get; set; }
        public Risk ItemPhoneHedge { get; set; }
        public Risk ItemTimeHedge { get; set; }
        public Risk ItemVelocityHedge { get; set; }
        public bool InvoiceHeaderIsGift { get; set; }
        public bool InvoiceHeaderReturnsAccepted { get; set; }
    }
}
