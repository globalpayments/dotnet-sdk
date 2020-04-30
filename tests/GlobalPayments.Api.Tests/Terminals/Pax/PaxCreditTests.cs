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
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.PAX_S300,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "10.12.220.172",
                //IpAddress = "192.168.0.31",
                Port = "10009",
                Timeout = 30000,
                RequestIdProvider = new RandomIdProvider()
            });
            Assert.IsNotNull(_device);
        }

        [TestMethod]
        public void CreditSale() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Sale(10m)
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
                ExpMonth = 1,
                ExpYear = 20,
                Cvn = "123"
            };

            var address = new Address {
                StreetAddress1 = "1 Heartland Way",
                PostalCode = "95124"
            };

            var response = _device.Sale(11m)
                .WithAllowDuplicates(true)
                .WithPaymentMethod(card)
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditSaleManualFSA() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var card = new CreditCardData {
                Number = "4393421234561236",
                ExpMonth = 12,
                ExpYear = 29,
                Cvn = "123"
            };

            var address = new Address {
                StreetAddress1 = "1 Heartland Way",
                PostalCode = "60523"
            };
            var autoSub = new AutoSubstantiation {
                ClinicSubTotal = 3,
                DentalSubTotal = 3,
                PrescriptionSubTotal = 3,
                VisionSubTotal = 2
            };

            var response = _device.Sale(11m)
                .WithAllowDuplicates(true)
                .WithPaymentMethod(card)
                .WithAddress(address)
                .WithAutoSubstantiation(autoSub)
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
                ExpYear = 25,
                Cvn = "123"
            };

            var address = new Address {
                StreetAddress1 = "1 Heartland Way",
                PostalCode = "95124"
            };

            var response = _device.Sale(12m)
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
            _device.Sale(1).Execute();
        }

        [TestMethod]
        public void CreditAuth_Capture() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Authorize(12m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var captureResponse = _device.Capture(12m)
                .WithTransactionId(response.TransactionId)
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditAuth_CaptureManual() {
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

            var response = _device.Authorize(12m)
                .WithPaymentMethod(card)
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var captureResponse = _device.Capture(12m)
                .WithTransactionId(response.TransactionId)
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditAuthNoAmount() {
            _device.Authorize(1m).Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CaptureNoTransactionId() {
            _device.Capture(1m).Execute();
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

            var saleResponse = _device.Sale(16m)
                .WithPaymentMethod(card)
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .WithRequestMultiUseToken(true)
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);

            var returnResponse = _device.Refund(16m)
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

            var returnResponse = _device.Refund(14m)
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

            var response = _device.Verify()
                .WithPaymentMethod(card)
                .WithAddress(address)
                .WithRequestMultiUseToken(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.Token);

            var returnResponse = _device.Refund(15m)
                .WithToken(response.Token)
                .Execute();
            Assert.IsNotNull(returnResponse);
            Assert.AreEqual("00", returnResponse.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditRefundNoAmount() {
            _device.Refund(1).Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditRefundByTransactionIdNoAuthCode() {
            _device.Refund(13m)
                .WithTransactionId("1234567")
                .Execute();
        }

        [TestMethod]
        public void CreditVerify() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Verify().Execute();
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

            var response = _device.Verify()
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

            var response = _device.Verify()
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

            var saleResponse = _device.Sale(16m)
                .WithPaymentMethod(card)
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .WithRequestMultiUseToken(true)
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);

            var voidResponse = _device.Void().WithTransactionId(saleResponse.TransactionId).Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditVoidNoTransactionId() {
            _device.Void().Execute();
        }

        [TestMethod]
        public void CreditDuplicateTransaction() {
            var card = new CreditCardData {
                Number = "4005554444444460",
                ExpMonth = 12,
                ExpYear = 20,
                Cvn = "123"
            };

            var address = new Address {
                StreetAddress1 = "1 Heartland Way",
                PostalCode = "95124"
            };

            var response = _device.Sale(11m)
                .WithAllowDuplicates(false)
                .WithPaymentMethod(card)
                .WithAddress(address)
                .WithReferenceNumber(12345)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var duplicate = _device.Sale(11m)
                .WithAllowDuplicates(false)
                .WithPaymentMethod(card)
                .WithAddress(address)
                .WithReferenceNumber(12345)
                .Execute();
            Assert.IsNotNull(duplicate);
            Assert.AreEqual("00", duplicate.ResponseCode);
            Assert.AreEqual(response.AuthorizationCode, duplicate.AuthorizationCode);
        }
    }
}
