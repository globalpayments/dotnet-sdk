using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.Pax {
    [TestClass]
    public class PaxCreditTests {
        IDeviceInterface _device;

        public PaxCreditTests() {
            _device = DeviceService.Create(new ServicesConfig {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A",
                ServiceUrl = "https://cert.api2.heartlandportico.com",
                DeviceConnectionConfig = new ConnectionConfig {
                    DeviceType = DeviceType.PAX_S300,
                    ConnectionMode = ConnectionModes.TCP_IP,
                    IpAddress = "10.12.220.172",
                    Port = "10009"
                }
            });
            Assert.IsNotNull(_device);
        }

        [TestMethod]
        public void CreditSale() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.CreditSale(1, 10m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditSaleManual() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var card = new CreditCardData {
                Number = "4005554444444460",
                ExpMonth = 12,
                ExpYear = 17,
                Cvn = "123"
            };

            var address = new Address {
                StreetAddress1 = "1 Heartland Way",
                PostalCode = "95124"
            };

            var response = _device.CreditSale(1, 11m)
                .WithAllowDuplicates(true)
                .WithPaymentMethod(card)
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditSaleWithSignatureCapture() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var card = new CreditCardData {
                Number = "4005554444444460",
                ExpMonth = 12,
                ExpYear = 17,
                Cvn = "123"
            };

            var address = new Address {
                StreetAddress1 = "1 Heartland Way",
                PostalCode = "95124"
            };

            var response = _device.CreditSale(1, 12m)
                .WithAllowDuplicates(true)
                .WithPaymentMethod(card)
                .WithAddress(address)
                .WithSignatureCapture(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditSaleNoAmount() {
            _device.CreditSale(1).Execute();
        }

        [TestMethod]
        public void CreditAuth_Capture() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.CreditAuth(1, 12m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var captureResponse = _device.CreditCapture(2, 12m)
                .WithTransactionId(response.TransactionId)
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditAuthNoAmount() {
            _device.CreditAuth(1).Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CaptureNoTransactionId() {
            _device.CreditCapture(1).Execute();
        }

        [TestMethod]
        public void CreditRefundByTransactionId() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var card = new CreditCardData {
                Number = "4005554444444460",
                ExpMonth = 12,
                ExpYear = 17,
                Cvn = "123"
            };

            var address = new Address {
                StreetAddress1 = "1 Heartland Way",
                PostalCode = "95124"
            };

            var saleResponse = _device.CreditSale(1, 16m)
                .WithPaymentMethod(card)
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .WithRequestMultiUseToken(true)
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);

            var returnResponse = _device.CreditRefund(2, 16m)
                .WithTransactionId(saleResponse.TransactionId)
                .WithAuthCode(saleResponse.AuthorizationCode)
                .Execute();
            Assert.IsNotNull(returnResponse);
            Assert.AreEqual("00", returnResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditRefundByCard() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var card = new CreditCardData {
                Number = "4005554444444460",
                ExpMonth = 12,
                ExpYear = 17,
                Cvn = "123"
            };

            var returnResponse = _device.CreditRefund(2, 14m)
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(returnResponse);
            Assert.AreEqual("00", returnResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditRefundByToken() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var card = new CreditCardData {
                Number = "4005554444444460",
                ExpMonth = 12,
                ExpYear = 17,
                Cvn = "123"
            };

            var address = new Address {
                StreetAddress1 = "1 Heartland Way",
                PostalCode = "95124"
            };

            var response = _device.CreditVerify(11)
                .WithPaymentMethod(card)
                .WithAddress(address)
                .WithRequestMultiUseToken(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.Token);

            var returnResponse = _device.CreditRefund(2, 15m)
                .WithToken(response.Token)
                .Execute();
            Assert.IsNotNull(returnResponse);
            Assert.AreEqual("00", returnResponse.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditRefundNoAmount() {
            _device.CreditRefund(1).Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditRefundByTransactionIdNoAuthCode() {
            _device.CreditRefund(2, 13m)
                .WithTransactionId("1234567")
                .Execute();
        }

        [TestMethod]
        public void CreditVerify() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.CreditVerify(1).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditVerifyManual() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var card = new CreditCardData {
                Number = "4005554444444460",
                ExpMonth = 12,
                ExpYear = 17,
                Cvn = "123"
            };

            var address = new Address {
                StreetAddress1 = "1 Heartland Way",
                PostalCode = "95124"
            };

            var response = _device.CreditVerify(1)
                .WithPaymentMethod(card)
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Tokenize() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.CreditVerify(1)
                .WithRequestMultiUseToken(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.Token);
        }

        [TestMethod]
        public void CreditVoid() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var card = new CreditCardData {
                Number = "4005554444444460",
                ExpMonth = 12,
                ExpYear = 17,
                Cvn = "123"
            };

            var address = new Address {
                StreetAddress1 = "1 Heartland Way",
                PostalCode = "95124"
            };

            var saleResponse = _device.CreditSale(1, 16m)
                .WithPaymentMethod(card)
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .WithRequestMultiUseToken(true)
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);

            var voidResponse = _device.CreditVoid(1).WithTransactionId(saleResponse.TransactionId).Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditVoidNoTransactionId() {
            _device.CreditVoid(1).Execute();
        }
    }
}
