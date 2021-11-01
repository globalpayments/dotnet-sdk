﻿namespace GlobalPayments.Api.Terminals.Abstractions {
    public interface IEODResponse : IDeviceResponse {
        IDeviceResponse AttachmentResponse { get; }
        IDeviceResponse BatchCloseResponse { get; }
        IDeviceResponse EmvOfflineDeclineResponse { get; }
        IDeviceResponse EmvPDLResponse { get; }
        IDeviceResponse EmvTransactionCertificationResponse { get; }
        IDeviceResponse HeartBeatResponse { get; }
        IDeviceResponse ReversalResponse { get; }
        ISAFResponse SAFResponse { get; }
        IBatchReportResponse BatchReportResponse { get; }

        string AttachmentResponseText { get; }
        string BatchCloseResponseText { get; }
        string EmvOfflineDeclineResponseText { get; }
        string EmvPDLResponseText { get; }
        string EmvTransactionCertificationResponseText { get; }
        string HeartBeatResponseText { get; }
        string ReversalResponseText { get; }
        string SafResponseText { get; }
        string BatchReportResponseText { get; }
    }
}
