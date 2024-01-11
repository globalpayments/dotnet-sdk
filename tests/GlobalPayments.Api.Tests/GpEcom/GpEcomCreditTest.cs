using System.Collections.Generic;
using System.Linq;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpEcom {
    [TestClass]
    public class GpEcomCreditTest {
        CreditCardData card;

        [TestInitialize]
        public void Init() {
            var config = new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RebatePassword = "rebate",
                RefundPassword = "refund",
                RequestLogger = new RequestConsoleLogger()
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
        public void CreditAuthorization_WithDynamicDescriptor() {
            string dynamicDescriptor = "MyCompany LLC";

            var authorization = card.Authorize(5m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithDynamicDescriptor(dynamicDescriptor)
                .Execute();
            Assert.IsNotNull(authorization);
            Assert.AreEqual("00", authorization.ResponseCode, authorization.ResponseMessage);

            var capture = authorization.Capture(5m)
                .WithDynamicDescriptor(dynamicDescriptor)
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
                .WithMultiCapture()
                .Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode, capture.ResponseMessage);

            var capture2 = authorization.Capture(5m)
                .WithMultiCapture()
                .Execute();
            Assert.IsNotNull(capture2);
            Assert.AreEqual("00", capture2.ResponseCode, capture2.ResponseMessage);

            var capture3 = authorization.Capture(7m)
                .WithMultiCapture()
                .Execute();
            Assert.IsNotNull(capture3);
            Assert.AreEqual("00", capture3.ResponseCode, capture3.ResponseMessage);
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
        public void Credit_SupplementaryData() {
            var response = card.Authorize(10m)
                .WithCurrency("GBP")
                .WithSupplementaryData("taxInfo", "VATREF", "7636372833321")
                .WithSupplementaryData("indentityInfo", "Passport", "PPS736353")
                .WithSupplementaryData("RANDOM_KEY1", "Passport", "PPS736353")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);

            var captureResponse = response.Capture(10m)
                .WithSupplementaryData("taxInfo", "VATREF", "7636372833321")                
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode, captureResponse.ResponseMessage);
        }

        [TestMethod]
        public void CardBlockingPaymentRequest()
        {
            var cardTypesBlocked = new BlockedCardType();
            cardTypesBlocked.Commercialdebit = true;
            cardTypesBlocked.Consumerdebit = true;

            var authorization = card.Authorize(14)
                .WithCurrency("USD")
                .WithBlockedCardType(cardTypesBlocked)
                .Execute();

            Assert.IsNotNull(authorization);
            Assert.AreEqual("00", authorization.ResponseCode);
        }

        [TestMethod]
        public void CreditFraudResponse() {
            var billingAddress = new Address {
                StreetAddress1 = "Flat 123",
                StreetAddress2 = "House 456",
                StreetAddress3 = "Cul-De-Sac",
                City = "Halifax",
                Province = "West Yorkshire",
                State = "Yorkshire and the Humber",
                Country = "GB",
                PostalCode = "E77 4QJ"
            };

            var shippingAddress = new Address {
                StreetAddress1 = "House 456",
                StreetAddress2 = "987 The Street",
                StreetAddress3 = "Basement Flat",
                City = "Chicago",
                State = "Illinois",
                Province = "Mid West",
                Country = "US",
                PostalCode = "50001"
            };

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

        [TestMethod, Ignore]
        //Ignored because the token expires after 60 seconds and it will fail with the current token due to this fact
        public void AuthMobileApplePay() {
            var token = new CreditCardData {
                Token = "{\"version\":\"EC_v1\",\"data\":\"VHHTScEN6Ody3/iZ9wjDpIdDxtfhZi/LoMi3JJDuU+2T89xx3gSk0S2eUb0qHfBDpcXy47xloFxmuuug5w18lbSukaDnDKOmBRehApysrXROQyTsJHqFtjT78D/EnSLEcWg/0dLk9P9smS7lZCNNf6ys57k6VnT9CNZ5S8JGlW/2aj/V3vc/64T/9H75kihnShIYTVkBAuC+hgIavHuvPrwRiO5OjwXEkHhn2yAokoQuzu63nxhWOk8D4ecwC/1zDOwsuxvGqCqyNeoDW1NgckE64bE5FZYMfqGFHxnd1d+bc7ddG53AQS/m2H+dx3RQr/QG1gru1ICZKRvjRYH00nhS3mkmrnnco50mYWy2Q6GKbJVdIX4NprYEzs3uFXSM52v7IbK9NSwJIE+Qq4dq/c1wcbhdESERHB/ah1K0s64=\",\"signature\":\"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\",\"header\":{\"ephemeralPublicKey\":\"MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEhT7kTBXZPFnR0Em4O510OaV9OGf1pIXoCWO45qNTcD6n+iWTsuw9+k8hqsre6POpEzJnO/QgTsxKoZ7XodBocw==\",\"publicKeyHash\":\"lYM2CUctbz7n9Be2kWdhmGvm0WM8pi8jtBYf5rI5KGo=\",\"transactionId\":\"da9d497fd310fad44b72fb513784aed3a143c4306173acefbd3fc2326ad18d18\"}}",
                MobileType = MobilePaymentMethodType.APPLEPAY
            };

            var response = token.Charge(10m)
                .WithCurrency("USD")
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
            RecurringPaymentMethod storedCard = new RecurringPaymentMethod("20190729-GlobalApi", "20190729-GlobalApi-Credit");

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
            RecurringPaymentMethod storedCard = new RecurringPaymentMethod("20190729-GlobalApi", "20190729-GlobalApi-Credit");

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
        public void FraudManagement_DecisionManager() {
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

            var products = new List<Product> {
                new Product {
                            ProductId = "SKU251584",
                            ProductName = "Magazine Subscription",
                            Quantity = 12,
                            UnitPrice = 12,
                            Gift = true,
                            Type = "subscription",
                            Risk = "Low"
                        },
                new Product {
                            ProductId = "SKU8884784",
                            ProductName = "Charger",
                            Quantity = 10,
                            UnitPrice = 12,
                            Gift = false,
                            Type = "electronic_good",
                            Risk = "High"
                        }
            };

            Transaction response = card.Charge(199.99m)
                    .WithCurrency("EUR")
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithDecisionManager(decisionManager)
                    .WithCustomerData(customer)
                    .WithMiscProductData(products)
                    .WithCustomData("fieldValue01", "fieldValue02", "fieldValue03", "fieldValue04")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void FraudManagement_withFraudRules() {
            string ruleId = "853c1d37-6e9f-467e-9ffc-182210b40c6b";
            FraudFilterMode mode = FraudFilterMode.OFF;
            FraudRuleCollection fraudRuleCollection = new FraudRuleCollection();
            fraudRuleCollection.AddRule(ruleId, mode);

            Transaction response = card.Charge(199.99m)
                    .WithCurrency("EUR")
                    .WithFraudFilter(FraudFilterMode.ACTIVE, fraudRuleCollection)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            var rule = response?.FraudResponse?.Rules?.FirstOrDefault(x => x.Id == ruleId);
            Assert.IsNotNull(rule);
            Assert.AreEqual(rule.Action,"NOT_EXECUTED");
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
