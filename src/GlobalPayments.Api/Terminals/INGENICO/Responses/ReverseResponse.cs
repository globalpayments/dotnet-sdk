using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Terminals.INGENICO {
    public class ReverseResponse : IngenicoTerminalResponse, IDeviceResponse{
        public ReverseResponse(byte[] buffer) : base(buffer) {
            ParseResponse(buffer);
        }

        public override void ParseResponse(byte[] response) {
            base.ParseResponse(response);
            Status = ((ReverseStatus)Encoding.UTF8.GetString(response.SubArray(2, 1)).ToInt32()).ToString();
        }
    }
}
