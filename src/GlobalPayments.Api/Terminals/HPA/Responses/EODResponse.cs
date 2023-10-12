using System;
using System.Text;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.HPA.Responses {
    public class EODResponse : SipBaseResponse, IEODResponse {
        private StringBuilder _sendSafMessageBuilder;
        private StringBuilder _batchReportMessageBuilder;

        public IDeviceResponse AttachmentResponse { get; private set; }
        public IDeviceResponse BatchCloseResponse { get; private set; }
        public IDeviceResponse EmvOfflineDeclineResponse { get; private set; }
        public IDeviceResponse EmvPDLResponse { get; private set; }
        public IDeviceResponse EmvTransactionCertificationResponse { get; private set; }
        public IDeviceResponse HeartBeatResponse { get; private set; }
        public IDeviceResponse ReversalResponse { get; private set; }
        public ISAFResponse SAFResponse { get; private set; }
        public IBatchReportResponse BatchReportResponse { get; private set; }

        private string attachementResponseText;
        private string batchCloseResponseText;
        private string emvOfflineDeclineResponseText;
        private string emvPDLResponseText;
        private string emvTransactionCertificationResponseText;
        private string heartBeatResponseText;
        private string reversalResponseText;
        private string safResponseText;
        private string batchReportResponseText;

        public string RespDateTime { get; set; }
        public int BatchId { get; set; }
        public int GatewayResponseCode { get; set; }
        public string GatewayResponseMessage { get; set; }

        public string AttachmentResponseText {
            get {
                if (AttachmentResponse != null) {
                    return AttachmentResponse.DeviceResponseText;
                }
                return attachementResponseText;
            }
            private set { attachementResponseText = value; }
        }

        public string BatchCloseResponseText {
            get {
                if (BatchCloseResponse != null) {
                    return BatchCloseResponse.DeviceResponseText;
                }
                return batchCloseResponseText;
            }
            private set { batchCloseResponseText = value; }
        }

        public string EmvOfflineDeclineResponseText {
            get {
                if (EmvOfflineDeclineResponse != null) {
                    return EmvOfflineDeclineResponse.DeviceResponseText;
                }
                return emvOfflineDeclineResponseText;
            }
            private set { emvOfflineDeclineResponseText = value; }
        }

        public string EmvPDLResponseText {
            get {
                if (EmvPDLResponse != null) {
                    return EmvPDLResponse.DeviceResponseText;
                }
                return emvPDLResponseText;
            }
            private set { emvPDLResponseText = value; }
        }

        public string EmvTransactionCertificationResponseText {
            get {
                if (EmvTransactionCertificationResponse != null) {
                    return EmvTransactionCertificationResponse.DeviceResponseText;
                }
                return emvTransactionCertificationResponseText;
            }
            private set { emvTransactionCertificationResponseText = value; }
        }

        public string HeartBeatResponseText {
            get {
                if (HeartBeatResponse != null) {
                    return HeartBeatResponse.DeviceResponseText;
                }
                return heartBeatResponseText;
            }
            private set { heartBeatResponseText = value; }
        }

        public string ReversalResponseText {
            get {
                if (ReversalResponse != null) {
                    return ReversalResponse.DeviceResponseText;
                }
                return reversalResponseText;
            }
            private set { reversalResponseText = value; }
        }

        public string SafResponseText {
            get {
                if (SAFResponse != null) {
                    return SAFResponse.DeviceResponseText;
                }
                return safResponseText;
            }
            private set { safResponseText = value; }
        }

        public string BatchReportResponseText {
            get {
                if (BatchReportResponse != null) {
                    return BatchReportResponse.DeviceResponseText;
                }
                return batchReportResponseText;
            }
            private set { batchReportResponseText = value; }
        }

        public IBatchReportResponse getBatchReportResponse() {
            return BatchReportResponse;
        }

        public IBatchReportResponse getSafReportResponse()
        {
            return BatchReportResponse;
        }

        public ISAFResponse getSAFResponse() {
            return SAFResponse;
        }

        public EODResponse(byte[] buffer, params string[] messageIds) : base(buffer, messageIds) {
            if (_sendSafMessageBuilder != null) {
                string messages = _sendSafMessageBuilder.ToString();
                if (!string.IsNullOrEmpty(messages)) {
                    try {
                        SAFResponse = new SAFResponse(Encoding.UTF8.GetBytes(messages), EODCommandType.SENDSAF);
                    }
                    catch (ApiException exc) {
                        safResponseText = exc.Message;
                    }
                }
            }

            if (_batchReportMessageBuilder != null) {
                string messages = _batchReportMessageBuilder.ToString();
                if (!string.IsNullOrEmpty(messages)) {
                    try {
                        BatchReportResponse = new BatchReportResponse(Encoding.UTF8.GetBytes(messages), EODCommandType.GET_BATCH_REPORT);
                    }
                    catch (ApiException exc) {
                        batchReportResponseText = exc.Message;
                    }
                }
            }
        }

        internal override void MapResponse(Element response) {
            base.MapResponse(response);

            if (Command.Equals(EODCommandType.SENDSAF, StringComparison.OrdinalIgnoreCase)) {
                if (_sendSafMessageBuilder == null) {
                    _sendSafMessageBuilder = new StringBuilder();
                }
                _sendSafMessageBuilder.Append(currentMessage).Append('\r');
            }
            else if (Command.Equals(EODCommandType.GET_BATCH_REPORT, StringComparison.OrdinalIgnoreCase)) {
                if (_batchReportMessageBuilder == null) {
                    _batchReportMessageBuilder = new StringBuilder();
                }
                _batchReportMessageBuilder.Append(currentMessage).Append('\r');
            }
            else if (Command.Equals(EODCommandType.GET_SAF_REPORT, StringComparison.OrdinalIgnoreCase))
            {
                if (_batchReportMessageBuilder == null)
                {
                    _batchReportMessageBuilder = new StringBuilder();
                }
                _batchReportMessageBuilder.Append(currentMessage).Append('\r');
            }
            else if (Command.Equals(EODCommandType.HEARTBEAT, StringComparison.OrdinalIgnoreCase)) {
                try {
                    HeartBeatResponse = new HeartBeatResponse(Encoding.UTF8.GetBytes(currentMessage), EODCommandType.HEARTBEAT);
                }
                catch (ApiException exc) {
                    batchReportResponseText = exc.Message;
                }
            }
            else if (Command.Equals(EODCommandType.END_OF_DAY, StringComparison.OrdinalIgnoreCase)) {
                AttachmentResponseText = response.GetValue<string>("Attachment");
                BatchCloseResponseText = response.GetValue<string>("BatchClose");
                EmvOfflineDeclineResponseText = response.GetValue<string>("EMVOfflineDecline");
                EmvPDLResponseText = response.GetValue<string>("EMVPDL");
                EmvTransactionCertificationResponseText = response.GetValue<string>("TransactionCertificate");
                HeartBeatResponseText = response.GetValue<string>("HeartBeat");
                ReversalResponseText = response.GetValue<string>("Reversal");
                SafResponseText = response.GetValue<string>("SendSAF");
                BatchReportResponseText = response.GetValue<string>("GetBatchReport");
            }
            else {
                try {
                    SipBaseResponse subResponse = new SipBaseResponse(Encoding.UTF8.GetBytes(currentMessage), Command);
                    if (Command.Equals(EODCommandType.REVERSAL, StringComparison.OrdinalIgnoreCase)) { ReversalResponse = subResponse; }
                    else if (Command.Equals(EODCommandType.EMV_OFFLINE_DECLINE, StringComparison.OrdinalIgnoreCase)) { EmvOfflineDeclineResponse = subResponse; }
                    else if (Command.Equals(EODCommandType.TRANSACTION_CERTIFICATE, StringComparison.OrdinalIgnoreCase)) { EmvTransactionCertificationResponse = subResponse; }
                    else if (Command.Equals(EODCommandType.ATTACHMENT, StringComparison.OrdinalIgnoreCase)) { AttachmentResponse = subResponse; }
                    else if (Command.Equals(EODCommandType.EMV_PARAMETER_DOWNLOAD, StringComparison.OrdinalIgnoreCase)) { EmvPDLResponse = subResponse; }
                    else if (Command.Equals(EODCommandType.BATCH_CLOSE, StringComparison.OrdinalIgnoreCase)) { BatchCloseResponse = subResponse; }
                }
                catch (ApiException exc) {
                    if (Command.Equals(EODCommandType.REVERSAL, StringComparison.OrdinalIgnoreCase)) { reversalResponseText = exc.Message; }
                    else if (Command.Equals(EODCommandType.EMV_OFFLINE_DECLINE, StringComparison.OrdinalIgnoreCase)) { emvOfflineDeclineResponseText = exc.Message; }
                    else if (Command.Equals(EODCommandType.TRANSACTION_CERTIFICATE, StringComparison.OrdinalIgnoreCase)) { emvTransactionCertificationResponseText = exc.Message; }
                    else if (Command.Equals(EODCommandType.ATTACHMENT, StringComparison.OrdinalIgnoreCase)) { attachementResponseText = exc.Message; }
                    else if (Command.Equals(EODCommandType.EMV_PARAMETER_DOWNLOAD, StringComparison.OrdinalIgnoreCase)) { emvPDLResponseText = exc.Message; }
                    else if (Command.Equals(EODCommandType.BATCH_CLOSE, StringComparison.OrdinalIgnoreCase)) { batchCloseResponseText = exc.Message; }
                }
            }
        }
    }
}
