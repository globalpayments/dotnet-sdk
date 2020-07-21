using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.VAPS | Target.Transit | Target.NWS)]
    public enum CardHolderAuthenticationEntity {
        [Map(Target.VAPS, "0")]
		[Map(Target.Transit, "NOT_AUTHENTICATED")]
        [Map(Target.NWS, "0")]
        NotAuthenticated,

        [Map(Target.VAPS, "1")]
		[Map(Target.Transit, "ICC_OFFLINE_PIN")]
        [Map(Target.NWS, "1")]
        ICC,

        [Map(Target.VAPS, "2")]
		[Map(Target.Transit, "CARD_ACCEPTANCE_DEVICE")]
        [Map(Target.NWS, "2")]
        CAD,

        [Map(Target.VAPS, "3")]
		[Map(Target.Transit, "AUTHORIZING_AGENT_ONLINE_PIN")]
        [Map(Target.NWS, "3")]
        AuthorizingAgent,

        [Map(Target.VAPS, "4")]
		[Map(Target.Transit, "MERCHANT_CARD_ACCEPTOR_SIGNATURE")]
        [Map(Target.NWS, "4")]
        ByMerchant,

        [Map(Target.VAPS, "5")]
		[Map(Target.Transit, "OTHER")]
        [Map(Target.NWS, "5")]
        Other,

        [Map(Target.VAPS, "8")]
        [Map(Target.NWS, "8")]
        CallCenter,

        [Map(Target.VAPS, "9")]
        [Map(Target.NWS, "9")]
        CardIssuer
    }
}
