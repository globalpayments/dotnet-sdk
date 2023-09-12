using System;
using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpEcom {
    [TestClass]
    public class GpEcomSecure3dServiceTest {
        private CreditCardData card;
        private RecurringPaymentMethod stored;
        private Address shippingAddress;
        private Address billingAddress;
        private BrowserData browserData;

        public GpEcomSecure3dServiceTest() {
            GatewayConfig config = new GpEcomConfig {
                MerchantId = "myMerchantId",
                AccountId = "ecom3ds",
                SharedSecret = "secret",
                MethodNotificationUrl = "https://www.example.com/methodNotificationUrl",
                ChallengeNotificationUrl = "https://www.example.com/challengeNotificationUrl",
                MerchantContactUrl = "https://www.example.com/merchantAboutUrl",
                Secure3dVersion = Secure3dVersion.Any,                
                RequestLogger = new RequestConsoleLogger()
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

         [TestMethod]
         public void FullCycle_v1() {
             card.Number = "4012001037141112";

             var secure = Secure3dService.CheckEnrollment(card)
                 .WithAmount(1m)
                 .WithCurrency("USD")
                 .Execute();

             Assert.AreEqual("False", secure.Enrolled);
         }

        [TestMethod]
        public void FullCycle_v1_ConfigException() {
            card.Number = "4012001037141112";

            var exceptionCaught = false;
            try {
                Secure3dService.CheckEnrollment(card)
                    .WithAmount(1m)
                    .WithCurrency("USD")
                    .Execute(Secure3dVersion.One);
            }
            catch (ConfigurationException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Secure 3d is not configured for version One.", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void FullCycle_v2() {
            // check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                    .Execute();
            Assert.IsNotNull(secureEcom);

            // create card data
            card.ExpMonth = 12;
            card.ExpYear = 2025;
            card.CardHolderName = "John Smith";
          

            if (secureEcom.Enrolled.Equals("True")) {
                Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);

                // initiate authentication
                ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)                        
                        .WithAmount(10.01m)
                        .WithCurrency("GBP")
                        //.WithChallengeRequestIndicator(ChallengeRequestIndicator.NO_PREFERENCE)
                        .WithOrderCreateDate(DateTime.Now)
                        .WithAddress(billingAddress, AddressType.Billing)
                        .WithAddress(shippingAddress, AddressType.Shipping)
                        .WithBrowserData(browserData)
                        .Execute();
                Assert.IsNotNull(initAuth);

                // get authentication data
                secureEcom = Secure3dService.GetAuthenticationData()
                        .WithServerTransactionId(initAuth.ServerTransactionId)
                        .Execute();
                card.ThreeDSecure = secureEcom;

                if (secureEcom.Status.Equals("AUTHENTICATION_SUCCESSFUL")) {
                    Transaction response = card.Charge(10.01m)
                            .WithCurrency("USD")
                            .Execute();
                    Assert.IsNotNull(response);
                    Assert.AreEqual("00", response.ResponseCode);
                }
                else Assert.Fail("Signature verification Assert.Failed.");
            }
            else Assert.Fail("Card not enrolled.");
        }
        
        [TestMethod]
        public void FullCycle_v2_FrictionlessCards() {
            var pairs = new Dictionary<string, string> {
                { "4263970000005262", "2.1.0" },
                { "4222000006724235", "2.1.0" },
                { "4222000006285344", "2.2.0" },
                { "4222000009719489", "2.2.0" }
            };

            foreach (var cardSample in pairs) {
                card.Number = cardSample.Key;
                // check enrollment
                var secureEcom = Secure3dService.CheckEnrollment(card)
                    .Execute();
                Assert.IsNotNull(secureEcom);

                // create card data
                card.ExpMonth = 12;
                card.ExpYear = 2025;
                card.CardHolderName = "John Smith";

                if (secureEcom.Enrolled.Equals("True")) {
                    Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);

                    // initiate authentication
                    ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                        .WithAmount(10.01m)
                        .WithCurrency("GBP")
                        //.WithChallengeRequestIndicator(ChallengeRequestIndicator.NO_PREFERENCE)
                        .WithOrderCreateDate(DateTime.Now)
                        .WithAddress(billingAddress, AddressType.Billing)
                        .WithAddress(shippingAddress, AddressType.Shipping)
                        .WithBrowserData(browserData)
                        .Execute();
                    
                    Assert.IsNotNull(initAuth);
                    Assert.AreEqual(Secure3dVersion.Two, initAuth.Version);
                    Assert.AreEqual("AUTHENTICATION_SUCCESSFUL", initAuth.Status);
                    Assert.IsFalse(initAuth.ChallengeMandated);
                    Assert.IsNotNull(initAuth.AcsTransactionId);
                    Assert.IsNotNull(initAuth.AcsReferenceNumber);
                    Assert.IsNotNull(initAuth.AuthenticationValue);
                    Assert.IsNotNull(initAuth.ServerTransactionId);
                    Assert.AreEqual("05", initAuth.Eci);
                    Assert.AreEqual(cardSample.Value, initAuth.AcsEndVersion);

                    // get authentication data
                    secureEcom = Secure3dService.GetAuthenticationData()
                        .WithServerTransactionId(initAuth.ServerTransactionId)
                        .Execute();
                    card.ThreeDSecure = secureEcom;

                    if (secureEcom.Status.Equals("AUTHENTICATION_SUCCESSFUL"))
                    {
                        Transaction response = card.Charge(10.01m)
                            .WithCurrency("USD")
                            .Execute();
                        Assert.IsNotNull(response);
                        Assert.AreEqual("00", response.ResponseCode);
                    }
                    else Assert.Fail("Signature verification Assert.Failed.");
                }
                else Assert.Fail("Card not enrolled.");
            }
        }
        
        [TestMethod]
        public void FullCycle_v2_ChallengeRequiredCards() {
            var pairs = new Dictionary<string, string> {
                { "4012001038488884", "2.1.0" },
                { "4222000001227408", "2.2.0" }
            };

            foreach (var cardSample in pairs) {
                card.Number = cardSample.Key;

                ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                    .Execute(Secure3dVersion.Two);
                Assert.IsNotNull(secureEcom);

                if (secureEcom.Enrolled.Equals("True"))
                {
                    Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);

                    // initiate authentication
                    ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                        .WithAmount(250m)
                        .WithCurrency("USD")
                        .WithOrderCreateDate(DateTime.Now)
                        .WithOrderId(secureEcom.OrderId)
                        .WithAddress(billingAddress, AddressType.Billing)
                        .WithAddress(shippingAddress, AddressType.Shipping)
                        .WithMethodUrlCompletion(MethodUrlCompletion.NO)
                        .WithBrowserData(browserData)
                        .Execute();

                    Assert.IsNotNull(initAuth);
                    Assert.AreEqual("CHALLENGE_REQUIRED", initAuth.Status);
                    Assert.IsNotNull(initAuth.PayerAuthenticationRequest);
                    Assert.IsNotNull(initAuth.AcsTransactionId);
                    Assert.IsNotNull(initAuth.AcsReferenceNumber);
                    Assert.IsNotNull(initAuth.AuthenticationType);
                    Assert.IsNull(initAuth.Eci);
                    Assert.AreEqual(cardSample.Value, initAuth.AcsEndVersion);

                    // get authentication data
                    secureEcom = Secure3dService.GetAuthenticationData()
                        .WithServerTransactionId(initAuth.ServerTransactionId)
                        .Execute();
                    card.ThreeDSecure = secureEcom;

                    if (secureEcom.Status.Equals("CHALLENGE_REQUIRED")) {
                        Transaction response = card.Charge(10.01m)
                            .WithCurrency("USD")
                            .Execute();
                        Assert.IsNotNull(response);
                        Assert.AreEqual("00", response.ResponseCode);
                    }
                    else Assert.Fail("Signature verification failed.");
                }
                else Assert.Fail("Card not enrolled.");
            }
        }

        [TestMethod]
        public void FullCycle_v2_with_phone_number_validation()
        {
            // check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                    .Execute(Secure3dVersion.Two);
            Assert.IsNotNull(secureEcom);

            if (secureEcom.Enrolled.Equals("True"))
            {
                Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);

                // initiate authentication
                ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                        .WithAmount(10.01m)
                        .WithCurrency("USD")
                        .WithMobileNumber("+54", "(0341) 156-123456")
                        .WithHomeNumber("+54", "(0341) 156-456789")
                        .WithWorkNumber("+54", "(0341) 156-654321")
                        .WithOrderCreateDate(DateTime.Now)
                        .WithAddress(billingAddress, AddressType.Billing)
                        .WithAddress(shippingAddress, AddressType.Shipping)
                        .WithBrowserData(browserData)
                        .Execute();
                Assert.IsNotNull(initAuth);

                // get authentication data
                secureEcom = Secure3dService.GetAuthenticationData()
                        .WithServerTransactionId(initAuth.ServerTransactionId)
                        .Execute();
                card.ThreeDSecure = secureEcom;

                if (secureEcom.Status.Equals("AUTHENTICATION_SUCCESSFUL"))
                {
                    Transaction response = card.Charge(10.01m)
                            .WithCurrency("USD")
                            .Execute();
                    Assert.IsNotNull(response);
                    Assert.AreEqual("00", response.ResponseCode);
                }
                else Assert.Fail("Signature verification Assert.Failed.");
            }
            else Assert.Fail("Card not enrolled.");
        }

        [TestMethod]
        public void FullCycle_Any() {
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                    .WithAmount(1m)
                    .WithCurrency("USD")
                    .Execute(Secure3dVersion.Any);
            Assert.IsNotNull(secureEcom);

            if (secureEcom.Enrolled.Equals("True")) {
                if (secureEcom.Version.Equals(Secure3dVersion.Two)) {
                    // initiate authentication
                    ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                            .WithAmount(10.01m)
                            .WithCurrency("USD")
                            .WithOrderCreateDate(DateTime.Now)
                            .WithAddress(billingAddress, AddressType.Billing)
                            .WithAddress(shippingAddress, AddressType.Shipping)
                            .WithBrowserData(browserData)
                            .Execute();
                    Assert.IsNotNull(initAuth);

                    // get authentication data
                    secureEcom = Secure3dService.GetAuthenticationData()
                            .WithServerTransactionId(initAuth.ServerTransactionId)
                            .Execute();
                    card.ThreeDSecure = secureEcom;

                    if (secureEcom.Status.Equals("AUTHENTICATION_SUCCESSFUL")) {
                        Transaction response = card.Charge(10.01m)
                                .WithCurrency("USD")
                                .Execute();
                        Assert.IsNotNull(response);
                        Assert.AreEqual("00", response.ResponseCode);
                    }
                    else Assert.Fail("Signature verification Assert.Failed.");
                }
                else {
                    // authenticate
                    ThreeDSecureAcsClient authClient = new ThreeDSecureAcsClient(secureEcom.IssuerAcsUrl);
                    var authResponse = authClient.Authenticate(secureEcom.PayerAuthenticationRequest, secureEcom.MerchantData.ToString());

                    string payerAuthenticationResponse = authResponse.getAuthResponse();
                    MerchantDataCollection md = MerchantDataCollection.Parse(authResponse.getMerchantData());

                    // verify signature through the service and affix to the card object
                    secureEcom = Secure3dService.GetAuthenticationData()
                            .WithPayerAuthenticationResponse(payerAuthenticationResponse)
                            .WithMerchantData(md)
                            .Execute();
                    card.ThreeDSecure = secureEcom;

                    if (secureEcom.Status.Equals("Y")) {
                        Transaction response = card.Charge().Execute();
                        Assert.IsNotNull(response);
                        Assert.AreEqual("00", response.ResponseCode);
                    }
                    else Assert.Fail("Signature verification Assert.Failed.");
                }
            }
            else Assert.Fail("Card not enrolled.");
        }

        [TestMethod]
        public void FullCycle_v2_StoredCard() {
            // check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(stored)
                .Execute(Secure3dVersion.Two);
            Assert.IsNotNull(secureEcom);

            if (secureEcom.Enrolled.Equals("True")) {
                Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);

                // initiate authentication
                ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(stored, secureEcom)
                        .WithAmount(10.01m)
                        .WithCurrency("USD")
                        .WithOrderCreateDate(DateTime.Now)
                        .WithAddress(billingAddress, AddressType.Billing)
                        .WithAddress(shippingAddress, AddressType.Shipping)
                        .WithBrowserData(browserData)
                        .WithMethodUrlCompletion(MethodUrlCompletion.NO)
                        .Execute();
                Assert.IsNotNull(initAuth);

                // get authentication data
                secureEcom = Secure3dService.GetAuthenticationData()
                            .WithServerTransactionId(initAuth.ServerTransactionId)
                            .Execute();
                card.ThreeDSecure = secureEcom;

                if (secureEcom.Status.Equals("AUTHENTICATION_SUCCESSFUL")) {
                    Transaction response = stored.Charge(10.01m)
                            .WithCurrency("USD")
                            .Execute();
                    Assert.IsNotNull(response);
                    Assert.AreEqual("00", response.ResponseCode);
                }
                else Assert.Fail("Signature verification failed.");
            }
            else Assert.Fail("Card not enrolled.");
        }

        [TestMethod]
        public void FullCycle_v2_OTB() {
            // check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                        .Execute(Secure3dVersion.Two);
            Assert.IsNotNull(secureEcom);

            if (secureEcom.Enrolled.Equals("True")) {
                Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);

                // initiate authentication
                ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                        .WithAmount(10.01m)
                        .WithCurrency("USD")
                        .WithOrderCreateDate(DateTime.Now)
                        .WithAddress(billingAddress, AddressType.Billing)
                        .WithAddress(shippingAddress, AddressType.Shipping)
                        .WithBrowserData(browserData)
                        .WithMethodUrlCompletion(MethodUrlCompletion.NO)
                        .Execute();
                Assert.IsNotNull(initAuth);

                // get authentication data
                secureEcom = Secure3dService.GetAuthenticationData()
                        .WithServerTransactionId(initAuth.ServerTransactionId)
                        .Execute();
                card.ThreeDSecure = secureEcom;

                if (secureEcom.Status.Equals("AUTHENTICATION_SUCCESSFUL")) {
                    Transaction response = card.Verify()
                            .WithCurrency("USD")
                            .Execute();
                    Assert.IsNotNull(response);
                    Assert.AreEqual("00", response.ResponseCode);
                }
                else Assert.Fail("Signature verification failed.");
            }
            else Assert.Fail("Card not enrolled.");
        }

        [TestMethod]
        public void FullCycle_v2_OTB_StoredCard() {
            // check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(stored)
                        .Execute(Secure3dVersion.Two);
            Assert.IsNotNull(secureEcom);

            if (secureEcom.Enrolled.Equals("True")) {
                Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);

                // initiate authentication
                ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                        .WithAmount(10.01m)
                        .WithCurrency("USD")
                        .WithOrderCreateDate(DateTime.Now)
                        .WithAddress(billingAddress, AddressType.Billing)
                        .WithAddress(shippingAddress, AddressType.Shipping)
                        .WithBrowserData(browserData)
                        .WithMethodUrlCompletion(MethodUrlCompletion.NO)
                        .Execute();
                Assert.IsNotNull(initAuth);

                // get authentication data
                secureEcom = Secure3dService.GetAuthenticationData()
                        .WithServerTransactionId(initAuth.ServerTransactionId)
                        .Execute();
                card.ThreeDSecure = secureEcom;

                if (secureEcom.Status.Equals("AUTHENTICATION_SUCCESSFUL")) {
                    Transaction response = stored.Verify()
                            .WithCurrency("USD")
                            .Execute();
                    Assert.IsNotNull(response);
                    Assert.AreEqual("00", response.ResponseCode);
                }
                else Assert.Fail("Signature verification failed.");
            }
            else Assert.Fail("Card not enrolled.");
        }

        [TestMethod]
        public void OptionalRequestLevelFields() {
            // check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                        .Execute(Secure3dVersion.Two);
            Assert.IsNotNull(secureEcom);

            if (secureEcom.Enrolled.Equals("True")) {
                Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);

                // initiate authentication
                ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                        .WithAmount(10.01m)
                        .WithCurrency("USD")
                        .WithOrderCreateDate(DateTime.Now)
                        .WithAddress(billingAddress, AddressType.Billing)
                        .WithAddress(shippingAddress, AddressType.Shipping)
                        .WithBrowserData(browserData)
                        .WithMethodUrlCompletion(MethodUrlCompletion.NO)

                        // optionals
                        .WithMerchantInitiatedRequestType(MerchantInitiatedRequestType.RECURRING_TRANSACTION)

                        .Execute();
                Assert.IsNotNull(initAuth);

                // get authentication data
                secureEcom = Secure3dService.GetAuthenticationData()
                        .WithServerTransactionId(initAuth.ServerTransactionId)
                        .Execute();
                card.ThreeDSecure = secureEcom;

                if (secureEcom.Status.Equals("AUTHENTICATION_SUCCESSFUL")) {
                    Transaction response = card.Charge(10.01m)
                            .WithCurrency("USD")
                            .Execute();
                    Assert.IsNotNull(response);
                    Assert.AreEqual("00", response.ResponseCode);
                }
                else Assert.Fail("Signature verification failed.");
            }
            else Assert.Fail("Card not enrolled.");
        }

        [TestMethod]
        public void OptionalOrderLevelFields() {
            // check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                        .Execute(Secure3dVersion.Two);
            Assert.IsNotNull(secureEcom);

            if (secureEcom.Enrolled.Equals("True")) {
                Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);

                // initiate authentication
                ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                        .WithAmount(250m)
                        .WithCurrency("USD")
                        .WithOrderCreateDate(DateTime.Now)
                        .WithAddress(billingAddress, AddressType.Billing)
                        .WithAddress(shippingAddress, AddressType.Shipping)
                        .WithBrowserData(browserData)
                        .WithMethodUrlCompletion(MethodUrlCompletion.NO)

                        // optionals
                        .WithGiftCardCount(1)
                        .WithGiftCardCurrency("USD")
                        .WithGiftCardAmount(250m)
                        .WithDeliveryEmail("james.mason@example.com")
                        .WithDeliveryTimeFrame(DeliveryTimeFrame.ELECTRONIC_DELIVERY)
                        .WithShippingMethod(ShippingMethod.ANOTHER_VERIFIED_ADDRESS)
                        .WithShippingNameMatchesCardHolderName(true)
                        .WithPreOrderIndicator(PreOrderIndicator.FUTURE_AVAILABILITY)
                        .WithPreOrderAvailabilityDate(DateTime.Parse("2019-04-18"))
                        .WithReorderIndicator(ReorderIndicator.REORDER)
                        .WithOrderTransactionType(OrderTransactionType.GOODS_SERVICE_PURCHASE)

                        .Execute();
                Assert.IsNotNull(initAuth);

                // get authentication data
                secureEcom = Secure3dService.GetAuthenticationData()
                        .WithServerTransactionId(initAuth.ServerTransactionId)
                        .Execute();
                card.ThreeDSecure = secureEcom;

                if (secureEcom.Status.Equals("AUTHENTICATION_SUCCESSFUL")) {
                    Transaction response = card.Charge(10.01m)
                            .WithCurrency("USD")
                            .Execute();
                    Assert.IsNotNull(response);
                    Assert.AreEqual("00", response.ResponseCode);
                }
                else Assert.Fail("Signature verification failed.");
            }
            else Assert.Fail("Card not enrolled.");
        }

        [TestMethod]
        public void OptionalPayerLevelFields() {
            // check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                        .Execute(Secure3dVersion.Two);
            Assert.IsNotNull(secureEcom);

            if (secureEcom.Enrolled.Equals("True")) {
                Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);

                // initiate authentication
                ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                        .WithAmount(250m)
                        .WithCurrency("USD")
                        .WithOrderCreateDate(DateTime.Now)
                        .WithAddress(billingAddress, AddressType.Billing)
                        .WithAddress(shippingAddress, AddressType.Shipping)
                        .WithBrowserData(browserData)
                        .WithMethodUrlCompletion(MethodUrlCompletion.NO)

                        // optionals
                        .WithCustomerAccountId("6dcb24f5-74a0-4da3-98da-4f0aa0e88db3")
                        .WithAccountAgeIndicator(AgeIndicator.LESS_THAN_THIRTY_DAYS)
                        .WithAccountCreateDate(DateTime.Parse("2019-01-10"))
                        .WithAccountChangeDate(DateTime.Parse("2019-01-28"))
                        .WithAccountChangeIndicator(AgeIndicator.THIS_TRANSACTION)
                        .WithPasswordChangeDate(DateTime.Parse("2019-01-15"))
                        .WithPasswordChangeIndicator(AgeIndicator.LESS_THAN_THIRTY_DAYS)
                        .WithHomeNumber("44", "123456798")
                        .WithWorkNumber("44", "1801555888")
                        .WithPaymentAccountCreateDate(DateTime.Parse("2019-01-01"))
                        .WithPaymentAccountAgeIndicator(AgeIndicator.LESS_THAN_THIRTY_DAYS)
                        .WithPreviousSuspiciousActivity(false)
                        .WithNumberOfPurchasesInLastSixMonths(3)
                        .WithNumberOfTransactionsInLast24Hours(1)
                        .WithNumberOfTransactionsInLastYear(5)
                        .WithNumberOfAddCardAttemptsInLast24Hours(1)
                        .WithShippingAddressCreateDate(DateTime.Parse("2019-01-28"))
                        .WithShippingAddressUsageIndicator(AgeIndicator.THIS_TRANSACTION)

                        .Execute();
                Assert.IsNotNull(initAuth);

                // get authentication data
                secureEcom = Secure3dService.GetAuthenticationData()
                        .WithServerTransactionId(initAuth.ServerTransactionId)
                        .Execute();
                card.ThreeDSecure = secureEcom;

                if (secureEcom.Status.Equals("AUTHENTICATION_SUCCESSFUL")) {
                    Transaction response = card.Charge(10.01m)
                            .WithCurrency("USD")
                            .Execute();
                    Assert.IsNotNull(response);
                    Assert.AreEqual("00", response.ResponseCode);
                }
                else Assert.Fail("Signature verification failed.");
            }
            else Assert.Fail("Card not enrolled.");
        }

        [TestMethod]
        public void OptionalPriorAuthenticationData() {
            // check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                        .Execute(Secure3dVersion.Two);
            Assert.IsNotNull(secureEcom);

            if (secureEcom.Enrolled.Equals("True")) {
                Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);

                // initiate authentication
                ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                        .WithAmount(250m)
                        .WithCurrency("USD")
                        .WithOrderCreateDate(DateTime.Now)
                        .WithAddress(billingAddress, AddressType.Billing)
                        .WithAddress(shippingAddress, AddressType.Shipping)
                        .WithBrowserData(browserData)
                        .WithMethodUrlCompletion(MethodUrlCompletion.NO)

                        // optionals
                        .WithPriorAuthenticationMethod(PriorAuthenticationMethod.FRICTIONLESS_AUTHENTICATION)
                        .WithPriorAuthenticationTransactionId("26c3f619-39a4-4040-bf1f-6fd433e6d615")
                        .WithPriorAuthenticationTimestamp(DateTime.Parse("2019-01-10T12:57:33.333Z"))
                        .WithPriorAuthenticationData("string")

                        .Execute();
                Assert.IsNotNull(initAuth);

                // get authentication data
                secureEcom = Secure3dService.GetAuthenticationData()
                        .WithServerTransactionId(initAuth.ServerTransactionId)
                        .Execute();
                card.ThreeDSecure = secureEcom;

                if (secureEcom.Status.Equals("AUTHENTICATION_SUCCESSFUL")) {
                    Transaction response = card.Charge(10.01m)
                            .WithCurrency("USD")
                            .Execute();
                    Assert.IsNotNull(response);
                    Assert.AreEqual("00", response.ResponseCode);
                }
                else Assert.Fail("Signature verification failed.");
            }
            else Assert.Fail("Card not enrolled.");
        }

        [TestMethod]
        public void OptionalRecurringData() {
            // check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                        .Execute(Secure3dVersion.Two);
            Assert.IsNotNull(secureEcom);

            if (secureEcom.Enrolled.Equals("True")) {
                Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);

                // initiate authentication
                ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                        .WithAmount(250m)
                        .WithCurrency("USD")
                        .WithOrderCreateDate(DateTime.Now)
                        .WithAddress(billingAddress, AddressType.Billing)
                        .WithAddress(shippingAddress, AddressType.Shipping)
                        .WithBrowserData(browserData)
                        .WithMethodUrlCompletion(MethodUrlCompletion.NO)

                        // optionals
                        .WithMaxNumberOfInstallments(5)
                        .WithRecurringAuthorizationFrequency(25)
                        .WithRecurringAuthorizationExpiryDate(DateTime.Parse("2019-08-25"))

                        .Execute();
                Assert.IsNotNull(initAuth);

                // get authentication data
                secureEcom = Secure3dService.GetAuthenticationData()
                        .WithServerTransactionId(initAuth.ServerTransactionId)
                        .Execute();
                card.ThreeDSecure = secureEcom;

                if (secureEcom.Status.Equals("AUTHENTICATION_SUCCESSFUL")) {
                    Transaction response = card.Charge(10.01m)
                            .WithCurrency("USD")
                            .Execute();
                    Assert.IsNotNull(response);
                    Assert.AreEqual("00", response.ResponseCode);
                }
                else Assert.Fail("Signature verification failed.");
            }
            else Assert.Fail("Card not enrolled.");
        }

        [TestMethod]
        public void OptionalPayerLoginData() {
            // check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                        .Execute(Secure3dVersion.Two);
            Assert.IsNotNull(secureEcom);

            if (secureEcom.Enrolled.Equals("True")) {
                Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);

                // initiate authentication
                ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                        .WithAmount(250m)
                        .WithCurrency("USD")
                        .WithOrderCreateDate(DateTime.Now)
                        .WithAddress(billingAddress, AddressType.Billing)
                        .WithAddress(shippingAddress, AddressType.Shipping)
                        .WithBrowserData(browserData)
                        .WithMethodUrlCompletion(MethodUrlCompletion.NO)

                        // optionals
                        .WithCustomerAuthenticationData("string")
                        .WithCustomerAuthenticationTimestamp(DateTime.Parse("2019-01-28T12:57:33.333Z")) //, DateTimeFormat.forPattern("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'")))
                        .WithCustomerAuthenticationMethod(CustomerAuthenticationMethod.MERCHANT_SYSTEM_AUTHENTICATION)

                        .Execute();
                Assert.IsNotNull(initAuth);

                // get authentication data
                secureEcom = Secure3dService.GetAuthenticationData()
                        .WithServerTransactionId(initAuth.ServerTransactionId)
                        .Execute();
                card.ThreeDSecure = secureEcom;

                if (secureEcom.Status.Equals("AUTHENTICATION_SUCCESSFUL")) {
                    Transaction response = card.Charge(10.01m)
                            .WithCurrency("USD")
                            .Execute();
                    Assert.IsNotNull(response);
                    Assert.AreEqual("00", response.ResponseCode);
                }
                else Assert.Fail("Signature verification failed.");
            }
            else Assert.Fail("Card not enrolled.");
        }

        [TestMethod]
        public void OptionalMobileFields() {
            // check enrollment
            card.Number = "4012001038488884";

            const string ephemeralPublicKey = "{" +
                                              "\"kty\":\"EC\"," +
                                              "\"crv\":\"P-256\"," +
                                              "\"x\":\"WWcpTjbOqiu_1aODllw5rYTq5oLXE_T0huCPjMIRbkI\"," +
                                              "\"y\":\"Wz_7anIeadV8SJZUfr4drwjzuWoUbOsHp5GdRZBAAiw\"" +
                                              "}";

            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                        .Execute(Secure3dVersion.Two);
            Assert.IsNotNull(secureEcom);

            if (secureEcom.Enrolled.Equals("True")) {
                Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);

                // initiate authentication
                ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                        .WithAmount(250m)
                        .WithCurrency("USD")
                        .WithAuthenticationSource(AuthenticationSource.MOBILE_SDK)
                        .WithOrderCreateDate(DateTime.Now)
                        .WithOrderId(secureEcom.OrderId)
                        .WithAddress(billingAddress, AddressType.Billing)
                        .WithAddress(shippingAddress, AddressType.Shipping)
                        .WithAddressMatchIndicator(false)
                        .WithMethodUrlCompletion(MethodUrlCompletion.NO)
                        .WithMessageCategory(MessageCategory.PAYMENT_AUTHENTICATION)
                        .WithCustomerEmail("customer@domain.com")
                        //.WithBrowserData(browserData)

                        // optionals
                        .WithApplicationId("f283b3ec-27da-42a1-acea-f3f70e75bbdc")
                        .WithSdkInterface(SdkInterface.BOTH)
                        .WithSdkUiTypes(SdkUiType.TEXT, SdkUiType.SINGLE_SELECT, SdkUiType.MULTI_SELECT, SdkUiType.OOB, SdkUiType.HTML_OTHER)                        
                        .WithMaximumTimeout(5)
                        .WithReferenceNumber("3DS_LOA_SDK_PPFU_020100_00007")
                        .WithSdkTransactionId("b2385523-a66c-4907-ac3c-91848e8c0067")
                        .WithEncodedData("ew0KCSJEViI6ICIxLjAiLA0KCSJERCI6IHsNCgkJIkMwMDEiOiAiQW5kcm9pZCIsDQoJCSJDMDAyIjogIkhUQyBPbmVfTTgiLA0KCQkiQzAwNCI6ICI1LjAuMSIsDQoJCSJDMDA1IjogImVuX1VTIiwNCgkJIkMwMDYiOiAiRWFzdGVybiBTdGFuZGFyZCBUaW1lIiwNCgkJIkMwMDciOiAiMDY3OTc5MDMtZmI2MS00MWVkLTk0YzItNGQyYjc0ZTI3ZDE4IiwNCgkJIkMwMDkiOiAiSm9obidzIEFuZHJvaWQgRGV2aWNlIg0KCX0sDQoJIkRQTkEiOiB7DQoJCSJDMDEwIjogIlJFMDEiLA0KCQkiQzAxMSI6ICJSRTAzIg0KCX0sDQoJIlNXIjogWyJTVzAxIiwgIlNXMDQiXQ0KfQ0K")
                        .WithEphemeralPublicKey(ephemeralPublicKey)
                        
                        .Execute();
                
                Assert.IsNotNull(initAuth);
                Assert.AreEqual("CHALLENGE_REQUIRED", initAuth.Status);
                Assert.IsNotNull(initAuth.PayerAuthenticationRequest);
                Assert.IsNotNull(initAuth.AcsInterface);
                Assert.IsNotNull(initAuth.AcsTransactionId);
                Assert.IsNotNull(initAuth.AcsUiTemplate);
                Assert.IsNotNull(initAuth.AcsReferenceNumber);

                // get authentication data
                secureEcom = Secure3dService.GetAuthenticationData()
                        .WithServerTransactionId(initAuth.ServerTransactionId)
                        .Execute();
                card.ThreeDSecure = secureEcom;

                if (secureEcom.Status.Equals("CHALLENGE_REQUIRED")) {
                    Transaction response = card.Charge(10.01m)
                            .WithCurrency("USD")
                            .Execute();
                    Assert.IsNotNull(response);
                    Assert.AreEqual("00", response.ResponseCode);
                }
                else Assert.Fail("Signature verification failed.");
            }
            else Assert.Fail("Card not enrolled.");
        }

        [TestMethod]
        public void Manual_3DS_Data() {
            var card = new CreditCardData {
                Number = "4012001038488884",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "131",
                CardHolderName = "James Mason",
                ThreeDSecure = new ThreeDSecure {
                    AuthenticationValue = "ODQzNjgwNjU0ZjM3N2JmYTg0NTM=",
                    DirectoryServerTransactionId = "c272b04f-6e7b-43a2-bb78-90f4fb94aa25",
                    Eci = "5",
                    MessageVersion = "2.1.0"
                }
            };

            Transaction response = card.Charge(10.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void ExceptionTest() {
            // supply existing customer/payer ref
            string customerId = "20190819-Realex";
            // supply existing card/payment method ref
            string paymentId = "20190819-Realex-Credit";

            // create the payment method object
            RecurringPaymentMethod paymentMethod = new RecurringPaymentMethod(customerId, paymentId);

            var exceptionCaught = false;
            try {
                Secure3dService.CheckEnrollment(paymentMethod)
                    .Execute(Secure3dVersion.Two);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: NotFound - Payment method not found", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
    }
}
