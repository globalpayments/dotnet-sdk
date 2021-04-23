using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Terminals.Ingenico.Requests {
    public class TransactionOutcomeRequest : DeviceResponse {
        private byte[] _buffer;

        public new TransactionStatus Status { get; private set; }
        public string Amount { get; private set; }
        public string CurrencyCode { get; private set; }
        public string PrivateData { get; private set; }
        public DataResponse RepField { get; private set; }

        internal TransactionOutcomeRequest(byte[] buffer) {
            _buffer = buffer;
            ParseData();
        }

        private void ParseData() {
            Status = (TransactionStatus)Encoding.GetEncoding(28591).GetString(_buffer.SubArray(2, 1)).ToInt32();
            Amount = Encoding.GetEncoding(28591).GetString(_buffer.SubArray(3, 8));
            RepField = new DataResponse(_buffer.SubArray(12, 55));
            CurrencyCode = Encoding.GetEncoding(28591).GetString(_buffer.SubArray(67, 3));
            PrivateData = Encoding.GetEncoding(28591).GetString(_buffer.SubArray(70, _buffer.Length - 70));
            DeviceResponseText = Encoding.GetEncoding(28591).GetString(_buffer);
        }

        public override string ToString() {
            return DeviceResponseText;
        }
    }
}
