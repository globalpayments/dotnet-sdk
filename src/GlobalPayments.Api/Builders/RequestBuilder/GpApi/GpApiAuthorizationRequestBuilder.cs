using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.GpApi;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.Logging;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.PaymentMethods.PaymentInterfaces;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace GlobalPayments.Api.Builders.RequestBuilder.GpApi {
    internal class GpApiAuthorizationRequestBuilder : IRequestBuilder<AuthorizationBuilder> {
        private static AuthorizationBuilder Builder;
        private static Dictionary<string, string> MaskedValues;

        public Request BuildRequest(AuthorizationBuilder builder, GpApiConnector gateway) {
            DisposeMaskedValues();
            Builder = builder;
            var merchantUrl = !string.IsNullOrEmpty(gateway.GpApiConfig.MerchantId) ? $"/merchants/{gateway.GpApiConfig.MerchantId}" : string.Empty;
            var paymentMethod = new JsonDoc()
                .Set("entry_mode", GetEntryMode(builder, gateway.GpApiConfig.Channel)); // [MOTO, ECOM, IN_APP, CHIP, SWIPE, MANUAL, CONTACTLESS_CHIP, CONTACTLESS_SWIPE]
            paymentMethod.Set("narrative", !string.IsNullOrEmpty(builder.DynamicDescriptor) ? builder.DynamicDescriptor : null);
            if (builder.PaymentMethod is CreditCardData && (builder.TransactionModifier == TransactionModifier.EncryptedMobile || builder.TransactionModifier == TransactionModifier.DecryptedMobile))
            {
                var digitalWallet = new JsonDoc();
                var creditCardData = (builder.PaymentMethod as CreditCardData);
                //Digital Wallet
                if (builder.TransactionModifier == TransactionModifier.EncryptedMobile) {
                    var payment_token = new JsonDoc();
                    switch (creditCardData.MobileType) {
                        case EncyptedMobileType.CLICK_TO_PAY:
                            payment_token.Set("data", creditCardData.Token);
                            break;
                        default:
                            payment_token = JsonDoc.Parse(creditCardData.Token);
                            break;
                    }
                    digitalWallet
                            .Set("payment_token", payment_token);

                }
                else if (builder.TransactionModifier == TransactionModifier.DecryptedMobile) {
                    var tokenFormat = DigitalWalletTokenFormat.CARD_NUMBER;
                    digitalWallet
                        .Set("token", creditCardData.Token)
                        .Set("token_format", DigitalWalletTokenFormat.CARD_NUMBER)
                        .Set("expiry_month", creditCardData.ExpMonth.HasValue ? creditCardData.ExpMonth.ToString().PadLeft(2, '0') : null)
                        .Set("expiry_year", creditCardData.ExpYear.HasValue ? creditCardData.ExpYear.ToString().PadLeft(4, '0').Substring(2, 2) : null)
                        .Set("cryptogram", creditCardData.Cryptogram)
                        .Set("eci", creditCardData.Eci);
                }
                digitalWallet.Set("provider", (builder.PaymentMethod as CreditCardData).MobileType);
                paymentMethod.Set("digital_wallet", digitalWallet);
            }
            else
            {
                if (builder.PaymentMethod is ICardData) {
                    var cardData = builder.PaymentMethod as ICardData;

                    var card = new JsonDoc()
                    .Set("number", cardData.Number)
                    .Set("expiry_month", cardData.ExpMonth.HasValue ? cardData.ExpMonth.ToString().PadLeft(2, '0') : null)
                    .Set("expiry_year", cardData.ExpYear.HasValue ? cardData.ExpYear.ToString().PadLeft(4, '0').Substring(2, 2) : null)
                    //.Set("track", "")
                    .Set("tag", builder.TagData)
                    .Set("cvv", cardData.Cvn)
                    .Set("avs_address", builder.BillingAddress?.StreetAddress1)
                    .Set("avs_postal_code", builder.BillingAddress?.PostalCode)
                    .Set("authcode", builder.OfflineAuthCode)
                    .Set("brand_reference", builder.CardBrandTransactionId);

                    card.Set("chip_condition", EnumConverter.GetMapping(Target.GP_API, builder.EmvChipCondition)); // [PREV_SUCCESS, PREV_FAILED]

                    if (!(builder.TransactionType == TransactionType.Tokenize || builder.TransactionType == TransactionType.Verify)) {
                        card.Set("cvv_indicator", cardData.CvnPresenceIndicator != 0 ? EnumConverter.GetMapping(Target.GP_API, cardData.CvnPresenceIndicator) : null); // [ILLEGIBLE, NOT_PRESENT, PRESENT]
                        card.Set("funding", builder.PaymentMethod?.PaymentMethodType == PaymentMethodType.Debit ? "DEBIT" : "CREDIT"); // [DEBIT, CREDIT]
                    }

                    var maskedValue = new Dictionary<string, string>();
                    maskedValue.Add("payment_method.card.expiry_month", card.GetValue<string>("expiry_month"));
                    maskedValue.Add("payment_method.card.expiry_year", card.GetValue<string>("expiry_year"));
                    maskedValue.Add("payment_method.card.cvv", card.GetValue<string>("cvv"));
                    MaskedValues = ProtectSensitiveData.HideValues(maskedValue);

                    MaskedValues = ProtectSensitiveData.HideValue("payment_method.card.number", card.GetValue<string>("number"), 4, 6);

                    var hasToken = builder.PaymentMethod is ITokenizable tokenData && !string.IsNullOrEmpty(tokenData.Token);

                    if (!hasToken) {
                        paymentMethod.Set("card", card);
                    }
                    //Brand reference when card was tokenized
                    else {
                        JsonDoc brand = new JsonDoc()
                            .Set("brand_reference", builder.CardBrandTransactionId);
                        if (brand.HasKeys()) {
                            paymentMethod.Set("card", brand);
                        }
                    }


                    if (builder.TransactionType == TransactionType.Tokenize) {
                        var tokenizationData = new JsonDoc()
                            .Set("account_name", gateway.GpApiConfig.AccessTokenInfo.TokenizationAccountName)
                            .Set("reference", builder.ClientTransactionId ?? Guid.NewGuid().ToString())
                            .Set("usage_mode", EnumConverter.GetMapping(Target.GP_API, builder.PaymentMethodUsageMode))
                            .Set("card", card);

                        var maskedVValue = new Dictionary<string, string>();
                        maskedVValue.Add("card.expiry_month", card.GetValue<string>("expiry_month"));
                        maskedVValue.Add("card.expiry_year", card.GetValue<string>("expiry_year"));
                        maskedVValue.Add("card.cvv", card.GetValue<string>("cvv"));

                        DisposeMaskedValues();
                        MaskedValues = ProtectSensitiveData.HideValues(maskedVValue);
                        MaskedValues = ProtectSensitiveData.HideValue("card.number", card.GetValue<string>("number"), 4, 6);

                        Request.MaskedValues = MaskedValues;

                        return new Request {
                            Verb = HttpMethod.Post,
                            Endpoint = $"{merchantUrl}{GpApiRequest.PAYMENT_METHODS_ENDPOINT}",
                            RequestBody = tokenizationData.ToString(),
                        };
                    }
                    else if (builder.TransactionType == TransactionType.DccRateLookup)
                    {
                        // tokenized payment method
                        if (builder.PaymentMethod is ITokenizable) {
                            string token = ((ITokenizable)builder.PaymentMethod).Token;
                            if (!string.IsNullOrEmpty(token))
                            {
                                paymentMethod.Set("id", token);
                            }
                        }

                        var RequestData = new JsonDoc()
                           .Set("account_name", gateway.GpApiConfig.AccessTokenInfo.TransactionProcessingAccountName)
                           .Set("account_id", gateway.GpApiConfig.AccessTokenInfo.TransactionProcessingAccountID)
                           .Set("channel", EnumConverter.GetMapping(Target.GP_API, gateway.GpApiConfig.Channel))
                           .Set("reference", builder.ClientTransactionId ?? Guid.NewGuid().ToString())
                           .Set("amount", builder.Amount.ToNumericCurrencyString())
                           .Set("currency", builder.Currency)
                           .Set("country", gateway.GpApiConfig.Country)
                           .Set("payment_method", paymentMethod);

                        Request.MaskedValues = MaskedValues;

                        return new Request {
                            Verb = HttpMethod.Post,
                            Endpoint = $"{merchantUrl}{GpApiRequest.DCC_ENDPOINT}",
                            RequestBody = RequestData.ToString(),
                        };
                    }
                    else if (builder.TransactionType == TransactionType.Verify)
                    {
                        if (builder.RequestMultiUseToken && string.IsNullOrEmpty((builder.PaymentMethod as ITokenizable).Token)) {
                            var tokenizationData = new JsonDoc()
                                .Set("account_name", gateway.GpApiConfig.AccessTokenInfo.TokenizationAccountName)
                                .Set("account_id", gateway.GpApiConfig.AccessTokenInfo.TokenizationAccountID)
                                .Set("reference", builder.ClientTransactionId ?? Guid.NewGuid().ToString())
                                .Set("usage_mode", EnumConverter.GetMapping(Target.GP_API, builder.PaymentMethodUsageMode))
                                .Set("fingerprint_mode", builder.CustomerData?.DeviceFingerPrint ?? null)
                                .Set("card", card);

                            var maskedVValue = new Dictionary<string, string>();
                            maskedVValue.Add("card.expiry_month", card.GetValue<string>("expiry_month"));
                            maskedVValue.Add("card.expiry_year", card.GetValue<string>("expiry_year"));
                            maskedVValue.Add("card.cvv", card.GetValue<string>("cvv"));

                            DisposeMaskedValues();
                            MaskedValues = ProtectSensitiveData.HideValues(maskedVValue);
                            MaskedValues = ProtectSensitiveData.HideValue("card.number", card.GetValue<string>("number"), 4, 6);

                            Request.MaskedValues = MaskedValues;

                            return new Request {
                                Verb = HttpMethod.Post,
                                Endpoint = $"{merchantUrl}{GpApiRequest.PAYMENT_METHODS_ENDPOINT}",
                                RequestBody = tokenizationData.ToString(),
                            };
                        }
                        else
                        {
                            var verificationData = new JsonDoc()
                                .Set("account_name", gateway.GpApiConfig.AccessTokenInfo.TransactionProcessingAccountName)
                                .Set("account_id", gateway.GpApiConfig.AccessTokenInfo.TransactionProcessingAccountID)
                                .Set("channel", EnumConverter.GetMapping(Target.GP_API, gateway.GpApiConfig.Channel))
                                .Set("reference", builder.ClientTransactionId ?? Guid.NewGuid().ToString())
                                .Set("currency", builder.Currency)
                                .Set("country", gateway.GpApiConfig.Country);
                                

                            if (builder.PaymentMethod is ITokenizable || builder.PaymentMethod is CreditCardData) {
                                if (hasToken) {
                                    paymentMethod
                                        .Set("id", (builder.PaymentMethod as ITokenizable).Token)
                                        .Set("fingerprint_mode", builder.CustomerData?.DeviceFingerPrint ?? null);
                                }
                                
                                //Authentication
                                if (builder.PaymentMethod is CreditCardData) {
                                    paymentMethod.Set("name", (builder.PaymentMethod as CreditCardData).CardHolderName);

                                    var secureEcom = (builder.PaymentMethod as CreditCardData).ThreeDSecure;
                                    if (secureEcom != null) {
                                        var authentication = new JsonDoc().Set("id", secureEcom.ServerTransactionId);
                                        paymentMethod.Set("authentication", authentication);
                                    }
                                }                                
                            }
                            verificationData.Set("payment_method", paymentMethod);

                            if (builder.StoredCredential != null) {
                                SetRequestStoredCredentials(builder.StoredCredential, verificationData);
                            }

                            Request.MaskedValues = MaskedValues;

                            return new Request {
                                Verb = HttpMethod.Post,
                                Endpoint = $"{merchantUrl}{GpApiRequest.VERIFICATIONS_ENDPOINT}",
                                RequestBody = verificationData.ToString(),
                            };
                        }
                    }

                }
                else if (builder.PaymentMethod is ITrackData)
                {
                    var track = builder.PaymentMethod as ITrackData;

                    var card = new JsonDoc()
                        .Set("track", track.Value)
                        .Set("tag", builder.TagData)
                        .Set("avs_address", builder.BillingAddress?.StreetAddress1)
                        .Set("avs_postal_code", builder.BillingAddress?.PostalCode)
                        .Set("authcode", builder.OfflineAuthCode);

                    if (builder.TransactionType == TransactionType.Verify) {
                        paymentMethod.Set("card", card);

                        var verificationData = new JsonDoc()
                            .Set("account_name", gateway.GpApiConfig.AccessTokenInfo.TransactionProcessingAccountName)
                            .Set("account_id", gateway.GpApiConfig.AccessTokenInfo.TransactionProcessingAccountID)
                            .Set("channel", EnumConverter.GetMapping(Target.GP_API, gateway.GpApiConfig.Channel))
                            .Set("reference", builder.ClientTransactionId ?? Guid.NewGuid().ToString())
                            .Set("currency", builder.Currency)
                            .Set("country", gateway.GpApiConfig.Country)
                            .Set("payment_method", paymentMethod)
                            .Set("fingerprint_mode", builder.CustomerData?.DeviceFingerPrint ?? null);

                        //Request.MaskedValues = MaskedValues;

                        return new Request {
                            Verb = HttpMethod.Post,
                            Endpoint = $"{merchantUrl}{GpApiRequest.VERIFICATIONS_ENDPOINT}",
                            RequestBody = verificationData.ToString(),
                        };
                    }

                    if (builder.TransactionType == TransactionType.Sale || builder.TransactionType == TransactionType.Refund)
                    {
                        if (string.IsNullOrEmpty(track.Value)) {
                            card.Set("number", track.Pan);
                            card.Set("expiry_month", track.Expiry?.Substring(2, 2));
                            card.Set("expiry_year", track.Expiry?.Substring(0, 2));
                        }
                        if (string.IsNullOrEmpty(builder.TagData)) {
                            card.Set("chip_condition", EnumConverter.GetMapping(Target.GP_API, builder.EmvChipCondition)); // [PREV_SUCCESS, PREV_FAILED]
                        }
                    }

                    if (builder.TransactionType == TransactionType.Sale) {
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
            }
            // payment method storage mode
            if (builder.RequestMultiUseToken) {
                //ToDo: there might be a typo: should be storage_mode
                paymentMethod.Set("storage_mode", "ON_SUCCESS");
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
                    var authentication = new JsonDoc().Set("id", secureEcom.ServerTransactionId);
                    var three_ds = new JsonDoc()
                        .Set("exempt_status", secureEcom.ExemptStatus?.ToString())
                        .Set("message_version", secureEcom.MessageVersion)
                        .Set("eci", secureEcom.Eci)
                        .Set("server_trans_reference", secureEcom.ServerTransactionId)
                        .Set("ds_trans_reference", secureEcom.DirectoryServerTransactionId)
                        .Set("value", secureEcom.AuthenticationValue);
                    authentication.Set("three_ds", three_ds);

                    paymentMethod.Set("authentication", authentication);
                }

                paymentMethod.Set("fingerprint_mode", builder.CustomerData?.DeviceFingerPrint ?? null);
            }

            if (builder.PaymentMethod is EBT) {
                paymentMethod.Set("name", (builder.PaymentMethod as EBT).CardHolderName);
            }

            if (builder.PaymentMethod is eCheck) {
                eCheck check = (builder.PaymentMethod as eCheck);
                paymentMethod.Set("name", check.CheckHolderName)
                    .Set("narrative", check.MerchantNotes);

                var bankTransfer = new JsonDoc()
                    .Set("account_number", check.AccountNumber)
                    .Set("account_type", (check.AccountType != null) ? EnumConverter.GetMapping(Target.GP_API, check.AccountType) : null)
                    .Set("check_reference", check.CheckReference)
                    .Set("sec_code", check.SecCode);

                var bank = new JsonDoc()
                    .Set("code", check.RoutingNumber)
                    .Set("name", check.BankName);

                var address = new JsonDoc()
                    .Set("line_1", check.BankAddress?.StreetAddress1)
                    .Set("line_2", check.BankAddress?.StreetAddress2)
                    .Set("line_3", check.BankAddress?.StreetAddress3)
                    .Set("city", check.BankAddress?.City)
                    .Set("postal_code", check.BankAddress?.PostalCode)
                    .Set("state", check.BankAddress?.State)
                    .Set("country", check.BankAddress?.CountryCode);

                bank.Set("address", address);

                bankTransfer.Set("bank", bank);

                paymentMethod.Set("bank_transfer", bankTransfer);

                DisposeMaskedValues();
                var maskedValue = new Dictionary<string, string>();
                maskedValue.Add("payment_method.bank_transfer.account_number", check.AccountNumber);
                maskedValue.Add("payment_method.bank_transfer.bank.code", check.RoutingNumber);

                MaskedValues = ProtectSensitiveData.HideValues(maskedValue, 4);                
            }

            if (builder.PaymentMethod is AlternativePaymentMethod) {
                var alternatepaymentMethod = (AlternativePaymentMethod)builder.PaymentMethod;

                paymentMethod.Set("name", alternatepaymentMethod.AccountHolderName);

                var apm = new JsonDoc()
                    .Set("provider", alternatepaymentMethod.AlternativePaymentMethodType?.ToString()?.ToLower())
                    .Set("address_override_mode", alternatepaymentMethod.AddressOverrideMode);
                paymentMethod.Set("apm", apm);
            }

            if (builder.PaymentMethod is BankPayment) {
                var bankpaymentMethod = (BankPayment)builder.PaymentMethod;

                var apm = new JsonDoc()
                   .Set("provider", PaymentProvider.OPEN_BANKING.ToString())
                   .Set("countries", bankpaymentMethod.Countries?.ToArray());
                paymentMethod.Set("apm", apm);

                var bankPaymentType = bankpaymentMethod.BankPaymentType ?? OpenBankingProvider.GetBankPaymentType(builder.Currency);

                var bankTransfer = new JsonDoc()
                  .Set("account_number", bankPaymentType == BankPaymentType.FASTERPAYMENTS ? bankpaymentMethod.AccountNumber : "")
                  .Set("iban", bankPaymentType == BankPaymentType.SEPA ? bankpaymentMethod.Iban : "");

                var bank = new JsonDoc()
                    .Set("code", bankpaymentMethod.SortCode)
                    .Set("name", bankpaymentMethod.AccountName);
                bankTransfer.Set("bank", bank);

                var remittance = new JsonDoc()
                    .Set("type", builder.RemittanceReferenceType.ToString())
                    .Set("value", builder.RemittanceReferenceValue);
                bankTransfer.Set("remittance_reference", remittance);

                paymentMethod.Set("bank_transfer", bankTransfer);

                DisposeMaskedValues();
                var maskedValue = new Dictionary<string, string>();
                maskedValue.Add("payment_method.bank_transfer.account_number", bankTransfer.GetValue<string>("account_number"));
                maskedValue.Add("payment_method.bank_transfer.iban", bankTransfer.GetValue<string>("iban"));

                MaskedValues = ProtectSensitiveData.HideValues(maskedValue, 4);
            }

            if (builder.PaymentMethod is BNPL) {
                BNPL bnpl = (BNPL)builder.PaymentMethod;

                var bnplType = new JsonDoc().Set("provider", EnumConverter.GetMapping(Target.GP_API, bnpl.BNPLType));

                paymentMethod.Set("name", builder.CustomerData?.FirstName + " " + builder.CustomerData?.LastName)
                    .Set("bnpl", bnplType);
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

            if (builder.TransactionType == TransactionType.Create && builder.PayByLinkData is PayByLinkData)
            {
                var payByLinkData = builder.PayByLinkData;
                var requestData = new JsonDoc()
                    .Set("account_id", gateway.GpApiConfig.AccessTokenInfo.TransactionProcessingAccountID)
                    .Set("usage_limit", payByLinkData.UsageLimit.ToString())
                    .Set("usage_mode", EnumConverter.GetMapping(Target.GP_API, payByLinkData.UsageMode))
                    .Set("images", payByLinkData.Images)
                    .Set("description", builder.Description ?? null)
                    .Set("type", payByLinkData.Type?.ToString())
                    .Set("expiration_date", payByLinkData.ExpirationDate ?? null);

                var transaction = new JsonDoc()
                    .Set("country", gateway.GpApiConfig.Country)
                    .Set("amount", builder.Amount.ToNumericCurrencyString())
                    .Set("channel", EnumConverter.GetMapping(Target.GP_API, gateway.GpApiConfig.Channel))
                    .Set("currency", builder.Currency)
                    .Set("allowed_payment_methods", GetAllowedPaymentMethod(payByLinkData.AllowedPaymentMethods));

                requestData.Set("transactions", transaction)
                    .Set("reference", builder.ClientTransactionId ?? Guid.NewGuid().ToString())
                    .Set("shipping_amount", payByLinkData.ShippingAmount.ToNumericCurrencyString())
                    .Set("shippable", payByLinkData.IsShippable != null && payByLinkData.IsShippable.Value ? "YES" : "NO")
                    .Set("account_name", gateway.GpApiConfig.AccessTokenInfo.TransactionProcessingAccountName)
                    .Set("name", payByLinkData.Name ?? null);

                var notification = new JsonDoc()
                    .Set("cancel_url", payByLinkData.CancelUrl)
                    .Set("return_url", payByLinkData.ReturnUrl)
                    .Set("status_url", payByLinkData.StatusUpdateUrl);

                requestData.Set("notifications", notification)
                    .Set("status", payByLinkData.Status.ToString());

                return new Request {
                    Verb = HttpMethod.Post,
                    Endpoint = $"{merchantUrl}{GpApiRequest.PAYBYLINK_ENDPOINT}",
                    RequestBody = requestData.ToString(),
                };
            }

            if (builder.TransactionType == TransactionType.TransferFunds)
            {
                if (!(builder.PaymentMethod is AccountFunds)) {
                    throw new UnsupportedTransactionException("Payment method doesn't support funds transfers");
                }
                var fundsData = builder.PaymentMethod as AccountFunds;
                var payload = new JsonDoc()
                   .Set("account_id", fundsData.AccountId)
                   .Set("account_name", fundsData.AccountName)
                   .Set("recipient_account_id", fundsData.RecipientAccountId)
                   .Set("reference", builder.ClientTransactionId ?? GenerationUtils.GenerateOrderId())
                   .Set("amount", builder.Amount.ToNumericCurrencyString())
                   .Set("description", builder.Description)
                   .Set("usable_balance_mode", fundsData.UsableBalanceMode?.ToString());

                var endpoint = merchantUrl;
                if (!string.IsNullOrEmpty(fundsData.MerchantId)) {
                    endpoint = $"/merchants/{fundsData.MerchantId}";
                }
                return new Request
                {
                    Verb = HttpMethod.Post,
                    Endpoint = $"{endpoint}{GpApiRequest.TRANSFER_ENDPOINT}",
                    RequestBody = payload.ToString(),
                };
            }

            var data = new JsonDoc()
                .Set("account_name", gateway.GpApiConfig.AccessTokenInfo.TransactionProcessingAccountName)
                .Set("account_id", gateway.GpApiConfig.AccessTokenInfo.TransactionProcessingAccountID)
                .Set("type", builder.TransactionType == TransactionType.Refund ? "REFUND" : "SALE") // [SALE, REFUND]
                .Set("channel", EnumConverter.GetMapping(Target.GP_API, gateway.GpApiConfig.Channel)) // [CP, CNP]
                .Set("capture_mode", GetCaptureMode(builder)) // [AUTO, LATER, MULTIPLE]
                                                              //.Set("remaining_capture_count", "") //Pending Russell
                .Set("authorization_mode", builder.AllowPartialAuth ? "PARTIAL" : null)
                .Set("amount", builder.Amount.ToNumericCurrencyString())
                .Set("currency", builder.Currency)
                .Set("reference", builder.ClientTransactionId ?? Guid.NewGuid().ToString());
            if (builder.PaymentMethod is CreditCardData && ((builder.PaymentMethod as CreditCardData).MobileType == EncyptedMobileType.CLICK_TO_PAY)) {
                data.Set("masked", builder.MaskedDataResponse ? "YES" : "NO");
            }
            data.Set("description", builder.Description)
                //.Set("order_reference", builder.OrderId)
                .Set("gratuity_amount", builder.Gratuity.ToNumericCurrencyString())
                .Set("cashback_amount", builder.CashBackAmount.ToNumericCurrencyString())
                .Set("surcharge_amount", builder.SurchargeAmount.ToNumericCurrencyString())
                .Set("convenience_amount", builder.ConvenienceAmount.ToNumericCurrencyString())
                .Set("country", gateway.GpApiConfig.Country)
                //.Set("language", EnumConverter.GetMapping(Target.GP_API, Language))
                .Set("ip_address", builder.CustomerIpAddress);
                //.Set("site_reference", "") //            
                data.Set("merchant_category", builder.MerchantCategory.ToString() ?? null);
            
            data.Set("currency_conversion", !string.IsNullOrEmpty(builder.DccRateData?.DccId) ? new JsonDoc().Set("id", builder.DccRateData.DccId) : null)
                .Set("payment_method", paymentMethod)
                .Set("risk_assessment", builder.FraudFilterMode != null ? MapFraudManagement(builder) : null)
                .Set("link", !string.IsNullOrEmpty(builder.PaymentLinkId) ? new JsonDoc()
                .Set("id", builder.PaymentLinkId) : null);

            if (builder.PaymentMethod is eCheck || builder.PaymentMethod is AlternativePaymentMethod || builder.PaymentMethod is BNPL) {
                var payer = SetPayerInformation(builder);
                if (payer.HasKeys()) {
                    data.Set("payer", payer);
                }
            }

            // set order reference
            if (!string.IsNullOrEmpty(builder.OrderId)) {
                var order = new JsonDoc()
                    .Set("reference", builder.OrderId);

                data.Set("order", order);
            }

            if (builder.PaymentMethod is AlternativePaymentMethod || builder.PaymentMethod is BNPL) {
                SetOrderInformation(builder, ref data);
            }

            if (builder.PaymentMethod is AlternativePaymentMethod ||
                builder.PaymentMethod is BNPL ||
                builder.PaymentMethod is BankPayment)
            {
                data.Set("notifications", SetNotificationUrls(builder));
            }

            // stored credential
            if (builder.StoredCredential != null) {
                SetRequestStoredCredentials(builder.StoredCredential, data);               
            }

            Request.MaskedValues = MaskedValues;

            return new Request {
                Verb = HttpMethod.Post,
                Endpoint = $"{merchantUrl}{GpApiRequest.TRANSACTION_ENDPOINT}",
                RequestBody = data.ToString(),
            };
        }

        private void SetRequestStoredCredentials(StoredCredential storedCredential, JsonDoc request)
        {
            request.Set("initiator", EnumConverter.GetMapping(Target.GP_API, storedCredential.Initiator));
                var storedCredentialJson = new JsonDoc()
                    .Set("model", EnumConverter.GetMapping(Target.GP_API, storedCredential.Type))
                    .Set("reason", EnumConverter.GetMapping(Target.GP_API, storedCredential.Reason))
                    .Set("sequence", EnumConverter.GetMapping(Target.GP_API, storedCredential.Sequence));
            request.Set("stored_credential", storedCredentialJson);            
        }

        private static void DisposeMaskedValues()
        {
            MaskedValues = null;
            ProtectSensitiveData.DisposeCollection();
        }

        private static JsonDoc SetNotificationUrls(AuthorizationBuilder builder)
        {
            INotificationData payment = null;

            if (builder.PaymentMethod is AlternativePaymentMethod) {
                payment = (builder.PaymentMethod) as AlternativePaymentMethod;
            }
            if (builder.PaymentMethod is BNPL) {
                payment = builder.PaymentMethod as BNPL;
            }
            if (builder.PaymentMethod is BankPayment) {
                payment = builder.PaymentMethod as BankPayment;
            }

            var notifications = new JsonDoc()
                  .Set("return_url", payment?.ReturnUrl)
                  .Set("status_url", payment?.StatusUpdateUrl)
                  .Set("cancel_url", payment?.CancelUrl);

            return notifications;
        }

        private static string[] GetAllowedPaymentMethod(PaymentMethodName[] allowedPaymentMethods) {
            if (allowedPaymentMethods == null)
                return null;
            
            string[] result = new string[allowedPaymentMethods.Length];
            for (int i = 0; i < (allowedPaymentMethods.Length); i++ ) {
                result[i] = EnumConverter.GetMapping(Target.GP_API, allowedPaymentMethods[i]);
            }

            return result;
        }

        public static List<Dictionary<string, object>> MapFraudManagement(AuthorizationBuilder builder)
        {
            List<Dictionary<string, object>> rules = new List<Dictionary<string, object>>();
            if (builder.FraudRules != null) {
                foreach (var fraudRule in builder.FraudRules.Rules) {
                    Dictionary<string, object> rule = new Dictionary<string, object>();
                    rule.Add("reference", fraudRule.Key);
                    rule.Add("mode", fraudRule.Mode.ToString());

                    rules.Add(rule);
                }
            }

            var result = new List<Dictionary<string, object>>();

            var item = new Dictionary<string, object>();
            item.Add("mode", builder.FraudFilterMode.ToString());
            if (rules.Count > 0) {
                item.Add("rules", rules);
            }
            result.Add(item);

            return result;
        }

        private static string GetEntryMode(AuthorizationBuilder builder,Channel channel)
        {
            if (channel == Channel.CardPresent)
            {
                if (builder.PaymentMethod is ITrackData) {
                    var paymentMethod = (ITrackData)builder.PaymentMethod;
                    if (!string.IsNullOrEmpty(builder.TagData))
                    {
                        if (paymentMethod.EntryMethod == EntryMethod.Proximity)
                        {
                            return "CONTACTLESS_CHIP";
                        }
                        var emvData = EmvUtils.ParseTagData(builder.TagData);
                        if (emvData.isContactlessMsd())
                        {
                            return "CONTACTLESS_SWIPE";
                        }
                        return "CHIP";
                    }
                    if (paymentMethod.EntryMethod == EntryMethod.Swipe) 
                    {
                        return "SWIPE";
                    }
                }
                if (builder.PaymentMethod is ICardData && ((ICardData)builder.PaymentMethod).CardPresent)
                {
                    return "MANUAL";
                }
                return "SWIPE";
            } else
            {
                if (builder.PaymentMethod is ICardData) 
                {
                    var paymentMethod = (ICardData)builder.PaymentMethod;
                    if (paymentMethod.ReaderPresent) 
                    {
                        return "ECOM";
                    }
                    else
                    {
                        switch (paymentMethod?.EntryMethod) {
                        case ManualEntryMethod.Phone:
                            return "PHONE";
                        case ManualEntryMethod.Moto:
                            return "MOTO";
                        case ManualEntryMethod.Mail:
                            return "MAIL";
                            default:
                            break;
                        }
                    }
                }
                if(builder.TransactionModifier == TransactionModifier.EncryptedMobile &&
                    builder.PaymentMethod is CreditCardData &&
                    ((CreditCardData)builder.PaymentMethod).HasInAppPaymentData()) {
                    return "IN_APP";
                }

                return "ECOM";
            }
        }

        private static JsonDoc SetPayerInformation(AuthorizationBuilder builder)
        {
            JsonDoc payer = new JsonDoc();
            payer.Set("reference", builder.CustomerId ?? builder.CustomerData?.Id);
            if(builder.PaymentMethod is eCheck)
            {
                JsonDoc billingAddress = GetBasicAddressInformation(builder.BillingAddress); 
                    
                if (billingAddress.HasKeys()) {
                    payer.Set("billing_address", billingAddress);
                }

                if (builder.CustomerData != null) {
                    payer.Set("name", builder.CustomerData.FirstName + " " + builder.CustomerData.LastName);
                    payer.Set("date_of_birth", builder.CustomerData.DateOfBirth);
                }
                payer.Set("landline_phone", builder.CustomerData?.HomePhone?.ToNumeric() ?? builder.HomePhone?.ToString());
                payer.Set("mobile_phone", builder.CustomerData?.MobilePhone?.ToNumeric() ?? builder.MobilePhone?.ToString());
            }
            else if(builder.PaymentMethod is AlternativePaymentMethod)
            {
                JsonDoc homePhone = new JsonDoc();

                homePhone.Set("country_code", builder.HomePhone?.CountryCode)
                        .Set("subscriber_number", builder.HomePhone?.Number);

                if (homePhone.HasKeys()) {
                    payer.Set("home_phone", homePhone);
                }

                JsonDoc workPhone = new JsonDoc();

                workPhone.Set("country_code", builder.WorkPhone?.CountryCode)
                        .Set("subscriber_number", builder.WorkPhone?.Number);

                if (workPhone.HasKeys()) {
                    payer.Set("work_phone", workPhone);
                }
            }
            else if(builder.PaymentMethod is BNPL && builder.CustomerData != null) {
               
                payer.Set("email", builder.CustomerData.Email)
                     .Set("date_of_birth", builder.CustomerData.DateOfBirth);

                JsonDoc billing_address = GetBasicAddressInformation(builder.BillingAddress);                
                
                if (builder.CustomerData != null) {
                    billing_address
                           .Set("first_name", builder.CustomerData?.FirstName)
                           .Set("last_name", builder.CustomerData?.LastName);
                }

                payer.Set("billing_address", billing_address);

                if (builder.CustomerData.Phone != null) {
                    JsonDoc homePhone = new JsonDoc();

                    homePhone.Set("country_code", builder.CustomerData.Phone?.CountryCode)
                            .Set("subscriber_number", builder.CustomerData.Phone?.Number);

                    payer.Set("contact_phone", homePhone);
                    
                }
                if (builder.CustomerData.Documents != null) {

                    var documents = new List<Dictionary<string, object>>();
                    foreach (var document in builder.CustomerData.Documents) {
                        var doc = new Dictionary<string, object>();
                        
                        doc.Add("type", document.Type.ToString());
                        doc.Add("reference", document.Reference);
                        doc.Add("issuer", document.Issuer);

                        documents.Add(doc);                            
                    }
                    payer.Set("documents", documents);
                }
            }
            return payer;
        }

        private static JsonDoc GetBasicAddressInformation(Address address) {
            return new JsonDoc().Set("line_1", address?.StreetAddress1)
                            .Set("line_2", address?.StreetAddress2)
                            .Set("city", address?.City)
                            .Set("postal_code", address?.PostalCode)
                            .Set("state", address?.State)
                            .Set("country", address?.CountryCode);
        }

        private static string GetCaptureMode(AuthorizationBuilder builder) {
            if (builder.MultiCapture) {
                return "MULTIPLE";
            }
            else if (builder.TransactionType == TransactionType.Auth) {
                return "LATER";
            }
            return "AUTO";
        }

        private static JsonDoc SetItemDetailsListForBNPL(AuthorizationBuilder builder, ref JsonDoc order)
        {
            var items = new List<Dictionary<string, object>>();                        
                foreach (var product in builder.MiscProductData) {
                    var item = new Dictionary<string, object>();                    
                    var qta = product.Quantity;
                    var taxAmount = product.TaxAmount ?? 0;
                    var unitAmount = product.UnitPrice ?? 0;
                    var netUnitAmount = product.NetUnitAmount ?? 0;
                    var discountAmount = product.DiscountAmount ?? 0;
                    item.Add("reference", product.ProductId ?? null);
                    item.Add("label", product.ProductName ?? null);
                    item.Add("description", product.Description ?? null);
                    item.Add("quantity", qta.ToString());
                    item.Add("unit_amount", unitAmount.ToNumericCurrencyString());
                    item.Add("total_amount", (qta * unitAmount).ToNumericCurrencyString());
                    item.Add("tax_amount", taxAmount.ToNumericCurrencyString());
                    item.Add("discount_amount", discountAmount != 0 ? discountAmount.ToNumericCurrencyString().RemoveInitialZero() : "0");
                    item.Add("tax_percentage", product.TaxPercentage != 0 ? product.TaxPercentage.ToNumericCurrencyString().RemoveInitialZero() : "0");
                    item.Add("net_unit_amount", netUnitAmount.ToNumericCurrencyString());
                    item.Add("gift_card_currency", product.GiftCardCurrency);
                    item.Add("url", product.Url);
                    item.Add("image_url", product.ImageUrl);
                    items.Add(item);
                }
                         
            return order.Set("items", items);
        }

        private static JsonDoc SetItemDetailsListForApm(AuthorizationBuilder builder, ref JsonDoc order) {
            decimal taxTotalAmount = 0, itemsAmount = 0;
            decimal? orderAmount = null;
            if (builder.MiscProductData != null) {
                var items = new List<Dictionary<string, object>>();
                foreach (var product in builder.MiscProductData) {
                    var qta = product.Quantity ?? 0;
                    var taxAmount = product.TaxAmount ?? 0;
                    var unitAmount = product.UnitPrice ?? 0;
                    var item = new Dictionary<string, object>();
                    item.Add("reference", product.ProductId);
                    item.Add("label", product.ProductName);
                    item.Add("description", product.Description);
                    item.Add("quantity", qta);
                    item.Add("unit_amount", unitAmount.ToNumericCurrencyString());
                    item.Add("unit_currency", product.UnitCurrency);
                    item.Add("tax_amount", taxAmount.ToNumericCurrencyString());
                    item.Add("amount", (qta * unitAmount).ToNumericCurrencyString());
                    items.Add(item);
                    taxTotalAmount += taxAmount;
                    itemsAmount += unitAmount;
                }

                order.Set("tax_amount", taxTotalAmount.ToNumericCurrencyString());
                order.Set("item_amount", itemsAmount.ToNumericCurrencyString());
                var shippingAmount = builder.ShippingAmt ?? 0;
                order.Set("shipping_amount", builder.ShippingAmt.ToNumericCurrencyString());
                order.Set("insurance_offered", builder.OrderDetails != null ? (builder.OrderDetails.HasInsurance ? "YES" : "NO") : null);
                order.Set("shipping_discount", builder.ShippingDiscount?.ToNumericCurrencyString());
                order.Set("insurance_amount", builder.OrderDetails?.InsuranceAmount?.ToNumericCurrencyString());
                order.Set("handling_amount", builder.OrderDetails?.HandlingAmount?.ToNumericCurrencyString());
                var insuranceAmount = builder.OrderDetails?.InsuranceAmount ?? 0;
                var handlingAmount = builder.OrderDetails?.HandlingAmount ?? 0;
                orderAmount = itemsAmount + taxTotalAmount + handlingAmount + insuranceAmount + shippingAmount;
                order.Set("amount", orderAmount.ToNumericCurrencyString());
                order.Set("currency", builder.Currency);
                order.Set("items", items);
            }

            return order;
        }

        private static JsonDoc SetOrderInformation(AuthorizationBuilder builder, ref JsonDoc requestBody) {
            JsonDoc order;
            if (requestBody.Has("order")) {
                order = requestBody.Get("order");
            }
            else {
                order = new JsonDoc();
            }
            order.Set("description", builder.OrderDetails?.Description);

            var shippingAddress = new JsonDoc();
            if (builder.ShippingAddress != null) {
                shippingAddress
                    .Set("line_1", builder.ShippingAddress.StreetAddress1)
                    .Set("line_2", builder.ShippingAddress.StreetAddress2)
                    .Set("line_3", builder.ShippingAddress.StreetAddress3)
                    .Set("city", builder.ShippingAddress.City)
                    .Set("postal_code", builder.ShippingAddress.PostalCode)
                    .Set("state", builder.ShippingAddress.State)
                    .Set("country", builder.ShippingAddress.CountryCode);                
            }

            var shippingPhone = new JsonDoc()
                .Set("country_code", builder.ShippingPhone?.CountryCode)
                .Set("subscriber_number", builder.ShippingPhone?.Number);

            if (shippingPhone.HasKeys()) {
                order.Set("shipping_phone", shippingPhone);
            }

            //AlternativePaymentMethod
            if (builder.PaymentMethod is AlternativePaymentMethod) { 
                if (builder.MiscProductData != null) {
                    SetItemDetailsListForApm(builder, ref order);
                }
            }

            //Buy Now Pay Later
            if (builder.PaymentMethod is BNPL) {
                order.Set("shipping_method", builder.BNPLShippingMethod.ToString());

                if (builder.MiscProductData != null) {
                    SetItemDetailsListForBNPL(builder, ref order);
                }

                if (builder.CustomerData != null) {
                     shippingAddress
                            .Set("first_name", builder.CustomerData?.FirstName)
                            .Set("last_name", builder.CustomerData?.LastName);                    
                }
            }

            if (shippingAddress.HasKeys()) {
                order.Set("shipping_address", shippingAddress);
            }

            if (!requestBody.Has("order") && order.HasKeys()) {               
                requestBody.Set("order", order);                
            }

            return requestBody;
        }
    }
}
