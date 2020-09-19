using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System;

namespace GlobalPayments.Api.ServiceConfigs {
    public class AcceptorConfig {
        // DE??
        public Address Address { get; set; }

        // DE22 - POS DATA CODE
        private CardDataInputCapability cardDataInputCapability = CardDataInputCapability.MagStripe_KeyEntry;
        private CardHolderAuthenticationCapability cardHolderAuthenticationCapability = CardHolderAuthenticationCapability.None;
        private bool cardCaptureCapability = false;
        private OperatingEnvironment operatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Attended;
        private CardHolderAuthenticationEntity cardHolderAuthenticationEntity = CardHolderAuthenticationEntity.NotAuthenticated;
        private CardDataOutputCapability cardDataOutputCapability = CardDataOutputCapability.None;
        private TerminalOutputCapability terminalOutputCapability = TerminalOutputCapability.None;
        private PinCaptureCapability pinCaptureCapability = PinCaptureCapability.TwelveCharacters;

        // DE48_2 - HARDWARE SOFTWARE CONFIG
        public string HardwareLevel{ get; set; }
        public string SoftwareLevel{ get; set; }
        public string OperatingSystemLevel{ get; set; }
        // DE48_33 | DE62.NPC - POS CONFIGURATION
        public string Timezone{ get; set; }
        public bool? SupportsPartialApproval{ get; set; }
        public bool? SupportsReturnBalance{ get; set; }
        public bool? SupportsCashOver{ get; set; }
        public bool? MobileDevice{ get; set; }
        public bool? SupportsShutOffAmount{ get; set; }
        public bool? SupportsDiscoverNetworkReferenceId{ get; set; }
        public bool? SupportsAvsCnvVoidReferrals{ get; set; }
        public bool? SupportsEmvPin{ get; set; }

        // DE127 - FORWARDING DATA
        private EncryptionType supportedEncryptionType = EncryptionType.TEP2;

        // DE??
        // DE22 - POS DATA CODE
        public CardDataInputCapability GetCardDataInputCapability() {
            return cardDataInputCapability;
        }
        public void SetCardDataInputCapability(CardDataInputCapability cardDataInputCapability) {
            this.cardDataInputCapability = cardDataInputCapability;
        }
        public CardHolderAuthenticationCapability GetCardHolderAuthenticationCapability() {
            return cardHolderAuthenticationCapability;
        }
        public void SetCardHolderAuthenticationCapability(CardHolderAuthenticationCapability cardHolderAuthenticationCapability) {
            this.cardHolderAuthenticationCapability = cardHolderAuthenticationCapability;
        }
        public bool IsCardCaptureCapability() {
            return cardCaptureCapability;
        }
        public void SetCardCaptureCapability(bool cardCaptureCapability) {
            this.cardCaptureCapability = cardCaptureCapability;
        }
        public OperatingEnvironment GetOperatingEnvironment() {
            return operatingEnvironment;
        }
        public void SetOperatingEnvironment(OperatingEnvironment operatingEnvironment) {
            this.operatingEnvironment = operatingEnvironment;
        }
        public CardHolderAuthenticationEntity GetCardHolderAuthenticationEntity() {
            return cardHolderAuthenticationEntity;
        }
        public void SetCardHolderAuthenticationEntity(CardHolderAuthenticationEntity cardHolderAuthenticationEntity) {
            this.cardHolderAuthenticationEntity = cardHolderAuthenticationEntity;
        }
        public CardDataOutputCapability GetCardDataOutputCapability() {
            return cardDataOutputCapability;
        }
        public void SetCardDataOutputCapability(CardDataOutputCapability cardDataOutputCapability) {
            this.cardDataOutputCapability = cardDataOutputCapability;
        }
        public TerminalOutputCapability GetTerminalOutputCapability() {
            return terminalOutputCapability;
        }
        public void SetTerminalOutputCapability(TerminalOutputCapability terminalOutputCapability) {
            this.terminalOutputCapability = terminalOutputCapability;
        }
        public PinCaptureCapability GetPinCaptureCapability() {
            return pinCaptureCapability;
        }
        public void SetPinCaptureCapability(PinCaptureCapability pinCaptureCapability) {
            this.pinCaptureCapability = pinCaptureCapability;
        }
        // DE48_2 - HARDWARE SOFTWARE CONFIG
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
        public string GetPosConfigForIssuerData() {
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

        // DE127 FORWARDING DATA
        public EncryptionType GetSupportedEncryptionType() {
            return supportedEncryptionType;
        }
        public void SetSupportedEncryptionType(EncryptionType supportedEncryptionType) {
            this.supportedEncryptionType = supportedEncryptionType;
        }

        public void Validate() {
            string hardwareLevel = (HardwareLevel == null) ? "" : HardwareLevel;
            string softwareLevel = (SoftwareLevel == null) ? "" : SoftwareLevel;
            string operatingSystemLevel = (OperatingSystemLevel == null) ? "" : OperatingSystemLevel;
            if (string.Concat(hardwareLevel, softwareLevel, operatingSystemLevel).Length > 20) {
                throw new ConfigurationException("The values for Hardware, Software and Operating System Level cannot exceed a combined length of 20 characters.");
            }
            if (Address != null) {
                if (Address.Name == null || Address.StreetAddress1 == null || Address.City == null) {
                    throw new ConfigurationException("Missing Acceptor Address Field: Name, Street1 or City.");
                }
                if (Address.PostalCode == null || Address.State == null || Address.Country == null) {
                    throw new ConfigurationException("Missing Acceptor Address Field: PostalCode, State/Region or Country.");
                }
            }
        }
    }
}
