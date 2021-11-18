namespace GlobalPayments.Api.Terminals.UPA
{
    internal class RootRequest
    {
        public string Message { get; set; }
        public BaseRequest  Data { get; set; }
    }
}
