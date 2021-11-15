using System.Collections.Generic;

namespace GlobalPayments.Api.Terminals.UPA
{
    public class GroupResponse
    {
        public string MultipleMessage { get; set; }
        public string MerchantName { get; set; }
        public HostResponse Host { get; set; }
        public PaymentResponse Payment { get; set; }
        public EmvResponse Emv { get; set; }
        public TransactionResponseDetail Transaction { get; set; }
        public List<SafDetailResponse> SafDetails { get; set; }
        public BatchRecordResponse BatchRecord { get; set; }
    }
}
