using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Network.Entities;

namespace GlobalPayments.Api.Network.Elements {
    public class DE127_ForwardingDataEntry {
        public DE127_ForwardingDataTag Tag{ get; set; }
        public string RecordId{ get; set; }
        public string RecordType{ get; set; }
        public string KeyBlockDataType{ get; set; }
        public string EncryptedFieldMatrix{ get; set; }
        public EncryptionType TepType{ get; set; }
        public string CardSecurityCode{ get; set; }
        public string EtbBlock{ get; set; }
        public string EntryData{ get; set; }

        public ServiceType ServiceType { get; set; }
        public OperationType OperationType { get; set; }
        public string ServiceCodeOrigin { get; set; }
        public string ServiceResponseCode { get; set; }
        public string Ksn { get; set; }
        public string EncryptedData { get; set; }
        public string TokenizationType { get; set; }
        public string TokenizedFieldMatrix { get; set; }
        public string TokenizationOperationType { get; set; }
        public string MerchantId { get; set; }
        public string TokenOrAcctNum { get; set; }
        public string ExpiryDate { get; set; }
        public bool IsFileAction { get; set; }
    }
}
