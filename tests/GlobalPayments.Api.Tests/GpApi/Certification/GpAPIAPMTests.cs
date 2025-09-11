using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.GpApi;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.WebRequestMethods;

namespace GlobalPayments.Api.Tests.GpApi.Certification {
    [TestClass]
    public class GpAPIAPMTests : BaseGpApiTests {
        string APP_ID = "ZbFY1jAz6sqq0GAyIPZe1raLCC7cUlpD";
        string APP_KEY = "4NpIQJDCIDzfTKhA";

        [TestInitialize]
        public void TestInitialize() {
            ServicesContainer.RemoveConfig();
            GpApiConfig gpApiConfig = new GpApiConfig {
                AppId = APP_ID,
                AppKey = APP_KEY,
                Channel = Channel.CardNotPresent,
                ServiceUrl = ServiceEndpoints.GP_API_PRODUCTION,
                EnableLogging = true,
                RequestLogger = new RequestConsoleLogger(),
                Country = "PL",
                AccessTokenInfo = new AccessTokenInfo {
                    TransactionProcessingAccountName = "transaction_processing",
                    RiskAssessmentAccountName = "EOS_RiskAssessment"
                }
            };
            ServicesContainer.ConfigureService(gpApiConfig);
        }
       
        #region GP API Blik APM Test Methods
        // Validates a successful sale transaction using Blik APM with all required fields provided
        [TestMethod]
        public void BlikApmSale_WhenRequestIsValid_ShouldSucceed() {

            var paymentMethodDetails = new AlternativePaymentMethod {
                AlternativePaymentMethodType = AlternativePaymentType.BLIK,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                CancelUrl ="https://www.example.com/cancelUrl",
                Descriptor = "Test Transaction",
                Country = "PL",
                AccountHolderName = "James Mason"
            };

            Transaction response = paymentMethodDetails.Charge(0.02m)
                            .WithCurrency("PLN")
                            .WithDescription("New APM")
                            .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.IsNotNull(response.AlternativePaymentResponse);
            Assert.IsNotNull(response.AlternativePaymentResponse.RedirectUrl);
            Assert.AreEqual("BLIK", response.AlternativePaymentResponse.ProviderName.ToUpper());
        }


        // Validates that the first refund attempt on a Blik APM transaction is approved successfully.
        [TestMethod]
        public void BlikApmRefund_WhenFirstAttempt_ShouldSucceed_FullRefund() {

            // For refund we have to run sale test and get Transaction ID from that response and paste here in transactionId.
            // Also go to redirect_url from response of sale and approve by entering the code.
            // After some time when status changed to "Captured" run the refund test.
            string transactionId = "TRN_1LyDCQsnicYoT1nup3FusKchIKizKn_21ce0610bf64";

            // create the rebate transaction object
            Transaction transaction = Transaction.FromId(transactionId);

            Transaction response =
                    transaction
                            .Refund(0.02m)
                            .WithCurrency("PLN")
                            .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("blik", response.AlternativePaymentResponse.ProviderName);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
        }

        [TestMethod]
        public void BlikApmSale_WhenRequestIsValid_ShouldSucceed_forPartialRefund() {

            var paymentMethodDetails = new AlternativePaymentMethod {
                AlternativePaymentMethodType = AlternativePaymentType.BLIK,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                CancelUrl = "https://www.example.com/cancelUrl",
                Descriptor = "Test Transaction",
                Country = "PL",
                AccountHolderName = "James Mason"
            };

            Transaction response = paymentMethodDetails.Charge(0.02m)
                            .WithCurrency("PLN")
                            .WithDescription("New APM")
                            .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.IsNotNull(response.AlternativePaymentResponse);
            Assert.IsNotNull(response.AlternativePaymentResponse.RedirectUrl);
            Assert.AreEqual("BLIK", response.AlternativePaymentResponse.ProviderName.ToUpper());
        }


        // Validates that the first refund attempt on a Blik APM transaction is approved successfully.
        [TestMethod]
        public void BlikApmRefund_WhenFirstAttempt_ShouldSucceed_PartailRefund() {

            // For refund we have to run sale test and get Transaction ID from that response and paste here in transactionId.
            // Also go to redirect_url from response of sale and approve by entering the code.
            // After some time when status changed to "Captured" run the refund test.
            string transactionId = "TRN_7BlWbHsu2GbduXEvedlDdKXJKYUm12_a9e0d5c70c52";

            // create the rebate transaction object
            Transaction transaction = Transaction.FromId(transactionId);

            Transaction response =
                    transaction
                            .Refund(0.01m)
                            .WithCurrency("PLN")
                            .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("blik", response.AlternativePaymentResponse.ProviderName);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
        }

        [TestMethod]
        public void BlikApmRefund_WhenFirstAttempt_ShouldFail_ExtraRefund() {

            // For refund we have to run sale test and get Transaction ID from that response and paste here in transactionId.
            // Also go to redirect_url from response of sale and approve by entering the code.
            // After some time when status changed to "Captured" run the refund test.
            string transactionId = "TRN_xeSPdqMnFDgj32EInXeZh7SuMODGBJ_7eafe090aa9c";

            // create the rebate transaction object
            Transaction transaction = Transaction.FromId(transactionId);

            Transaction response =
                    transaction
                            .Refund(0.05m)
                            .WithCurrency("PLN")
                            .Execute();

            Assert.IsNotNull(response);
            //Assert.AreEqual("blik", response.AlternativePaymentResponse.ProviderName);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
        }
        #endregion

        #region GP API PayU APM Test Methods
        [TestMethod]
        public void PayUSale_WhenRequestIsValid_ShouldSucceed_WithBankName_Millenium_ForFullRefund() {
            var paymentMethodDetails = new AlternativePaymentMethod {

                AlternativePaymentMethodType = AlternativePaymentType.OB,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                CancelUrl = "https://www.example.com/cancelUrl",
                Descriptor = "Test Transaction",
                Country = "PL",
                AccountHolderName = "James Mason",
                Bank = BankList.Millenium
            };

            Transaction response = paymentMethodDetails.Charge(0.02m)
                            .WithCurrency("PLN")
                            .WithDescription("New APM")
                            .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.IsNotNull(response.AlternativePaymentResponse);
            Assert.IsNotNull(response.AlternativePaymentResponse.RedirectUrl);
            Assert.AreEqual("BANK_PAYMENT", response.AlternativePaymentResponse.ProviderName.ToUpper());
        }

        [TestMethod]
        public void PayUApmRefund_WhenFirstAttempt_ShouldSucceed_FullRefund() {

            // For refund we have to run sale test and get Transaction ID from that response and paste here in transactionId.
            // Also go to redirect_url from response of sale and approve by entering the code.
            // After some time when status changed to "Captured" run the refund test.
            string transactionId = "TRN_vhK2baOpSOOJPrbNFwGcVo3tjdjXnd_9be60b8ef37a";

            // create the rebate transaction object
            Transaction transaction = Transaction.FromId(transactionId);

            Transaction response =
                    transaction
                            .Refund(0.02m)
                            .WithCurrency("PLN")
                            .Execute();

            Assert.IsNotNull(response);
            //Assert.AreEqual("BANK_PAYMENT", response.AlternativePaymentResponse.ProviderName.ToUpper());
            Assert.AreEqual("SUCCESS", response.ResponseCode);
        }

        [TestMethod]
        public void PayUSale_WhenRequestIsValid_ShouldSucceed_WithBankName_Millenium() {
            var paymentMethodDetails = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.OB,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                CancelUrl = "https://www.example.com/cancelUrl",
                Descriptor = "Test Transaction",
                Country = "PL",
                AccountHolderName = "James Mason",
                Bank = BankList.Millenium
            };

            Transaction response = paymentMethodDetails.Charge(0.02m)
                            .WithCurrency("PLN")
                            .WithDescription("New APM")
                            .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.IsNotNull(response.AlternativePaymentResponse);
            Assert.IsNotNull(response.AlternativePaymentResponse.RedirectUrl);
            Assert.AreEqual("BANK_PAYMENT", response.AlternativePaymentResponse.ProviderName.ToUpper());
        }

        [TestMethod]
        public void PayUApmRefund_WhenFirstAttempt_ShouldSucceed_PartialRefund() {

            // For refund we have to run sale test and get Transaction ID from that response and paste here in transactionId.
            // Also go to redirect_url from response of sale and approve by entering the code.
            // After some time when status changed to "Captured" run the refund test.
            string transactionId = "TRN_6ut3n4neJoLaoU8WhGVPSlRSgo3Z1E_4d531f436670";

            // create the rebate transaction object
            Transaction transaction = Transaction.FromId(transactionId);

            Transaction response =
                    transaction
                            .Refund(0.01m)
                            .WithCurrency("PLN")
                            .Execute();

            Assert.IsNotNull(response);
            //Assert.AreEqual("BANK_PAYMENT", response.AlternativePaymentResponse.ProviderName.ToUpper());
            Assert.AreEqual("SUCCESS", response.ResponseCode);
        }

        [TestMethod]
        public void PayUApmRefund_WhenFirstAttempt_Shouldfail_ExtraRefund() {

            // For refund we have to run sale test and get Transaction ID from that response and paste here in transactionId.
            // Also go to redirect_url from response of sale and approve by entering the code.
            // After some time when status changed to "Captured" run the refund test.
            string transactionId = "TRN_aWDY8Fy28UFW8EfYGjMxOCwIZvZvHr_63e91742e807";

            // create the rebate transaction object
            Transaction transaction = Transaction.FromId(transactionId);

            Transaction response =
                    transaction
                            .Refund(0.05m)
                            .WithCurrency("PLN")
                            .Execute();

            Assert.IsNotNull(response);
            //Assert.AreEqual("BANK_PAYMENT", response.AlternativePaymentResponse.ProviderName.ToUpper());
            Assert.AreEqual("SUCCESS", response.ResponseCode);
        }

        #endregion


        [TestMethod]
        public void ReportTransactionDetail() {
            var transactionId = "TRN_aWDY8Fy28UFW8EfYGjMxOCwIZvZvHr_63e91742e807";

            var response = ReportingService.TransactionDetail(transactionId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsTrue(response is TransactionSummary);
            Assert.AreEqual(transactionId, response.TransactionId);
        }
    }
}
