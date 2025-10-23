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
        private Address shippingAddress;
        private Address billingAddress;
        private Customer newCustomer;
        /// <summary>
        /// Initializes the test by configuring the GP API service with test credentials and settings.
        /// </summary>
        [TestInitialize]
        public void TestInitialize() {
            ServicesContainer.RemoveConfig();
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);

            gpApiConfig.Country = "US";
            gpApiConfig.AppId = "hkjrcsGDhWiDt8GEhoDMKy3pzFz5R0Bo";
            gpApiConfig.AppKey = "cQOKHoAAvNIcEN8s";
            gpApiConfig.ServiceUrl = ServiceEndpoints.GP_API_TEST;
            gpApiConfig.AccessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountName = "GPECOM_HPP_Transaction_Processing"
            };
            ServicesContainer.ConfigureService(gpApiConfig);

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
        }

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
    }
}
