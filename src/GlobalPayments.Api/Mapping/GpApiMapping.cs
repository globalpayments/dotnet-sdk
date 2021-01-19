using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Mapping {
    public class GpApiMapping {
        public static Transaction MapResponse(string rawResponse) {
            Transaction transaction = new Transaction();

            if (!string.IsNullOrEmpty(rawResponse)) {
                JsonDoc json = JsonDoc.Parse(rawResponse);

                // ToDo: Map transaction values
                transaction.TransactionId = json.GetValue<string>("id");
                transaction.BalanceAmount = json.GetValue<string>("amount").ToAmount();
                transaction.Timestamp = json.GetValue<string>("time_created");
                transaction.ResponseMessage = json.GetValue<string>("status");
                transaction.ReferenceNumber = json.GetValue<string>("reference");
                transaction.BatchSummary = new BatchSummary {
                    SequenceNumber = json.GetValue<string>("batch_id")
                };
                transaction.ResponseCode = json.Get("action").GetValue<string>("result_code");
                transaction.Token = json.GetValue<string>("id");

                if (json.Has("payment_method")) {
                    JsonDoc paymentMethod = json.Get("payment_method");
                    transaction.AuthorizationCode = paymentMethod.GetValue<string>("result");
                    if (paymentMethod.Has("card")) {
                        JsonDoc card = paymentMethod.Get("card");
                        transaction.CardType = card.GetValue<string>("brand");
                        transaction.CardLast4 = card.GetValue<string>("masked_number_last4");
                        transaction.CvnResponseMessage = card.GetValue<string>("cvv_result");
                    }
                }
                if (json.Has("card")) {
                    JsonDoc card = json.Get("card");
                    transaction.CardNumber = card.GetValue<string>("number");
                    transaction.CardType = card.GetValue<string>("brand");
                    transaction.CardExpMonth = card.GetValue<int>("expiry_month");
                    transaction.CardExpYear = card.GetValue<int>("expiry_year");
                }
            }

            return transaction;
        }

        public static TransactionSummary MapTransactionSummary(JsonDoc doc) {
            JsonDoc paymentMethod = doc.Get("payment_method");

            JsonDoc card = paymentMethod?.Get("card");

            var summary = new TransactionSummary {
                //ToDo: Map all transaction properties
                TransactionId = doc.GetValue<string>("id"),
                TransactionDate = doc.GetValue<DateTime>("time_created"),
                TransactionStatus = doc.GetValue<string>("status"),
                TransactionType = doc.GetValue<string>("type"),
                Channel = doc.GetValue<string>("channel"),
                Amount = doc.GetValue<string>("amount").ToAmount(),
                Currency = doc.GetValue<string>("currency"),
                ReferenceNumber = doc.GetValue<string>("reference"),
                ClientTransactionId = doc.GetValue<string>("reference"),
                // ?? = doc.GetValue<DateTime>("time_created_reference"),
                BatchSequenceNumber = doc.GetValue<string>("batch_id"),
                Country = doc.GetValue<string>("country"),
                // ?? = doc.GetValue<string>("action_create_id"),
                OriginalTransactionId = doc.GetValue<string>("parent_resource_id"),

                GatewayResponseMessage = paymentMethod?.GetValue<string>("message"),
                EntryMode = paymentMethod?.GetValue<string>("entry_mode"),
                CardHolderName = paymentMethod?.GetValue<string>("name"),

                CardType = card?.GetValue<string>("brand"),
                AuthCode = card?.GetValue<string>("authcode"),
                BrandReference = card?.GetValue<string>("brand_reference"),
                AquirerReferenceNumber = card?.GetValue<string>("arn"),
                MaskedCardNumber = card?.GetValue<string>("masked_number_first6last4"),
            };

            return summary;
        }

        public static DepositSummary MapDepositSummary(JsonDoc doc) {
            var summary = new DepositSummary {
                DepositId = doc.GetValue<string>("id"),
                DepositDate = doc.GetValue<DateTime>("time_created"),
                Status = doc.GetValue<string>("status"),
                Type = doc.GetValue<string>("funding_type"),
                Amount = doc.GetValue<string>("amount").ToAmount(),
                Currency = doc.GetValue<string>("currency"),

                MerchantNumber = doc.Get("system")?.GetValue<string>("mid"),
                MerchantHierarchy = doc.Get("system")?.GetValue<string>("hierarchy"),
                MerchantName = doc.Get("system")?.GetValue<string>("name"),
                MerchantDbaName = doc.Get("system")?.GetValue<string>("dba"),

                SalesTotalCount = doc.Get("sales")?.GetValue<int>("count") ?? default(int),
                SalesTotalAmount = doc.Get("sales")?.GetValue<string>("amount").ToAmount(),

                RefundsTotalCount = doc.Get("refunds")?.GetValue<int>("count") ?? default(int),
                RefundsTotalAmount = doc.Get("refunds")?.GetValue<string>("amount").ToAmount(),

                ChargebackTotalCount = doc.Get("disputes")?.Get("chargebacks")?.GetValue<int>("count") ?? default(int),
                ChargebackTotalAmount = doc.Get("disputes")?.Get("chargebacks")?.GetValue<string>("amount").ToAmount(),

                AdjustmentTotalCount = doc.Get("disputes")?.Get("reversals")?.GetValue<int>("count") ?? default(int),
                AdjustmentTotalAmount = doc.Get("disputes")?.Get("reversals")?.GetValue<string>("amount").ToAmount(),

                FeesTotalAmount = doc.Get("fees")?.GetValue<string>("amount").ToAmount(),
            };

            return summary;
        }

        public static DisputeSummary MapDisputeSummary(JsonDoc doc) {
            var summary = new DisputeSummary {
                CaseId = doc.GetValue<string>("id"),
                CaseIdTime = doc.GetValue<DateTime>("time_created"),
                CaseStatus = doc.GetValue<string>("status"),
                CaseStage = doc.GetValue<string>("stage"),
                CaseAmount = doc.GetValue<string>("amount").ToAmount(),
                CaseCurrency = doc.GetValue<string>("currency"),
                CaseMerchantId = doc.Get("system")?.GetValue<string>("mid"),
                MerchantHierarchy = doc.Get("system")?.GetValue<string>("hierarchy"),
                TransactionMaskedCardNumber = doc.Get("payment_method")?.Get("card")?.GetValue<string>("number"),
                TransactionARN = doc.Get("payment_method")?.Get("card")?.GetValue<string>("arn"),
                TransactionCardType = doc.Get("payment_method")?.Get("card")?.GetValue<string>("brand"),
                ReasonCode = doc.GetValue<string>("reason_code"),
                Reason = doc.GetValue<string>("reason_description"),
                RespondByDate = doc.GetValue<DateTime>("time_to_respond_by"),
                Result = doc.GetValue<string>("result"),
                LastAdjustmentAmount = doc.GetValue<string>("last_adjustment_amount").ToAmount(),
                LastAdjustmentCurrency = doc.GetValue<string>("last_adjustment_currency"),
                LastAdjustmentFunding = doc.GetValue<string>("last_adjustment_funding")
            };

            return summary;
        }
    }
}
