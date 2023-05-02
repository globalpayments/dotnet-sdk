using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Utils;
using System;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE56_OriginalDataElements : IDataElement<DE56_OriginalDataElements> {
        public string MessageTypeIdentifier { get; set; }
        public string SystemTraceAuditNumber { get; set; }
        public string TransactionDateTime { get; set; }
        public string AcquiringInstitutionId { get; set; }

        public DE56_OriginalDataElements FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            MessageTypeIdentifier = sp.ReadString(4);
            SystemTraceAuditNumber = sp.ReadString(6);
            TransactionDateTime = sp.ReadString(12);
            AcquiringInstitutionId = sp.ReadLLVAR();
            return this;
        }
        public byte[] ToByteArray() {
            string rvalue = string.Concat(MessageTypeIdentifier, SystemTraceAuditNumber, TransactionDateTime);
            // put the acquirer id if present
            if (!string.IsNullOrEmpty(AcquiringInstitutionId)) {
                rvalue = string.Concat(rvalue, StringUtils.PadLeft(AcquiringInstitutionId.Length, 2, '0'), AcquiringInstitutionId);
            }
            else {
                rvalue = string.Concat(rvalue, "00");
            }
            return Encoding.ASCII.GetBytes(rvalue);
        }
        public new string ToString() {
            return Encoding.UTF8.GetString(ToByteArray());
        }
    }
}
