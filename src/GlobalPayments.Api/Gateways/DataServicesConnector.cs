using System;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Utils;
using System.Net.Http;
using GlobalPayments.Api.Entities;
using System.Net;
using System.Collections.Generic;
using System.Globalization;

namespace GlobalPayments.Api.Gateways {
    internal class DataServicesConnector : RestGateway, IReportingService {
        private string _clientId;
        private string _clientSecret;

        public string ClientId {
            get { return _clientId; }
            set {
                _clientId = value;
                if (Headers.ContainsKey("x-ibm-client-id"))
                    Headers["x-ibm-client-id"] = value;
                else Headers.Add("x-ibm-client-id", value);
            }
        }
        public string ClientSecret {
            get { return _clientSecret; }
            set {
                _clientSecret = value;
                if (Headers.ContainsKey("x-ibm-client-secret"))
                    Headers["x-ibm-client-secret"] = value;
                else Headers.Add("x-ibm-client-secret", value);
            }
        }
        public string UserId { get; set; }

        public DataServicesConnector() {
            Headers.Add("accept", "application/json");
            Headers.Add("reporting_type", "");
            //Headers.Add("source", "ecomm");
            Timeout = 30000;
        }

        public T ProcessReport<T>(ReportBuilder<T> builder) where T : class {
            var request = new JsonDoc();

            string requestId = "10004"; // TODO: Generate this?
            string reportType = MapReportType(builder.ReportType);

            // transaction reporting
            if (builder is TransactionReportBuilder<T>) {
                var trb = builder as TransactionReportBuilder<T>;

                request.Set("requestId", requestId);
                request.Set("transactionId", trb.TransactionId);
                request.Set("userId", UserId);
                request.Set("reportType", reportType);
                request.Set("startDepositDate", trb.SearchBuilder.StartDepositDate?.ToString("dd/MM/yyyy"));
                request.Set("endDepositDate", trb.SearchBuilder.EndDepositDate?.ToString("dd/MM/yyyy"));
                request.Set("mId", trb.SearchBuilder.MerchantId);
                request.Set("hierarchy", trb.SearchBuilder.Hierarchy);
                request.Set("timezone", trb.SearchBuilder.Timezone);
                request.Set("depositReference", trb.SearchBuilder.DepositReference);
                request.Set("orderId", trb.SearchBuilder.OrderId);
                request.Set("cardNumber", string.Format("{0}{1}", trb.SearchBuilder.CardNumberFirstSix ?? "", trb.SearchBuilder.CardNumberLastFour ?? ""));
                request.Set("startTransactionLocalTime", trb.SearchBuilder.LocalTransactionStartTime?.ToString("dd/MM/yyyy hh:mm:ss"));
                request.Set("endTransactionLocalTime", trb.SearchBuilder.LocalTransactionEndTime?.ToString("dd/MM/yyyy hh:mm:ss"));
                request.Set("paymentAmount", trb.SearchBuilder.Amount.ToNumericCurrencyString());
                request.Set("bankAccountNumber", trb.SearchBuilder.BankAccountNumber);
                request.Set("caseNumber", trb.SearchBuilder.CaseNumber);
                request.Set("caseId", trb.SearchBuilder.CaseId);
            }

            Headers["reporting_type"] = reportType;
            var response = DoTransaction(HttpMethod.Post, reportType + "/", request.ToString());
            return MapResponse<T>(response, builder.ReportType);
        }

        private string MapReportType(ReportType type) {
            switch (type) {
                case ReportType.FindTransactions:
                    return "transaction";
                case ReportType.FindDeposits:
                    return "deposit";
                case ReportType.FindDisputes:
                    return "dispute";
                default:
                    throw new UnsupportedTransactionException();
            }
        }

        private T MapResponse<T>(string response, ReportType reportType) {
            var doc = JsonDoc.Parse(response);

            if (doc.Has("error")) {
                var error = doc.Get("error");
                var errorType = error.GetValue<string>("errorType");
                var errorCode = error.GetValue<string>("errorCode");
                var errorMessage = error.GetValue<string>("errorMsg");

                // If it's 1205, then it's no data... which is not an error
                if (errorCode != "1205") {
                    throw new GatewayException(errorMessage, errorCode, errorType);
                }
            }

            Func<string, DateTime?> formatDate = (date) => {
                if (!string.IsNullOrEmpty(date)) {
                    var pattern = "yyyy-MM-dd";
                    if (date.Contains(" "))
                        pattern += " hh:mm:ss";

                    return DateTime.ParseExact(date, pattern, CultureInfo.InvariantCulture);
                }
                return null;
            };

            T rvalue = Activator.CreateInstance<T>();
            if (reportType.HasFlag(ReportType.FindTransactions)) {
                Func<JsonDoc, TransactionSummary> hydrateTransactionSummary = (root) => {
                    var summary = new TransactionSummary {
                        MerchantHierarchy = root.GetValue<string>("merchantHierarchy"),
                        MerchantName = root.GetValue<string>("merchantName"),
                        MerchantDbaName = root.GetValue<string>("merchantDbaName"),
                        MerchantNumber = root.GetValue<string>("merchantNumber"),
                        MerchantCategory = root.GetValue<string>("merchantCategory"),
                        DepositDate = formatDate(root.GetValue<string>("transactionDepositDate")),
                        DepositReference = root.GetValue<string>("transactionDepositReference"),
                        DepositType = root.GetValue<string>("transactionDepositType"),
                        ServiceName = root.GetValue<string>("transactionType"),
                        OrderId = root.GetValue<string>("transactionOrderId"),
                        TransactionLocalDate = formatDate(root.GetValue<string>("transactionLocalTime")),
                        TransactionDate = formatDate(root.GetValue<string>("transactionTime")),
                        Amount = root.GetValue<string>("transactionAmount").ToAmount(),
                        Currency = root.GetValue<string>("transactionCurrency"),
                        DepositAmount = root.GetValue<string>("transactionMerchantAmount").ToAmount(),
                        DepositCurrency = root.GetValue<string>("transactionMerchantCurrency"),
                        MerchantId = root.GetValue<string>("transactionMid"),
                        TerminalId = root.GetValue<string>("transactionTid"),
                        BatchSequenceNumber = root.GetValue<string>("transactionBatchReference"),
                        EntryMode = root.GetValue<string>("transactionEntryMode"),
                        AquirerReferenceNumber = root.GetValue<string>("transactionArn"),
                        ReferenceNumber = root.GetValue<string>("transactionReferenceNumber"),
                        CardType = root.GetValue<string>("transactionCardType"),
                        MaskedCardNumber = root.GetValue<string>("transactionCardNo"),
                        AuthCode = root.GetValue<string>("transactionAuthcode"),
                        SchemeReferenceData = root.GetValue<string>("transactionSrd"),
                        AdjustmentAmount = root.GetValue<string>("transactionAdjustAmount").ToAmount(),
                        AdjustmentCurrency = root.GetValue<string>("transactionAdjustCurrency"),
                        AdjustmentReason = root.GetValue<string>("transactionAdjustReason")
                    };

                    return summary;
                };

                if (rvalue is IEnumerable<TransactionSummary>) {
                    var list = rvalue as List<TransactionSummary>;
                    if (doc.Has("merchantTransactionDetails")) {
                        foreach (var transaction in doc.GetEnumerator("merchantTransactionDetails")) {
                            list.Add(hydrateTransactionSummary(transaction));
                        }
                    }
                }
            }
            else if (reportType.HasFlag(ReportType.FindDeposits)) {
                Func<JsonDoc, DepositSummary> hydrateDepositSummary = (root) => {
                    var summary = new DepositSummary {
                        MerchantHierarchy = root.GetValue<string>("merchantHierarchy"),
                        MerchantName = root.GetValue<string>("merchantName"),
                        MerchantDbaName = root.GetValue<string>("merchantDba"),
                        MerchantNumber = root.GetValue<string>("merchantNumber"),
                        MerchantCategory = root.GetValue<string>("merchantCategory"),
                        DepositDate = DateTime.Parse(root.GetValue<string>("depositDate")),
                        Reference = root.GetValue<string>("depositReference"),
                        Amount = root.GetValue<string>("depositPaymentAmount").ToAmount(),
                        Currency = root.GetValue<string>("depositPaymentCurrency"),
                        Type = root.GetValue<string>("depositType"),
                        RoutingNumber = root.GetValue<string>("depositRoutingNumber"),
                        AccountNumber = root.GetValue<string>("depositAccountNumber"),
                        Mode = root.GetValue<string>("depositMode"),
                        SummaryModel = root.GetValue<string>("depositSummaryModel"),
                        SalesTotalCount = root.GetValue<int>("salesTotalNo"),
                        SalesTotalAmount = root.GetValue<string>("salesTotalAmount").ToAmount(),
                        SalesTotalCurrency = root.GetValue<string>("salesTotalCurrency"),
                        RefundsTotalCount = root.GetValue<int>("refundsTotalNo"),
                        RefundsTotalAmount = root.GetValue<string>("refundsTotalAmount").ToAmount(),
                        RefundsTotalCurrency = root.GetValue<string>("refundsTotalCurrency"),
                        ChargebackTotalCount = root.GetValue<int>("disputeCbTotalNo"),
                        ChargebackTotalAmount = root.GetValue<string>("disputeCbTotalAmount").ToAmount(),
                        ChargebackTotalCurrency = root.GetValue<string>("disputeCbTotalCurrency"),
                        RepresentmentTotalCount = root.GetValue<int>("disputeRepresentmentTotalNo"),
                        RepresentmentTotalAmount = root.GetValue<string>("disputeRepresentmentTotalAmount").ToAmount(),
                        RepresentmentTotalCurrency = root.GetValue<string>("disputeRepresentmentTotalCurrency"),
                        FeesTotalAmount = root.GetValue<string>("feesTotalAmount").ToAmount(),
                        FeesTotalCurrency = root.GetValue<string>("feesTotalCurrency"),
                        AdjustmentTotalCount = root.GetValue<int>("adjustmentTotalNumber"),
                        AdjustmentTotalAmount = root.GetValue<string>("adjustmentTotalAmount").ToAmount(),
                        AdjustmentTotalCurrency = root.GetValue<string>("adjustmentTotalCurrency")
                    };
                    return summary;
                };

                if (rvalue is IEnumerable<DepositSummary>) {
                    var list = rvalue as List<DepositSummary>;
                    if (doc.Has("merchantDepositDetails")) {
                        foreach (var deposit in doc.GetEnumerator("merchantDepositDetails")) {
                            list.Add(hydrateDepositSummary(deposit));
                        }
                    }
                }
            }
            else if (reportType.HasFlag(ReportType.FindDisputes)) {
                Func<JsonDoc, DisputeSummary> hydrateDisputeSummary = (root) => {
                    var summary = new DisputeSummary {
                        MerchantHierarchy = root.GetValue<string>("merchantHierarchy"),
                        MerchantName = root.GetValue<string>("merchantName"),
                        MerchantDbaName = root.GetValue<string>("merchantDba"),
                        MerchantNumber = root.GetValue<string>("merchantNumber"),
                        MerchantCategory = root.GetValue<string>("merchantCategory"),
                        DepositDate = formatDate(root.GetValue<string>("disputeDepositDate")),
                        DepositReference = root.GetValue<string>("disputeDepositReference"),
                        DepositType = root.GetValue<string>("disputeDepositType"),
                        Type = root.GetValue<string>("disputeType"),
                        CaseAmount = root.GetValue<string>("disputeCaseAmount").ToAmount(),
                        CaseCurrency = root.GetValue<string>("disputeCaseCurrency"),
                        CaseStatus = root.GetValue<string>("disputeCaseStatus"),
                        CaseDescription = root.GetValue<string>("disputeCaseDescription"),
                        TransactionOrderId = root.GetValue<string>("disputeTransactionOrderId"),
                        TransactionLocalTime = formatDate(root.GetValue<string>("disputeTransactionLocalTime")),
                        TransactionTime = formatDate(root.GetValue<string>("disputeTransactionTime")),
                        TransactionType = root.GetValue<string>("disputeTransactionType"),
                        TransactionAmount = root.GetValue<string>("disputeTransactionAmount").ToAmount(),
                        TransactionCurrency = root.GetValue<string>("disputeTransactionCurrency"),
                        CaseNumber = root.GetValue<string>("disputeCaseNo"),
                        CaseTime = formatDate(root.GetValue<string>("disputeCaseTime")),
                        CaseId = root.GetValue<string>("disputeCaseId"),
                        CaseIdTime = formatDate(root.GetValue<string>("disputeCaseIdTime")),
                        CaseMerchantId = root.GetValue<string>("disputeCaseMid"),
                        CaseTerminalId = root.GetValue<string>("disputeCaseTid"),
                        TransactionARN = root.GetValue<string>("disputeTransactionARN"),
                        TransactionReferenceNumber = root.GetValue<string>("disputeTransactionReferenceNumber"),
                        TransactionSRD = root.GetValue<string>("disputeTransactionSRD"),
                        TransactionAuthCode = root.GetValue<string>("disputeTransactionAuthcode"),
                        TransactionCardType = root.GetValue<string>("disputeTransactionCardType"),
                        TransactionMaskedCardNumber = root.GetValue<string>("disputeTransactionCardNo"),
                        Reason = root.GetValue<string>("disputeReason"),
                        IssuerComment = root.GetValue<string>("disputeIssuerComment"),
                        IssuerCaseNumber = root.GetValue<string>("disputeIssuerCaseNo"),
                        DisputeAmount = root.GetValue<string>("disputeAmount").ToAmount(),
                        DisputeCurrency = root.GetValue<string>("disputeCurrency"),
                        DisputeCustomerAmount = root.GetValue<string>("disputeCustomerAmount").ToAmount(),
                        DisputeCustomerCurrency = root.GetValue<string>("disputeCustomerCurrency"),
                        RespondByDate = formatDate(root.GetValue<string>("disputeRespondByDate")),
                        CaseOriginalReference = root.GetValue<string>("disputeCaseOriginalReference")
                    };
                    return summary;
                };

                if (rvalue is IEnumerable<DisputeSummary>) {
                    var list = rvalue as List<DisputeSummary>;
                    if (doc.Has("merchantDisputeDetails")) {
                        foreach (var dispute in doc.GetEnumerator("merchantDisputeDetails")) {
                            list.Add(hydrateDisputeSummary(dispute));
                        }
                    }
                }
            }

            return rvalue;
        }

        protected override string HandleResponse(GatewayResponse response) {
            if (response.StatusCode != HttpStatusCode.OK) {
                var message = JsonDoc.ParseSingleValue<string>(response.RawResponse, "moreInformation");
                throw new GatewayException(string.Format("Status Code: {0} - {1}", response.StatusCode, message));
            }
            return response.RawResponse;
        }
    }
}
