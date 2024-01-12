using Newtonsoft.Json;

namespace GlobalPayments.Api.Terminals.UPA
{
    public class BasicResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }
}