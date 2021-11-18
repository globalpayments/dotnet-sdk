using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace GlobalPayments.Api.Tests.Terminals.UPA
{
    [TestClass]
    public class UpaCreditTests
    {
        IDeviceInterface _device;

        public UpaCreditTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.NUCLEUS_SATURN_1000,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "192.168.1.6",
                Port = "8081",
                Timeout = 30000,
                RequestIdProvider = new RandomIdProvider()
            });
            Assert.IsNotNull(_device);
        }

        [TestCleanup]
        public void Cleanup() {
            Thread.Sleep(2500);
        }

        [TestMethod]
        public void CreditSale() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Sale(10m)
                .WithEcrId(13)
                .WithClerkId(123)
                .WithGratuity(0m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditSaleWithTokenCreation()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Sale(10m)
                .WithEcrId(13)
                .WithClerkId(123)
                .WithGratuity(0m)
                .WithRequestMultiUseToken(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Token);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
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

            var response = _device.Sale(30m)
                .WithEcrId(13)
                .WithClerkId(123)
                .WithPaymentMethod(card)
                .WithTaxAmount(24.12m)
                .WithGratuity(0m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        // Performing a transaction by manually sending card data is currently not supported
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
                .WithEcrId(13)
                .WithPaymentMethod(card)
                .WithTaxType(TaxType.TAXEXEMPT)
                .WithGratuity(0m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditVoid()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Sale(10m)
                .WithEcrId(13)
                .WithClerkId(123)
                .WithGratuity(0m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var voidResponse = _device.Void()
                .WithTerminalRefNumber(response.TerminalRefNumber)
                .WithTransactionId(response.TransactionId)
                .Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditVoidNoTransactionId()
        {
            _device.Void()
                .WithEcrId(13)
                .WithTerminalRefNumber("12")
                .Execute();
        }

        [TestMethod]
        public void CreditRefundByCard()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var returnResponse = _device.Refund(14m)
                .WithEcrId(13)
                .Execute();
            Assert.IsNotNull(returnResponse);
            Assert.AreEqual("00", returnResponse.ResponseCode);
        }

        [TestMethod, Ignore]
        // Manually sending card and token data is currently not supported
        public void CreditRefundByToken()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            
            var card = new CreditCardData
            {
                Number = "4005554444444460",
                ExpMonth = 12,
                ExpYear = 17,
                Cvn = "123"
            };

            var returnResponse = _device.Refund(15m)
                .WithPaymentMethod(card)
                .WithEcrId(13)
                .WithClerkId(13)
                .WithRequestMultiUseToken(true)
                .Execute();
            Assert.IsNotNull(returnResponse);
            Assert.AreEqual("00", returnResponse.ResponseCode);
        }

        [TestMethod, Ignore]
        public void CreditTipAdjust()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.TipAdjust()
                .WithEcrId(13)
                .WithClerkId(123)
                .WithGratuity(12.12m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EndOfDay()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            _device.EcrId = "13";
            var response = _device.EndOfDay();
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void Verify()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Verify()
                .WithEcrId(13)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void VerifyWithTokenCreation()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Verify()
                .WithEcrId(13)
                .WithRequestMultiUseToken(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Token);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        // Manually sending card data is currently not supported
        public void VerifyManual()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var card = new CreditCardData
            {
                Number = "4005554444444460",
                ExpMonth = 12,
                ExpYear = 17,
                Cvn = "123"
            };

            var response = _device.Verify()
                .WithEcrId(13)
                .WithPaymentMethod(card)
                .WithClerkId(1234)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
    }
}
