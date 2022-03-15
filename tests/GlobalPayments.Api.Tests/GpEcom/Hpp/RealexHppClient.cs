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
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Tests.Realex.Hpp {
    public class RealexResponseHandler : DelegatingHandler {
        private string _sharedSecret;
        private IPaymentMethod paymentMethod;

        public RealexResponseHandler(string sharedSecret) {
            _sharedSecret = sharedSecret;           
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
            var requestHash = json.GetValue<string>("SHA1HASH");

            // gather additional information
            var shippingCode = json.GetValue<string>("SHIPPING_CODE");
            var shippingCountry = json.GetValue<string>("SHIPPING_CO");
            var billingCode = json.GetValue<string>("BILLING_CODE");
            var billingCountry = json.GetValue<string>("BILLING_CO");
            var fraudFilterMode = json.GetValue<string>("HPP_FRAUDFILTER_MODE");


            List<string> hashParam = new List<string>
            {
                timestamp,
                merchantId,
                orderId,
                amount,
                currency
            };

            //create the card/APM/LPM object
            if (json.Has("PM_METHODS")) {
                string[] apmTypes = json.GetValue<string>("PM_METHODS").Split("\\|");
                string apmType = apmTypes[0];

                AlternativePaymentMethod apm = new AlternativePaymentMethod();
                apm.AlternativePaymentMethodType = (AlternativePaymentType)(Enum.Parse(typeof(AlternativePaymentType), apmType));
                apm.ReturnUrl = json.GetValue<string>("MERCHANT_RESPONSE_URL");
                apm.StatusUpdateUrl = json.GetValue<string>("HPP_TX_STATUS_URL");

                if(apmType == AlternativePaymentType.PAYPAL.ToString()) {
                    //cancelUrl for Paypal example
                    apm.CancelUrl = "https://www.example.com/failure/cancelUrl";
                }

                apm.Country = json.GetValue<string>("HPP_CUSTOMER_COUNTRY");
                apm.AccountHolderName = json.GetValue<string>("HPP_CUSTOMER_FIRSTNAME") + " " + json.GetValue<string>("HPP_CUSTOMER_LASTNAME");

                paymentMethod = apm;
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
            if (json.Has("OFFER_SAVE_CARD"))
            {
                if(json.Has("PAYER_REF"))
                    hashParam.Add(json.GetValue<string>("PAYER_REF"));
                if(json.Has("PMT_REF"))
                    hashParam.Add(json.GetValue<string>("PMT_REF"));
            }

            if (json.Has("HPP_FRAUDFILTER_MODE"))
            {
                hashParam.Add(json.GetValue<string>("HPP_FRAUDFILTER_MODE"));
            }

            // check hash
            var newhash = GenerationUtils.GenerateHash(_sharedSecret, hashParam.ToArray());
            if (!newhash.Equals(requestHash)) {
                return BadRequest("Incorrect hash. Please check your code and the Developers Documentation.");
            }

            // configure the container
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = merchantId,
                AccountId = account,
                SharedSecret = _sharedSecret
            }, "realexResponder");
                       

            // build request
            AuthorizationBuilder gatewayRequest = null;
            if (amount == null) {
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

                var gatewayResponse = gatewayRequest.Execute("realexResponder");
                if (gatewayResponse.ResponseCode.Equals("00") || gatewayResponse.ResponseCode.Equals("01"))
                    return BuildResponse(HttpStatusCode.OK, ConvertResponse(json, gatewayResponse));
                else return BadRequest(gatewayResponse.ResponseMessage);
            }
            catch (ApiException exc) {
                return ServerError(exc.Message);
            }
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
            response.Set("SHA1HASH", GenerationUtils.GenerateHash(_sharedSecret, trans.Timestamp, merchantId, trans.OrderId, trans.ResponseCode, trans.ResponseMessage, trans.TransactionId, trans.AuthorizationCode));
            response.Set("DCC_INFO_REQUST", request.GetValue<string>("DCC_INFO"));
            response.Set("HPP_FRAUDFILTER_MODE", request.GetValue<string>("HPP_FRAUDFILTER_MODE"));            
            if(trans?.FraudResponse?.Rules != null)
            {
                response.Set("HPP_FRAUDFILTER_RESULT", trans.FraudResponse?.Result);

                foreach (var rule in trans.FraudResponse.Rules)
                {
                    response.Set("HPP_FRAUDFILTER_RULE_" + rule.Id, rule.Action);
                }
            }
            if(trans?.AlternativePaymentResponse != null) {
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

        public RealexHppClient(string url, string sharedSecret) {
            _serviceUrl = url;
            _sharedSecret = sharedSecret;
        }

        public string SendRequest(string json) {
            HttpClient httpClient = new HttpClient(new RealexResponseHandler(_sharedSecret), true) {
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
            catch (Exception) {
                throw;
            }
            finally { }
        }
    }
}
