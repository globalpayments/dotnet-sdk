using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.Enums {
    public enum TokenizationFieldMatrix {
        [Map(Target.NWS, "1")]
        AccountNumber,
        [Map(Target.NWS, "2")]
        TokenizedData
    }
}
