using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi
{
    [TestClass]
    public class GpApiOpenBankingTest : BaseGpApiTests
    {
        private const string currency = "GBP";
        private const decimal amount = 10.99m;

        [TestInitialize]
        public void TestInitialize() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = AppId,
                AppKey = AppKey,
                Channel = Channel.CardNotPresent,
                RequestLogger = new RequestConsoleLogger(),
                EnableLogging = true,
            });
        }

        [TestMethod]
        public void FasterPaymentsCharge() {
            var bankPayment = FasterPaymentsConfig();

            var trn = bankPayment.Charge(amount)
                .WithCurrency(currency)
                .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                .Execute();

            AssertOpenBankingResponse(trn);

            Debug.Write(trn.BankPaymentResponse.RedirectUrl);
            Thread.Sleep(3000);

            var response = ReportingService.TransactionDetail(trn.TransactionId)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(trn.TransactionId, response.TransactionId);
            Assert.IsNotNull(response.BankPaymentResponse.SortCode);
            Assert.IsNull(response.BankPaymentResponse.Iban);
            Assert.IsNotNull(response.BankPaymentResponse.AccountNumber);
        }

        [TestMethod]
        public void SEPACharge() {
            var bankPayment = SepaConfig();

            var trn = bankPayment.Charge(amount)
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
            Assert.IsNotNull(response.BankPaymentResponse?.Iban);
            Assert.IsNull(response.BankPaymentResponse?.SortCode);
            Assert.IsNull(response.BankPaymentResponse?.AccountNumber);
        }

        [TestMethod]
        public void ReportFindOBTransactionsByStartDateAndEndDate() {
            var response = ReportingService.FindTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated)
                .Where(SearchCriteria.StartDate, StartDate)
                .And(SearchCriteria.EndDate, EndDate)
                .And(SearchCriteria.PaymentProvider, PaymentProvider.OPEN_BANKING)
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Results is List<TransactionSummary>);

            foreach (var rs in response.Results) {
                Assert.AreEqual(EnumConverter.GetMapping(Target.GP_API, PaymentMethodName.BankPayment), rs.PaymentType.ToString().ToUpper());
                Assert.IsTrue(EndDate >= rs.TransactionDate);
                Assert.IsTrue(StartDate <= rs.TransactionDate);
            }
        }
        
        [TestMethod]
        public void FasterPaymentsChargeThenRefund() {
            var bankPayment = FasterPaymentsConfig();

            var trn = bankPayment.Charge(amount)
                .WithCurrency(currency)
                .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                .Execute();

            AssertOpenBankingResponse(trn);

            Thread.Sleep(3000);

            var errorFound = false;
            try {
                trn.Refund(amount)
                    .WithCurrency(currency)
                    .Execute();
            } catch (BuilderException e) {
                errorFound = true;
                Assert.AreEqual("The Refund is not supported for BankPayment", e.Message);
            } finally {
                Assert.IsTrue(errorFound);
            }     
        }
        
        [TestMethod]
        public void SepaChargeThenRefund() {
            var bankPayment = SepaConfig();

            var trn = bankPayment.Charge(amount)
                .WithCurrency("EUR")
                .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                .Execute();

            AssertOpenBankingResponse(trn);

            Thread.Sleep(3000);

            var errorFound = false;
            try {
                trn.Refund(amount)
                    .WithCurrency(currency)
                    .Execute();
            } catch (BuilderException e) {
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
                bankPayment.Charge(amount)
                    .WithCurrency(currency)
                    .Execute();
            } catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadGateway - Unable to process your request due to an error with a system down stream.", e.Message);
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
                bankPayment.Charge(amount)
                    .WithCurrency(currency)
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, null)
                    .Execute();
            } catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadGateway - Unable to process your request due to an error with a system down stream.", e.Message);
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
                bankPayment.Charge(amount)
                    .WithCurrency(currency)
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            } catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadGateway - Unable to process your request due to an error with a system down stream.", e.Message);
                Assert.AreEqual("50046", e.ResponseMessage);
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
                bankPayment.Charge(amount)
                    .WithCurrency(currency)
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            } catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadGateway - Unable to process your request due to an error with a system down stream.", e.Message);
                Assert.AreEqual("50046", e.ResponseMessage);
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
                bankPayment.Charge(amount)
                    .WithCurrency(currency)
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            } catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadGateway - Unable to process your request due to an error with a system down stream.", e.Message);
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
                bankPayment.Charge(amount)
                    .WithCurrency(currency)
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            } catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields payment_method.bank_transfer.bank.name", e.Message);
                Assert.AreEqual("40005", e.ResponseMessage);
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
                bankPayment.Charge(amount)
                    .WithCurrency(currency)
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            } catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadGateway - Unable to process your request due to an error with a system down stream.", e.Message);
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
                bankPayment.Charge(amount)
                    .WithCurrency("EUR")
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            } catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadGateway - Unable to process your request due to an error with a system down stream.", e.Message);
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
                bankPayment.Charge(amount)
                    .WithCurrency("EUR")
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            } catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadGateway - Unable to process your request due to an error with a system down stream.", e.Message);
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
                bankPayment.Charge(amount)
                    .WithCurrency("EUR")
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            } catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields payment_method.bank_transfer.bank.name", e.Message);
                Assert.AreEqual("40005", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }     
        }
        
        [TestMethod]
        public void SepaChargeMissingInvalidCurrency() {
            var bankPayment = SepaConfig();

            var errorFound = false;
            try {
                bankPayment.Charge(amount)
                    .WithCurrency(currency)
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            } catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadGateway - Unable to process your request due to an error with a system down stream.", e.Message);
                Assert.AreEqual("50046", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }     
        }
        
        [TestMethod]
        public void SepaChargeMissingCADCurrency() {
            var bankPayment = SepaConfig();

            var errorFound = false;
            try {
                bankPayment.Charge(amount)
                    .WithCurrency("CAD")
                    .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                    .Execute();
            } catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadGateway - Unable to process your request due to an error with a system down stream.", e.Message);
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
            var bankPayment = new BankPayment();
            bankPayment.AccountNumber = "99999999";
            bankPayment.SortCode = "407777";
            bankPayment.AccountName = "Minal";
            bankPayment.Countries = new List<string>() { "GB", "IE" };
            bankPayment.ReturnUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net";
            bankPayment.StatusUpdateUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net";

            return bankPayment;
        }

        private BankPayment SepaConfig() {
            var bankPayment = new BankPayment();
            bankPayment.Iban = "GB33BUKB20201555555555";
            bankPayment.AccountName = "AccountName";
            bankPayment.ReturnUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net";
            bankPayment.StatusUpdateUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net";

            return bankPayment;
        }
    }
}
