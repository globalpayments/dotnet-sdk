using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi
{
    [TestClass]
    public class GpApiOpenBankingTest : BaseGpApiTests {
        private const string CURRENCY = "GBP";
        private const decimal AMOUNT = 10.99m;

        [TestInitialize]
        public void TestInitialize() {
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);
        }

        [TestMethod]
        public void FasterPaymentsCharge() {
            var bankPayment = FasterPaymentsConfig();

            var trn = bankPayment.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                .Execute();

            AssertOpenBankingResponse(trn);

            Debug.Write(trn.BankPaymentResponse.RedirectUrl);
            Thread.Sleep(3000);

            var response = ReportingService.TransactionDetail(trn.TransactionId)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(trn.TransactionId, response.TransactionId);
            Assert.IsNotNull(response.AccountNumberLast4);
            Assert.IsNotNull(response.BankPaymentResponse.SortCode);
            Assert.IsNotNull(response.BankPaymentResponse.AccountName);
            Assert.IsNull(response.BankPaymentResponse.Iban);
        }

        [TestMethod]
        public void SepaCharge() {
            var bankPayment = SepaConfig();

            var trn = bankPayment.Charge(AMOUNT)
                .WithCurrency("EUR")
                .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                .Execute();

            AssertOpenBankingResponse(trn);
            Debug.Write(trn.BankPaymentResponse.RedirectUrl);
            Thread.Sleep(3000);

            var response = ReportingService.TransactionDetail(trn.TransactionId)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(trn.TransactionId, response.TransactionId);
            Assert.IsNotNull(response.BankPaymentResponse?.MaskedIbanLast4);
            Assert.IsNull(response.BankPaymentResponse?.SortCode);
            Assert.IsNull(response.BankPaymentResponse?.AccountNumber);
        }

        [TestMethod]
        public void ReportFindObTransactionsByStartDateAndEndDate() {
            var response = ReportingService.FindTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated)
                .Where(SearchCriteria.StartDate, StartDate)
                .And(SearchCriteria.EndDate, EndDate)
                .And(SearchCriteria.PaymentProvider, PaymentProvider.OPEN_BANKING)
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Results is List<TransactionSummary>);

            foreach (var rs in response.Results) {
                Assert.AreEqual(EnumConverter.GetMapping(Target.GP_API, PaymentMethodName.BankPayment),
                    rs.PaymentType.ToUpper());
                Assert.IsTrue(EndDate >= rs.TransactionDate);
                Assert.IsTrue(StartDate <= rs.TransactionDate);
            }
        }

        [TestMethod]
        public void FasterPaymentsChargeThenRefund() {
            var bankPayment = FasterPaymentsConfig();

            var trn = bankPayment.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                .Execute();

            AssertOpenBankingResponse(trn);

            Thread.Sleep(3000);

            var errorFound = false;
            try {
                trn.Refund(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .Execute();
            } 
            catch (BuilderException e) {
                errorFound = true;
                Assert.AreEqual("The Refund is not supported for BankPayment", e.Message);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void SepaChargeThenRefund() {
            var bankPayment = SepaConfig();

            var trn = bankPayment.Charge(AMOUNT)
                .WithCurrency("EUR")
                .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                .Execute();

            AssertOpenBankingResponse(trn);

            Thread.Sleep(3000);

            var errorFound = false;
            try {
                trn.Refund(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .Execute();
            } 
            catch (BuilderException e) {
                errorFound = true;
                Assert.AreEqual("The Refund is not supported for BankPayment", e.Message);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void FasterPaymentsMissingRemittanceReference() {
            var bankPayment = FasterPaymentsConfig();

            var errorFound = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .Execute();
            }
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual(
                    "Status Code: BadGateway - Unable to process your request due to an error with a system down stream.",
                    e.Message);
                Assert.AreEqual("50046", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void FasterPaymentsMissingRemittanceReferenceValue() {
            var bankPayment = FasterPaymentsConfig();

            var errorFound = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, null)
                    .Execute();
            } 
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual(
                    "Status Code: BadGateway - Unable to process your request due to an error with a system down stream.",
                    e.Message);
                Assert.AreEqual("50046", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void FasterPaymentsMissingReturnUrl() {
            var bankPayment = FasterPaymentsConfig();
            bankPayment.ReturnUrl = null;

            var errorFound = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            } 
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual(
                    "Status Code: BadRequest - return_url value is invalid. Please check the format and data provided is correct.",
                    e.Message);
                Assert.AreEqual("40090", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void FasterPaymentsMissingStatusUrl() {
            var bankPayment = FasterPaymentsConfig();
            bankPayment.StatusUpdateUrl = null;

            var errorFound = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            } 
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual(
                    "Status Code: BadRequest - status_url value is invalid. Please check the format and data provided is correct.",
                    e.Message);
                Assert.AreEqual("40090", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void FasterPaymentsMissingAccountNumber() {
            var bankPayment = FasterPaymentsConfig();
            bankPayment.AccountNumber = null;

            var errorFound = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            }
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual(
                    "Status Code: BadGateway - Unable to process your request due to an error with a system down stream.",
                    e.Message);
                Assert.AreEqual("50046", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void FasterPaymentsMissingAccountName() {
            var bankPayment = FasterPaymentsConfig();
            bankPayment.AccountName = null;

            var errorFound = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            }
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual(
                    "Status Code: BadRequest - payment_method.bank_transfer.bank.name value is invalid. Please check the format and data provided is correct.",
                    e.Message);
                Assert.AreEqual("40090", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void FasterPaymentsMissingSortCode() {
            var bankPayment = FasterPaymentsConfig();
            bankPayment.SortCode = null;

            var errorFound = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            }
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual(
                    "Status Code: BadGateway - Unable to process your request due to an error with a system down stream.",
                    e.Message);
                Assert.AreEqual("50046", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void FasterPaymentsInvalidCurrency() {
            var bankPayment = FasterPaymentsConfig();

            var errorFound = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency("EUR")
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            }
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual(
                    "Status Code: BadGateway - Unable to process your request due to an error with a system down stream.",
                    e.Message);
                Assert.AreEqual("50046", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void SepaChargeMissingIban() {
            var bankPayment = SepaConfig();
            bankPayment.Iban = null;

            var errorFound = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency("EUR")
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            }
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual(
                    "Status Code: BadGateway - Unable to process your request due to an error with a system down stream.",
                    e.Message);
                Assert.AreEqual("50046", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void SepaChargeMissingAccountName() {
            var bankPayment = SepaConfig();
            bankPayment.AccountName = null;

            var errorFound = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency("EUR")
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            }
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual(
                    "Status Code: BadRequest - payment_method.bank_transfer.bank.name value is invalid. Please check the format and data provided is correct.",
                    e.Message);
                Assert.AreEqual("40090", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void SepaChargeMissingInvalidCurrency() {
            var bankPayment = SepaConfig();

            var errorFound = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            }
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual(
                    "Status Code: BadGateway - Unable to process your request due to an error with a system down stream.",
                    e.Message);
                Assert.AreEqual("50046", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void SepaChargeMissingCadCurrency() {
            var bankPayment = SepaConfig();

            var errorFound = false;
            try {
                bankPayment.Charge(AMOUNT)
                    .WithCurrency("CAD")
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            }
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual(
                    "Status Code: BadGateway - Unable to process your request due to an error with a system down stream.",
                    e.Message);
                Assert.AreEqual("50046", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        private void AssertOpenBankingResponse(Transaction trn) {
            Assert.AreEqual(TransactionStatus.Initiated.ToString().ToUpper(), trn.ResponseMessage.ToUpper());
            Assert.IsNotNull(trn.TransactionId);
            Assert.IsNotNull(trn.BankPaymentResponse.RedirectUrl);
        }

        private BankPayment FasterPaymentsConfig() {
            var bankPayment = new BankPayment {
                AccountNumber = "99999999",
                SortCode = "407777",
                AccountName = "Minal",
                Countries = new List<string>() { "GB", "IE" },
                ReturnUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net",
                StatusUpdateUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net"
            };

            return bankPayment;
        }

        private BankPayment SepaConfig() {
            var bankPayment = new BankPayment {
                Iban = "GB33BUKB20201555555555",
                AccountName = "AccountName",
                ReturnUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net",
                StatusUpdateUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net"
            };

            return bankPayment;
        }
    }
}