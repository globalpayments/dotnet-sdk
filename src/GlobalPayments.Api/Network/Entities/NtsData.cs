using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    public class NtsData {
        public FallbackCode FallbackCode { get; set; }
        public AuthorizerCode AuthorizerCode { get; set; }
        public DebitAuthorizerCode DebitAuthorizerCode { get; set; }

        public NtsData(FallbackCode fallbackCode, AuthorizerCode authorizerCode, string debitAuthorizerCode) {
            this.FallbackCode = fallbackCode;
            this.AuthorizerCode = authorizerCode;
            //this.DebitAuthorizerCode = ReverseStringEnumMap<DebitAuthorizerCode>.Parse<DebitAuthorizerCode>(debitAuthorizerCode);
            this.DebitAuthorizerCode = EnumConverter.FromMapping<DebitAuthorizerCode>(Target.NWS, debitAuthorizerCode);
            if (this.DebitAuthorizerCode == 0) {
                this.DebitAuthorizerCode = DebitAuthorizerCode.NonPinDebitCard;
            }
        }

        public NtsData(FallbackCode fallbackCode = FallbackCode.None, AuthorizerCode authorizerCode = AuthorizerCode.Interchange_Authorized, DebitAuthorizerCode debitAuthorizerCode = DebitAuthorizerCode.NonPinDebitCard) {
            this.FallbackCode = fallbackCode;
            this.AuthorizerCode = authorizerCode;
            this.DebitAuthorizerCode = debitAuthorizerCode;
        }

        public new string ToString() {
            return string.Concat(EnumConverter.GetMapping(Target.NWS, FallbackCode)
                    , EnumConverter.GetMapping(Target.NWS, AuthorizerCode)
                    , EnumConverter.GetMapping(Target.NWS, DebitAuthorizerCode));
        }

        public static NtsData FromString(string data) {
            if (data == null) {
                return null;
            }
            NtsData rvalue = new NtsData();
            string fallbackStr = data.Substring(0, 2);
            string authorizorCodeStr = data.Substring(2, 1);
            string debitAuthorizorCode = data.Substring(3);
            //rvalue.FallbackCode = ReverseStringEnumMap<FallbackCode>.Parse<FallbackCode>(fallbackStr);
            //rvalue.AuthorizerCode = ReverseStringEnumMap< AuthorizerCode>.Parse<AuthorizerCode>(authorizorCodeStr);
            //rvalue.DebitAuthorizerCode = ReverseStringEnumMap<DebitAuthorizerCode>.Parse<DebitAuthorizerCode>(debitAuthorizorCode);
            rvalue.FallbackCode = EnumConverter.FromMapping<FallbackCode>(Target.NWS, fallbackStr);
            rvalue.AuthorizerCode = EnumConverter.FromMapping<AuthorizerCode>(Target.NWS, authorizorCodeStr);
            rvalue.DebitAuthorizerCode = EnumConverter.FromMapping<DebitAuthorizerCode>(Target.NWS, debitAuthorizorCode);

            return rvalue;
        }

        public static NtsData InterchangeAuthorized() {
            return new NtsData(FallbackCode.None, AuthorizerCode.Interchange_Authorized);
        }

        public static NtsData HostAuthorized(FallbackCode fallbackCode) {
            return new NtsData(fallbackCode, AuthorizerCode.Host_Authorized);
        }

        public static NtsData VoiceAuthorized() {
            return VoiceAuthorized(DebitAuthorizerCode.NonPinDebitCard);
        }

        public static NtsData VoiceAuthorized(DebitAuthorizerCode debitAuthorizer) {
            return new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized, debitAuthorizer);
        }
    }
}
