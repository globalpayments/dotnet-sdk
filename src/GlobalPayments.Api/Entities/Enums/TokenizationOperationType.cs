using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.Enums {
    public enum TokenizationOperationType {
        [Map(Target.NWS, "1")]
        Tokenize,
        [Map(Target.NWS, "2")]
        DeTokenize,
        [Map(Target.NWS, "3")]
        DeleteToken,
        [Map(Target.NWS, "4")]
        UpdateToken
    }
}
