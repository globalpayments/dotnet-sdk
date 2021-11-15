namespace GlobalPayments.Api.Terminals.UPA
{
    internal class BaseRequest
    {
        public string Command { get; set; }
        public string EcrId { get; set; }
        public int RequestId { get; set; }
        public GroupRequest Data { get; set; }
    }
}
