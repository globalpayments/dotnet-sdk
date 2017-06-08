using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.Realex.Hpp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Realex {
    [TestClass]
    public class RealexHppTests {
        HostedService _service;
        RealexHppClient _client;

        public RealexHppTests() {
            _client = new RealexHppClient("https://pay.sandbox.realexpayments.com/pay", "secret");
            _service = new HostedService(new ServicesConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "hpp",
                SharedSecret = "secret",
                ServiceUrl = "https://api.sandbox.realexpayments.com/epage-remote.cgi",
                HostedPaymentConfig = new HostedPaymentConfig {
                    Language = "GB",
                    ResponseUrl = "http://requestb.in/10q2bjb1"
                }
            });
        }

        [TestMethod]
        public void CreditAuth() {
            var json = _service.Authorize(1m)
                .WithCurrency("EUR")
                .WithCustomerId("123456")
                .WithAddress(new Address {
                    PostalCode = "123|56",
                    Country = "IRELAND"
                }).Serialize();
            Assert.IsNotNull(json);

            var response = _client.SendRequest(json);
            var parsedResponse = _service.ParseResponse(response);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", parsedResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditSale() {
            var json = _service.Charge(1m)
                .WithCurrency("EUR")
                .WithCustomerId("123456")
                .WithAddress(new Address {
                    PostalCode = "123|56",
                    Country = "IRELAND"
                }).Serialize();
            Assert.IsNotNull(json);

            var response = _client.SendRequest(json);
            var parsedResponse = _service.ParseResponse(response);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", parsedResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditVerify() {
            var json = _service.Verify()
                .WithCurrency("EUR")
                .WithCustomerId("123456")
                .WithAddress(new Address {
                    PostalCode = "123|56",
                    Country = "IRELAND"
                }).Serialize();
            Assert.IsNotNull(json);

            var response = _client.SendRequest(json);
            var parsedResponse = _service.ParseResponse(response);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", parsedResponse.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void AuthNoAmount() {
            _service.Authorize(null).WithCurrency("USD").Serialize();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void AuthNoCurrency() {
            _service.Authorize(10m).Serialize();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void SaleNoAmount() {
            _service.Charge(null).WithCurrency("USD").Serialize();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void SaleNoCurrency() {
            _service.Charge(10m).Serialize();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void VerifyNoCurrency() {
            _service.Verify().Serialize();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void VerifyWithAmount() {
            _service.Verify().WithAmount(10m).Serialize();
        }
    }
}
