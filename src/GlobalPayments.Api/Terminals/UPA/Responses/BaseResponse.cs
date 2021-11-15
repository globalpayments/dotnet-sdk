namespace GlobalPayments.Api.Terminals.UPA
{
    public class BaseResponse
    {
        public string Response { get; set; }
        public int? EcrId { get; set; }
        public int RequestId { get; set; }
        public CmdResult CmdResult { get; set; }
        public GroupResponse Data { get; set; }
    }
}
