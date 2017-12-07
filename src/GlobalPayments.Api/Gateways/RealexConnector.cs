using System.Collections.Generic;
using System.Text.RegularExpressions;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways {
    internal class RealexConnector : XmlGateway, IPaymentGateway, IRecurringService {
        public string MerchantId { get; set; }
        public string AccountId { get; set; }
        public string SharedSecret { get; set; }
        public string Channel { get; set; }
        public string RebatePassword { get; set; }
        public string RefundPassword { get; set; }
        public bool SupportsHostedPayments {  get { return true; } }
        public bool SupportsRetrieval { get { return false; } }
        public bool SupportsUpdatePaymentDetails { get { return true; } }
        public HostedPaymentConfig HostedPaymentConfig { get; set; }

        #region transaction handling
        public Transaction ProcessAuthorization(AuthorizationBuilder builder) {
            var et = new ElementTree();
            string timestamp = builder.Timestamp ?? GenerationUtils.GenerateTimestamp();
            string orderId = builder.OrderId ?? GenerationUtils.GenerateOrderId();

            // Build Request
            var request = et.Element("request")
                .Set("timestamp", timestamp)
                .Set("type", MapAuthRequestType(builder));
            et.SubElement(request, "merchantid").Text(MerchantId);
            et.SubElement(request, "account", AccountId);
            et.SubElement(request, "channel", Channel);
            et.SubElement(request, "orderid", orderId);
            if (builder.Amount.HasValue) {
                et.SubElement(request, "amount").Text(builder.Amount.ToNumericCurrencyString()).Set("currency", builder.Currency);
            }

            // Hydrate the payment data fields
            if (builder.PaymentMethod is CreditCardData) {
                var card = builder.PaymentMethod as CreditCardData;

                var cardElement = et.SubElement(request, "card");
                et.SubElement(cardElement, "number", card.Number);
                et.SubElement(cardElement, "expdate", card.ShortExpiry);
                et.SubElement(cardElement, "chname").Text(card.CardHolderName);
                et.SubElement(cardElement, "type", card.CardType.ToUpper());

                if (card.Cvn != null) {
                    var cvnElement = et.SubElement(cardElement, "cvn");
                    et.SubElement(cvnElement, "number", card.Cvn);
                    et.SubElement(cvnElement, "presind", (int)card.CvnPresenceIndicator);
                }

                // TODO: mpi
                if (card.ThreeDSecure != null) {
                    var mpi = et.SubElement(request, "mpi");
                    et.SubElement(mpi, "cavv", card.ThreeDSecure.Cavv);
                    et.SubElement(mpi, "xid", card.ThreeDSecure.Xid);
                    et.SubElement(mpi, "eci", card.ThreeDSecure.Eci);
                }

                // issueno
                string hash = string.Empty;
                if (builder.TransactionType == TransactionType.Verify)
                    hash = GenerationUtils.GenerateHash(SharedSecret, timestamp, MerchantId, orderId, card.Number);
                else hash = GenerationUtils.GenerateHash(SharedSecret, timestamp, MerchantId, orderId, builder.Amount.ToNumericCurrencyString(), builder.Currency, card.Number);
                et.SubElement(request, "sha1hash").Text(hash);
            }
            if (builder.PaymentMethod is RecurringPaymentMethod) {
                var recurring = builder.PaymentMethod as RecurringPaymentMethod;
                et.SubElement(request, "payerref").Text(recurring.CustomerKey);
                et.SubElement(request, "paymentmethod").Text(recurring.Key ?? recurring.Id);

                if (!string.IsNullOrEmpty(builder.Cvn)) {
                    var paymentData = et.SubElement(request, "paymentdata");
                    var cvn = et.SubElement(paymentData, "cvn");
                    et.SubElement(cvn, "number").Text(builder.Cvn);
                }

                string hash = string.Empty;
                if (builder.TransactionType == TransactionType.Verify)
                    hash = GenerationUtils.GenerateHash(SharedSecret, timestamp, MerchantId, orderId, recurring.CustomerKey);
                else hash = GenerationUtils.GenerateHash(SharedSecret, timestamp, MerchantId, orderId, builder.Amount.ToNumericCurrencyString(), builder.Currency, recurring.CustomerKey);
                et.SubElement(request, "sha1hash").Text(hash);
            }
            else {
                // TODO: Token Processing
                //et.SubElement(request, "sha1hash", GenerateHash(order, token));
            }

            // refund hash
            if (builder.TransactionType == TransactionType.Refund)
                et.SubElement(request, "refundhash", GenerationUtils.GenerateHash(RefundPassword) ?? string.Empty);

            // This needs to be figured out based on txn type and set to 0, 1 or MULTI
            if (builder.TransactionType == TransactionType.Sale || builder.TransactionType == TransactionType.Auth) {
                var autoSettle = builder.TransactionType == TransactionType.Sale ? "1" : "0";
                et.SubElement(request, "autosettle").Set("flag", autoSettle);
            }

            // TODO: needs to be multiple
            if (builder.Description != null) {
                var comments = et.SubElement(request, "comments");
                et.SubElement(comments, "comment", builder.Description).Set("id", "1");
            }

            // fraudfilter
            if (builder.RecurringType != null || builder.RecurringSequence != null) {
                et.SubElement(request, "recurring")
                    .Set("type", builder.RecurringType.ToString().ToLower())
                    .Set("sequence", builder.RecurringSequence.ToString().ToLower());
            }

            // tssinfo
            if (builder.CustomerId != null || builder.ProductId != null || builder.CustomerId != null || builder.ClientTransactionId != null || builder.BillingAddress != null || builder.ShippingAddress != null) {
                var tssInfo = et.SubElement(request, "tssinfo");
                et.SubElement(tssInfo, "custnum", builder.CustomerId);
                et.SubElement(tssInfo, "prodid", builder.ProductId);
                et.SubElement(tssInfo, "varref", builder.ClientTransactionId);
                et.SubElement(tssInfo, "custipaddress", builder.CustomerIpAddress);
                if (builder.BillingAddress != null)
                    tssInfo.Append(BuildAddress(et, builder.BillingAddress));
                if (builder.ShippingAddress != null)
                    tssInfo.Append(BuildAddress(et, builder.ShippingAddress));
            }

            //et.SubElement(request, "mobile");
            //et.SubElement(request, "token", token);

            var response = DoTransaction(et.ToString(request));
            return MapResponse(response);
        }

        public string SerializeRequest(AuthorizationBuilder builder) {
            // check for hpp config
            if (HostedPaymentConfig == null)
                throw new ApiException("Hosted configuration missing, Please check you configuration.");

            var encoder = (HostedPaymentConfig.Version == HppVersion.VERSION_2) ? null : JsonEncoders.Base64Encoder;
            var request = new JsonDoc(encoder);

            var orderId = builder.OrderId ?? GenerationUtils.GenerateOrderId();
            var timestamp = builder.Timestamp ?? GenerationUtils.GenerateTimestamp();

            // check for right transaction types
            if (builder.TransactionType != TransactionType.Sale && builder.TransactionType != TransactionType.Auth && builder.TransactionType != TransactionType.Verify)
                throw new UnsupportedTransactionException("Only Charge and Authorize are supported through hpp.");

            request.Set("MERCHANT_ID", MerchantId);
            request.Set("ACCOUNT", AccountId);
            request.Set("CHANNEL", Channel);
            request.Set("ORDER_ID", orderId);
            if(builder.Amount != null)
                request.Set("AMOUNT", builder.Amount.ToNumericCurrencyString());
            request.Set("CURRENCY", builder.Currency);
            request.Set("TIMESTAMP", timestamp);
            request.Set("AUTO_SETTLE_FLAG", (builder.TransactionType == TransactionType.Sale) ? "1" : "0");
            request.Set("COMMENT1", builder.Description);
            // request.Set("COMMENT2", );
            if(HostedPaymentConfig.RequestTransactionStabilityScore.HasValue)
                request.Set("RETURN_TSS", HostedPaymentConfig.RequestTransactionStabilityScore.Value ? "1" : "0");
            if(HostedPaymentConfig.DynamicCurrencyConversionEnabled.HasValue)
                request.Set("DCC_ENABLE", HostedPaymentConfig.DynamicCurrencyConversionEnabled.Value ? "1" : "0");
            if (builder.HostedPaymentData != null) {
                request.Set("CUST_NUM", builder.HostedPaymentData.CustomerNumber);
                if(HostedPaymentConfig.DisplaySavedCards.HasValue && builder.HostedPaymentData.CustomerKey != null)
                    request.Set("HPP_SELECT_STORED_CARD", builder.HostedPaymentData.CustomerKey);
                if(builder.HostedPaymentData.OfferToSaveCard.HasValue)
                    request.Set("OFFER_SAVE_CARD", builder.HostedPaymentData.OfferToSaveCard.Value ? "1" : "0");
                if(builder.HostedPaymentData.CustomerExists.HasValue)
                request.Set("PAYER_EXIST", builder.HostedPaymentData.CustomerExists.Value ? "1" : "0");
                if (!HostedPaymentConfig.DisplaySavedCards.HasValue)
                    request.Set("PAYER_REF", builder.HostedPaymentData.CustomerKey);
                request.Set("PMT_REF", builder.HostedPaymentData.PaymentKey);
                request.Set("PROD_ID", builder.HostedPaymentData.ProductId);
            }
            if (builder.ShippingAddress != null) {
                request.Set("SHIPPING_CODE", builder.ShippingAddress.PostalCode);
                request.Set("SHIPPING_CO", builder.ShippingAddress.Country);
            }
            if (builder.BillingAddress != null) {
                request.Set("BILLING_CODE", builder.BillingAddress.PostalCode);
                request.Set("BILLING_CO", builder.BillingAddress.Country);
            }
            request.Set("CUST_NUM", builder.CustomerId);
            request.Set("VAR_REF", builder.ClientTransactionId);
            request.Set("HPP_LANG", HostedPaymentConfig.Language);
            request.Set("MERCHANT_RESPONSE_URL", HostedPaymentConfig.ResponseUrl);
            request.Set("CARD_PAYMENT_BUTTON", HostedPaymentConfig.PaymentButtonText);
            if(HostedPaymentConfig.CardStorageEnabled.HasValue)
                request.Set("CARD_STORAGE_ENABLE", HostedPaymentConfig.CardStorageEnabled.Value ? "1" : "0");
            if (builder.TransactionType == TransactionType.Verify)
                request.Set("VALIDATE_CARD_ONLY", builder.TransactionType == TransactionType.Verify ? "1" : "0");
            if (HostedPaymentConfig.FraudFilterMode != FraudFilterMode.NONE)
                request.Set("HPP_FRAUDFILTER_MODE", HostedPaymentConfig.FraudFilterMode.ToString());
            if (builder.RecurringType != null || builder.RecurringSequence != null) {
                request.Set("RECURRING_TYPE", builder.RecurringType.ToString().ToLower());
                request.Set("RECURRING_SEQUENCE", builder.RecurringSequence.ToString().ToLower());
            }
            request.Set("HPP_VERSION", HostedPaymentConfig.Version);
            request.Set("HPP_POST_DIMENSIONS", HostedPaymentConfig.PostDimensions);
            request.Set("HPP_POST_RESPONSE", HostedPaymentConfig.PostResponse);

            var toHash = new List<string> {
                timestamp,
                MerchantId,
                orderId,
                (builder.Amount != null) ? builder.Amount.ToNumericCurrencyString() : null,
                builder.Currency
            };

            if (HostedPaymentConfig.CardStorageEnabled.HasValue || (builder.HostedPaymentData != null && builder.HostedPaymentData.OfferToSaveCard.HasValue) || HostedPaymentConfig.DisplaySavedCards.HasValue) {
                toHash.Add(builder.HostedPaymentData.CustomerKey ?? null);
                toHash.Add(builder.HostedPaymentData.PaymentKey ?? null);
            }

            if (HostedPaymentConfig.FraudFilterMode != FraudFilterMode.NONE) {
                toHash.Add(HostedPaymentConfig.FraudFilterMode.ToString());
            }

            request.Set("SHA1HASH", GenerationUtils.GenerateHash(SharedSecret, toHash.ToArray()));
            return request.ToString();
        }

        public Transaction ManageTransaction(ManagementBuilder builder) {
            var et = new ElementTree();
            string timestamp = GenerationUtils.GenerateTimestamp();
            string orderId = builder.OrderId ?? GenerationUtils.GenerateOrderId();

            // Build Request
            var request = et.Element("request")
                .Set("timestamp", timestamp)
                .Set("type", MapManageRequestType(builder.TransactionType));
            et.SubElement(request, "merchantid").Text(MerchantId);
            et.SubElement(request, "account", AccountId);
            et.SubElement(request, "channel", Channel);
            et.SubElement(request, "orderid", orderId);
            et.SubElement(request, "pasref", builder.TransactionId);
            if (builder.Amount.HasValue)
                et.SubElement(request, "amount", builder.Amount.ToNumericCurrencyString()).Set("currency", builder.Currency);
            else if (builder.TransactionType == TransactionType.Capture)
                throw new BuilderException("Amount cannot be null for capture.");

            // payer authentication response
            if (builder.TransactionType == TransactionType.VerifySignature)
                et.SubElement(request, "pares", builder.PayerAuthenticationResponse);

            // rebate hash
            if (builder.TransactionType == TransactionType.Refund) {
                et.SubElement(request, "authcode").Text(builder.AuthorizationCode);
                et.SubElement(request, "refundhash", GenerationUtils.GenerateHash(RebatePassword ?? string.Empty));
            }

            // reason code
            if (builder.ReasonCode != null)
                et.SubElement(request, "reasoncode").Text(builder.ReasonCode.ToString());

            // TODO: needs to be multiple
            if (builder.Description != null) {
                var comments = et.SubElement(request, "comments");
                et.SubElement(comments, "comment", builder.Description).Set("id", "1");
            }

            et.SubElement(request, "sha1hash", GenerationUtils.GenerateHash(SharedSecret, timestamp, MerchantId, orderId, builder.Amount.ToNumericCurrencyString(), builder.Currency, ""));

            var response = DoTransaction(et.ToString(request));
            return MapResponse(response);
        }

        public T ProcessReport<T>(ReportBuilder<T> builder) where T : class {
            throw new UnsupportedTransactionException("Reporting functionality is not supported through this gateway.");
        }

        public TResult ProcessRecurring<TResult>(RecurringBuilder<TResult> builder) where TResult : class {
            var et = new ElementTree();
            string timestamp = GenerationUtils.GenerateTimestamp();
            string orderId = builder.OrderId ?? GenerationUtils.GenerateOrderId();

            // Build Request
            var request = et.Element("request")
                .Set("type", MapRecurringRequestType(builder))
                .Set("timestamp", timestamp);
            et.SubElement(request, "merchantid").Text(MerchantId);
            et.SubElement(request, "account", AccountId);
            et.SubElement(request, "orderid", orderId);

            if (builder.TransactionType == TransactionType.Create || builder.TransactionType == TransactionType.Edit) {
                if (builder.Entity is Customer) {
                    var customer = builder.Entity as Customer;
                    request.Append(BuildCustomer(et, customer));
                    et.SubElement(request, "sha1hash").Text(GenerationUtils.GenerateHash(SharedSecret, timestamp, MerchantId, orderId, null, null, customer.Key));
                }
                else if (builder.Entity is RecurringPaymentMethod) {
                    var payment = builder.Entity as RecurringPaymentMethod;
                    var cardElement = et.SubElement(request, "card");
                    et.SubElement(cardElement, "ref").Text(payment.Key ?? payment.Id);
                    et.SubElement(cardElement, "payerref").Text(payment.CustomerKey);

                    if (payment.PaymentMethod != null) {
                        var card = payment.PaymentMethod as CreditCardData;
                        string expiry = card.ShortExpiry;
                        et.SubElement(cardElement, "number").Text(card.Number);
                        et.SubElement(cardElement, "expdate").Text(expiry);
                        et.SubElement(cardElement, "chname").Text(card.CardHolderName);
                        et.SubElement(cardElement, "type").Text(card.CardType);

                        string sha1hash = string.Empty;
                        if (builder.TransactionType == TransactionType.Create)
                            sha1hash = GenerationUtils.GenerateHash(SharedSecret, timestamp, MerchantId, orderId, null, null, payment.CustomerKey, card.CardHolderName, card.Number);
                        else sha1hash = GenerationUtils.GenerateHash(SharedSecret, timestamp, MerchantId, payment.CustomerKey, payment.Key ?? payment.Id, expiry, card.Number);
                        et.SubElement(request, "sha1hash").Text(sha1hash);
                    }
                }
            }
            else if (builder.TransactionType == TransactionType.Delete) {
                if (builder.Entity is RecurringPaymentMethod) {
                    var payment = builder.Entity as RecurringPaymentMethod;
                    var cardElement = et.SubElement(request, "card");
                    et.SubElement(cardElement, "ref").Text(payment.Key ?? payment.Id);
                    et.SubElement(cardElement, "payerref").Text(payment.CustomerKey);

                    string sha1hash = GenerationUtils.GenerateHash(SharedSecret, timestamp, MerchantId, payment.CustomerKey, payment.Key ?? payment.Id);
                    et.SubElement(request, "sha1hash").Text(sha1hash);
                }
            }

            var response = DoTransaction(et.ToString(request));
            return MapRecurringResponse<TResult>(response, builder);
        }

        #endregion

        #region response mapping
        private Transaction MapResponse(string rawResponse) {
            var root = new ElementTree(rawResponse).Get("response");

            CheckResponse(root);
            var result = new Transaction {
                ResponseCode = root.GetValue<string>("result"),
                ResponseMessage = root.GetValue<string>("message"),
                CvnResponseCode = root.GetValue<string>("cvnresult"),
                AvsResponseCode = root.GetValue<string>("avspostcoderesponse"),
                Timestamp = root.GetAttribute<string>("timestamp"),
                TransactionReference = new TransactionReference {
                    AuthCode = root.GetValue<string>("authcode"),
                    OrderId = root.GetValue<string>("orderid"),
                    PaymentMethodType = PaymentMethodType.Credit,
                    TransactionId = root.GetValue<string>("pasref")
                }
            };

            // 3d secure enrolled
            if (root.Has("enrolled")) {
                result.ThreeDSecure = new ThreeDSecure();
                result.ThreeDSecure.Enrolled = root.GetValue<string>("enrolled") == "Y";
                result.ThreeDSecure.PayerAuthenticationRequest = root.GetValue<string>("pareq");
                result.ThreeDSecure.Xid = root.GetValue<string>("xid");
                result.ThreeDSecure.IssuerAcsUrl = root.GetValue<string>("url");
            }

            // threedsecure
            if (root.Has("threedsecure")) {
                result.ThreeDSecure = new ThreeDSecure();
                result.ThreeDSecure.Status = root.GetValue<string>("status");
                result.ThreeDSecure.Eci = root.GetValue<string>("eci");
                result.ThreeDSecure.Xid = root.GetValue<string>("xid");
                result.ThreeDSecure.Cavv = root.GetValue<string>("cavv");
                result.ThreeDSecure.Algorithm = root.GetValue<int>("algorithm");
            }

            return result;
        }

        private TResult MapRecurringResponse<TResult>(string rawResponse, RecurringBuilder<TResult> builder) where TResult : class {
            var root = new ElementTree(rawResponse).Get("response");

            // check response
            CheckResponse(root);
            return builder.Entity as TResult;
        }

        private void CheckResponse(Element root, List<string> acceptedCodes = null) {
            if (acceptedCodes == null)
                acceptedCodes = new List<string> { "00" };

            var responseCode = root.GetValue<string>("result");
            var responseMessage = root.GetValue<string>("message");
            if (!acceptedCodes.Contains(responseCode)) {
                throw new GatewayException(
                    string.Format("Unexpected Gateway Response: {0} - {1}", responseCode, responseMessage),
                    responseCode,
                    responseMessage
                );
            }
        }
        #endregion

        #region transaction type mapping
        private string MapAuthRequestType<T>(T builder) where T : TransactionBuilder<Transaction> {
            var trans = builder.TransactionType;
            var payment = builder.PaymentMethod;

            switch (trans) {
                case TransactionType.Sale:
                case TransactionType.Auth:
                    if (payment is Credit) {
                        if (builder.TransactionModifier == TransactionModifier.Offline) {
                            if (builder.PaymentMethod != null)
                                return "manual";
                            return "offline";
                        }
                        return "auth";
                    }
                    else return "receipt-in";
                case TransactionType.Capture:
                    return "settle";
                case TransactionType.Verify:
                    if (payment is Credit)
                        return "otb";
                    else {
                        if (builder.TransactionModifier == TransactionModifier.Secure3D)
                            return "realvault-3ds-verifyenrolled";
                        else return "receipt-in-otb";
                    }
                case TransactionType.Refund:
                    if (payment is Credit)
                        return "credit";
                    else return "payment-out";
                case TransactionType.VerifyEnrolled:
                    return "3ds-verifyenrolled";
                default:
                    throw new UnsupportedTransactionException();
            }
        }

        private string MapManageRequestType(TransactionType trans) {
            switch (trans) {
                case TransactionType.Capture:
                    return "settle";
                case TransactionType.Hold:
                    return "hold";
                case TransactionType.Refund:
                    return "rebate";
                case TransactionType.Release:
                    return "release";
                case TransactionType.Void:
                case TransactionType.Reversal:
                    return "void";
                case TransactionType.VerifySignature:
                    return "3ds-verifysig";
                default:
                    return "unknown";
            }
        }

        private string MapRecurringRequestType<T>(RecurringBuilder<T> builder) where T : class {
            var entity = builder.Entity;
            switch (builder.TransactionType) {
                case TransactionType.Create:
                    if (entity is Customer)
                        return "payer-new";
                    else if (entity is IPaymentMethod)
                        return "card-new";
                    throw new UnsupportedTransactionException();
                case TransactionType.Edit:
                    if (entity is Customer)
                        return "payer-edit";
                    else if (entity is IPaymentMethod)
                        return "card-update-card";
                    throw new UnsupportedTransactionException();
                case TransactionType.Delete:
                    if (entity is RecurringPaymentMethod)
                        return "card-cancel-card";
                    throw new UnsupportedTransactionException();
                default:
                    throw new UnsupportedTransactionException();
            }
        }
        #endregion

        #region hydration
        private Element BuildCustomer(ElementTree et, Customer customer) {
            var payer = et.Element("payer")
                        .Set("ref", customer.Key ?? GenerationUtils.GenerateRecurringKey())
                        .Set("type", "Retail");
            et.SubElement(payer, "title", customer.Title);
            et.SubElement(payer, "firstname", customer.FirstName);
            et.SubElement(payer, "surname", customer.LastName);
            et.SubElement(payer, "company", customer.Company);

            if (customer.Address != null) {
                var address = et.SubElement(payer, "address");
                et.SubElement(address, "line1", customer.Address.StreetAddress1);
                et.SubElement(address, "line2", customer.Address.StreetAddress2);
                et.SubElement(address, "line3", customer.Address.StreetAddress3);
                et.SubElement(address, "city", customer.Address.City);
                et.SubElement(address, "county", customer.Address.Province);
                et.SubElement(address, "postcode", customer.Address.PostalCode);
                var country = et.SubElement(address, "country", customer.Address.Country);
                if (country != null)
                    country.Set("code", "GB"); // TODO: I need a mapping for this somehow
            }

            var phone = et.SubElement(payer, "phonenumbers");
            et.SubElement(phone, "home", customer.HomePhone);
            et.SubElement(phone, "work", customer.WorkPhone);
            et.SubElement(phone, "fax", customer.Fax);
            et.SubElement(phone, "mobile", customer.MobilePhone);

            et.SubElement(payer, "email", customer.Email);

            // comments
            return payer;
        }

        private Element BuildAddress(ElementTree et, Address address) {
            if (address == null)
                return null;

            var code = address.PostalCode;
            if (!string.IsNullOrEmpty(code) && !code.Contains("|")) {
                code = string.Format("{0}|{1}", address.PostalCode, address.StreetAddress1);
                if (address.Country == "GB") {
                    code = string.Format("{0}|{1}", Regex.Replace(address.PostalCode, "[^0-9]", ""), Regex.Replace(address.StreetAddress1, "[^0-9]", ""));
                }
            }

            var addressNode = et.Element("address").Set("type", address.Type == AddressType.Billing ? "billing" : "shipping");
            et.SubElement(addressNode, "code").Text(code);
            et.SubElement(addressNode, "country").Text(address.Country);

            return addressNode;
        }
        #endregion
    }
}
