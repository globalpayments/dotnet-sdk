using System.IO;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Extensions;

namespace GlobalPayments.Api.Terminals.PAX {
    public class SignatureResponse : PaxTerminalResponse, ISignatureResponse {
        private DeviceType _deviceType;

        public int TotalLength { get; set; }
        public int ResponseLegth { get; set; }

        public SignatureResponse(byte[] response, DeviceType deviceType = DeviceType.PAX_S300) : base(response, PAX_MSG_ID.A09_RSP_GET_SIGNATURE, PAX_MSG_ID.A21_RSP_DO_SIGNATURE) {
            _deviceType = deviceType;
        }
    }
}
