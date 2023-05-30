using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.TransactionApi.Request;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.TransactionApi {
    [TestClass]
    public class TransactionApiCreditTests {

        private CreditCardData card = new CreditCardData();
        private const string currency = "840";
        private Customer customer;
        private Address address;
        private TransactionData transData;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.ConfigureService(new TransactionApiConfig {
                RequestLogger = new RequestConsoleLogger(),
                EnableLogging = true,
                Region = "US",
                AccountCredential = "800000052925:80039923:eWcWNJhfxiJ7QyEHSHndWk4VHKbSmSue",
                AppSecret = "lucQKkwz3W3RGzABkSWUVZj1Mb0Yx3E9chAA8ESUVAv"
            });
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
                CountryCode = CountryCode.Country_840,
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
                .WithClerkId("Al091-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239091")
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
            var response = card.Charge(10.00m)
                .WithClerkId("Al092-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239092")
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
                .WithClerkId("Al093-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239093")
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
                .WithClerkId("Al094-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239094")
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
            var saleResponse = card.Charge(11.00m)
                .WithClerkId("Al095-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239095")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", saleResponse.ResponseCode);

            Transaction transaction = Transaction.FromClientTransactionId(saleResponse.ReferenceNumber, TransactionType.Sale, PaymentMethodType.Credit);
            var response = transaction.Void(null, 11.00m)
                .WithCurrency(currency)
                .WithGenerateReceipt(true)
                .Execute();
            Assert.AreEqual("voided", response.ResponseCode);
        }

        [TestMethod]
        public void CreditVoidWithCreditReturnId() {
            var returnResponse = card.Refund(14.00m)
                .WithClerkId("Al096-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239096")
                .WithCustomer(customer)
                .WithAddress(address, AddressType.Billing)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", returnResponse.ResponseCode);

            Transaction transaction = Transaction.FromId(returnResponse.TransactionId, TransactionType.Refund, PaymentMethodType.Credit);
            var response = transaction.Void(null, 14.00m)
                .WithCurrency(currency)
                .WithGenerateReceipt(true)
                .Execute();
            Assert.AreEqual("voided", response.ResponseCode);
        }

        [TestMethod]
        public void CreditVoidWithCreditReturnReferenceId() {
            var returnResponse = card.Refund(15.00m)
                .WithClerkId("Al097-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239097")
                .WithCustomer(customer)
                .WithAddress(address, AddressType.Billing)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", returnResponse.ResponseCode);

            Transaction transaction = Transaction.FromClientTransactionId(returnResponse.ReferenceNumber, TransactionType.Refund, PaymentMethodType.Credit);
            var response = transaction.Void(null, 15.00m)
                .WithCurrency(currency)
                .WithGenerateReceipt(true)
                .Execute();
            Assert.AreEqual("voided", response.ResponseCode);
        }

        [TestMethod]
        public void CreditEditWithCreditSaleId() {
            var saleResponse = card.Charge(16.00m)
                .WithClerkId("Al098-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239098")
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
                .WithAmount(15.59m)
                .WithCurrency(currency)
                .WithGratuity(10.00m)
                .WithInvoiceNumber("2390187")
                .WithAllowDuplicates(true)
                .WithGenerateReceipt(true)
                .Execute();
            Assert.AreEqual("approved", response.ResponseCode);
        }

        [TestMethod]
        public void CreditEditWithCreditReferenceId() {
            var saleResponse = card.Charge(17.00m)
                .WithClerkId("Al099-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239099")
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
                .WithAmount(16.09m)
                .WithCurrency(currency)
                .WithInvoiceNumber("2390187")
                .WithAllowDuplicates(true)
                .WithGenerateReceipt(true)
                .Execute();
            Assert.AreEqual("approved", response.ResponseCode);
        }

        [TestMethod]
        public void CreditRefundForUSRegion() {
            var response = card.Refund(18.00m)
                .WithClerkId("Al085-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239085")
                .WithCustomer(customer)
                .WithAddress(address, AddressType.Billing)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", response.ResponseCode);
        }

        [TestMethod]
        public void TestUSCreditRefundWithCreditSaleId() {
            var saleResponse = card.Charge(19.00m)
                .WithClerkId("Al086-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239086")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", saleResponse.ResponseCode);

            Transaction transaction = Transaction.FromId(saleResponse.TransactionId, PaymentMethodType.Credit);
            var response = transaction.Refund(19.00m)
                          .WithCurrency(currency)
                          .WithAmount(10.00m)
                          .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("approved", response.ResponseCode);
        }

        [TestMethod]
        public void TestUSCreditRefundWithReferenceId() {
            var saleResponse = card.Charge(21.01m)
                .WithClerkId("Al080-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239080")
                .WithCustomer(customer)
                .WithShippingDate(System.DateTime.Now)
                .WithAddress(address, AddressType.Billing)
                .WithAddress(address, AddressType.Shipping)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", saleResponse.ResponseCode);

            Transaction transaction = Transaction.FromClientTransactionId(saleResponse.ReferenceNumber, PaymentMethodType.Credit);
            var response = transaction.Refund(11.01m)
                        .WithCurrency(currency)
                        .WithAmount(11.01m)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("approved", response.ResponseCode);
        }

        [TestMethod]
        public void CreditSaleByCreditSaleId() {
            var saleResponse = card.Charge(11.57m)
                .WithClerkId("Al081-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239081")
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
            var saleResponse = card.Charge(12.40m)
                .WithClerkId("Al082-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("339082")
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
        public void CreditReturnWithCreditReferenceId() {
            var returnResponse = card.Refund(13.60m)
                .WithClerkId("Al083-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239083")
                .WithCustomer(customer)
                .WithAddress(address, AddressType.Billing)
                .WithTransactionData(transData)
                .WithClientTransactionId("Ref" + DateTime.Now.ToString("yyyyMMddHHmmss"))
                .Execute();
            Assert.AreEqual("approved", returnResponse.ResponseCode);

            Transaction transaction =  Transaction.FromClientTransactionId(
                                    returnResponse.ReferenceNumber, TransactionType.Refund, PaymentMethodType.Credit);

            var response = transaction.Fetch().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("approved", response.ResponseCode);
        }

        [TestMethod]
        public void CreditReturnWithTransactionId() {
            var returnResponse = card.Refund(14.70m)
                .WithClerkId("Al084-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239084")
                .WithCustomer(customer)
                .WithAddress(address, AddressType.Billing)
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

            var saleResponse = card.Charge(26.55m)
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

            var refundResponse = card.Refund(29.00m)
                .WithClerkId("Al085-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239085")
                .WithCustomer(customer)
                .WithAddress(address, AddressType.Billing)
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
                .WithClerkId("Al093-John Doe")
                .WithCurrency(currency)
                .WithInvoiceNumber("239093")
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
            card.Token = "4713280c-f1f5-4d2b-886a-f19e6eb420c3";
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