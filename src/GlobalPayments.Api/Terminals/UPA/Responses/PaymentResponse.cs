namespace GlobalPayments.Api.Terminals.UPA
{
    public class PaymentResponse
    {
        public string CardHolderName { get; set; }
        public string CardType { get; set; }
        public string CardGroup { get; set; }
        public string EbtType { get; set; }
        public string CardAcquisition { get; set; }
        public string MaskedPan { get; set; }
        public int SignatureLine { get; set; }
        public int PinVerified { get; set; }
        public int QpsQualified { get; set; }
        public int StoreAndForward { get; set; }
        public decimal ClerkId { get; set; }
        public decimal InvoiceNbr { get; set; }
        public string TrackData { get; set; }
        public int TrackNumber { get; set; }
    }
}
