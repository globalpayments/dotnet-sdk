using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.Reporting;
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
        private const string LINK_CREATE = "LINK_CREATE";
        private const string LINK_EDIT = "LINK_EDIT";


        public static Transaction MapResponse(string rawResponse) {
            Transaction transaction = new Transaction();

            if (!string.IsNullOrEmpty(rawResponse)) {
                JsonDoc json = JsonDoc.Parse(rawResponse);

                transaction.ResponseCode = json.Get("action")?.GetValue<string>("result_code");
                transaction.ResponseMessage = json.GetValue<string>("status");

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
                        transaction.TokenUsageMode = GetPaymentMethodUsageMode(json);
                        transaction.Timestamp = json.GetValue<string>("time_created");
                        transaction.ReferenceNumber = json.GetValue<string>("reference");
                        transaction.CardType = json.Get("card")?.GetValue<string>("brand");
                        transaction.CardNumber = json.Get("card")?.GetValue<string>("number");
                        transaction.CardLast4 = json.Get("card")?.GetValue<string>("masked_number_last4");
                        transaction.CardExpMonth = json.Get("card")?.GetValue<int>("expiry_month");
                        transaction.CardExpYear = json.Get("card")?.GetValue<int>("expiry_year");
                        return transaction;
                    case LINK_CREATE:
                    case LINK_EDIT:
                        transaction.PayLinkResponse = MapPayLinkResponse(json);
                        if (json.Has("transactions")) {
                            var trn = json.Get("transactions");
                            transaction.BalanceAmount = trn.GetValue<string>("amount").ToAmount();
                            transaction.PayLinkResponse.AllowedPaymentMethods = GetAllowedPaymentMethods(trn);
                        }
                        return transaction;
                    default:
                        break;
                }

                transaction.TransactionId = json.GetValue<string>("id");
                transaction.BalanceAmount = json.GetValue<string>("amount").ToAmount();
                transaction.AuthorizedAmount = (json.GetValue<string>("status").ToUpper().Equals(TransactionStatus.Preauthorized.ToString().ToUpper()) && !string.IsNullOrEmpty(json.GetValue<string>("amount"))) ?
                json.GetValue<string>("amount").ToAmount() : null;
                transaction.Timestamp = json.GetValue<string>("time_created");
                transaction.ResponseMessage = json.GetValue<string>("status");
                transaction.ReferenceNumber = json.GetValue<string>("reference");
                transaction.ClientTransactionId = json.GetValue<string>("reference");
                transaction.FingerPrint = json.Get("payment_method")?.GetValue<string>("fingerprint") ?? null;
                transaction.FingerPrintIndicator = json.Get("payment_method")?.GetValue<string>("fingerprint_presence_indicator") ?? null;
                transaction.BatchSummary = new BatchSummary {
                    BatchReference = json.GetValue<string>("batch_id")
                };
                transaction.Token = json.Get("payment_method")?.GetValue<string>("id");                
                transaction.AuthorizationCode = json.Get("payment_method")?.GetValue<string>("result");
                transaction.CardType = json.Get("payment_method")?.Get("card")?.GetValue<string>("brand");
                transaction.CardLast4 = json.Get("payment_method")?.Get("card")?.GetValue<string>("masked_number_last4");
                transaction.CvnResponseMessage = json.Get("payment_method")?.Get("card")?.GetValue<string>("cvv_result");
                transaction.CardBrandTransactionId = json.Get("payment_method")?.Get("card")?.GetValue<string>("brand_reference");
                transaction.AvsResponseCode = json.Get("payment_method")?.Get("card")?.GetValue<string>("avs_postal_code_result");
                transaction.AvsAddressResponse = json.Get("payment_method")?.Get("card")?.GetValue<string>("avs_address_result");
                transaction.AvsResponseMessage = json.Get("payment_method")?.Get("card")?.GetValue<string>("avs_action");                
                if (json.Get("payment_method")?.Get("card")?.Has("provider") ?? false) {
                    transaction.CardIssuerResponse = MapCardIssuerResponse(json.Get("payment_method")?.Get("card")?.Get("provider"));
                }
                transaction.MultiCapture = GetIsMultiCapture(json);
                transaction.PaymentMethodType = GetPaymentMehodType(json) ?? transaction.PaymentMethodType;
                transaction.DccRateData = MapDccInfo(json);
                transaction.FraudFilterResponse = json.Has("risk_assessment") ? MapFraudManagement(json) : null;
            }

            return transaction;
        }

        private static bool GetIsMultiCapture(JsonDoc json)
        {
            if (!string.IsNullOrEmpty(json.GetValue<string>("capture_mode"))) {
                switch (json.GetValue<string>("capture_mode")) {
                    case "MULTIPLE":
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }

        private static PaymentMethodType? GetPaymentMehodType(JsonDoc json)
        {
            if (json.Get("payment_method")?.Has("bank_transfer") ?? false) {
                return PaymentMethodType.ACH;
            }
            else if (json.Get("payment_method")?.Has("apm") ?? false) {
                return PaymentMethodType.APM;
            }
            return null;
        }

        private static PaymentMethodUsageMode? GetPaymentMethodUsageMode(JsonDoc json) 
        {
            if (json.Has("usage_mode")) {
                if (json.GetValue<string>("usage_mode").Equals(EnumConverter.GetMapping(Target.GP_API, PaymentMethodUsageMode.Single))) {
                    return PaymentMethodUsageMode.Single;
                }
                else if (json.GetValue<string>("usage_mode").Equals(EnumConverter.GetMapping(Target.GP_API, PaymentMethodUsageMode.Multiple))) {
                    return PaymentMethodUsageMode.Multiple;
                }
            }
            return null;
        }


        /// <summary>
        /// Map response for an APM transaction
        /// </summary>
        /// <param name="response"></param>
        /// <returns>Transaction</returns>
        public static Transaction MapResponseAPM(string rawResponse)
        {
            var apm = new AlternativePaymentResponse();
            var transaction = MapResponse(rawResponse);

            JsonDoc json = JsonDoc.Parse(rawResponse);

            var paymentMethodApm = json.Get("payment_method")?.Get("apm");
            apm.RedirectUrl = json.Get("payment_method")?.GetValue<string>("redirect_url");
            apm.ProviderName = paymentMethodApm?.GetValue<string>("provider");
            apm.Ack = paymentMethodApm?.GetValue<string>("ack");
            apm.SessionToken = paymentMethodApm?.GetValue<string>("session_token");
            apm.CorrelationReference = paymentMethodApm?.GetValue<string>("correlation_reference");
            apm.VersionReference = paymentMethodApm?.GetValue<string>("version_reference");
            apm.BuildReference = paymentMethodApm?.GetValue<string>("build_reference");
            apm.TimeCreatedReference = paymentMethodApm?.GetValue<DateTime?>("time_created_reference", DateConverter);
            apm.TransactionReference = paymentMethodApm?.GetValue<string>("transaction_reference");
            apm.SecureAccountReference = paymentMethodApm?.GetValue<string>("secure_account_reference");
            apm.ReasonCode = paymentMethodApm?.GetValue<string>("reason_code");
            apm.PendingReason = paymentMethodApm?.GetValue<string>("pending_reason");
            apm.GrossAmount = paymentMethodApm?.GetValue<string>("gross_amount")?.ToAmount();
            apm.PaymentTimeReference = paymentMethodApm?.GetValue<DateTime?>("payment_time_reference", DateConverter);
            apm.PaymentType = paymentMethodApm?.GetValue<string>("payment_type");
            apm.PaymentStatus = paymentMethodApm?.GetValue<string>("payment_status");
            apm.Type = paymentMethodApm?.GetValue<string>("type");
            apm.ProtectionEligibility = paymentMethodApm?.GetValue<string>("protection_eligibilty");
            apm.FeeAmount = paymentMethodApm?.GetValue<string>("fee_amount")?.ToAmount();

            var authorization = json.Get("payment_method")?.Get("authorization");
            apm.AuthStatus = authorization?.GetValue<string>("status");
            apm.AuthAmount = authorization?.GetValue<string>("amount")?.ToAmount();
            apm.AuthAck = authorization?.GetValue<string>("ack");
            apm.AuthCorrelationReference = authorization?.GetValue<string>("correlation_reference");
            apm.AuthVersionReference = authorization?.GetValue<string>("version_reference");
            apm.AuthBuildReference = authorization?.GetValue<string>("build_reference");
            apm.AuthPendingReason = authorization?.GetValue<string>("pending_reason");
            apm.AuthProtectionEligibility = authorization?.GetValue<string>("protection_eligibilty");
            apm.AuthProtectionEligibilityType = authorization?.GetValue<string>("protection_eligibilty_type");
            apm.AuthReference = authorization?.GetValue<string>("reference");

            transaction.AlternativePaymentResponse = apm;

            return transaction;
        }

        public static PayLinkResponse MapPayLinkResponse(JsonDoc doc) 
        {
            var payLinkResponse = new PayLinkResponse();
            payLinkResponse.Id = doc.GetValue<string>("id");
            payLinkResponse.AccountName = doc.GetValue<string>("account_name");
            payLinkResponse.Url = doc.GetValue<string>("url");
            payLinkResponse.Status = GetPayLinkStatus(doc);
            payLinkResponse.Type = GetPayLinkType(doc);
            payLinkResponse.UsageMode = GetPaymentMethodUsageMode(doc);
            payLinkResponse.UsageLimit = doc.GetValue<int>("usage_limit");
            payLinkResponse.Reference = doc.GetValue<string>("reference");
            payLinkResponse.Name = doc.GetValue<string>("name");
            payLinkResponse.Description = doc.GetValue<string>("description");            
            payLinkResponse.ViewedCount = doc.GetValue<string>("viewed_count");
            payLinkResponse.ExpirationDate = doc.GetValue<DateTime?>("expiration_date", DateConverter);
            payLinkResponse.IsShippable = GetIsShippable(doc);

            return payLinkResponse;
        }

        private static bool? GetIsShippable(JsonDoc doc) 
        {
            if (doc.Has("shippable")) {
                return doc.GetValue<string>("shippable").ToUpper() == "YES" ? true : false;
            }
            return null;
        }

        public static TransactionSummary MapTransactionSummary(JsonDoc doc) 
        {
            var summary = new TransactionSummary {
                TransactionId = doc.GetValue<string>("id"),
                TransactionDate = doc.GetValue<DateTime?>("time_created", DateConverter),
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
                BatchCloseDate = doc.GetValue<DateTime?>("batch_time_created", DateConverter),
                DepositDate = doc.GetValue<DateTime?>("deposit_time_created", DateConverter),

                MerchantId = doc.Get("system")?.GetValue<string>("mid"),
                MerchantHierarchy = doc.Get("system")?.GetValue<string>("hierarchy"),
                MerchantName = doc.Get("system")?.GetValue<string>("name"),
                MerchantDbaName = doc.Get("system")?.GetValue<string>("dba"),
            };

            if (doc.Has("payment_method"))
            {
                JsonDoc paymentMethod = doc.Get("payment_method");
                summary.GatewayResponseMessage = paymentMethod?.GetValue<string>("message");
                summary.EntryMode = paymentMethod?.GetValue<string>("entry_mode");
                summary.CardHolderName = paymentMethod?.GetValue<string>("name");

                if (paymentMethod.Has("card"))
                {
                    JsonDoc card = paymentMethod?.Get("card");
                    summary.CardType = card?.GetValue<string>("brand");
                    summary.AuthCode = card?.GetValue<string>("authcode");
                    summary.BrandReference = card?.GetValue<string>("brand_reference");
                    summary.AquirerReferenceNumber = card?.GetValue<string>("arn");
                    summary.MaskedCardNumber = card?.GetValue<string>("masked_number_first6last4");
                    summary.PaymentType = EnumConverter.GetMapping(Target.GP_API, PaymentMethodName.Card);
                }
                else if(paymentMethod.Has("digital_wallet")) 
                {
                    JsonDoc digitalWallet = paymentMethod?.Get("digital_wallet");
                    summary.MaskedCardNumber = digitalWallet?.GetValue<string>("masked_token_first6last4");
                    summary.PaymentType = EnumConverter.GetMapping(Target.GP_API, PaymentMethodName.DigitalWallet);
                }
                else if(paymentMethod.Has("bank_transfer")) 
                {
                    JsonDoc bankTransfer = paymentMethod?.Get("bank_transfer");
                    summary.AccountNumberLast4 = bankTransfer?.GetValue<string>("masked_account_number_last4");
                    summary.AccountType = bankTransfer?.GetValue<string>("account_type");
                    summary.PaymentType = EnumConverter.GetMapping(Target.GP_API, PaymentMethodName.BankTransfer);
                }
                else if(paymentMethod.Has("apm")) 
                {
                    JsonDoc apm = paymentMethod?.Get("apm");
                    var alternativePaymentResponse = new AlternativePaymentResponse();
                    alternativePaymentResponse.RedirectUrl = apm?.GetValue<string>("redirect_url");
                    alternativePaymentResponse.ProviderName = apm?.GetValue<string>("provider");
                    alternativePaymentResponse.ProviderReference = apm?.GetValue<string>("provider_reference");
                    summary.AlternativePaymentResponse = alternativePaymentResponse;
                    summary.PaymentType = EnumConverter.GetMapping(Target.GP_API, PaymentMethodName.APM);
                }
            }

            summary.FraudManagementResponse = doc.Has("risk_assessment") ? MapFraudManagementReport(doc.Get("risk_assessment")) : null;
            return summary;
        }

        public static PayLinkSummary MapPayLinkSummary(JsonDoc doc)
        {
            var summary = new PayLinkSummary();            
            
            summary.MerchantId = doc.GetValue<string>("merchant_id");
            summary.MerchantName = doc.GetValue<string>("merchant_name");
            summary.AccountId = doc.GetValue<string>("account_id");
            summary.AccountName = doc.GetValue<string>("account_name");
            summary.Id = doc.GetValue<string>("id");
            summary.Url = doc.GetValue<string>("url");
            summary.Status = GetPayLinkStatus(doc);
            summary.Type = GetPayLinkType(doc);             
            summary.UsageMode = GetPaymentMethodUsageMode(doc);
            summary.UsageLimit = doc.GetValue<string>("usage_limit");            
            summary.Reference = doc.GetValue<string>("reference");
            summary.Name = doc.GetValue<string>("name");
            summary.Description = doc.GetValue<string>("description");            
            summary.ViewedCount = doc.GetValue<string>("viewed_count");
            summary.ExpirationDate = doc.GetValue<DateTime?>("expiration_date", DateConverter);

            summary.Shippable = doc.GetValue<string>("shippable");
            summary.UsageCount = doc.GetValue<string>("usage_count");
            summary.Images = doc.GetArray<string>("images").ToArray();//GetImages(doc); //ToDo 
            summary.ShippingAmount = doc.GetValue<string>("shipping_amount");            

            if (doc.Has("transactions")) {
                summary.Amount = doc.Get("transactions").GetValue<string>("amount").ToAmount();
                summary.Currency = doc.Get("transactions").GetValue<string>("currency");
                summary.AllowedPaymentMethods = GetAllowedPaymentMethods(doc.Get("transactions"));
                if (doc.Get("transactions").Has("transaction_list")) {
                    summary.Transactions = new List<TransactionSummary>();
                    foreach (var transaction in doc.Get("transactions").GetArray<JsonDoc>("transaction_list") ?? Enumerable.Empty<JsonDoc>()) {
                        summary.Transactions.Add(CreateTransactionSummary(transaction));
                    }
                }
            }
            return summary;
        }

       
        private static TransactionSummary CreateTransactionSummary(JsonDoc doc)
        {
            var transaction = new TransactionSummary();
            transaction.TransactionId = doc.GetValue<string>("id");
            transaction.TransactionDate = doc.GetValue<DateTime?>("time_created", DateConverter);
            transaction.TransactionStatus = doc.GetValue<string>("status");
            transaction.TransactionType = doc.GetValue<string>("type");
            transaction.Channel = doc.GetValue<string>("channel");
            transaction.Amount = doc.GetValue<string>("amount").ToAmount();
            transaction.Currency = doc.GetValue<string>("currency");
            transaction.ReferenceNumber = doc.GetValue<string>("reference");
            transaction.ClientTransactionId = doc.GetValue<string>("reference");

            return transaction;
        }

        private static List<PaymentMethodName> GetAllowedPaymentMethods(JsonDoc doc) 
        {
            List<PaymentMethodName> List = null;

            if (doc.Has("allowed_payment_methods")) {
                List = new List<PaymentMethodName>();
                foreach (var item in doc.GetArray<string>("allowed_payment_methods")) {
                    PaymentMethodName methodName = EnumConverter.FromMapping<PaymentMethodName>(Target.GP_API, item);
                    List.Add(methodName);
                }
            }
            return List;
        }

        private static PayLinkType? GetPayLinkType(JsonDoc doc) 
        {
            if (doc.Has("type")) {
                return (PayLinkType)Enum.Parse(typeof(PayLinkType), doc.GetValue<string>("type").ToUpper());                            
            }
            return null;
        }

        private static PayLinkStatus? GetPayLinkStatus(JsonDoc doc) 
        {
            if (doc.Has("status")) {
                return (PayLinkStatus)Enum.Parse(typeof(PayLinkStatus), doc.GetValue<string>("status"));                
            }
            return null;            
        }

        private static DateTime? DateConverter(object value) 
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString())) {
                return Convert.ToDateTime(value);
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
            else if (reportType == ReportType.DocumentDisputeDetail && result is DisputeDocument) {                
                    result = MapDisputeDocument(json) as T;                
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
            else if (reportType == ReportType.FindPayLinkPaged && result is PagedResult<PayLinkSummary>) {
                SetPagingInfo(result as PagedResult<PayLinkSummary>, json);
                foreach (var doc in json.GetArray<JsonDoc>("links") ?? Enumerable.Empty<JsonDoc>()) {
                    (result as PagedResult<PayLinkSummary>).Add(MapPayLinkSummary(doc));
                }                
            }
            else if (reportType == ReportType.PayLinkDetail && result is PayLinkSummary) {
                    result = MapPayLinkSummary(json) as T;
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
                DepositDate = doc.GetValue<DateTime?>("time_created", DateConverter),
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

        public static DisputeDocument MapDisputeDocument(JsonDoc doc)
        {
            return new DisputeDocument
            {
                Id = doc.GetValue<string>("id"),
                Base64Content = doc.GetValue<string>("b64_content"),               
            };
        }

        public static DisputeSummary MapDisputeSummary(JsonDoc doc) {
            var summary = new DisputeSummary {
                CaseId = doc.GetValue<string>("id"),
                CaseIdTime = doc.GetValue<DateTime?>("time_created", DateConverter),
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
                summary.RespondByDate = doc.GetValue<DateTime?>("time_to_respond_by", DateConverter);
            }

            var counter = 0;

            foreach (var item in doc.GetArray<JsonDoc>("documents") ?? Enumerable.Empty<JsonDoc>())
            {
                if (string.IsNullOrEmpty(item.GetValue<string>("id"))) {
                    var document = new DisputeDocument
                    {
                        Id = doc.GetValue<string>("id"),
                        Type = !string.IsNullOrEmpty(doc.GetValue<string>("type")) ? doc.GetValue<string>("type") : null,
                    };
                    summary.Documents[counter] = document;
                    counter++;
                }
            }
            return summary;
        }

        public static DisputeSummary MapSettlementDisputeSummary(JsonDoc doc) {
            var summary = MapDisputeSummary(doc);

            summary.CaseIdTime = doc.GetValue<DateTime?>("stage_time_created", DateConverter);
            summary.DepositDate = doc.GetValue<DateTime?>("deposit_time_created",DateConverter);
            summary.DepositReference = doc.GetValue<string>("deposit_id");
            summary.TransactionTime = doc.Get("transaction")?.GetValue<DateTime?>("time_created", DateConverter);
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

        private static FraudManagementResponse MapFraudManagement(JsonDoc response)
        {
            var fraudFilterResponse = new FraudManagementResponse();
            if (response.Has("risk_assessment")) {
                var fraudResponses = response.GetArray<JsonDoc>("risk_assessment");
                foreach (var fraudResponse in fraudResponses) {
                    fraudFilterResponse = MapFraudManagementReport(fraudResponse);                   
                }
                return fraudFilterResponse;
            }
            return null;
        }

        private static FraudManagementResponse MapFraudManagementReport(JsonDoc response)
        {
            var fraudFilterResponse = new FraudManagementResponse();
            var fraudResponse = response;
            fraudFilterResponse.FraudResponseMode = fraudResponse.GetValue<string>("mode") ?? null;
            fraudFilterResponse.FraudResponseResult = fraudResponse.Has("result") ? EnumConverter.FromMapping<FraudFilterResult>(Target.GP_API, fraudResponse.GetValue("result")).ToString() : string.Empty;
            fraudFilterResponse.FraudResponseMessage = fraudResponse.GetValue<string>("message") ?? null;
            if (fraudResponse.Has("rules"))
            {
                fraudFilterResponse.FraudResponseRules = new List<FraudRule>();
                foreach (var rule in fraudResponse.GetArray<JsonDoc>("rules") ?? Enumerable.Empty<JsonDoc>())
                {
                    var fraudRule = new FraudRule();
                    fraudRule.Key = rule.GetValue<string>("reference") ?? null;
                    fraudRule.Mode = (FraudFilterMode)(Enum.Parse(typeof(FraudFilterMode), rule.GetValue<string>("mode")));
                    fraudRule.Description = rule.GetValue<string>("description") ?? null;
                    fraudRule.result = rule.Has("result") ? EnumConverter.FromMapping<FraudFilterResult>(Target.GP_API, rule.GetValue("result")).ToString() : null;
                    fraudFilterResponse.FraudResponseRules.Add(fraudRule);
                }
            }
            return fraudFilterResponse;
        }

        public static DccRateData MapDccInfo(JsonDoc response)
        {
            var currencyConversion = response;

            if((!response.Get("action").GetValue("type").Equals("RATE_LOOKUP")) && !response.Has("currency_conversion")) {
                return null;
            }
            if (response.Has("currency_conversion")) {
                currencyConversion = response.Get("currency_conversion");
            }
            
            return new DccRateData
            { 
                CardHolderCurrency = currencyConversion.GetValue<string>("payer_currency") ?? null,
                CardHolderAmount = currencyConversion.GetValue<string>("payer_amount").ToAmount() ?? null,
                CardHolderRate = currencyConversion.GetValue<string>("exchange_rate").ToDecimal() ?? null,
                MerchantCurrency = currencyConversion.GetValue<string>("currency") ?? null,
                MerchantAmount = currencyConversion.GetValue<string>("amount").ToAmount() ?? null,
                MarginRatePercentage = currencyConversion.GetValue<string>("margin_rate_percentage") ??  null,
                ExchangeRateSourceName = currencyConversion.GetValue<string>("exchange_rate_source") ?? null,
                CommissionPercentage = currencyConversion.GetValue<string>("commission_percentage") ?? null,
                ExchangeRateSourceTimestamp = currencyConversion.GetValue<DateTime?>("exchange_rate_time_created", DateConverter),
                DccId = currencyConversion.GetValue<string>("id") ?? null
            };
        }

        private static Secure3dVersion Parse3DSVersion(string messageVersion) {
            if (messageVersion.StartsWith("1."))
                return Secure3dVersion.One;
            if (messageVersion.StartsWith("2."))
                return Secure3dVersion.Two;
            return Secure3dVersion.Any;
        }

        public static Transaction Map3DSecureData(string rawResponse)
        {
            if (!string.IsNullOrEmpty(rawResponse))
            {
                JsonDoc json = JsonDoc.Parse(rawResponse);

                var transaction = new Transaction
                {
                    ThreeDSecure = new ThreeDSecure
                    {
                        ServerTransactionId = json.GetValue<string>("id"),                        
                        MessageVersion = json.Get("three_ds")?.GetValue<string>("message_version"),
                        Version = Parse3DSVersion(json.Get("three_ds")?.GetValue<string>("message_version")),
                        Status = json.GetValue<string>("status"),
                        DirectoryServerStartVersion = json.Get("three_ds")?.GetValue<string>("ds_protocol_version_start"),
                        DirectoryServerEndVersion = json.Get("three_ds")?.GetValue<string>("ds_protocol_version_end"),
                        AcsStartVersion = json.Get("three_ds")?.GetValue<string>("acs_protocol_version_start"),
                        AcsEndVersion = json.Get("three_ds")?.GetValue<string>("acs_protocol_version_end"),
                        AcsReferenceNumber = json.Get("three_ds")?.GetValue<string>("acs_reference_number"),
                        ProviderServerTransRef = json.Get("three_ds")?.GetValue<string>("server_trans_ref"),
                        Enrolled = json.Get("three_ds")?.GetValue<string>("enrolled_status"),
                        Eci = !string.IsNullOrEmpty(json.Get("three_ds")?.GetValue<string>("eci")) ? json.Get("three_ds")?.GetValue<string>("eci") : null,
                        AcsInfoIndicator = json.Get("three_ds")?.GetArray<string>("acs_info_indicator"),
                        ChallengeMandated = json.Get("three_ds")?.GetValue<string>("challenge_status") == "MANDATED",
                        PayerAuthenticationRequest = (!string.IsNullOrEmpty(json.Get("three_ds").GetValue<string>("acs_challenge_request_url")) && json.GetValue<string>("status") == "CHALLENGE_REQUIRED") ?
                            json.Get("three_ds")?.GetValue<string>("challenge_value") :
                            json.Get("three_ds")?.Get("method_data")?.GetValue<string>("encoded_method_data"),
                        IssuerAcsUrl = (!string.IsNullOrEmpty(json.Get("three_ds").GetValue<string>("acs_challenge_request_url")) && json.GetValue<string>("status") == "CHALLENGE_REQUIRED") ?
                            json.Get("three_ds")?.GetValue<string>("acs_challenge_request_url") :
                            json.Get("three_ds")?.GetValue<string>("method_url"),
                        Currency = json.GetValue<string>("currency"),
                        Amount = json.GetValue<string>("amount").ToAmount(),
                        AuthenticationValue = json.Get("three_ds")?.GetValue<string>("authentication_value"),
                        DirectoryServerTransactionId = json.Get("three_ds")?.GetValue<string>("ds_trans_ref"),
                        AcsTransactionId = json.Get("three_ds")?.GetValue<string>("acs_trans_ref"),
                        StatusReason = json.Get("three_ds")?.GetValue<string>("status_reason"),
                        MessageCategory = json.Get("three_ds")?.GetValue<string>("message_category"),
                        MessageType = json.Get("three_ds")?.GetValue<string>("message_type"),
                        SessionDataFieldName = json.Get("three_ds")?.GetValue<string>("session_data_field_name"),
                        ChallengeReturnUrl = json.Get("notifications")?.GetValue<string>("challenge_return_url"),
                        LiabilityShift = json.Get("three_ds")?.GetValue<string>("liability_shift"),
                        AuthenticationSource = json.Get("three_ds")?.GetValue<string>("authentication_source"),
                        AuthenticationType = json.Get("three_ds")?.GetValue<string>("authentication_request_type"),
                        //AcsInfoIndicator = json.Get("three_ds")?.GetArray<string>("acs_decoupled_response_indicator"),
                        WhitelistStatus = json.Get("three_ds")?.GetValue<string>("whitelist_status"),
                        MessageExtensions = new List<MessageExtension>()
                    }
                };

                // Mobile data
                if (!string.IsNullOrEmpty(json.GetValue<string>("source")) && json.GetValue<string>("source").Equals("MOBILE_SDK"))
                {
                    if (json.Get("three_ds")?.Get("mobile_data") != null)
                    {
                        JsonDoc mobile_data = json.Get("three_ds").Get("mobile_data");

                        transaction.ThreeDSecure.PayerAuthenticationRequest = mobile_data.GetValue<string>("acs_signed_content");

                        if (mobile_data.Get("acs_rendering_type").HasKeys())
                        {
                            JsonDoc acs_rendering_type = mobile_data.Get("acs_rendering_type");
                            transaction.ThreeDSecure.AcsInterface = acs_rendering_type.GetValue<string>("acs_interface");
                            transaction.ThreeDSecure.AcsUiTemplate = acs_rendering_type.GetValue<string>("acs_ui_template");
                        }
                    }
                }

                var messageExtensions = json.Get("three_ds")?.GetEnumerator("message_extension");

                if (messageExtensions != null)
                {
                    foreach (JsonDoc messageExtension in messageExtensions)
                    {
                        MessageExtension msgExtension = new MessageExtension
                        {
                            CriticalityIndicator = messageExtension.GetValue<string>("criticality_indicator"),
                            MessageExtensionData = messageExtension.GetValue<JsonDoc>("data")?.ToString(),
                            MessageExtensionId = messageExtension.GetValue<string>("id"),
                            MessageExtensionName = messageExtension.GetValue<string>("name")
                        };
                        transaction.ThreeDSecure.MessageExtensions.Add(msgExtension);
                    }
                }

                return transaction;
            }
            return new Transaction();
        }

        public static CardIssuerResponse MapCardIssuerResponse(JsonDoc response)
        {
            return new CardIssuerResponse { 
                Result = response.GetValue<string>("result") ?? null,
                AvsResult = response.GetValue<string>("avs_result") ?? null,
                CvvResult = response.GetValue<string>("cvv_result") ?? null,
                AvsAddressResult = response.GetValue<string>("avs_address_result") ?? null,
                AvsPostalCodeResult = response.GetValue<string>("avs_postal_code_result") ?? null,
            };
        }
    }
}