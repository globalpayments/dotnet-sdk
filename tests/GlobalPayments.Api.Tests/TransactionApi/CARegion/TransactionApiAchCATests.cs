using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.TransactionApi.Request;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.TransactionApi {
    [TestClass]
    public class TransactionApiAchCATests {

        private eCheck eCheckObj;
        private const string currency = "124";
        private Customer customer;
        private TransactionData transData;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.ConfigureService(new TransactionApiConfig {
                RequestLogger = new RequestConsoleLogger(),
                EnableLogging = true,
                Region = "CA",
                AccountCredential = "800000052925:80039996:58xcGM3pbTtzcidVPY65XBqbB1EzWoD3",
                AppSecret = "lucQKkwz3W3RGzABkSWUVZj1Mb0Yx3E9chAA8ESUVAv"
            }); ;
        }

        [TestInitialize]
        public void TestInitialize() {
            customer = new Customer {
                Title = "Mr.",
                FirstName = "Joe",
                MiddleName = "Henry",
                LastName = "Doe"
            };

            var billingAddress = new Address {
                StreetAddress1 = "2600 NW",
                StreetAddress2 = "23th Street",
                City = "Lindon",
                State = "Utah",
                Country = "USA",
                PostalCode = "84042"
            };

            customer.Address = billingAddress;
            eCheckObj = new eCheck {
                AccountNumber = "001221111221",
                AccountType = AccountType.CHECKING,
                SecCode = SecCode.PPD,
                CheckNumber = "121002039",
                BranchTransitNumber = "12345",
                FinancialInstitutionNumber = "999"
            };

            transData = new TransactionData {
                CountryCode = CountryCode.Country_124,
                PaymentPurposeCode = "150",
                Language = LanguageEnum.En_Ca
            };
        }

        [TestMethod]
        public void TestCheckSalesCARegion() {
            eCheckObj.CheckNumber = DateTime.Now.ToString("yyMMddHHmmss");
            var response = eCheckObj.Charge(23.09m)
                        .WithInvoiceNumber("239017")
                        .WithCurrency(currency)
                        .WithAddress(new Address(), AddressType.Billing)
                        .WithCustomer(customer)
                        .WithTransactionData(transData)
                        .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                        .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("check_submitted", response.ResponseCode);
        }

        [TestMethod]
        public void TestCheckRefundCARegion() {
            eCheckObj.CheckNumber = DateTime.Now.ToString("yyMMddHHmmss");
            var response = eCheckObj.Refund(10.00m)
                        .WithCurrency(currency)
                        .WithAddress(new Address(), AddressType.Billing)
                        .WithCustomer(customer)
                        .WithTransactionData(transData)
                        .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                        .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("check_submitted", response.ResponseCode);
        }

        [TestMethod]
        public void TestCACheckRefundWithCheckSaleId() {
            eCheckObj.CheckNumber = DateTime.Now.ToString("yyMMddHHmmss");
            var checkResponse = eCheckObj.Charge(23.09m)
                        .WithInvoiceNumber("239017")
                        .WithCurrency(currency)
                        .WithAddress(new Address(), AddressType.Billing)
                        .WithCustomer(customer)
                        .WithTransactionData(transData)
                        .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                        .Execute();

            Assert.IsNotNull(checkResponse);
            Assert.AreEqual("check_submitted", checkResponse.ResponseCode);

            Transaction transaction = Transaction.FromId(checkResponse.TransactionId, PaymentMethodType.ACH);
            eCheckObj.CheckNumber = DateTime.Now.ToString("yyMMddHHmmss");
            var response = transaction.Refund(13.09m)
                        .WithCurrency(currency)
                        .WithBankTransferDetails(eCheckObj)
                        .WithPaymentPurposeCode("150")
                        .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("check_submitted", response.ResponseCode);
        }

        [TestMethod]
        public void TestCheckRefundWithReferenceId() {
            eCheckObj.CheckNumber = DateTime.Now.ToString("yyMMddHHmmss");
            var checkResponse = eCheckObj.Charge(23.09m)
                        .WithInvoiceNumber("239017")
                        .WithCurrency(currency)
                        .WithAddress(new Address(), AddressType.Billing)
                        .WithCustomer(customer)
                        .WithTransactionData(transData)
                        .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                        .Execute();

            Assert.IsNotNull(checkResponse);
            Assert.AreEqual("check_submitted", checkResponse.ResponseCode);

            Transaction transaction = Transaction.FromClientTransactionId(checkResponse.ReferenceNumber, PaymentMethodType.ACH);
            eCheckObj.CheckNumber = DateTime.Now.ToString("yyMMddHHmmss");
            var response = transaction.Refund(23.09m)
                        .WithCurrency(currency)
                        .WithBankTransferDetails(eCheckObj)
                        .WithPaymentPurposeCode("150")
                        .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("check_submitted", response.ResponseCode);
        }

        [TestMethod]
        public void CheckSaleByCheckSaleId() {
            eCheckObj.CheckNumber = DateTime.Now.ToString("yyMMddHHmmss");
            var checkResponse = eCheckObj.Charge(23.09m)
                        .WithInvoiceNumber("239017")
                        .WithCurrency(currency)
                        .WithAddress(new Address(), AddressType.Billing)
                        .WithCustomer(customer)
                        .WithTransactionData(transData)
                        .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                        .Execute();

            Assert.IsNotNull(checkResponse);
            Assert.AreEqual("check_submitted", checkResponse.ResponseCode);

            Transaction transaction = Transaction.FromId(checkResponse.TransactionId, TransactionType.Sale, PaymentMethodType.ACH);

            var response = transaction.Fetch().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("check_submitted", response.ResponseCode);
        }

        [TestMethod]
        public void CheckSaleWithReferenceId() {
            eCheckObj.CheckNumber = DateTime.Now.ToString("yyMMddHHmmss");
            var checkResponse = eCheckObj.Charge(23.09m)
                        .WithInvoiceNumber("239017")
                        .WithCurrency(currency)
                        .WithAddress(new Address(), AddressType.Billing)
                        .WithCustomer(customer)
                        .WithTransactionData(transData)
                        .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                        .Execute();

            Assert.IsNotNull(checkResponse);
            Assert.AreEqual("check_submitted", checkResponse.ResponseCode);

            Transaction transaction = Transaction.FromClientTransactionId(
                    checkResponse.ReferenceNumber, TransactionType.Sale, PaymentMethodType.ACH);

            var response = transaction.Fetch().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("check_submitted", response.ResponseCode);
        }

        [TestMethod]
        public void CheckReturnWithCheckReturnId() {
            eCheckObj.CheckNumber = DateTime.Now.ToString("yyMMddHHmmss");
            var returnResponse = eCheckObj.Refund(23.09m)
                        .WithCurrency(currency)
                        .WithAddress(new Address(), AddressType.Billing)
                        .WithCustomer(customer)
                        .WithTransactionData(transData)
                        .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                        .Execute();

            Assert.IsNotNull(returnResponse);
            Assert.AreEqual("check_submitted", returnResponse.ResponseCode);

            Transaction transaction = Transaction.FromId(returnResponse.TransactionId, TransactionType.Refund, PaymentMethodType.ACH);

            var response = transaction.Fetch().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("check_submitted", response.ResponseCode);
        }

        [TestMethod]
        public void CheckReturnWithCheckReferenceId() {
            eCheckObj.CheckNumber = DateTime.Now.ToString("yyMMddHHmmss");
            var returnResponse = eCheckObj.Refund(23.09m)
                        .WithCurrency(currency)
                        .WithAddress(new Address(), AddressType.Billing)
                        .WithCustomer(customer)
                        .WithTransactionData(transData)
                        .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                        .Execute();

            Assert.IsNotNull(returnResponse);
            Assert.AreEqual("check_submitted", returnResponse.ResponseCode);

            Transaction transaction = Transaction.FromClientTransactionId(
                                    returnResponse.ReferenceNumber, TransactionType.Refund, PaymentMethodType.ACH);

            var response = transaction.Fetch().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("check_submitted", response.ResponseCode);
        }

        [TestMethod]
        public void TestCheckSaleWithToken() {
            eCheckObj.CheckNumber = DateTime.Now.ToString("yyMMddHHmmss");
            var response = eCheckObj.Charge(24.23m)
                        .WithInvoiceNumber("239017")
                        .WithRequestMultiUseToken(true)
                        .WithCurrency(currency)
                        .WithAddress(new Address(), AddressType.Billing)
                        .WithCustomer(customer)
                        .WithTransactionData(transData)
                        .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                        .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("check_submitted", response.ResponseCode);
            Assert.IsNotNull(response.CheckDetail.Token);
        }

        [TestMethod]
        public void TestCheckSaleWithMultiUseToken() {
            eCheckObj.CheckNumber = DateTime.Now.ToString("yyMMddHHmmss");
            var response = eCheckObj.Charge(24.59m)
                        .WithInvoiceNumber("239017")
                        .WithRequestMultiUseToken(true)
                        .WithCurrency(currency)
                        .WithAddress(new Address(), AddressType.Billing)
                        .WithCustomer(customer)
                        .WithTransactionData(transData)
                        .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                        .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("check_submitted", response.ResponseCode);
            Assert.IsNotNull(response.CheckDetail.Token);
            eCheckObj.Token = response.CheckDetail.Token;

            var responseUsingToken = eCheckObj.Charge(23.99m)
                        .WithInvoiceNumber("239017")
                        .WithCurrency(currency)
                        .WithAddress(new Address(), AddressType.Billing)
                        .WithCustomer(customer)
                        .WithTransactionData(transData)
                        .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                        .Execute();
            Assert.AreEqual("check_submitted", responseUsingToken.ResponseCode);
        }

        [TestMethod]
        public void TestCheckRefundWithMultiUseToken() {
            eCheckObj.CheckNumber = DateTime.Now.ToString("yyMMddHHmmss");
            var response = eCheckObj.Charge(27.59m)
                        .WithInvoiceNumber("239017")
                        .WithRequestMultiUseToken(true)
                        .WithCurrency(currency)
                        .WithAddress(new Address(), AddressType.Billing)
                        .WithCustomer(customer)
                        .WithTransactionData(transData)
                        .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                        .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("check_submitted", response.ResponseCode);
            Assert.IsNotNull(response.CheckDetail.Token);
            eCheckObj.Token = response.CheckDetail.Token;

            var refundResponse = eCheckObj.Refund(11.75m)
                       .WithCurrency(currency)
                       .WithAddress(new Address(), AddressType.Billing)
                       .WithCustomer(customer)
                       .WithTransactionData(transData)
                       .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                       .Execute();

            Assert.IsNotNull(refundResponse);
            Assert.AreEqual("check_submitted", refundResponse.ResponseCode);
        }
    }
}