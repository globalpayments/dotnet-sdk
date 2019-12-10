using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Tests.OpenPath.Realex
{
    [TestClass]
    public class CreditTests
    {
        CreditCardData card;

        [TestInitialize]
        public void Init()
        {

            ServicesContainer.ConfigureService(new GatewayConfig
            {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RebatePassword = "rebate",
                RefundPassword = "refund",
                ServiceUrl = "https://api.sandbox.realexpayments.com/epage-remote.cgi",
                OpenPathApiKey = "fteaWC5MYeVKdZ2EaQASDDgCtAS4Fh2zrzW4Yyds",
                OpenPathApiUrl = "http://localhost:50451/v1/globalpayments"
            });

            card = new CreditCardData
            {
                Number = "4111111111111111",
                ExpMonth = 2,
                ExpYear = 2022,
                Cvn = "222",
                CardHolderName = "John Doe"
            };
        }


        [TestMethod]
        public void CreditAuthorization()
        {
            var authorization = card.Authorize(14m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(authorization);
            Assert.AreEqual("00", authorization.ResponseCode, authorization.ResponseMessage);

            var capture = authorization.Capture(14m)
                .Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode, capture.ResponseMessage);
        }


        [TestMethod]
        public void TestOpenPath_BounceBack()
        {
            var transaction = card.Charge(15m)
                .WithCurrency("USD")
                .WithRecurringInfo(RecurringType.Fixed, RecurringSequence.First)
                .WithAccountType(AccountType.CHECKING)
                .WithAddress(new Address
                {
                    City = "Lake Forest",
                    Country = "United States",
                    CountryCode = "US",
                    PostalCode = "92630",
                    Province = "CA"
                })
                .WithClientTransactionId("TRANSACTION001")
                .WithCustomerId("1")
                .WithDescription("Test description")
                .WithEcommerceInfo(new EcommerceInfo
                {
                    Channel = EcommerceChannel.ECOM,
                    ShipDay = 1,
                    ShipMonth = 10
                })
                .WithInvoiceNumber("INVOICE001")
                .WithProductId("PRODUCT001")
                .WithAllowDuplicates(true);
            var response = transaction.Execute();

            if (response.ResponseMessage == "OpenPathBouncedBack")
            {
                response = transaction.Execute();
            }


            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void TestOpenPath_Approved_Processed_In_Stripe()
        {
            var transaction = card.Charge(15m)
                .WithCurrency("USD")
                .WithRecurringInfo(RecurringType.Fixed, RecurringSequence.First)
                .WithAccountType(AccountType.CHECKING)
                .WithAddress(new Address
                {
                    City = "Manila",
                    Country = "Philippines",
                    CountryCode = "PH",
                    PostalCode = "1772",
                    Province = "00"
                })
                .WithClientTransactionId("TRANSACTION001")
                .WithCustomerId("1")
                .WithDescription("Test description")
                .WithEcommerceInfo(new EcommerceInfo
                {
                    Channel = EcommerceChannel.ECOM,
                    ShipDay = 1,
                    ShipMonth = 10
                })
                .WithInvoiceNumber("INVOICE001")
                .WithProductId("PRODUCT001")
                .WithAllowDuplicates(true);
            var response = transaction.Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void TestOpenPath_Approved()
        {
            var transaction = card.Charge(15m)
                .WithCurrency("USD")
                .WithRecurringInfo(RecurringType.Fixed, RecurringSequence.First)
                .WithAccountType(AccountType.CHECKING)
                .WithAddress(new Address
                {
                    City = "Lake Forest",
                    Country = "United States",
                    CountryCode = "US",
                    PostalCode = "92630",
                    Province = "CA"
                })
                .WithClientTransactionId("TRANSACTION001")
                .WithCustomerId("1")
                .WithDescription("Test description")
                .WithEcommerceInfo(new EcommerceInfo
                {
                    Channel = EcommerceChannel.ECOM,
                    ShipDay = 1,
                    ShipMonth = 10
                })
                .WithInvoiceNumber("INVOICE001")
                .WithProductId("PRODUCT001")
                .WithAllowDuplicates(true);
            var response = transaction.Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void TestOpenPath_Declined_Because_Of_Country()
        {
            var response = card.Charge(15m)
                .WithCurrency("USD")
                .WithRecurringInfo(RecurringType.Fixed, RecurringSequence.First)
                .WithAccountType(AccountType.CHECKING)
                .WithAddress(new Address
                {
                    City = "Singapore",
                    Country = "Singapore",
                    CountryCode = "SG",
                    PostalCode = "1772",
                    Province = "NCR"
                })
                .WithClientTransactionId("TRANSACTION001")
                .WithCustomerId("1")
                .WithDescription("Test description")
                .WithEcommerceInfo(new EcommerceInfo
                {
                    Channel = EcommerceChannel.ECOM,
                    ShipDay = 1,
                    ShipMonth = 10
                })
                .WithInvoiceNumber("INVOICE001")
                .WithProductId("PRODUCT001")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }
    }
}
