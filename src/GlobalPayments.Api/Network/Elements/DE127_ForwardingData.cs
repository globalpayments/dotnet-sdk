using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE127_ForwardingData : IDataElement<DE127_ForwardingData> {
        private LinkedList<DE127_ForwardingDataEntry> entries;

        private int GetEntryCount() {
            return entries.Count;
        }

        public DE127_ForwardingData() {
            entries = new LinkedList<DE127_ForwardingDataEntry>();
        }

        public void AddEncryptionData(EncryptionType encryptionType, string ktb) {
            AddEncryptionData(encryptionType, ktb, null);
        }

        public void AddEncryptionData(EncryptionType encryptionType, string ktb, string cvn) {
            if (cvn == null) {
                cvn = "       ";
            }
            DE127_ForwardingDataEntry entry = new DE127_ForwardingDataEntry();
            entry.Tag = DE127_ForwardingDataTag.E3_EncryptedData;
            entry.RecordId = "E3";
            entry.RecordType = "001";
            entry.KeyBlockDataType = "v";
            entry.EncryptedFieldMatrix = string.IsNullOrEmpty(cvn.Trim()) ? "03" : "04";
            entry.TepType = encryptionType;
            entry.CardSecurityCode = cvn;
            entry.EtbBlock = ktb;
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
                //entry.Tag = sp.ReadStringConstant<DE127_ForwardingDataTag> (3);
                entry.Tag = EnumConverter.FromMapping<DE127_ForwardingDataTag>(Target.NWS, sp.ReadString(3));
                string data = sp.ReadLLLVAR();
                switch (entry.Tag) {
                    case DE127_ForwardingDataTag.E3_EncryptedData:
                        StringParser ed = new StringParser(Encoding.ASCII.GetBytes(data));
                        entry.RecordId = ed.ReadString(2);
                        entry.RecordType = ed.ReadString(3);
                        entry.KeyBlockDataType = ed.ReadString(1);
                        entry.EncryptedFieldMatrix = ed.ReadString(2);
                        //entry.TepType = ed.ReadStringConstant<EncryptionType>(1);
                        entry.TepType = EnumConverter.FromMapping<EncryptionType>(Target.NWS, sp.ReadString(1));
                        ed.ReadString(18); // reserved
                        entry.CardSecurityCode = ed.ReadString(7);
                        ed.ReadString(45); // reserved
                        entry.EtbBlock = ed.ReadLLLVAR();
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
