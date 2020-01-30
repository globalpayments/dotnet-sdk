using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.OpenPath {

    [TestClass]
    public class PorticoCreditTests {

        CreditCardData card;
        CreditTrackData track;
        string currency;

        [TestInitialize]
        public void Init() {

            ServicesContainer.ConfigureService(new GatewayConfig {

                // global payment portico attributes
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w",
                ServiceUrl = "https://cert.api2.heartlandportico.com",

                // openpath attributes
                OpenPathApiKey = "ZFQ4CTapPpZAEmjFAGeZfJsRaaFsafuZepCzV9TY",
                OpenPathApiUrl = "https://staging-api.openpath.io/v1/globalpayments"

            });

            card = new CreditCardData {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            track = new CreditTrackData {
                Value = "<E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|>;",
                EncryptionData = new EncryptionData {
                    Version = "01"
                }
            };

            currency = "USD";

        }

        [TestMethod]
        public void CreditAuthorization() {

            var response = card.Authorize(14m)
                .WithCurrency(currency)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var capture = response.Capture(16m).WithGratuity(2m).Execute();

            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);

        }

        [TestMethod]
        public void CreditAuthWithConvenienceAmt() {
            var response = card.Authorize(14m)
                .WithCurrency(currency)
                .WithAllowDuplicates(true)
                .WithConvenienceAmount(2m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            TransactionSummary report = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(report);
            Assert.AreEqual(2m, report.ConvenienceAmount);
        }

        [TestMethod]
        public void CreditAuthWithShippingAmt() {
            var response = card.Authorize(14m)
                .WithCurrency(currency)
                .WithAllowDuplicates(true)
                .WithShippingAmt(2m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            TransactionSummary report = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(report);
            Assert.AreEqual(2m, report.ShippingAmount);
        }

        [TestMethod]
        public void CreditSale()
        {
            var billingAddress = new Address();
            billingAddress.StreetAddress1 = "200 Spectrum Center Drive";
            billingAddress.StreetAddress2 = "Suite 4123";
            billingAddress.City = "Irvine";
            billingAddress.State = "CA";
            billingAddress.Country = "US";
            billingAddress.PostalCode = "92618";
            var response = card.Charge(15m)
                .WithCurrency(currency)
                .WithAllowDuplicates(true)
                .WithAddress(billingAddress)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditSaleWithConvenienceAmt() {
            var response = card.Charge(15m)
                .WithCurrency(currency)
                .WithAllowDuplicates(true)
                .WithConvenienceAmount(2m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            TransactionSummary report = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(report);
            Assert.AreEqual(2m, report.ConvenienceAmount);
        }

        [TestMethod]
        public void CreditSaleWithShippingAmt() {
            var response = card.Charge(15m)
                .WithCurrency(currency)
                .WithAllowDuplicates(true)
                .WithShippingAmt(2m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            TransactionSummary report = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(report);
            Assert.AreEqual(2m, report.ShippingAmount);
        }

        [TestMethod]
        public void CreditOfflineAuth() {
            var response = card.Authorize(16m)
                .WithCurrency(currency)
                .WithOfflineAuthCode("12345")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditOfflineAuthWithConvenienceAmt() {
            var response = card.Authorize(16m)
                .WithCurrency(currency)
                .WithOfflineAuthCode("12345")
                .WithAllowDuplicates(true)
                .WithConvenienceAmount(2m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            TransactionSummary report = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(report);
            Assert.AreEqual(2m, report.ConvenienceAmount);
        }

        [TestMethod]
        public void CreditOfflineWithShippingAmt() {
            var response = card.Authorize(16m)
                .WithCurrency(currency)
                .WithOfflineAuthCode("12345")
                .WithAllowDuplicates(true)
                .WithShippingAmt(2m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            TransactionSummary report = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(report);
            Assert.AreEqual(2m, report.ShippingAmount);
        }

        [TestMethod]
        public void CreditOfflineSale() {
            var response = card.Charge(17m)
                .WithCurrency(currency)
                .WithOfflineAuthCode("12345")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditOfflineSaleWithConvenienceAmt() {
            var response = card.Charge(17m)
                .WithCurrency(currency)
                .WithOfflineAuthCode("12345")
                .WithAllowDuplicates(true)
                .WithConvenienceAmount(2m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            TransactionSummary report = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(report);
            Assert.AreEqual(2m, report.ConvenienceAmount);
        }

        [TestMethod]
        public void CreditOfflineSaleWithShippingAmt() {
            var response = card.Charge(17m)
                .WithCurrency(currency)
                .WithOfflineAuthCode("12345")
                .WithAllowDuplicates(true)
                .WithShippingAmt(2m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            TransactionSummary report = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(report);
            Assert.AreEqual(2m, report.ShippingAmount);
        }

        [TestMethod]
        public void CreditRefund() {

            var response = card.Refund(16m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

        }

        [TestMethod]
        public void CreditReverse() {
            var response = card.Reverse(15m)
                .WithCurrency(currency)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditVerify() {
            var response = card.Verify()
                .WithAllowDuplicates(true)
                .WithCurrency(currency)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditVoidFromTransactionId() {
            var response = card.Authorize(10.00m)
                .WithCurrency(currency)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var voidResponse = Transaction.FromId(response.TransactionId)
                .Void()
                .Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditTestWithNewCryptoURL() {

            ServicesContainer.ConfigureService(new GatewayConfig {

                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A",
                ServiceUrl = "https://cert.api2-c.heartlandportico.com",

                // openpath attributes
                OpenPathApiKey = "ZFQ4CTapPpZAEmjFAGeZfJsRaaFsafuZepCzV9TY",
                OpenPathApiUrl = "https://unittest-api.openpath.io/v1/globalpayments"

            });
            card = new CreditCardData {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };
            var response = card.Authorize(14m)
                .WithCurrency(currency)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

    }

}
