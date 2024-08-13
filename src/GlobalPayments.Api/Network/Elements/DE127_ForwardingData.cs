using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Network.Elements
{
    public class DE127_ForwardingData : IDataElement<DE127_ForwardingData> {
        public LinkedList<DE127_ForwardingDataEntry> entries;

        public ServiceType ServiceType { get; set; }
        public OperationType OperationType { get; set; }
        public EncryptedFieldMatrix EncryptedField { get; set; }
        public TokenizationType TokenizationType { get; set; }
        public TokenizationFieldMatrix TokenizedFieldMatrix { get; set; }
        public TokenizationOperationType TokenizationOperationType { get; set; }
        public string MerchantId { get; set; }
        public string TokenOrAcctNum { get; set; }
        public string ExpiryDate { get; set; }


        private int GetEntryCount() {
            return entries.Count;
        }

        public DE127_ForwardingData() {
            entries = new LinkedList<DE127_ForwardingDataEntry>();
        }

        public void AddEncryptionData(EncryptionType encryptionType, string ktb, string ksn) {
            AddEncryptionData(encryptionType, ktb, ksn, null);
        }

        public void AddEncryptionData(EncryptionType encryptionType, string ktb, string ksn, string cvn) {
            if (cvn == null) {
                cvn = "       ";
            }
            DE127_ForwardingDataEntry entry = new DE127_ForwardingDataEntry();            
            if(encryptionType == EncryptionType.TEP1 || encryptionType == EncryptionType.TEP2) {
                entry.Tag = DE127_ForwardingDataTag.E3_EncryptedData;
                entry.RecordId = "E3";
                entry.RecordType = "001";
                entry.KeyBlockDataType = "v";
                entry.EncryptedFieldMatrix = string.IsNullOrEmpty(cvn.Trim()) ? "03" : "04";
                entry.TepType = encryptionType;
                entry.CardSecurityCode = cvn;
                entry.EtbBlock = ktb;

            }
            if (EncryptionType.TDES == encryptionType) {
                entry.Tag = DE127_ForwardingDataTag.Encryption_3DES;
                entry.RecordId = EnumConverter.GetMapping(Target.NWS, RecordId.Encryption_3DE);
                entry.RecordType = "001";
                entry.ServiceType = ServiceType.GPN_API;
                entry.TepType = encryptionType;
                entry.EncryptedFieldMatrix = EnumConverter.GetMapping(Target.NWS, EncryptedField);
                entry.OperationType = OperationType;
                entry.Ksn = ksn.PadRight(24,' ');
                entry.EncryptedData = ktb;
            }

            Add(entry);
        }

        public void AddTokenizationData(TokenizationType tokenizationType) {
            DE127_ForwardingDataEntry entry = new DE127_ForwardingDataEntry();
            entry.Tag = DE127_ForwardingDataTag.Tokenization_TOK;
            entry.RecordId = EnumConverter.GetMapping(Target.NWS, RecordId.Tokenization_TD);
            entry.RecordType = "001";
            entry.ServiceType = ServiceType.GPN_API;
            entry.TokenizationType = EnumConverter.GetMapping(Target.NWS, tokenizationType);
            entry.TokenizedFieldMatrix = EnumConverter.GetMapping(Target.NWS, TokenizedFieldMatrix);
            entry.TokenizationOperationType = EnumConverter.GetMapping(Target.NWS, TokenizationOperationType);
            entry.MerchantId = (StringUtils.PadRight(MerchantId, 32, ' '));
            entry.TokenOrAcctNum = (StringUtils.PadRight(TokenOrAcctNum, 128, ' '));
            entry.ExpiryDate = (ExpiryDate != null ? ExpiryDate : StringUtils.PadRight("", 4, ' '));

            Add(entry);
        }

        public void Add(DE127_ForwardingDataEntry entry) {
            entries.Clear();
            entries.AddLast(entry);
        }

        public DE127_ForwardingData FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            int entryCount = sp.ReadInt(2);
            for (int i = 0; i < entryCount; i++) {
                DE127_ForwardingDataEntry entry = new DE127_ForwardingDataEntry();
                entry.Tag = EnumConverter.FromMapping<DE127_ForwardingDataTag>(Target.NWS, sp.ReadString(3));
                string data = sp.ReadLLLVAR();
                switch (entry.Tag) {
                    case DE127_ForwardingDataTag.E3_EncryptedData:
                        StringParser ed = new StringParser(Encoding.ASCII.GetBytes(data));
                        entry.RecordId = ed.ReadString(2);
                        entry.RecordType = ed.ReadString(3);
                        entry.KeyBlockDataType = ed.ReadString(1);
                        entry.EncryptedFieldMatrix = ed.ReadString(2);
                        entry.TepType = EnumConverter.FromMapping<EncryptionType>(Target.NWS, sp.ReadString(1));
                        ed.ReadString(18); // reserved
                        entry.CardSecurityCode = ed.ReadString(7);
                        ed.ReadString(45); // reserved
                        entry.EtbBlock = ed.ReadLLLVAR();
                        break;
                    case DE127_ForwardingDataTag.Encryption_3DES:
                        StringParser ed3Des = new StringParser(Encoding.ASCII.GetBytes(data));
                        entry.RecordId = ed3Des.ReadString(2);
                        entry.RecordType = ed3Des.ReadString(3);
                        entry.ServiceType = EnumConverter.FromMapping<ServiceType>(Target.NWS, ed3Des.ReadString(1));
                        entry.TepType = EnumConverter.FromMapping<EncryptionType>(Target.NWS, ed3Des.ReadString(1));
                        entry.EncryptedFieldMatrix = ed3Des.ReadString(1);
                        entry.OperationType = EnumConverter.FromMapping<OperationType>(Target.NWS, ed3Des.ReadString(1));
                        entry.ServiceCodeOrigin = ed3Des.ReadString(2) ?? "  ";
                        entry.ServiceResponseCode = ed3Des.ReadString(3) ?? "  ";
                        ed3Des.ReadString(2);
                        entry.Ksn = ed3Des.ReadString(24);
                        ed3Des.ReadString(8);
                        entry.EncryptedData = ed3Des.ReadString(256);
                        ed3Des.ReadString(32);
                        ed3Des.ReadLLLVAR();
                        break;
                    case DE127_ForwardingDataTag.Tokenization_TOK:
                        StringParser edToken = new StringParser(Encoding.ASCII.GetBytes(data));
                        entry.RecordId = edToken.ReadString(2);
                        entry.RecordType = edToken.ReadString(3);
                        entry.ServiceType = EnumConverter.FromMapping<ServiceType>(Target.NWS, edToken.ReadString(1));
                        entry.TokenizationType =  edToken.ReadString(1);
                        entry.TokenizedFieldMatrix = edToken.ReadString(1);
                        entry.TokenizationOperationType = edToken.ReadString(1);
                        edToken.ReadString(7);
                        entry.MerchantId = edToken.ReadString(32);
                        entry.TokenOrAcctNum  = edToken.ReadString(128);
                        entry.ExpiryDate = edToken.ReadString(4);
                        edToken.ReadString(36);
                        break;
                    default:
                        entry.EntryData = data;
                        break;
                }
                entries.AddLast(entry);
            }

            return this;
        }

        public byte[] ToByteArray() {
            string rvalue = StringUtils.PadLeft(GetEntryCount(), 2, '0');
            foreach (DE127_ForwardingDataEntry entry in entries) {
                rvalue = string.Concat(rvalue, EnumConverter.GetMapping(Target.NWS, entry.Tag));
                switch (entry.Tag) {
                    case DE127_ForwardingDataTag.E3_EncryptedData:
                        string entryData = string.Concat(entry.RecordId,
                                entry.RecordType,
                                entry.KeyBlockDataType,
                                entry.EncryptedFieldMatrix,
                                EnumConverter.GetMapping(Target.NWS, entry.TepType),
                                StringUtils.PadRight("", 18, ' '),
                                entry.CardSecurityCode,
                                StringUtils.PadRight("", 45, ' '),
                                StringUtils.ToLLLVar(entry.EtbBlock));
        
                        rvalue = string.Concat(rvalue, StringUtils.ToLLLVar(entryData));
                        break;
                    case DE127_ForwardingDataTag.Encryption_3DES:
                        string entryDataStr = string.Concat(entry.RecordId,
                                entry.RecordType,
                                EnumConverter.GetMapping(Target.NWS, entry.ServiceType),
                                1,
                                entry.EncryptedFieldMatrix,
                                EnumConverter.GetMapping(Target.NWS, entry.OperationType),
                                StringUtils.PadRight(entry.ServiceCodeOrigin, 2, ' '),
                                StringUtils.PadRight(entry.ServiceResponseCode, 3,' '),
                                StringUtils.PadRight("", 2, ' '),
                                StringUtils.PadRight(entry.Ksn, 24, ' '),
                                StringUtils.PadRight("", 8, ' '),
                                StringUtils.PadRight(entry.EncryptedData, 256, ' '),
                                StringUtils.PadRight("", 32, ' '));

                        rvalue = string.Concat(rvalue, StringUtils.ToLLLVar(entryDataStr));
                        break;
                        case DE127_ForwardingDataTag.Tokenization_TOK:
                                string entryDataTok = string.Concat(entry.RecordId,
                                entry.RecordType,
                                EnumConverter.GetMapping(Target.NWS, entry.ServiceType),
                                entry.TokenizationType,
                                entry.TokenizedFieldMatrix,
                                entry.TokenizationOperationType,
                                StringUtils.PadRight("", 7, ' '),
                                StringUtils.PadRight(entry.MerchantId, 32, ' '),
                                StringUtils.PadRight(entry.TokenOrAcctNum, 128, ' '),
                                entry.ExpiryDate != null ? entry.ExpiryDate : StringUtils.PadRight("", 4, ' '),
                                StringUtils.PadRight("", 36, ' '));
                         rvalue = string.Concat(rvalue, StringUtils.ToLLLVar(entryDataTok));
                        break;
                    default:
                        rvalue = string.Concat(rvalue,StringUtils.ToLLLVar(entry.EntryData));
                        break;
                }
            }
            return Encoding.ASCII.GetBytes(rvalue);
        }

        public new string ToString() {
            return Encoding.UTF8.GetString(ToByteArray());
        }
    }
}
