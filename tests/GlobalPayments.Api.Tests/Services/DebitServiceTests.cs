using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Services {
    [TestClass]
    public class DebitServiceTests {
        DebitService service;
        DebitTrackData card;

        public DebitServiceTests() {
            service = new DebitService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MaePAQBr-1QAqjfckFC8FTbRTT120bVQUlfVOjgCBw"
            });

            card = new DebitTrackData {
                Value = "&lt;E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                PinBlock = "32539F50C245A6A93D123412324000AA",
                EncryptionData = EncryptionData.Version1()
            };
        }

        [TestMethod]
        public void DebitServiceSale() {
            Transaction response = service.Charge(14m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DebitServiceRefundByCard() {
            Transaction response = service.Charge(14m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction refundResponse = service.Refund(14m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(refundResponse);
            Assert.AreEqual("00", refundResponse.ResponseCode);
        }

        [TestMethod]
        public void DebitServiceReverseByCard() {
            Transaction response = service.Charge(16m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction reverseResponse = service.Reverse(16m)
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("00", reverseResponse.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(UnsupportedTransactionException))]
        public void DebitServiceReverseByTransactionId() {
            Transaction response = service.Charge(17m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // TODO: Figure out how to handle multiple payment methods... thanks plano...
            Transaction reverseResponse = service.Reverse(17m)
                .WithCurrency("USD")
                .WithTransactionId(response.TransactionId)
                .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("00", reverseResponse.ResponseCode);
        }
    }
}
