using Newtonsoft.Json;

namespace GlobalPayments.Api.Terminals.UPA
{
    public class EmvResponse
    {
        [JsonProperty("4F")]
        public string Emv4F { get; set; }
        [JsonProperty("50")]
        public string Emv50 { get; set; }
        [JsonProperty("5F20")]
        public string Emv5F20 { get; set; }
        [JsonProperty("5F2A")]
        public string Emv5F2A { get; set; }
        [JsonProperty("5F34")]
        public string Emv5F34 { get; set; }
        [JsonProperty("82")]
        public string Emv82 { get; set; }
        [JsonProperty("84")]
        public string Emv84 { get; set; }
        [JsonProperty("8A")]
        public string Emv8A { get; set; }
        [JsonProperty("95")]
        public string Emv95 { get; set; }
        [JsonProperty("99")]
        public string Emv99 { get; set; }
        [JsonProperty("9A")]
        public string Emv9A { get; set; }
        [JsonProperty("9B")]
        public string Emv9B { get; set; }
        [JsonProperty("9C")]
        public string Emv9C { get; set; }
        [JsonProperty("9F02")]
        public string Emv9F02 { get; set; }
        [JsonProperty("9F03")]
        public string Emv9F03 { get; set; }
        [JsonProperty("9F06")]
        public string Emv9F06 { get; set; }
        [JsonProperty("9F08")]
        public string Emv9F08 { get; set; }
        [JsonProperty("9F0D")]
        public string Emv9F0D { get; set; }
        [JsonProperty("9F0E")]
        public string Emv9F0E { get; set; }
        [JsonProperty("9F0F")]
        public string Emv9F0F { get; set; }
        [JsonProperty("9F10")]
        public string Emv9F10 { get; set; }
        [JsonProperty("9F12")]
        public string Emv9F12 { get; set; }
        [JsonProperty("9F1A")]
        public string Emv9F1A { get; set; }
        [JsonProperty("9F1E")]
        public string Emv9F1E { get; set; }
        [JsonProperty("9F26")]
        public string Emv9F26 { get; set; }
        [JsonProperty("9F27")]
        public string Emv9F27 { get; set; }
        [JsonProperty("9F33")]
        public string Emv9F33 { get; set; }
        [JsonProperty("9F34")]
        public string Emv9F34 { get; set; }
        [JsonProperty("9F35")]
        public string Emv9F35 { get; set; }
        [JsonProperty("9F36")]
        public string Emv9F36 { get; set; }
        [JsonProperty("9F37")]
        public string Emv9F37 { get; set; }
        [JsonProperty("9F40")]
        public string Emv9F40 { get; set; }
        [JsonProperty("9F41")]
        public string Emv9F41 { get; set; }
        public string TacDefault { get; set; }
        public string TacDenial { get; set; }
        public string TacOnline { get; set; }
    }

}
