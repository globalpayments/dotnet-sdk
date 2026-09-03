using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.GpApi {
    /// <summary>
    /// Contains tests for creating Hosted Payment Page (HPP) Pay By Link transactions using the GP API.
    /// </summary>
    [TestClass]
    public class GpApiHPPPayByLinksTests : BaseGpApiTests {
        private const string EuConfigName = "EuConfig";
        private const string CashpressoConfigName = "CashpressoHpp";
        private const string CashpressoAppId = "hlZAokTftDazLlWDPe8E6VAz5g9rSDPg";
        private const string CashpressoAppKey = "ThDO2fISzzWCgkCZ"; //gitleaks:allow
        private Address shippingAddress;
        private Address billingAddress;
        private Address germanyAddress;
        private Customer newCustomer;
        /// <summary>
        /// Initializes the test by configuring the GP API service with test credentials and settings.
        /// </summary>
        [TestInitialize]
        public void TestInitialize() {
            ServicesContainer.RemoveConfig();
            ServicesContainer.RemoveConfig(EuConfigName);
            ServicesContainer.RemoveConfig(CashpressoConfigName);

            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            gpApiConfig.Country = "US";
            gpApiConfig.AppId = "hkjrcsGDhWiDt8GEhoDMKy3pzFz5R0Bo";
            gpApiConfig.AppKey = "cQOKHoAAvNIcEN8s";
            gpApiConfig.ServiceUrl = ServiceEndpoints.GP_API_TEST;
            gpApiConfig.AccessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountName = "GPECOM_HPP_Transaction_Processing"
            };
            ServicesContainer.ConfigureService(gpApiConfig);

            var euConfig = GpApiConfigSetup(EuHppAppId, EuHppAppKey, Channel.CardNotPresent);
            euConfig.Country = "US";
            euConfig.ServiceUrl = ServiceEndpoints.GP_API_EU_TEST;
            euConfig.AccessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountName = "GPECOM_Transaction_Processing_CNP"
            };
            ServicesContainer.ConfigureService(euConfig, EuConfigName);

            var cashpressoConfig = GpApiConfigSetup(CashpressoAppId, CashpressoAppKey, Channel.CardNotPresent);
            cashpressoConfig.Country = "DE";
            cashpressoConfig.ServiceUrl = "https://apis-qa.globalpay.com/ucp";
            cashpressoConfig.AccessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountName = "GPECOM_CASHPRESSO_APM_Transaction_Processing"
            };
            ServicesContainer.ConfigureService(cashpressoConfig, CashpressoConfigName);

            billingAddress = new Address {
                StreetAddress1 = "8 MY ROAD",
                StreetAddress2 = "BILL_STREET2",
                StreetAddress3 = "BILL_STREET3",
                City = "LONDON",
                PostalCode = "E2 7EF",
                State = "IL",
                CountryCode = "840",
                Country = "US"
            };

            shippingAddress = new Address {
                StreetAddress1 = "Flat 123",
                StreetAddress2 = "House 456",
                StreetAddress3 = "Btower",
                City = "Chicago",
                PostalCode = "E2 7EF",
                State = "IL",
                CountryCode = "840",
                Country = "US"
            };

            newCustomer = new Customer() {
                Email = "JAMESMASON@EXAMPLE.COM",
                FirstName = "JAMES",
                LastName = "Smith",
                Language = "EN",
                IsShippingAddressSameAsBilling = false,
                Status = "NEW",
                Phone = new PhoneNumber() {
                    CountryCode = "44",
                    Number = "7853283864"
                },
            };

            germanyAddress = new Address
            {
                StreetAddress1 = "Hauptstrasse 25",
                StreetAddress2 = "Apartment 12",
                StreetAddress3 = "Gebaeude C",
                City = "Munich",
                PostalCode = "80331",
                State = "BY",
                Country = "DE",
                CountryCode = "DE"
            };
        }

        #region Positive Tests

        /// <summary>
        /// Creates an HPP Pay By Link for a new customer and verifies the link is active.
        /// </summary>
        [TestMethod]
        public void CreateHPPPayByLink_WithNewCustomer_ReturnsSuccess() {

            var payByLink = new PayByLinkData() {
                Type = PayByLinkType.HOSTED_PAYMENT_PAGE,
                UsageMode = PaymentMethodUsageMode.Single,
                AllowedPaymentMethods = new PaymentMethodName[] {
                    PaymentMethodName.Card
                },
                UsageLimit = 1,
                Name = "Mobile Bill Payment",
                IsShippable = false,
                ShippingAmount = 100,

                ExpirationDate = DateTime.UtcNow.AddDays(10), //date('Y-m-d H:i:s') + 10;
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                CancelUrl = "https://www.example.com/returnUrl",
                Configuration = new PaymentMethodConfiguration {
                     IsAddressOverrideAllowed = true,
                     IsShippingAddressEnabled = true,
                     ChallengeRequestIndicator = ChallengeRequestIndicator.NO_CHALLENGE_REQUESTED,
                     ExemptStatus = ExemptStatus.LOW_VALUE,
                     IsBillingAddressRequired = true,
                     StorageMode = StorageMode.OFF
                },
            };

            var response = PayByLinkService.Create(payByLink, 10)
                .WithCurrency("USD")
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithCustomerData(newCustomer)
                .WithDescription("HPP_Links_Test")
                .WithPhoneNumber("99", "1801555999", PhoneNumberType.Shipping)
                .Execute();

            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(PayByLinkStatus.ACTIVE.ToString().ToUpper(), response.ResponseMessage.ToUpper());
            Assert.IsNotNull(response.PayByLinkResponse.Url);
            Assert.IsNotNull(response.PayByLinkResponse.Id);
        }

        /// <summary>
        /// Creates an HPP Pay By Link for an existing active customer and verifies the link is active.
        /// </summary>
        [TestMethod]
        public void CreateHPPPayByLink_WithExistingActiveCustomer_ReturnsSuccess() {

            newCustomer.Id = "PYR_4f23b94af9294efb8b839e9d1b3f74e1";
            newCustomer.Status = "ACTIVE";

            var payByLink = new PayByLinkData() {
                Type = PayByLinkType.HOSTED_PAYMENT_PAGE,
                UsageMode = PaymentMethodUsageMode.Single,
                AllowedPaymentMethods = new PaymentMethodName[] {
                    PaymentMethodName.Card
                },
                UsageLimit = 1,
                Name = "Mobile Bill Payment",
                IsShippable = false,
                ShippingAmount = 1,

                ExpirationDate = DateTime.UtcNow.AddDays(10), //date('Y-m-d H:i:s') + 10;
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                CancelUrl = "https://www.example.com/returnUrl",
                Configuration = new PaymentMethodConfiguration {
                    IsAddressOverrideAllowed = true,
                    IsShippingAddressEnabled = true,
                    ChallengeRequestIndicator = ChallengeRequestIndicator.NO_CHALLENGE_REQUESTED,
                    ExemptStatus = ExemptStatus.LOW_VALUE,
                    IsBillingAddressRequired = true,
                    StorageMode = StorageMode.OFF
                },
            };

            var response = PayByLinkService.Create(payByLink, 10)
                .WithCurrency("USD")
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithCustomerData(newCustomer)
                .WithDescription("HPP_Links_Test")
                .WithPhoneNumber("99", "1801555999", PhoneNumberType.Shipping)
                .Execute();

            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(PayByLinkStatus.ACTIVE.ToString().ToUpper(), response.ResponseMessage.ToUpper());
            Assert.IsNotNull(response.PayByLinkResponse.Url);
            Assert.IsNotNull(response.PayByLinkResponse.Id);

        }

        /// <summary>
        /// Creates an HPP Pay By Link with Click to Pay as the only enabled digital wallet provider.
        /// </summary>
        [TestMethod]
        public void CreateHPPPayByLink_WithClickToPay_ReturnsSuccess() {

            var payByLink = new PayByLinkData() {
                Type = PayByLinkType.HOSTED_PAYMENT_PAGE,
                UsageMode = PaymentMethodUsageMode.Single,
                AllowedPaymentMethods = new PaymentMethodName[] {
                    PaymentMethodName.Card
                },
                UsageLimit = 1,
                Name = "Mobile Bill Payment",
                IsShippable = false,
                ShippingAmount = 1,
                ExpirationDate = DateTime.UtcNow.AddDays(10),
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                CancelUrl = "https://www.example.com/returnUrl",
                Configuration = new PaymentMethodConfiguration {
                    IsAddressOverrideAllowed = true,
                    IsShippingAddressEnabled = true,
                    ChallengeRequestIndicator = ChallengeRequestIndicator.NO_CHALLENGE_REQUESTED,
                    ExemptStatus = ExemptStatus.LOW_VALUE,
                    IsBillingAddressRequired = true,
                    StorageMode = StorageMode.OFF,
                    EntryMode = PaymentEntryMode.Ecom,
                    DigitalWalletProviders = new DigitalWalletProvider[] {
                        DigitalWalletProvider.CLICK_TO_PAY
                    }
                },
            };

            var response = PayByLinkService.Create(payByLink, 10)
                .WithCurrency("USD")
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithCustomerData(newCustomer)
                .WithDescription("HPP_Links_Test")
                .WithPhoneNumber("99", "1801555999", PhoneNumberType.Shipping)
                .Execute(EuConfigName);

            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(PayByLinkStatus.ACTIVE.ToString().ToUpper(), response.ResponseMessage.ToUpper());
            Assert.IsNotNull(response.PayByLinkResponse.Url);
            Assert.IsNotNull(response.PayByLinkResponse.Id);
        }

        /// <summary>
        /// Creates an HPP Pay By Link with Apple Pay and Google Pay digital wallets (no Click to Pay).
        /// </summary>
        [TestMethod]
        public void CreateHPPPayByLink_WithApplePayAndGooglePayNoClickToPay_ReturnsSuccess() {

            var payByLink = new PayByLinkData() {
                Type = PayByLinkType.HOSTED_PAYMENT_PAGE,
                UsageMode = PaymentMethodUsageMode.Single,
                AllowedPaymentMethods = new PaymentMethodName[] {
                    PaymentMethodName.Card
                },
                UsageLimit = 1,
                Name = "Mobile Bill Payment",
                IsShippable = true,
                ShippingAmount = 1,
                SubmitButtonLabel = "SUBMIT NOW",
                ExpirationDate = DateTime.UtcNow.AddDays(10),
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                CancelUrl = "https://www.example.com/returnUrl",
                Configuration = new PaymentMethodConfiguration {
                    IsAddressOverrideAllowed = true,
                    IsShippingAddressEnabled = true,
                    ChallengeRequestIndicator = ChallengeRequestIndicator.CHALLENGE_MANDATED,
                    ExemptStatus = ExemptStatus.LOW_VALUE,
                    IsBillingAddressRequired = true,
                    StorageMode = StorageMode.ALWAYS,
                    EntryMode = PaymentEntryMode.Ecom,
                    DigitalWalletProviders = new DigitalWalletProvider[] {
                        DigitalWalletProvider.GOOGLEPAY,
                        DigitalWalletProvider.APPLEPAY
                    }
                },
            };

            var response = PayByLinkService.Create(payByLink, 10)
                .WithCurrency("USD")
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithCustomerData(newCustomer)
                .WithDescription("HPP_Links_Test")
                .WithPhoneNumber("99", "1801555999", PhoneNumberType.Shipping)
                .Execute(EuConfigName);

            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(PayByLinkStatus.ACTIVE.ToString().ToUpper(), response.ResponseMessage.ToUpper());
            Assert.IsNotNull(response.PayByLinkResponse.Url);
            Assert.IsNotNull(response.PayByLinkResponse.Id);
        }

        /// <summary>
        /// Creates an HPP Pay By Link with digital wallets (including Click to Pay) and iframe display configuration.
        /// </summary>
        [TestMethod]
        public void CreateHPPPayByLink_WithDigitalWalletsAndDisplayConfig_ReturnsSuccess() {

            var payByLink = new PayByLinkData() {
                Type = PayByLinkType.HOSTED_PAYMENT_PAGE,
                UsageMode = PaymentMethodUsageMode.Single,
                AllowedPaymentMethods = new PaymentMethodName[] {
                    PaymentMethodName.Card
                },
                UsageLimit = 1,
                Name = "Mobile Bill Payment",
                IsShippable = true,
                ShippingAmount = 1,
                SubmitButtonLabel = "SUBMIT NOW",
                ExpirationDate = DateTime.UtcNow.AddDays(10),
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                CancelUrl = "https://www.example.com/returnUrl",
                DisplayConfiguration = new DisplayConfiguration {
                    IframeDimensionsDomain = "https://www.example.com",
                    IframeResponseDomain = "https://www.example.com"
                },
                Configuration = new PaymentMethodConfiguration {
                    IsAddressOverrideAllowed = true,
                    IsShippingAddressEnabled = true,
                    ChallengeRequestIndicator = ChallengeRequestIndicator.CHALLENGE_MANDATED,
                    ExemptStatus = ExemptStatus.LOW_VALUE,
                    IsBillingAddressRequired = true,
                    StorageMode = StorageMode.ALWAYS,
                    EntryMode = PaymentEntryMode.Ecom,
                    DigitalWalletProviders = new DigitalWalletProvider[] {
                        DigitalWalletProvider.GOOGLEPAY,
                        DigitalWalletProvider.APPLEPAY,
                        DigitalWalletProvider.CLICK_TO_PAY
                    }
                },
            };

            var response = PayByLinkService.Create(payByLink, 10)
                .WithCurrency("USD")
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithCustomerData(newCustomer)
                .WithDescription("HPP_Links_Test")
                .WithPhoneNumber("99", "1801555999", PhoneNumberType.Shipping)
                .Execute(EuConfigName);

            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(PayByLinkStatus.ACTIVE.ToString().ToUpper(), response.ResponseMessage.ToUpper());
            Assert.IsNotNull(response.PayByLinkResponse.Url);
            Assert.IsNotNull(response.PayByLinkResponse.Id);
        }

        /// <summary>
        /// Creates an HPP Pay By Link with order surcharges and digital wallets, using valid surcharge amounts.
        /// </summary>
        [TestMethod]
        public void CreateHPPPayByLink_WithSurchargeAndDigitalWallets_ReturnsSuccess() {

            var payByLink = new PayByLinkData() {
                Type = PayByLinkType.HOSTED_PAYMENT_PAGE,
                UsageMode = PaymentMethodUsageMode.Single,
                AllowedPaymentMethods = new PaymentMethodName[] {
                    PaymentMethodName.Card
                },
                UsageLimit = 1,
                Name = "Mobile Bill Payment",
                IsShippable = false,
                ShippingAmount = 1,
                IsDccEnabled = true,
                ExpirationDate = DateTime.UtcNow.AddDays(10),
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                CancelUrl = "https://www.example.com/returnUrl",
                Surcharges = new Surcharge[] {
                    new Surcharge { CardType = SurchargeCardType.DEBIT, Amount = 1000.01m },
                    new Surcharge { CardType = SurchargeCardType.CREDIT, Amount = 1000.02m },
                    new Surcharge { CardType = SurchargeCardType.COMMERCIAL, Amount = 1000.03m }
                },
                Configuration = new PaymentMethodConfiguration {
                    IsAddressOverrideAllowed = true,
                    IsShippingAddressEnabled = true,
                    ChallengeRequestIndicator = ChallengeRequestIndicator.NO_CHALLENGE_REQUESTED,
                    ExemptStatus = ExemptStatus.LOW_VALUE,
                    IsBillingAddressRequired = true,
                    StorageMode = StorageMode.OFF,
                    EntryMode = PaymentEntryMode.Ecom,
                    DigitalWalletProviders = new DigitalWalletProvider[] {
                        DigitalWalletProvider.GOOGLEPAY,
                        DigitalWalletProvider.APPLEPAY,
                        DigitalWalletProvider.CLICK_TO_PAY
                    }
                },
            };

            var response = PayByLinkService.Create(payByLink, 1000)
                .WithCurrency("USD")
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithCustomerData(newCustomer)
                .WithDescription("HPP_Links_Test")
                .WithPhoneNumber("99", "1801555999", PhoneNumberType.Shipping)
                .Execute(EuConfigName);

            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(PayByLinkStatus.ACTIVE.ToString().ToUpper(), response.ResponseMessage.ToUpper());
            Assert.IsNotNull(response.PayByLinkResponse.Url);
            Assert.IsNotNull(response.PayByLinkResponse.Id);
            Assert.IsNotNull(response.PayByLinkResponse.Surcharges);
            Assert.AreEqual(3, response.PayByLinkResponse.Surcharges.Length);
            Assert.AreEqual(SurchargeCardType.DEBIT, response.PayByLinkResponse.Surcharges[0].CardType);
            Assert.AreEqual(1000.01m, response.PayByLinkResponse.Surcharges[0].Amount);
            Assert.AreEqual(SurchargeCardType.CREDIT, response.PayByLinkResponse.Surcharges[1].CardType);
            Assert.AreEqual(1000.02m, response.PayByLinkResponse.Surcharges[1].Amount);
            Assert.AreEqual(SurchargeCardType.COMMERCIAL, response.PayByLinkResponse.Surcharges[2].CardType);
            Assert.AreEqual(1000.03m, response.PayByLinkResponse.Surcharges[2].Amount);
        }

        /// <summary>
        /// Creates an HPP Pay By Link exercising the full request: surcharges, digital wallets,
        /// submit button label, and iframe display configuration.
        /// </summary>
        [TestMethod]
        public void CreateHPPPayByLink_WithSurchargeDigitalWalletsAndDisplayConfig_ReturnsSuccess() {

            var payByLink = new PayByLinkData() {
                Type = PayByLinkType.HOSTED_PAYMENT_PAGE,
                UsageMode = PaymentMethodUsageMode.Single,
                AllowedPaymentMethods = new PaymentMethodName[] {
                    PaymentMethodName.Card
                },
                UsageLimit = 1,
                Name = "Mobile Bill Payment",
                IsShippable = true,
                ShippingAmount = 1,
                IsDccEnabled = true,
                SubmitButtonLabel = "SUBMIT NOW",
                ExpirationDate = DateTime.UtcNow.AddDays(10),
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                CancelUrl = "https://www.example.com/returnUrl",
                DisplayConfiguration = new DisplayConfiguration {
                    IframeDimensionsDomain = "https://www.example.com",
                    IframeResponseDomain = "https://www.example.com"
                },
                Surcharges = new Surcharge[] {
                    new Surcharge { CardType = SurchargeCardType.DEBIT, Amount = 1000.01m },
                    new Surcharge { CardType = SurchargeCardType.CREDIT, Amount = 1000.02m },
                    new Surcharge { CardType = SurchargeCardType.COMMERCIAL, Amount = 1000.03m }
                },
                Configuration = new PaymentMethodConfiguration {
                    IsAddressOverrideAllowed = true,
                    IsShippingAddressEnabled = true,
                    ChallengeRequestIndicator = ChallengeRequestIndicator.NO_CHALLENGE_REQUESTED,
                    ExemptStatus = ExemptStatus.LOW_VALUE,
                    IsBillingAddressRequired = true,
                    StorageMode = StorageMode.OFF,
                    EntryMode = PaymentEntryMode.Ecom,
                    DigitalWalletProviders = new DigitalWalletProvider[] {
                        DigitalWalletProvider.GOOGLEPAY,
                        DigitalWalletProvider.APPLEPAY,
                        DigitalWalletProvider.CLICK_TO_PAY
                    }
                },
            };

            var response = PayByLinkService.Create(payByLink, 1000)
                .WithCurrency("USD")
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithCustomerData(newCustomer)
                .WithDescription("HPP_Links_Test")
                .WithPhoneNumber("99", "1801555999", PhoneNumberType.Shipping)
                .Execute(EuConfigName);

            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(PayByLinkStatus.ACTIVE.ToString().ToUpper(), response.ResponseMessage.ToUpper());
            Assert.IsNotNull(response.PayByLinkResponse.Url);
            Assert.IsNotNull(response.PayByLinkResponse.Id);
            Assert.IsNotNull(response.PayByLinkResponse.Surcharges);
            Assert.AreEqual(3, response.PayByLinkResponse.Surcharges.Length);
            Assert.AreEqual(SurchargeCardType.DEBIT, response.PayByLinkResponse.Surcharges[0].CardType);
            Assert.AreEqual(1000.01m, response.PayByLinkResponse.Surcharges[0].Amount);
            Assert.AreEqual(SurchargeCardType.CREDIT, response.PayByLinkResponse.Surcharges[1].CardType);
            Assert.AreEqual(1000.02m, response.PayByLinkResponse.Surcharges[1].Amount);
            Assert.AreEqual(SurchargeCardType.COMMERCIAL, response.PayByLinkResponse.Surcharges[2].CardType);
            Assert.AreEqual(1000.03m, response.PayByLinkResponse.Surcharges[2].Amount);
            Assert.IsNotNull(response.PayByLinkResponse.Transactions);
            Assert.IsTrue(response.PayByLinkResponse.Transactions.Count > 0);
            Assert.IsNotNull(response.PayByLinkResponse.Transactions[0].TransactionId);
            Assert.IsNotNull(response.PayByLinkResponse.Transactions[0].TransactionStatus);
        }

        #endregion

        #region Negative Tests

        /// <summary>
        /// Verifies that a surcharge amount below the order total is rejected with a GatewayException.
        /// </summary>
        [TestMethod]
        public void CreateHPPPayByLink_WithInvalidSurchargeAndDigitalWallets_ThrowsGatewayException() {

            var payByLink = new PayByLinkData() {
                Type = PayByLinkType.HOSTED_PAYMENT_PAGE,
                UsageMode = PaymentMethodUsageMode.Single,
                AllowedPaymentMethods = new PaymentMethodName[] {
                    PaymentMethodName.Card
                },
                UsageLimit = 1,
                Name = "Mobile Bill Payment",
                IsShippable = false,
                ShippingAmount = 1,
                ExpirationDate = DateTime.UtcNow.AddDays(10),
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                CancelUrl = "https://www.example.com/returnUrl",
                Surcharges = new Surcharge[] {
                    new Surcharge { CardType = SurchargeCardType.DEBIT, Amount = 1 },
                    new Surcharge { CardType = SurchargeCardType.CREDIT, Amount = 2 },
                    new Surcharge { CardType = SurchargeCardType.COMMERCIAL, Amount = 3 }
                },
                Configuration = new PaymentMethodConfiguration {
                    IsAddressOverrideAllowed = true,
                    IsShippingAddressEnabled = true,
                    ChallengeRequestIndicator = ChallengeRequestIndicator.NO_CHALLENGE_REQUESTED,
                    ExemptStatus = ExemptStatus.LOW_VALUE,
                    IsBillingAddressRequired = true,
                    StorageMode = StorageMode.OFF,
                    EntryMode = PaymentEntryMode.Ecom,
                    DigitalWalletProviders = new DigitalWalletProvider[] {
                        DigitalWalletProvider.GOOGLEPAY,
                        DigitalWalletProvider.APPLEPAY,
                        DigitalWalletProvider.CLICK_TO_PAY
                    }
                },
            };

            var ex = Assert.ThrowsException<GatewayException>(() => {
                PayByLinkService.Create(payByLink, 10)
                    .WithCurrency("USD")
                    .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithCustomerData(newCustomer)
                    .WithDescription("HPP_Links_Test")
                    .WithPhoneNumber("99", "1801555999", PhoneNumberType.Shipping)
                    .Execute(EuConfigName);
            });

            Assert.AreEqual("UNKNOWN_RESPONSE", ex.ResponseCode);
            Assert.AreEqual("50012", ex.ResponseMessage);
            Assert.AreEqual("Status Code: NotImplemented - Invalid surcharge amount configured. Please contact the merchant.",
                ex.Message);
        }

        /// <summary>
        /// Verifies that an incompatible Click to Pay configuration (APM-only method with USD) is rejected
        /// with a GatewayException.
        /// </summary>
        [TestMethod]
        public void CreateHPPPayByLink_WithClickToPay_InvalidConfiguration_ThrowsGatewayException() {

            var payByLink = new PayByLinkData() {
                Type = PayByLinkType.HOSTED_PAYMENT_PAGE,
                UsageMode = PaymentMethodUsageMode.Single,
                AllowedPaymentMethods = new PaymentMethodName[] {
                    PaymentMethodName.APM
                },
                UsageLimit = 1,
                Name = "Mobile Bill Payment",
                IsShippable = false,
                ShippingAmount = 1,
                ExpirationDate = DateTime.UtcNow.AddDays(10),
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                CancelUrl = "https://www.example.com/returnUrl",
                Configuration = new PaymentMethodConfiguration {
                    IsAddressOverrideAllowed = true,
                    IsShippingAddressEnabled = true,
                    ChallengeRequestIndicator = ChallengeRequestIndicator.NO_CHALLENGE_REQUESTED,
                    ExemptStatus = ExemptStatus.LOW_VALUE,
                    IsBillingAddressRequired = true,
                    StorageMode = StorageMode.OFF,
                    EntryMode = PaymentEntryMode.Ecom,
                    DigitalWalletProviders = new DigitalWalletProvider[] {
                        DigitalWalletProvider.CLICK_TO_PAY
                    }
                },
            };

            var ex = Assert.ThrowsException<GatewayException>(() => {
                PayByLinkService.Create(payByLink, 10)
                    .WithCurrency("USD")
                    .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithCustomerData(newCustomer)
                    .WithDescription("HPP_Links_Test")
                    .WithPhoneNumber("99", "1801555999", PhoneNumberType.Shipping)
                    .Execute(EuConfigName);
            });

            Assert.AreEqual("UNKNOWN_RESPONSE", ex.ResponseCode);
            Assert.AreEqual("50012", ex.ResponseMessage);
            Assert.AreEqual("Status Code: NotImplemented - Currency USD not allowed. Please contact merchant.",
                ex.Message);
        }

        #endregion

        /// <summary>
        /// Creates a Cashpresso HPP Pay By Link for a new customer and verifies the link is active.
        /// </summary>
        [TestMethod]
        public void CreateHPPPayByLink_WithCashpressoApm_NewCustomer_ReturnsSuccess()
        {
            newCustomer.Status = "NEW";
            newCustomer.FirstName = "Cajimus";
            newCustomer.LastName = "Kibuwuh";
            newCustomer.Language = "en";
            newCustomer.Email = "Cajimus.Kibuwuh8286@example.com";
            newCustomer.Phone = new PhoneNumber { CountryCode = "+49", Number = "996946283" };

            var payByLink = new PayByLinkData()
            {
                Type = PayByLinkType.HOSTED_PAYMENT_PAGE,
                UsageMode = PaymentMethodUsageMode.Single,
                AllowedPaymentMethods = new PaymentMethodName[] {
                    PaymentMethodName.Card,
                    PaymentMethodName.CASHPRESSO
                },
                UsageLimit = 1,
                Name = "Mobile Bill Payment",
                IsShippable = false,
                ShippingAmount = 0,
                ExpirationDate = DateTime.UtcNow.AddDays(10),
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                CancelUrl = "https://www.example.com/returnUrl",
                Configuration = new PaymentMethodConfiguration
                {
                    IsAddressOverrideAllowed = true,
                    IsShippingAddressEnabled = true,
                    ChallengeRequestIndicator = ChallengeRequestIndicator.NO_CHALLENGE_REQUESTED,
                    ExemptStatus = ExemptStatus.LOW_VALUE,
                    IsBillingAddressRequired = true,
                    StorageMode = StorageMode.OFF,
                    ApmConfigurations = new ApmConfiguration[] {
                        new ApmConfiguration {
                            Provider = AlternativePaymentType.CASHPRESSO,
                            PaymentPlans = new CashpressoPaymentPlan[] {
                                CashpressoPaymentPlan.PAY_IN_3_INSTALLMENTS,
                                CashpressoPaymentPlan.PAY_30_DAYS,
                                CashpressoPaymentPlan.FLEXIBLE
                            }
                        }
                    }
                },
                DisplayConfiguration = new DisplayConfiguration
                {
                    IframeDimensionsDomain = "https://www.example.com",
                    IframeResponseDomain = "https://www.example.com",
                    CardholderName = "YES",
                    Cvv = "YES"
                }
            };

            var response = PayByLinkService.Create(payByLink, 650)
                .WithCurrency("EUR")
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithAddress(germanyAddress, AddressType.Shipping)
                .WithAddress(germanyAddress, AddressType.Billing)
                .WithCustomerData(newCustomer)
                .WithDescription("HPP_Cashpresso_New_Customer_Test")
                .WithPhoneNumber("49", "609568831", PhoneNumberType.Shipping)
                .WithCashpressoShippingMethod(CashpressoShippingMethod.DELIVERY)
                .WithShippingDate(new DateTime(2028, 1, 1))
                .WithMiscProductData(new System.Collections.Generic.List<Product> {
                    new Product {
                        ProductName = "Iphone 16",
                        ProductId = "IPH65434",
                        Quantity = 1,
                        UnitPrice = 650,
                        TaxAmount = 0
                    }
                })
                .Execute(CashpressoConfigName);

            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(PayByLinkStatus.ACTIVE.ToString().ToUpper(), response.ResponseMessage.ToUpper());
            Assert.IsNotNull(response.PayByLinkResponse.Url);
            Assert.IsNotNull(response.PayByLinkResponse.Id);
        }

        /// <summary>
        /// Creates an HPP Pay By Link that enables the Cashpresso (BNPL) alternative payment method
        /// and verifies the link is active.
        /// </summary>
        [TestMethod]
        public void CreateHPPPayByLink_WithCashpressoApm_ExistingCustomer_ReturnsSuccess()
        {

            newCustomer.Id = "PYR_992a3181a1bb493ead11474ce0fbd567";
            newCustomer.Status = "ACTIVE";
            newCustomer.FirstName = "Cajimus";
            newCustomer.LastName = "Kibuwuh";
            newCustomer.Language = "en";
            newCustomer.Email = "Cajimus.Kibuwuh8286@example.com";
            newCustomer.Phone = new PhoneNumber { CountryCode = "+49", Number = "996946283" };

            var payByLink = new PayByLinkData()
            {
                Type = PayByLinkType.HOSTED_PAYMENT_PAGE,
                UsageMode = PaymentMethodUsageMode.Single,
                AllowedPaymentMethods = new PaymentMethodName[] {
                    PaymentMethodName.Card,
                    PaymentMethodName.CASHPRESSO
                },
                UsageLimit = 1,
                Name = "Mobile Bill Payment",
                IsShippable = false,
                ShippingAmount = 0,
                ExpirationDate = DateTime.UtcNow.AddDays(10),
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                CancelUrl = "https://www.example.com/returnUrl",
                Configuration = new PaymentMethodConfiguration
                {
                    IsAddressOverrideAllowed = true,
                    IsShippingAddressEnabled = true,
                    ChallengeRequestIndicator = ChallengeRequestIndicator.NO_CHALLENGE_REQUESTED,
                    ExemptStatus = ExemptStatus.LOW_VALUE,
                    IsBillingAddressRequired = true,
                    StorageMode = StorageMode.OFF,
                    ApmConfigurations = new ApmConfiguration[] {
                        new ApmConfiguration {
                            Provider = AlternativePaymentType.CASHPRESSO,
                            PaymentPlans = new CashpressoPaymentPlan[] {
                                CashpressoPaymentPlan.PAY_IN_3_INSTALLMENTS,
                                CashpressoPaymentPlan.PAY_30_DAYS
                            }
                        }
                    }
                },
                DisplayConfiguration = new DisplayConfiguration
                {
                    IframeDimensionsDomain = "https://www.example.com",
                    IframeResponseDomain = "https://www.example.com",
                    CardholderName = "YES",
                    Cvv = "YES"
                }
            };

            var response = PayByLinkService.Create(payByLink, 650)
                .WithCurrency("EUR")
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithAddress(germanyAddress, AddressType.Shipping)
                .WithAddress(germanyAddress, AddressType.Billing)
                .WithCustomerData(newCustomer)
                .WithDescription("HPP_Cashpresso_Test")
                .WithPhoneNumber("49", "609568831", PhoneNumberType.Shipping)
                .WithCashpressoShippingMethod(CashpressoShippingMethod.DELIVERY)
                .WithShippingDate(new DateTime(2028, 1, 1))
                .WithMiscProductData(new System.Collections.Generic.List<Product> {
                    new Product {
                        ProductName = "Iphone 16",
                        ProductId = "IPH65434",
                        Quantity = 1,
                        UnitPrice = 650,
                        TaxAmount = 0
                    }
                })
                .Execute(CashpressoConfigName);

            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(PayByLinkStatus.ACTIVE.ToString().ToUpper(), response.ResponseMessage.ToUpper());
            Assert.IsNotNull(response.PayByLinkResponse.Url);
            Assert.IsNotNull(response.PayByLinkResponse.Id);
        }

        /// <summary>
        /// Rejects a Cashpresso HPP Pay By Link when the link type is missing.
        /// </summary>
        [TestMethod]
        public void CreateHPPPayByLink_WithCashpressoApm_WithoutType_ThrowsGatewayException()
        {
            newCustomer.Id = "PYR_992a3181a1bb493ead11474ce0fbd567";
            newCustomer.Status = "ACTIVE";
            newCustomer.FirstName = "Cajimus";
            newCustomer.LastName = "Kibuwuh";
            newCustomer.Language = "en";
            newCustomer.Email = "Cajimus.Kibuwuh8286@example.com";
            newCustomer.Phone = new PhoneNumber { CountryCode = "+49", Number = "996946283" };

            var payByLink = new PayByLinkData {
                Type = null,
                UsageMode = PaymentMethodUsageMode.Single,
                AllowedPaymentMethods = new PaymentMethodName[] {
                    PaymentMethodName.Card,
                    PaymentMethodName.CASHPRESSO
                },
                UsageLimit = 1,
                Name = "Mobile Bill Payment",
                IsShippable = false,
                ExpirationDate = DateTime.UtcNow.AddDays(10),
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                CancelUrl = "https://www.example.com/returnUrl",
                Configuration = new PaymentMethodConfiguration {
                    IsAddressOverrideAllowed = true,
                    IsShippingAddressEnabled = true,
                    ChallengeRequestIndicator = ChallengeRequestIndicator.NO_CHALLENGE_REQUESTED,
                    ExemptStatus = ExemptStatus.LOW_VALUE,
                    IsBillingAddressRequired = true,
                    StorageMode = StorageMode.OFF,
                    ApmConfigurations = new ApmConfiguration[] {
                        new ApmConfiguration {
                            Provider = AlternativePaymentType.CASHPRESSO,
                            PaymentPlans = new CashpressoPaymentPlan[] {
                                CashpressoPaymentPlan.PAY_IN_3_INSTALLMENTS,
                                CashpressoPaymentPlan.PAY_30_DAYS,
                                CashpressoPaymentPlan.FLEXIBLE
                            }
                        }
                    }
                }
            };

            var ex = Assert.ThrowsException<GatewayException>(() => {
                PayByLinkService.Create(payByLink, 650)
                    .WithCurrency("EUR")
                    .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                    .WithAddress(germanyAddress, AddressType.Shipping)
                    .WithAddress(germanyAddress, AddressType.Billing)
                    .WithCustomerData(newCustomer)
                    .WithDescription("HPP_Cashpresso_Missing_Type_Test")
                    .WithPhoneNumber("49", "609568831", PhoneNumberType.Shipping)
                    .WithCashpressoShippingMethod(CashpressoShippingMethod.DELIVERY)
                    .WithShippingDate(new DateTime(2028, 1, 1))
                    .Execute(CashpressoConfigName);
            });

            Assert.IsTrue(ex.Message.Contains("type"));
        }

    }
}
