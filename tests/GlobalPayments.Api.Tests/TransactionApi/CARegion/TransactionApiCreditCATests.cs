using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.TransactionApi.Request;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.TransactionApi {
    [TestClass]
    public class TransactionApiCreditCATests {

        private CreditCardData card = new CreditCardData();
        private const string currency = "124";
        private Customer customer;
        private Address address;
        private TransactionData transData;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.ConfigureService(new TransactionApiConfig {
                RequestLogger = new RequestConsoleLogger(),
                EnableLogging = true,
                Region = "CA",
                AccountCredential = "800000052925:80039923:eWcWNJhfxiJ7QyEHSHndWk4VHKbSmSue",
                AppSecret = "lucQKkwz3W3RGzABkSWUVZj1Mb0Yx3E9chAA8ESUVAv"
            }); ;
        }

        [TestInitialize]
        public void TestInitialize() {
            card = new CreditCardData {
                Number = "4012888888881881",
                ExpMonth = 03,
                ExpYear = 23,
                Cvn = "345",
                CardHolderName = "Joe Doe"
            };

            customer = new Customer {
                Id = "id", 
                Title = "title",
                FirstName = "firstname",
                MiddleName = "middlename",
                LastName = "lastname",
                BusinessName  = "BusinessName",
                Email = "email.email@email.com",
                MobilePhone = "mobilephone",
                Comments = "notes"
            };

            address = new Address {
                StreetAddress1 = "StreetAddress1",
                StreetAddress2 = "StreetAddress2",
                City = "city",
                State = "state",
                Country = "country",
                PostalCode = "postalcode",
            };
            transData = new TransactionData {
                CountryCode = CountryCode.Country_124,
                EcommerceIndicator = EcomIndicator.Indicator1,
                SoftDescriptor = "soft",
                AddressVerificationService = true,
                CreateToken = false,
                GenerateReceipt = true,
                PartialApproval = true
            };
        }

        [TestMethod]
        public void CreditAuths() {
            var response = card.Authorize(00.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", response.ResponseCode);
        }

        [TestMethod]
        public void CreditSales() {
            var response = card.Charge(11.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", response.ResponseCode);
        }

        [TestMethod]
        public void CreditVerify() {
            var response = card.Verify()
                .WithAmount(00.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", response.ResponseCode);
        }

        [TestMethod]
        public void CreditVoidWithCreditSaleId() {
            var saleResponse = card.Charge(12.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", saleResponse.ResponseCode);

            Transaction transaction = Transaction.FromId(saleResponse.TransactionId, TransactionType.Sale, PaymentMethodType.Credit);
            var response = transaction.Void(null, 12.00m)
                .WithCurrency(currency)
                .WithGenerateReceipt(true)
                .Execute();
            Assert.AreEqual("voided", response.ResponseCode);
        }

        [TestMethod]
        public void CreditVoidWithReferenceId() {
            var saleResponse = card.Charge(12.50m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", saleResponse.ResponseCode);

            Transaction transaction = Transaction.FromClientTransactionId(saleResponse.ReferenceNumber, TransactionType.Sale, PaymentMethodType.Credit);
            var response = transaction.Void(null, 12.50m)
                .WithCurrency(currency)
                .WithGenerateReceipt(true)
                .Execute();
            Assert.AreEqual("voided", response.ResponseCode);
        }

        [TestMethod]
        public void CreditVoidWithCreditReturnId() {
            var returnResponse = card.Refund(13.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithAddress(address, AddressType.Billing)
                .WithCustomer(customer)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", returnResponse.ResponseCode);

            Transaction transaction = Transaction.FromId(returnResponse.TransactionId, TransactionType.Refund, PaymentMethodType.Credit);
            var response = transaction.Void(null, 13.00m)
                .WithCurrency(currency)
                .WithGenerateReceipt(true)
                .Execute();
            Assert.AreEqual("voided", response.ResponseCode);
        }

        [TestMethod]
        public void CreditVoidWithCreditReturnReferenceId() {
            var returnResponse = card.Refund(14.05m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithAddress(address, AddressType.Billing)
                .WithCustomer(customer)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", returnResponse.ResponseCode);

            Transaction transaction = Transaction.FromClientTransactionId(returnResponse.ReferenceNumber, TransactionType.Refund, PaymentMethodType.Credit);
            var response = transaction.Void(null, 14.05m)
                .WithCurrency(currency)
                .WithGenerateReceipt(true)
                .Execute();
            Assert.AreEqual("voided", response.ResponseCode);
        }

        [TestMethod]
        public void CreditEditWithCreditSaleId() {
            var saleResponse = card.Charge(15.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", saleResponse.ResponseCode);

            Transaction transaction = Transaction.FromId(saleResponse.TransactionId, PaymentMethodType.Credit);
            var response = transaction.Edit()
                .WithAmount(15.09m)
                .WithCurrency(currency)
                .WithGratuity(10.90m)
                .WithInvoiceNumber("239087")
                .WithAllowDuplicates(true)
                .WithGenerateReceipt(true)
                .Execute();
            Assert.AreEqual("approved", response.ResponseCode);
        }

        [TestMethod]
        public void CreditEditWithCreditReferenceId() {
            var saleResponse = card.Charge(16.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", saleResponse.ResponseCode);

            Transaction transaction = Transaction.FromClientTransactionId(saleResponse.ReferenceNumber, PaymentMethodType.Credit);
            var response = transaction.Edit()
                .WithAmount(16.49m)
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithAllowDuplicates(true)
                .WithGenerateReceipt(true)
                .Execute();
            Assert.AreEqual("approved", response.ResponseCode);
        }

        [TestMethod]
        public void CreditRefundForCARegion() {
            var response = card.Refund(17.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithAddress(address, AddressType.Billing)
                .WithCustomer(customer)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", response.ResponseCode);
        }

        [TestMethod]
        public void TestCACreditRefundWithCreditSaleId() {
            var saleResponse = card.Charge(18.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", saleResponse.ResponseCode);
            Transaction transaction = Transaction.FromId(saleResponse.TransactionId, PaymentMethodType.Credit);
            var response = transaction.Refund(18.00m)
                          .WithCurrency(currency)
                          .WithAmount(14.55m)
                          .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("approved", response.ResponseCode);
        }

        [TestMethod]
        public void TestCACreditRefundWithReferenceId() {
            var saleResponse = card.Charge(19.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", saleResponse.ResponseCode);

            Transaction transaction = Transaction.FromClientTransactionId(saleResponse.ReferenceNumber, PaymentMethodType.Credit);

            var response = transaction.Refund(09.00m)
                        .WithCurrency(currency)
                        .WithAmount(09.00m)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("approved", response.ResponseCode);
        }

        [TestMethod]
        public void CreditSaleByCreditSaleId() {
            var saleResponse = card.Charge(20.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", saleResponse.ResponseCode);

            Transaction transaction = Transaction.FromId(saleResponse.TransactionId, TransactionType.Sale, PaymentMethodType.Credit);

            var response = transaction.Fetch().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("approved", response.ResponseCode);
        }

        [TestMethod]
        public void CreditSaleWithCreditReferenceId() {
            var saleResponse = card.Charge(21.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", saleResponse.ResponseCode);

            Transaction transaction = Transaction.FromClientTransactionId(
                    saleResponse.ReferenceNumber, TransactionType.Sale, PaymentMethodType.Credit);

            var response = transaction.Fetch().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("approved", response.ResponseCode);
        }

        [TestMethod]
        public void CreditReturnWithTransactionId() {
            var returnResponse = card.Refund(22.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithAddress(address, AddressType.Billing)
                .WithCustomer(customer)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", returnResponse.ResponseCode);

            Transaction transaction = Transaction.FromId(returnResponse.TransactionId, TransactionType.Refund, PaymentMethodType.Credit);

            var response = transaction.Fetch().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("approved", response.ResponseCode);
        }

        [TestMethod]
        public void CreditReturnWithCreditReferenceId() {
            var returnResponse = card.Refund(23.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithAddress(address, AddressType.Billing)
                .WithCustomer(customer)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", returnResponse.ResponseCode);

            Transaction transaction = Transaction.FromClientTransactionId(
                                    returnResponse.ReferenceNumber, TransactionType.Refund, PaymentMethodType.Credit);

            var response = transaction.Fetch().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("approved", response.ResponseCode);
        }

        [TestMethod]
        public void CreditAuthsWithToken() {
            var response = card.Authorize(00.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithRequestMultiUseToken(true)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", response.ResponseCode);
            Assert.IsNotNull(response.CardDetail.Token);
        }

        [TestMethod]
        public void CreditAuthsWithMultiToken() {
            var saleResponse = card.Charge(28.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithRequestMultiUseToken(true)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", saleResponse.ResponseCode);
            Assert.IsNotNull(saleResponse.CardDetail.Token);
            card.Token = saleResponse.CardDetail.Token;

            var authResponse = card.Authorize(00.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", authResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditSaleWithMultiToken() {
            var response = card.Authorize(00.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithRequestMultiUseToken(true)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", response.ResponseCode);
            Assert.IsNotNull(response.CardDetail.Token);
            card.Token = response.CardDetail.Token;
            
            var saleResponse = card.Charge(26.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", saleResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditReturnWithMultiToken() {
            var response = card.Authorize(00.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithRequestMultiUseToken(true)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", response.ResponseCode);
            Assert.IsNotNull(response.CardDetail.Token);
            card.Token = response.CardDetail.Token;

            var refundResponse = card.Refund(27.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithAddress(address, AddressType.Billing)
                .WithCustomer(customer)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", refundResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditVerifyWithMultiToken() {
            var response = card.Authorize(00.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithRequestMultiUseToken(true)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", response.ResponseCode);
            Assert.IsNotNull(response.CardDetail.Token);
            card.Token = response.CardDetail.Token;

            var verifyResponse = card.Verify()
                .WithAmount(00.00m)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", verifyResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditSalesForPartialAproval() {
            var response = card.Charge(13.17m)
                .WithClerkId("Al092-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239092")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("partially_approved", response.ResponseCode);
        }

        [TestMethod]
        public void CreditVoidForPartialAprovalWithRefNumber() {
            var response = card.Charge(13.17m)
                .WithClerkId("Al092-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239092")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("partially_approved", response.ResponseCode);

            Transaction transaction = Transaction.FromClientTransactionId(response.ReferenceNumber, TransactionType.Refund, PaymentMethodType.Credit);
            var voidResponse = transaction.Void(null, Convert.ToDecimal(response.PaymentDetail.Amount))
                .WithCurrency(currency)
                .WithGenerateReceipt(true)
                .Execute();
            Assert.AreEqual("voided", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditVoidForPartialApprovalWithCreditSaleId() {
            var saleResponse = card.Charge(13.17m)
                .WithClerkId("Al094-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239094")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("partially_approved", saleResponse.ResponseCode);

            Transaction transaction = Transaction.FromId(saleResponse.TransactionId, TransactionType.Sale, PaymentMethodType.Credit);
            var response = transaction.Void(null, Convert.ToDecimal(saleResponse.PaymentDetail.Amount))
                .WithCurrency(currency)
                .WithGenerateReceipt(true)
                .Execute();
            Assert.AreEqual("voided", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void CreditAuthWithSingleUseToken() {
            card.Token = "657c5588-533c-486b-a49e-da0dbdfbdbaa";
            var response = card.Authorize(00.00m)
                .WithPaymentMethodUsageMode(PaymentMethodUsageMode.Single)
                .WithClerkId("Al090-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239087")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithRequestMultiUseToken(true)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", response.ResponseCode);
        }

    }
}