using System;
using System.Collections.Generic;
using System.Text;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.HPA.Responses {
    public class BatchReportResponse : SipKvpResponse, IBatchReportResponse {
        private TransactionSummary _lastTransactionSummary;

        public BatchSummary BatchSummary { get; private set; }
        public CardBrandSummary VisaSummary { get; private set; }
        public CardBrandSummary MasterCardSummary { get; private set; }
        public CardBrandSummary AmexSummary { get; private set; }
        public CardBrandSummary DiscoverSummary { get; private set; }
        public CardBrandSummary PaypalSummary { get; private set; }
        public List<TransactionSummary> TransactionSummaries { get; private set; }

        public BatchReportResponse(byte[] buffer, params string[] messageIds) : base(buffer, messageIds) { }

        internal override void MapResponse(Element response) {
            category = response.GetValue<string>("TableCategory");

            if (category != null) {
                if (category.Equals("BATCH DETAIL", StringComparison.OrdinalIgnoreCase) || category.Equals("BATCH SUMMARY", StringComparison.OrdinalIgnoreCase)) {
                    base.MapResponse(response);
                    if (BatchSummary == null) {
                        BatchSummary = new BatchSummary();
                    }

                    BatchSummary.MerchantName = fieldValues.GetValue<string>("MerchantName");
                    BatchSummary.SiteId = fieldValues.GetValue<string>("SiteId");
                    BatchSummary.DeviceId = fieldValues.GetValue<string>("DeviceId");
                    BatchSummary.Id = fieldValues.GetValue<int>("BatchId");
                    BatchSummary.SequenceNumber = fieldValues.GetValue<string>("BatchSeqNbr");
                    BatchSummary.Status = fieldValues.GetValue<string>("BatchStatus");
                    BatchSummary.OpenTime = fieldValues.GetValue<string>("OpenUtcDT").ToDateTime();
                    BatchSummary.OpenTransactionId = fieldValues.GetValue<string>("OpenTxnId");
                    BatchSummary.CloseTransactionId = fieldValues.GetValue<string>("CloseTxnId");
                    BatchSummary.CloseCount = fieldValues.GetValue<int>("BatchTxnCnt");
                    BatchSummary.TotalAmount = fieldValues.GetAmount("BatchTxnAmt");
                    BatchSummary.CreditCount = fieldValues.GetValue<int>("CreditCnt");
                    BatchSummary.CreditAmount = fieldValues.GetAmount("CreditAmt");
                    BatchSummary.DebitCount = fieldValues.GetValue<int>("DebitCnt");
                    BatchSummary.DebitAmount = fieldValues.GetAmount("DebitAmt");
                    BatchSummary.SaleCount = fieldValues.GetValue<int>("SaleCnt");
                    BatchSummary.SaleAmount = fieldValues.GetAmount("SaleAmt");
                    BatchSummary.ReturnCount = fieldValues.GetValue<int>("ReturnCtn");
                    BatchSummary.ReturnAmount = fieldValues.GetAmount("ReturnAmt");
                }

                if (category.Equals("VISA CARD SUMMARY", StringComparison.OrdinalIgnoreCase) ||
                        category.Equals("MASTERCARD CARD SUMMARY", StringComparison.OrdinalIgnoreCase) ||
                        category.Equals("AMERICAN EXPRESS CARD SUMMARY", StringComparison.OrdinalIgnoreCase) ||
                        category.Equals("DISCOVER CARD SUMMARY", StringComparison.OrdinalIgnoreCase) ||
                        category.Equals("PAYPAL CARD SUMMARY", StringComparison.OrdinalIgnoreCase)) {
                    try { 
                        CardBrandSummary brandSummary = new CardBrandSummary(Encoding.UTF8.GetBytes(currentMessage), "GetBatchReport");
                        if (category.Equals("VISA CARD SUMMARY")) { VisaSummary = brandSummary; }
                        if (category.Equals("MASTERCARD CARD SUMMARY")) { MasterCardSummary = brandSummary; }
                        if (category.Equals("AMERICAN EXPRESS CARD SUMMARY")) { AmexSummary = brandSummary; }
                        if (category.Equals("DISCOVERY CARD SUMMARY")) { DiscoverSummary = brandSummary; }
                        if (category.Equals("PAYPAL CARD SUMMARY")) { PaypalSummary = brandSummary; }
                    }
                    catch (ApiException) { /* NOM NOM */ }
                }
                
                if (category.StartsWith("TRANSACTION", StringComparison.OrdinalIgnoreCase)) {
                    base.MapResponse(response);
                    TransactionSummary summary = new TransactionSummary();
                    if (category.Equals(lastCategory, StringComparison.OrdinalIgnoreCase)) {
                        summary = _lastTransactionSummary;
                    }

                    summary.ReferenceNumber = fieldValues.GetValue<string>("ReferenceNumber");
                    summary.TransactionDate = fieldValues.GetValue<string>("TransactionTime").ToDateTime();
                    summary.TransactionStatus = fieldValues.GetValue<string>("TransactionStatus");
                    summary.MaskedCardNumber = fieldValues.GetValue<string>("MaskedPAN");
                    summary.CardType = fieldValues.GetValue<string>("CardType");
                    summary.TransactionType = fieldValues.GetValue<string>("TransactionType");
                    summary.CardEntryMethod = fieldValues.GetValue<string>("CardAcquisition");
                    summary.AuthCode = fieldValues.GetValue<string>("ApprovalCode");
                    summary.GatewayResponseCode = fieldValues.GetValue<string>("Responsecode");
                    summary.GatewayResponseMessage = fieldValues.GetValue<string>("ResponseText");
                    summary.CashBackAmount = fieldValues.GetAmount("CashbackAmount");
                    summary.GratuityAmount = fieldValues.GetAmount("TipAmount");
                    summary.AuthorizedAmount = fieldValues.GetAmount("AuthorizedAmount");
                    summary.SettlementAmount = fieldValues.GetAmount("SettleAmount");
                    summary.Amount = fieldValues.GetAmount("RequestedAmount");

                    if (!category.Equals(lastCategory, StringComparison.OrdinalIgnoreCase)) {
                        if (TransactionSummaries == null) {
                            TransactionSummaries = new List<TransactionSummary>();
                        }
                        TransactionSummaries.Add(summary);
                    }
                    _lastTransactionSummary = summary;
                }
                lastCategory = category;
            }
        }

        private CardType MapCardType(string category) {
            if (category.Equals(CardSummaryType.VISA, StringComparison.OrdinalIgnoreCase)) {
                return CardType.VISA;
            }
            else if (category.Equals(CardSummaryType.MC, StringComparison.OrdinalIgnoreCase)) {
                return CardType.MC;
            }
            else if (category.Equals(CardSummaryType.AMEX, StringComparison.OrdinalIgnoreCase)) {
                return CardType.AMEX;
            }
            else if (category.Equals(CardSummaryType.DISCOVER, StringComparison.OrdinalIgnoreCase)) {
                return CardType.DISC;
            }
            else if (category.Equals(CardSummaryType.PAYPAL, StringComparison.OrdinalIgnoreCase)) {
                return CardType.PAYPALECOMMERCE;
            }
            else throw new ApiException(string.Format("Unknown Category Value: {0}", category));
        }
    }
}