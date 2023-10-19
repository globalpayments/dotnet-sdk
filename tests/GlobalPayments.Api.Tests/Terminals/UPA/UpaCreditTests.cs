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

        private int validYear = DateTime.Now.Year + 1;

        public UpaCreditTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.UPA_DEVICE,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "192.168.1.130",
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

            var autoSub = new AutoSubstantiation {
                PrescriptionSubTotal = 10m,
                ClinicSubTotal = 1m,
                DentalSubTotal = 2m,
                VisionSubTotal = 2m
            };

            var response = _device.Sale(10m)
                .WithEcrId(13)
                .WithClerkId(123)
                .WithGratuity(0m)
                .WithRequestMultiUseToken(true)
                .WithCardOnFileIndicator(StoredCredentialInitiator.CardHolder)
                .WithCardBrandTransId("transId")
                .WithAutoSubstantiation(autoSub)
                .WithInvoiceNumber(new Random().Next(1000000, 9999999).ToString())
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void PreAuth() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var autoSub = new AutoSubstantiation {
                PrescriptionSubTotal = 10m,
                ClinicSubTotal = 1m,
                DentalSubTotal = 2m,
                VisionSubTotal = 2m
            };

            var response = _device.Authorize()
                .WithEcrId(13)
                .WithClerkId(123)
                // .WithRequestMultiUseToken(true)
                // .WithToken("2323")
                // .WithCardOnFileIndicator(StoredCredentialInitiator.CardHolder)
                .WithCardBrandTransId("transId")
                .WithAmount(10m)
                .WithTerminalRefNumber("1234")
                .WithAutoSubstantiation(autoSub)
                .WithInvoiceNumber("12345")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void PreAuthWithIncrementalAndCompletion() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };                

            var preAuthResponse = _device.Authorize()
               .WithEcrId(13)
               .WithClerkId(123)
               .WithCardBrandTransId("transId")
               .WithAmount(100m)               
               .Execute();
            Assert.IsNotNull(preAuthResponse);
            Assert.AreEqual("00", preAuthResponse.ResponseCode);

            var lodging = new Lodging
            {
                FolioNumber = 1,
                StayDuration = 3,
                CheckInDate = DateTime.Now,
                CheckOutDate = DateTime.Now.AddDays(3),
                DailyRate = 11m,
                PreferredCustomer = 1,
                ExtraChargeTotal = 10m,
                ExtraChargeTypes = new ExtraChargeTypes()
                { 
                    HasRestaurantCharge = true,
                    HasGiftShopCharge = true,
                    HasMiniBarCharge = true,
                    HasTelephoneCharge = true,
                    HasLaundryCharge = true,
                    HasOtherCharge = true,
                }
            };

            var incrementalPreAuthResponse = _device.Authorize()
                .WithEcrId(13)
                .WithPreAuthAmount(preAuthResponse.TransactionAmount)
                .WithClerkId(123)                
                .WithCardBrandTransId("transId")
                .WithAmount(50m)
                .WithTerminalRefNumber(preAuthResponse.ReferenceNumber)
                .WithLodging(lodging)
                .Execute();
            Assert.IsNotNull(incrementalPreAuthResponse);
            Assert.AreEqual("00", incrementalPreAuthResponse.ResponseCode);     

            var completionResponse = _device.Capture(145m)
                .WithEcrId(13)                
                .WithTransactionId(preAuthResponse.TransactionId)
                .WithTerminalRefNumber(preAuthResponse.ReferenceNumber)
                .Execute();
           
            Assert.IsNotNull(completionResponse);
            Assert.AreEqual("00", completionResponse.ResponseCode);
        }

        [TestMethod]
        public void AuthCompletion()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Capture()
                .WithEcrId(13)
                .WithAmount(12m)
                .WithTerminalRefNumber("1234567")
                .WithTransactionId("12")// response.TransactionId)
                .WithTaxAmount(5m)
                .WithGratuity(1m)
                .WithTaxType(TaxType.TAXEXEMPT)
                .WithInvoiceNumber("12")
                .WithProcessCPC(1)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

        }

        [TestMethod]
        public void AuthCompletionUseTransId()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var authResponse = _device.Authorize(10.00m)
                .WithEcrId(13)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(authResponse);
            Assert.AreEqual("00", authResponse.ResponseCode);

            Thread.Sleep(1500);

            var captureResponse = _device.Capture(10.00m)
                .WithTransactionId(authResponse.TransactionId)
                .Execute();
            Assert.IsNotNull(authResponse);
            Assert.AreEqual("00", authResponse.ResponseCode);
        }

        [TestMethod]
        public void Tokenize()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var autoSub = new AutoSubstantiation {
                PrescriptionSubTotal = 10m,
                ClinicSubTotal = 1m,
                DentalSubTotal = 2m,
                VisionSubTotal = 2m
            };

            var response = _device.Tokenize()
                .WithEcrId(13)
                .WithClerkId(123)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DeletePreAuth()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.DeletePreAuth()
                .WithEcrId(13)
                .WithTerminalRefNumber("1234567")
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
                .WithInvoiceNumber(new Random().Next(10000, 99999).ToString())
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

        [TestMethod]
        public void CreditTipAdjust() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var saleResponse = _device.Sale(15.12m)
                .WithEcrId(13)
                .WithClerkId(123)
                .WithGratuity(0.00m)
                .Execute();

            Assert.IsNotNull(saleResponse);

            Thread.Sleep(2500);

            var tipAdjustResponse = _device.TipAdjust(3.00m)
                .WithTerminalRefNumber(saleResponse.TerminalRefNumber)
                .WithEcrId(13)                
                .Execute();
            Assert.IsNotNull(tipAdjustResponse);
            Assert.AreEqual(18.12m, tipAdjustResponse.TransactionAmount);
            Assert.AreEqual("00", tipAdjustResponse.ResponseCode);
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
                .WithCardOnFileIndicator(StoredCredentialInitiator.CardHolder)
                .WithCardBrandTransId("transId")
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

        [TestMethod]
        public void SaleWithMktData()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var card = new CreditCardData
            {
                Number = "4005554444444460",
                ExpMonth = 12,
                ExpYear = validYear,
                Cvn = "123"
            };

            var response = _device.Sale(1.23m)
                .WithGratuity(0.00m)
                .WithEcrId(13)
                .WithPaymentMethod(card)
                .WithClerkId(1234)
                .WithInvoiceNumber("12345")
                .WithShippingDate(new DateTime(2022, 7, 26))
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(MessageException),
            "Connection not established within the specified timeout.")]
        public void testConnectionTimeout()
        {
            IDeviceInterface timeoutDevice = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.NUCLEUS_SATURN_1000,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "192.168.0.105",
                Port = "8081",
                Timeout = 1, // overly-short timeout value to trigger exception in UpaTcpInterface
                RequestIdProvider = new RandomIdProvider()
            });
            Assert.IsNotNull(timeoutDevice);

            var response = _device.Sale(123m)
                .Execute();
        }
        
    }
}
