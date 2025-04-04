﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                .WithTerminalRefNumber("1234")
                .WithAutoSubstantiation(autoSub)
                .WithInvoiceNumber("1234")
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
                .WithTerminalRefNumber(preAuthResponse.ReferenceNumber)
                .WithLodging(lodging)
                .Execute();
            Assert.IsNotNull(incrementalPreAuthResponse);
            Assert.AreEqual("00", incrementalPreAuthResponse.DeviceResponseCode);

            var completionResponse = _device.Capture(145m)
                .WithEcrId("13")
                .WithTransactionId(preAuthResponse.TransactionId)
                .WithTerminalRefNumber(preAuthResponse.ReferenceNumber)
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
                    //.WithInvoiceNumber("12")
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
                .WithTransactionId(authResponse.TerminalRefNumber)
                .Execute();
            Assert.IsNotNull(authResponse);
            Assert.AreEqual("00", authResponse.DeviceResponseCode);
        }

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
        public void DeletePreAuth() {
            var response = _device.DeletePreAuth()
                .WithEcrId("13")
                .WithTransactionId("200071138640")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.DeviceResponseText);
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

        [TestMethod]
        public void CreditVoid() {
            var response = _device.Sale(10m)
                .WithEcrId(13)
                .WithClerkId(123)
                .WithGratuity(0m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);

            Thread.Sleep(5000);
            var voidResponse = _device.Void()
            //.WithTransactionId(response.TransactionId) // use TransactionId or TerminalRefNumber
            .WithTerminalRefNumber(response.TerminalRefNumber)
            .WithEcrId("13")
            .Execute();

            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.DeviceResponseCode);
        }

        [TestMethod]
        public void UpdateTaxInfoWithNonCommercialCardFailure() {
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
            Assert.AreEqual("Failed", response.Status);
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
                .WithTerminalRefNumber(responseSale.TerminalRefNumber)
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

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditVoidNoTransactionId() {
            _device.Void()
                .WithEcrId("13")
                .WithTerminalRefNumber("12")
                .Execute();
        }

        [TestMethod]
        public void CreditRefundByCard() {
            var returnResponse = _device.Refund(14m)
                .WithInvoiceNumber(new Random().Next(10000, 99999).ToString())
                .WithEcrId(13)
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
        public void CreditTipAdjust() {
            var saleResponse = _device.Sale(10.00m)
                .WithEcrId(13)
                .WithClerkId(123)
                .WithGratuity(0.00m)
                .Execute();
            Assert.IsNotNull(saleResponse);

            Thread.Sleep(2500);

            var tipAdjustResponse = _device.TipAdjust(3.00m)
                .WithTransactionId(saleResponse.TransactionId)
                .WithEcrId("13")
                .Execute();
            Assert.IsNotNull(tipAdjustResponse);
            Assert.AreEqual(13.00m, tipAdjustResponse.TransactionAmount);
            Assert.AreEqual("00", tipAdjustResponse.DeviceResponseCode);
        }

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

        [TestMethod]
        public void SaleReversal() {
            var response = _device.Sale(10)
                .WithGratuity(0m)
                .WithEcrId(13)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.TerminalRefNumber);

            Thread.Sleep(5000);
            var reverseResponse = _device.Reverse()
                //.WithTransactionId(response.TransactionId)
                .WithTransactionId(response.TerminalRefNumber)
                .WithEcrId("12")
                .Execute();

            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("00", reverseResponse.DeviceResponseCode);
        }

        [TestMethod]
        public void VerifyWithTokenCreation() {
            var response = _device.Verify()
                .WithEcrId(13)
                .WithRequestMultiUseToken(true)
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

        [TestMethod]
        [ExpectedException(typeof(MessageException), "Connection not established within the specified timeout.")]
        public void testConnectionTimeout() {
            IDeviceInterface timeoutDevice = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.NUCLEUS_SATURN_1000,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "192.168.213.79",
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