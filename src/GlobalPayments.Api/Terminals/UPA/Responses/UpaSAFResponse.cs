using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Terminals.UPA {
    public class UpaSAFResponse : ISAFResponse {
        // There is a section in Enums.cs for SummaryType, but there is overlapping types that have different values between terminals
        // Let's leave these here for now. We can put in a better solution later
        const string AUTHORIZED = "AUTHORIZED TRANSACTIONS";
        const string PENDING = "PENDING TRANSACTIONS";
        const string FAILED = "FAILED TRANSACTIONS";

        const string INVALID_RESPONSE_FORMAT = "The response received is not in the proper format.";

        public UpaSAFResponse(JsonDoc root) {
            Approved = new Dictionary<SummaryType, SummaryResponse>();
            Pending = new Dictionary<SummaryType, SummaryResponse>();
            Declined = new Dictionary<SummaryType, SummaryResponse>();

            var firstDataNode = root.Get("data");
            if (firstDataNode == null) {
                throw new MessageException(INVALID_RESPONSE_FORMAT);
            }

            var cmdResult = firstDataNode.Get("cmdResult");
            if (cmdResult == null) {
                throw new MessageException(INVALID_RESPONSE_FORMAT);
            }

            Status = cmdResult.GetValue<string>("result");
            if (string.IsNullOrEmpty(Status)) {
                var errorCode = cmdResult.GetValue<string>("errorCode");
                var errorMsg = cmdResult.GetValue<string>("errorMessage");
                DeviceResponseText = $"Error: {errorCode} - {errorMsg}";
            }
            else {
                // If the Status is not "Success", there is either nothing to process, or something else went wrong.
                // Skip the processing of the rest of the message, as we'll likely hit null reference exceptions
                if (Status == "Success") {
                    var secondDataNode = firstDataNode.Get("data");
                    if (secondDataNode == null) {
                        throw new MessageException(INVALID_RESPONSE_FORMAT);
                    }
                    Multiplemessage = secondDataNode.GetValue<string>("multipleMessage");
                    var safDetailsList = secondDataNode.GetEnumerator("SafDetails");

                    if (safDetailsList != null) {
                        foreach (var detail in safDetailsList) {
                            if (TotalAmount == null)
                                TotalAmount = 0.00m;

                            TotalAmount += detail.GetValue<decimal>("SafTotal");
                            TotalCount += detail.GetValue<int>("SafCount");

                            SummaryResponse summaryResponse = new SummaryResponse()
                            {
                                TotalAmount = detail.GetValue<decimal>("SafTotal"),
                                Count = detail.GetValue<int>("SafCount"),
                                SummaryType = MapSummaryType(detail.GetValue<string>("SafType")),
                                Transactions = new List<TransactionSummary>()
                            };

                            if (detail.Has("SafRecords")) {
                                foreach (var record in detail.GetEnumerator("SafRecords")) {
                                    TransactionSummary transactionSummary = new TransactionSummary()
                                    {
                                        TransactionType = record.GetValue<string>("transactionType"),
                                        TerminalRefNumber = record.GetValue<string>("transId"), // The sample XML says tranNo?
                                        ReferenceNumber = record.GetValue<string>("referenceNumber"),
                                        SafReferenceNumber = record.GetValue<string>("safReferenceNumber"),                                        
                                        GratuityAmount = record.GetValue<decimal>("tipAmount"),
                                        TaxAmount = record.GetValue<decimal>("taxAmount"),
                                        Amount = record.GetValue<decimal>("baseAmount"),
                                        AuthorizedAmount = record.GetValue<decimal>("authorizedAmount"),
                                        //AmountDue = record.GetValue<decimal>("totalAmount"),
                                        CardType = record.GetValue<string>("cardType"),
                                        MaskedCardNumber = record.GetValue<string>("maskedPan"),
                                        TransactionDate = record.GetValue<DateTime>("transactionTime"),
                                        AuthCode = record.GetValue<string>("approvalCode"),
                                        HostTimeout = record.GetValue<bool>("hostTimeOut"),
                                        CardEntryMethod = record.GetValue<string>("cardAcquisition"),
                                        Status = record.GetValue<string>("responseCode"),
                                        //record.GetValue<decimal>("requestAmount")
                                    };
                                    summaryResponse.Transactions.Add(transactionSummary);
                                }
                            }

                            if (summaryResponse.SummaryType == SummaryType.Approved) {
                                if (Approved == null)
                                {
                                    Approved = new Dictionary<SummaryType, SummaryResponse>();
                                }
                                Approved.Add(summaryResponse.SummaryType, summaryResponse);
                            }
                            else if (summaryResponse.SummaryType == SummaryType.Pending) {
                                if (Pending == null) {
                                    Pending = new Dictionary<SummaryType, SummaryResponse>();
                                }
                                Pending.Add(summaryResponse.SummaryType, summaryResponse);
                            }
                            else if (summaryResponse.SummaryType == SummaryType.Declined) {
                                if (Declined == null) {
                                    Declined = new Dictionary<SummaryType, SummaryResponse>();
                                }
                                Declined.Add(summaryResponse.SummaryType, summaryResponse);
                            }
                        }
                    }
                }
                else { // the only other option is "Failed"
                    var errorCode = cmdResult.GetValue<string>("errorCode");
                    var errorMsg = cmdResult.GetValue<string>("errorMessage");
                    DeviceResponseText = $"Error: {errorCode} - {errorMsg}";
                }
            }
        }

        private SummaryType MapSummaryType(string safType) {
            switch (safType)
            {
                case AUTHORIZED:
                    return SummaryType.Approved;
                case PENDING:
                    return SummaryType.Pending;
                case FAILED:
                    return SummaryType.Declined;
                default:
                    return SummaryType.Declined;
            }
        }

        public string Multiplemessage { get; set; }

        // ISAFResponse interface
        public int TotalCount { get; set; }
        public decimal? TotalAmount  { get; set; }
        public Dictionary<SummaryType, SummaryResponse> Approved { get; set; }
        public Dictionary<SummaryType, SummaryResponse> Pending { get; set; }
        public Dictionary<SummaryType, SummaryResponse> Declined { get; set; }

        // IDeviceResponse interface
        public string Status { get; set; }
        public string Command { get; set; }
        public string Version { get; set; }
        public string DeviceResponseCode { get; set; }
        public string DeviceResponseText { get; set; }
        public string ReferenceNumber { get; set; }
    }
}
