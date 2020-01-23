using System.Reflection;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api {
    public class AcceptorConfig {
        public CardDataInputCapability CardDataInputCapability { get; set; }
        public CardHolderAuthenticationCapability CardHolderAuthenticationCapability { get; set; }
        public bool CardCaptureCapability { get; set; }
        public OperatingEnvironment OperatingEnvironment { get; set; }
        public CardHolderAuthenticationEntity CardHolderAuthenticationEntity { get; set; }
        public CardDataOutputCapability CardDataOutputCapability { get; set; }
        public TerminalOutputCapability TerminalOutputCapability { get; set; }
        public PinCaptureCapability PinCaptureCapability { get; set; }

        public AcceptorConfig() {
            CardDataInputCapability = CardDataInputCapability.MagStripe_KeyEntry;
            CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.None;
            OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Attended;
            CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.NotAuthenticated;
            CardDataOutputCapability = CardDataOutputCapability.None;
            TerminalOutputCapability = TerminalOutputCapability.None;
            PinCaptureCapability = PinCaptureCapability.TwelveCharacters;
        }

        public void Validate(Target target) {
            var props = GetType().GetRuntimeProperties();
            foreach (var prop in props) {
                var attrib = prop.PropertyType.GetTypeInfo().GetCustomAttribute<MapTargetAttribute>();
                if(attrib != null && attrib.Target.HasFlag(target)) {
                    object enumValue = prop.GetValue(this);
                    string value = EnumConverter.GetMapping(target, enumValue);

                    if (string.IsNullOrEmpty(value)) {
                        throw new ConfigurationException("AcceptorConfig Error: Value {0} is not valid {1} for this connector.".FormatWith(enumValue.ToString(), prop.Name));
                    }
                }
            }
        }
    }
}
