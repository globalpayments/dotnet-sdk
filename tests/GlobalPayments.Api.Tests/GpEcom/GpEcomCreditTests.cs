using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests {
    [TestClass]
    public class GpEcomCreditTests {
        CreditCardData card;

        [TestInitialize]
        public void Init() {
            var config = new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RebatePassword = "rebate",
                RefundPassword = "refund"
            };
            ServicesContainer.ConfigureService(config);

            config.AccountId = "apidcc";
            ServicesContainer.ConfigureService(config, "dcc");

            card = new CreditCardData {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123",
                CardHolderName = "Joe Smith"
            };
        }

        [TestMethod]
        public void CreditAuthorization() {
            var authorization = card.Authorize(14m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(authorization);
            Assert.AreEqual("00", authorization.ResponseCode, authorization.ResponseMessage);

            var capture = authorization.Capture(14m)
                .Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode, capture.ResponseMessage);
        }

        [TestMethod]
        public void CreditAuthorizationForMultiCapture() {
            var authorization = card.Authorize(14m)
                .WithCurrency("USD")
                .WithMultiCapture(true)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(authorization);
            Assert.AreEqual("00", authorization.ResponseCode, authorization.ResponseMessage);

            var capture = authorization.Capture(3m)
                .Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode, capture.ResponseMessage);

            var capture2 = authorization.Capture(5m)
                .Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode, capture.ResponseMessage);

            var capture3 = authorization.Capture(7m)
                .Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode, capture.ResponseMessage);
        }

        [TestMethod]
        public void CreditSale() {
            var response = card.Charge(15m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void CreditSaleWithRecurring() {
            var response = card.Charge(15m)
                .WithCurrency("USD")
                .WithRecurringInfo(RecurringType.Fixed, RecurringSequence.First)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void CreditRefund() {
            var response = card.Refund(16m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void CreditRebate() {
            var response = card.Charge(17m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);

            var rebate = response.Refund(17m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(rebate);
            Assert.AreEqual("00", rebate.ResponseCode, rebate.ResponseMessage);
        }

        [TestMethod]
        public void CreditVoid() {
            var response = card.Charge(15m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);

            var voidResponse = response.Void().Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode, voidResponse.ResponseMessage);
        }

        [TestMethod]
        public void CreditVerify() {
            var response = card.Verify()
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void Credit_SupplimentaryData() {
            var response = card.Authorize(10m)
                .WithCurrency("GBP")
                .WithSupplementaryData("leg", "value1", "value2", "value3")
                .WithSupplementaryData("leg", "value1", "value2", "value3")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);

            var captureResponse = response.Capture(10m)
                .WithSupplementaryData("leg", "value1", "value2", "value3")
                .WithSupplementaryData("leg", "value1", "value2", "value3")
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode, captureResponse.ResponseMessage);
        }

        [TestMethod]
        public void CreditFraudResponse() {
            var billingAddress = new Address();
            billingAddress.StreetAddress1 = "Flat 123";
            billingAddress.StreetAddress2 = "House 456";
            billingAddress.StreetAddress3 = "Cul-De-Sac";
            billingAddress.City = "Halifax";
            billingAddress.Province = "West Yorkshire";
            billingAddress.State = "Yorkshire and the Humber";
            billingAddress.Country = "GB";
            billingAddress.PostalCode = "E77 4QJ";

            var shippingAddress = new Address();
            shippingAddress.StreetAddress1 = "House 456";
            shippingAddress.StreetAddress2 = "987 The Street";
            shippingAddress.StreetAddress3 = "Basement Flat";
            shippingAddress.City = "Chicago";
            shippingAddress.State = "Illinois";
            shippingAddress.Province = "Mid West";
            shippingAddress.Country = "US";
            shippingAddress.PostalCode = "50001";

            var fraudResponse = card.Charge(199.99m)
                .WithCurrency("EUR")
                .WithAddress(billingAddress, AddressType.Billing)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithProductId("SID9838383")
                .WithClientTransactionId("Car Part HV")
                .WithCustomerId("E8953893489")
                .WithCustomerIpAddress("123.123.123.123")
                .Execute();
            Assert.IsNotNull(fraudResponse);
            Assert.AreEqual("00", fraudResponse.ResponseCode, fraudResponse.ResponseMessage);
        }

        [TestMethod, Ignore]
        public void AuthMobileGooglePay() {
            var token = new CreditCardData {
                Token = "{\"version\":\"EC_v1\",\"data\":\"dvMNzlcy6WNB\",\"header\":{\"ephemeralPublicKey\":\"MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEWdNhNAHy9kO2Kol33kIh7k6wh6E\",\"transactionId\":\"fd88874954acdb299c285f95a3202ad1f330d3fd4ebc22a864398684198644c3\",\"publicKeyHash\":\"h7WnNVz2gmpTSkHqETOWsskFPLSj31e3sPTS2cBxgrk\"}}",
                MobileType = MobilePaymentMethodType.GOOGLEPAY
            };

            var response = token.Charge(15m)
                .WithCurrency("EUR")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void StoredCredential_Sale() {
            StoredCredential storedCredential = new StoredCredential {
                Type = StoredCredentialType.OneOff,
                Initiator = StoredCredentialInitiator.CardHolder,
                Sequence = StoredCredentialSequence.First
            };

            Transaction response = card.Charge(15m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithStoredCredential(storedCredential)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.SchemeId);
        }

        [TestMethod]
        public void StoredCredential_OTB() {
            StoredCredential storedCredential = new StoredCredential {
                Type = StoredCredentialType.OneOff,
                Initiator = StoredCredentialInitiator.CardHolder,
                Sequence = StoredCredentialSequence.First
            };

            Transaction response = card.Verify()
                    .WithAllowDuplicates(true)
                    .WithStoredCredential(storedCredential)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.SchemeId);
        }

        [TestMethod]
        public void StoredCredential_ReceiptIn() {
            RecurringPaymentMethod storedCard = new RecurringPaymentMethod("03e28f0e-492e-80bd-20ec318e9334", "3c4af936-483e-a393-f558bec2fb2a");

            StoredCredential storedCredential = new StoredCredential {
                Type = StoredCredentialType.Recurring,
                Initiator = StoredCredentialInitiator.Merchant,
                Sequence = StoredCredentialSequence.Subsequent
            };

            Transaction response = storedCard.Authorize(15.15m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithStoredCredential(storedCredential)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.SchemeId);
        }

        [TestMethod]
        public void StoredCredential_ReceiptIn_OTB() {
            RecurringPaymentMethod storedCard = new RecurringPaymentMethod("03e28f0e-492e-80bd-20ec318e9334", "3c4af936-483e-a393-f558bec2fb2a");

            StoredCredential storedCredential = new StoredCredential {
                Type = StoredCredentialType.Recurring,
                Initiator = StoredCredentialInitiator.Merchant,
                Sequence = StoredCredentialSequence.Subsequent
            };

            Transaction response = storedCard.Verify()
                            .WithAllowDuplicates(true)
                            .WithStoredCredential(storedCredential)
                            .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.SchemeId);
        }

        [TestMethod]
        public void FraudManagement_Decisionmanager() {
            Address billingAddress = new Address {
                StreetAddress1 = "Flat 123",
                StreetAddress2 = "House 456",
                StreetAddress3 = "Cul-De-Sac",
                City = "Halifax",
                Province = "West Yorkshire",
                State = "Yorkshire and the Humber",
                Country = "GB",
                PostalCode = "E77 4QJ"
            };

            Address shippingAddress = new Address {
                StreetAddress1 = "House 456",

                StreetAddress2 = "987 The Street",
                StreetAddress3 = "Basement Flat",
                City = "Chicago",
                State = "Illinois",
                Province = "Mid West",
                Country = "US",
                PostalCode = "50001",
            };

            Customer customer = new Customer {
                Id = "e193c21a-ce64-4820-b5b6-8f46715de931",
                FirstName = "James",
                LastName = "Mason",
                DateOfBirth = "01011980",
                CustomerPassword = "VerySecurePassword",
                Email = "text@example.com",
                DomainName = "example.com",
                HomePhone = "+35312345678",
                DeviceFingerPrint = "devicefingerprint",
            };

            DecisionManager decisionManager = new DecisionManager {
                BillToHostName = "example.com",
                BillToHttpBrowserCookiesAccepted = true,
                BillToHttpBrowserEmail = "jamesmason@example.com",
                BillToHttpBrowserType = "Mozilla",
                BillToIpNetworkAddress = "123.123.123.123",
                BusinessRulesCoreThreshold = "40",
                BillToPersonalId = "741258963",
                InvoiceHeaderTenderType = "consumer",
                InvoiceHeaderIsGift = true,
                DecisionManagerProfile = "DemoProfile",
                InvoiceHeaderReturnsAccepted = true,
                ItemHostHedge = Risk.High,
                ItemNonsensicalHedge = Risk.High,
                ItemObscenitiesHedge = Risk.High,
                ItemPhoneHedge = Risk.High,
                ItemTimeHedge = Risk.High,
                ItemVelocityHedge = Risk.High,
            };

            Transaction response = card.Charge(199.99m)
                    .WithCurrency("EUR")
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithDecisionManager(decisionManager)
                    .WithCustomerData(customer)
                    .WithMiscProductData("SKU251584", "Magazine Subscription", "12", "1200", "true", "subscription", "Low")
                    .WithMiscProductData("SKU8884784", "Charger", "10", "1200", "false", "electronic_good", "High")
                    .WithCustomData("fieldValue01", "fieldValue02", "fieldValue03", "fieldValue04")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DccRateLookup_Charge() {
            Transaction dccResponse = card.GetDccRate(DccRateType.Sale, DccProcessor.Fexco)
                .WithAmount(10.01m)
                .WithCurrency("EUR")
                .Execute("dcc");
            Assert.IsNotNull(dccResponse);
            Assert.AreEqual("00", dccResponse.ResponseCode);

            Transaction saleResponse = card.Charge(10.01m)
                    .WithCurrency("EUR")
                    .WithDccRateData(dccResponse.DccRateData)
                    .WithOrderId(dccResponse.OrderId)
                    .Execute("dcc");
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
        }

        [TestMethod]
        public void DccRateLookup_Auth() {
            Transaction dccResponse = card.GetDccRate(DccRateType.Sale, DccProcessor.Fexco)
                    .WithAmount(10.01m)
                    .WithCurrency("EUR")
                    .Execute("dcc");
            Assert.IsNotNull(dccResponse);
            Assert.AreEqual("00", dccResponse.ResponseCode);

            Transaction authResponse = card.Authorize(10.01m)
                    .WithCurrency("EUR")
                    .WithOrderId(dccResponse.OrderId)
                    .WithDccRateData(dccResponse.DccRateData)
                    .Execute("dcc");
            Assert.IsNotNull(authResponse);
            Assert.AreEqual("00", authResponse.ResponseCode);
        }
    }
}
