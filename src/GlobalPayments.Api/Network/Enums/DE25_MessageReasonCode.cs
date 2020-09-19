using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE25_MessageReasonCode {
        [Map(Target.NWS, "1376")]
        AuthCapture,
        [Map(Target.NWS, "1377")]
        StandInCapture,
        [Map(Target.NWS, "1378")]
        VoiceCapture,
        [Map(Target.NWS, "1379")]
        PinDebit_EBT_Acknowledgement,
        [Map(Target.NWS, "4021")]
        TimeoutWaitingForResponse,
        [Map(Target.NWS, "4351")]
        MerchantInitiatedReversal,
        [Map(Target.NWS, "4352")]
        CustomerInitiatedReversal,
        [Map(Target.NWS, "4353")]
        CustomerInitiated_PartialApproval,
        [Map(Target.NWS, "4354")]
        SystemTimeout_Malfunction,
        [Map(Target.NWS, "4355")]
        ForceVoid_PartialApproval,
        [Map(Target.NWS, "4356")]
        ForceVoid_ApprovedTransaction
    }
}
