using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE22_CardHolderPresence {
        [Map(Target.NWS, "0")]
        CardHolder_Present,
        [Map(Target.NWS, "1")]
        CardHolder_NotPresent,
        [Map(Target.NWS, "2")]
        CardHolder_NotPresent_MailOrder,
        [Map(Target.NWS, "3")]
        CardHolder_NotPresent_Telephone,
        [Map(Target.NWS, "4")]
        CardHolder_NotPresent_StandingAuth,
        [Map(Target.NWS, "9")]
        CardHolder_NotPresent_RecurringBilling,
        [Map(Target.NWS, "S")]
        CardHolder_NotPresent_Internet,
        [Map(Target.NWS, "T")]
        ThreeD_Secure_Authenticated,
        [Map(Target.NWS, "U")]
        ThreeD_Secure_Authentication_Attempted,
        [Map(Target.NWS, "V")]
        ThreeD_Secure_Authentication_Failed,
        [Map(Target.NWS, "W")]
        InApp_DiscoverCard
    }
}
