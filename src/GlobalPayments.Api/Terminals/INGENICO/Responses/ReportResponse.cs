using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Terminals.Ingenico.Responses {
    public class ReportResponse : IngenicoTerminalResponse, ITerminalReport {
        internal ReportResponse(byte[] buffer) : base(buffer) {
            ParseResponse(buffer);
        }

        public override void ParseResponse(byte[] response) {
            base.ParseResponse(response);
            _privateData = Encoding.UTF8.GetString(response.SubArray(70, response.Length - 70));
        }
    }
}
