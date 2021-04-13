using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPayments.Api.Mapping {
    public class GpApiMapping {
        public static Transaction MapResponse(string rawResponse) {
            Transaction transaction = new Transaction();

            if (!string.IsNullOrEmpty(rawResponse)) {
                JsonDoc json = JsonDoc.Parse(rawResponse);

                transaction.TransactionId = json.GetValue<string>("id");
                transaction.BalanceAmount = json.GetValue<string>("amount").ToAmount();
                transaction.Timestamp = json.GetValue<string>("time_created");
                transaction.ResponseMessage = json.GetValue<string>("status");
                transaction.ReferenceNumber = json.GetValue<string>("reference");
                transaction.ClientTransactionId = json.GetValue<string>("reference");
                transaction.BatchSummary = new BatchSummary {
                    SequenceNumber = json.GetValue<string>("batch_id")
                };
                transaction.ResponseCode = json.Get("action").GetValue<string>("result_code");
                string id = json.GetValue<string>("id");
                if ((id ?? string.Empty).StartsWith("PMT_")) {
                    transaction.Token = id;
                }
                if (json.Has("payment_method")) {
                    JsonDoc paymentMethod = json.Get("payment_method");
                    transaction.Token = paymentMethod.GetValue<string>("id");
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
                TransactionId = doc.GetValue<string>("id"),
                TransactionDate = doc.GetValue<DateTime>("time_created"),
                TransactionStatus = doc.GetValue<string>("status"),
                TransactionType = doc.GetValue<string>("type"),
                Channel = doc.GetValue<string>("channel"),
                Amount = doc.GetValue<string>("amount").ToAmount(),
                Currency = doc.GetValue<string>("currency"),
                ReferenceNumber = doc.GetValue<string>("reference"),
                ClientTransactionId = doc.GetValue<string>("reference"),
                TransactionLocalDate = doc.GetValue<DateTime?>("time_created_reference", DateConverter),
                BatchSequenceNumber = doc.GetValue<string>("batch_id"),
                Country = doc.GetValue<string>("country"),
                OriginalTransactionId = doc.GetValue<string>("parent_resource_id"),
                DepositReference = doc.GetValue<string>("deposit_id"),
                DepositDate = doc.GetValue<DateTime?>("deposit_time_created", DateConverter),

                GatewayResponseMessage = paymentMethod?.GetValue<string>("message"),
                EntryMode = paymentMethod?.GetValue<string>("entry_mode"),
                CardHolderName = paymentMethod?.GetValue<string>("name"),

                CardType = card?.GetValue<string>("brand"),
                AuthCode = card?.GetValue<string>("authcode"),
                BrandReference = card?.GetValue<string>("brand_reference"),
                AquirerReferenceNumber = card?.GetValue<string>("arn"),
                MaskedCardNumber = card?.GetValue<string>("masked_number_first6last4"),

                MerchantId = doc.Get("system")?.GetValue<string>("mid"),
                MerchantHierarchy = doc.Get("system")?.GetValue<string>("hierarchy"),
                MerchantName = doc.Get("system")?.GetValue<string>("name"),
                MerchantDbaName = doc.Get("system")?.GetValue<string>("dba"),
            };

            return summary;
        }

        private static DateTime? DateConverter(object value) {
            if (value != null && !string.IsNullOrEmpty(value.ToString())) {
                return DateTime.Parse(value.ToString());
            }
            return null;
        }

        public static T MapReportResponse<T>(string rawResponse, ReportType reportType) where T : class {
            T result = Activator.CreateInstance<T>();

            JsonDoc json = JsonDoc.Parse(rawResponse);

            if (reportType == ReportType.TransactionDetail && result is TransactionSummary) {
                result = MapTransactionSummary(json) as T;
            }
            else if ((reportType == ReportType.FindTransactionsPaged || reportType == ReportType.FindSettlementTransactionsPaged) && result is PagedResult<TransactionSummary>) {
                SetPagingInfo(result as PagedResult<TransactionSummary>, json);
                foreach (var doc in json.GetArray<JsonDoc>("transactions") ?? Enumerable.Empty<JsonDoc>()) {
                    (result as PagedResult<TransactionSummary>).Add(MapTransactionSummary(doc));
                }
            }
            else if (reportType == ReportType.DepositDetail && result is DepositSummary) {
                result = MapDepositSummary(json) as T;
            }
            else if (reportType == ReportType.FindDepositsPaged && result is PagedResult<DepositSummary>) {
                SetPagingInfo(result as PagedResult<DepositSummary>, json);
                foreach (var doc in json.GetArray<JsonDoc>("deposits") ?? Enumerable.Empty<JsonDoc>()) {
                    (result as PagedResult<DepositSummary>).Add(MapDepositSummary(doc));
                }
            }
            else if (reportType == ReportType.DisputeDetail && result is DisputeSummary) {
                result = MapDisputeSummary(json) as T;
            }
            else if (reportType == ReportType.FindDisputesPaged && result is PagedResult<DisputeSummary>) {
                SetPagingInfo(result as PagedResult<DisputeSummary>, json);
                foreach (var doc in json.GetArray<JsonDoc>("disputes") ?? Enumerable.Empty<JsonDoc>()) {
                    (result as PagedResult<DisputeSummary>).Add(MapDisputeSummary(doc));
                }
            }
            else if (reportType == ReportType.SettlementDisputeDetail && result is DisputeSummary) {
                result = MapSettlementDisputeSummary(json) as T;
            }
            else if (reportType == ReportType.FindSettlementDisputesPaged && result is PagedResult<DisputeSummary>) {
                SetPagingInfo(result as PagedResult<DisputeSummary>, json);
                foreach (var doc in json.GetArray<JsonDoc>("disputes") ?? Enumerable.Empty<JsonDoc>()) {
                    (result as PagedResult<DisputeSummary>).Add(MapSettlementDisputeSummary(doc));
                }
            }

            return result;
        }

        private static void SetPagingInfo<T>(PagedResult<T> result, JsonDoc json) where T : class {
            result.TotalRecordCount = json.GetValue<int>("total_record_count", "total_count");
            result.PageSize = json.Get("paging")?.GetValue<int>("page_size") ?? default(int);
            result.Page = json.Get("paging")?.GetValue<int>("page") ?? default(int);
            result.Order = json.Get("paging")?.GetValue<string>("order");
            result.OrderBy = json.Get("paging")?.GetValue<string>("order_by");
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
                CaseIdTime = doc.GetValue<DateTime?>("time_created"),
                CaseStatus = doc.GetValue<string>("status"),
                CaseStage = doc.GetValue<string>("stage"),
                CaseAmount = doc.GetValue<string>("amount").ToAmount(),
                CaseCurrency = doc.GetValue<string>("currency"),
                ReasonCode = doc.GetValue<string>("reason_code"),
                Reason = doc.GetValue<string>("reason_description"),
                Result = doc.GetValue<string>("result"),
                CaseMerchantId = doc.Get("system")?.GetValue<string>("mid"),
                CaseTerminalId = doc.Get("system")?.GetValue<string>("tid"),
                MerchantHierarchy = doc.Get("system")?.GetValue<string>("hierarchy"),
                MerchantName = doc.Get("system")?.GetValue<string>("name"),
                MerchantDbaName = doc.Get("system")?.GetValue<string>("dba"),
                LastAdjustmentAmount = doc.GetValue<string>("last_adjustment_amount").ToAmount(),
                LastAdjustmentCurrency = doc.GetValue<string>("last_adjustment_currency"),
                LastAdjustmentFunding = doc.GetValue<string>("last_adjustment_funding"),
                TransactionMaskedCardNumber = doc.Get("payment_method")?.Get("card")?.GetValue<string>("number"),
                TransactionARN = doc.Get("payment_method")?.Get("card")?.GetValue<string>("arn"),
                TransactionCardType = doc.Get("payment_method")?.Get("card")?.GetValue<string>("brand"),
            };

            if (!string.IsNullOrEmpty(doc.GetValue<string>("time_to_respond_by"))) {
                summary.RespondByDate = doc.GetValue<DateTime?>("time_to_respond_by");
            }

            return summary;
        }

        public static DisputeSummary MapSettlementDisputeSummary(JsonDoc doc) {
            var summary = MapDisputeSummary(doc);

            summary.CaseIdTime = doc.GetValue<DateTime?>("stage_time_created");
            summary.TransactionTime = doc.Get("transaction")?.GetValue<DateTime?>("time_created");
            summary.TransactionType = doc.Get("transaction")?.GetValue<string>("type");
            summary.TransactionAmount = doc.Get("transaction")?.GetValue<string>("amount").ToAmount();
            summary.TransactionCurrency = doc.Get("transaction")?.GetValue<string>("currency");
            summary.TransactionReferenceNumber = doc.Get("transaction")?.GetValue<string>("reference");
            summary.TransactionMaskedCardNumber = doc.Get("transaction")?.Get("payment_method")?.Get("card")?.GetValue<string>("masked_number_first6last4");
            summary.TransactionARN = doc.Get("transaction")?.Get("payment_method")?.Get("card")?.GetValue<string>("arn");
            summary.TransactionCardType = doc.Get("transaction")?.Get("payment_method")?.Get("card")?.GetValue<string>("brand");
            summary.TransactionAuthCode = doc.Get("transaction")?.Get("payment_method")?.Get("card")?.GetValue<string>("authcode");
            
            return summary;
        }


        private static Secure3dVersion Parse3DSVersion(string messageVersion) {
            if (messageVersion.StartsWith("1."))
                return Secure3dVersion.One;
            if (messageVersion.StartsWith("2."))
                return Secure3dVersion.Two;
            return Secure3dVersion.Any;
        }

        public static Transaction Map3DSecureData(string rawResponse) {
            if (!string.IsNullOrEmpty(rawResponse)) {
                JsonDoc json = JsonDoc.Parse(rawResponse);

                return new Transaction {
                    ThreeDSecure = new ThreeDSecure {
                        ServerTransactionId = json.GetValue<string>("id"),
                        Status = json.GetValue<string>("status"),
                        Currency = json.GetValue<string>("currency"),
                        Amount = json.GetValue<string>("amount").ToAmount(),

                        Version = Parse3DSVersion(json.Get("three_ds")?.GetValue<string>("message_version")),
                        MessageVersion = json.Get("three_ds")?.GetValue<string>("message_version"),
                        DirectoryServerStartVersion = json.Get("three_ds")?.GetValue<string>("ds_protocol_version_start"),
                        DirectoryServerEndVersion = json.Get("three_ds")?.GetValue<string>("ds_protocol_version_end"),
                        DirectoryServerTransactionId = json.Get("three_ds")?.GetValue<string>("ds_trans_ref"),
                        AcsStartVersion = json.Get("three_ds")?.GetValue<string>("acs_protocol_version_start"),
                        AcsEndVersion = json.Get("three_ds")?.GetValue<string>("acs_protocol_version_end"),
                        AcsTransactionId = json.Get("three_ds")?.GetValue<string>("acs_trans_ref"),
                        Enrolled = json.Get("three_ds")?.GetValue<string>("enrolled_status"),
                        Eci = json.Get("three_ds")?.GetValue<string>("eci")?.ToInt32(),
                        AuthenticationValue = json.Get("three_ds")?.GetValue<string>("authentication_value"),
                        ChallengeMandated = json.Get("three_ds")?.GetValue<string>("challenge_status") == "MANDATED",
                        IssuerAcsUrl = !string.IsNullOrEmpty(json.Get("three_ds")?.GetValue<string>("method_url")) ?
                            json.Get("three_ds")?.GetValue<string>("method_url") :
                            json.Get("three_ds")?.GetValue<string>("acs_challenge_request_url"),
                        ChallengeReturnUrl = json.Get("notifications")?.GetValue<string>("challenge_return_url"),
                        SessionDataFieldName = json.Get("three_ds")?.GetValue<string>("session_data_field_name"),
                        MessageType = json.Get("three_ds")?.GetValue<string>("message_type"),
                        PayerAuthenticationRequest = json.Get("three_ds")?.GetValue<string>("challenge_value"),
                        StatusReason = json.Get("three_ds")?.GetValue<string>("status_reason"),
                        MessageCategory = json.Get("three_ds")?.GetValue<string>("message_category"),
                    }
                };
            }
            return new Transaction();
        }
    }
}
