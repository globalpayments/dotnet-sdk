using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Utils.Logging;

namespace GlobalPayments.Api.Tests.GpEcom.Hpp {
    public class RealexResponseHandler : DelegatingHandler {
        private string _sharedSecret;
        private IPaymentMethod paymentMethod;
        private ShaHashType _shaHashType;

        public RealexResponseHandler(string sharedSecret, ShaHashType shaHashType = ShaHashType.SHA1) {
            _sharedSecret = sharedSecret;
            _shaHashType = shaHashType;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            string response = await request.Content.ReadAsStringAsync();

            // gather information
            var json = JsonDoc.Parse(response, JsonEncoders.Base64Encoder);
            var timestamp = json.GetValue<string>("TIMESTAMP");
            var merchantId = json.GetValue<string>("MERCHANT_ID");
            var account = json.GetValue<string>("ACCOUNT");
            var orderId = json.GetValue<string>("ORDER_ID");
            var amount = json.GetValue<string>("AMOUNT");
            var currency = json.GetValue<string>("CURRENCY");
            var autoSettle = json.GetValue<int>("AUTO_SETTLE_FLAG") == 1;
            var description = json.GetValue<string>("COMMENT1");            
            var shaHashTagName = _shaHashType + "HASH";
            var requestHash = json.GetValue<string>(shaHashTagName);

            // gather additional information
            var shippingCode = json.GetValue<string>("SHIPPING_CODE");
            var shippingCountry = json.GetValue<string>("SHIPPING_CO");
            var billingCode = json.GetValue<string>("BILLING_CODE");
            var billingCountry = json.GetValue<string>("BILLING_CO");
            var fraudFilterMode = json.GetValue<string>("HPP_FRAUDFILTER_MODE");
            BlockedCardType cardTypesBlocking = null;

            if (!string.IsNullOrEmpty(json.GetValue<string>("BLOCK_CARD_TYPE"))) {
                var cardTypes = json.GetValue<string>("BLOCK_CARD_TYPE");
                cardTypesBlocking = new BlockedCardType();
                foreach (var cardType in cardTypes.Split("|")) {
                    var cardTypeEnum = EnumConverter.FromDescription<BlockCardType>(cardType);
                    switch (cardTypeEnum)
                    {
                        case BlockCardType.COMMERCIAL_CREDIT:
                            cardTypesBlocking.Commercialcredit = true;
                            break;
                        case BlockCardType.COMMERCIAL_DEBIT:
                            cardTypesBlocking.Commercialdebit = true;
                            break;
                        case BlockCardType.CONSUMER_CREDIT:
                            cardTypesBlocking.Consumercredit = true;
                            break;
                        case BlockCardType.CONSUMER_DEBIT:
                            cardTypesBlocking.Consumerdebit = true;
                            break;
                        default:
                            break;
                    }
                }                
            }


            List<string> hashParam = new List<string>
            {
                timestamp,
                merchantId,
                orderId,
                amount,
                currency
            };

            //create the card/APM/LPM/OB object
            if (json.Has("PM_METHODS")) {
                string[] apmTypes = json.GetValue<string>("PM_METHODS").Split("|");
                string apmType = apmTypes[0];                
               
                //OB
                if (apmTypes.Contains(HostedPaymentMethods.OB.ToString())) {
                    var card = new BankPayment { 
                        SortCode = json.GetValue<string>("HPP_OB_DST_ACCOUNT_SORT_CODE"),
                        AccountNumber = json.GetValue<string>("HPP_OB_DST_ACCOUNT_NUMBER"),
                        AccountName = json.GetValue<string>("HPP_OB_DST_ACCOUNT_NAME"),
                        BankPaymentType = (BankPaymentType)(Enum.Parse(typeof(BankPaymentType), json.GetValue<string>("HPP_OB_PAYMENT_SCHEME"))),
                        Iban = json.GetValue<string>("HPP_OB_DST_ACCOUNT_IBAN"),
                        ReturnUrl = json.GetValue<string>("MERCHANT_RESPONSE_URL"),
                        StatusUpdateUrl = json.GetValue<string>("HPP_TX_STATUS_URL")
                    };

                    paymentMethod = card;

                    if (!string.IsNullOrEmpty(card.SortCode))
                        hashParam.Add(card.SortCode);
                    if (!string.IsNullOrEmpty(card.AccountNumber))
                        hashParam.Add(card.AccountNumber);
                    if (!string.IsNullOrEmpty(card.Iban))
                        hashParam.Add(card.Iban);
                   
                }
                else {
                    AlternativePaymentMethod apm = new AlternativePaymentMethod();
                    apm.AlternativePaymentMethodType = (AlternativePaymentType)(Enum.Parse(typeof(AlternativePaymentType), apmType));
                    apm.ReturnUrl = json.GetValue<string>("MERCHANT_RESPONSE_URL");
                    apm.StatusUpdateUrl = json.GetValue<string>("HPP_TX_STATUS_URL");                    

                    if (apmType.Equals(AlternativePaymentType.PAYPAL.ToString())) {
                        apm.CancelUrl = "https://www.example.com/failure/cancelURL";
                    }
                    apm.Country = json.GetValue<string>("HPP_CUSTOMER_COUNTRY");
                    apm.AccountHolderName = json.GetValue<string>("HPP_CUSTOMER_FIRSTNAME") + " " + json.GetValue<string>("HPP_CUSTOMER_LASTNAME");
                    
                    paymentMethod = apm;
                }
                
            }
            else {
                CreditCardData card = new CreditCardData {
                    Number = "4111111111111111",
                    ExpMonth = 12,
                    ExpYear = 2025,
                    Cvn = "123",
                    CardHolderName = "John Smithe"
                };

                paymentMethod = card;
            }

            //for stored card
            if (json.Has("OFFER_SAVE_CARD")) {
                if(json.Has("PAYER_REF"))
                    hashParam.Add(json.GetValue<string>("PAYER_REF"));
                if(json.Has("PMT_REF"))
                    hashParam.Add(json.GetValue<string>("PMT_REF"));
            }

            if (json.Has("HPP_FRAUDFILTER_MODE")) {
                hashParam.Add(json.GetValue<string>("HPP_FRAUDFILTER_MODE"));
            }

            // check hash
            var newhash = GenerationUtils.GenerateHash(_sharedSecret, _shaHashType, hashParam.ToArray());
            if (!newhash.Equals(requestHash)) {
                return BadRequest("Incorrect hash. Please check your code and the Developers Documentation.");
            }

            // configure the container
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = merchantId,
                AccountId = account,
                SharedSecret = _sharedSecret,
                ShaHashType = _shaHashType,
                RequestLogger = new RequestConsoleLogger()
            }, "realexResponder");                       

            // build request
            AuthorizationBuilder gatewayRequest = null;
            if (amount.ToAmount().Equals(0m) || amount == null) {
                var validate = json.GetValue<int>("VALIDATE_CARD_ONLY") == 1;
                if (validate)
                    gatewayRequest = ((CreditCardData)paymentMethod).Verify();
                else gatewayRequest = ((CreditCardData)paymentMethod).Verify().WithRequestMultiUseToken(true);
            }
            else {
                if (autoSettle)
                {
                    if (paymentMethod is CreditCardData)
                    {
                        gatewayRequest = ((CreditCardData)paymentMethod).Charge(amount.ToAmount());
                    }
                    if (paymentMethod is AlternativePaymentMethod)
                    {
                        gatewayRequest = ((AlternativePaymentMethod)paymentMethod).Charge(amount.ToAmount());
                    }
                    if(paymentMethod is BankPayment)
                    {
                        var gatewayBankRequest = AddRemittanceRef(((BankPayment)paymentMethod).Charge(amount.ToAmount())
                            .WithCurrency(currency)
                            .WithDescription(description),json);
                        var gatewayResponse = gatewayBankRequest.Execute();
                        if (gatewayResponse.BankPaymentResponse.PaymentStatus.Equals("PAYMENT_INITIATED"))
                            return BuildResponse(HttpStatusCode.OK, ConvertResponse(json, gatewayResponse));
                        else return BadRequest(gatewayResponse.ResponseMessage);
                    }
                }
                else
                {
                    gatewayRequest = ((CreditCardData)paymentMethod).Authorize(amount.ToAmount());
                }
            }

            try {
                gatewayRequest.WithCurrency(currency).WithOrderId(orderId).WithTimestamp(timestamp);
                if (billingCode != null || billingCountry != null) {
                    gatewayRequest.WithAddress(new Address { PostalCode = billingCode, Country = billingCountry });
                }
                if (shippingCode != null || shippingCountry != null) {
                    gatewayRequest.WithAddress(new Address { PostalCode = shippingCode, Country = shippingCountry }, AddressType.Shipping);
                }

                if(fraudFilterMode != null)
                {
                    gatewayRequest.WithFraudFilter((FraudFilterMode)Enum.Parse(typeof(FraudFilterMode), fraudFilterMode), getFraudFilterRules(json));
                }
                if (cardTypesBlocking != null)
                {
                    gatewayRequest.WithBlockedCardType(cardTypesBlocking); 
                }

                var gatewayResponse = gatewayRequest.Execute("realexResponder");
                if (gatewayResponse.ResponseCode.Equals("00") || gatewayResponse.ResponseCode.Equals("01"))
                    return BuildResponse(HttpStatusCode.OK, ConvertResponse(json, gatewayResponse));
                else return BadRequest(gatewayResponse.ResponseMessage);
            }
            catch (ApiException exc) {
                return ServerError(exc.Message);
            }
        }

        private AuthorizationBuilder AddRemittanceRef(AuthorizationBuilder gatewayRequest ,JsonDoc json) {
            var REF_TYPE = json.GetValue<string>("HPP_OB_REMITTANCE_REF_TYPE");
            var REF_VALUE = json.GetValue<string>("HPP_OB_REMITTANCE_REF_VALUE");
            return gatewayRequest.WithRemittanceReference(
                (RemittanceReferenceType)Enum.Parse(typeof(RemittanceReferenceType), REF_TYPE),
                REF_VALUE
            );
        }

        private FraudRuleCollection getFraudFilterRules(JsonDoc json)
        {
            var hppKeys = json.Keys.Where(x => x.StartsWith("HPP_FRAUDFILTER_RULE_")).ToList();
            if (hppKeys == null || !hppKeys.Any())
                return null;
            FraudRuleCollection rules = new FraudRuleCollection();
            foreach (var key in hppKeys) 
            {
                rules.AddRule(key.Replace("HPP_FRAUDFILTER_RULE_", ""), (FraudFilterMode) Enum.Parse(typeof(FraudFilterMode),json.GetValue<string>(key)));
            }
            return rules;
        }

        private HttpResponseMessage BuildResponse(HttpStatusCode code, string reason) {
            return new HttpResponseMessage(code) { Content = new StringContent(reason) };
        }

        private HttpResponseMessage BadRequest(string reason = "") {
            return BuildResponse(HttpStatusCode.BadRequest, reason);
        }

        private HttpResponseMessage ServerError(string reason = "") {
            return BuildResponse(HttpStatusCode.InternalServerError, reason);
        }

        private string ConvertBankResponse(JsonDoc request, Transaction trans)
        {
            var merchantId = request.GetValue<string>("MERCHANT_ID");
            var account = request.GetValue<string>("ACCOUNT");

            // begin building response
            var response = new JsonDoc(JsonEncoders.Base64Encoder);
            response.Set("MERCHANT_ID", merchantId);
            response.Set("ACCOUNT", request.GetValue<string>("ACCOUNT"));
            response.Set("ORDER_ID", trans.OrderId);
            response.Set("TIMESTAMP", trans.Timestamp);
            response.Set("RESULT", trans.ResponseCode);
            response.Set("PASREF", trans.TransactionId);
            response.Set("AUTHCODE", trans.AuthorizationCode);
            response.Set("AVSPOSTCODERESULT", trans.AvsResponseCode);
            response.Set("CVNRESULT", trans.CvnResponseCode);
            response.Set("HPP_LANG", request.GetValue<string>("HPP_LANG"));
            response.Set("SHIPPING_CODE", request.GetValue<string>("SHIPPING_CODE"));
            response.Set("SHIPPING_CO", request.GetValue<string>("SHIPPING_CO"));
            response.Set("BILLING_CODE", request.GetValue<string>("BILLING_CODE"));
            response.Set("BILLING_CO", request.GetValue<string>("BILLING_CO"));
            response.Set("ECI", request.GetValue<string>("ECI"));
            response.Set("CAVV", request.GetValue<string>("CAVV"));
            response.Set("XID", request.GetValue<string>("XID"));
            response.Set("MERCHANT_RESPONSE_URL", request.GetValue<string>("MERCHANT_RESPONSE_URL"));
            response.Set("CARD_PAYMENT_BUTTON", request.GetValue<string>("CARD_PAYMENT_BUTTON"));
            response.Set("MESSAGE", trans.ResponseMessage);
            response.Set("AMOUNT", trans.AuthorizedAmount);
            response.Set("SHA1HASH", GenerationUtils.GenerateHash(_sharedSecret, trans.Timestamp, merchantId, trans.OrderId, trans.ResponseCode, trans.ResponseMessage, trans.TransactionId, trans.AuthorizationCode));
            response.Set("DCC_INFO_REQUST", request.GetValue<string>("DCC_INFO"));
            response.Set("HPP_FRAUDFILTER_MODE", request.GetValue<string>("HPP_FRAUDFILTER_MODE"));
            response.Set("REDIRECT_URL", trans.BankPaymentResponse?.RedirectUrl);



            return response.ToString();
        }

        private string ConvertResponse(JsonDoc request, Transaction trans) {
            var merchantId = request.GetValue<string>("MERCHANT_ID");
            var account = request.GetValue<string>("ACCOUNT");

            // begin building response
            var response = new JsonDoc(JsonEncoders.Base64Encoder);            
                response.Set("MERCHANT_ID", merchantId);
                response.Set("ACCOUNT", request.GetValue<string>("ACCOUNT"));
                response.Set("ORDER_ID", trans.OrderId);
                response.Set("TIMESTAMP", trans.Timestamp);
                response.Set("RESULT", trans.ResponseCode);
                response.Set("PASREF", trans.TransactionId);
                response.Set("AUTHCODE", trans.AuthorizationCode);
                response.Set("AVSPOSTCODERESULT", trans.AvsResponseCode);
                response.Set("CVNRESULT", trans.CvnResponseCode);
                response.Set("HPP_LANG", request.GetValue<string>("HPP_LANG"));
                response.Set("SHIPPING_CODE", request.GetValue<string>("SHIPPING_CODE"));
                response.Set("SHIPPING_CO", request.GetValue<string>("SHIPPING_CO"));
                response.Set("BILLING_CODE", request.GetValue<string>("BILLING_CODE"));
                response.Set("BILLING_CO", request.GetValue<string>("BILLING_CO"));
                response.Set("ECI", request.GetValue<string>("ECI"));
                response.Set("CAVV", request.GetValue<string>("CAVV"));
                response.Set("XID", request.GetValue<string>("XID"));
                response.Set("MERCHANT_RESPONSE_URL", request.GetValue<string>("MERCHANT_RESPONSE_URL"));
                response.Set("CARD_PAYMENT_BUTTON", request.GetValue<string>("CARD_PAYMENT_BUTTON"));
                response.Set("MESSAGE", trans.ResponseMessage);
                response.Set("AMOUNT", trans.AuthorizedAmount);
                response.Set($"{_shaHashType}HASH", GenerationUtils.GenerateHash(_sharedSecret, _shaHashType, trans.Timestamp, merchantId, trans.OrderId, trans.ResponseCode, trans.ResponseMessage, trans.TransactionId, trans.AuthorizationCode));
                response.Set("DCC_INFO_REQUST", request.GetValue<string>("DCC_INFO"));
                response.Set("HPP_FRAUDFILTER_MODE", request.GetValue<string>("HPP_FRAUDFILTER_MODE"));
                response.Set("BLOCK_CARD_TYPE", request.GetValue<string>("BLOCK_CARD_TYPE"));
            if (trans?.FraudResponse?.Rules != null)
                {
                    response.Set("HPP_FRAUDFILTER_RESULT", trans.FraudResponse?.Result);

                    foreach (var rule in trans.FraudResponse.Rules)
                    {
                        response.Set("HPP_FRAUDFILTER_RULE_" + rule.Id, rule.Action);
                    }
                }
                if (trans?.AlternativePaymentResponse != null)
                {
                    AlternativePaymentResponse alternativePaymentResponse = trans.AlternativePaymentResponse;
                    response.Set("HPP_CUSTOMER_FIRSTNAME", request.GetValue<string>("HPP_CUSTOMER_FIRSTNAME"));
                    response.Set("HPP_CUSTOMER_LASTNAME", request.GetValue<string>("HPP_CUSTOMER_LASTNAME"));
                    response.Set("HPP_CUSTOMER_COUNTRY", request.GetValue<string>("HPP_CUSTOMER_COUNTRY"));
                    response.Set("PAYMENTMETHOD", alternativePaymentResponse.ProviderName);
                    response.Set("PAYMENTPURPOSE", alternativePaymentResponse.PaymentPurpose);
                    response.Set("HPP_CUSTOMER_BANK_ACCOUNT", alternativePaymentResponse.BankAccount);
                }          

            return response.ToString();
        }
    }

    public class RealexHppClient {
        private string _serviceUrl;
        private string _sharedSecret;
        private ShaHashType _shaHashType;

        public RealexHppClient(string url, string sharedSecret, ShaHashType shaHashType = ShaHashType.SHA1) {
            _serviceUrl = url;
            _sharedSecret = sharedSecret;
            _shaHashType = shaHashType;
        }

        public string SendRequest(string json) {
            HttpClient httpClient = new HttpClient(new RealexResponseHandler(_sharedSecret, _shaHashType), true) {
                Timeout = TimeSpan.FromMilliseconds(60000)
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _serviceUrl);
            HttpResponseMessage response = null;
            try {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                response = httpClient.SendAsync(request).Result;
                var rawResponse = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode != HttpStatusCode.OK) {
                    throw new Exception(rawResponse);
                }
                return rawResponse;
            }
            catch (Exception ex) {
                throw;
            }
            finally { }
        }
    }
}
