﻿using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.UPA
{
    public class UpaEODResponse : IEODResponse
    {
        const string INVALID_RESPONSE_FORMAT = "The response received is not in the proper format.";

        public UpaEODResponse(JsonDoc root)
        {
            JsonDoc firstDataNode = !TerminalUtilities.IsGpApiResponse(root) ? root.Get("data") : root.Get("response");
            if (TerminalUtilities.IsGpApiResponse(root))
            {
                DeviceResponseText = root.GetValue<string>("status");
                DeviceResponseCode = root.Get("action").GetValue<string>("result_code");
            }

            if (firstDataNode == null)
            {
                throw new MessageException(INVALID_RESPONSE_FORMAT);
            }

            var cmdResult = firstDataNode.Get("cmdResult");
            if (cmdResult == null)
            {
                throw new MessageException(INVALID_RESPONSE_FORMAT);
            }

            Status = cmdResult.GetValue<string>("result");
            Command = firstDataNode.GetValue<string>("response");
            EcrId = firstDataNode.GetValue<string>("EcrId");
            if (string.IsNullOrEmpty(Status)) 
            {
                var errorCode = cmdResult.GetValue<string>("errorCode");
                var errorMsg = cmdResult.GetValue<string>("errorMessage");
                DeviceResponseText = $"Error: {errorCode} - {errorMsg}";
            }
            else
            {
                // Unlike in other response types, this data should always be here, even if the Status is "Failed"
                var secondDataNode = firstDataNode.Get("data");
                if (secondDataNode == null)
                {
                    throw new MessageException(INVALID_RESPONSE_FORMAT);
                }

                Multiplemessage = secondDataNode.GetValue<string>("multipleMessage");

                var host = secondDataNode.Get("host");

                if (host != null)
                {
                    RespDateTime = host.GetValue<string>("respDateTime");
                    BatchId = host.GetValue<int>("batchId");
                    GatewayResponseCode = host.GetValue<int>("gatewayResponseCode");
                    GatewayResponseMessage = host.GetValue<string>("gatewayResponseMessage");
                }
            }
        }

        public string RequestId { get; set; }
        public string Multiplemessage { get; set; }

        public IDeviceResponse AttachmentResponse { get; set; }

        public IDeviceResponse BatchCloseResponse { get; set; }

        public IDeviceResponse EmvOfflineDeclineResponse { get; set; }

        public IDeviceResponse EmvPDLResponse { get; set; }

        public IDeviceResponse EmvTransactionCertificationResponse { get; set; }

        public IDeviceResponse HeartBeatResponse { get; set; }

        public IDeviceResponse ReversalResponse { get; set; }

        public ISAFResponse SAFResponse { get; set; }

        public IBatchReportResponse BatchReportResponse { get; set; }
        public string RespDateTime { get; set; }
        public int BatchId { get; set; }
        public int GatewayResponseCode { get; set; }
        public string GatewayResponseMessage { get; set; }

        public string AttachmentResponseText { get; set; }

        public string BatchCloseResponseText { get; set; }

        public string EmvOfflineDeclineResponseText { get; set; }

        public string EmvPDLResponseText { get; set; }

        public string EmvTransactionCertificationResponseText { get; set; }

        public string HeartBeatResponseText { get; set; }

        public string ReversalResponseText { get; set; }

        public string SafResponseText { get; set; }

        public string BatchReportResponseText { get; set; }

        public string Status { get; set; }
        public string Command { get; set; }
        public string Version { get; set; }
        public string DeviceResponseCode { get; set; }
        public string DeviceResponseText { get; set; }
        public string ReferenceNumber { get; set; }
        public string EcrId { get; set; }
    }
}
