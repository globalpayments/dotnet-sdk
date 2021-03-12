using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Terminals.Ingenico.Requests {
    public class TransactionOutcomeRequest {
        private byte[] _buffer;

        public DataResponse RepField { get; set; }
        public TransactionStatus Status { get; set; }

        public TransactionOutcomeRequest(byte[] buffer) {
            _buffer = buffer;
            ParseData();
        }

        private void ParseData() {
            Status = (TransactionStatus)Encoding.UTF8.GetString(_buffer.SubArray(2, 1)).ToInt32();
            RepField = new DataResponse(_buffer.SubArray(11, 55));
        }
    }
}
