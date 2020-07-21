using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum FeeType {
        [Map(Target.NWS, "00")]
        TransactionFee,
        [Map(Target.NWS, "22")]
        Surcharge
    }
}
