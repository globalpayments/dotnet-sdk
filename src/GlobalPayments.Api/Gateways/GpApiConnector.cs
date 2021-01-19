using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Mapping;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace GlobalPayments.Api.Gateways {
    internal class GpApiConnector : RestGateway, IPaymentGateway, IReportingService, ISecure3dProvider {
        private const string IDEMPOTENCY_HEADER = "x-gp-idempotency";

        public string AppId { get; set; }
        public string AppKey { get; set; }
        public int? SecondsToExpire { get; set; }
        public IntervalToExpire? IntervalToExpire { get; set; }
        public Channel Channel { get; set; }
        public Language Language { get; set; }
        public string Country { get; set; }

        private string _AccessToken;
        public string AccessToken {
            get {
                return _AccessToken;
            }
            internal set {
                _AccessToken = value;
                if (string.IsNullOrEmpty(_AccessToken)) {
                    Headers.Remove("Authorization");
                }
                else {
                    Headers["Authorization"] = $"Bearer {_AccessToken}";
                }
            }
        }
        private string _DataAccountName;
        public string DataAccountName {
            get {
                if (string.IsNullOrEmpty(_DataAccountName)) {
                    throw new GatewayException("DataAccountName is not set");
                }
                return _DataAccountName;
            }
            internal set {
                _DataAccountName = value;
            }
        }
        private string _DisputeManagementAccountName;
        public string DisputeManagementAccountName {
            get {
                if (string.IsNullOrEmpty(_DisputeManagementAccountName)) {
                    throw new GatewayException("DisputeManagementAccountName is not set");
                }
                return _DisputeManagementAccountName;
            }
            internal set {
                _DisputeManagementAccountName = value;
            }
        }
        private string _TokenizationAccountName;
        public string TokenizationAccountName {
            get {
                if (string.IsNullOrEmpty(_TokenizationAccountName)) {
                    throw new GatewayException("TokenizationAccountName is not set");
                }
                return _TokenizationAccountName;
            }
            internal set {
                _TokenizationAccountName = value;
            }
        }
        private string _TransactionProcessingAccountName;
        public string TransactionProcessingAccountName {
            get {
                if (string.IsNullOrEmpty(_TransactionProcessingAccountName)) {
                    throw new GatewayException("TransactionProcessingAccountName is not set");
                }
                return _TransactionProcessingAccountName;
            }
            internal set {
                _TransactionProcessingAccountName = value;
            }
        }

        public bool SupportsHostedPayments { get { return true; } }

        public Secure3dVersion Version { get { return Secure3dVersion.Any; } }

        internal GpApiConnector() {
            // Set required api version header
            Headers["X-GP-Version"] = "2020-04-10"; //"2020-01-20";
            Headers["Accept"] = "application/json";
            Headers["Accept-Encoding"] = "gzip";
        }

        public void SignIn() {
            var response = GetAccessToken();

            AccessToken = response.Token;
            DataAccountName = response.DataAccountName;
            DisputeManagementAccountName = response.DisputeManagementAccountName;
            TokenizationAccountName = response.TokenizationAccountName;
            TransactionProcessingAccountName = response.TransactionProcessingAccountName;
        }

        public void SignOut() {
            //SendEncryptedRequest<SessionInfo>(SessionInfo.SignOut());
        }

        public GpApiTokenResponse GetAccessToken() {
            AccessToken = null;

            GpApiRequest request = GpApiSessionInfo.SignIn(AppId, AppKey, SecondsToExpire, IntervalToExpire);

            string response = base.DoTransaction(HttpMethod.Post, request.Endpoint, request.RequestBody);

            return Activator.CreateInstance(typeof(GpApiTokenResponse), new object[] { response }) as GpApiTokenResponse;
        }

        private string DoTransactionWithIdempotencyKey(HttpMethod verb, string endpoint, string data = null, Dictionary<string, string> queryStringParams = null, string idempotencyKey = null) {
            if (!string.IsNullOrEmpty(idempotencyKey)) {
                Headers[IDEMPOTENCY_HEADER] = idempotencyKey;
            }
            try {
                return base.DoTransaction(verb, endpoint, data, queryStringParams);
            }
            finally {
                Headers.Remove(IDEMPOTENCY_HEADER);
            }
        }

        public string DoTransaction(HttpMethod verb, string endpoint, string data = null, Dictionary<string, string> queryStringParams = null, string idempotencyKey = null) {
            if (string.IsNullOrEmpty(AccessToken)) {
                SignIn();
            }
            try {
                return DoTransactionWithIdempotencyKey(verb, endpoint, data, queryStringParams, idempotencyKey);
            }
            catch (GatewayException ex) {
                if (ex.ResponseCode == "NOT_AUTHENTICATED") {
                    SignIn();
                    return DoTransactionWithIdempotencyKey(verb, endpoint, data, queryStringParams, idempotencyKey);
                }
                throw ex;
            }
        }

        protected override string HandleResponse(GatewayResponse response) {
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent) {
                var parsed = JsonDoc.Parse(response.RawResponse);
                if (parsed.Has("error_code")) {
                    string errorCode = parsed.GetValue<string>("error_code");
                    string detailedErrorCode = parsed.GetValue<string>("detailed_error_code");
                    string detailedErrorDescription = parsed.GetValue<string>("detailed_error_description");

                    throw new GatewayException($"Status Code: {response.StatusCode} - {detailedErrorDescription}", errorCode, detailedErrorCode);
                }
                throw new GatewayException($"Status Code: {response.StatusCode}", responseMessage: response.RawResponse);
            }
            return response.RawResponse;
        }

        private string GetEntryMode(AuthorizationBuilder builder) {
            if (builder.PaymentMethod is ICardData card) {
                if (card.ReaderPresent) {
                    return card.CardPresent ? "MANUAL" : "IN_APP";
                }
                else {
                    return card.CardPresent ? "MANUAL" : "ECOM";
                }
            }
            else if (builder.PaymentMethod is ITrackData track) {
                if (builder.TagData != null) {
                    return track.EntryMethod.Equals(EntryMethod.Swipe) ? "CHIP" : "CONTACTLESS_CHIP";
                }
                else if (builder.HasEmvFallbackData) {
                    return "CONTACTLESS_SWIPE";
                }
                return "SWIPE";
            }
            return "ECOM";
        }

        private string GetCaptureMode(AuthorizationBuilder builder) {
            if (builder.MultiCapture) {
                return "MULTIPLE";
            }
            else if (builder.TransactionType == TransactionType.Auth) {
                return "LATER";
            }
            return "AUTO";
        }

        public Transaction ProcessAuthorization(AuthorizationBuilder builder) {
            if (string.IsNullOrEmpty(AccessToken)) {
                SignIn();
            }

            var paymentMethod = new JsonDoc()
                .Set("entry_mode", GetEntryMode(builder)); // [MOTO, ECOM, IN_APP, CHIP, SWIPE, MANUAL, CONTACTLESS_CHIP, CONTACTLESS_SWIPE]

            if (builder.PaymentMethod is ICardData) {
                var cardData = builder.PaymentMethod as ICardData;

                var card = new JsonDoc()
                    .Set("number", cardData.Number)
                    .Set("expiry_month", cardData.ExpMonth.HasValue ? cardData.ExpMonth.ToString().PadLeft(2, '0') : null)
                    .Set("expiry_year", cardData.ExpYear.HasValue ? cardData.ExpYear.ToString().PadLeft(4, '0').Substring(2, 2) : null)
                    //.Set("track", "")
                    .Set("tag", builder.TagData)
                    .Set("cvv", cardData.Cvn)
                    .Set("cvv_indicator", EnumConverter.GetMapping(Target.GP_API, cardData.CvnPresenceIndicator)) // [ILLEGIBLE, NOT_PRESENT, PRESENT]
                    .Set("avs_address", builder.BillingAddress?.StreetAddress1)
                    .Set("avs_postal_code", builder.BillingAddress?.PostalCode)
                    .Set("funding", builder.PaymentMethod?.PaymentMethodType == PaymentMethodType.Debit ? "DEBIT" : "CREDIT") // [DEBIT, CREDIT]
                    .Set("authcode", builder.OfflineAuthCode);
                    //.Set("brand_reference", "")

                if (builder.EmvChipCondition != null) {
                    card.Set("chip_condition", EnumConverter.GetMapping(Target.GP_API, builder.EmvChipCondition)); // [PREV_SUCCESS, PREV_FAILED]
                }

                paymentMethod.Set("card", card);

                if (builder.TransactionType == TransactionType.Verify) {
                    if (builder.RequestMultiUseToken) {
                        var tokenizationData = new JsonDoc()
                            .Set("account_name", TokenizationAccountName)
                            .Set("reference", builder.ClientTransactionId ?? Guid.NewGuid().ToString())
                            .Set("name", "")
                            .Set("card", card);

                        var tokenizationResponse = DoTransaction(HttpMethod.Post, "/payment-methods", tokenizationData.ToString(), idempotencyKey: builder.IdempotencyKey);

                        return GpApiMapping.MapResponse(tokenizationResponse);
                    }
                    else {
                        if (builder.PaymentMethod is ITokenizable && !string.IsNullOrEmpty((builder.PaymentMethod as ITokenizable).Token)) {
                            var tokenizationResponse = DoTransaction(HttpMethod.Get, $"/payment-methods/{(builder.PaymentMethod as ITokenizable).Token}");

                            return GpApiMapping.MapResponse(tokenizationResponse);
                        }
                        else {
                            var verificationData = new JsonDoc()
                                .Set("account_name", TransactionProcessingAccountName)
                                .Set("channel", EnumConverter.GetMapping(Target.GP_API, Channel))
                                .Set("reference", builder.ClientTransactionId ?? Guid.NewGuid().ToString())
                                .Set("currency", builder.Currency)
                                .Set("country", builder.BillingAddress?.Country ?? Country)
                                .Set("payment_method", paymentMethod);

                            var verificationResponse = DoTransaction(HttpMethod.Post, "/verifications", verificationData.ToString(), idempotencyKey: builder.IdempotencyKey);

                            return GpApiMapping.MapResponse(verificationResponse);
                        }
                    }
                }
            }
            else if (builder.PaymentMethod is ITrackData) {
                var track = builder.PaymentMethod as ITrackData;

                var card = new JsonDoc()
                    .Set("track", track.Value)
                    .Set("tag", builder.TagData)
                    //.Set("cvv", cardData.Cvn)
                    //.Set("cvv_indicator", "") // [ILLEGIBLE, NOT_PRESENT, PRESENT]
                    .Set("avs_address", builder.BillingAddress?.StreetAddress1)
                    .Set("avs_postal_code", builder.BillingAddress?.PostalCode)
                    .Set("authcode", builder.OfflineAuthCode);
                    //.Set("brand_reference", "")

                if (builder.TransactionType == TransactionType.Sale || builder.TransactionType == TransactionType.Refund) {
                    if (string.IsNullOrEmpty(track.Value)) {
                        card.Set("number", track.Pan);
                        card.Set("expiry_month", track.Expiry?.Substring(2, 2));
                        card.Set("expiry_year", track.Expiry?.Substring(0, 2));
                    }
                    if (string.IsNullOrEmpty(builder.TagData)) {
                        card.Set("chip_condition", EnumConverter.GetMapping(Target.GP_API, builder.EmvChipCondition)); // [PREV_SUCCESS, PREV_FAILED]
                    }
                    card.Set("funding", builder.PaymentMethod?.PaymentMethodType == PaymentMethodType.Debit ? "DEBIT" : "CREDIT"); // [DEBIT, CREDIT]
                }

                paymentMethod.Set("card", card);
            }

            // tokenized payment method
            if (builder.PaymentMethod is ITokenizable) {
                string token = ((ITokenizable)builder.PaymentMethod).Token;
                if (!string.IsNullOrEmpty(token)) {
                    paymentMethod.Set("id", token);
                }
            }

            // pin block
            if (builder.PaymentMethod is IPinProtected) {
                paymentMethod.Get("card")?.Set("pin_block", ((IPinProtected)builder.PaymentMethod).PinBlock);
            }

            // authentication
            if (builder.PaymentMethod is CreditCardData) {
                paymentMethod.Set("name", (builder.PaymentMethod as CreditCardData).CardHolderName);

                var secureEcom = (builder.PaymentMethod as CreditCardData).ThreeDSecure;
                if (secureEcom != null) {
                    var three_ds = new JsonDoc()
                        // Indicates the version of 3DS
                        .Set("message_version", secureEcom.MessageVersion)
                        // An indication of the degree of the authentication and liability shift obtained for this transaction.
                        // It is determined during the 3D Secure process.
                        .Set("eci", secureEcom.Eci)
                        // The authentication value created as part of the 3D Secure process.
                        .Set("value", secureEcom.AuthenticationValue)
                        // The reference created by the 3DSecure provider to identify the specific authentication attempt.
                        .Set("server_trans_ref", secureEcom.ServerTransactionId)
                        // The reference created by the 3DSecure Directory Server to identify the specific authentication attempt.
                        .Set("ds_trans_ref", secureEcom.DirectoryServerTransactionId);

                    var authentication = new JsonDoc().Set("three_ds", three_ds);

                    paymentMethod.Set("authentication", authentication);
                }
            }

            // encryption
            if (builder.PaymentMethod is IEncryptable) {
                var encryptionData = ((IEncryptable)builder.PaymentMethod).EncryptionData;

                if (encryptionData != null) {
                    var encryption = new JsonDoc()
                        .Set("version", encryptionData.Version);

                    if (!string.IsNullOrEmpty(encryptionData.KTB)) {
                        encryption.Set("method", "KTB");
                        encryption.Set("info", encryptionData.KTB);
                    }
                    else if (!string.IsNullOrEmpty(encryptionData.KSN)) {
                        encryption.Set("method", "KSN");
                        encryption.Set("info", encryptionData.KSN);
                    }

                    if (encryption.Has("info")) {
                        paymentMethod.Set("encryption", encryption);
                    }
                }
            }

            var data = new JsonDoc()
                .Set("account_name", TransactionProcessingAccountName)
                .Set("type", builder.TransactionType == TransactionType.Refund ? "REFUND" : "SALE") // [SALE, REFUND]
                .Set("channel", EnumConverter.GetMapping(Target.GP_API, Channel)) // [CP, CNP]
                .Set("capture_mode", GetCaptureMode(builder)) // [AUTO, LATER, MULTIPLE]
                 //.Set("remaining_capture_count", "") //Pending Russell
                .Set("authorization_mode", builder.AllowPartialAuth ? "PARTIAL" : "WHOLE")
                .Set("amount", builder.Amount.ToNumericCurrencyString())
                .Set("currency", builder.Currency)
                .Set("reference", builder.ClientTransactionId ?? Guid.NewGuid().ToString())
                .Set("description", builder.Description)
                .Set("order_reference", builder.OrderId)
                .Set("gratuity_amount", builder.Gratuity.ToNumericCurrencyString())
                .Set("cashback_amount", builder.CashBackAmount.ToNumericCurrencyString())
                .Set("surcharge_amount", builder.SurchargeAmount.ToNumericCurrencyString())
                .Set("convenience_amount", builder.ConvenienceAmount.ToNumericCurrencyString())
                .Set("country", builder.BillingAddress?.Country ?? Country)
                //.Set("language", EnumConverter.GetMapping(Target.GP_API, Language))
                .Set("ip_address", builder.CustomerIpAddress)
                //.Set("site_reference", "") //
                .Set("payment_method", paymentMethod);

            // stored credential
            if (builder.StoredCredential != null) {
                data.Set("initiator", EnumConverter.GetMapping(Target.GP_API, builder.StoredCredential.Initiator));
                var storedCredential = new JsonDoc()
                    .Set("model", EnumConverter.GetMapping(Target.GP_API, builder.StoredCredential.Type))
                    .Set("reason", EnumConverter.GetMapping(Target.GP_API, builder.StoredCredential.Reason))
                    .Set("sequence", EnumConverter.GetMapping(Target.GP_API, builder.StoredCredential.Sequence));
                data.Set("stored_credential", storedCredential);
            }

            var response = DoTransaction(HttpMethod.Post, "/transactions", data.ToString(), idempotencyKey: builder.IdempotencyKey);

            return GpApiMapping.MapResponse(response);
        }

        public Transaction ManageTransaction(ManagementBuilder builder) {
            string response = string.Empty;

            if (builder.TransactionType == TransactionType.Capture) {
                var data = new JsonDoc()
                    .Set("amount", builder.Amount.ToNumericCurrencyString())
                    .Set("gratuity_amount", builder.Gratuity.ToNumericCurrencyString());
                response = DoTransaction(HttpMethod.Post, $"/transactions/{builder.TransactionId}/capture", data.ToString(), idempotencyKey: builder.IdempotencyKey);
            }
            else if (builder.TransactionType == TransactionType.Refund) {
                var data = new JsonDoc()
                    .Set("amount", builder.Amount.ToNumericCurrencyString());
                response = DoTransaction(HttpMethod.Post, $"/transactions/{builder.TransactionId}/refund", data.ToString(), idempotencyKey: builder.IdempotencyKey);
            }
            else if (builder.TransactionType == TransactionType.Reversal) {
                var data = new JsonDoc()
                    .Set("amount", builder.Amount.ToNumericCurrencyString());
                response = DoTransaction(HttpMethod.Post, $"/transactions/{builder.TransactionId}/reversal", data.ToString(), idempotencyKey: builder.IdempotencyKey);
            }
            else if (builder.TransactionType == TransactionType.TokenUpdate && builder.PaymentMethod is CreditCardData) {
                var cardData = builder.PaymentMethod as CreditCardData;

                var card = new JsonDoc()
                    .Set("expiry_month", cardData.ExpMonth.HasValue ? cardData.ExpMonth.ToString().PadLeft(2, '0') : string.Empty)
                    .Set("expiry_year", cardData.ExpYear.HasValue ? cardData.ExpYear.ToString().PadLeft(4, '0').Substring(2, 2) : string.Empty);

                var payload = new JsonDoc()
                    .Set("card", card);

                response = DoTransaction(new HttpMethod("PATCH"), $"/payment-methods/{(builder.PaymentMethod as ITokenizable).Token}/edit", payload.ToString(), idempotencyKey: builder.IdempotencyKey);
            }
            else if (builder.TransactionType == TransactionType.TokenDelete && builder.PaymentMethod is ITokenizable) {
                response = DoTransaction(HttpMethod.Post, $"/payment-methods/{(builder.PaymentMethod as ITokenizable).Token}/delete", idempotencyKey: builder.IdempotencyKey);
            }
            else if (builder.TransactionType == TransactionType.Detokenize && builder.PaymentMethod is ITokenizable) {
                response = DoTransaction(HttpMethod.Post, $"/payment-methods/{(builder.PaymentMethod as ITokenizable).Token}/detokenize", idempotencyKey: builder.IdempotencyKey);
            }
            else if (builder.TransactionType == TransactionType.DisputeAcceptance) {
                response = DoTransaction(HttpMethod.Post, $"/disputes/{builder.DisputeId}/acceptance", idempotencyKey: builder.IdempotencyKey);
            }
            else if (builder.TransactionType == TransactionType.DisputeChallenge) {
                var data = new JsonDoc()
                    .Set("documents", builder.DisputeDocuments);

                response = DoTransaction(HttpMethod.Post, $"/disputes/{builder.DisputeId}/challenge", data.ToString(), idempotencyKey: builder.IdempotencyKey);
            }
            return GpApiMapping.MapResponse(response);
        }

        public string SerializeRequest(AuthorizationBuilder builder) {
            throw new NotImplementedException();
        }

        public T ProcessReport<T>(ReportBuilder<T> builder) where T : class {
            if (string.IsNullOrEmpty(AccessToken)) {
                SignIn();
            }

            string reportUrl = string.Empty;
            Dictionary<string, string> queryStringParams = new Dictionary<string, string>();

            Action<Dictionary<string, string>, string, string> addQueryStringParam = (queryParams, name, value) => {
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value)) {
                    queryParams.Add(name, value);
                }
            };

            if (builder is TransactionReportBuilder<T>) {
                var trb = builder as TransactionReportBuilder<T>;

                switch (builder.ReportType) {
                    case ReportType.TransactionDetail:
                        reportUrl = $"/transactions/{trb.TransactionId}";
                        break;
                    case ReportType.FindTransactions:
                        reportUrl = "/transactions";

                        addQueryStringParam(queryStringParams, "page", trb.Page?.ToString());
                        addQueryStringParam(queryStringParams, "page_size", trb.PageSize?.ToString());
                        addQueryStringParam(queryStringParams, "order_by", EnumConverter.GetMapping(Target.GP_API, trb.TransactionOrderBy));
                        addQueryStringParam(queryStringParams, "order", EnumConverter.GetMapping(Target.GP_API, trb.TransactionOrder));
                        addQueryStringParam(queryStringParams, "id", trb.TransactionId);
                        addQueryStringParam(queryStringParams, "type", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.PaymentType));
                        addQueryStringParam(queryStringParams, "channel", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.Channel));
                        addQueryStringParam(queryStringParams, "amount", trb.SearchBuilder.Amount.ToNumericCurrencyString());
                        addQueryStringParam(queryStringParams, "currency", trb.SearchBuilder.Currency);
                        addQueryStringParam(queryStringParams, "number_first6", trb.SearchBuilder.CardNumberFirstSix);
                        addQueryStringParam(queryStringParams, "number_last4", trb.SearchBuilder.CardNumberLastFour);
                        addQueryStringParam(queryStringParams, "token_first6", trb.SearchBuilder.TokenFirstSix);
                        addQueryStringParam(queryStringParams, "token_last4", trb.SearchBuilder.TokenLastFour);
                        addQueryStringParam(queryStringParams, "account_name", trb.SearchBuilder.AccountName);
                        addQueryStringParam(queryStringParams, "brand", trb.SearchBuilder.CardBrand);
                        addQueryStringParam(queryStringParams, "brand_reference", trb.SearchBuilder.BrandReference);
                        addQueryStringParam(queryStringParams, "authcode", trb.SearchBuilder.AuthCode);
                        addQueryStringParam(queryStringParams, "reference", trb.SearchBuilder.ReferenceNumber);
                        addQueryStringParam(queryStringParams, "status", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.TransactionStatus));
                        addQueryStringParam(queryStringParams, "from_time_created", trb.StartDate?.ToString("yyyy-MM-dd"));
                        addQueryStringParam(queryStringParams, "to_time_created", trb.EndDate?.ToString("yyyy-MM-dd"));
                        addQueryStringParam(queryStringParams, "country", trb.SearchBuilder.Country);
                        addQueryStringParam(queryStringParams, "batch_id", trb.SearchBuilder.BatchId);
                        addQueryStringParam(queryStringParams, "entry_mode", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.PaymentEntryMode));
                        addQueryStringParam(queryStringParams, "name", trb.SearchBuilder.Name);
                        break;
                    case ReportType.FindSettlementTransactions:
                        reportUrl = "/settlement/transactions";

                        addQueryStringParam(queryStringParams, "page", trb.Page?.ToString());
                        addQueryStringParam(queryStringParams, "page_size", trb.PageSize?.ToString());
                        addQueryStringParam(queryStringParams, "order", EnumConverter.GetMapping(Target.GP_API, trb.TransactionOrder));
                        addQueryStringParam(queryStringParams, "order_by", EnumConverter.GetMapping(Target.GP_API, trb.TransactionOrderBy));
                        addQueryStringParam(queryStringParams, "number_first6", trb.SearchBuilder.CardNumberFirstSix);
                        addQueryStringParam(queryStringParams, "number_last4", trb.SearchBuilder.CardNumberLastFour);
                        addQueryStringParam(queryStringParams, "deposit_status", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.DepositStatus));
                        addQueryStringParam(queryStringParams, "account_name", DataAccountName);
                        addQueryStringParam(queryStringParams, "brand", trb.SearchBuilder.CardBrand);
                        addQueryStringParam(queryStringParams, "arn", trb.SearchBuilder.AquirerReferenceNumber);
                        addQueryStringParam(queryStringParams, "brand_reference", trb.SearchBuilder.BrandReference);
                        addQueryStringParam(queryStringParams, "authcode", trb.SearchBuilder.AuthCode);
                        addQueryStringParam(queryStringParams, "reference", trb.SearchBuilder.ReferenceNumber);
                        addQueryStringParam(queryStringParams, "status", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.TransactionStatus));
                        addQueryStringParam(queryStringParams, "from_time_created", trb.StartDate?.ToString("yyyy-MM-dd"));
                        addQueryStringParam(queryStringParams, "to_time_created", trb.EndDate?.ToString("yyyy-MM-dd"));
                        addQueryStringParam(queryStringParams, "deposit_id", trb.SearchBuilder.DepositId);
                        addQueryStringParam(queryStringParams, "from_deposit_time_created", trb.SearchBuilder.StartDepositDate?.ToString("yyyy-MM-dd"));
                        addQueryStringParam(queryStringParams, "to_deposit_time_created", trb.SearchBuilder.EndDepositDate?.ToString("yyyy-MM-dd"));
                        addQueryStringParam(queryStringParams, "from_batch_time_created", trb.SearchBuilder.StartBatchDate?.ToString("yyyy-MM-dd"));
                        addQueryStringParam(queryStringParams, "to_batch_time_created", trb.SearchBuilder.EndBatchDate?.ToString("yyyy-MM-dd"));
                        addQueryStringParam(queryStringParams, "system.mid", trb.SearchBuilder.MerchantId);
                        addQueryStringParam(queryStringParams, "system.hierarchy", trb.SearchBuilder.SystemHierarchy);
                        break;
                    case ReportType.DepositDetail:
                        reportUrl = $"/settlement/deposits/{trb.SearchBuilder.DepositId}";
                        break;
                    case ReportType.FindDeposits:
                        reportUrl = "/settlement/deposits";

                        addQueryStringParam(queryStringParams, "page", trb.Page?.ToString());
                        addQueryStringParam(queryStringParams, "page_size", trb.PageSize?.ToString());
                        addQueryStringParam(queryStringParams, "order_by", EnumConverter.GetMapping(Target.GP_API, trb.DepositOrderBy));
                        addQueryStringParam(queryStringParams, "order", EnumConverter.GetMapping(Target.GP_API, trb.DepositOrder));
                        addQueryStringParam(queryStringParams, "account_name", DataAccountName);
                        addQueryStringParam(queryStringParams, "from_time_created", trb.StartDate?.ToString("yyyy-MM-dd"));
                        addQueryStringParam(queryStringParams, "to_time_created", trb.EndDate?.ToString("yyyy-MM-dd"));
                        addQueryStringParam(queryStringParams, "id", trb.SearchBuilder.DepositId);
                        addQueryStringParam(queryStringParams, "status", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.DepositStatus));
                        addQueryStringParam(queryStringParams, "amount", trb.SearchBuilder.Amount.ToNumericCurrencyString());
                        addQueryStringParam(queryStringParams, "masked_account_number_last4", trb.SearchBuilder.AccountNumberLastFour);
                        addQueryStringParam(queryStringParams, "system.mid", trb.SearchBuilder.MerchantId);
                        addQueryStringParam(queryStringParams, "system.hierarchy", trb.SearchBuilder.SystemHierarchy);
                        break;
                    case ReportType.DisputeDetail:
                        reportUrl = $"/disputes/{trb.SearchBuilder.DisputeId}";
                        break;
                    case ReportType.FindDisputes:
                        reportUrl = "/disputes";

                        addQueryStringParam(queryStringParams, "page", trb.Page?.ToString());
                        addQueryStringParam(queryStringParams, "page_size", trb.PageSize?.ToString());
                        addQueryStringParam(queryStringParams, "order_by", EnumConverter.GetMapping(Target.GP_API, trb.DisputeOrderBy));
                        addQueryStringParam(queryStringParams, "order", EnumConverter.GetMapping(Target.GP_API, trb.DisputeOrder));
                        addQueryStringParam(queryStringParams, "arn", trb.SearchBuilder.AquirerReferenceNumber);
                        addQueryStringParam(queryStringParams, "brand", trb.SearchBuilder.CardBrand);
                        addQueryStringParam(queryStringParams, "status", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.DisputeStatus));
                        addQueryStringParam(queryStringParams, "stage", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.DisputeStage));
                        addQueryStringParam(queryStringParams, "from_stage_time_created", trb.SearchBuilder.StartStageDate?.ToString("yyyy-MM-dd"));
                        addQueryStringParam(queryStringParams, "to_stage_time_created", trb.SearchBuilder.EndStageDate?.ToString("yyyy-MM-dd"));
                        addQueryStringParam(queryStringParams, "adjustment_funding", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.AdjustmentFunding));
                        addQueryStringParam(queryStringParams, "from_adjustment_time_created", trb.SearchBuilder.StartAdjustmentDate?.ToString("yyyy-MM-dd"));
                        addQueryStringParam(queryStringParams, "to_adjustment_time_created", trb.SearchBuilder.EndAdjustmentDate?.ToString("yyyy-MM-dd"));
                        addQueryStringParam(queryStringParams, "system.mid", trb.SearchBuilder.MerchantId);
                        addQueryStringParam(queryStringParams, "system.hierarchy", trb.SearchBuilder.SystemHierarchy);
                        break;
                    case ReportType.SettlementDisputeDetail:
                        reportUrl = $"/settlement/disputes/{trb.SearchBuilder.SettlementDisputeId}";
                        addQueryStringParam(queryStringParams, "account_name", DataAccountName);
                        break;
                    case ReportType.FindSettlementDisputes:
                        reportUrl = "/settlement/disputes";

                        addQueryStringParam(queryStringParams, "account_name", DataAccountName);
                        addQueryStringParam(queryStringParams, "page", trb.Page?.ToString());
                        addQueryStringParam(queryStringParams, "page_size", trb.PageSize?.ToString());
                        addQueryStringParam(queryStringParams, "order_by", EnumConverter.GetMapping(Target.GP_API, trb.DisputeOrderBy));
                        addQueryStringParam(queryStringParams, "order", EnumConverter.GetMapping(Target.GP_API, trb.DisputeOrder));
                        addQueryStringParam(queryStringParams, "arn", trb.SearchBuilder.AquirerReferenceNumber);
                        addQueryStringParam(queryStringParams, "brand", trb.SearchBuilder.CardBrand);
                        addQueryStringParam(queryStringParams, "STATUS", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.DisputeStatus));
                        addQueryStringParam(queryStringParams, "stage", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.DisputeStage));
                        addQueryStringParam(queryStringParams, "from_stage_time_created", trb.SearchBuilder.StartStageDate?.ToString("yyyy-MM-dd"));
                        addQueryStringParam(queryStringParams, "to_stage_time_created", trb.SearchBuilder.EndStageDate?.ToString("yyyy-MM-dd"));
                        addQueryStringParam(queryStringParams, "adjustment_funding", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.AdjustmentFunding));
                        addQueryStringParam(queryStringParams, "from_adjustment_time_created", trb.SearchBuilder.StartAdjustmentDate?.ToString("yyyy-MM-dd"));
                        addQueryStringParam(queryStringParams, "to_adjustment_time_created", trb.SearchBuilder.EndAdjustmentDate?.ToString("yyyy-MM-dd"));
                        addQueryStringParam(queryStringParams, "system.mid", trb.SearchBuilder.MerchantId);
                        addQueryStringParam(queryStringParams, "system.hierarchy", trb.SearchBuilder.SystemHierarchy);
                        break;
                    default:
                        break;
                }
            }

            var response = DoTransaction(HttpMethod.Get, reportUrl, queryStringParams: queryStringParams);

            return MapReportResponse<T>(response, builder.ReportType);
        }

        private T MapReportResponse<T>(string rawResponse, ReportType reportType) where T : class {
            T result = Activator.CreateInstance<T>();

            JsonDoc json = JsonDoc.Parse(rawResponse);

            if (reportType == ReportType.TransactionDetail && result is TransactionSummary) {
                result = GpApiMapping.MapTransactionSummary(json) as T;
            }
            else if ((reportType == ReportType.FindTransactions || reportType == ReportType.FindSettlementTransactions)  && result is IEnumerable<TransactionSummary>) {
                IEnumerable<JsonDoc> transactions = json.GetArray<JsonDoc>("transactions");
                foreach (var doc in transactions ?? Enumerable.Empty<JsonDoc>()) {
                    (result as List<TransactionSummary>).Add(GpApiMapping.MapTransactionSummary(doc));
                }
            }
            else if(reportType == ReportType.DepositDetail && result is DepositSummary) {
                result = GpApiMapping.MapDepositSummary(json) as T;
            }
            else if (reportType == ReportType.FindDeposits && result is IEnumerable<DepositSummary>) {
                IEnumerable<JsonDoc> deposits = json.GetArray<JsonDoc>("deposits");
                foreach (var doc in deposits ?? Enumerable.Empty<JsonDoc>()) {
                    (result as List<DepositSummary>).Add(GpApiMapping.MapDepositSummary(doc));
                }
            }
            else if ((reportType == ReportType.DisputeDetail || reportType == ReportType.SettlementDisputeDetail) && result is DisputeSummary) {
                result = GpApiMapping.MapDisputeSummary(json) as T;
            }
            else if ((reportType == ReportType.FindDisputes || reportType == ReportType.FindSettlementDisputes) && result is IEnumerable<DisputeSummary>) {
                IEnumerable<JsonDoc> disputes = json.GetArray<JsonDoc>("disputes");
                foreach (var doc in disputes ?? Enumerable.Empty<JsonDoc>()) {
                    (result as List<DisputeSummary>).Add(GpApiMapping.MapDisputeSummary(doc));
                }
            }

            return result;
        }

        public Transaction ProcessSecure3d(Secure3dBuilder builder) {
            throw new NotImplementedException();

            SignIn();

            if (builder.TransactionType == TransactionType.VerifyEnrolled) {
                var paymentMethod = new JsonDoc();

                if (builder.PaymentMethod is ICardData cardData) {
                    var card = new JsonDoc()
                        .Set("number", cardData.Number)
                        .Set("expiry_month", cardData.ExpMonth?.ToString().PadLeft(2, '0'))
                        .Set("expiry_year", cardData.ExpYear?.ToString().PadLeft(4, '0').Substring(2, 2));

                    paymentMethod.Set("card", card);
                }

                // ToDo: let the user config this
                var notifications = new JsonDoc()
                    .Set("challenge_return_url", "https://ensi808o85za.x.pipedream.net/")
                    .Set("three_ds_method_return_url", "https://ensi808o85za.x.pipedream.net/");

                var data = new JsonDoc()
                    .Set("account_name", TransactionProcessingAccountName)
                    .Set("reference", builder.ReferenceNumber ?? Guid.NewGuid().ToString())
                    .Set("channel", EnumConverter.GetMapping(Target.GP_API, Channel))
                    .Set("amount", builder.Amount.ToNumericCurrencyString())
                    .Set("currency", builder.Currency)
                    .Set("country", Country)
                    .Set("payment_method", paymentMethod)
                    .Set("notifications", notifications);

                var response = DoTransaction(HttpMethod.Post, "/authentications", data.ToString(), idempotencyKey: builder.IdempotencyKey);

                // ToDo: Create mapping function
                JsonDoc json = JsonDoc.Parse(response);

                Func<string, Secure3dVersion> parseVersion = (version) => {
                    if (version.StartsWith("1."))
                        return Secure3dVersion.One;
                    if (version.StartsWith("2."))
                        return Secure3dVersion.Two;
                    return Secure3dVersion.Any;
                };

                return new Transaction {
                    // ToDo: Complete required mappings
                    ThreeDSecure = new ThreeDSecure {
                        ServerTransactionId = json.GetValue<string>("id"),
                        MessageVersion = json.Get("three_ds")?.GetValue<string>("message_version"),
                        Version = parseVersion(json.Get("three_ds")?.GetValue<string>("message_version")),
                        DirectoryServerStartVersion = json.Get("three_ds")?.GetValue<string>("ds_protocol_version_start"),
                        DirectoryServerEndVersion = json.Get("three_ds")?.GetValue<string>("ds_protocol_version_end"),
                        AcsStartVersion = json.Get("three_ds")?.GetValue<string>("acs_protocol_version_start"),
                        AcsEndVersion = json.Get("three_ds")?.GetValue<string>("acs_protocol_version_end"),
                        Enrolled = json.Get("three_ds")?.GetValue<string>("enrolled_status"),
                        Eci = json.Get("three_ds")?.GetValue<string>("eci")?.ToInt32(),
                        ChallengeMandated = json.Get("three_ds")?.GetValue<string>("challenge_status") == "MANDATED",
                    }
                };
            }
            else if (builder.TransactionType == TransactionType.InitiateAuthentication) {
                var order = new JsonDoc()
                    .Set("time_created_reference", builder.OrderCreateDate?.ToString("yyyy-MM-ddThh:mm:ss.fffZ"))
                    .Set("amount", builder.Amount.ToNumericCurrencyString())
                    .Set("currency", builder.Currency)
                    .Set("reference", builder.ReferenceNumber)
                    .Set("address_match_indicator", builder.AddressMatchIndicator)
                    .Set("gift_card_count", builder.GiftCardCount)
                    .Set("gift_card_currency", builder.GiftCardCurrency)
                    .Set("gift_card_amount", builder.GiftCardAmount.ToNumericCurrencyString())
                    .Set("delivery_email", builder.DeliveryEmail)
                    .Set("delivery_timeframe", builder.DeliveryTimeframe?.ToString())
                    .Set("shipping_method", builder.ShippingMethod?.ToString())
                    .Set("shipping_name_matches_cardholder_name", builder.ShippingNameMatchesCardHolderName)
                    .Set("preorder_indicator", builder.PreOrderIndicator?.ToString())
                    .Set("preorder_availability_date", builder.PreOrderAvailabilityDate?.ToString("yyyy-MM-dd"))
                    .Set("reorder_indicator", builder.ReorderIndicator?.ToString())
                    .Set("category", builder.MessageCategory.ToString());

                if (builder.ShippingAddress != null) {
                    var shippingAddress = new JsonDoc()
                        .Set("line1", builder.ShippingAddress.StreetAddress1)
                        .Set("line2", builder.ShippingAddress.StreetAddress2)
                        .Set("line3", builder.ShippingAddress.StreetAddress3)
                        .Set("city", builder.ShippingAddress.City)
                        .Set("postal_code", builder.ShippingAddress.PostalCode)
                        .Set("state", builder.ShippingAddress.State)
                        .Set("country", builder.ShippingAddress.CountryCode);
                    
                    order.Set("shipping_address", shippingAddress);
                }

                var paymentMethod = new JsonDoc();

                if (builder.PaymentMethod is ICardData cardData) {
                    var card = new JsonDoc()
                        .Set("number", cardData.Number)
                        .Set("expiry_month", cardData.ExpMonth?.ToString().PadLeft(2, '0'))
                        .Set("expiry_year", cardData.ExpYear?.ToString().PadLeft(4, '0').Substring(2, 2));

                    paymentMethod.Set("card", card);
                }

                var payer = new JsonDoc()
                    .Set("reference", builder.CustomerAccountId) //ToDo: Confirm
                    .Set("account_age", builder.AccountAgeIndicator?.ToString())
                    .Set("account_creation_date", builder.AccountCreateDate?.ToString("yyyy-MM-dd"))
                    .Set("account_change_date", builder.AccountChangeDate?.ToString("yyyy-MM-dd"))
                    .Set("account_change_indicator", builder.AccountChangeIndicator?.ToString())
                    .Set("account_password_change_date", builder.PasswordChangeDate?.ToString("yyyy-MM-dd"))
                    .Set("account_password_change_indicator", builder.PasswordChangeIndicator?.ToString())
                    .Set("home_phone", new JsonDoc()
                        .Set("country_code", builder.HomeCountryCode)
                        .Set("subscriber_number", builder.HomeNumber)
                    )
                    .Set("work_phone", new JsonDoc()
                        .Set("country_code", builder.WorkCountryCode)
                        .Set("subscriber_number", builder.WorkNumber)
                    )
                    .Set("payment_account_creation_date", builder.PaymentAccountCreateDate?.ToString("yyyy-MM-dd"))
                    .Set("payment_account_age_indicator", builder.PaymentAgeIndicator?.ToString())
                    .Set("suspicious_account_activity", builder.PreviousSuspiciousActivity)
                    .Set("purchases_last_6months_count", builder.NumberOfPurchasesInLastSixMonths)
                    .Set("transactions_last_24hours_count", builder.NumberOfTransactionsInLast24Hours)
                    .Set("transaction_last_year_count", builder.NumberOfTransactionsInLastYear)
                    .Set("provision_attempt_last_24hours_count", builder.NumberOfAddCardAttemptsInLast24Hours)
                    .Set("shipping_address_time_created_reference", builder.ShippingAddressCreateDate?.ToString("yyyy-MM-dd"))
                    .Set("shipping_address_creation_indicator", builder.ShippingAddressUsageIndicator?.ToString());

                var payerPrior3DSAuthenticationData = new JsonDoc()
                    .Set("authentication_method", builder.PriorAuthenticationMethod?.ToString())
                    .Set("acs_transaction_reference", builder.PriorAuthenticationTransactionId)
                    .Set("authentication_timestamp", builder.PriorAuthenticationTimestamp?.ToString("yyyy-MM-ddThh:mm:ss.fffZ"))
                    .Set("authentication_data", builder.PriorAuthenticationData);

                var recurringAuthorizationData = new JsonDoc()
                    .Set("max_number_of_instalments", builder.MaxNumberOfInstallments)
                    .Set("frequency", builder.RecurringAuthorizationFrequency)
                    .Set("expiry_date", builder.RecurringAuthorizationExpiryDate?.ToString("yyyy-MM-dd"));

                var payerLoginData = new JsonDoc()
                    .Set("authentication_data", builder.CustomerAuthenticationData)
                    .Set("authentication_timestamp", builder.CustomerAuthenticationTimestamp?.ToString("yyyy-MM-ddThh:mm:ss.fffZ"))
                    .Set("authentication_type", builder.CustomerAuthenticationMethod?.ToString());

                var browserData = new JsonDoc()
                    .Set("accept_header", builder.BrowserData.AcceptHeader)
                    .Set("color_depth", builder.BrowserData.ColorDepth.ToString())
                    .Set("ip", builder.BrowserData.IpAddress)
                    .Set("java_enabled", builder.BrowserData.JavaEnabled)
                    .Set("javascript_enabled", builder.BrowserData.JavaScriptEnabled)
                    .Set("language", builder.BrowserData.Language)
                    .Set("screen_height", builder.BrowserData.ScreenHeight)
                    .Set("screen_width", builder.BrowserData.ScreenWidth)
                    .Set("challenge_window_size", builder.BrowserData.ChallengeWindowSize.ToString())
                    .Set("timezone", builder.BrowserData.Timezone)
                    .Set("user_agent", builder.BrowserData.UserAgent);

                var data = new JsonDoc()
                    .Set("method_url_completion_status", builder.MethodUrlCompletion)
                    .Set("source", builder.AuthenticationSource)
                    .Set("merchant_contact_url", "") //ToDo: Confirm
                    .Set("order", order)
                    .Set("payment_method", paymentMethod)
                    .Set("payer", payer)
                    .Set("payer_prior_three_ds_authentication_data", payerPrior3DSAuthenticationData)
                    .Set("recurring_authorization_data", recurringAuthorizationData)
                    .Set("payer_login_data", payerLoginData)
                    .Set("browser_data", browserData);
                
                var response = DoTransaction(HttpMethod.Post, $"/authentications/{builder.ServerTransactionId}/initiate", data.ToString(), idempotencyKey: builder.IdempotencyKey);

                // ToDo: Create mapping function
                JsonDoc json = JsonDoc.Parse(response);

                return new Transaction {
                    ThreeDSecure = new ThreeDSecure {
                        ServerTransactionId = json.GetValue<string>("id"),
                    }
                };
            }
            else if (builder.TransactionType == TransactionType.VerifySignature) {
                var response = DoTransaction(HttpMethod.Get, $"/authentications/{builder.ServerTransactionId}/result");

                // ToDo: Create mapping function
                JsonDoc json = JsonDoc.Parse(response);

                return new Transaction {
                    ThreeDSecure = new ThreeDSecure {
                        Currency = json.GetValue<string>("currency"),
                        Amount = json.GetValue<string>("amount").ToAmount(),
                        // three_ds data
                        MessageVersion = json.Get("three_ds")?.GetValue<string>("message_version"),
                        Eci = json.Get("three_ds")?.GetValue<string>("eci")?.ToInt32(),
                        AuthenticationValue = json.Get("three_ds")?.GetValue<string>("authentication_value"),
                        ServerTransactionId = json.Get("three_ds")?.GetValue<string>("server_trans_ref"),
                        DirectoryServerTransactionId = json.Get("three_ds")?.GetValue<string>("ds_trans_ref"),
                        AcsTransactionId = json.Get("three_ds")?.GetValue<string>("acs_trans_ref"),
                        Status = json.Get("three_ds")?.GetValue<string>("status"),
                        StatusReason = json.Get("three_ds")?.GetValue<string>("status_reason"),
                        MessageCategory = json.Get("three_ds")?.GetValue<string>("message_category"),
                    }
                };
            }
            return new Transaction();
        }
    }
}
