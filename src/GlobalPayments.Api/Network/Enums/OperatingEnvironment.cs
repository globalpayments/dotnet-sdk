using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.VAPS | Target.Transit | Target.NWS)]
    public enum OperatingEnvironment {
        [Map(Target.VAPS, "0")]
		[Map(Target.Transit, "NO_TERMINAL")]
        [Map(Target.NWS, "0")]
        NoTerminalUsed,

        [Map(Target.VAPS, "1")]
		[Map(Target.Transit, "ON_MERCHANT_PREMISES_ATTENDED")]
        [Map(Target.NWS, "1")]
        OnPremises_CardAcceptor_Attended,

        [Map(Target.VAPS, "2")]
		[Map(Target.Transit, "ON_MERCHANT_PREMISES_UNATTENDED")]
        [Map(Target.NWS, "2")]
        OnPremises_CardAcceptor_Unattended,

        [Map(Target.VAPS, "3")]
		[Map(Target.Transit, "OFF_MERCHANT_PREMISES_ATTENDED")]
        [Map(Target.NWS, "3")]
        OffPremises_CardAcceptor_Attended,

        [Map(Target.VAPS, "4")]
		[Map(Target.Transit, "OFF_MERCHANT_PREMISES_UNATTENDED")]
        [Map(Target.NWS, "4")]
        OffPremises_CardAcceptor_Unattended,

        [Map(Target.VAPS, "5")]
		[Map(Target.Transit, "ON_CUSTOMER_PREMISES_UNATTENDED")]
        [Map(Target.NWS, "0")]
        OnPremises_CardHolder_Unattended,

        [Map(Target.Transit, "OFF_MERCHANT_PREMISES_MPOS")]
        OffPremises_CardAcceptor_Unattended_Mobile,

        [Map(Target.VAPS, "9")]
		[Map(Target.Transit, "ON_MERCHANT_PREMISES_MPOS")]
        [Map(Target.NWS, "9")]
        OnPremises_CardAcceptor_Unattended_Mobile,

        [Map(Target.Transit, "OFF_MERCHANT_PREMISES_CUSTOMER_POS")]
        OffPremises_CardAcceptor_CardHolder_POS,

        [Map(Target.Transit, "ON_MERCHANT_PREMISES_CUSTOMER_POS")]
        OnPremises_CardAcceptor_CardHolder_POS,

        [Map(Target.Transit, "OFF_CUSTOMER_PREMISES_UNATTENDED")]
        OffPremises_CardHolder_Unattended,

        [Map(Target.Transit, "UNKNOWN")]
        Unknown, 

        [Map(Target.Transit, "ELECTRONIC_DELIVERY_AMEX")]
        AMEX_Electronic_Delivery,

        [Map(Target.Transit, "PHYSICAL_DELIVERY_AMEX")]
        AMEX_Physical_Delivery,

        [Map(Target.VAPS, "S")]
        [Map(Target.NWS, "S")]
        Internet_With_SSL,

        [Map(Target.VAPS, "T")]
        Internet_Without_SSL,

        [Map(Target.VAPS, "U")]
        OnPremises_CardAcceptor_Attended_Mobile
    }
}
