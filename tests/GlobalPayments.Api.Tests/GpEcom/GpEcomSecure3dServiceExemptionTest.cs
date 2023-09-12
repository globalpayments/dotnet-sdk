using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpEcom {
    [TestClass]
    public class GpEcomSecure3dServiceExemptionTest {
        private CreditCardData card;
        private RecurringPaymentMethod stored;
        private Address shippingAddress;
        private Address billingAddress;
        private BrowserData browserData;

        public GpEcomSecure3dServiceExemptionTest() {
            GatewayConfig config = new GpEcomConfig {
                MerchantId = "myMerchantId",
                AccountId = "ecomeos",
                SharedSecret = "secret",
                MethodNotificationUrl = "https://www.example.com/methodNotificationUrl",
                ChallengeNotificationUrl = "https://www.example.com/challengeNotificationUrl",
                Secure3dVersion = Secure3dVersion.Any,
            };
            ServicesContainer.ConfigureService(config);

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

        /// <summary>
        /// 'APPLY_EXEMPTION' - Amount is less than or equal to 250 EUR (or converted equivalent)
        /// The 3D Secure Service will populate the outbound authentication message with the appropriate exemption flag.
        /// </summary>
        [TestMethod]
        public void FullCycle_v2_EOS_ApplyExemption() {
            // check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                    .Execute(Secure3dVersion.Two);

            Assert.IsNotNull(secureEcom);

            if (secureEcom.Enrolled.Equals("True")) {
                Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);

                // initiate authentication
                ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                    .WithAmount(10.01m)
                    .WithCurrency("EUR")
                    .WithOrderCreateDate(DateTime.Now)
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithBrowserData(browserData)
                    .WithEnableExemptionOptimization(true)
                    .Execute();
                
                Assert.IsNotNull(initAuth);
                Assert.AreEqual(ExemptReason.APPLY_EXEMPTION.ToString(), initAuth.ExemptReason);
                Assert.AreEqual(ExemptStatus.TRANSACTION_RISK_ANALYSIS, initAuth.ExemptStatus);

                // get authentication data
                secureEcom = Secure3dService.GetAuthenticationData()
                    .WithServerTransactionId(initAuth.ServerTransactionId)
                    .Execute();

                card.ThreeDSecure = secureEcom;

                if (secureEcom.Status.Equals("AUTHENTICATION_SUCCESSFUL")) {
                    Transaction response = card.Charge(10.01m)
                        .WithCurrency("EUR")
                        .Execute();

                    Assert.IsNotNull(response);
                    Assert.AreEqual("00", response.ResponseCode);
                }
                else Assert.Fail("Signature verification Assert.Failed.");
            }
            else Assert.Fail("Card not enrolled.");
        }

        /// <summary>
        /// 'CONTINUE' - Amount is above 250 EUR and less than or equal to 500 EUR (or converted equivalent)
        /// The 3D Secure Service will populate the outbound authentication as normal.
        /// </summary>
        [TestMethod]
        public void FullCycle_v2_EOS_Continue() {
            // check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                .Execute(Secure3dVersion.Two);

            Assert.IsNotNull(secureEcom);

            if (secureEcom.Enrolled.Equals("True")) {
                Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);

                // initiate authentication
                ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                    .WithAmount(300m)
                    .WithCurrency("EUR")
                    .WithOrderCreateDate(DateTime.Now)
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithBrowserData(browserData)
                    .WithEnableExemptionOptimization(true)
                    .Execute();

                Assert.IsNotNull(initAuth);
                Assert.AreEqual(ExemptReason.CONTINUE.ToString(), initAuth.ExemptReason);
                Assert.IsNull(initAuth.ExemptStatus);

                // get authentication data
                secureEcom = Secure3dService.GetAuthenticationData()
                    .WithServerTransactionId(initAuth.ServerTransactionId)
                    .Execute();

                card.ThreeDSecure = secureEcom;

                if (secureEcom.Status.Equals("AUTHENTICATION_SUCCESSFUL")) {
                    Transaction response = card.Charge(10.01m)
                        .WithCurrency("EUR")
                        .Execute();

                    Assert.IsNotNull(response);
                    Assert.AreEqual("00", response.ResponseCode);
                }
                else Assert.Fail("Signature verification Assert.Failed.");
            }
            else Assert.Fail("Card not enrolled.");
        }

        /// <summary>
        /// 'FORCE_SECURE' - Amount is above 500 EUR and less than or equal to 750 EUR (or converted equivalent)
        /// The 3D Secure Service will populate the outbound authentication message indicating a challenge is mandated.
        /// This will always force a challenge to be applied, regardless of test card used.
        /// </summary>
        [TestMethod]
        public void FullCycle_v2_EOS_ForceSecure() {
            // check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                .Execute(Secure3dVersion.Two);

            Assert.IsNotNull(secureEcom);

            if (secureEcom.Enrolled.Equals("True")) {
                Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);

                // initiate authentication
                ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                    .WithAmount(550m)
                    .WithCurrency("EUR")
                    .WithOrderCreateDate(DateTime.Now)
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithBrowserData(browserData)
                    .WithEnableExemptionOptimization(true)
                    .Execute();

                Assert.IsNotNull(initAuth);
                Assert.AreEqual("CHALLENGE_REQUIRED", initAuth.Status);
                Assert.AreEqual(ExemptReason.FORCE_SECURE.ToString(), initAuth.ExemptReason);
                Assert.IsNull(initAuth.ExemptStatus);

                // get authentication data
                secureEcom = Secure3dService.GetAuthenticationData()
                    .WithServerTransactionId(initAuth.ServerTransactionId)
                    .Execute();

                Assert.IsNotNull(secureEcom);
                Assert.AreEqual("CHALLENGE_REQUIRED", secureEcom.Status);
            }
            else Assert.Fail("Card not enrolled.");
        }

        /// <summary>
        /// 'BLOCK' - Amount is above 750 EUR (or converted equivalent)
        /// The transaction will be blocked, and a 202 Accepted response will be returned.
        /// </summary>
        [TestMethod]
        public void FullCycle_v2_EOS_Block() {
            // check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                .Execute(Secure3dVersion.Two);

            Assert.IsNotNull(secureEcom);

            if (secureEcom.Enrolled.Equals("True")) {
                Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);

                try {
                    // initiate authentication
                    ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                        .WithAmount(800m)
                        .WithCurrency("EUR")
                        .WithOrderCreateDate(DateTime.Now)
                        .WithAddress(billingAddress, AddressType.Billing)
                        .WithAddress(shippingAddress, AddressType.Shipping)
                        .WithBrowserData(browserData)
                        .WithEnableExemptionOptimization(true)
                        .Execute();
                }
                catch (GatewayException ex) {
                    var message = ex.Message.Replace("\n", "").Replace("\r", "").Replace("\"", "'");
                    Assert.AreEqual(@"Status Code: Accepted - {  'eos_reason' : 'Blocked by Transaction Risk Analysis.'}", message);
                }
            }
            else Assert.Fail("Card not enrolled.");
        }
    }
}
