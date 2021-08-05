using System;
using System.Reflection;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
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
        public string HardwareLevel { get; set; }
        public string SoftwareLevel { get; set; }
        public string OperatingSystemLevel { get; set; }
        public string Timezone { get; set; }
        public bool? SupportsPartialApproval { get; set; }
        public bool? SupportsReturnBalance { get; set; }
        public bool? SupportsCashOver { get; set; }
        public bool? MobileDevice { get; set; }
        public bool? SupportsShutOffAmount { get; set; }
        public bool? SupportsDiscoverNetworkReferenceId { get; set; }
        public bool? SupportsAvsCnvVoidReferrals { get; set; }
        public bool? SupportsEmvPin { get; set; }
        public EncryptionType SupportedEncryptionType { get; set; }
        public Address Address { get; set; }
        public bool? PerformDateCheck { get; set; }
        public bool? EchoSettlementData { get; set; }
        public bool? IncludeLoyaltyData { get; set; }

        public AcceptorConfig() {
            CardDataInputCapability = CardDataInputCapability.MagStripe_KeyEntry;
            CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.None;
            OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Attended;
            CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.NotAuthenticated;
            CardDataOutputCapability = CardDataOutputCapability.None;
            TerminalOutputCapability = TerminalOutputCapability.None;
            PinCaptureCapability = PinCaptureCapability.TwelveCharacters;
            SupportedEncryptionType = EncryptionType.TEP2;
        }

        public void Validate(Target target) {
            var props = GetType().GetRuntimeProperties();
            foreach (var prop in props) {
                var attrib = prop.PropertyType.GetTypeInfo().GetCustomAttribute<MapTargetAttribute>();
                if (attrib != null && attrib.Target.HasFlag(target)) {
                    object enumValue = prop.GetValue(this);
                    string value = EnumConverter.GetMapping(target, enumValue);

                    if (string.IsNullOrEmpty(value)) {
                        throw new ConfigurationException("AcceptorConfig Error: Value {0} is not valid {1} for this connector.".FormatWith(enumValue.ToString(), prop.Name));
                    }
                }
            }
        }
        public bool HasPosConfiguration_MessageControl() {
            return (!string.IsNullOrEmpty(Timezone)
                    || SupportsPartialApproval != null
                    || SupportsReturnBalance != null
                    || SupportsCashOver != null
                    || MobileDevice != null);
        }
        public bool HasPosConfiguration_IssuerData() {
            return (SupportsPartialApproval != null
                    || SupportsShutOffAmount != null
                    || SupportsReturnBalance != null
                    || SupportsDiscoverNetworkReferenceId != null
                    || SupportsAvsCnvVoidReferrals != null
                    || SupportsEmvPin != null
                    || MobileDevice != null);
        }

        public bool HasPosConfiguration_MessageData() {
            return (PerformDateCheck != null
                    || EchoSettlementData != null
                    || IncludeLoyaltyData != null);
        }

        public string GetPosConfigForIssuerData()        {
            string rvalue = SupportsPartialApproval != null ? (bool)SupportsPartialApproval ? "Y" : "N" : "N";
            rvalue = string.Concat(rvalue, (SupportsShutOffAmount != null ? (bool)SupportsShutOffAmount ? "Y" : "N" : "N")
                    , ("N")
                    , (SupportsReturnBalance != null ? (bool)SupportsReturnBalance ? "Y" : "N" : "N")
                    , (SupportsDiscoverNetworkReferenceId != null ? (bool)SupportsDiscoverNetworkReferenceId ? "Y" : "N" : "N")
                    , (SupportsAvsCnvVoidReferrals != null ? (bool)SupportsAvsCnvVoidReferrals ? "Y" : "N" : "N")
                    , (SupportsEmvPin != null ? (bool)SupportsEmvPin ? "Y" : "N" : "N")
                    , (MobileDevice != null ? (bool)MobileDevice ? "Y" : "N" : "N")
                    , ("N"));
            return rvalue;
        }
    }
}
