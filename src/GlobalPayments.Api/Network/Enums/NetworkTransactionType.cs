using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum NetworkTransactionType {
        [Map(Target.NWS, "NT")]
        KeepAlive,
        [Map(Target.NWS, "EH")]
        Transaction
    }
}
