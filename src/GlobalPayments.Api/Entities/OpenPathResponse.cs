using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class OpenPathResponse
    {
        public OpenPathResponse()
        {
            Results = new List<string>();
        }

        public OpenPathStatusType Status { get; set; }
        public string Message { get; set; }
        public long TransactionId { get; set; }
        public List<string> Results { get; set; }
        public OpenPathResultModel GatewayConnectorResult { get; set; }
        public GatewayConfig BouncebackConfig { get; set; }
    }
}