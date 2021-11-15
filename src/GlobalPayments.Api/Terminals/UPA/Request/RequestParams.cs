namespace GlobalPayments.Api.Terminals.UPA
{
    internal class RequestParams
    {
        public int? ClerkId { get; set; }
        public int? TokenRequest { get; set; }
        public string TokenValue { get; set; }
        public int? DisplayOption { get; set; }
        public string LineItemLeft { get; set; }
        public string LineItemRight { get; set; }
        public int? Batch { get; set; }
    }
}
