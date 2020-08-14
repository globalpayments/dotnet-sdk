using GlobalPayments.Api.Utils;
using System;
using System.Text;

namespace GlobalPayments.Api.Terminals.Ingenico.Requests {
    public class PATRequest {
        private byte[] _buffer;

        public PATRequestType RequestType { get; set; }
        public string WaiterId { get; set; }
        public string TableId { get; set; }
        public string TID { get; set; }
        public string TerminalCurrency { get; set; }
        public string XMLData { get; set; }
        public TransactionOutcomeRequest TransactionOutcome { get; set; }

        // Assign passed value of buffer into private variable.
        public PATRequest(byte[] buffer) {
            _buffer = buffer;
            ParseData();
        }

        private void ParseData() {
            string strBuffer = Encoding.UTF8.GetString(_buffer);

            // Validating if data is XML Type
            if (strBuffer.Contains(INGENICO_GLOBALS.XML_TAG)) {
                RequestType = PATRequestType.XMLData;
                XMLData = Encoding.UTF8.GetString(_buffer);
            }
            else {
                // Otherwise check if what type of message frame the request is.
                // Length of Ingenico Message Frame 2
                if (_buffer.Length == INGENICO_GLOBALS.MSG_FRAME_TWO_LEN) {
                    RequestType = PATRequestType.TransactionOutcome;
                    TransactionOutcome = new TransactionOutcomeRequest(_buffer);
                }
                else {
                    RequestType = (PATRequestType)Convert
                        .ToInt32(strBuffer.Substring(11, 1));

                    var tlvData = new TypeLengthValue(_buffer);

                    WaiterId = tlvData.GetValue((byte)'O', typeof(string))?.ToString();
                    TableId = tlvData.GetValue((byte)'L', typeof(string))?.ToString();
                    TID = tlvData.GetValue((byte)'T', typeof(string))?.ToString();
                    TerminalCurrency = tlvData.GetValue((byte)'C', typeof(string))?.ToString();
                }
            }
        }

        public override string ToString() {
            return Encoding.UTF8.GetString(_buffer);
        }
    }
}
