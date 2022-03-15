using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System;
using System.Net.Http;

namespace GlobalPayments.Api.Entities {
    internal class GpApiSecure3DRequestBuilder {
        internal static GpApiRequest BuildRequest(Secure3dBuilder builder, GpApiConnector gateway) {
            var merchantUrl = !string.IsNullOrEmpty(gateway.MerchantId) ? $"/merchants/{gateway.MerchantId}" : string.Empty;
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
                }

                var notifications = new JsonDoc()
                    .Set("challenge_return_url", gateway.ChallengeNotificationUrl)
                    .Set("three_ds_method_return_url", gateway.MethodNotificationUrl);

                var data = new JsonDoc()
                    .Set("account_name", gateway.TransactionProcessingAccountName)
                    .Set("reference", builder.ReferenceNumber ?? Guid.NewGuid().ToString())
                    .Set("channel", EnumConverter.GetMapping(Target.GP_API, gateway.Channel))
                    .Set("amount", builder.Amount.ToNumericCurrencyString())
                    .Set("currency", builder.Currency)
                    .Set("country", gateway.Country)
                    .Set("preference", builder.ChallengeRequestIndicator?.ToString())
                    .Set("source", builder.AuthenticationSource.ToString())
                    .Set("initator", EnumConverter.GetMapping(Target.GP_API, builder.StoredCredential?.Initiator))
                    .Set("stored_credential", storedCredential.HasKeys() ? storedCredential : null)
                    .Set("payment_method", paymentMethod)
                    .Set("notifications", notifications.HasKeys() ? notifications : null);

                return new GpApiRequest {
                    Verb = HttpMethod.Post,
                    Endpoint = $"{merchantUrl}/authentications",
                    RequestBody = data.ToString(),
                };
            }
            else if (builder.TransactionType == TransactionType.InitiateAuthentication) {
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
                }
                #endregion

                #region Notifications
                var notifications = new JsonDoc()
                    .Set("challenge_return_url", gateway.ChallengeNotificationUrl)
                    .Set("three_ds_method_return_url", gateway.MethodNotificationUrl);
                #endregion

                #region Order
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
                    for (int i = 0; i < builder.MobileData?.SdkUiTypes.Length; i++)
                    {
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
                    .Set("notifications", notifications.HasKeys() ? notifications : null)
                    .Set("order", order.HasKeys() ? order : null)
                    .Set("payer", payer.HasKeys() ? payer : null)
                    .Set("payer_prior_three_ds_authentication_data", payerPrior3DSAuthenticationData.HasKeys() ? payerPrior3DSAuthenticationData : null)
                    .Set("recurring_authorization_data", recurringAuthorizationData.HasKeys() ? recurringAuthorizationData : null)
                    .Set("payer_login_data", payerLoginData.HasKeys() ? payerLoginData : null)
                    .Set("browser_data", browserData.HasKeys() && builder.AuthenticationSource != AuthenticationSource.MOBILE_SDK ? browserData : null)
                    .Set("mobile_data", mobileData.HasKeys() && builder.AuthenticationSource == AuthenticationSource.MOBILE_SDK ? mobileData : null)
                    .Set("merchant_contact_url", gateway.MerchantContactUrl);

                return new GpApiRequest {
                    Verb = HttpMethod.Post,
                    Endpoint = $"{merchantUrl}/authentications/{builder.ServerTransactionId}/initiate",
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

                return new GpApiRequest {
                    Verb = HttpMethod.Post,
                    Endpoint = $"{merchantUrl}/authentications/{builder.ServerTransactionId}/result",
                    RequestBody = data?.ToString()
                };
            }
            return null;
        }
    }
}
