using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Terminals.Ingenico {
    public class POSIdentifierResponse : IngenicoTerminalResponse, IInitializeResponse {
        public string SerialNumber { get; set; }

        public POSIdentifierResponse(byte[] buffer)
            : base(buffer, ParseFormat.PID) {
        }

        public override void ParseResponse(byte[] response) {
            base.ParseResponse(response);
            SerialNumber = Encoding.UTF8.GetString(response.SubArray(12, 55)).Trim();
        }
    }
}
