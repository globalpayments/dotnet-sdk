using Newtonsoft.Json;

namespace GlobalPayments.Api.Terminals.UPA
{
    public class CmdResult
    {
        [JsonProperty("result")]
        public string Result { get; set; }
    }
}