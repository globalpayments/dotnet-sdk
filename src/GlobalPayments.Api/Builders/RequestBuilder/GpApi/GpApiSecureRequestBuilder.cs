using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.GpApi;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.Logging;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace GlobalPayments.Api.Builders.RequestBuilder.GpApi {
    internal class GpApiSecureRequestBuilder<T> : IRequestBuilder<SecureBuilder<T>> where T : class    {
        private static Secure3dBuilder _3dBuilder { get; set; }
        private static Dictionary<string, string> MaskedValues;
        public Request BuildRequest(SecureBuilder<T> builder, GpApiConnector gateway) {
            DisposeMaskedValues();
            if (builder is FraudBuilder<T>) {
                var merchantUrl = !string.IsNullOrEmpty(gateway.GpApiConfig.MerchantId) ? $"/merchants/{gateway.GpApiConfig.MerchantId}" : string.Empty;
                switch (builder.TransactionType) {
                    case TransactionType.RiskAssess:
                        var requestData = new JsonDoc()
                            .Set("account_name", gateway.GpApiConfig.AccessTokenInfo.RiskAssessmentAccountName)
                            .Set("account_id", gateway.GpApiConfig.AccessTokenInfo.RiskAssessmentAccountID)
                            .Set("reference", builder.ReferenceNumber ?? Guid.NewGuid().ToString())
                            .Set("source", builder.AuthenticationSource?.ToString())
                            .Set("merchant_contact_url", gateway.GpApiConfig.MerchantContactUrl)
                            .Set("order", SetOrderParam(builder))
                            .Set("payment_method", SetPaymentMethodParam(builder))
                            .Set("payer", SetPayerParam(builder))
                            .Set("payer_prior_three_ds_authentication_data", SetPayerPrior3DSAuthenticationDataParam(builder))
                            .Set("recurring_authorization_data", SetRecurringAuthorizationDataParam(builder))
                            .Set("payer_login_data", SetPayerLoginDataParam(builder))
                            .Set("browser_data", SetBrowserDataParam(builder));

                        Request.MaskedValues = MaskedValues;

                        return new Request {
                            Verb = HttpMethod.Post,
                            Endpoint = $"{merchantUrl}{GpApiRequest.RISK_ASSESSMENTS}",
                            RequestBody = requestData.ToString(),
                        };
                    default:
                        break;
                }

                return null;
            }
            else if(builder is Secure3dBuilder) {
               return BuildSecure3dBuilderRequest(builder as Secure3dBuilder, gateway);
            }

            return null;
            
        }
        private Request BuildSecure3dBuilderRequest(Secure3dBuilder builder, GpApiConnector gateway) {
            DisposeMaskedValues();
            _3dBuilder = builder;
            var merchantUrl = !string.IsNullOrEmpty(gateway.GpApiConfig.MerchantId) ? $"/merchants/{gateway.GpApiConfig.MerchantId}" : string.Empty;
            if (builder.TransactionType == TransactionType.VerifyEnrolled) {
                var storedCredential = new JsonDoc()
                    .Set("model", EnumConverter.GetMapping(Target.GP_API, builder.StoredCredential?.Type))
                    .Set("reason", EnumConverter.GetMapping(Target.GP_API, builder.StoredCredential?.Reason))
                    .Set("sequence", EnumConverter.GetMapping(Target.GP_API, builder.StoredCredential?.Sequence));

                var paymentMethod = new JsonDoc();

                if (builder.PaymentMethod is ITokenizable tokenized && !string.IsNullOrEmpty(tokenized.Token)) {
                    paymentMethod.Set("id", tokenized.Token);
                }
                else if (builder.PaymentMethod is ICardData cardData) {
                    var card = new JsonDoc()
                        .Set("number", cardData.Number)
                        .Set("expiry_month", cardData.ExpMonth?.ToString().PadLeft(2, '0'))
                        .Set("expiry_year", cardData.ExpYear?.ToString().PadLeft(4, '0').Substring(2, 2));

                    paymentMethod.Set("card", card);

                    MaskPaymentMethodSensitiveData(card);
                }

                var notifications = new JsonDoc()
                    .Set("challenge_return_url", gateway.GpApiConfig.ChallengeNotificationUrl)
                    .Set("three_ds_method_return_url", gateway.GpApiConfig.MethodNotificationUrl)
                    .Set("decoupled_notification_url", builder.DecoupledNotificationUrl);

                var data = new JsonDoc()
                    .Set("account_name", gateway.GpApiConfig.AccessTokenInfo.TransactionProcessingAccountName)
                    .Set("account_id", gateway.GpApiConfig.AccessTokenInfo.TransactionProcessingAccountID)
                    .Set("reference", builder.ReferenceNumber ?? Guid.NewGuid().ToString())
                    .Set("channel", EnumConverter.GetMapping(Target.GP_API, gateway.GpApiConfig.Channel))
                    .Set("amount", builder.Amount.ToNumericCurrencyString())
                    .Set("currency", builder.Currency)
                    .Set("country", gateway.GpApiConfig.Country)
                    .Set("preference", builder.ChallengeRequestIndicator?.ToString())
                    .Set("source", builder.AuthenticationSource.ToString())
                    .Set("initator", EnumConverter.GetMapping(Target.GP_API, builder.StoredCredential?.Initiator))
                    .Set("stored_credential", storedCredential.HasKeys() ? storedCredential : null)
                    .Set("payment_method", paymentMethod)
                    .Set("notifications", notifications.HasKeys() ? notifications : null);

                Request.MaskedValues = MaskedValues;

                return new Request {
                    Verb = HttpMethod.Post,
                    Endpoint = $"{merchantUrl}{GpApiRequest.AUTHENTICATIONS_ENDPOINT}",
                    RequestBody = data.ToString(),
                };
            }
            else if (builder.TransactionType == TransactionType.InitiateAuthentication)
            {
                #region Stored credential
                var storedCredential = new JsonDoc()
                    .Set("model", EnumConverter.GetMapping(Target.GP_API, builder.StoredCredential?.Type))
                    .Set("reason", EnumConverter.GetMapping(Target.GP_API, builder.StoredCredential?.Reason))
                    .Set("sequence", EnumConverter.GetMapping(Target.GP_API, builder.StoredCredential?.Sequence));
                #endregion

                #region Payment method
                var paymentMethod = new JsonDoc();

                if (builder.PaymentMethod is ITokenizable tokenized && !string.IsNullOrEmpty(tokenized.Token)) {
                    paymentMethod.Set("id", tokenized.Token);
                }
                else if (builder.PaymentMethod is ICardData cardData) {
                    var card = new JsonDoc()
                        .Set("number", cardData.Number)
                        .Set("expiry_month", cardData.ExpMonth?.ToString().PadLeft(2, '0'))
                        .Set("expiry_year", cardData.ExpYear?.ToString().PadLeft(4, '0').Substring(2, 2));

                    paymentMethod.Set("card", card);

                    MaskPaymentMethodSensitiveData(card);
                }
                #endregion

                #region Notifications
                var notifications = new JsonDoc()
                    .Set("challenge_return_url", gateway.GpApiConfig.ChallengeNotificationUrl)
                    .Set("three_ds_method_return_url", gateway.GpApiConfig.MethodNotificationUrl)
                    .Set("decoupled_notification_url", builder.DecoupledNotificationUrl);
                #endregion               

                #region Payer
                var homePhone = new JsonDoc()
                    .Set("country_code", builder.HomeCountryCode)
                    .Set("subscriber_number", builder.HomeNumber);

                var workPhone = new JsonDoc()
                    .Set("country_code", builder.WorkCountryCode)
                    .Set("subscriber_number", builder.WorkNumber);

                var payer = new JsonDoc()
                    .Set("reference", builder.CustomerAccountId) //ToDo: Confirm
                    .Set("account_age", builder.AccountAgeIndicator?.ToString())
                    .Set("account_creation_date", builder.AccountCreateDate?.ToString("yyyy-MM-dd"))
                    .Set("account_change_date", builder.AccountChangeDate?.ToString("yyyy-MM-dd"))
                    .Set("account_change_indicator", builder.AccountChangeIndicator?.ToString())
                    .Set("account_password_change_date", builder.PasswordChangeDate?.ToString("yyyy-MM-dd"))
                    .Set("account_password_change_indicator", builder.PasswordChangeIndicator?.ToString())
                    .Set("home_phone", homePhone.HasKeys() ? homePhone : null)
                    .Set("work_phone", workPhone.HasKeys() ? workPhone : null)
                    .Set("payment_account_creation_date", builder.PaymentAccountCreateDate?.ToString("yyyy-MM-dd"))
                    .Set("payment_account_age_indicator", builder.PaymentAgeIndicator?.ToString())
                    .Set("suspicious_account_activity", builder.PreviousSuspiciousActivity)
                    .Set("purchases_last_6months_count", builder.NumberOfPurchasesInLastSixMonths)
                    .Set("transactions_last_24hours_count", builder.NumberOfTransactionsInLast24Hours)
                    .Set("transaction_last_year_count", builder.NumberOfTransactionsInLastYear)
                    .Set("provision_attempt_last_24hours_count", builder.NumberOfAddCardAttemptsInLast24Hours)
                    .Set("shipping_address_time_created_reference", builder.ShippingAddressCreateDate?.ToString("yyyy-MM-dd"))
                    .Set("shipping_address_creation_indicator", builder.ShippingAddressUsageIndicator?.ToString());
                #endregion

                #region Payer prior 3DS authentication data
                var payerPrior3DSAuthenticationData = new JsonDoc()
                    .Set("authentication_method", builder.PriorAuthenticationMethod?.ToString())
                    .Set("acs_transaction_reference", builder.PriorAuthenticationTransactionId)
                    .Set("authentication_timestamp", builder.PriorAuthenticationTimestamp?.ToString("yyyy-MM-ddThh:mm:ss.fffZ"))
                    .Set("authentication_data", builder.PriorAuthenticationData);
                #endregion

                #region Recurring authorization data
                var recurringAuthorizationData = new JsonDoc()
                    .Set("max_number_of_instalments", builder.MaxNumberOfInstallments)
                    .Set("frequency", builder.RecurringAuthorizationFrequency)
                    .Set("expiry_date", builder.RecurringAuthorizationExpiryDate?.ToString("yyyy-MM-dd"));
                #endregion

                #region Payer login data
                var payerLoginData = new JsonDoc()
                    .Set("authentication_data", builder.CustomerAuthenticationData)
                    .Set("authentication_timestamp", builder.CustomerAuthenticationTimestamp?.ToString("yyyy-MM-ddThh:mm:ss.fffZ"))
                    .Set("authentication_type", builder.CustomerAuthenticationMethod?.ToString());
                #endregion

                #region Browser data
                var browserData = new JsonDoc()
                    .Set("accept_header", builder.BrowserData?.AcceptHeader)
                    .Set("color_depth", builder.BrowserData?.ColorDepth.ToString())
                    .Set("ip", builder.BrowserData?.IpAddress)
                    .Set("java_enabled", builder.BrowserData?.JavaEnabled)
                    .Set("javascript_enabled", builder.BrowserData?.JavaScriptEnabled)
                    .Set("language", builder.BrowserData?.Language)
                    .Set("screen_height", builder.BrowserData?.ScreenHeight)
                    .Set("screen_width", builder.BrowserData?.ScreenWidth)
                    .Set("challenge_window_size", builder.BrowserData?.ChallengeWindowSize.ToString())
                    .Set("timezone", builder.BrowserData?.Timezone)
                    .Set("user_agent", builder.BrowserData?.UserAgent);
                #endregion

                #region MobileData
                string[] ModifySdkUiTypes() {
                    string[] result = new string[(int)builder.MobileData?.SdkUiTypes.Length];
                    for (int i = 0; i < builder.MobileData?.SdkUiTypes.Length; i++) {
                        result[i] = EnumConverter.GetMapping(Target.GP_API, builder.MobileData?.SdkUiTypes[i]);
                    }
                    return result;
                }

                var mobileData = new JsonDoc()
                    .Set("encoded_data", builder.MobileData?.EncodedData)
                    .Set("application_reference", builder.MobileData?.ApplicationReference)
                    .Set("sdk_interface", builder.MobileData?.SdkInterface.ToString())
                    .Set("sdk_ui_type", builder.MobileData != null && builder.MobileData?.SdkUiTypes.Length > 0 ? ModifySdkUiTypes() : null)
                    .Set("ephemeral_public_key", builder.MobileData?.EphemeralPublicKey)
                    .Set("maximum_timeout", builder.MobileData?.MaximumTimeout)
                    .Set("reference_number", builder.MobileData?.ReferenceNumber)
                    .Set("sdk_trans_reference", builder.MobileData?.SdkTransReference);
                #endregion

                #region ThreeDS
                var threeDS = new JsonDoc()
                    .Set("source", builder.AuthenticationSource.ToString())
                    .Set("preference", builder.ChallengeRequestIndicator?.ToString())
                    .Set("message_version", builder.MessageVersion?.ToString());
                #endregion

                var data = new JsonDoc()
                    .Set("three_ds", threeDS.HasKeys() ? threeDS : null)
                    .Set("initator", EnumConverter.GetMapping(Target.GP_API, builder.StoredCredential?.Initiator))
                    .Set("stored_credential", storedCredential.HasKeys() ? storedCredential : null)
                    .Set("method_url_completion_status", builder.MethodUrlCompletion.ToString())
                    .Set("payment_method", paymentMethod.HasKeys() ? paymentMethod : null)
                    .Set("notifications", notifications.HasKeys() ? notifications : null);
                if (builder.DecoupledFlowRequest.HasValue) {
                    data.Set("decoupled_flow_request", builder.DecoupledFlowRequest.Value ? DecoupledFlowRequest.DECOUPLED_PREFERRED.ToString() : DecoupledFlowRequest.DO_NOT_USE_DECOUPLED.ToString());
                }
                data.Set("decoupled_flow_timeout", builder.DecoupledFlowTimeout.HasValue ? builder.DecoupledFlowTimeout.ToString() : null)
                    .Set("order", SetOrderParam(_3dBuilder as SecureBuilder<T>, true))
                    .Set("payer", payer.HasKeys() ? payer : null)
                    .Set("payer_prior_three_ds_authentication_data", payerPrior3DSAuthenticationData.HasKeys() ? payerPrior3DSAuthenticationData : null)
                    .Set("recurring_authorization_data", recurringAuthorizationData.HasKeys() ? recurringAuthorizationData : null)
                    .Set("payer_login_data", payerLoginData.HasKeys() ? payerLoginData : null)
                    .Set("browser_data", browserData.HasKeys() && builder.AuthenticationSource != AuthenticationSource.MOBILE_SDK ? browserData : null)
                    .Set("mobile_data", mobileData.HasKeys() && builder.AuthenticationSource == AuthenticationSource.MOBILE_SDK ? mobileData : null)
                    .Set("merchant_contact_url", gateway.GpApiConfig.MerchantContactUrl);

                Request.MaskedValues = MaskedValues;

                return new Request {
                    Verb = HttpMethod.Post,
                    Endpoint = $"{merchantUrl}{GpApiRequest.AUTHENTICATIONS_ENDPOINT}/{builder.ServerTransactionId}/initiate",
                    RequestBody = data.ToString(),
                };
            }
            else if (builder.TransactionType == TransactionType.VerifySignature) {
                JsonDoc data = null;
                if (!string.IsNullOrEmpty(builder.PayerAuthenticationResponse)) {
                    data = new JsonDoc().Set("three_ds",
                        new JsonDoc().Set("challenge_result_value", builder.PayerAuthenticationResponse)
                    );
                }

                return new Request {
                    Verb = HttpMethod.Post,
                    Endpoint = $"{merchantUrl}{GpApiRequest.AUTHENTICATIONS_ENDPOINT}/{builder.ServerTransactionId}/result",
                    RequestBody = data?.ToString()
                };
            }
            else if (builder.TransactionType == TransactionType.RiskAssess) {
                JsonDoc threeDS = null;
                threeDS.Set("account_name", gateway.GpApiConfig.AccessTokenInfo.TransactionProcessingAccountName)
                   .Set("reference", builder.ReferenceNumber ?? Guid.NewGuid().ToString())
                   .Set("source", builder.AuthenticationSource.ToString())
                   .Set("merchant_contact_url", gateway.GpApiConfig.MerchantContactUrl)
                   .Set("order", SetOrderParam(_3dBuilder as SecureBuilder<T>));

                return new Request {
                    Verb = HttpMethod.Post,
                    Endpoint = $"{merchantUrl}{GpApiRequest.RISK_ASSESSMENTS}",
                    RequestBody = threeDS.ToString(),
                };

            }
            return null;
        }

        private void MaskPaymentMethodSensitiveData(JsonDoc card)
        {
            var maskedValue = new Dictionary<string, string>();
            maskedValue.Add("payment_method.card.expiry_month", card.GetValue<string>("expiry_month"));
            maskedValue.Add("payment_method.card.expiry_year", card.GetValue<string>("expiry_year"));

            MaskedValues = ProtectSensitiveData.HideValues(maskedValue);
            MaskedValues = ProtectSensitiveData.HideValue("payment_method.card.number", card.GetValue<string>("number"), 4, 6);
        }

        private static JsonDoc InitiateAuthenticationData(GpApiConfig config)
        {            
            #region ThreeDS
            var threeDS = new JsonDoc()
                .Set("source", _3dBuilder.AuthenticationSource.ToString())
                .Set("preference", _3dBuilder.ChallengeRequestIndicator?.ToString())
                .Set("message_version", _3dBuilder.MessageVersion?.ToString())
                .Set("message_category", EnumConverter.GetMapping(Target.GP_API, _3dBuilder.MessageCategory));
            #endregion

            var data = new JsonDoc()
                .Set("three_ds", threeDS.HasKeys() ? threeDS : null)
                .Set("initator", EnumConverter.GetMapping(Target.GP_API, _3dBuilder.StoredCredential?.Initiator))
                .Set("stored_credential", SetStoreCredentialParam())
                .Set("method_url_completion_status", _3dBuilder.MethodUrlCompletion.ToString())
                .Set("merchant_contact_url", config.MerchantContactUrl)
                .Set("order", SetOrderParam(_3dBuilder as SecureBuilder<T>))
                .Set("payment_method", SetPaymentMethodParam(_3dBuilder as SecureBuilder<T>))
                .Set("payer", SetPayerParam(_3dBuilder as SecureBuilder<T>))
                .Set("payer_prior_three_ds_authentication_data", SetPayerPrior3DSAuthenticationDataParam(_3dBuilder as SecureBuilder<T>))
                .Set("recurring_authorization_data", SetRecurringAuthorizationDataParam(_3dBuilder as SecureBuilder<T>))
                .Set("payer_login_data", SetPayerLoginDataParam(_3dBuilder as SecureBuilder<T>))
                .Set("browser_data", _3dBuilder.BrowserData != null && _3dBuilder.AuthenticationSource != AuthenticationSource.MOBILE_SDK ? SetBrowserDataParam(_3dBuilder as SecureBuilder<T>) : null)
                .Set("mobile_data", _3dBuilder.MobileData != null && _3dBuilder.AuthenticationSource == AuthenticationSource.MOBILE_SDK ? SetMobileDataParam() : null);
                
            
            var notifications = new JsonDoc()
                .Set("challenge_return_url", config.ChallengeNotificationUrl)
                .Set("three_ds_method_return_url", config.MethodNotificationUrl)
                .Set("decoupled_notification_url", _3dBuilder.DecoupledNotificationUrl);

            data.Set("notifications", notifications.HasKeys() ? notifications : null);
            if (_3dBuilder.DecoupledFlowRequest.HasValue) {
                data.Set("decoupled_flow_request", _3dBuilder.DecoupledFlowRequest.Value ? DecoupledFlowRequest.DECOUPLED_PREFERRED.ToString() : DecoupledFlowRequest.DO_NOT_USE_DECOUPLED.ToString());
            }
            data.Set("decoupled_flow_timeout", _3dBuilder.DecoupledFlowTimeout.HasValue ? _3dBuilder.DecoupledFlowTimeout.ToString() : null);

            return data;           
        }

        private static JsonDoc SetMobileDataParam()
        {
            string[] ModifySdkUiTypes() {
                string[] result = new string[(int)_3dBuilder.MobileData?.SdkUiTypes.Length];
                for (int i = 0; i < _3dBuilder.MobileData?.SdkUiTypes.Length; i++) {
                    result[i] = EnumConverter.GetMapping(Target.GP_API, _3dBuilder.MobileData?.SdkUiTypes[i]);

                }
                return result;
            }

            var mobileData = new JsonDoc()
                .Set("encoded_data", _3dBuilder.MobileData?.EncodedData)
                .Set("application_reference", _3dBuilder.MobileData?.ApplicationReference)
                .Set("sdk_interface", _3dBuilder.MobileData?.SdkInterface.ToString())
                .Set("sdk_ui_type", _3dBuilder.MobileData != null && _3dBuilder.MobileData?.SdkUiTypes.Length > 0 ? ModifySdkUiTypes() : null)
                .Set("ephemeral_public_key", _3dBuilder.MobileData?.EphemeralPublicKey)
                .Set("maximum_timeout", _3dBuilder.MobileData?.MaximumTimeout)
                .Set("reference_number", _3dBuilder.MobileData?.ReferenceNumber)
                .Set("sdk_trans_reference", _3dBuilder.MobileData?.SdkTransReference);

            return mobileData.HasKeys() ? mobileData : null;
        }

        private static JsonDoc SetBrowserDataParam(SecureBuilder<T> builder)
        {
            var browserData = new JsonDoc()
                .Set("accept_header", builder.BrowserData?.AcceptHeader)
                .Set("color_depth", builder.BrowserData?.ColorDepth.ToString())
                .Set("ip", builder.BrowserData?.IpAddress)
                .Set("java_enabled", builder.BrowserData?.JavaEnabled)
                .Set("javascript_enabled", builder.BrowserData?.JavaScriptEnabled)
                .Set("language", builder.BrowserData?.Language)
                //.Set("screen_height", builder.BrowserData?.ScreenHeight)
                //.Set("screen_width", builder.BrowserData?.ScreenWidth)
                .Set("challenge_window_size", builder.BrowserData?.ChallengeWindowSize.ToString())
                .Set("timezone", builder.BrowserData?.Timezone)
                .Set("user_agent", builder.BrowserData?.UserAgent);

            return browserData.HasKeys() ? browserData : null;
        }

        private static JsonDoc SetPayerLoginDataParam(SecureBuilder<T> builder)
        {
            var payerLoginData = new JsonDoc()
                .Set("authentication_data", builder.CustomerAuthenticationData)
                .Set("authentication_timestamp", builder.CustomerAuthenticationTimestamp?.ToString("yyyy-MM-ddThh:mm:ss.fffZ"))
                .Set("authentication_type", builder.CustomerAuthenticationMethod?.ToString());

            return payerLoginData.HasKeys() ? payerLoginData : null;
        }

        private static JsonDoc SetRecurringAuthorizationDataParam(SecureBuilder<T> builder)
        {
            var recurringAuthorizationData = new JsonDoc()
                .Set("max_number_of_instalments", builder.MaxNumberOfInstallments)
                .Set("frequency", builder.RecurringAuthorizationFrequency)
                .Set("expiry_date", builder.RecurringAuthorizationExpiryDate?.ToString("yyyy-MM-dd"));

            return recurringAuthorizationData.HasKeys() ? recurringAuthorizationData : null;
        }

        private static JsonDoc SetPayerPrior3DSAuthenticationDataParam(SecureBuilder<T> builder)
        {
            var payerPrior3DSAuthenticationData = new JsonDoc()
                .Set("authentication_method", builder.PriorAuthenticationMethod?.ToString())
                .Set("acs_transaction_reference", builder.PriorAuthenticationTransactionId)
                .Set("authentication_timestamp", builder.PriorAuthenticationTimestamp?.ToString("yyyy-MM-ddThh:mm:ss.fffZ"))
                .Set("authentication_data", builder.PriorAuthenticationData);

            return payerPrior3DSAuthenticationData.HasKeys() ? payerPrior3DSAuthenticationData : null;
        }

        private static JsonDoc SetPayerParam(SecureBuilder<T> builder) {
            
            var homePhone = new JsonDoc()
                .Set("country_code", builder.HomeCountryCode)
                .Set("subscriber_number", builder.HomeNumber);

            var workPhone = new JsonDoc()
                .Set("country_code", builder.WorkCountryCode)
                .Set("subscriber_number", builder.WorkNumber);

            var mobilePhone = new JsonDoc()
                .Set("country_code", builder.MobileCountryCode)
                .Set("subscriber_number", builder.MobileNumber);

            var payer = new JsonDoc()
                .Set("reference", builder.CustomerAccountId)
                .Set("account_age", builder.AccountAgeIndicator?.ToString())
                .Set("account_creation_date", builder.AccountCreateDate?.ToString("yyyy-MM-dd"))
                .Set("account_change_date", builder.AccountChangeDate?.ToString("yyyy-MM-dd"))
                .Set("account_change_indicator", builder.AccountChangeIndicator?.ToString())
                .Set("account_password_change_date", builder.PasswordChangeDate?.ToString("yyyy-MM-dd"))
                .Set("account_password_change_indicator", builder.PasswordChangeIndicator?.ToString())
                .Set("home_phone", homePhone.HasKeys() ? homePhone : null)
                .Set("work_phone", workPhone.HasKeys() ? workPhone : null)
                .Set("mobile_phone", mobilePhone.HasKeys() ? mobilePhone : null)
                .Set("payment_account_creation_date", builder.PaymentAccountCreateDate?.ToString("yyyy-MM-dd"))
                .Set("payment_account_age_indicator", builder.PaymentAgeIndicator?.ToString())
                .Set("suspicious_account_activity", builder.PreviousSuspiciousActivity)
                .Set("purchases_last_6months_count", builder.NumberOfPurchasesInLastSixMonths)
                .Set("transactions_last_24hours_count", builder.NumberOfTransactionsInLast24Hours)
                .Set("transaction_last_year_count", builder.NumberOfTransactionsInLastYear)
                .Set("provision_attempt_last_24hours_count", builder.NumberOfAddCardAttemptsInLast24Hours)
                .Set("shipping_address_time_created_reference", builder.ShippingAddressCreateDate?.ToString("yyyy-MM-ddThh:mm:ss"))
                .Set("shipping_address_creation_indicator", builder.ShippingAddressUsageIndicator?.ToString());

            if(builder.BillingAddress != null) {
                var billingAddress = new JsonDoc()
                    .Set("line1", builder.BillingAddress.StreetAddress1)
                    .Set("line2", builder.BillingAddress.StreetAddress2)
                    .Set("line3", builder.BillingAddress.StreetAddress3)
                    .Set("city", builder.BillingAddress.City)
                    .Set("postal_code", builder.BillingAddress.PostalCode)
                    .Set("state", builder.BillingAddress.State)
                    .Set("country", CountryUtils.GetNumericCodeByCountry(builder.BillingAddress.CountryCode));

                payer.Set("billing_address", billingAddress);
            }

            return payer.HasKeys() ? payer : null;
           
        }

        private static JsonDoc SetOrderParam(SecureBuilder<T> builder, bool isSecure3dBuilder = false)
        {
            var order = new JsonDoc()
                .Set("time_created_reference", builder.OrderCreateDate?.ToString("yyyy-MM-ddThh:mm:ss.fffZ"))
                .Set("amount", builder.Amount.ToNumericCurrencyString())
                .Set("currency", builder.Currency)
                .Set("reference", builder.OrderId ?? Guid.NewGuid().ToString())
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
                .Set("category", builder.OrderTransactionType.ToString());
            

            if (builder.ShippingAddress != null) {
                var shippingAddress = new JsonDoc()
                    .Set("line1", builder.ShippingAddress.StreetAddress1)
                    .Set("line2", builder.ShippingAddress.StreetAddress2)
                    .Set("line3", builder.ShippingAddress.StreetAddress3)
                    .Set("city", builder.ShippingAddress.City)
                    .Set("postal_code", builder.ShippingAddress.PostalCode)
                    .Set("state", builder.ShippingAddress.State)
                    .Set("country", isSecure3dBuilder ? builder.ShippingAddress.CountryCode : CountryUtils.GetCountryCodeByCountry(builder.ShippingAddress.CountryCode));

                order.Set("shipping_address", shippingAddress);
            }

            return order.HasKeys() ? order : null;
        }
       

        private static JsonDoc VerifyEnrolled(GpApiConfig config)
        {            
            var notifications = new JsonDoc()
                .Set("challenge_return_url", config.ChallengeNotificationUrl)
                .Set("three_ds_method_return_url", config.MethodNotificationUrl)
                .Set("decoupled_notification_url", _3dBuilder.DecoupledNotificationUrl);

            var threeDS = new JsonDoc()
                .Set("account_name", config.AccessTokenInfo.TransactionProcessingAccountName)
                .Set("account_id", config.AccessTokenInfo.TransactionProcessingAccountID)
                .Set("channel", EnumConverter.GetMapping(Target.GP_API, config.Channel))
                .Set("country", config.Country)
                .Set("reference", _3dBuilder.ReferenceNumber ?? Guid.NewGuid().ToString())                
                .Set("amount", _3dBuilder.Amount.ToNumericCurrencyString())
                .Set("currency", _3dBuilder.Currency)                
                .Set("preference", _3dBuilder.ChallengeRequestIndicator?.ToString())
                .Set("source", _3dBuilder.AuthenticationSource.ToString())
                .Set("payment_method", SetPaymentMethodParam(_3dBuilder as SecureBuilder<T>))
                .Set("notifications", notifications.HasKeys() ? notifications : null)
                .Set("initator", EnumConverter.GetMapping(Target.GP_API, _3dBuilder.StoredCredential?.Initiator))
                .Set("stored_credential", SetStoreCredentialParam());           

            return threeDS;
        }

        private static JsonDoc SetPaymentMethodParam(SecureBuilder<T> builder, bool is3DSecure = false)
        {
            var paymentMethod = new JsonDoc();

            if (builder.PaymentMethod is ITokenizable tokenized && !string.IsNullOrEmpty(tokenized.Token)) {
                paymentMethod.Set("id", tokenized.Token);
            }
            else if (builder.PaymentMethod is ICardData cardData) {
                var card = new JsonDoc()
                    .Set("brand", cardData.CardType.ToUpper())
                    .Set("number", cardData.Number)
                    .Set("expiry_month", cardData.ExpMonth?.ToString().PadLeft(2, '0'))
                    .Set("expiry_year", cardData.ExpYear?.ToString().PadLeft(4, '0').Substring(2, 2));

                paymentMethod.Set("card", card)
                .Set("name", !string.IsNullOrEmpty(cardData.CardHolderName) ? cardData.CardHolderName : null);

                var maskedValue = new Dictionary<string, string>();
                maskedValue.Add("payment_method.card.expiry_month", card.GetValue<string>("expiry_month"));
                maskedValue.Add("payment_method.card.expiry_year", card.GetValue<string>("expiry_year"));

                MaskedValues = ProtectSensitiveData.HideValues(maskedValue);
                MaskedValues = ProtectSensitiveData.HideValue("payment_method.card.number", cardData.Number, 4, 6);
            }

            return paymentMethod;            
        }

        private static JsonDoc SetStoreCredentialParam()
        {
            var storedCredential = new JsonDoc()
                    .Set("model", EnumConverter.GetMapping(Target.GP_API, _3dBuilder.StoredCredential?.Type))
                    .Set("reason", EnumConverter.GetMapping(Target.GP_API, _3dBuilder.StoredCredential?.Reason))
                    .Set("sequence", EnumConverter.GetMapping(Target.GP_API, _3dBuilder.StoredCredential?.Sequence));

            return storedCredential.HasKeys() ? storedCredential : null;            
        }

        private static void DisposeMaskedValues()
        {
            MaskedValues = null;
            ProtectSensitiveData.DisposeCollection();
        }
    }
}
