using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE124_SundryData : IDataElement<DE124_SundryData> {
        public int EntryCount { get; set; }
        public LinkedList<DE124_SundryEntry> Entries { get; set; }

        public DE124_SundryData() {
            Entries = new LinkedList<DE124_SundryEntry>();
        }

        public DE124_SundryData FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            EntryCount = sp.ReadInt(2);
            for (int i = 0; i < EntryCount; i++) {
                DE124_SundryEntry entry = new DE124_SundryEntry();
                //entry.Tag = sp.ReadStringConstant<DE124_SundryDataTag>(2);
                entry.Tag = EnumConverter.FromMapping<DE124_SundryDataTag>(Target.NWS, sp.ReadString(2));
                string data = sp.ReadLLLVAR();
                switch (entry.Tag) {
                    case DE124_SundryDataTag.ClientSuppliedData:
                        entry.CustomerData = data;
                        break;
                    case DE124_SundryDataTag.PiggyBack_CollectTransaction:
                        StringParser ed = new StringParser(Encoding.ASCII.GetBytes(data));
                        entry.PrimaryAccountNumber = ed.ReadLLVAR();
                        entry.ProcessingCode = new DE3_ProcessingCode().FromByteArray(Encoding.ASCII.GetBytes(ed.ReadString(6)));
                        entry.TransactionAmount = StringUtils.ToAmount(ed.ReadString(12));
                        entry.SystemTraceAuditNumber = ed.ReadString(6);
                        entry.TransactionLocalDateTime = ed.ReadString(12);
                        entry.ExpirationDate = ed.ReadString(4);
                        entry.PosDataCode = new DE22_PosDataCode().FromByteArray(Encoding.ASCII.GetBytes(ed.ReadString(12)));
                        entry.FunctionCode = ed.ReadString(3);
                        entry.MessageReasonCode = ed.ReadString(4);
                        entry.ApprovalCode = ed.ReadString(6);
                        entry.BatchNumber = ed.ReadString(10);
                        entry.CardType = ed.ReadString(4);
                        entry.MessageTypeIndicator = ed.ReadString(4);
                        entry.OriginalStan = ed.ReadString(6);
                        entry.OriginalDateTime = ed.ReadString(12);
                        entry.CardIssuerData = new DE62_CardIssuerData().FromByteArray(Encoding.ASCII.GetBytes(ed.ReadLLLVAR()));
                        entry.ProductData = new DE63_ProductData().FromByteArray(Encoding.ASCII.GetBytes(ed.ReadLLLVAR()));
                        break;
                    case DE124_SundryDataTag.PiggyBack_AuthCaptureData:
                        StringParser ed1 = new StringParser(Encoding.ASCII.GetBytes(data));
                        entry.SystemTraceAuditNumber = ed1.ReadString(6);
                        entry.ApprovalCode = ed1.ReadString(6);
                        entry.TransactionAmount = StringUtils.ToAmount(ed1.ReadString(12));
                        entry.CustomerData = ed1.ReadRemaining();
                        break;
                }
                Entries.AddLast(entry);
            }
            return this;
        }

        public byte[] ToByteArray() {
            string rvalue = StringUtils.PadLeft(EntryCount + "", 2, '0');

            foreach (DE124_SundryEntry entry in Entries) {
                rvalue = string.Concat(rvalue,(EnumConverter.GetMapping(Target.NWS, entry.Tag)));
                switch (entry.Tag) {
                    case DE124_SundryDataTag.ClientSuppliedData: {
                            string length = StringUtils.PadLeft(entry.CustomerData.Length + "", 3, '0');
                            rvalue = string.Concat(rvalue,(length + entry.CustomerData));
                        } break;
                    case DE124_SundryDataTag.PiggyBack_CollectTransaction: {
                            string entryData = string.Concat(StringUtils.ToLLVar(entry.PrimaryAccountNumber)
                            ,(Encoding.UTF8.GetString(entry.ProcessingCode.ToByteArray()))
                            ,(StringUtils.ToNumeric(entry.TransactionAmount, 12))
                            ,(entry.SystemTraceAuditNumber)
                            ,(entry.TransactionLocalDateTime)
                            ,(entry.ExpirationDate)
                            ,(Encoding.UTF8.GetString(entry.PosDataCode.ToByteArray()))
                            ,(entry.FunctionCode)
                            ,(entry.MessageReasonCode)
                            ,(entry.ApprovalCode)
                            ,(entry.BatchNumber)
                            ,(entry.CardType)
                            ,(entry.MessageTypeIndicator)
                            ,(entry.OriginalStan)
                            ,(entry.OriginalDateTime)
                            ,(Encoding.UTF8.GetString(entry.CardIssuerData.ToByteArray()))
                            ,(Encoding.UTF8.GetString(entry.ProductData.ToByteArray())));

                            rvalue = string.Concat(rvalue,(StringUtils.ToLLLVar(entryData)));
                        } break;
                    case DE124_SundryDataTag.PiggyBack_AuthCaptureData: {
                            string entryData = string.Concat(entry.SystemTraceAuditNumber
                            ,(entry.ApprovalCode)
                            ,(StringUtils.ToNumeric(entry.TransactionAmount, 12))
                            ,(entry.CustomerData));

                            rvalue = string.Concat(rvalue,(StringUtils.ToLLLVar(entryData)));
                        } break;
                }
            }

            return Encoding.ASCII.GetBytes(rvalue);
        }

        public new string ToString() {
            return Encoding.UTF8.GetString(ToByteArray());
        }
    }
}
