using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.VAPS | Target.Transit)]
    public enum CardHolderAuthenticationEntity {
        [Map(Target.VAPS, "0")]
		[Map(Target.Transit, "NOT_AUTHENTICATED")]
        NotAuthenticated,

        [Map(Target.VAPS, "1")]
		[Map(Target.Transit, "ICC_OFFLINE_PIN")]
        ICC,

        [Map(Target.VAPS, "2")]
		[Map(Target.Transit, "CARD_ACCEPTANCE_DEVICE")]
        CAD,

        [Map(Target.VAPS, "3")]
		[Map(Target.Transit, "AUTHORIZING_AGENT_ONLINE_PIN")]
        AuthorizingAgent,

        [Map(Target.VAPS, "4")]
		[Map(Target.Transit, "MERCHANT_CARD_ACCEPTOR_SIGNATURE")]
        ByMerchant,

        [Map(Target.VAPS, "5")]
		[Map(Target.Transit, "OTHER")]
        Other,

        [Map(Target.VAPS, "8")]
        CallCenter,

        [Map(Target.VAPS, "9")]
        CardIssuer
    }
}
