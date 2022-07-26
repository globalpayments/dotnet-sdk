using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;
using System.Collections.Generic;

namespace GlobalPayments.Api.Terminals.UPA
{
    public class BatchReportResponse : ITerminalReport {
        const string INVALID_RESPONSE_FORMAT = "The response received is not in the proper format.";

        public BatchReportResponse(JsonDoc root) {
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

                    MerchantName = secondDataNode.GetValue<string>("merchantName");
                    Multiplemessage = secondDataNode.GetValue<string>("multipleMessage");

                    var batchRecord = secondDataNode.Get("batchRecord");
                    if (batchRecord != null) {
                        BatchRecord = new BatchRecordResponse()
                        {
                            BatchId = batchRecord.GetValue<int>("batchId"),
                            BatchSeqNbr = batchRecord.GetValue<int>("batchSeqNbr"),
                            BatchStatus = batchRecord.GetValue<string>("batchStatus"),
                            OpenUtcDateTime = batchRecord.GetValue<string>("openUtcDateTime"),
                            CloseUtcDateTime = batchRecord.GetValue<string>("closeUtcDateTime"),
                            OpenTnxId = batchRecord.GetValue<string>("openTnxId"),
                            TotalAmount = batchRecord.GetValue<decimal>("totalAmount"),
                            TotalCnt = batchRecord.GetValue<int>("totalCnt"),
                            CreditCnt = batchRecord.GetValue<int>("credictCnt"),
                            CreditAmt = batchRecord.GetValue<decimal>("creditAmt"),
                            DebitCnt = batchRecord.GetValue<int>("debitCnt"),
                            DebitAmt = batchRecord.GetValue<decimal>("debitAmt"),
                            SaleCnt = batchRecord.GetValue<int>("saleCnt"),
                            SaleAmt = batchRecord.GetValue<decimal>("saleAmt"),
                            ReturnCnt = batchRecord.GetValue<int>("returnCnt"),
                            ReturnAmt = batchRecord.GetValue<decimal>("returnAmt"),
                            TotalGratuityAmt = batchRecord.GetValue<decimal>("totalGratuityAmt"),
                            BatchTransactions = new List<BatchTransactionResponse>()
                        };

                        foreach (var transaction in batchRecord.GetEnumerator("batchTransactions")) {
                            var batchTransaction = new BatchTransactionResponse();
                            batchTransaction.CardType = transaction.GetValue<string>("cardType");
                            batchTransaction.TotalAmount = transaction.GetValue<decimal>("totalAmount");
                            batchTransaction.TotalCnt = transaction.GetValue<int>("totalCnt");
                            batchTransaction.CreditCnt = transaction.GetValue<int>("creditCnt");
                            batchTransaction.CreditAmt = transaction.GetValue<decimal>("creditAmt");
                            batchTransaction.DebitCnt = transaction.GetValue<int>("debitCnt");
                            batchTransaction.DebitAmt = transaction.GetValue<decimal>("debitAmt");
                            batchTransaction.SaleCnt = transaction.GetValue<int>("saleCnt");
                            batchTransaction.SaleAmt = transaction.GetValue<decimal>("saleAmt");
                            batchTransaction.ReturnCnt = transaction.GetValue<int>("returnCnt");
                            batchTransaction.ReturnAmt = transaction.GetValue<decimal>("returnAmt");
                            batchTransaction.TotalGratuityAmt = transaction.GetValue<decimal>("totalGratuityAmt");
                            BatchRecord.BatchTransactions.Add(batchTransaction);
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

        public string Status { get; set; }
        public string Command { get; set; }
        public string Version { get; set; }
        public string DeviceResponseCode { get; set; }
        public string DeviceResponseText { get; set; }
        public string ReferenceNumber { get; set; }

        public string Message { get; set; }
        public string Response { get; set; }
        public int EcrId { get; set; }
        public int RequestId { get; set; }
        public string Result { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public string Multiplemessage { get; set; }
        public string MerchantName { get; set; }
        public BatchRecordResponse BatchRecord { get; set; }
    }
}
