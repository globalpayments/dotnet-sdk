using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE48_34_MessageConfiguration : IDataElement<DE48_34_MessageConfiguration> {
        public bool? PerformDateCheck { get; set; }
        public bool? EchoSettlementData { get; set; }
        public bool? IncludeLoyaltyData { get; set; }
        public string TransactionGroupId { get; set; }
        public bool? IncrementalAuthIndicator { get; set; }

        public DE48_34_MessageConfiguration FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            PerformDateCheck = sp.ReadBoolean();
            EchoSettlementData = sp.ReadBoolean();
            IncludeLoyaltyData = sp.ReadBoolean();
            TransactionGroupId = sp.ReadString(6);
            IncrementalAuthIndicator = sp.ReadBoolean();
            return this;
        }

        public byte[] ToByteArray() {
            string rvalue = string.Concat(
                PerformDateCheck != null ? (bool)PerformDateCheck ? "1" : "0" : "0",
                EchoSettlementData != null ? (bool)EchoSettlementData ? "1" : "0" : "0",
                IncludeLoyaltyData != null ? (bool)IncludeLoyaltyData ? "1" : "0" : "0",
                string.IsNullOrEmpty(TransactionGroupId) ? "      " : TransactionGroupId,
                IncrementalAuthIndicator != null ? (bool)IncrementalAuthIndicator ? "Y" : "N" : "N"
            );

            if (!string.IsNullOrEmpty(TransactionGroupId)) {
                rvalue = string.Concat(rvalue,TransactionGroupId);
            }
            return Encoding.ASCII.GetBytes(rvalue);
        }

        public new string ToString() {
            return Encoding.UTF8.GetString(ToByteArray());
        }
    }
}
