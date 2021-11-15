using System.Collections.Generic;

namespace GlobalPayments.Api.Terminals.UPA
{
    public class SafDetailResponse
    {
        public string SafType { get; set; }
        public int SafCount { get; set; }
        public decimal SafTotal { get; set; }
        public List<SafRecordResponse> SafRecords { get; set; }    
    }
}
