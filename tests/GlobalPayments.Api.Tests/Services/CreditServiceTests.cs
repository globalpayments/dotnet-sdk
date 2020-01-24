using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Services {
    [TestClass]
    public class CreditServiceTests {
        private CreditService service;
        private CreditCardData card;

        public CreditServiceTests() {
            service = new CreditService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w"
            });

            card = new CreditCardData {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2015,
                Cvn = "123"
            };
        }

        [TestMethod]
        public void CreditServiceAuthCapture() {
            Transaction response = service.Authorize(10m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditServiceSale() {
            Transaction response = service.Charge(11.01m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditServiceEdit() {
            Transaction response = service.Charge(12m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction editResponse = service.Edit(response.TransactionId)
                .WithAmount(14m)
                .WithGratuity(2m)
                .Execute();
            Assert.IsNotNull(editResponse);
            Assert.AreEqual("00", editResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditServiceCommercialEdit() {
            Transaction response = service.Charge(13m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .WithCommercialRequest(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var commercialData = new CommercialData(TaxType.SALESTAX) {
                TaxAmount = 1m
            };

            Transaction editResponse = service.Edit(response.TransactionId)
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(editResponse);
            Assert.AreEqual("00", editResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditServiceRefundByCard() {
            Transaction response = service.Charge(14m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
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
        public void CreditServiceRefundByTransactionId() {
            Transaction response = service.Charge(15m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction refundResponse = service.Refund(15m)
                .WithCurrency("USD")
                .WithTransactionId(response.TransactionId)
                .Execute();
            Assert.IsNotNull(refundResponse);
            Assert.AreEqual("00", refundResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditServiceReverseByCard() {
            Transaction response = service.Charge(16m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction reverseResponse = service.Reverse(16m)
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("00", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditServiceReverseByTransactionId() {
            Transaction response = service.Charge(17m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction reverseResponse = service.Reverse(17m)
                .WithTransactionId(response.TransactionId)
                .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("00", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditServiceReverseByClientId() {
            Transaction response = service.Charge(18m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .WithClientTransactionId("123456789")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction reverseResponse = service.Reverse(18m)
                .WithClientTransactionId("123456789")
                .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("00", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditServiceVerify() {
            Transaction response = service.Verify()
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditServiceVoid() {
            Transaction response = service.Charge(19m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction voidResponse = service.Void(response.TransactionId).Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }
    }
}
