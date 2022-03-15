using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace GlobalPayments.Api.Entities {
    internal class GpApiAuthorizationRequestBuilder {
        internal static GpApiRequest BuildRequest(AuthorizationBuilder builder, GpApiConnector gateway) {
            var merchantUrl = !string.IsNullOrEmpty(gateway.MerchantId) ? $"/merchants/{gateway.MerchantId}" : string.Empty;
            var paymentMethod = new JsonDoc()
                .Set("entry_mode", GetEntryMode(builder, gateway.Channel)); // [MOTO, ECOM, IN_APP, CHIP, SWIPE, MANUAL, CONTACTLESS_CHIP, CONTACTLESS_SWIPE]
            if (builder.PaymentMethod is CreditCardData && (builder.TransactionModifier == TransactionModifier.EncryptedMobile || builder.TransactionModifier == TransactionModifier.DecryptedMobile))
            {
                var digitalWallet = new JsonDoc();
                var creditCardData = (builder.PaymentMethod as CreditCardData);
                //Digital Wallet
                if (builder.TransactionModifier == TransactionModifier.EncryptedMobile)
                {
                    digitalWallet
                        .Set("payment_token", JsonDoc.Parse(creditCardData.Token));

                }
                else if (builder.TransactionModifier == TransactionModifier.DecryptedMobile)
                {
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

                    if (!(builder.TransactionType == TransactionType.Tokenize || builder.TransactionType == TransactionType.Verify))
                    {
                        card.Set("cvv_indicator", cardData.CvnPresenceIndicator != 0 ? EnumConverter.GetMapping(Target.GP_API, cardData.CvnPresenceIndicator) : null); // [ILLEGIBLE, NOT_PRESENT, PRESENT]
                        card.Set("funding", builder.PaymentMethod?.PaymentMethodType == PaymentMethodType.Debit ? "DEBIT" : "CREDIT"); // [DEBIT, CREDIT]
                    }
                    
                    paymentMethod.Set("card", card);                    

                    if (builder.TransactionType == TransactionType.Tokenize) {
                        var tokenizationData = new JsonDoc()
                            .Set("account_name", gateway.TokenizationAccountName)
                            .Set("reference", builder.ClientTransactionId ?? Guid.NewGuid().ToString())
                            .Set("usage_mode", EnumConverter.GetMapping(Target.GP_API, builder.PaymentMethodUsageMode))                            
                            .Set("card", card);

                        return new GpApiRequest {
                            Verb = HttpMethod.Post,
                            Endpoint = $"{merchantUrl}/payment-methods",
                            RequestBody = tokenizationData.ToString(),
                        };
                    }
                    else if (builder.TransactionType == TransactionType.DccRateLookup)
                    {
                        // tokenized payment method
                        if (builder.PaymentMethod is ITokenizable)
                        {
                            string token = ((ITokenizable)builder.PaymentMethod).Token;
                            if (!string.IsNullOrEmpty(token))
                            {
                                paymentMethod.Set("id", token);
                            }
                        }

                        var RequestData = new JsonDoc()
                           .Set("account_name", gateway.TransactionProcessingAccountName)
                           .Set("channel", EnumConverter.GetMapping(Target.GP_API, gateway.Channel))
                           .Set("reference", builder.ClientTransactionId ?? Guid.NewGuid().ToString())
                           .Set("amount", builder.Amount.ToNumericCurrencyString())
                           .Set("currency", builder.Currency)
                           .Set("country", gateway.Country)
                           .Set("payment_method", paymentMethod);

                        return new GpApiRequest
                        {
                            Verb = HttpMethod.Post,
                            Endpoint = $"{merchantUrl}/currency-conversions",
                            RequestBody = RequestData.ToString(),
                        };
                    }
                    else if (builder.TransactionType == TransactionType.Verify) {
                        if (builder.RequestMultiUseToken && string.IsNullOrEmpty((builder.PaymentMethod as ITokenizable).Token)) {
                            var tokenizationData = new JsonDoc()
                                .Set("account_name", gateway.TokenizationAccountName)
                                .Set("reference", builder.ClientTransactionId ?? Guid.NewGuid().ToString())
                                .Set("usage_mode", EnumConverter.GetMapping(Target.GP_API, builder.PaymentMethodUsageMode))
                                .Set("fingerprint_mode", builder.CustomerData?.DeviceFingerPrint ?? null)
                                .Set("card", card);

                            return new GpApiRequest {
                                Verb = HttpMethod.Post,
                                Endpoint = $"{merchantUrl}/payment-methods",
                                RequestBody = tokenizationData.ToString(),
                            };
                        }                    
                        else
                        {
                            var verificationData = new JsonDoc()
                                .Set("account_name", gateway.TransactionProcessingAccountName)
                                .Set("channel", EnumConverter.GetMapping(Target.GP_API, gateway.Channel))
                                .Set("reference", builder.ClientTransactionId ?? Guid.NewGuid().ToString())
                                .Set("currency", builder.Currency)
                                .Set("country", gateway.Country)
                                .Set("payment_method", paymentMethod);

                            if (builder.PaymentMethod is ITokenizable && !string.IsNullOrEmpty((builder.PaymentMethod as ITokenizable).Token)) {
                                verificationData.Remove("payment_method");
                                verificationData.Set("payment_method", new JsonDoc()
                                    .Set("entry_mode", GetEntryMode(builder, gateway.Channel))
                                    .Set("id", (builder.PaymentMethod as ITokenizable).Token)
                                    .Set("fingerprint_mode", builder.CustomerData?.DeviceFingerPrint ?? null)
                                );
                            }

                            return new GpApiRequest {
                                Verb = HttpMethod.Post,
                                Endpoint = $"{merchantUrl}/verifications",
                                RequestBody = verificationData.ToString(),
                            };
                        }
                    }
                }
                else if (builder.PaymentMethod is ITrackData) {
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
                            .Set("account_name", gateway.TransactionProcessingAccountName)
                            .Set("channel", EnumConverter.GetMapping(Target.GP_API, gateway.Channel))
                            .Set("reference", builder.ClientTransactionId ?? Guid.NewGuid().ToString())
                            .Set("currency", builder.Currency)
                            .Set("country", gateway.Country)
                            .Set("payment_method", paymentMethod)
                            .Set("fingerprint_mode", builder.CustomerData?.DeviceFingerPrint ?? null);

                        return new GpApiRequest {
                            Verb = HttpMethod.Post,
                            Endpoint = $"{merchantUrl}/verifications",
                            RequestBody = verificationData.ToString(),
                        };
                    }

                    if (builder.TransactionType == TransactionType.Sale || builder.TransactionType == TransactionType.Refund) {
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
            if (builder.PaymentMethod is IPinProtected)
            {
                paymentMethod.Get("card")?.Set("pin_block", ((IPinProtected)builder.PaymentMethod).PinBlock);
            }

            // authentication
            if (builder.PaymentMethod is CreditCardData)
            {
                paymentMethod.Set("name", (builder.PaymentMethod as CreditCardData).CardHolderName);

                var secureEcom = (builder.PaymentMethod as CreditCardData).ThreeDSecure;
                if (secureEcom != null)
                {
                    var authentication = new JsonDoc().Set("id", secureEcom.ServerTransactionId);

                    paymentMethod.Set("authentication", authentication);
                }

                paymentMethod.Set("fingerprint_mode", builder.CustomerData?.DeviceFingerPrint ?? null);
            }

            if(builder.PaymentMethod is EBT)
            {
                paymentMethod.Set("name", (builder.PaymentMethod as EBT).CardHolderName);
            }

            if (builder.PaymentMethod is eCheck)
            {
                eCheck check = (builder.PaymentMethod as eCheck);
                paymentMethod.Set("name", check.CheckHolderName);

                var bankTransfer = new JsonDoc()
                    .Set("account_number", check.AccountNumber)
                    .Set("account_type", (check.AccountType != null) ? EnumConverter.GetMapping(Target.GP_API, check.AccountType) : null)
                    .Set("check_reference", check.CheckReference)
                    .Set("sec_code", check.SecCode)
                    .Set("narrative", check.MerchantNotes);

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

            }

            if (builder.PaymentMethod is AlternativePaymentMethod)
            {
                var alternatepaymentMethod = (AlternativePaymentMethod)builder.PaymentMethod;
                
                paymentMethod.Set("name", alternatepaymentMethod.AccountHolderName);

                var apm = new JsonDoc()
                    .Set("provider", alternatepaymentMethod.AlternativePaymentMethodType?.ToString()?.ToLower())
                    .Set("address_override_mode", alternatepaymentMethod.AddressOverrideMode);
                paymentMethod.Set("apm", apm);
            }

            // encryption
            if (builder.PaymentMethod is IEncryptable)
            {
                var encryptionData = ((IEncryptable)builder.PaymentMethod).EncryptionData;

                if (encryptionData != null)
                {
                    var encryption = new JsonDoc()
                        .Set("version", encryptionData.Version);

                    if (!string.IsNullOrEmpty(encryptionData.KTB))
                    {
                        encryption.Set("method", "KTB");
                        encryption.Set("info", encryptionData.KTB);
                    }
                    else if (!string.IsNullOrEmpty(encryptionData.KSN))
                    {
                        encryption.Set("method", "KSN");
                        encryption.Set("info", encryptionData.KSN);
                    }

                    if (encryption.Has("info"))
                    {
                        paymentMethod.Set("encryption", encryption);
                    }
                }
            }           

            var data = new JsonDoc()
                .Set("account_name", gateway.TransactionProcessingAccountName)
                .Set("type", builder.TransactionType == TransactionType.Refund ? "REFUND" : "SALE") // [SALE, REFUND]
                .Set("channel", EnumConverter.GetMapping(Target.GP_API, gateway.Channel)) // [CP, CNP]
                .Set("capture_mode", GetCaptureMode(builder)) // [AUTO, LATER, MULTIPLE]
                //.Set("remaining_capture_count", "") //Pending Russell
                .Set("authorization_mode", builder.AllowPartialAuth ? "PARTIAL" : null)
                .Set("amount", builder.Amount.ToNumericCurrencyString())
                .Set("currency", builder.Currency)
                .Set("reference", builder.ClientTransactionId ?? Guid.NewGuid().ToString())
                .Set("description", builder.Description)
                //.Set("order_reference", builder.OrderId)
                .Set("gratuity_amount", builder.Gratuity.ToNumericCurrencyString())
                .Set("cashback_amount", builder.CashBackAmount.ToNumericCurrencyString())
                .Set("surcharge_amount", builder.SurchargeAmount.ToNumericCurrencyString())
                .Set("convenience_amount", builder.ConvenienceAmount.ToNumericCurrencyString())
                .Set("country", gateway.Country)
                //.Set("language", EnumConverter.GetMapping(Target.GP_API, Language))
                .Set("ip_address", builder.CustomerIpAddress)
                //.Set("site_reference", "") //
                .Set("currency_conversion", !string.IsNullOrEmpty(builder.DccRateData?.DccId) ? new JsonDoc().Set("id", builder.DccRateData.DccId) : null)                
                .Set("payment_method", paymentMethod)                
                .Set("link", !string.IsNullOrEmpty(builder.PaymentLinkId) ? new JsonDoc()
                            .Set("id", builder.PaymentLinkId) : null );

            if (builder.PaymentMethod is eCheck || builder.PaymentMethod is AlternativePaymentMethod) {
                data.Set("payer", setPayerInformation(builder));
            }

            // set order reference
            if (!string.IsNullOrEmpty(builder.OrderId))
            {
                var order = new JsonDoc()
                    .Set("reference", builder.OrderId);

                data.Set("order", order);
            }

            if (builder.PaymentMethod is AlternativePaymentMethod) {
                setOrderInformation(builder, ref data);

                var alternatepaymentMethod = (AlternativePaymentMethod)builder.PaymentMethod;

                var notifications = new JsonDoc()
                   .Set("return_url", alternatepaymentMethod?.ReturnUrl)
                   .Set("status_url", alternatepaymentMethod?.StatusUpdateUrl)
                   .Set("cancel_url", alternatepaymentMethod?.CancelUrl);

                data.Set("notifications", notifications);
            }

            // stored credential
            if (builder.StoredCredential != null) {
                data.Set("initiator", EnumConverter.GetMapping(Target.GP_API, builder.StoredCredential.Initiator));
                var storedCredential = new JsonDoc()
                    .Set("model", EnumConverter.GetMapping(Target.GP_API, builder.StoredCredential.Type))
                    .Set("reason", EnumConverter.GetMapping(Target.GP_API, builder.StoredCredential.Reason))
                    .Set("sequence", EnumConverter.GetMapping(Target.GP_API, builder.StoredCredential.Sequence));
                data.Set("stored_credential", storedCredential);
            }

            return new GpApiRequest {
                Verb = HttpMethod.Post,
                Endpoint = $"{merchantUrl}/transactions",
                RequestBody = data.ToString(),
            };
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

        private static JsonDoc setPayerInformation(AuthorizationBuilder builder)
        {
            JsonDoc payer = new JsonDoc();
            payer.Set("reference", builder.CustomerId ?? builder.CustomerData?.Id);
            if(builder.PaymentMethod is eCheck)
            {
                JsonDoc billingAddress = new JsonDoc();
                    
                billingAddress.Set("line_1", builder.BillingAddress?.StreetAddress1)
                        .Set("line_2", builder.BillingAddress?.StreetAddress2)
                        .Set("city", builder.BillingAddress?.City)
                        .Set("postal_code", builder.BillingAddress?.PostalCode)
                        .Set("state", builder.BillingAddress?.State)
                        .Set("country", builder.BillingAddress.CountryCode);

                payer.Set("billing_address", billingAddress);

                if (builder.CustomerData != null) 
                {
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

                payer.Set("home_phone", homePhone);

                JsonDoc workPhone = new JsonDoc();

                workPhone.Set("country_code", builder.WorkPhone?.CountryCode)
                        .Set("subscriber_number", builder.WorkPhone?.Number);

                payer.Set("work_phone", workPhone);
            }
            return payer;
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

        private static JsonDoc setOrderInformation(AuthorizationBuilder builder, ref JsonDoc requestBody)
        {
            JsonDoc order;
            if (requestBody.Has("order"))
            {
                order = requestBody.Get("order");
            }
            else
            {
                order = new JsonDoc();
            }
            order.Set("description", builder.OrderDetails?.Description);

            if (builder.ShippingAddress != null)
            {
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

            var shippingPhone = new JsonDoc()
                .Set("country_code", builder.ShippingPhone?.CountryCode)
                .Set("subscriber_number", builder.ShippingPhone?.Number);

            order.Set("shipping_phone", shippingPhone);

            decimal taxTotalAmount = 0;
            decimal itemsAmount = 0;
            decimal? orderAmount = null;
            if (builder.MiscProductData != null)
            {
                var items = new List<Dictionary<string,object>>();
                foreach (var product in builder.MiscProductData)
                {
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
            if (!requestBody.Has("order"))
            {
                requestBody.Set("order", order);
            }

            return requestBody;
        }
    }
}
