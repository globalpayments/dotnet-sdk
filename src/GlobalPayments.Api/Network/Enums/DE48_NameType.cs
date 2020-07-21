using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE48_NameType {
        [Map(Target.NWS, "0")]
        CardHolderName,
        [Map(Target.NWS, "1")]
        CompanyName,
        [Map(Target.NWS, "2")]
        Secondary_Joint_AccountName
    }
}
