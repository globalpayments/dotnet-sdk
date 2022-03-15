using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils.Logging; 
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GlobalPayments.Api.Tests.GpApi
{
    [TestClass]
    public class GpApiApmTest : BaseGpApiTests
    {
        private AlternativePaymentMethod paymentMethod;
        private string currency;
        private Address shippingAddress;

        [TestInitialize]
        public void TestInitialize()
        {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = APP_ID,
                AppKey = APP_KEY,
                Channel = Channel.CardNotPresent,
                RequestLogger = new RequestConsoleLogger()
            });

            paymentMethod = new AlternativePaymentMethod {
                AlternativePaymentMethodType = AlternativePaymentType.PAYPAL,
                ReturnUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net",
                StatusUpdateUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net",
                CancelUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net",
                Descriptor = "Test Transaction",
                Country = "GB",
                AccountHolderName = "James Mason"
            };

            currency = "USD";

            // shipping address
            shippingAddress = new Address {
                StreetAddress1 = "Apartment 852",
                StreetAddress2 = "Complex 741",
                StreetAddress3 = "no",
                City = "Chicago",
                PostalCode = "5001",
                State = "IL",
                CountryCode = "US"
            };
        }

        /**
         * How to have a success running test. When you will run the test in the console it will be printed the
         * paypal redirect url. You need to copy the link and open it in a browser, do the login wih your paypal
         * credentials and authorize the payment in the paypal form. You will be redirected to a blank page with a
         * printed message like this: { "success": true }. This has to be done within a 25 seconds timeframe.
         * In case you need more time update the sleep() to what you need.
         */

        [TestMethod]
        public void PayPalCharge_fullCycle() {
            var response = paymentMethod.Charge(1.34m)
                .WithCurrency(currency)
                .WithDescription("New APM")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual("INITIATED", response.ResponseMessage);

            Console.WriteLine("copy the link and open it in a browser, do the login wih your paypal credentials and authorize the payment in the paypal form. You will be redirected to a blank page with a printed message like this: { \"success\": true }. This has to be done within a 25 seconds timeframe.");
            Console.WriteLine(response.AlternativePaymentResponse.RedirectUrl);

            Thread.Sleep(25000);

            var startDate = DateTime.Now;

            var responseFind = ReportingService.FindTransactionsPaged(1, 1)
                .WithTransactionId(response.TransactionId)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, startDate)
                .Execute();

            Assert.IsNotNull(responseFind);
            Assert.IsTrue(responseFind.TotalRecordCount > 0);

            var transactionSummary = responseFind.Results.FirstOrDefault();
            Assert.IsTrue(transactionSummary.AlternativePaymentResponse is AlternativePaymentResponse);
            Assert.AreEqual(AlternativePaymentType.PAYPAL.ToString().ToLower(), transactionSummary.AlternativePaymentResponse.ProviderName);
            Assert.AreEqual("PENDING", transactionSummary.TransactionStatus);
            Assert.IsNotNull(transactionSummary.AlternativePaymentResponse.ProviderReference);

            var transaction = Transaction.FromId(transactionSummary.TransactionId, null, PaymentMethodType.APM);
            transaction.AlternativePaymentResponse = transactionSummary.AlternativePaymentResponse;

            response = transaction.Confirm().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual("CAPTURED", response.ResponseMessage);
        }

        [TestMethod]
        public void PayPalCapture_fullCycle() {
            var response = paymentMethod.Authorize(1.34m)
                .WithCurrency(currency)
                .WithDescription("New APM")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual("INITIATED", response.ResponseMessage);

            Console.WriteLine("copy the link and open it in a browser, do the login wih your paypal credentials and authorize the payment in the paypal form. You will be redirected to a blank page with a printed message like this: { \"success\": true }. This has to be done within a 25 seconds timeframe.");
            Console.WriteLine(response.AlternativePaymentResponse.RedirectUrl);

            Thread.Sleep(25000);

            var startDate = DateTime.Now;

            var responseFind = ReportingService.FindTransactionsPaged(1, 1)
                .WithTransactionId(response.TransactionId)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, startDate)
                .Execute();

            Assert.IsNotNull(responseFind);
            Assert.IsTrue(responseFind.TotalRecordCount > 0);

            var transactionSummary = responseFind.Results.FirstOrDefault();
            Assert.IsFalse(string.IsNullOrEmpty(transactionSummary.TransactionId));
            Assert.IsTrue(transactionSummary.AlternativePaymentResponse is AlternativePaymentResponse);
            Assert.AreEqual(AlternativePaymentType.PAYPAL.ToString().ToLower(), transactionSummary.AlternativePaymentResponse.ProviderName);
            Assert.AreEqual("PENDING", transactionSummary.TransactionStatus);
            Assert.IsNotNull(transactionSummary.AlternativePaymentResponse.ProviderReference);

            var transaction = Transaction.FromId(transactionSummary.TransactionId, null, PaymentMethodType.APM);
            transaction.AlternativePaymentResponse = transactionSummary.AlternativePaymentResponse;

            response = transaction.Confirm().Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual("PREAUTHORIZED", response.ResponseMessage);

            var capture = transaction.Capture().Execute();

            Assert.IsNotNull(capture);
            Assert.AreEqual("SUCCESS", capture.ResponseCode);
            Assert.AreEqual("CAPTURED", capture.ResponseMessage);
        }

        [TestMethod]
        public void PayPalFullCycle_Refund() {
            var trn = paymentMethod.Charge(1.22m)
                .WithCurrency(currency)
                .WithDescription("New APM")
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual("INITIATED", trn.ResponseMessage);

            Console.WriteLine("copy the link and open it in a browser, do the login wih your paypal credentials and authorize the payment in the paypal form. You will be redirected to a blank page with a printed message like this: { \"success\": true }. This has to be done within a 25 seconds timeframe.");
            Console.WriteLine(trn.AlternativePaymentResponse.RedirectUrl);

            Thread.Sleep(25000);

            var startDate = DateTime.Now;

            var responseFind = ReportingService.FindTransactionsPaged(1, 1)
                .WithTransactionId(trn.TransactionId)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, startDate)
                .Execute();

            Assert.IsNotNull(responseFind);
            Assert.IsTrue(responseFind.TotalRecordCount > 0);

            var transactionSummary = responseFind.Results.FirstOrDefault();
            Assert.IsFalse(string.IsNullOrEmpty(transactionSummary.TransactionId));
            Assert.IsTrue(transactionSummary.AlternativePaymentResponse is AlternativePaymentResponse);
            Assert.AreEqual(AlternativePaymentType.PAYPAL.ToString().ToLower(), transactionSummary.AlternativePaymentResponse.ProviderName);
            Assert.AreEqual("PENDING", transactionSummary.TransactionStatus);
            Assert.IsNotNull(transactionSummary.AlternativePaymentResponse.ProviderReference);

            var transaction = Transaction.FromId(transactionSummary.TransactionId, null, PaymentMethodType.APM);
            transaction.AlternativePaymentResponse = transactionSummary.AlternativePaymentResponse;

            var response = transaction.Confirm().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual("CAPTURED", response.ResponseMessage);

            var trnRefund = transaction.Refund().WithCurrency(currency).Execute();
            Assert.IsNotNull(trnRefund);
            Assert.AreEqual("SUCCESS", trnRefund.ResponseCode);
            Assert.AreEqual("CAPTURED", trnRefund.ResponseMessage);
        }

        [TestMethod, Ignore]
        //Sandbox returning: Can't CAPTURE a Transaction that is already CAPTURED
        public void PayPalFullCycle_Reverse() {
            var trn = paymentMethod.Charge(1.22m)
                .WithCurrency(currency)
                .WithDescription("New APM")
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual("INITIATED", trn.ResponseMessage);

            Console.WriteLine("copy the link and open it in a browser, do the login wih your paypal credentials and authorize the payment in the paypal form. You will be redirected to a blank page with a printed message like this: { \"success\": true }. This has to be done within a 25 seconds timeframe.");
            Console.WriteLine(trn.AlternativePaymentResponse.RedirectUrl);

            Thread.Sleep(25000);

            var startDate = DateTime.Now;

            var response = ReportingService.FindTransactionsPaged(1, 1)
                                .WithTransactionId(trn.TransactionId)
                                .Where(SearchCriteria.StartDate, startDate)
                                .And(SearchCriteria.EndDate, startDate)
                                .Execute();

            Assert.IsNotNull(response);
            Assert.IsTrue(response.TotalRecordCount > 0);
            var transactionSummary = response.Results.FirstOrDefault();
            Assert.IsFalse(string.IsNullOrEmpty(transactionSummary.TransactionId));
            Assert.IsTrue(transactionSummary.AlternativePaymentResponse is AlternativePaymentResponse);
            Assert.AreEqual(AlternativePaymentType.PAYPAL.ToString().ToLower(), transactionSummary.AlternativePaymentResponse.ProviderName);
            Assert.AreEqual("PENDING", transactionSummary.TransactionStatus);
            Assert.IsNotNull(transactionSummary.AlternativePaymentResponse.ProviderReference);

            var transaction = Transaction.FromId(transactionSummary.TransactionId, null,PaymentMethodType.APM);
            transaction.AlternativePaymentResponse = transactionSummary.AlternativePaymentResponse;

            var responsetrn = transaction.Confirm().Execute();
            Assert.IsNotNull(responsetrn);
            Assert.AreEqual("SUCCESS", responsetrn.ResponseCode);
            Assert.AreEqual("CAPTURED", responsetrn.ResponseMessage);

            var trnReverse = responsetrn.Reverse().WithCurrency(currency).Execute();

            Assert.IsNotNull(trnReverse);
            Assert.AreEqual("SUCCESS", trnReverse.ResponseCode);
            Assert.AreEqual("REVERSED", trnReverse.ResponseMessage);
        }

        [TestMethod]
        public void PayPalMultiCapture_fullCycle() {
            var response = paymentMethod.Authorize(3m)
                .WithCurrency(currency)
                .WithMultiCapture(true)
                .WithDescription("PayPal Multicapture")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual("INITIATED", response.ResponseMessage);

            Console.WriteLine("copy the link and open it in a browser, do the login wih your paypal credentials and authorize the payment in the paypal form. You will be redirected to a blank page with a printed message like this: { \"success\": true }. This has to be done within a 25 seconds timeframe.");
            Console.WriteLine(response.AlternativePaymentResponse.RedirectUrl);

            Thread.Sleep(25000);

            var startDate = DateTime.Now;

            var responseFind = ReportingService.FindTransactionsPaged(1, 1)
                .WithTransactionId(response.TransactionId)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, startDate)
                .Execute();

            Assert.IsNotNull(responseFind);
            Assert.IsTrue(responseFind.TotalRecordCount > 0);

            var transactionSummary = responseFind.Results.FirstOrDefault();
            Assert.IsFalse(string.IsNullOrEmpty(transactionSummary.TransactionId));
            Assert.IsTrue(transactionSummary.AlternativePaymentResponse is AlternativePaymentResponse);
            Assert.AreEqual(AlternativePaymentType.PAYPAL.ToString().ToLower(), transactionSummary.AlternativePaymentResponse.ProviderName);
            Assert.AreEqual("PENDING", transactionSummary.TransactionStatus);
            Assert.IsNotNull(transactionSummary.AlternativePaymentResponse.ProviderReference);

            var transaction = Transaction.FromId(transactionSummary.TransactionId, null, PaymentMethodType.APM);
            transaction.AlternativePaymentResponse = transactionSummary.AlternativePaymentResponse;

            var responseConf = transaction.Confirm().Execute();
            
            Assert.IsNotNull(responseConf);
            Assert.AreEqual("SUCCESS", responseConf.ResponseCode);
            Assert.AreEqual("PREAUTHORIZED", responseConf.ResponseMessage);

            var capture = transaction.Capture(1).Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("SUCCESS", capture.ResponseCode);
            Assert.AreEqual("CAPTURED", capture.ResponseMessage);

            var capture2 = transaction.Capture(2).Execute();
            Assert.IsNotNull(capture2);
            Assert.AreEqual("SUCCESS", capture2.ResponseCode);
            Assert.AreEqual("CAPTURED", capture2.ResponseMessage);
        }

        [TestMethod]
        // unit_amount is actually the total amount for the item; waiting info about the shipping_discount
        public void PayPalChargeWithoutConfirm() {
            var order = new OrderDetails {
                InsuranceAmount = 10,
                HandlingAmount = 2,
                Description = "Order description",
                HasInsurance = true
            };

            var products = new List<Product> {
                new Product {
                    ProductId = "SKU251584",
                    ProductName = "Magazine Subscription",
                    Description = "Product description 1",
                    Quantity = 1,
                    UnitPrice = 7,
                    UnitCurrency = currency,
                    TaxAmount = 0.5m
                },
                new Product {
                    ProductId = "SKU8884784",
                    ProductName = "Charger",
                    Description = "Product description 2",
                    Quantity = 2,
                    UnitPrice = 6,
                    UnitCurrency = currency,
                    TaxAmount = 0.5m
                }
            };

            var response = paymentMethod.Charge(29m)
                        .WithCurrency(currency)
                        .WithDescription("New APM Uplift")
                        .WithAddress(shippingAddress, AddressType.Shipping)
                        .WithCustomerId("REF123456789")
                        .WithMiscProductData(products)
                        .WithPhoneNumber("44", "124 445 556", PhoneNumberType.Work)
                        .WithPhoneNumber("44", "124 444 333", PhoneNumberType.Home)
                        .WithPhoneNumber("1", "258 3697 144", PhoneNumberType.Shipping)
                        .WithOrderId("124214-214221")
                        .WithShippingAmt(3)
                        //.WithShippingDiscount(1)
                        .WithOrderDetails(order)
                        .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual("INITIATED", response.ResponseMessage);
        }
    }
}
