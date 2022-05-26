using Newtonsoft.Json;

namespace GlobalPayments.Api.Entities {
    public class DisputeDocument {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("b64_content")]
        public string Base64Content { get; set; }
    }
}
