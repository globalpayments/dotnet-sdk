using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPayments.Api.Mapping {
    public class GpApiMapping {
        private const string BATCH_CLOSE = "CLOSE";
        private const string PAYMENT_METHOD_CREATE = "PAYMENT_METHOD_CREATE";
        private const string PAYMENT_METHOD_DETOKENIZE = "PAYMENT_METHOD_DETOKENIZE";
        private const string PAYMENT_METHOD_EDIT = "PAYMENT_METHOD_EDIT";
        private const string PAYMENT_METHOD_DELETE = "PAYMENT_METHOD_DELETE";


        public static Transaction MapResponse(string rawResponse) {
            Transaction transaction = new Transaction();

            if (!string.IsNullOrEmpty(rawResponse)) {
                JsonDoc json = JsonDoc.Parse(rawResponse);

                transaction.ResponseCode = json.Get("action")?.GetValue<string>("result_code");

                string actionType = json.Get("action")?.GetValue<string>("type");

                switch (actionType) {
                    case BATCH_CLOSE:
                        transaction.BatchSummary = new BatchSummary {
                            BatchReference = json.GetValue<string>("id"),
                            Status = json.GetValue<string>("status"),
                            TotalAmount = json.GetValue<string>("amount").ToAmount(),
                            TransactionCount = json.GetValue<int>("transaction_count"),
                        };
                        return transaction;
                    case PAYMENT_METHOD_CREATE:
                    case PAYMENT_METHOD_DETOKENIZE:
                    case PAYMENT_METHOD_EDIT:
                    case PAYMENT_METHOD_DELETE:
                        transaction.Token = json.GetValue<string>("id");
                        transaction.Timestamp = json.GetValue<string>("time_created");
                        transaction.ReferenceNumber = json.GetValue<string>("reference");
                        transaction.CardType = json.Get("card")?.GetValue<string>("brand");
                        transaction.CardNumber = json.Get("card")?.GetValue<string>("number");
                        transaction.CardLast4 = json.Get("card")?.GetValue<string>("masked_number_last4");
                        transaction.CardExpMonth = json.Get("card")?.GetValue<int>("expiry_month");
                        transaction.CardExpYear = json.Get("card")?.GetValue<int>("expiry_year");
                        return transaction;
                    default:
                        break;
                }

                transaction.TransactionId = json.GetValue<string>("id");
                transaction.BalanceAmount = json.GetValue<string>("amount").ToAmount();
                transaction.Timestamp = json.GetValue<string>("time_created");
                transaction.ResponseMessage = json.GetValue<string>("status");
                transaction.ReferenceNumber = json.GetValue<string>("reference");
                transaction.ClientTransactionId = json.GetValue<string>("reference");
                transaction.BatchSummary = new BatchSummary {
                    BatchReference = json.GetValue<string>("batch_id")
                };
                transaction.Token = json.Get("payment_method")?.GetValue<string>("id");
                transaction.AuthorizationCode = json.Get("payment_method")?.GetValue<string>("result");
                transaction.CardType = json.Get("payment_method")?.Get("card")?.GetValue<string>("brand");
                transaction.CardLast4 = json.Get("payment_method")?.Get("card")?.GetValue<string>("masked_number_last4");
                transaction.CvnResponseMessage = json.Get("payment_method")?.Get("card")?.GetValue<string>("cvv_result");
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
            else if (reportType == ReportType.StoredPaymentMethodDetail && result is StoredPaymentMethodSummary) {
                result = MapStoredPaymentMethodSummary(json) as T;
            }
            else if (reportType == ReportType.FindStoredPaymentMethodsPaged && result is PagedResult<StoredPaymentMethodSummary>) {
                SetPagingInfo(result as PagedResult<StoredPaymentMethodSummary>, json);
                foreach (var doc in json.GetArray<JsonDoc>("payment_methods") ?? Enumerable.Empty<JsonDoc>()) {
                    (result as PagedResult<StoredPaymentMethodSummary>).Add(MapStoredPaymentMethodSummary(doc));
                }
            }
            else if (reportType == ReportType.ActionDetail && result is ActionSummary) {
                result = MapActionSummary(json) as T;
            }
            else if (reportType == ReportType.FindActionsPaged && result is PagedResult<ActionSummary>) {
                SetPagingInfo(result as PagedResult<ActionSummary>, json);
                foreach (var doc in json.GetArray<JsonDoc>("actions") ?? Enumerable.Empty<JsonDoc>()) {
                    (result as PagedResult<ActionSummary>).Add(MapActionSummary(doc));
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

        public static StoredPaymentMethodSummary MapStoredPaymentMethodSummary(JsonDoc doc) {
            return new StoredPaymentMethodSummary {
                Id = doc.GetValue<string>("id"),
                TimeCreated = doc.GetValue<DateTime>("time_created"),
                Status = doc.GetValue<string>("status"),
                Reference = doc.GetValue<string>("reference"),
                Name = doc.GetValue<string>("name"),
                CardLast4 = doc.Get("card")?.GetValue<string>("number_last4"),
                CardType = doc.Get("card")?.GetValue<string>("brand"),
                CardExpMonth = doc.Get("card")?.GetValue<string>("expiry_month"),
                CardExpYear = doc.Get("card")?.GetValue<string>("expiry_year"),
            };
        }

        public static ActionSummary MapActionSummary(JsonDoc doc) {
            return new ActionSummary {
                Id = doc.GetValue<string>("id"),
                Type = doc.GetValue<string>("type"),
                TimeCreated = doc.GetValue<DateTime>("time_created"),
                Resource = doc.GetValue<string>("resource"),
                Version = doc.GetValue<string>("version"),
                ResourceId = doc.GetValue<string>("resource_id"),
                ResourceStatus = doc.GetValue<string>("resource_status"),
                HttpResponseCode = doc.GetValue<string>("http_response_code"),
                ResponseCode = doc.GetValue<string>("response_code"),
                AppId = doc.GetValue<string>("app_id"),
                AppName = doc.GetValue<string>("app_name"),
                AccountId = doc.GetValue<string>("account_id"),
                AccountName = doc.GetValue<string>("account_name"),
                MerchantName = doc.GetValue<string>("merchant_name"),
            };
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
