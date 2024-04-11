using Newtonsoft.Json;

namespace GlobalPayments.Api.Terminals.UPA
{
    public class Data
    {
        [JsonProperty("cmdResult")]
        public CmdResult CmdResult { get; set; }

        [JsonProperty("response")]
        public string Response { get; set; }

        [JsonProperty("EcrId")]
        public string EcrId { get; set; }

        [JsonProperty("requestId")]
        public string RequestId { get; set; }
    }
}