using System;
using System.Diagnostics;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpEcom
{

    [TestClass]
    public class GpEcomOpenBankingTest
    {
        private const decimal AMOUNT = 7.8m;
        private const string CURRENCY = "GBP";
        private string RemittanceReferenceValue = "Nike Bounce Shoes";

        [TestInitialize]
        public void TestInitialize() {
            ServicesContainer.RemoveConfig();
            
            var config = new GpEcomConfig {
                MerchantId = "openbankingsandbox",
                SharedSecret = "sharedsecret",
                AccountId = "internet3",
                EnableBankPayment = true,
                ShaHashType = ShaHashType.SHA512,
                RequestLogger = new RequestConsoleLogger()
            };
            ServicesContainer.ConfigureService(config);
        }

        [TestMethod]
        public void OpenBanking_FasterPaymentsCharge() {
            var bankPayment = FasterPaymentsConfig();

            var transaction = bankPayment.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithRemittanceReference(RemittanceReferenceType.TEXT, RemittanceReferenceValue)
                .Execute();

            AssertTransactionResponse(transaction);

            Debug.Write(transaction.BankPaymentResponse.RedirectUrl);

            Thread.Sleep(2000);

            var detail = ReportingService.BankPaymentDetail(transaction.BankPaymentResponse.Id, 1, 10)
                .Execute();

            Assert.IsNotNull(detail);
            Assert.IsNull(detail.Results[0].BankPaymentResponse.SortCode);
            Assert.IsNull(detail.Results[0].BankPaymentResponse.AccountNumber);
            Assert.IsNull(detail.Results[0].BankPaymentResponse.AccountName);
            Assert.IsNull(detail.Results[0].BankPaymentResponse.Iban);
        }
        
        [TestMethod]
        public void OpenBanking_FasterPaymentsCharge_AllSHATypes() {
            foreach (ShaHashType shaHashType in Enum.GetValues(typeof(ShaHashType))) {
                var config = new GpEcomConfig {
                    MerchantId = "openbankingsandbox",
                    SharedSecret = "sharedsecret",
                    AccountId = "internet",
                    EnableBankPayment = true,
                    ShaHashType = shaHashType,
                    RequestLogger = new RequestConsoleLogger()
                };
            
                ServicesContainer.ConfigureService(config, shaHashType.ToString());

                var bankPayment = FasterPaymentsConfig();

                var transaction = bankPayment.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute(shaHashType.ToString());

                AssertTransactionResponse(transaction);

                Thread.Sleep(2000);

                var detail = ReportingService.BankPaymentDetail(transaction.BankPaymentResponse.Id, 1, 10)
                    .Execute(shaHashType.ToString());

                Assert.IsNotNull(detail);
                Assert.IsNull(detail.Results[0].BankPaymentResponse.SortCode);
                Assert.IsNull(detail.Results[0].BankPaymentResponse.AccountNumber);
                Assert.IsNull(detail.Results[0].BankPaymentResponse.AccountName);
                Assert.IsNull(detail.Results[0].BankPaymentResponse.Iban);
            }
        }

        [TestMethod]
        public void OpenBanking_SEPACharge() {
            var bankPayment = SepaConfig();

            var transaction = bankPayment.Charge(AMOUNT)
                .WithCurrency("EUR")
                .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                .Execute();

            AssertTransactionResponse(transaction);

            Debug.Write(transaction.BankPaymentResponse.RedirectUrl);

            Thread.Sleep(2000);

            var detail = ReportingService.BankPaymentDetail(transaction.BankPaymentResponse.Id, 1, 10)
                .Execute();

            Assert.IsNotNull(detail);
            Assert.IsNull(detail.Results[0].BankPaymentResponse.SortCode);
            Assert.IsNull(detail.Results[0].BankPaymentResponse.AccountNumber);
            Assert.IsNull(detail.Results[0].BankPaymentResponse.AccountName);
            Assert.IsNull(detail.Results[0].BankPaymentResponse.Iban);
        }

        [TestMethod]
        public void OpenBanking_BankPaymentList() {
            var result = ReportingService.FindBankPaymentTransactions(1, 10)
                .Where(SearchCriteria.StartDate, DateTime.Now.AddDays(-5))
                .And(SearchCriteria.EndDate, DateTime.Now)
                .Execute();

            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Results.Count);
        }

        [TestMethod]
        public void OpenBanking_BankPaymentList_EmptyList() {
            const BankPaymentStatus status = BankPaymentStatus.REQUEST_CONSUMER_CONSENT;
            var result = ReportingService.FindBankPaymentTransactions(1, 10)
                .Where(SearchCriteria.StartDate, DateTime.Now.AddDays(-5))
                .And(SearchCriteria.EndDate, DateTime.Now)
                .And(SearchCriteria.BankPaymentStatus, status)
                .Execute();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Results.Count);
        }

        [TestMethod]
        public void OpenBanking_BankPaymentList_WithReturnPii() {
            var result = ReportingService.FindBankPaymentTransactions(1, 10)
                .Where(SearchCriteria.StartDate, DateTime.Now.AddDays(-5))
                .And(SearchCriteria.EndDate, DateTime.Now)
                .And(SearchCriteria.ReturnPII, true)
                .Execute();

            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Results.Count);
            foreach (var item in result.Results) {
                if (item.BankPaymentResponse.Type == null) {
                    continue;
                }

                switch ((BankPaymentType)Enum.Parse(typeof(BankPaymentType), item.BankPaymentResponse.Type.ToString())) {
                    case BankPaymentType.FASTERPAYMENTS:
                        Assert.IsNotNull(item.BankPaymentResponse.SortCode);
                        Assert.IsNotNull(item.BankPaymentResponse.AccountNumber);
                        Assert.IsNotNull(item.BankPaymentResponse.AccountName);
                        break;
                    case BankPaymentType.SEPA:
                        Assert.IsNotNull(item.BankPaymentResponse.Iban);
                        Assert.IsNotNull(item.BankPaymentResponse.AccountName);
                        break;
                    default:
                        break;
                }
            }
        }

        [TestMethod]
        public void OpenBanking_GetBankPaymentById() {
            const string obTransId = "DuVGjawYd1m8UkbZyi";
            var detail = ReportingService.BankPaymentDetail(obTransId, 1, 10)
                .Execute();

            Assert.IsNotNull(detail);
            Assert.AreEqual(1, detail.Results.Count);
        }

        [TestMethod]
        public void OpenBanking_GetBankPaymentById_RandomId() {
            var obTransId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 18);
            var detail = ReportingService.BankPaymentDetail(obTransId, 1, 10)
                .Execute();

            Assert.IsNotNull(detail);
            Assert.AreEqual(0, detail.Results.Count);
        }

        [TestMethod]
        public void OpenBanking_GetBankPaymentById_InvalidId() {
            var obTransId = Guid.NewGuid().ToString().Replace("-", "");

            var exceptionCaught = false;
            try {
                ReportingService.BankPaymentDetail(obTransId, 1, 10)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - obTransId is invalid", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void OpenBanking_AllPaymentDetails() {
            var bankPayment = FasterPaymentsConfig();
            bankPayment.Iban = "123456";

            var transaction = bankPayment.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                .Execute();

            AssertTransactionResponse(transaction);
        }

        [TestMethod]
        public void OpenBanking_FasterPaymentsCharge_CADCurrency() {
            var bankPayment = FasterPaymentsConfig();

            var exceptionCaught = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency("CAD")
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - payment.scheme is invalid", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void OpenBanking_FasterPayments_MissingRemittanceReference() {
            var bankPayment = FasterPaymentsConfig();

            var exceptionCaught = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - payment.remittance_reference cannot be null", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void OpenBanking_FasterPayments_MissingRemittanceReferenceValue() {
            var bankPayment = FasterPaymentsConfig();

            var exceptionCaught = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithRemittanceReference(RemittanceReferenceType.PAN, null)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - remittance_reference.value cannot be blank or null", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void OpenBanking_FasterPayments_MissingReturnUrl() {
            var bankPayment = FasterPaymentsConfig();
            bankPayment.ReturnUrl = null;

            var exceptionCaught = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithRemittanceReference(RemittanceReferenceType.PAN, "Nike Bounce Shoes")
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - return_url must not be null", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void OpenBanking_FasterPayments_MissingStatusUrl() {
            var bankPayment = FasterPaymentsConfig();
            bankPayment.StatusUpdateUrl = null;

            var exceptionCaught = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithRemittanceReference(RemittanceReferenceType.PAN, "Nike Bounce Shoes")
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - status_url must not be null", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void OpenBanking_FasterPayments_MissingAccountNumber() {
            var bankPayment = FasterPaymentsConfig();
            bankPayment.AccountNumber = null;

            var exceptionCaught = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithRemittanceReference(RemittanceReferenceType.PAN, "Nike Bounce Shoes")
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - Invalid Payment Scheme required fields", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void OpenBanking_FasterPayments_MissingSortCode() {
            var bankPayment = FasterPaymentsConfig();
            bankPayment.SortCode = null;

            var exceptionCaught = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithRemittanceReference(RemittanceReferenceType.PAN, "Nike Bounce Shoes")
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - Invalid Payment Scheme required fields", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void OpenBanking_FasterPayments_MissingAccountName() {
            var bankPayment = FasterPaymentsConfig();
            bankPayment.AccountName = null;

            var exceptionCaught = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithRemittanceReference(RemittanceReferenceType.PAN, "Nike Bounce Shoes")
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - payment.destination.name is invalid", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void OpenBanking_FasterPayments_InvalidCurrency() {
            var bankPayment = FasterPaymentsConfig();

            var exceptionCaught = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency("EUR")
                    .WithRemittanceReference(RemittanceReferenceType.PAN, "Nike Bounce Shoes")
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.IsTrue(ex.Message.Contains("Invalid Payment Scheme required fields"));
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void OpenBanking_Sepa_MissingIban() {
            var bankPayment = SepaConfig();
            bankPayment.Iban = null;

            var exceptionCaught = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency("EUR")
                    .WithRemittanceReference(RemittanceReferenceType.PAN, "Nike Bounce Shoes")
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.IsTrue(ex.Message.Contains("Invalid Payment Scheme required fields"));
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void OpenBanking_Sepa_MissingName() {
            var bankPayment = SepaConfig();
            bankPayment.AccountName = null;

            var exceptionCaught = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency("EUR")
                    .WithRemittanceReference(RemittanceReferenceType.PAN, "Nike Bounce Shoes")
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.IsTrue(ex.Message.Contains("payment.destination.name is invalid"));
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void OpenBanking_Sepa_InvalidCurrency()  {
            var bankPayment = SepaConfig();

            var exceptionCaught = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithRemittanceReference(RemittanceReferenceType.PAN, "Nike Bounce Shoes")
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - Invalid Payment Scheme required fields", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void FasterPaymentsRefund()
        {
            var bankPayment = FasterPaymentsConfig();

            var trn = bankPayment.Charge(AMOUNT)
                .WithCurrency("GBP")
                .WithRemittanceReference(RemittanceReferenceType.TEXT, RemittanceReferenceValue)
                .Execute();

            AssertTransactionResponse(trn);

            Debug.Write(trn.BankPaymentResponse.RedirectUrl);

            Thread.Sleep(45000);

            var refund = trn.Refund(AMOUNT)
                .WithCurrency("GBP")
                .Execute();

            Assert.AreEqual(BankPaymentStatus.INITIATION_REJECTED.ToString(), refund.ResponseMessage);
            Assert.IsNotNull(refund.TransactionId);
            Assert.IsNotNull(refund.ClientTransactionId);
            Assert.IsNull(refund.BankPaymentResponse.RedirectUrl);

            var response = ReportingService.BankPaymentDetail(trn.BankPaymentResponse.Id, 1, 1)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(1, response.Results.Count);
            Assert.AreEqual(trn.BankPaymentResponse.Id, response.Results[0].TransactionId);
            Assert.IsNull(response.Results[0].BankPaymentResponse.Iban);
            Assert.IsNull(response.Results[0].BankPaymentResponse.SortCode);
            Assert.IsNull(response.Results[0].BankPaymentResponse.AccountNumber);
            Assert.IsNull(response.Results[0].BankPaymentResponse.AccountName);
        }

        [TestMethod]
        public void SEPARefund()
        {
            var bankPayment = SepaConfig();

            var trn = bankPayment.Charge(AMOUNT)
                .WithCurrency("EUR")
                .WithRemittanceReference(RemittanceReferenceType.TEXT, RemittanceReferenceValue)
                .Execute();

            AssertTransactionResponse(trn);

            Debug.Write(trn.BankPaymentResponse.RedirectUrl);

            Thread.Sleep(45000);

            var refund = trn.Refund(AMOUNT)
                .WithCurrency("EUR")
                .Execute();

            Assert.AreEqual(BankPaymentStatus.INITIATION_FAILED.ToString(), refund.ResponseMessage);
            Assert.IsNotNull(refund.TransactionId);
            Assert.IsNotNull(refund.ClientTransactionId);
            Assert.IsNull(refund.BankPaymentResponse.RedirectUrl);

            var response = ReportingService.BankPaymentDetail(trn.BankPaymentResponse.Id, 1, 1)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(1, response.Results.Count);
            Assert.AreEqual(trn.BankPaymentResponse.Id, response.Results[0].TransactionId);
            Assert.IsNull(response.Results[0].BankPaymentResponse.Iban);
            Assert.IsNull(response.Results[0].BankPaymentResponse.SortCode);
            Assert.IsNull(response.Results[0].BankPaymentResponse.AccountNumber);
            Assert.IsNull(response.Results[0].BankPaymentResponse.AccountName);
        }

        private static BankPayment FasterPaymentsConfig() =>
            new BankPayment {
                AccountNumber = "12345678",
                SortCode = "406650",
                AccountName = "AccountName",
                ReturnUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net",
                StatusUpdateUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net"
               
            };

        private static BankPayment SepaConfig() =>
            new BankPayment {
                Iban = "123456",
                AccountName = "AccountName",
                ReturnUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net",
                StatusUpdateUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net"
            };

        private static void AssertTransactionResponse(Transaction transaction) {
            Assert.IsNotNull(transaction);
            Assert.IsNotNull(transaction.TransactionId);
            Assert.IsNotNull(transaction.OrderId);
            Assert.IsNotNull(transaction.BankPaymentResponse.Id);
            Assert.IsNotNull(transaction.BankPaymentResponse.RedirectUrl);
            Assert.AreEqual(BankPaymentStatus.PAYMENT_INITIATED.ToString(), transaction.ResponseMessage);
        }
    }
}