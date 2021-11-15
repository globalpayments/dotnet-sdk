namespace GlobalPayments.Api.Terminals.UPA
{
    internal class GroupRequest
    {
        public RequestParams Params { get; set; }
        public TransactionRequest Transaction { get; set; }
    }
}
