using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.GP_API)]
    public enum DisputeStage {
        [Map(Target.GP_API, "RETRIEVAL")]
        Retrieval,

        [Map(Target.GP_API, "CHARGEBACK")]
        Chargeback,

        [Map(Target.GP_API, "REVERSAL")]
        Reversal,

        [Map(Target.GP_API, "SECOND_CHARGEBACK")]
        SecondChargeback,

        [Map(Target.GP_API, "PRE_ARBITRATION")]
        PreArbitration,

        [Map(Target.GP_API, "ARBITRATION")]
        Arbitration,

        [Map(Target.GP_API, "PRE_COMPLIANCE")]
        PreCompliance,

        [Map(Target.GP_API, "COMPLIANCE")]
        Compliance,

        [Map(Target.GP_API, "GOODFAITH")]
        Goodfaith
    }
}
