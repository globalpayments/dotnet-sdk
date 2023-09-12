using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Logging;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace GlobalPayments.Api.Gateways {
    internal class Gp3DSProvider : RestGateway, ISecure3dProvider {
        public string AccountId { get; set; }
        public string ChallengeNotificationUrl { get; set; }
        public string MerchantContactUrl { get; set; }
        public string MerchantId { get; set; }
        public string MethodNotificationUrl { get; set; }
        public string SharedSecret { get; set; }
        public Secure3dVersion Version { get { return Secure3dVersion.Two; } }
        private Dictionary<string, string> MaskedValues;

        public Transaction ProcessSecure3d(Secure3dBuilder builder) {
            MaskedValues = null;
            TransactionType transType = builder.TransactionType;
            IPaymentMethod paymentMethod = builder.PaymentMethod;
            ISecure3d secure3d = (ISecure3d)paymentMethod;
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd'T'hh:mm:ss.ffffff");

            JsonDoc request = new JsonDoc();
            if (transType.Equals(TransactionType.VerifyEnrolled)) {
                request.Set("request_timestamp", timestamp);
                request.Set("merchant_id", MerchantId);
                request.Set("account_id", AccountId);
                request.Set("method_notification_url", MethodNotificationUrl);

                string hashValue = string.Empty;
                if (paymentMethod is CreditCardData cardData) {
                    request.Set("number", cardData.Number);
                    request.Set("scheme", MapCardScheme(CardUtils.GetBaseCardType(cardData.CardType).ToUpper()));
                    hashValue = cardData.Number;                   

                    MaskedValues = ProtectSensitiveData.HideValue("number", cardData.Number, 4, 6);
                }
                else if (paymentMethod is RecurringPaymentMethod storedCard) {
                    request.Set("payer_reference", storedCard.CustomerKey);
                    request.Set("payment_method_reference", storedCard.Key);
                    hashValue = storedCard.CustomerKey;
                }

                string hash = GenerationUtils.GenerateHash(SharedSecret, timestamp, MerchantId, hashValue);
                SetAuthHeader(hash);

                Request.MaskedValues = MaskedValues;

                string rawResponse = DoTransaction(HttpMethod.Post, "protocol-versions", request.ToString());
                return MapResponse(rawResponse);
            }
            else if (transType.Equals(TransactionType.VerifySignature)) {
                string hash = GenerationUtils.GenerateHash(SharedSecret, timestamp, MerchantId, builder.ServerTransactionId);
                SetAuthHeader(hash);

                var queryValues = new Dictionary<string, string>();
                queryValues.Add("merchant_id", MerchantId);
                queryValues.Add("request_timestamp", timestamp);

                string rawResponse = DoTransaction(HttpMethod.Get, string.Format("authentications/{0}", builder.ServerTransactionId), null, queryValues);
                return MapResponse(rawResponse);
            }
            else if (transType.Equals(TransactionType.InitiateAuthentication)) {
                string orderId = builder.OrderId;
                if (string.IsNullOrEmpty(orderId)) {
                    orderId = GenerationUtils.GenerateOrderId();
                }
                ThreeDSecure secureEcom = secure3d.ThreeDSecure;

                request.Set("request_timestamp", timestamp);
                request.Set("authentication_source", builder.AuthenticationSource.ToString());
                request.Set("authentication_request_type", builder.AuthenticationRequestType.ToString());
                request.Set("message_category", builder.MessageCategory.ToString());
                request.Set("message_version", secureEcom.AcsEndVersion);
                request.Set("server_trans_id", secureEcom.ServerTransactionId);
                request.Set("merchant_id", MerchantId);
                request.Set("account_id", AccountId);
                request.Set("challenge_notification_url", ChallengeNotificationUrl);
                request.Set("challenge_request_indicator", builder.ChallengeRequestIndicator.ToString());
                request.Set("method_url_completion", builder.MethodUrlCompletion.ToString());
                request.Set("merchant_contact_url", MerchantContactUrl);
                request.Set("merchant_initiated_request_type", builder.MerchantInitiatedRequestType?.ToString());
                request.Set("whitelist_status", builder.WhitelistStatus);
                request.Set("decoupled_flow_request", builder.DecoupledFlowRequest ?? null);
                request.Set("decoupled_flow_timeout", builder.DecoupledFlowTimeout ?? null);
                request.Set("decoupled_notification_url", builder.DecoupledNotificationUrl ?? null);
                request.Set("enable_exemption_optimization", builder.EnableExemptionOptimization);

                // card details
                string hashValue = string.Empty;
                JsonDoc cardDetail = request.SubElement("card_detail");
                if (paymentMethod is CreditCardData cardData) {
                    hashValue = cardData.Number;
                    cardDetail.Set("number", cardData.Number);
                    cardDetail.Set("scheme", CardUtils.GetBaseCardType(cardData.CardType).ToUpper());
                    cardDetail.Set("expiry_month", cardData.ExpMonth.ToString());
                    cardDetail.Set("expiry_year", cardData.ExpYear.ToString().Substring(2));
                    cardDetail.Set("full_name", cardData.CardHolderName);

                    var maskedValue = new Dictionary<string, string>();
                    maskedValue.Add("card_detail.expiry_month", cardData.ExpMonth.ToString());
                    maskedValue.Add("card_detail.expiry_year", cardData.ExpYear.ToString().Substring(2));

                    MaskedValues = ProtectSensitiveData.HideValues(maskedValue);
                    MaskedValues = ProtectSensitiveData.HideValue("card_detail.number", cardData.Number, 4, 6);

                    if (!string.IsNullOrEmpty(cardData.CardHolderName)) {
                        string[] names = cardData.CardHolderName.Split(' ');
                        if (names.Length >= 1) {
                            cardDetail.Set("first_name", names[0]);
                        }
                        if (names.Length >= 2) {
                            cardDetail.Set("last_name", string.Join(" ", names.Skip(1)));
                        }
                    }
                }
                else if (paymentMethod is RecurringPaymentMethod storedCard) {
                    hashValue = storedCard.CustomerKey;
                    cardDetail.Set("payer_reference", storedCard.CustomerKey);
                    cardDetail.Set("payment_method_reference", storedCard.Key);
                }

                // order details
                JsonDoc order = request.SubElement("order");
                order.Set("amount", builder.Amount.ToNumericCurrencyString());
                order.Set("currency", builder.Currency);
                order.Set("id", orderId);
                order.Set("address_match_indicator", builder.AddressMatchIndicator ? "true" : "false");
                order.Set("date_time_created", builder.OrderCreateDate?.ToString("yyyy-MM-dd'T'hh:mm'Z'"));
                order.Set("gift_card_count", builder.GiftCardCount);
                order.Set("gift_card_currency", builder.GiftCardCurrency);
                order.Set("gift_card_amount", builder.GiftCardAmount.ToNumericCurrencyString());
                order.Set("delivery_email", builder.DeliveryEmail);
                order.Set("delivery_timeframe", builder.DeliveryTimeframe?.ToString());
                order.Set("shipping_method", builder.ShippingMethod?.ToString());
                order.Set("shipping_name_matches_cardholder_name", builder.ShippingNameMatchesCardHolderName);
                order.Set("preorder_indicator", builder.PreOrderIndicator?.ToString());
                order.Set("reorder_indicator", builder.ReorderIndicator?.ToString());
                order.Set("transaction_type", builder.OrderTransactionType?.ToString());
                order.Set("preorder_availability_date", builder.PreOrderAvailabilityDate?.ToString("yyyy-MM-dd"));

                // shipping address
                Address shippingAddress = builder.ShippingAddress;
                if (shippingAddress != null) {
                    JsonDoc shippingAddressElement = order.SubElement("shipping_address");
                    shippingAddressElement.Set("line1", shippingAddress.StreetAddress1);
                    shippingAddressElement.Set("line2", shippingAddress.StreetAddress2);
                    shippingAddressElement.Set("line3", shippingAddress.StreetAddress3);
                    shippingAddressElement.Set("city", shippingAddress.City);
                    shippingAddressElement.Set("postal_code", shippingAddress.PostalCode);
                    shippingAddressElement.Set("state", shippingAddress.State);
                    shippingAddressElement.Set("country", shippingAddress.CountryCode);
                }

                // payer
                JsonDoc payer = request.SubElement("payer");
                payer.Set("email", builder.CustomerEmail);
                payer.Set("id", builder.CustomerAccountId);
                payer.Set("account_age", builder.AccountAgeIndicator?.ToString());
                payer.Set("account_creation_date", builder.AccountCreateDate?.ToString("yyyy-MM-dd"));
                payer.Set("account_change_indicator", builder.AccountChangeIndicator?.ToString());
                payer.Set("account_change_date", builder.AccountChangeDate?.ToString("yyyy-MM-dd"));
                payer.Set("account_password_change_indicator", builder.PasswordChangeIndicator?.ToString());
                payer.Set("account_password_change_date", builder.PasswordChangeDate?.ToString("yyyy-MM-dd"));
                payer.Set("payment_account_age_indicator", builder.PaymentAgeIndicator?.ToString());
                payer.Set("payment_account_creation_date", builder.PaymentAccountCreateDate?.ToString("yyyy-MM-dd"));
                payer.Set("purchase_count_last_6months", builder.NumberOfPurchasesInLastSixMonths);
                payer.Set("transaction_count_last_24hours", builder.NumberOfTransactionsInLast24Hours);
                payer.Set("transaction_count_last_year", builder.NumberOfTransactionsInLastYear);
                payer.Set("provision_attempt_count_last_24hours", builder.NumberOfAddCardAttemptsInLast24Hours);
                payer.Set("shipping_address_creation_indicator", builder.ShippingAddressUsageIndicator?.ToString());
                payer.Set("shipping_address_creation_date", builder.ShippingAddressCreateDate?.ToString("yyyy-MM-dd"));

                // suspicious activity
                if (builder.PreviousSuspiciousActivity != null) {
                    payer.Set("suspicious_account_activity", builder.PreviousSuspiciousActivity.Value ? "SUSPICIOUS_ACTIVITY" : "NO_SUSPICIOUS_ACTIVITY");
                }

                // home phone
                if (!string.IsNullOrEmpty(builder.HomeNumber)) {
                    payer.SubElement("home_phone")
                        .Set("country_code", builder.HomeCountryCode.ToNumeric())
                        .Set("subscriber_number", builder.HomeNumber.ToNumeric());
                }

                // work phone
                if (!string.IsNullOrEmpty(builder.WorkNumber)) {
                    payer.SubElement("work_phone")
                        .Set("country_code", builder.WorkCountryCode.ToNumeric())
                        .Set("subscriber_number", builder.WorkNumber.ToNumeric());
                }

                // payer login data
                if (builder.HasPayerLoginData) {
                    request.SubElement("payer_login_data")
                        .Set("authentication_data", builder.CustomerAuthenticationData)
                        .Set("authentication_timestamp", builder.CustomerAuthenticationTimestamp?.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"))
                        .Set("authentication_type", builder.CustomerAuthenticationMethod?.ToString());
                }

                // prior authentication data
                if (builder.HasPriorAuthenticationData) {
                    request.SubElement("payer_prior_three_ds_authentication_data")
                        .Set("authentication_method", builder.PriorAuthenticationMethod?.ToString())
                        .Set("acs_transaction_id", builder.PriorAuthenticationTransactionId)
                        .Set("authentication_timestamp", builder.PriorAuthenticationTimestamp?.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"))
                        .Set("authentication_data", builder.PriorAuthenticationData);
                }

                // recurring authorization data
                if (builder.HasRecurringAuthData) {
                    request.SubElement("recurring_authorization_data")
                        .Set("max_number_of_instalments", builder.MaxNumberOfInstallments)
                        .Set("frequency", builder.RecurringAuthorizationFrequency)
                        .Set("expiry_date", builder.RecurringAuthorizationExpiryDate?.ToString("yyyy-MM-dd"));
                }

                // billing details
                Address billingAddress = builder.BillingAddress;
                if (billingAddress != null) {
                    JsonDoc billingAddressElement = payer.SubElement("billing_address");
                    billingAddressElement.Set("line1", billingAddress.StreetAddress1);
                    billingAddressElement.Set("line2", billingAddress.StreetAddress2);
                    billingAddressElement.Set("line3", billingAddress.StreetAddress3);
                    billingAddressElement.Set("city", billingAddress.City);
                    billingAddressElement.Set("postal_code", billingAddress.PostalCode);
                    billingAddressElement.Set("state", billingAddress.State);
                    billingAddressElement.Set("country", billingAddress.CountryCode);
                }

                // mobile phone
                if (!string.IsNullOrEmpty(builder.MobileNumber)) {
                    JsonDoc mobilePhone = payer.SubElement("mobile_phone");
                    mobilePhone.Set("country_code", builder.MobileCountryCode.ToNumeric());
                    mobilePhone.Set("subscriber_number", builder.MobileNumber.ToNumeric());
                }

                // browser_data
                BrowserData broswerData = builder.BrowserData;
                if (broswerData != null) {
                    JsonDoc browserDataElement = request.SubElement("browser_data");
                    browserDataElement.Set("accept_header", broswerData.AcceptHeader);
                    browserDataElement.Set("color_depth", broswerData.ColorDepth.ToString());
                    browserDataElement.Set("ip", broswerData.IpAddress);
                    browserDataElement.Set("java_enabled", broswerData.JavaEnabled);
                    browserDataElement.Set("javascript_enabled", broswerData.JavaScriptEnabled);
                    browserDataElement.Set("language", broswerData.Language);
                    browserDataElement.Set("screen_height", broswerData.ScreenHeight);
                    browserDataElement.Set("screen_width", broswerData.ScreenWidth);
                    browserDataElement.Set("challenge_window_size", broswerData.ChallengeWindowSize.ToString());
                    browserDataElement.Set("timezone", broswerData.Timezone);
                    browserDataElement.Set("user_agent", broswerData.UserAgent);
                }

                // mobile fields
                if (builder.HasMobileFields) {
                    JsonDoc sdkInformationElement = request.SubElement("sdk_information")
                        .Set("application_id", builder.ApplicationId)
                        .Set("ephemeral_public_key", builder.EphemeralPublicKey)
                        .Set("maximum_timeout", builder.MaximumTimeout.ToString().PadLeft(2, '0'))
                        .Set("reference_number", builder.ReferenceNumber)
                        .Set("sdk_trans_id", builder.SdkTransactionId)
                        .Set("encoded_data", builder.EncodedData)
                    ;

                    // device render options
                    if (builder.SdkInterface != null || builder.SdkUiTypes != null) {
                        var dro = sdkInformationElement.SubElement("device_render_options");
                        dro.Set("sdk_interface", builder.SdkInterface?.ToString());
                        if (builder.SdkUiTypes != null) {
                            var uiTypes = new List<string>();
                            foreach (var sdkuiType in builder.SdkUiTypes) {
                                uiTypes.Add(sdkuiType.ToString());
                            }
                            dro.Set("sdk_ui_type", uiTypes.ToArray());
                        }
                    }
                }

                string hash = GenerationUtils.GenerateHash(SharedSecret, timestamp, MerchantId, hashValue, secureEcom.ServerTransactionId);
                SetAuthHeader(hash);

                Request.MaskedValues = MaskedValues;

                string rawResponse = DoTransaction(HttpMethod.Post, "authentications", request.ToString());
                return MapResponse(rawResponse);
            }

            throw new ApiException(string.Format("Unknown transaction type {0}.", transType));
        }

        private void SetAuthHeader(string value) {
            if (!Headers.ContainsKey("Authorization")) {
                Headers.Add("Authorization", string.Format("securehash {0}", value));
            }
            else Headers["Authorization"] = string.Format("securehash {0}", value);
            Headers["X-GP-Version"] = "2.2.0";
        }              

        private Transaction MapResponse(string rawResponse) {
            
            JsonDoc doc = JsonDoc.Parse(rawResponse);

            ThreeDSecure secureEcom = new ThreeDSecure();

            // check enrolled
            secureEcom.ServerTransactionId = doc.GetValue<string>("server_trans_id");
            if (doc.Has("enrolled")) {
                secureEcom.Enrolled = doc.GetValue<string>("enrolled");
            }
            secureEcom.IssuerAcsUrl = doc.GetValue<string>("method_url", "challenge_request_url");

            // get authentication data
            secureEcom.AcsTransactionId = doc.GetValue<string>("acs_trans_id");
            secureEcom.DirectoryServerTransactionId = doc.GetValue<string>("ds_trans_id");
            secureEcom.AuthenticationType = doc.GetValue<string>("authentication_type");
            secureEcom.AuthenticationValue = doc.GetValue<string>("authentication_value");
            secureEcom.Eci = doc.GetValue<string>("eci");
            secureEcom.Status = doc.GetValue<string>("status");
            secureEcom.StatusReason = doc.GetValue<string>("status_reason");
            secureEcom.AuthenticationSource = doc.GetValue<string>("authentication_source");
            secureEcom.MessageCategory = doc.GetValue<string>("message_category");
            secureEcom.MessageVersion = doc.GetValue<string>("message_version");
            secureEcom.AcsInfoIndicator = doc.GetArray<string>("acs_info_indicator");
            secureEcom.DecoupledResponseIndicator = doc.GetValue<string>("decoupled_response_indicator");
            secureEcom.WhitelistStatus = doc.GetValue<string>("whitelist_status");
            secureEcom.ExemptReason = doc.GetValue<string>("eos_reason");
            if (secureEcom.ExemptReason == ExemptReason.APPLY_EXEMPTION.ToString()) {
                secureEcom.ExemptStatus = ExemptStatus.TRANSACTION_RISK_ANALYSIS;
            }

            // challenge mandated
            if (doc.Has("challenge_mandated")) {
                secureEcom.ChallengeMandated = doc.GetValue<bool>("challenge_mandated");
            }

            // initiate authentication
            secureEcom.CardHolderResponseInfo = doc.GetValue<string>("cardholder_response_info");

            // device_render_options
            if (doc.Has("device_render_options")) {
                JsonDoc renderOptions = doc.Get("device_render_options");
                secureEcom.SdkInterface = renderOptions.GetValue<string>("sdk_interface");
                secureEcom.SdkUiType = renderOptions.GetArray<string>("sdk_ui_type");
            }

            // message_extension
            if (doc.Has("message_extension")) {
                secureEcom.MessageExtensions = new List<MessageExtension>();
                foreach (JsonDoc messageExtension in doc.GetEnumerator("message_extension")) {
                    MessageExtension msgExtension = new MessageExtension
                    {
                        CriticalityIndicator = messageExtension.GetValue<string>("criticality_indicator"),
                        MessageExtensionData = messageExtension.GetValue<JsonDoc>("data")?.ToString(),
                        MessageExtensionId = messageExtension.GetValue<string>("id"),
                        MessageExtensionName = messageExtension.GetValue<string>("name")
                    };
                    secureEcom.MessageExtensions.Add(msgExtension);
                }
            }

            // versions
            secureEcom.DirectoryServerEndVersion = doc.GetValue<string>("ds_protocol_version_end");
            secureEcom.DirectoryServerStartVersion = doc.GetValue<string>("ds_protocol_version_start");
            secureEcom.AcsEndVersion = doc.GetValue<string>("acs_protocol_version_end");
            secureEcom.AcsStartVersion = doc.GetValue<string>("acs_protocol_version_start");
            
            // payer authentication request
            if (doc.Has("method_data")) {
                JsonDoc methodData = doc.Get("method_data");
                secureEcom.PayerAuthenticationRequest = methodData.GetValue<string>("encoded_method_data");
            }
            else if (doc.Has("encoded_creq")) {
                secureEcom.PayerAuthenticationRequest = doc.GetValue<string>("encoded_creq");
            }

            if (secureEcom.AuthenticationSource == AuthenticationSource.MOBILE_SDK.ToString()) {
                secureEcom.PayerAuthenticationRequest = doc.GetValue<string>("acs_signed_content") ?? null;
                secureEcom.AcsInterface = doc.Get("acs_rendering_type")?.GetValue<string>("acs_interface") ?? null;
                secureEcom.AcsUiTemplate = doc.Get("acs_rendering_type")?.GetValue<string>("acs_ui_template") ?? null;
            }
            
            secureEcom.AcsReferenceNumber = doc.GetValue<string>("acs_reference_number") ?? null;

            Transaction response = new Transaction {
                ThreeDSecure = secureEcom
            };
            return response;
        }

        private string MapCardScheme(string cardType) {
            if (cardType.Equals("MC")) {
                return "MASTERCARD";
            }
            else if (cardType.Equals("DINERSCLUB")) {
                return "DINERS";
            }
            else return cardType;
        }

        protected override string HandleResponse(GatewayResponse response) {
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent) {
                throw new GatewayException(string.Format("Status Code: {0} - {1}", response.StatusCode, response.RawResponse));
            }
            return response.RawResponse;
        }
    }
}
