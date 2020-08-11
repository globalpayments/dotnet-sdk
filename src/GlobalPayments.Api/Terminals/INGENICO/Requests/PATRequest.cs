using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Terminals.Ingenico.Requests {
    public class PATRequest {
        private byte[] _buffer;

        public PATRequestType RequestType { get; set; }
        public string WaiterId { get; set; }
        public string TableId { get; set; }
        public string TID { get; set; }
        public string TerminalCurrency { get; set; }

        public TransactionStatus TransactionOutcomeStatus { get; set; }


        public PATRequest(byte[] buffer) {
            _buffer = buffer;
            ParseData();
        }

        private void ParseData() {
            string strBuffer = Encoding.UTF8.GetString(_buffer);

            // Validating if data is XML Type
            if (strBuffer.Contains(INGENICO_GLOBALS.XML_TAG)) {

            }
            else {
                // Otherwise check if what type of message frame the request is.
                // Length of Ingenico Message Frame 2
                if (_buffer.Length == INGENICO_GLOBALS.MSG_FRAME_TWO_LEN) {
                    TransactionOutcomeStatus = (TransactionStatus)Encoding.UTF8.GetString(_buffer.SubArray(2, 1)).ToInt32();
                }
                else {
                    RequestType = (PATRequestType)Convert.ToInt32(strBuffer.Substring(11, 1));

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
