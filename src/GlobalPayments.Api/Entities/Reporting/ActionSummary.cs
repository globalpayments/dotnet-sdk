using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities {
    public class ActionSummary {
        public string Id { get; internal set; }
        public string Type { get; internal set; }
        public DateTime TimeCreated { get; internal set; }
        public string Resource { get; internal set; }
        public string Version { get; internal set; }
        public string ResourceId { get; internal set; }
        public string ResourceStatus { get; internal set; }
        public string HttpResponseCode { get; internal set; }
        public string ResponseCode { get; internal set; }
        public string AppId { get; internal set; }
        public string AppName { get; internal set; }
        public string AccountId { get; internal set; }
        public string AccountName { get; internal set; }
        public string MerchantName { get; internal set; }
    }
}
