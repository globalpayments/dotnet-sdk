using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApi3DSecureTests : BaseGpApiTests {
        private CreditCardData card;
        private RecurringPaymentMethod stored;
        private Address shippingAddress;
        private Address billingAddress;
        private BrowserData browserData;

        public GpApi3DSecureTests() {
            //GatewayConfig config = new GpEcomConfig {
            //    MerchantId = "myMerchantId",
            //    AccountId = "ecom3ds",
            //    SharedSecret = "secret",
            //    MethodNotificationUrl = "https://www.example.com/methodNotificationUrl",
            //    ChallengeNotificationUrl = "https://www.example.com/challengeNotificationUrl",
            //    Secure3dVersion = Secure3dVersion.Any,
            //};
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
            });

            // create card data
            card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = 2025,
                CardHolderName = "John Smith"
            };

            // stored card
            stored = new RecurringPaymentMethod(
                "20190809-Realex",
                "20190809-Realex-Credit"
            );

            // shipping address
            shippingAddress = new Address {
                StreetAddress1 = "Apartment 852",
                StreetAddress2 = "Complex 741",
                StreetAddress3 = "no",
                City = "Chicago",
                PostalCode = "5001",
                State = "IL",
                CountryCode = "840"
            };

            // billing address
            billingAddress = new Address {
                StreetAddress1 = "Flat 456",
                StreetAddress2 = "House 789",
                StreetAddress3 = "no",
                City = "Halifax",
                PostalCode = "W5 9HR",
                CountryCode = "826"
            };

            // browser data
            browserData = new BrowserData {
                AcceptHeader = "text/html,application/xhtml+xml,application/xml;q=9,image/webp,img/apng,*/*;q=0.8",
                ColorDepth = ColorDepth.TWENTY_FOUR_BITS,
                IpAddress = "123.123.123.123",
                JavaEnabled = true,
                Language = "en",
                ScreenHeight = 1080,
                ScreenWidth = 1920,
                ChallengeWindowSize = ChallengeWindowSize.WINDOWED_600X400,
                Timezone = "0",
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64, x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.110 Safari/537.36"
            };
        }

        [Ignore]
        [TestMethod]
        public void FullCycle_v1() {
            card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = 2025,
                CardHolderName = "John Doe",
            };

            // Check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency("USD")
                .WithAmount(10.01m)
                .Execute(Secure3dVersion.Any);
            
            Assert.IsNotNull(secureEcom);

            if (secureEcom.Enrolled.Equals("ENROLLED")) {
                Assert.AreEqual(Secure3dVersion.One, secureEcom.Version);

                // Get authentication data
                secureEcom = Secure3dService.GetAuthenticationData()
                    .WithServerTransactionId(secureEcom.ServerTransactionId)
                    .Execute();

                card.ThreeDSecure = secureEcom;

                if (secureEcom.Status.Equals("SUCCESS_AUTHENTICATED")) {
                    Transaction response = card.Charge(10.01m)
                        .WithCurrency("USD")
                        .Execute();
                    Assert.IsNotNull(response);
                    Assert.AreEqual(SUCCESS, response?.ResponseCode);
                    Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
                }
                else Assert.Fail("Signature verification Assert.Failed.");
            }
            else Assert.Fail("Card not enrolled.");
        }

        [Ignore]
        [TestMethod]
        public void FullCycle_v2() {
            card = new CreditCardData {
                Number = "4012001038488884",
                ExpMonth = 12,
                ExpYear = 2025,
                CardHolderName = "John Doe"
            };

            // Check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency("USD")
                .WithAmount(10.01m)
                .Execute(Secure3dVersion.Any);

            Assert.IsNotNull(secureEcom);

            if (secureEcom.Enrolled.Equals("ENROLLED")) {
                Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);

                // Initiate authentication
                ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                    .WithAmount(10.01m)
                    .WithCurrency("USD")
                    .WithOrderCreateDate(DateTime.Now)
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithBrowserData(browserData)
                    .Execute();
                Assert.IsNotNull(initAuth);

                // Get authentication data
                secureEcom = Secure3dService.GetAuthenticationData()
                    .WithServerTransactionId(initAuth.ServerTransactionId)
                    .Execute();

                card.ThreeDSecure = secureEcom;

                if (secureEcom.Status.Equals("SUCCESS_AUTHENTICATED")) {
                    Transaction response = card.Charge(10.01m)
                        .WithCurrency("USD")
                        .Execute();
                    Assert.IsNotNull(response);
                    Assert.AreEqual(SUCCESS, response?.ResponseCode);
                    Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
                }
                else Assert.Fail("Signature verification Assert.Failed.");
            }
            else Assert.Fail("Card not enrolled.");
        }
    }
}
