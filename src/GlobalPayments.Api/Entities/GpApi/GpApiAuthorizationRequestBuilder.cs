using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System;
using System.Net.Http;

namespace GlobalPayments.Api.Entities {
    internal class GpApiAuthorizationRequestBuilder {
        internal static GpApiRequest BuildRequest(AuthorizationBuilder builder, GpApiConnector gateway) {
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

                card.Set("chip_condition", EnumConverter.GetMapping(Target.GP_API, builder.EmvChipCondition)); // [PREV_SUCCESS, PREV_FAILED]

                paymentMethod.Set("card", card);

                var tokenizationData = new JsonDoc()
                    .Set("account_name", gateway.TokenizationAccountName)
                    .Set("reference", builder.ClientTransactionId ?? Guid.NewGuid().ToString())
                    .Set("usage_mode", EnumConverter.GetMapping(Target.GP_API, builder.TokenUsageMode))
                    .Set("name", "")
                    .Set("card", card);

                if (builder.TransactionType == TransactionType.Tokenize) {
                    return new GpApiRequest {
                        Verb = HttpMethod.Post,
                        Endpoint = "/payment-methods",
                        RequestBody = tokenizationData.ToString(),
                    };
                }
                else if (builder.TransactionType == TransactionType.Verify) {
                    if (builder.RequestMultiUseToken && string.IsNullOrEmpty((builder.PaymentMethod as ITokenizable).Token)) {
                        return new GpApiRequest {
                            Verb = HttpMethod.Post,
                            Endpoint = "/payment-methods",
                            RequestBody = tokenizationData.ToString(),
                        };
                    }
                    else {
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
                                .Set("entry_mode", GetEntryMode(builder))
                                .Set("id", (builder.PaymentMethod as ITokenizable).Token)
                            );
                        }

                        return new GpApiRequest {
                            Verb = HttpMethod.Post,
                            Endpoint = "/verifications",
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
                        .Set("payment_method", paymentMethod);

                    return new GpApiRequest {
                        Verb = HttpMethod.Post,
                        Endpoint = "/verifications",
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

            // payment method storage mode
            if (builder.RequestMultiUseToken) {
                //ToDo: there might be a typo: should be storage_mode
                paymentMethod.Set("storage_model", "ON_SUCCESS");
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
                .Set("payment_method", paymentMethod);

            // set order reference
            if (!string.IsNullOrEmpty(builder.OrderId)) {
                var order = new JsonDoc()
                    .Set("reference", builder.OrderId);

                data.Set("order", order);
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
                Endpoint = "/transactions",
                RequestBody = data.ToString(),
            };
        }

        private static string GetEntryMode(AuthorizationBuilder builder) {
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

        private static string GetCaptureMode(AuthorizationBuilder builder) {
            if (builder.MultiCapture) {
                return "MULTIPLE";
            }
            else if (builder.TransactionType == TransactionType.Auth) {
                return "LATER";
            }
            return "AUTO";
        }
    }
}
