using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi
{
    [TestClass]
    public class GpApiBNPLTest : BaseGpApiTests
    {
        private BNPL paymentMethod;
        private string currency;
        private Address shippingAddress;
        private Address billingAddress;

        [TestInitialize]
        public void TestInitialize() {
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);

            paymentMethod = new BNPL {
                BNPLType = BNPLType.AFFIRM,
                ReturnUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net",
                StatusUpdateUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net",
                CancelUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net",
            };

            currency = "USD";

            // billing address
            billingAddress = new Address();
            billingAddress.StreetAddress1 = "10 Glenlake Pkwy NE";
            billingAddress.StreetAddress2 = "no";
            billingAddress.City = "Birmingham";
            billingAddress.PostalCode = "50001";
            billingAddress.CountryCode = "US";
            billingAddress.State = "IL";

            // shipping address
            shippingAddress = new Address();
            shippingAddress.StreetAddress1 = "Apartment 852";
            shippingAddress.StreetAddress2 = "Complex 741";
            shippingAddress.StreetAddress3 = "no";
            shippingAddress.City = "Birmingham";
            shippingAddress.PostalCode = "50001";
            shippingAddress.State = "IL";
            shippingAddress.CountryCode = "US";
        }

        [TestMethod]
        public void BNPL_FullCycle() {
            var customer = GenerateCustomerData();
            var products = GenerateProducts();

            var response = paymentMethod.Authorize(10)
                .WithCurrency(currency)
                .WithMiscProductData(products)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithPhoneNumber("1", "7708298000", PhoneNumberType.Shipping)
                .WithCustomerData(customer)
                .WithBNPLShippingMethod(BNPLShippingMethod.DELIVERY)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(TransactionStatus.Initiated.ToString().ToUpper(), response.ResponseMessage);

            Debug.Write(response.BNPLResponse.RedirectUrl);

            Thread.Sleep(6000);
            
            var trnInfo = ReportingService.TransactionDetail(response.TransactionId)
                .Execute();

            Assert.IsNotNull(trnInfo.TransactionStatus);

            if (trnInfo.TransactionStatus.Equals("PREAUTHORIZED") ) {
                var captured = response.Capture()
                    .Execute();

                Assert.IsNotNull(captured);
                Assert.AreEqual("SUCCESS", captured.ResponseCode);
                Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), captured.ResponseMessage);

                var refund = captured.Refund(5)
                    .WithCurrency(currency)
                    .Execute();

                Assert.IsNotNull(refund);
                Assert.AreEqual("SUCCESS", refund.ResponseCode);
                Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), refund.ResponseMessage);
            }
            else {
                Assert.AreEqual("INITIATED", trnInfo.TransactionStatus);
            }
        }
        
        [TestMethod]
        public void BNPL_WithIdempotency() {
            var idempotencyKey = Guid.NewGuid().ToString();
            var customer = GenerateCustomerData();
            var products = GenerateProducts();

            paymentMethod.Authorize(10)
                .WithCurrency(currency)
                .WithMiscProductData(products)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithCustomerData(customer)
                .WithIdempotencyKey(idempotencyKey)
                .Execute();

            var exceptionCaught = false;
            try {
                paymentMethod.Authorize(10)
                    .WithCurrency(currency)
                    .WithMiscProductData(products)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithCustomerData(customer)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.IsTrue(ex.Message.StartsWith("Status Code: Conflict - Idempotency Key seen before: id=" ));
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void FullRefund() {
            var response = ReportingService.FindTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Ascending)
                .Where(SearchCriteria.PaymentMethodName, PaymentMethodName.BNPL)
                .And(SearchCriteria.TransactionStatus, TransactionStatus.Captured)
                .And(SearchCriteria.PaymentType, PaymentType.Sale)
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Results.Count > 0);

            var trnSummary = response.Results[new Random().Next(0, response.Results.Count)];
            var trn = Transaction.FromId(trnSummary.TransactionId, trnSummary.PaymentType);

            var trnRefund = trn.Refund()
                .WithCurrency(trnSummary.Currency)
                .Execute();

            Assert.IsNotNull(trnRefund);
            Assert.AreEqual("SUCCESS", trnRefund.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trnRefund.ResponseMessage);
        }

        [TestMethod]
        public void BNPL_PartialRefund() {
            var customer = GenerateCustomerData();
            var products = GenerateProducts();

            var transaction = paymentMethod.Authorize(550)
                .WithCurrency(currency)
                .WithMiscProductData(products)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithPhoneNumber("41", "57774873", PhoneNumberType.Shipping)
                .WithCustomerData(customer)
                .WithBNPLShippingMethod(BNPLShippingMethod.DELIVERY)
                .WithOrderId("12365")
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual("SUCCESS", transaction.ResponseCode);
            Assert.AreEqual(TransactionStatus.Initiated.ToString().ToUpper(), transaction.ResponseMessage);
            Assert.IsNotNull(transaction.BNPLResponse.RedirectUrl);

            Debug.Write(transaction.BNPLResponse.RedirectUrl);

            Thread.Sleep(6000);

            var trnInfo = ReportingService.TransactionDetail(transaction.TransactionId)
                .Execute();

            Assert.IsNotNull(trnInfo.TransactionStatus);

            if (trnInfo.TransactionStatus.Equals("PREAUTHORIZED")) {
                var captureTrn = transaction.Capture()
                    .Execute();

                Assert.IsNotNull(captureTrn);
                Assert.AreEqual("SUCCESS", captureTrn.ResponseCode);
                Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), captureTrn.ResponseMessage);

                Thread.Sleep(6000);

                var trnRefund = captureTrn.Refund(100)
                    .WithCurrency(currency)
                    .Execute();

                Assert.IsNotNull(trnRefund);
                Assert.AreEqual("SUCCESS", trnRefund.ResponseCode);
                Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trnRefund.ResponseMessage);
            }
            else {
                Assert.AreEqual("INITIATED", trnInfo.TransactionStatus);
            }
        }

        [TestMethod]
        public void BNPL_MultipleRefund() {
            var customer = GenerateCustomerData();
            var products = GenerateProducts();

            var transaction = paymentMethod.Authorize(550)
                .WithCurrency(currency)
                .WithMiscProductData(products)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithPhoneNumber("41", "57774873", PhoneNumberType.Shipping)
                .WithCustomerData(customer)
                .WithBNPLShippingMethod(BNPLShippingMethod.DELIVERY)
                .WithOrderId("12365")
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual("SUCCESS", transaction.ResponseCode);
            Assert.AreEqual(TransactionStatus.Initiated.ToString().ToUpper(), transaction.ResponseMessage);
            Assert.IsNotNull(transaction.BNPLResponse.RedirectUrl);

            Debug.Write(transaction.BNPLResponse.RedirectUrl);
            Thread.Sleep(6000);

            var trnInfo = ReportingService.TransactionDetail(transaction.TransactionId)
                .Execute();

            Assert.IsNotNull(trnInfo.TransactionStatus);

            if (trnInfo.TransactionStatus.Equals("PREAUTHORIZED")) {
                var captureTrn = transaction.Capture()
                    .Execute();

                Assert.IsNotNull(captureTrn);
                Assert.AreEqual("SUCCESS", captureTrn.ResponseCode);
                Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), captureTrn.ResponseMessage);

                Thread.Sleep(6000);

                var trnRefund = captureTrn.Refund(100)
                    .WithCurrency(currency)
                    .Execute();

                Assert.IsNotNull(trnRefund);
                Assert.AreEqual("SUCCESS", trnRefund.ResponseCode);
                Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trnRefund.ResponseMessage);

                trnRefund = captureTrn.Refund(100)
                    .WithCurrency(currency)
                    .Execute();

                Assert.IsNotNull(trnRefund);
                Assert.AreEqual("SUCCESS", trnRefund.ResponseCode);
                Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trnRefund.ResponseMessage);
            }
            else {
                Assert.AreEqual("INITIATED", trnInfo.TransactionStatus);
            }
        }

        [TestMethod]
        public void BNPL_Reverse() {
            var customer = GenerateCustomerData();
            var products = GenerateProducts();

            var transaction = paymentMethod.Authorize(550)
                .WithCurrency(currency)
                .WithMiscProductData(products)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithPhoneNumber("41", "57774873", PhoneNumberType.Shipping)
                .WithCustomerData(customer)
                .WithBNPLShippingMethod(BNPLShippingMethod.DELIVERY)
                .WithOrderId("12365")
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual("SUCCESS", transaction.ResponseCode);
            Assert.AreEqual(TransactionStatus.Initiated.ToString().ToUpper(), transaction.ResponseMessage);
            Assert.IsNotNull(transaction.BNPLResponse.RedirectUrl);

            Debug.Write(transaction.BNPLResponse.RedirectUrl);

            Thread.Sleep(6000);

            var trnInfo = ReportingService.TransactionDetail(transaction.TransactionId)
                .Execute();

            Assert.IsNotNull(trnInfo.TransactionStatus);

            if (trnInfo.TransactionStatus.Equals("PREAUTHORIZED")) {
                var captureTrn = transaction.Reverse()
                    .Execute();

                Assert.IsNotNull(captureTrn);
                Assert.AreEqual("SUCCESS", captureTrn.ResponseCode);
                Assert.AreEqual(TransactionStatus.Reversed.ToString().ToUpper(), captureTrn.ResponseMessage);
            }
            else {
                Assert.AreEqual("INITIATED", trnInfo.TransactionStatus);
            }
        }

        [TestMethod]
        public void BNPL_OnlyMandatory() {
            var customer = GenerateCustomerData();
            var products = GenerateProducts();

            var transaction = paymentMethod.Authorize(550)
                .WithCurrency(currency)
                .WithMiscProductData(products)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithCustomerData(customer)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual("SUCCESS", transaction.ResponseCode);
            Assert.AreEqual(TransactionStatus.Initiated.ToString().ToUpper(), transaction.ResponseMessage);
            Assert.IsNotNull(transaction.BNPLResponse.RedirectUrl);
        }

        [TestMethod]
        public void BNPL_KlarnaProvider() {
            paymentMethod.BNPLType = BNPLType.KLARNA;
            var customer = GenerateCustomerData();
            var products = GenerateProducts();

            var transaction = paymentMethod.Authorize(550)
                .WithCurrency(currency)
                .WithMiscProductData(products)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithCustomerData(customer)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual("SUCCESS", transaction.ResponseCode);
            Assert.AreEqual(TransactionStatus.Initiated.ToString().ToUpper(), transaction.ResponseMessage.ToUpper());
            Assert.IsNotNull(transaction.BNPLResponse.RedirectUrl);

            Debug.Write(transaction.BNPLResponse.RedirectUrl);

            Thread.Sleep(6000);

            var trnInfo = ReportingService.TransactionDetail(transaction.TransactionId)
                .Execute();

            Assert.IsNotNull(trnInfo.TransactionStatus);

            if (trnInfo.TransactionStatus.Equals("PREAUTHORIZED")) {
                var captureTrn = transaction.Capture()
                    .Execute();

                Assert.IsNotNull(captureTrn);
                Assert.AreEqual("SUCCESS", captureTrn.ResponseCode);
                Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), captureTrn.ResponseMessage.ToUpper());
            }
            else {
                Assert.AreEqual("INITIATED", trnInfo.TransactionStatus);
            }
        }

        [TestMethod]
        public void BNPL_ClearPayProvider() {
            paymentMethod.BNPLType = BNPLType.CLEARPAY;
            var customer = GenerateCustomerData();
            var products = GenerateProducts();

            var transaction = paymentMethod.Authorize(550)
                .WithCurrency(currency)
                .WithMiscProductData(products)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithCustomerData(customer)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual("SUCCESS", transaction.ResponseCode);
            Assert.AreEqual(TransactionStatus.Initiated.ToString().ToUpper(), transaction.ResponseMessage);
            Assert.IsNotNull(transaction.BNPLResponse.RedirectUrl);
        }

        [TestMethod]
        public void BNPL_ClearPayProvider_PartialCapture() {
            paymentMethod.BNPLType = BNPLType.CLEARPAY;
            var customer = GenerateCustomerData();
            var products = GenerateProducts();

            var transaction = paymentMethod.Authorize(550)
                .WithCurrency(currency)
                .WithMiscProductData(products)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithCustomerData(customer)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual("SUCCESS", transaction.ResponseCode);
            Assert.AreEqual(TransactionStatus.Initiated.ToString().ToUpper(), transaction.ResponseMessage);
            Assert.IsNotNull(transaction.BNPLResponse.RedirectUrl);

            Debug.Write(transaction.BNPLResponse.RedirectUrl);

            Thread.Sleep(6000);

            var trnInfo = ReportingService.TransactionDetail(transaction.TransactionId)
                .Execute();

            Assert.IsNotNull(trnInfo.TransactionStatus);

            if (trnInfo.TransactionStatus.Equals("PREAUTHORIZED")) {
                var captureTrn = transaction.Capture(100)
                    .Execute();

                Assert.IsNotNull(captureTrn);
                Assert.AreEqual("SUCCESS", captureTrn.ResponseCode);
                Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), captureTrn.ResponseMessage);
            }
            else {
                Assert.AreEqual("INITIATED", trnInfo.TransactionStatus);
            }
        }

        [TestMethod]
        public void BNPL_ClearPayProvider_MultipleCapture() {
            paymentMethod.BNPLType = BNPLType.CLEARPAY;
            var customer = GenerateCustomerData();
            var products = GenerateProducts();

            var transaction = paymentMethod.Authorize(550)
                .WithCurrency(currency)
                .WithMiscProductData(products)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithCustomerData(customer)
                .WithMultiCapture(true)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual("SUCCESS", transaction.ResponseCode);
            Assert.AreEqual(TransactionStatus.Initiated.ToString().ToUpper(), transaction.ResponseMessage);
            Assert.IsNotNull(transaction.BNPLResponse.RedirectUrl);

            Debug.Write(transaction.BNPLResponse.RedirectUrl);

            Thread.Sleep(6000);

            var trnInfo = ReportingService.TransactionDetail(transaction.TransactionId)
                .Execute();

            Assert.IsNotNull(trnInfo.TransactionStatus);

            if (trnInfo.TransactionStatus.Equals("PREAUTHORIZED")) {
                var captureTrn = transaction.Capture(100)
                    .Execute();

                Assert.IsNotNull(captureTrn);
                Assert.AreEqual("SUCCESS", captureTrn.ResponseCode);
                Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), captureTrn.ResponseMessage);

                Thread.Sleep(6000);

                captureTrn = transaction.Capture(100)
                    .Execute();

                Assert.IsNotNull(captureTrn);
                Assert.AreEqual("SUCCESS", captureTrn.ResponseCode);
                Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), captureTrn.ResponseMessage);
            }
            else {
                Assert.AreEqual("INITIATED", trnInfo.TransactionStatus);
            }
        }

        [TestMethod]
        public void BNPL_InvalidStatusForCapture_NoRedirect() {
            var customer = GenerateCustomerData();
            var products = GenerateProducts();

            var transaction = paymentMethod.Authorize(550)
                .WithCurrency(currency)
                .WithMiscProductData(products)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithPhoneNumber("41", "57774873", PhoneNumberType.Shipping)
                .WithCustomerData(customer)
                .WithBNPLShippingMethod(BNPLShippingMethod.DELIVERY)
                .WithOrderId("12365")
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual("SUCCESS", transaction.ResponseCode);
            Assert.AreEqual(TransactionStatus.Initiated.ToString().ToUpper(), transaction.ResponseMessage);
            Assert.IsNotNull(transaction.BNPLResponse.RedirectUrl);

            var exceptionCaught = false;
            try {
                transaction.Capture()
                    .Execute();
            } catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("40090", e.ResponseMessage);
                Assert.AreEqual(
                    "Status Code: BadRequest - id value is invalid. Please check the format and data provided is correct.",
                    e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void GetBNPLTransactionById() {
            const string id = "TRN_o7PsaRAgOviqLCPHBaxDcqYO70oUhu";

            var trnInfo = ReportingService.TransactionDetail(id)
                .Execute();

            Assert.AreEqual(id, trnInfo.TransactionId);
        }

        [TestMethod]
        public void GetBNPLTransactionById_RandomTransactionId() {
            var id = Guid.NewGuid().ToString();

            var errorFound = false;
            try {
                ReportingService.TransactionDetail(id)
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.IsTrue(ex.Message.Contains(
                    $"Status Code: NotFound - Transactions {id} not found at this /ucp/transactions/{id}"));
                Assert.AreEqual("40118", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void GetBNPLTransactionById_NullTransactionId() {
            var errorFound = false;
            try {
                ReportingService.TransactionDetail(null)
                    .Execute();
            } catch (BuilderException ex) {
                errorFound = true;
                Assert.AreEqual("TransactionId cannot be null for this transaction type.", ex.Message);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void BNPL_MissingProducts() {
            var customer = GenerateCustomerData();

            var errorFound = false;
            try {
                paymentMethod.Authorize(10)
                    .WithCurrency(currency)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithPhoneNumber("1", "7708298000", PhoneNumberType.Shipping)
                    .WithCustomerData(customer)
                    .WithBNPLShippingMethod(BNPLShippingMethod.DELIVERY)
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields: order.items.",
                    ex.Message);
                Assert.AreEqual("40251", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void BNPL_MissingShippingAddress() {
            var customer = GenerateCustomerData();
            var products = GenerateProducts();

            var errorFound = false;
            try {
                paymentMethod.Authorize(10)
                    .WithCurrency(currency)
                    .WithMiscProductData(products)
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithPhoneNumber("1", "7708298000", PhoneNumberType.Shipping)
                    .WithCustomerData(customer)
                    .WithBNPLShippingMethod(BNPLShippingMethod.DELIVERY)
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadGateway - Processor System error", ex.Message);
                Assert.AreEqual("50143", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void BNPL_MissingBillingAddress() {
            var customer = GenerateCustomerData();
            var products = GenerateProducts();

            var errorFound = false;
            try {
                paymentMethod.Authorize(10)
                    .WithCurrency(currency)
                    .WithMiscProductData(products)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithPhoneNumber("1", "7708298000", PhoneNumberType.Shipping)
                    .WithCustomerData(customer)
                    .WithBNPLShippingMethod(BNPLShippingMethod.DELIVERY)
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadRequest - One of the parameter is missing from the request body.",
                    ex.Message);
                Assert.AreEqual("40297", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void BNPL_MissingCustomerData() {
            var products = GenerateProducts();

            var errorFound = false;
            try {
                paymentMethod.Authorize(10)
                    .WithCurrency(currency)
                    .WithMiscProductData(products)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithPhoneNumber("1", "7708298000", PhoneNumberType.Shipping)
                    .WithBNPLShippingMethod(BNPLShippingMethod.DELIVERY)
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields: payer.email.",
                    ex.Message);
                Assert.AreEqual("40251", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void BNPL_MissingAmount() {
            var customer = GenerateCustomerData();
            var products = GenerateProducts();

            var errorFound = false;
            try {
                paymentMethod.Authorize()
                    .WithCurrency(currency)
                    .WithMiscProductData(products)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithPhoneNumber("1", "7708298000", PhoneNumberType.Shipping)
                    .WithCustomerData(customer)
                    .WithBNPLShippingMethod(BNPLShippingMethod.DELIVERY)
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields amount", ex.Message);
                Assert.AreEqual("40005", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void BNPL_MissingCurrency() {
            var customer = GenerateCustomerData();
            var products = GenerateProducts();

            var errorFound = false;
            try {
                paymentMethod.Authorize(10)
                    .WithMiscProductData(products)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithPhoneNumber("1", "7708298000", PhoneNumberType.Shipping)
                    .WithCustomerData(customer)
                    .WithBNPLShippingMethod(BNPLShippingMethod.DELIVERY)
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields currency", ex.Message);
                Assert.AreEqual("40005", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void BNPL_MissingCustomerEmail() {
            var customer = GenerateCustomerData();
            customer.Email = null;
            var products = GenerateProducts();

            var errorFound = false;
            try {
                paymentMethod.Authorize(10)
                    .WithCurrency(currency)
                    .WithMiscProductData(products)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithPhoneNumber("1", "7708298000", PhoneNumberType.Shipping)
                    .WithCustomerData(customer)
                    .WithBNPLShippingMethod(BNPLShippingMethod.DELIVERY)
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields: payer.email.",
                    ex.Message);
                Assert.AreEqual("40251", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void BNPL_MissingCustomerPhoneNumber() {
            var customer = GenerateCustomerData();
            customer.Phone = null;
            var products = GenerateProducts();

            var errorFound = false;
            try {
                paymentMethod.Authorize(10)
                    .WithCurrency(currency)
                    .WithMiscProductData(products)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithPhoneNumber("1", "7708298000", PhoneNumberType.Shipping)
                    .WithCustomerData(customer)
                    .WithBNPLShippingMethod(BNPLShippingMethod.DELIVERY)
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual(
                    "Status Code: BadRequest - Request expects the following fields: payer.contact_phone.country_code.",
                    ex.Message);
                Assert.AreEqual("40251", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void BNPL_MissingProductId() {
            var customer = GenerateCustomerData();
            var products = GenerateProducts();
            products[0].ProductId = null;

            var errorFound = false;
            try {
                paymentMethod.Authorize(10)
                    .WithCurrency(currency)
                    .WithMiscProductData(products)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithPhoneNumber("1", "7708298000", PhoneNumberType.Shipping)
                    .WithCustomerData(customer)
                    .WithBNPLShippingMethod(BNPLShippingMethod.DELIVERY)
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual(
                    "Status Code: BadRequest - Request expects the following fields: order.items[0].reference.",
                    ex.Message);
                Assert.AreEqual("40251", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void BNPL_MissingProductDescription() {
            var customer = GenerateCustomerData();
            var products = GenerateProducts();
            products[0].Description = null;

            var errorFound = false;
            try {
                paymentMethod.Authorize(10)
                    .WithCurrency(currency)
                    .WithMiscProductData(products)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithPhoneNumber("1", "7708298000", PhoneNumberType.Shipping)
                    .WithCustomerData(customer)
                    .WithBNPLShippingMethod(BNPLShippingMethod.DELIVERY)
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual(
                    "Status Code: BadRequest - Request expects the following fields: order.items[0].description.",
                    ex.Message);
                Assert.AreEqual("40251", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void BNPL_MissingProductQuantity() {
            var customer = GenerateCustomerData();
            var products = GenerateProducts();
            products[0].Quantity = null;

            var errorFound = false;
            try {
                paymentMethod.Authorize(10)
                    .WithCurrency(currency)
                    .WithMiscProductData(products)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithPhoneNumber("1", "7708298000", PhoneNumberType.Shipping)
                    .WithCustomerData(customer)
                    .WithBNPLShippingMethod(BNPLShippingMethod.DELIVERY)
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual(
                    "Status Code: BadRequest - Request expects the following fields: order.items[0].quantity.",
                    ex.Message);
                Assert.AreEqual("40251", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void BNPL_MissingProductUrl() {
            var customer = GenerateCustomerData();
            var products = GenerateProducts();
            products[0].Url = null;

            var errorFound = false;
            try {
                paymentMethod.Authorize(10)
                    .WithCurrency(currency)
                    .WithMiscProductData(products)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithPhoneNumber("1", "7708298000", PhoneNumberType.Shipping)
                    .WithCustomerData(customer)
                    .WithBNPLShippingMethod(BNPLShippingMethod.DELIVERY)
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual(
                    "Status Code: BadRequest - Request expects the following fields: order.items[0].product_url.",
                    ex.Message);
                Assert.AreEqual("40251", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void BNPL_MissingProductImageUrl() {
            var customer = GenerateCustomerData();
            var products = GenerateProducts();
            products[0].ImageUrl = null;

            var errorFound = false;
            try {
                paymentMethod.Authorize(10)
                    .WithCurrency(currency)
                    .WithMiscProductData(products)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithPhoneNumber("1", "7708298000", PhoneNumberType.Shipping)
                    .WithCustomerData(customer)
                    .WithBNPLShippingMethod(BNPLShippingMethod.DELIVERY)
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual(
                    "Status Code: BadRequest - Request expects the following fields: order.items[0].product_image_url.",
                    ex.Message);
                Assert.AreEqual("40251", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        private Customer GenerateCustomerData() {
            return new Customer() {
                Id = "12345678", // GenerationUtils.GenerateOrderId();
                FirstName = "James",
                LastName = "Mason",
                Email = "james.mason@example.com",
                Phone = new PhoneNumber {
                    CountryCode = "1", Number = "7708298000", AreaCode = PhoneNumberType.Home.ToString()
                },
                Documents = new List<CustomerDocument>() {
                    new CustomerDocument
                        { Reference = "123456789", Issuer = "US", Type = CustomerDocumentType.PASSPORT }
                }
            };
        }

        private List<Product> GenerateProducts() {
            List<Product> products = new List<Product>();
            var product = new Product {
                ProductId = "92ebf294-f3ef-4aba-af30-6ebaf747de8f",
                ProductName = "iPhone 13",
                Description = "iPhone 13",
                Quantity = 1,
                UnitPrice = 550,
                TaxAmount = 1,
                DiscountAmount = 0,
                TaxPercentage = 0,
                NetUnitAmount = 550,
                GiftCardCurrency = currency,
                Url = "https://www.example.com/iphone.html",
                ImageUrl = "https://www.example.com/iphone.png"
            };

            products.Add(product);

            return products;
        }
    }
}