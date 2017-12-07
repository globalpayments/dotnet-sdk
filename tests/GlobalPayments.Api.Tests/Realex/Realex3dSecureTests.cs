using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Tests.Realex {
    [TestClass]
    public class Realex3dSecureTests {
        [TestInitialize]
        public void Init() {
            ServicesContainer.Configure(new ServicesConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "3dsecure",
                SharedSecret = "secret",
                RebatePassword = "rebate",
                RefundPassword = "refund",
                ServiceUrl = "https://api.sandbox.realexpayments.com/epage-remote.cgi"
            });
        }

        [TestMethod]
        public void AcsClientTest() {
            var authClient = new ThreeDSecureAcsClient("https://pit.3dsecure.net/VbVTestSuiteService/pit1/acsService/paReq?summary=MTNmMzI4NzgtNTdmZi00OWEzLWJhZTAtYzFhNzAxMDJkMGNi");
            Assert.IsNotNull(authClient.Authenticate("eJxlUsFSwjAQvfsVTO82TSm0MNs4FVBwRkUF8ZomK1Rpimkr6NebYBEdc8jsy27evrwNnO3ydesddZkVKnao6zktVKKQmVrGznx2cRo5Z+wEZiuNOHxAUWtkcI1lyZfYymTs+KIjZYRt30tl0H2WPRpFIuQyDULsdTvoMJgm9/jGoOnCTBPXB3KAhk2LFVcVAy7ezic3LAgD2ouANBBy1JMh6zULyDcGxXNkK+S6WnMll5vS7GmxA7JPgChqVekPFgUekAOAWq/Zqqo2ZZ+Q7Xbr/r/visKtX4HYSiBHcdPaRqVh3mWSJcM7Nb7t0O1iGs6n7cXnI025N7hSk1EMxFaA5BUy36MhpX7Y8r1+J+hTI39/Djy3kqwZRl4DYGN7JE3GJn4fgDFfm+EcnnRAgLtNodBUGFd/YiBHwYOx9VZUxrVxdjEb1aPXy5f5k27Tmzo/v75N4ti6vS+wbJlxikb0m84CIJaCNIMkzfxN9OdffAF4VML9"));
        }

        // full cycle
        [TestMethod]
        public void FullCycleWithMerchantData() {
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = 2018,
                CardHolderName = "John Smith"
            };

            var enrolled = card.VerifyEnrolled(1m, "USD");
            if (enrolled) {
                var secureEcom = card.ThreeDSecure;
                if (secureEcom != null) {
                    // authenticate
                    var authClient = new ThreeDSecureAcsClient(secureEcom.IssuerAcsUrl);
                    var authResponse = authClient.Authenticate(secureEcom.PayerAuthenticationRequest, secureEcom.MerchantData);
                    string payerAuthenticationResponse = authResponse.pares;
                    string md = authResponse.md;

                    // verify signature
                    if (card.VerifySignature(payerAuthenticationResponse, md)) {
                        var response = card.Charge().Execute();
                        Assert.IsNotNull(response);
                        Assert.AreEqual("00", response.ResponseCode);
                    }
                    else Assert.Fail("Signature verification failed.");
                }
                else Assert.Fail("Secure3Data was null.");
            }
            else Assert.Fail("Card not enrolled.");
        }

        [TestMethod]
        public void FullCycleWithNoMerchantData() {
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = 2018,
                CardHolderName = "John Smith"
            };

            var amount = 10m;
            var currency = "USD";
            var orderId = GenerationUtils.GenerateOrderId();

            var enrolled = card.VerifyEnrolled(amount, currency, orderId);
            if (enrolled) {
                var secureEcom = card.ThreeDSecure;
                if (secureEcom != null) {
                    // authenticate
                    var authClient = new ThreeDSecureAcsClient(secureEcom.IssuerAcsUrl);
                    var authResponse = authClient.Authenticate(secureEcom.PayerAuthenticationRequest, secureEcom.MerchantData);
                    string payerAuthenticationResponse = authResponse.pares;
                    string md = authResponse.md;

                    // verify signature
                    if (card.VerifySignature(payerAuthenticationResponse, amount, currency, orderId)) {
                        var response = card.Charge(amount)
                            .WithCurrency(currency)
                            .WithOrderId(orderId)
                            .Execute();
                        Assert.IsNotNull(response);
                        Assert.AreEqual("00", response.ResponseCode);
                    }
                    else Assert.Fail("Signature verification failed.");
                }
                else Assert.Fail("Secure3Data was null.");
            }
            else Assert.Fail("Card not enrolled.");
        }

        [TestMethod]
        public void VerifyEnrolledTrue() {
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = 2018,
                CardHolderName = "John Smith"
            };

            var enrolled = card.VerifyEnrolled(1m, "USD");
            Assert.IsTrue(enrolled);
            Assert.IsNotNull(card.ThreeDSecure);
            Assert.IsNotNull(card.ThreeDSecure.PayerAuthenticationRequest);
            Assert.IsNotNull(card.ThreeDSecure.IssuerAcsUrl);
            Assert.IsNotNull(card.ThreeDSecure.Xid);
        }

        [TestMethod]
        public void VerifyEnrolledFalse() {
            var card = new CreditCardData {
                Number = "4012001038443335",
                ExpMonth = 12,
                ExpYear = 2018,
                CardHolderName = "John Smith"
            };

            var enrolled = card.VerifyEnrolled(1m, "USD");
            Assert.IsFalse(enrolled);
            Assert.IsNull(card.ThreeDSecure);
        }

        [TestMethod]
        public void VerifySignatureSuccess() {
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = 2018,
                CardHolderName = "John Smith"
            };

            var result = card.VerifySignature("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", 1m, "USD", "j1zEegmPFkGiKJerv6xXCg");
            Assert.IsTrue(result);
            Assert.IsNotNull(card.ThreeDSecure.Cavv);
            Assert.IsNotNull(card.ThreeDSecure.Status);
            Assert.IsNotNull(card.ThreeDSecure.Algorithm);
            Assert.IsNotNull(card.ThreeDSecure.Eci);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void VerifySignatureBadOrderId() {
            new CreditCardData().VerifySignature("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", 1m, "USD", "orderId");
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void VerifySignatureNoPaymentResponse() {
            new CreditCardData().VerifySignature(null);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void VerifySignatureNoAmount() {
            new CreditCardData().VerifySignature("paymentResponse", null, "USD", "orderId");
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void VerifySignatureNoCurrency() {
            new CreditCardData().VerifySignature("paymentResponse", 10m, null, "orderId");
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void VerifySignatureNoOrderId() {
            new CreditCardData().VerifySignature("paymentResponse", 10m, "USD", null);
        }

        [TestMethod]
        public void Authorize3dSecure() {
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = 2018,
                CardHolderName = "Philip Marlowe",
                ThreeDSecure = new ThreeDSecure {
                    Cavv = "AAACBllleHchZTBWIGV4AAAAAAA=",
                    Xid = "crqAeMwkEL9r4POdxpByWJ1/wYg=",
                    Eci = "5"
                }
            };

            var response = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }
    }

    public class ThreeDSecureAcsClient {
        private string _serviceUrl;
        
        public ThreeDSecureAcsClient(string url) {
            _serviceUrl = url;
        }

        public dynamic Authenticate(string payerAuthRequest, string merchantData = "") {
            HttpClient httpClient = new HttpClient() {
                Timeout = TimeSpan.FromMilliseconds(60000)
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _serviceUrl);
            HttpResponseMessage response = null;
            try {
                var kvps = new List<KeyValuePair<string, string>>();
                kvps.Add(new KeyValuePair<string, string>("PaReq", payerAuthRequest));
                kvps.Add(new KeyValuePair<string, string>("TermUrl", @"https://www.mywebsite.com/process3dSecure"));
                kvps.Add(new KeyValuePair<string, string>("MD", merchantData));

                request.Content = new FormUrlEncodedContent(kvps);
                response = httpClient.SendAsync(request).Result;
                var rawResponse = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode != HttpStatusCode.OK) {
                    throw new Exception(rawResponse);
                }

                var authResponse = GetInputValue(rawResponse, "PaRes");
                var md = GetInputValue(rawResponse, "MD");

                return new {
                    pares = authResponse,
                    md = md
                };
            }
            catch (Exception) {
                throw;
            }
            finally { }
        }

        private string GetInputValue(string raw, string inputValue) {
            var searchString = string.Format("NAME=\"{0}\" VALUE=\"", inputValue);

            var index = raw.IndexOf(searchString);
            if (index > -1) {
                index = index + searchString.Length;

                var length = raw.IndexOf("\"", index) - index;
                return raw.Substring(index, length);
            }
            return null;
        }
    }
}
