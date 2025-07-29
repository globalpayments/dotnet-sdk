using System;
using System.Collections.Generic;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.UPA {
    [TestClass]
    public class UpaCreditTests {
        IDeviceInterface _device;

        private int validYear = DateTime.Now.Year + 1;
        private CreditCardData card;

        public UpaCreditTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.UPA_DEVICE,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "192.168.51.94",
                Port = "8081",
                Timeout = 60000,
                RequestIdProvider = new RandomIdProvider(),
                LogManagementProvider = new RequestConsoleLogger()
            });
            Assert.IsNotNull(_device);

            card = new CreditCardData();
            card.Number = "4111111111111111";
            card.ExpMonth = 12;
            card.ExpYear = validYear;
            card.Cvn = "123";
            card.CardHolderName = "Joe Smith";
            card.EntryMethod = ManualEntryMethod.Mail;
        }

        [TestCleanup]
        public void Cleanup() {
            Thread.Sleep(2500);
        }

        #region Unit Test case for Sale Scenario
        [TestMethod]
        public void CreditSale() {
            var autoSub = new AutoSubstantiation {
                PrescriptionSubTotal = 10m,
                ClinicSubTotal = 1m,
                DentalSubTotal = 2m,
                VisionSubTotal = 2m
            };

            decimal amount = 10.25m;  //10.25m; //for decline

            var response = _device.Sale(amount)
            .WithEcrId("13")
            .WithClerkId(123)
            .WithGratuity(0m)
            .WithCardBrandTransId("transId")
            .WithAutoSubstantiation(autoSub)
            .WithInvoiceNumber(new Random().Next(1000000, 9999999).ToString())
            .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("Failed", response.DeviceResponseText);
        }

        [TestMethod]
        public void CreditSale_OnlyMandatory() {
            var response = _device.Sale(10m)
                .WithEcrId(13)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void CreditSale_ZeroTip() {
            var response = _device.Sale(10m)
                .WithEcrId(13)
                .WithGratuity(0m)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void CreditSale_WithTip() {
            var response = _device.Sale(10m)
                .WithEcrId("25698")
                .WithGratuity(1m)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual(11.00m, response.TransactionAmount);
            Assert.AreEqual(1.00m, response.TipAmount);
        }
        #endregion

        #region Unit Test case for Auth Scenario
        [TestMethod]
        public void PreAuth() {
            var autoSub = new AutoSubstantiation {
                PrescriptionSubTotal = 10m,
                ClinicSubTotal = 1m,
                DentalSubTotal = 2m,
                VisionSubTotal = 2m
            };

            var response = _device.Authorize()
                .WithEcrId(13)
                .WithClerkId(123)
                .WithCardBrandTransId("transId")
                .WithAmount(10m)
                .WithAutoSubstantiation(autoSub)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.DeviceResponseText);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void PreAuthWithIncrementalAndCompletion() {
            var preAuthResponse = _device.Authorize()
               .WithEcrId(13)
               .WithClerkId(123)
               .WithCardBrandTransId("transId")
               .WithAmount(100m)
               .Execute();
            Assert.IsNotNull(preAuthResponse);
            Assert.AreEqual("00", preAuthResponse.DeviceResponseCode);

            Thread.Sleep(3500);

            var lodging = new Lodging {
                FolioNumber = 1,
                StayDuration = 3,
                CheckInDate = DateTime.Now,
                CheckOutDate = DateTime.Now.AddDays(3),
                DailyRate = 11m,
                PreferredCustomer = 1,
                //ExtraChargeTotal = 10m,
                ExtraChargeTypes = new List<ExtraChargeType>() {
                    {ExtraChargeType.Restaurant},
                    {ExtraChargeType.GiftShop},
                    {ExtraChargeType.MiniBar},
                    {ExtraChargeType.Telephone },
                    {ExtraChargeType.Laundry},
                    {ExtraChargeType.Other},
                },
            };

            var incrementalPreAuthResponse = _device.Authorize()
                .WithEcrId(13)
                .WithPreAuthAmount(preAuthResponse.TransactionAmount)
                .WithClerkId(123)
                .WithCardBrandTransId("transId")
                .WithAmount(50m)
                .WithTransactionId(preAuthResponse.TransactionId)
                .WithLodging(lodging)
                .Execute();
            Assert.IsNotNull(incrementalPreAuthResponse);
            Assert.AreEqual("00", incrementalPreAuthResponse.DeviceResponseCode);

            Thread.Sleep(3500);

            var completionResponse = _device.Capture(145m)
                .WithEcrId("13")
                .WithTransactionId(preAuthResponse.TransactionId)
                .Execute();

            Assert.IsNotNull(completionResponse);
            Assert.AreEqual("00", completionResponse.DeviceResponseCode);
        }

        [TestMethod]
        public void AuthCompletion() {
            var response = _device.Capture()
                    .WithEcrId("1")
                    .WithAmount(12m)
                    .WithTransactionId("200079053977")// response.TransactionId)
                    .WithTaxAmount(5m)
                    .WithGratuity(1m)
                    .WithTaxType(TaxType.TAXEXEMPT)
                    .WithProcessCPC(1)
                    .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.DeviceResponseText);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void AuthCompletionUseTransId() {
            var authResponse = _device.Authorize(10.00m)
                .WithEcrId(13)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(authResponse);
            Assert.AreEqual("00", authResponse.DeviceResponseCode);

            Thread.Sleep(1500);
            var captureResponse = _device.Capture(10.00m)
                .WithEcrId("13")
                .WithTransactionId(authResponse.TransactionId)
                .Execute();
            Assert.IsNotNull(authResponse);
            Assert.AreEqual("00", authResponse.DeviceResponseCode);
        }

        [TestMethod]
        public void DeletePreAuth() {
            var response = _device.DeletePreAuth()
                .WithEcrId("13")
                .WithTransactionId("200071138640")
                .WithAmount(1m)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.DeviceResponseText);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }
        #endregion

        [TestMethod]
        public void Tokenize() {
            var response = _device.Tokenize()
                .WithEcrId(13)
                .WithClerkId(123)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void MailOrder() {
            card.EntryMethod = ManualEntryMethod.Mail;

            var transAmount = 10;
            var response = _device.Sale(transAmount)
                .WithEcrId(12)
                .WithTaxAmount(2.18m)
                .WithTaxType(TaxType.TAXEXEMPT)
                .WithProcessCPC(true)
                .WithRequestMultiUseToken(true)
                .WithInvoiceNumber("123A10")
                .WithPaymentMethod(card)
                .WithAllowDuplicates(true)
                .WithClerkId(1234)
                .WithCardOnFileIndicator(StoredCredentialInitiator.CardHolder)
                .WithCardBrandTransId("transId")
                .WithShippingDate(DateTime.Now)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.TransactionId);
            Assert.AreEqual(transAmount, response.TransactionAmount);

        }

        [TestMethod]
        public void CreditSaleWithTokenCreation() {
            var response = _device.Sale(10m)
                .WithEcrId(13)
                .WithClerkId(123)
                .WithGratuity(0m)
                .WithRequestMultiUseToken(true)
                .WithCardOnFileIndicator(StoredCredentialInitiator.CardHolder)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Token);
            Assert.AreEqual("00", response.DeviceResponseCode);

            CreditCardData card = new CreditCardData {
                Token = response.Token
            };

            Thread.Sleep(5000);

            var tokenResponse = _device.Sale(11m)
                .WithEcrId(13)
                .WithClerkId(123)
                .WithPaymentMethod(card)
                .WithCardOnFileIndicator(StoredCredentialInitiator.Merchant)
                .WithCardBrandTransId("transId")
                .Execute();
            Assert.IsNotNull(tokenResponse);
            Assert.AreEqual("00", tokenResponse.DeviceResponseCode);
        }

        [TestMethod, Ignore]
        public void CreditSaleManual() {
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
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod, Ignore]
        // Performing a transaction by manually sending card data is currently not supported
        public void CreditSaleManualFSA() {
            var card = new CreditCardData {
                Number = "4393421234561236",
                ExpMonth = 12,
                ExpYear = 29,
                Cvn = "123"
            };

            var response = _device.Sale(11m)
                .WithEcrId(13)
                .WithPaymentMethod(card)
                .WithTaxType(TaxType.TAXEXEMPT)
                .WithGratuity(0m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        #region Unit test case for Void Transaction
        [TestMethod]
        // Test that voiding a credit sale by terminal reference number returns a successful response
        public void CreditSale_WhenVoidedByTerminalRefNumber_ShouldReturnSuccess() {
            // Perform a sale transaction
            var response = _device.Sale(10m)
                .WithEcrId(13)
                .WithClerkId(123)
                .WithGratuity(0m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);

            // Wait for the transaction to settle before voiding
            Thread.Sleep(5000);

            // Void the transaction using the terminal reference number
            var voidResponse = _device.Void()
                .WithTerminalRefNumber(response.TerminalRefNumber)
                .WithEcrId("13")
                .Execute();

            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.DeviceResponseCode);
        }

        [TestMethod]
        // Test that voiding a credit sale by transaction ID returns a successful response
        public void CreditSale_WhenVoidedByTransactionId_ShouldReturnSuccess() {
            // Perform a sale transaction
            var response = _device.Sale(10m)
                .WithEcrId(13)
                .WithClerkId(123)
                .WithGratuity(0m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);

            // Wait for the transaction to settle before voiding
            Thread.Sleep(5000);

            // Void the transaction using the transaction ID
            var voidResponse = _device.Void()
                .WithTransactionId(response.TransactionId)
                .WithEcrId("13")
                .Execute();

            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.DeviceResponseCode);
        }

        [TestMethod]
        // Test that attempting to void a transaction without providing a terminal reference number
        // or transaction ID throws the expected GatewayException
        public void CreditSale_VoidWithoutTransactionIdOrRefNumber_ShouldThrowGatewayException() {
            var ex = Assert.ThrowsException<GatewayException>(() => {
                var voidResponse = _device.Void()
                    .WithEcrId("13")
                    .Execute();
            });
            Assert.AreEqual("Unexpected Device Response : VOID003  - NO TRANNO OR REFERENCENUMBER SUPPLIED", ex.Message);
        }
        #endregion

        #region Unit test case for UpdateTax
        [TestMethod]
        // Test that updating tax info on a non-commercial card throws the expected GatewayException
        public void UpdateTaxInfo_ShouldThrowException_WhenCardIsNotCommercial() {
            // Perform a sale transaction with a non-commercial card
            var responseSale = _device.Sale(1m)
                .WithGratuity(0m)
                .WithEcrId(13)
                .Execute();

            Assert.IsNotNull(responseSale);
            Assert.AreEqual("Success", responseSale.Status);
            Assert.AreEqual("00", responseSale.DeviceResponseCode);

            // Wait for the transaction to settle before updating tax info
            Thread.Sleep(5000);

            // Attempt to update tax info and verify that the expected exception is thrown
            var ex = Assert.ThrowsException<GatewayException>(() => {
                var response = _device.UpdateTaxInfo(responseSale.TransactionAmount)
                    .WithTerminalRefNumber(responseSale.TerminalRefNumber)
                    .WithTransactionId(responseSale.TransactionId)
                    .WithTaxType(TaxType.TAXEXEMPT)
                    .WithEcrId("1")
                    .Execute();
            });
            Assert.AreEqual("Unexpected Device Response : CPC001 - NOT A COMMERCIAL CARD. CANNOT UPDATE TAX.", ex.Message);
        }

        [TestMethod]
        public void UpdateTaxInfo() {
            var responseSale = _device.Sale(10m)
                .WithGratuity(0m)
                .WithEcrId(13)
                .Execute();

            Assert.IsNotNull(responseSale);
            Assert.AreEqual("Success", responseSale.Status);
            Assert.AreEqual("00", responseSale.DeviceResponseCode);

            Thread.Sleep(5000);

            var response = _device.UpdateTaxInfo(responseSale.TransactionAmount)
                .WithTerminalRefNumber(responseSale.TerminalRefNumber)
                .WithTransactionId(responseSale.TransactionId)
                .WithTaxType(TaxType.TAXEXEMPT)
                .WithEcrId("1")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }
        #endregion

        [TestMethod]
        public void UpdateLodgingDetails() {
            var responseSale = _device.Sale(10m)
                .WithGratuity(0m)
                .WithEcrId(13)
                .Execute();
            Assert.IsNotNull(responseSale);
            Assert.AreEqual("Success", responseSale.Status);
            Assert.AreEqual("00", responseSale.DeviceResponseCode);

            Thread.Sleep(5000);

            var lodging = new Lodging();
            lodging.FolioNumber = 1;
            lodging.ExtraChargeTypes = new List<ExtraChargeType>() {
                    {ExtraChargeType.Telephone},
                    {ExtraChargeType.Other},
                };

            var response = _device.UpdateLodginDetail(responseSale.TransactionAmount)
                .WithTransactionId(responseSale.TransactionId)
                .WithLodgingData(lodging)
                .WithEcrId("1")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void ForceSale() {
            var trnAmount = 10;
            card.EntryMethod = ManualEntryMethod.Phone;

            var response = _device.Sale(trnAmount)
                .WithEcrId(12)
                .WithPaymentMethod(card)
                .WithClerkId(123)
                .WithTaxAmount(2.18m)
                .WithTaxType(TaxType.TAXEXEMPT)
                .WithGratuity(12.56m)
                .WithInvoiceNumber("123456789012345")
                .WithAllowDuplicates(true)
                .WithConfirmationAmount(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("FORCESALE", response.TransactionType);
        }

        [TestMethod]
        public void CreditSaleWithReqResLog() {
            var response = _device.Sale(10m)
                .WithEcrId(13)
                .WithClerkId(123)
                .WithGratuity(0m)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        #region Unit test case for Refund Scenario
        [TestMethod]
        public void CreditRefundByCard() {
            var returnResponse = _device.Refund(14m)
                .WithInvoiceNumber(new Random().Next(10000, 99999).ToString())
                .WithEcrId(13)
                .Execute();
            Assert.IsNotNull(returnResponse);
            Assert.AreEqual("00", returnResponse.DeviceResponseCode);
        }

        [TestMethod]
        // Test that refunding a sale by transaction ID returns a successful response
        public void CreditRefund_ByTransactionId_ShouldReturnSuccess() {
            // Perform a sale transaction
            var saleResponse = _device.Sale(2.00m)
                .WithEcrId(13)
                .WithClerkId(123)
                .WithGratuity(0m)
                .Execute();
            Assert.IsNotNull(saleResponse);

            Thread.Sleep(2500);

            // Refund the sale using the transaction ID
            var returnResponse = _device.Refund(2.00m)
                .WithTransactionId(saleResponse.TransactionId)
                .WithEcrId(13)
                .WithAllowDuplicates(false)
                .WithAuthCode("")
                .Execute();
            Assert.IsNotNull(returnResponse);
            Assert.AreEqual("00", returnResponse.DeviceResponseCode);
        }

        [TestMethod, Ignore]
        // Manually sending card and token data is currently not supported
        public void CreditRefundByToken() {
            var card = new CreditCardData {
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
            Assert.AreEqual("00", returnResponse.DeviceResponseCode);
        }

        [TestMethod]
        // Test that refunding with a zero amount throws the expected GatewayException
        public void CreditRefund_WithZeroAmount_ShouldThrowException() {
            var ex = Assert.ThrowsException<GatewayException>(() => {
                _device.Refund(0)
                .WithEcrId(13)
                .Execute();
            });
            Assert.AreEqual("Unexpected Device Response : SALE004 - TRANSACTION CANCELLED DUE TO INVALID BASE AMOUNT", ex.Message);
        }
        #endregion

        #region Unit test case for tipAdjust Scenario
        [TestMethod]
        // Test that adjusting the tip by terminal reference number updates the transaction amount and returns a successful response
        public void CreditTipAdjust_ByTerminalRefNumber_ShouldAdjustTipSuccessfully() {
            // Perform a sale transaction with zero tip
            var saleResponse = _device.Sale(10.00m)
                .WithEcrId(13)
                .WithClerkId(123)
                .WithGratuity(0.00m)
                .Execute();
            Assert.IsNotNull(saleResponse);

            Thread.Sleep(2500);

            // Adjust the tip using the terminal reference number
            var tipAdjustResponse = _device.TipAdjust(3.00m)
                .WithTerminalRefNumber(saleResponse.TerminalRefNumber)
                .WithEcrId("13")
                .Execute();
            Assert.IsNotNull(tipAdjustResponse);
            Assert.AreEqual(13.00m, tipAdjustResponse.TransactionAmount);
            Assert.AreEqual("00", tipAdjustResponse.DeviceResponseCode);
        }

        [TestMethod]
        // Test that adjusting the tip by transaction ID updates the transaction amount and returns a successful response
        public void CreditTipAdjust_ByTransactionId_ShouldAdjustTipSuccessfully() {
            // Perform a sale transaction with zero tip
            var saleResponse = _device.Sale(10.00m)
                .WithEcrId(13)
                .WithClerkId(123)
                .WithGratuity(0.00m)
                .Execute();
            Assert.IsNotNull(saleResponse);

            Thread.Sleep(2500);

            // Adjust the tip using the transaction ID
            var tipAdjustResponse = _device.TipAdjust(3.00m)
                .WithTransactionId(saleResponse.TransactionId)
                .WithEcrId("13")
                .Execute();
            Assert.IsNotNull(tipAdjustResponse);
            Assert.AreEqual(13.00m, tipAdjustResponse.TransactionAmount);
            Assert.AreEqual("00", tipAdjustResponse.DeviceResponseCode);
        }

        [TestMethod]
        // Test that adjusting the tip by invoice number updates the transaction amount and returns a successful response
        public void CreditTipAdjust_ByInvoiceNumber_ShouldAdjustTipSuccessfully() {
            // Perform a sale transaction with zero tip
            var saleResponse = _device.Sale(10.00m)
                .WithEcrId(13)
                .WithClerkId(123)
                .WithGratuity(0.00m)
                .Execute();
            Assert.IsNotNull(saleResponse);

            Thread.Sleep(2500);

            // Adjust the tip using a random invoice number
            var tipAdjustResponse = _device.TipAdjust(3.00m)
                .WithInvoiceNumber(new Random().Next(10000, 99999).ToString())
                .WithEcrId("13")
                .Execute();
            Assert.IsNotNull(tipAdjustResponse);
            Assert.AreEqual(13.00m, tipAdjustResponse.TransactionAmount);
            Assert.AreEqual("00", tipAdjustResponse.DeviceResponseCode);
        }
        #endregion

        [TestMethod]
        public void EndOfDay() {
            _device.EcrId = "13";

            var response = _device.EndOfDay();
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void Verify() {
            var response = _device.Verify()
            .WithEcrId(13)
            .WithCardOnFileIndicator(StoredCredentialInitiator.CardHolder)
            .WithCardBrandTransId("transId")
            .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        #region Unit test case for SaleReversal Scenario
        [TestMethod]
        // Test that reversing a credit sale by terminal reference number returns a successful response
        public void CreditSale_WhenReversedByTerminalRefNumber_ShouldReturnSuccess() {
            // Perform a sale transaction
            var response = _device.Sale(10)
                .WithGratuity(0m)
                .WithEcrId(13)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.TerminalRefNumber);

            // Wait for the transaction to settle before reversing
            Thread.Sleep(5000);

            // Reverse the transaction using the terminal reference number
            var reverseResponse = _device.Reverse()
                .WithTerminalRefNumber(response.TerminalRefNumber)
                .WithEcrId("12")
                .Execute();

            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("00", reverseResponse.DeviceResponseCode);
        }

        [TestMethod]
        // Test that attempting to reverse a sale with an invalid terminal reference number throws the expected GatewayException
        public void SaleReversal_WithInvalidTerminalRefNumber_ShouldThrowInvalidLengthException() {
            var ex = Assert.ThrowsException<GatewayException>(() => {
                var reverseResponse = _device.Reverse()
                    .WithTerminalRefNumber("23")
                    .WithEcrId("12")
                    .Execute();
            });
            Assert.AreEqual("Unexpected Device Response : ERR011 - [tranNo:23]-INVALID LENGTH", ex.Message);
        }
        
        [TestMethod]
        // Test that attempting to reverse a sale without providing a terminal reference number
        // throws the expected GatewayException indicating a missing mandatory field.
        public void SaleReversal_WithoutTerminalRefNumber_ThrowsMissingMandatoryFieldException() {
            var ex = Assert.ThrowsException<GatewayException>(() => {
                var reverseResponse = _device.Reverse()
                    .WithEcrId("12")
                    .Execute();
            });
            Assert.AreEqual("Unexpected Device Response : ERR013 - [tranNo]-MISSING MANDATORY FIELD", ex.Message);
        }
        #endregion

        [TestMethod]
        public void VerifyWithTokenCreation() {
            var response = _device.Verify()
                .WithEcrId(13)
                .WithRequestMultiUseToken(true)
                .WithCardOnFileIndicator(StoredCredentialInitiator.CardHolder)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Token);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod, Ignore]
        // Manually sending card data is currently not supported
        public void VerifyManual() {
            var card = new CreditCardData {
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
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void SaleWithMktData() {
            var card = new CreditCardData {
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
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        #region Unit Test case for Connection Timeout Scenario
        [TestMethod]
        // Test that a connection timeout during a sale operation throws the expected MessageException
        // run this test case without connecting UPA device
        public void TestConnectionTimeout_ShouldThrowException_WhenTimeoutOccurs() {
            // Create a device with an intentionally short timeout to trigger a timeout exception
            IDeviceInterface timeoutDevice = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.NUCLEUS_SATURN_1000,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "192.168.213.79",
                Port = "8081",
                Timeout = 1, // overly-short timeout value to trigger exception in UpaTcpInterface
                RequestIdProvider = new RandomIdProvider()
            });

            Assert.IsNotNull(timeoutDevice);

            // Assert that a MessageException is thrown and contains the expected timeout message
            // With this code to properly assert the exception message using string.Equals:
            var ex = Assert.ThrowsException<MessageException>(() => {
                var response = timeoutDevice.Sale(1m)
                    .WithEcrId(13)
                    .Execute();
            });
            Assert.IsTrue(
                ex.Message.Contains("A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond."));
        }
        #endregion
    }
}