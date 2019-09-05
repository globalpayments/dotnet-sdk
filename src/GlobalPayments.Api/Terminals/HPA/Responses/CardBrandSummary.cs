
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;
using System;

namespace GlobalPayments.Api.Terminals.HPA.Responses {
    public class CardBrandSummary : SipBaseResponse, ICardBrandSummary {
        public int CreditCount { get; private set; }
        public decimal? CreditAmount { get; private set; }
        public int DebitCount { get; private set; }
        public decimal? DebitAmount { get; private set; }
        public int SaleCount { get; private set; }
        public decimal? SaleAmount { get; private set; }
        public int RefundCount { get; private set; }
        public decimal? RefundAmount { get; private set; }
        public int TotalCount { get; private set; }
        public decimal? TotalAmount { get; private set; }
        public CardType CardType { get; set; }
      
        public CardBrandSummary(byte[] buffer, params string[] messageIds) : base(buffer, messageIds) { }

        internal override void MapResponse(Element response) {
            base.MapResponse(response);

            Element[] fields = response.GetAll("Field");
            for (int i = 0; i < fields.Length; i++) {
                Element field = fields[i];

                string key = field.GetValue<string>("Key");
                string value = field.GetValue<string>("Value");

                if (key.Equals("CardType", StringComparison.OrdinalIgnoreCase)) {
                    CardType = mapCardType(value);
                }
                else if (key.Equals("TransType", StringComparison.OrdinalIgnoreCase)) {
                    int count = fields[++i].GetValue<int>("Value");
                    decimal? amount = (fields[++i].GetValue<string>("Value")).ToAmount();

                    if (value.Equals("CREDIT", StringComparison.OrdinalIgnoreCase)) {
                        CreditCount = count;
                        CreditAmount = amount;
                    }
                    else if (value.Equals("DEBIT", StringComparison.OrdinalIgnoreCase)) { 
                        DebitCount = count;
                        DebitAmount = amount;
                    }
                    else if (value.Equals("SALE", StringComparison.OrdinalIgnoreCase)) {
                        SaleCount = count;
                        SaleAmount = amount;
                    }
                    else if (value.Equals("REFUND", StringComparison.OrdinalIgnoreCase)) {
                        RefundCount = count;
                        RefundAmount = amount;
                    }
                    else if (value.Equals("TOTAL", StringComparison.OrdinalIgnoreCase)) {
                        TotalCount = count;
                        TotalAmount = amount;
                    }
                }
            }
        }

        private CardType mapCardType(string value) {
            if (value.Equals("VISA", StringComparison.OrdinalIgnoreCase)) {
                return CardType.VISA;
            }
            else if (value.Equals("MASTERCARD", StringComparison.OrdinalIgnoreCase)) {
                return CardType.MC;
            }
            else if (value.Equals("AMERICAN EXPRESS", StringComparison.OrdinalIgnoreCase)) {
                return CardType.AMEX;
            }
            else if (value.Equals("DISCOVER", StringComparison.OrdinalIgnoreCase)) {
                return CardType.DISC;
            }
            else {
                return CardType.PAYPALECOMMERCE;
            }
        }
    }
}
