using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Ingenico;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Terminals.Ingenico {
    public class StateResponse : IngenicoTerminalResponse, IDeviceResponse {

        private TerminalStatus _terminalStatus;
        private SalesMode _salesMode;
        private string _terminalCapabilities;
        private string _additionalTerminalCapabilities;
        private string _appVersionNumber;
        private string _handsetNumber;
        private string _terminalId;

        public StateResponse(byte[] buffer) 
            : base(buffer, ParseFormat.State) {
        }

        public TerminalStatus TerminalStatus { get { return _terminalStatus; } set { } }
        public SalesMode SalesMode { get { return _salesMode; } set { } }
        public string TerminalCapabilities { get { return _terminalCapabilities; } set { } }
        public string AdditionalTerminalCapabilities { get { return _additionalTerminalCapabilities; } set { } }
        public string AppVersionNumber { get { return _appVersionNumber; } set { } }
        public string HandsetNumber { get { return _handsetNumber; } set { } }
        public string TerminalId { get { return _terminalId; } set { } }

        public override void ParseResponse(byte[] response) {
            if (response == null) {
                throw new ApiException("Response data is null");
            }

            if (response.Length < INGENICO_GLOBALS.RAW_RESPONSE_LENGTH) {
                byte[] newResponse = new byte[INGENICO_GLOBALS.RAW_RESPONSE_LENGTH];
                response.CopyTo(newResponse, 0);

                response = newResponse;
            }

            base.ParseResponse(response);

            var tlv = new TypeLengthValue(response.SubArray(12, 55));
            tlv.TLVFormat = TLVFormat.State;

            string terminalStatusData = (string)tlv.GetValue((byte)StateResponseCode.Status, typeof(string));
            _terminalStatus = (TerminalStatus)Convert.ToByte(terminalStatusData.Substring(0, 1));
            _salesMode = (SalesMode)Convert.ToByte(terminalStatusData.Substring(1, 1));
            _terminalCapabilities = terminalStatusData.Substring(2, 6);
            _additionalTerminalCapabilities = terminalStatusData.Substring(8, 10);
            _appVersionNumber = (string)tlv.GetValue((byte)StateResponseCode.AppVersionNumber, typeof(string));
            _handsetNumber = (string)tlv.GetValue((byte)StateResponseCode.HandsetNumber, typeof(string));
            _terminalId = (string)tlv.GetValue((byte)StateResponseCode.TerminalId, typeof(string));
        }
    }
}
