using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.TransactionApi.Request;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.TransactionApi {
    [TestClass]
    public class TransactionApiAchTests {
        
        private eCheck eCheckObj;
        private const string currency = "840";
        private Customer customer;
        private TransactionData transData;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.ConfigureService(new TransactionApiConfig {
                RequestLogger = new RequestConsoleLogger(),
                EnableLogging = true,
                Region = "US",
                AccountCredential = "800000052925:80039990:n7j9rGFUml1Du7rcRs7XGYdJdVMmZKzh",
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
                AccountNumber = "12121",
                RoutingNumber = "112000066",
                AccountType = AccountType.CHECKING,
                SecCode = SecCode.PPD,
                CheckNumber = "121002068"
            };
            
            transData = new TransactionData {
                CountryCode = CountryCode.Country_840,
                EntryClass = "WEB"
            };
        }

        [TestMethod]
        public void TestCheckSales() {
            eCheckObj.CheckNumber = DateTime.Now.ToString("yyMMddHHmmss");
            var response = eCheckObj.Charge(23.09m)
                        .WithInvoiceNumber("239087")
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
        public void TestCheckRefund() {
            eCheckObj.CheckNumber = DateTime.Now.ToString("yyMMddHHmmss");
            transData.EntryClass = "PPD";
            var response = eCheckObj.Refund(23.09m)
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
        public void TestCheckRefundWithCheckSaleId() {
            eCheckObj.CheckNumber = DateTime.Now.ToString("yyMMddHHmmss");
            var checkResponse = eCheckObj.Charge(23.09m)
                .WithInvoiceNumber("239087")
                .WithCurrency(currency)
                .WithAddress(new Address(), AddressType.Billing)
                .WithCustomer(customer)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();

            Assert.IsNotNull(checkResponse);
            Assert.AreEqual("check_submitted", checkResponse.ResponseCode);

            Transaction transaction = Transaction.FromId (checkResponse.TransactionId, PaymentMethodType.ACH );

            var response = transaction.Refund(13.09m)
                        .WithCurrency(currency)
                        .WithBankTransferDetails(eCheckObj)
                        .WithEntryClass("PPD")
                        .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("check_submitted", response.ResponseCode);
        }

        [TestMethod]
        public void TestCheckRefundWithReferenceId() {
            eCheckObj.CheckNumber = DateTime.Now.ToString("yyMMddHHmmss");
            transData.EntryClass = "PPD";
            var checkResponse = eCheckObj.Charge(23.09m)
                .WithInvoiceNumber("239087")
                .WithCurrency(currency)
                .WithAddress(new Address(), AddressType.Billing)
                .WithCustomer(customer)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();

            Assert.IsNotNull(checkResponse);

            Assert.AreEqual("check_submitted", checkResponse.ResponseCode);
            
            Transaction transaction = Transaction.FromClientTransactionId(checkResponse.ReferenceNumber, PaymentMethodType.ACH);

            var response = transaction.Refund(23.09m)
                        .WithCurrency(currency)
                        .WithBankTransferDetails(eCheckObj)
                        .WithEntryClass("PPD")
                        .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("check_submitted", response.ResponseCode);
        }

        [TestMethod]
        public void CheckSaleByCheckSaleId() {
            eCheckObj.CheckNumber = DateTime.Now.ToString("yyMMddHHmmss");
            var checkResponse = eCheckObj.Charge(23.09m)
                .WithInvoiceNumber("239087")
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
                .WithInvoiceNumber("239087")
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
            transData.EntryClass = "PPD";
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
            transData.EntryClass = "PPD";
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
            var response = eCheckObj.Charge(25.79m)
                        .WithInvoiceNumber("239087")
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
            var response = eCheckObj.Charge(25.09m)
                        .WithInvoiceNumber("239087")
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

            var responseUsingToken = eCheckObj.Charge(24.19m)
                        .WithInvoiceNumber("239087")
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
            var response = eCheckObj.Charge(25.09m)
                        .WithInvoiceNumber("239087")
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

            eCheckObj.CheckNumber = DateTime.Now.ToString("yyMMddHHmmss");
            transData.EntryClass = "PPD";
            var refundResponse = eCheckObj.Refund(23.89m)
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