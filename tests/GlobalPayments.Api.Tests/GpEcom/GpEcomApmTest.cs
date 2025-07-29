using System;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Net.WebRequestMethods;

namespace GlobalPayments.Api.Tests.GpEcom
{
    [TestClass]
    public class GpEcomApmTest
    {
        private const decimal AMOUNT = 10;

        [TestInitialize]
        public void Init() {
            ServicesContainer.ConfigureService(new GpEcomConfig
            {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                //Channel = "ECOM",
                RebatePassword = "rebate",
                RefundPassword = "refund",
                RequestLogger = new RequestConsoleLogger()
            });

            string APP_ID = "p2GgW0PntEUiUh4qXhJHPoDqj3G5GFGI";
            string APP_KEY = "lJk4Np5LoUEilFhH";
            GpApiConfig gpApiConfig = new GpApiConfig {
                AppId = APP_ID,
                AppKey = APP_KEY,
                Channel = Channel.CardNotPresent,
                ServiceUrl = "https://apis-sit.globalpay.com/ucp",
                EnableLogging = true,
                RequestLogger = new RequestConsoleLogger(),
                Country = "PL",
                AccessTokenInfo = new AccessTokenInfo {
                    TransactionProcessingAccountName = "GPECOM_BLIK_APM_Transaction_Processing",
                    RiskAssessmentAccountName = "EOS_RiskAssessment"
                }
            };
            ServicesContainer.ConfigureService(gpApiConfig,"blikConfig");

            GpApiConfig gpApiConfigPayU = new GpApiConfig {
                AppId = "ZbFY1jAz6sqq0GAyIPZe1raLCC7cUlpD",
                AppKey = "4NpIQJDCIDzfTKhA",
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
            ServicesContainer.ConfigureService(gpApiConfigPayU, "payuConfig");
        }

        [TestMethod]
        public void APMForCharge()
        {
            var PaymentMethodDetails = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.SOFORT,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                Descriptor = "Test Transaction",
                Country = "DE",
                AccountHolderName = "James Mason"
            };

            var response = PaymentMethodDetails.Charge(15m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("01", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void TestAlternativePaymentMethodForCharge_withAlternativePaymentMethodResponse()
        {
            var PaymentMethodDetails = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.TESTPAY,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                Descriptor = "Test Transaction",
                Country = "DE",
                AccountHolderName = "James Mason"
            };

            var response = PaymentMethodDetails.Charge(10m)
                .WithCurrency("EUR")
                .WithDescription("New APM")
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsNotNull(response?.AlternativePaymentResponse.AccountHolderName);
            Assert.IsNotNull(response?.AlternativePaymentResponse.Country);
            Assert.IsNotNull(response?.AlternativePaymentResponse.PaymentPurpose);
            Assert.IsNotNull(response?.AlternativePaymentResponse.RedirectUrl);
            Assert.AreEqual("01", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void APMWithoutAmount()
        {
            var PaymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.SOFORTUBERWEISUNG,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                Descriptor = "Test Transaction",
                Country = "DE",
                AccountHolderName = "James Mason"
            };
            try
            {
                PaymentMethod.Charge()
                    .WithCurrency("EUR")
                    .WithDescription("New APM")
                    .Execute();
            }
            catch (BuilderException ex)
            {
                Assert.AreEqual("Amount cannot be null for this transaction type.", ex.Message);
            }
        }

        [TestMethod]
        public void APMWithoutCurrency()
        {
            var PaymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.SOFORTUBERWEISUNG,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                Descriptor = "Test Transaction",
                Country = "DE",
                AccountHolderName = "James Mason"
            };
            try
            {
                PaymentMethod.Charge(10m)
                    .WithDescription("New APM")
                    .Execute();
            }
            catch (BuilderException ex)
            {
                Assert.AreEqual("Currency cannot be null for this transaction type.", ex.Message);
            }
        }

        [TestMethod]
        public void APMWithoutReturnUrl()
        {
            var PaymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.SOFORTUBERWEISUNG,
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                Descriptor = "Test Transaction",
                Country = "DE",
                AccountHolderName = "James Mason"
            };
            try
            {
                PaymentMethod.Charge(10m)
                    .WithCurrency("EUR")
                    .WithDescription("New APM")
                    .Execute();
            }
            catch (BuilderException ex)
            {
                Assert.AreEqual("ReturnUrl cannot be null for this transaction type.", ex.Message);
            }
        }

        [TestMethod]
        public void APMWithoutStatusUpdateUrl()
        {
            var PaymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.SOFORTUBERWEISUNG,
                ReturnUrl = "https://www.example.com/returnUrl",
                Descriptor = "Test Transaction",
                Country = "DE",
                AccountHolderName = "James Mason"
            };
            try
            {
                PaymentMethod.Charge(10m)
                    .WithCurrency("EUR")
                    .WithDescription("New APM")
                    .Execute();
            }
            catch (BuilderException ex)
            {
                Assert.AreEqual("StatusUpdateUrl cannot be null for this transaction type.", ex.Message);
            }
        }

        [TestMethod]
        public void ApmWithoutDescriptor()
        {
            var PaymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.SOFORTUBERWEISUNG,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/returnUrl",
                Country = "DE",
                AccountHolderName = "James Mason"
            };
            try
            {
                PaymentMethod.Charge(10m)
                    .WithCurrency("EUR")
                    .WithDescription("New APM")
                    .Execute();
            }
            catch (BuilderException ex)
            {
                Assert.AreEqual(
                    "PaymentMethod, ReturnUrl, StatusUpdateUrl, AccountHolderName, Country, Descriptor can not be null ",
                    ex.Message);
            }
        }

        [TestMethod]
        public void ApmWithoutAccountHolderName()
        {
            var PaymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.SOFORTUBERWEISUNG,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/returnUrl",
                Descriptor = "Test Transaction",
                Country = "DE",
            };
            try
            {
                PaymentMethod.Charge(10m)
                    .WithCurrency("EUR")
                    .WithDescription("New APM")
                    .Execute();
            }
            catch (BuilderException ex)
            {
                Assert.AreEqual("AccountHolderName cannot be null for this transaction type.", ex.Message);
            }
        }

        [TestMethod]
        public void ApmWithoutCountry()
        {
            var PaymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.SOFORTUBERWEISUNG,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/returnUrl",
                Descriptor = "Test Transaction",
                AccountHolderName = "James Mason"
            };
            try
            {
                PaymentMethod.Charge(10m)
                    .WithCurrency("EUR")
                    .WithDescription("New APM")
                    .Execute();
            }
            catch (BuilderException ex)
            {
                Assert.AreEqual(
                    "PaymentMethod, ReturnUrl, StatusUpdateUrl, AccountHolderName, Country, Descriptor can not be null ",
                    ex.Message);
            }
        }

        [TestMethod]
        public void Apm_AlternativePaymentMethodType_NotAllow()
        {
            var PaymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.SOFORTUBERWEISUNG,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/returnUrl",
                Descriptor = "Test Transaction",
                Country = "DE",
                AccountHolderName = "James Mason"
            };
            try
            {
                PaymentMethod.Charge(10m)
                    .WithCurrency("EUR")
                    .WithDescription("New APM")
                    .Execute();
            }
            catch (GatewayException ex)
            {
                Assert.AreEqual(
                    "Unexpected Gateway Response: 503 - Payment method type [SOFORTUBERWEISUNG] is not allowed",
                    ex.Message);
                Assert.AreEqual("503", ex.ResponseCode);
            }
        }

        [TestMethod]
        public void APMRefundPendingTransaction()
        {
            var PaymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.TESTPAY,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                Descriptor = "Test Transaction",
                Country = "DE",
                AccountHolderName = "James Mason"
            };

            try
            {
                var response = PaymentMethod.Charge(10m)
                    .WithCurrency("EUR")
                    .WithDescription("New APM")
                    .Execute();

                Assert.IsNotNull(response);
                Assert.AreEqual("01", response.ResponseCode);

                var refund = response.Refund(10m)
                    .WithCurrency("EUR")
                    .WithAlternativePaymentType(AlternativePaymentType.TESTPAY)
                    .Execute();
            }
            catch (GatewayException ex)
            {
                Assert.AreEqual("FAILED", ex.ResponseMessage);
            }
        }

        [TestMethod]
        public void APMPayByBankApp()
        {
            var paymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.PAYBYBANKAPP,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                Descriptor = "Test Transaction",
                Country = "GB",
                AccountHolderName = "James Mason"
            };

            var response = paymentMethod.Charge(10m)
                .WithCurrency("GBP")
                .WithDescription("New APM")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("01", response.ResponseCode);
        }

        [TestMethod]
        public void APMPayPal()
        {
            var paymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.PAYPAL,
                ReturnUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net",
                StatusUpdateUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net",
                CancelUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net",
                Descriptor = "Test Transaction",
                Country = "US",
                AccountHolderName = "James Mason"
            };

            const decimal amount = 10m;
            const string currency = "USD";

            var transaction = paymentMethod.Charge(amount)
                .WithCurrency(currency)
                .WithDescription("New APM")
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual("00", transaction.ResponseCode);
            Assert.IsNotNull(transaction.AlternativePaymentResponse.SessionToken);

            Console.WriteLine("https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_express-checkout&token=" +
                              transaction.AlternativePaymentResponse.SessionToken);

            Thread.Sleep(30000);

            transaction.AlternativePaymentResponse.ProviderReference = "SMKGK7K2BLEUA";

            try
            {
                var response = transaction.Confirm(amount)
                    .WithCurrency(currency)
                    .WithAlternativePaymentType(AlternativePaymentType.PAYPAL)
                    .Execute();

                Assert.IsNotNull(response);
                Assert.AreEqual("00", response.ResponseCode);
            }
            catch (GatewayException e)
            {
                Assert.AreEqual("Unexpected Gateway Response: 101 - Payment has not been authorized by the user.",
                    e.Message);
            }
        }

        [TestMethod]
        public void APMForRefund()
        {
            // a refund request requires the original order id
            string orderId = "20180614113445-5b2252d5aa6f9";
            // and the payments reference (pasref) from the authorization response
            string paymentsReference = "15289760863411679";
            // create the refund transaction object
            Transaction transaction = Transaction.FromId(paymentsReference, orderId);

            try
            {
                var response = transaction.Refund(10m)
                    .WithCurrency("EUR")
                    .WithAlternativePaymentType(AlternativePaymentType.TESTPAY) // set the APM method 
                    .Execute();

                Assert.IsNotNull(response);
                Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            }
            catch (GatewayException ex)
            {
                Assert.AreEqual("FAILED", ex.ResponseMessage);
            }
            // send the refund request for payment-credit an APM, we must specify the amount,currency and alternative payment method
        }

        #region GP API Blik APM Test Methods
        // Validates a successful sale transaction using Blik APM with all required fields provided
        [TestMethod]
        public void BlikApmSale_WhenRequestIsValid_ShouldSucceed() {

            var paymentMethodDetails = new AlternativePaymentMethod {
                AlternativePaymentMethodType = AlternativePaymentType.BLIK,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                Descriptor = "Test Transaction",
                Country = "PL",
                AccountHolderName = "James Mason"
            };

            Transaction response = paymentMethodDetails.Charge(AMOUNT)
                            .WithCurrency("PLN")
                            .WithDescription("New APM")
                            .Execute("blikConfig");

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.IsNotNull(response.AlternativePaymentResponse);
            Assert.IsNotNull(response.AlternativePaymentResponse.RedirectUrl);
            Assert.AreEqual("BLIK", response.AlternativePaymentResponse.ProviderName.ToUpper());
        }

        // Verifies that a sale transaction using Blik APM throws an exception when the ReturnUrl is missing.
        [TestMethod]
        public void BlikApmSale_WhenReturnUrlMissing_ShouldThrowException() {

            bool errorFound = false;
            try {
                var paymentMethodDetails = new AlternativePaymentMethod {
                    AlternativePaymentMethodType = AlternativePaymentType.BLIK,
                    StatusUpdateUrl = "https://www.example.com/statusUrl",
                    Descriptor = "Test Transaction",
                    Country = "PL",
                    AccountHolderName = "James Mason"
                };
                paymentMethodDetails.Charge(AMOUNT)
                                    .WithCurrency("PLN")
                                    .WithDescription("New APM")
                                    .Execute("blikConfig");
            }
            catch (BuilderException ex)
            {
                errorFound = true;
                Assert.AreEqual("ReturnUrl cannot be null for this transaction type.", ex.Message);
            }
            finally
            {
                Assert.IsTrue(errorFound);
            }
        }

        // Verifies that a sale transaction using Blik APM throws an exception when the statusUpdateUrl is missing.
        [TestMethod]
        public void BlikApmSale_WhenStatusUpdateUrlMissing_ShouldThrowException() {

            bool errorFound = false;
            try {
                var paymentMethodDetails = new AlternativePaymentMethod {
                    AlternativePaymentMethodType = AlternativePaymentType.BLIK,
                    ReturnUrl = "https://www.example.com/returnUrl",
                    Descriptor = "Test Transaction",
                    Country = "PL",
                    AccountHolderName = "James Mason"
                };
                paymentMethodDetails.Charge(AMOUNT)
                       .WithCurrency("PLN")
                       .WithDescription("New APM")
                       .Execute("blikConfig");
            }
            catch (BuilderException ex)
            {
                errorFound = true;
                Assert.AreEqual("StatusUpdateUrl cannot be null for this transaction type.", ex.Message);
            }
            finally
            {
                Assert.IsTrue(errorFound);
            }
        }

        // Validates that the first refund attempt on a Blik APM transaction is approved successfully.
        [TestMethod]
        public void BlikApmRefund_WhenFirstAttempt_ShouldSucceed() {

            // For refund we have to run sale test and get Transaction ID from that response and paste here in transactionId.
            // Also go to redirect_url from response of sale and approve by entering the code.
            // After some time when status changed to "Captured" run the refund test.
            string transactionId = "TRN_LfMXNiokxA9xzqZCokO5hTTHTdQCHe_02502c97b6aa";

            // create the rebate transaction object
            Transaction transaction = Transaction.FromId(transactionId);

            TransactionSummary transactionDetails =
                    ReportingService
                            .TransactionDetail(transactionId)
                            .Execute();
            transaction.AlternativePaymentResponse = transactionDetails.AlternativePaymentResponse;

            Transaction response =
                    transaction
                            .Refund(AMOUNT)
                            .WithCurrency("PLN")
                            .WithAlternativePaymentType(AlternativePaymentType.BLIK)
                            .Execute("blikConfig");

            Assert.IsNotNull(response);
            Assert.AreEqual("blik", response.AlternativePaymentResponse.ProviderName);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
        }

        // Ensures that a second refund attempt on the same Blik APM transaction returns a "Declined" response.
        [TestMethod]
        public void BlikApmRefund_WhenSecondAttempt_ShouldBeDeclined() {

            // Run Refund with same transaction Id given in first time blik apm refund
            string transactionId = "TRN_LfMXNiokxA9xzqZCokO5hTTHTdQCHe_02502c97b6aa";

            // create the rebate transaction object
            Transaction transaction = Transaction.FromId(transactionId);

            TransactionSummary transactionDetails =
                    ReportingService
                            .TransactionDetail(transactionId)
                            .Execute();
            transaction.AlternativePaymentResponse = transactionDetails.AlternativePaymentResponse;

            Transaction response =
                transaction
                        .Refund(AMOUNT)
                        .WithCurrency("PLN")
                        .WithAlternativePaymentType(AlternativePaymentType.BLIK)
                        .Execute("blikConfig");

            Assert.IsNotNull(response);
            Assert.AreEqual("blik", response.AlternativePaymentResponse.ProviderName);
            Assert.AreEqual("DECLINED", response.ResponseCode);
        }
        #endregion

        #region GP API PayU APM Test Methods
        [TestMethod]
        public void PayUSale_WhenRequestIsValid_ShouldSucceed_WithBankName_ING() {
            var paymentMethodDetails = new AlternativePaymentMethod {

                AlternativePaymentMethodType = AlternativePaymentType.OB,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                Descriptor = "Test Transaction",
                Country = "PL",
                AccountHolderName = "James Mason",
                Bank = BankList.ING_Bank_Slaski
            };

            Transaction response = paymentMethodDetails.Charge(0.01m)
                            .WithCurrency("PLN")
                            .WithDescription("New APM")
                            .Execute("payuConfig");

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.IsNotNull(response.AlternativePaymentResponse);
            Assert.IsNotNull(response.AlternativePaymentResponse.RedirectUrl);
            Assert.AreEqual("BANK_PAYMENT", response.AlternativePaymentResponse.ProviderName.ToUpper());
        }


        [TestMethod]
        public void PayUSale_WhenRequestIsValid_ShouldSucceed_WithBankName_mBank() {
            var paymentMethodDetails = new AlternativePaymentMethod {
                AlternativePaymentMethodType = AlternativePaymentType.OB,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                Descriptor = "Test Transaction",
                Country = "PL",
                AccountHolderName = "James Mason",
                Bank = BankList.M_Bank
            };

            Transaction response = paymentMethodDetails.Charge(0.01m)
                            .WithCurrency("PLN")
                            .WithDescription("New APM")
                            .Execute("payuConfig");

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.IsNotNull(response.AlternativePaymentResponse);
            Assert.IsNotNull(response.AlternativePaymentResponse.RedirectUrl);
            Assert.AreEqual("BANK_PAYMENT", response.AlternativePaymentResponse.ProviderName.ToUpper());
        }
        #endregion
    }
}