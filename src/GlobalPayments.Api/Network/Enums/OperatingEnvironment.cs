using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.VAPS | Target.Transit)]
    public enum OperatingEnvironment {
        [Map(Target.VAPS, "0")]
		[Map(Target.Transit, "NO_TERMINAL")]
        NoTerminalUsed,

        [Map(Target.VAPS, "1")]
		[Map(Target.Transit, "ON_MERCHANT_PREMISES_ATTENDED")]
        OnPremises_CardAcceptor_Attended,

        [Map(Target.VAPS, "2")]
		[Map(Target.Transit, "ON_MERCHANT_PREMISES_UNATTENDED")]
        OnPremises_CardAcceptor_Unattended,

        [Map(Target.VAPS, "3")]
		[Map(Target.Transit, "OFF_MERCHANT_PREMISES_ATTENDED")]
        OffPremises_CardAcceptor_Attended,

        [Map(Target.VAPS, "4")]
		[Map(Target.Transit, "OFF_MERCHANT_PREMISES_UNATTENDED")]
        OffPremises_CardAcceptor_Unattended,

        [Map(Target.VAPS, "5")]
		[Map(Target.Transit, "ON_CUSTOMER_PREMISES_UNATTENDED")]
        OnPremises_CardHolder_Unattended,

        [Map(Target.Transit, "OFF_MERCHANT_PREMISES_MPOS")]
        OffPremises_CardAcceptor_Unattended_Mobile,

        [Map(Target.VAPS, "9")]
		[Map(Target.Transit, "ON_MERCHANT_PREMISES_MPOS")]
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
        Internet_With_SSL,

        [Map(Target.VAPS, "T")]
        Internet_Without_SSL,

        [Map(Target.VAPS, "U")]
        OnPremises_CardAcceptor_Attended_Mobile
    }
}
