using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE124_SundryDataTag {
        [Map(Target.NWS, "00")]
        ClientSuppliedData,
        [Map(Target.NWS, "01")]
        PiggyBack_CollectTransaction,
        [Map(Target.NWS, "02")]
        PiggyBack_AuthCaptureData
    }
}
