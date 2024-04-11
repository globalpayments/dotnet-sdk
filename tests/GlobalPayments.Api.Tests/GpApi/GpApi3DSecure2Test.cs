using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApi3DSecure2Test : BaseGpApiTests {
       
        private CreditCardData card;
        private readonly Address shippingAddress;
        private readonly BrowserData browserData;
        private readonly MobileData mobileData;

        private const string Currency = "GBP";
        private static readonly decimal Amount = new decimal(10.01);

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);
        }

        public GpApi3DSecure2Test() {
            // Create card data
            card = new CreditCardData {
                Number = GpApi3DSTestCards.CARD_CHALLENGE_REQUIRED_V2_1,
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                CardHolderName = "John Smith"
            };

            // Shipping address
            shippingAddress = new Address {
                StreetAddress1 = "Apartment 852",
                StreetAddress2 = "Complex 741",
                StreetAddress3 = "no",
                City = "Chicago",
                PostalCode = "5001",
                State = "IL",
                CountryCode = "840"
            };

            // Browser data
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
                UserAgent =
                    "Mozilla/5.0 (Windows NT 6.1; Win64, x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.110 Safari/537.36"
            };

            // Mobile data
            mobileData =
                    new MobileData { 
                            EncodedData = "ew0KCSJEViI6ICIxLjAiLA0KCSJERCI6IHsNCgkJIkMwMDEiOiAiQW5kcm9pZCIsDQoJCSJDMDAyIjogIkhUQyBPbmVfTTgiLA0KCQkiQzAwNCI6ICI1LjAuMSIsDQoJCSJDMDA1IjogImVuX1VTIiwNCgkJIkMwMDYiOiAiRWFzdGVybiBTdGFuZGFyZCBUaW1lIiwNCgkJIkMwMDciOiAiMDY3OTc5MDMtZmI2MS00MWVkLTk0YzItNGQyYjc0ZTI3ZDE4IiwNCgkJIkMwMDkiOiAiSm9obidzIEFuZHJvaWQgRGV2aWNlIg0KCX0sDQoJIkRQTkEiOiB7DQoJCSJDMDEwIjogIlJFMDEiLA0KCQkiQzAxMSI6ICJSRTAzIg0KCX0sDQoJIlNXIjogWyJTVzAxIiwgIlNXMDQiXQ0KfQ0K"
                            ,ApplicationReference ="f283b3ec-27da-42a1-acea-f3f70e75bbdc"
                            ,SdkInterface = SdkInterface.BOTH                           
                            ,EphemeralPublicKey = 
                                    JsonDoc.Parse("{" +
                                                        "\"kty\":\"EC\"," +
                                                        "\"crv\":\"P-256\"," +
                                                        "\"x\":\"WWcpTjbOqiu_1aODllw5rYTq5oLXE_T0huCPjMIRbkI\",\"y\":\"Wz_7anIeadV8SJZUfr4drwjzuWoUbOsHp5GdRZBAAiw\"" +
                                                    "}"
                                    )                            
                            ,MaximumTimeout = 50
                            ,ReferenceNumber = "3DS_LOA_SDK_PPFU_020100_00007"
                            ,SdkTransReference = "b2385523-a66c-4907-ac3c-91848e8c0067" };
            mobileData.SetSdkUiTypes(SdkUiType.OOB);
        }

        #region Check Enrollment Challenge Required V2

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2() {
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());
            Assert.IsFalse(secureEcom.ChallengeMandated);
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_AllEntryModes() {
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());
            Assert.IsFalse(secureEcom.ChallengeMandated);
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_WithIdempotencyKey() {
            var idempotencyKey = Guid.NewGuid().ToString();

            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .WithIdempotencyKey(idempotencyKey)
                .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());
            Assert.IsFalse(secureEcom.ChallengeMandated);

            var exceptionCaught = false;
            try {
                Secure3dService.CheckEnrollment(card)
                    .WithCurrency(Currency)
                    .WithAmount(Amount)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.IsTrue(
                    ex.Message.StartsWith("Status Code: Conflict - Idempotency Key seen before: id=" + secureEcom.ServerTransactionId));
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_WithTokenizedCard() {
            var tokenizedCard = new CreditCardData { Token = card.Tokenize(), CardHolderName = "Jason Mason" };

            Assert.IsNotNull(tokenizedCard.Token);

            var secureEcom = Secure3dService.CheckEnrollment(tokenizedCard)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());
            Assert.IsFalse(secureEcom.ChallengeMandated);
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_AllPreferenceValues() {
            foreach (ChallengeRequestIndicator preference in Enum.GetValues(typeof(ChallengeRequestIndicator))) {
                var secureEcom = Secure3dService.CheckEnrollment(card)
                    .WithCurrency(Currency)
                    .WithAmount(Amount)
                    .WithChallengeRequestIndicator(preference)
                    .Execute();

                AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());
                Assert.IsFalse(secureEcom.ChallengeMandated);
            }
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_AllSources() {
            foreach (AuthenticationSource source in Enum.GetValues(typeof(AuthenticationSource))) {
                var secureEcom = Secure3dService.CheckEnrollment(card)
                    .WithCurrency(Currency)
                    .WithAmount(Amount)
                    .WithAuthenticationSource(source)
                    .Execute();

                AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());
                Assert.IsFalse(secureEcom.ChallengeMandated);
            }
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_StoredCredentials() {
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .WithStoredCredential(new StoredCredential {
                    Initiator = StoredCredentialInitiator.CardHolder,
                    Type = StoredCredentialType.Unscheduled,
                    Sequence = StoredCredentialSequence.First,
                    Reason = StoredCredentialReason.NoShow
                })
                .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());
            Assert.IsFalse(secureEcom.ChallengeMandated);
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_WithNullPaymentMethod() {
            card = new CreditCardData();

            var exceptionCaught = false;
            try {
                Secure3dService.CheckEnrollment(card)
                    .WithCurrency(Currency)
                    .WithAmount(Amount)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40007", ex.ResponseMessage);
                Assert.AreEqual(
                    "Status Code: BadRequest - Request expects the following conditionally mandatory fields number,expiry_month,expiry_year.",
                    ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        #endregion

        #region Check Enrollment Frictionless V2

        [TestMethod]
        public void CardHolderEnrolled_Frictionless_v2() {
            card.Number = GpApi3DSTestCards.CARD_AUTH_SUCCESSFUL_V2_1;

            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());
            Assert.IsFalse(secureEcom.ChallengeMandated);
        }

        [TestMethod]
        public void CardHolderEnrolled_Frictionless_v2_WithIdempotencyKey() {
            card.Number = GpApi3DSTestCards.CARD_AUTH_SUCCESSFUL_V2_1;
            var idempotencyKey = Guid.NewGuid().ToString();

            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .WithIdempotencyKey(idempotencyKey)
                .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());
            Assert.IsFalse(secureEcom.ChallengeMandated);

            var exceptionCaught = false;
            try {
                Secure3dService.CheckEnrollment(card)
                    .WithCurrency(Currency)
                    .WithAmount(Amount)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.IsTrue(
                    ex.Message.StartsWith("Status Code: Conflict - Idempotency Key seen before: id=" + secureEcom.ServerTransactionId));
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardHolderEnrolled_Frictionless_v2_WithTokenizedCard() {
            card.Number = GpApi3DSTestCards.CARD_AUTH_SUCCESSFUL_V2_1;
            var tokenizedCard = new CreditCardData { Token = card.Tokenize(), CardHolderName = "Jason Mason" };

            Assert.IsNotNull(tokenizedCard.Token);

            var secureEcom = Secure3dService.CheckEnrollment(tokenizedCard)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());
            Assert.IsFalse(secureEcom.ChallengeMandated);
        }

        [TestMethod]
        public void CardHolderEnrolled_Frictionless_v2_AllPreferenceValues() {
            card.Number = GpApi3DSTestCards.CARD_AUTH_SUCCESSFUL_V2_1;

            foreach (ChallengeRequestIndicator preference in Enum.GetValues(typeof(ChallengeRequestIndicator))) {
                var secureEcom = Secure3dService.CheckEnrollment(card)
                    .WithCurrency(Currency)
                    .WithAmount(Amount)
                    .WithChallengeRequestIndicator(preference)
                    .Execute();

                AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());
                Assert.IsFalse(secureEcom.ChallengeMandated);
            }
        }

        [TestMethod]
        public void CardHolderEnrolled_Frictionless_v2_StoredCredentials() {
            card.Number = GpApi3DSTestCards.CARD_AUTH_SUCCESSFUL_V2_1;

            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .WithStoredCredential(new StoredCredential {
                    Initiator = StoredCredentialInitiator.CardHolder,
                    Type = StoredCredentialType.Unscheduled,
                    Sequence = StoredCredentialSequence.First,
                    Reason = StoredCredentialReason.NoShow
                })
                .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());
            Assert.IsFalse(secureEcom.ChallengeMandated);
        }
       

        #endregion

        #region Initiate Challenge Required V2

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_Initiate() {
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());

            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .Execute();

            AssertThreeDSResponse(initAuth, Secure3dStatus.CHALLENGE_REQUIRED.ToString());
            Assert.IsTrue(initAuth.ChallengeMandated);
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_Initiate_With_IdempotencyKey() {
            var idempotencyKey = Guid.NewGuid().ToString();

            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());

            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithIdempotencyKey(idempotencyKey)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .Execute();

            AssertThreeDSResponse(initAuth, Secure3dStatus.CHALLENGE_REQUIRED.ToString());
            Assert.IsTrue(initAuth.ChallengeMandated);

            var exceptionCaught = false;
            try {
                Secure3dService.InitiateAuthentication(card, secureEcom)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithAuthenticationSource(AuthenticationSource.BROWSER)
                    .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                    .WithIdempotencyKey(idempotencyKey)
                    .WithOrderCreateDate(DateTime.Now)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithBrowserData(browserData)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.IsTrue(
                    ex.Message.StartsWith("Status Code: Conflict - Idempotency Key seen before: id=" + secureEcom.ServerTransactionId));
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_Initiate_TokenizedCard() {
            var tokenizedCard = new CreditCardData { Token = card.Tokenize(), CardHolderName = "Jason Mason" };

            Assert.IsNotNull(tokenizedCard.Token);

            var secureEcom = Secure3dService.CheckEnrollment(tokenizedCard)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());

            var initAuth = Secure3dService.InitiateAuthentication(tokenizedCard, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .Execute();

            AssertThreeDSResponse(initAuth, Secure3dStatus.CHALLENGE_REQUIRED.ToString());
            Assert.IsTrue(initAuth.ChallengeMandated);
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_Initiate_MethodUrl() {
            foreach (MethodUrlCompletion methodUrlCompletion in Enum.GetValues(typeof(MethodUrlCompletion))) {
                var secureEcom = Secure3dService.CheckEnrollment(card)
                    .WithCurrency(Currency)
                    .WithAmount(Amount)
                    .Execute();

                AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());

                var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithAuthenticationSource(AuthenticationSource.BROWSER)
                    .WithMethodUrlCompletion(methodUrlCompletion)
                    .WithOrderCreateDate(DateTime.Now)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithBrowserData(browserData)
                    .Execute();

                AssertThreeDSResponse(initAuth, Secure3dStatus.CHALLENGE_REQUIRED.ToString());
                Assert.IsTrue(initAuth.ChallengeMandated);
            }
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_Initiate_Without_ShippingAddress() {
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());

            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithOrderCreateDate(DateTime.Now)
                .WithBrowserData(browserData)
                .Execute();

            AssertThreeDSResponse(initAuth, Secure3dStatus.CHALLENGE_REQUIRED.ToString());
            Assert.IsTrue(initAuth.ChallengeMandated);
        }

        [DataTestMethod]
        [DataRow(AuthenticationSource.BROWSER)]
        //[DataRow(AuthenticationSource.MERCHANT_INITIATED)]
        //[DataRow(AuthenticationSource.MOBILE_SDK)]
        public void CardHolderEnrolled_ChallengeRequired_v2_Initiate_AllSources(AuthenticationSource source) {
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());

            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(source)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .Execute();

            AssertThreeDSResponse(initAuth, Secure3dStatus.CHALLENGE_REQUIRED.ToString());
            Assert.IsTrue(initAuth.ChallengeMandated);
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_Initiate_AllPreferenceValues() {
            foreach (ChallengeRequestIndicator preference in Enum.GetValues(typeof(ChallengeRequestIndicator))) {
                var secureEcom = Secure3dService.CheckEnrollment(card)
                    .WithCurrency(Currency)
                    .WithAmount(Amount)
                    .Execute();

                AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());

                var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithAuthenticationSource(AuthenticationSource.BROWSER)
                    .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                    .WithChallengeRequestIndicator(preference)
                    .WithOrderCreateDate(DateTime.Now)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithBrowserData(browserData)
                    .Execute();

                AssertThreeDSResponse(initAuth, Secure3dStatus.CHALLENGE_REQUIRED.ToString());
                Assert.IsTrue(initAuth.ChallengeMandated);
            }
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_Initiate_WithGiftCard() {
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());

            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .WithGiftCardAmount(Amount)
                .WithGiftCardCount(1)
                .WithGiftCardCurrency(Currency)
                .Execute();

            AssertThreeDSResponse(initAuth, Secure3dStatus.CHALLENGE_REQUIRED.ToString());
            Assert.IsTrue(initAuth.ChallengeMandated);
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_Initiate_WithDeliveryEmail() {
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());

            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .WithDeliveryEmail("asad@mailinator.com")
                .WithDeliveryTimeFrame(DeliveryTimeFrame.ELECTRONIC_DELIVERY)
                .Execute();

            AssertThreeDSResponse(initAuth, Secure3dStatus.CHALLENGE_REQUIRED.ToString());
            Assert.IsTrue(initAuth.ChallengeMandated);
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_Initiate_WithShippingMethod() {
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());

            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .WithShippingMethod(ShippingMethod.DIGITAL_GOODS)
                .WithShippingNameMatchesCardHolderName(true)
                .WithShippingAddressCreateDate(DateTime.Now)
                .WithShippingAddressUsageIndicator(AgeIndicator.LESS_THAN_THIRTY_DAYS)
                .Execute();

            AssertThreeDSResponse(initAuth, Secure3dStatus.CHALLENGE_REQUIRED.ToString());
            Assert.IsTrue(initAuth.ChallengeMandated);
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_Initiate_Without_PaymentMethod() {
            var secureEcom = new ThreeDSecure {
                ServerTransactionId = "AUT_" + Guid.NewGuid().ToString().Replace("-", "")
            };

            var exceptionCaught = false;
            try {
                card = new CreditCardData();
                Secure3dService.InitiateAuthentication(card, secureEcom)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithOrderCreateDate(DateTime.Now)
                    .WithBrowserData(browserData)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("40005", ex.ResponseMessage);
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual(
                    "Status Code: BadRequest - Request expects the following fields number", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_Initiate_NonExistentId() {
            var secureEcom = new ThreeDSecure {
                ServerTransactionId = Guid.NewGuid().ToString()
            };

            var exceptionCaught = false;
            try {
                Secure3dService.InitiateAuthentication(card, secureEcom)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithOrderCreateDate(DateTime.Now)
                    .WithBrowserData(browserData)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("40118", ex.ResponseMessage);
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual(
                    "Status Code: NotFound - Authentication " + secureEcom.ServerTransactionId +
                    " not found at this location.", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_Initiate_AcsNotPerformed() {
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());

            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .WithShippingMethod(ShippingMethod.DIGITAL_GOODS)
                .WithShippingNameMatchesCardHolderName(true)
                .WithShippingAddressCreateDate(DateTime.Now)
                .WithShippingAddressUsageIndicator(AgeIndicator.LESS_THAN_THIRTY_DAYS)
                .Execute();

            AssertThreeDSResponse(initAuth, Secure3dStatus.CHALLENGE_REQUIRED.ToString());
            Assert.IsTrue(initAuth.ChallengeMandated);

            var getAuthData = Secure3dService.GetAuthenticationData()
                .WithServerTransactionId(initAuth.ServerTransactionId)
                .Execute();

            Assert.AreEqual(Secure3dStatus.CHALLENGE_REQUIRED.ToString(), getAuthData.Status);
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_Initiate_DuplicateInitiate() {
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());

            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .Execute();

            AssertThreeDSResponse(initAuth, Secure3dStatus.CHALLENGE_REQUIRED.ToString());
            Assert.IsTrue(initAuth.ChallengeMandated);

            var exceptionCaught = false;
            try {
                Secure3dService.InitiateAuthentication(card, secureEcom)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithOrderCreateDate(DateTime.Now)
                    .WithBrowserData(browserData)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("50139", ex.ResponseMessage);
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
                Assert.AreEqual(
                    "Status Code: BadGateway - The Authentication Response is invalid, indicates an error occurred or no response was returned. The request should be considered as not authenticated.",
                    ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_Initiate_MobileSDK() {
            card.Number = GpApi3DSTestCards.CARD_CHALLENGE_REQUIRED_V2_2;

            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute(Secure3dVersion.Two);

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());
            Assert.AreEqual("ENROLLED", secureEcom.Enrolled);

            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.MOBILE_SDK)
                .WithMobileData(mobileData)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .Execute(Secure3dVersion.Two);

            AssertThreeDSResponse(initAuth, Secure3dStatus.CHALLENGE_REQUIRED.ToString());
            Assert.IsNotNull(initAuth.PayerAuthenticationRequest);
            Assert.IsNotNull(initAuth.AcsInterface);
            Assert.IsNotNull(initAuth.AcsUiTemplate);
            Assert.IsNotNull(initAuth.AcsReferenceNumber);
            Assert.IsNotNull(initAuth.ProviderServerTransRef);
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_Initiate_MobileDataAndBrowserData() {
            card.Number = GpApi3DSTestCards.CARD_CHALLENGE_REQUIRED_V2_2;

            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute(Secure3dVersion.Two);

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());
            Assert.AreEqual("ENROLLED", secureEcom.Enrolled);

            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.MOBILE_SDK)
                .WithMobileData(mobileData)
                .WithBrowserData(browserData)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)                
                .Execute(Secure3dVersion.Two);

            AssertThreeDSResponse(initAuth, Secure3dStatus.CHALLENGE_REQUIRED.ToString());
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_With_MobileData() {
            card.Number = GpApi3DSTestCards.CARD_AUTH_SUCCESSFUL_V2_1;

            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .WithAuthenticationSource(AuthenticationSource.MOBILE_SDK)
                .Execute(Secure3dVersion.Two);

            Assert.IsNotNull(secureEcom);
            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());
            Assert.AreEqual(Secure3dStatus.ENROLLED.ToString(), secureEcom.Enrolled);
            Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);

            ThreeDSecure initAuth =
                Secure3dService
                        .InitiateAuthentication(card, secureEcom)
                        .WithAmount(Amount)
                        .WithCurrency(Currency)                        
                        .WithAuthenticationSource(AuthenticationSource.MOBILE_SDK)
                        .WithMobileData(mobileData)
                        .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                        .WithOrderCreateDate(DateTime.Now)
                        .WithAddress(shippingAddress, AddressType.Shipping)                        
                        .Execute(Secure3dVersion.Two);

            Assert.IsNotNull(initAuth);            
            Assert.AreEqual(Secure3dStatus.ENROLLED.ToString(), initAuth.Enrolled);
            Assert.AreEqual(Secure3dVersion.Two, initAuth.Version);
            Assert.AreEqual(Secure3dStatus.SUCCESS_AUTHENTICATED.ToString(), initAuth.Status);
            Assert.AreEqual("YES", initAuth.LiabilityShift);
            Assert.IsFalse(initAuth.ChallengeMandated);
            Assert.IsNotNull(initAuth.IssuerAcsUrl);
            Assert.IsNotNull(initAuth.PayerAuthenticationRequest);
            Assert.AreEqual("05", initAuth.Eci);            

            // Get authentication data
            secureEcom =
                    Secure3dService
                            .GetAuthenticationData()
                            .WithServerTransactionId(initAuth.ServerTransactionId)
                            .Execute(Secure3dVersion.Two);            

            card.ThreeDSecure = secureEcom;

            // Create transaction
            Transaction response =
                    card
                            .Charge(Amount)
                            .WithCurrency(Currency)
                            .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), response.ResponseMessage);
        }

        [TestMethod]
        public void CardHolderEnrolled_Frictionless_v2_Initiate_With_MobileDataAndBrowserData() {
            card.Number = GpApi3DSTestCards.CARD_AUTH_SUCCESSFUL_V2_1;

            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .WithAuthenticationSource(AuthenticationSource.MOBILE_SDK)
                .Execute(Secure3dVersion.Two);

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());
            Assert.AreEqual(Secure3dStatus.ENROLLED.ToString(), secureEcom.Enrolled);

            ThreeDSecure initAuth =
                Secure3dService
                        .InitiateAuthentication(card, secureEcom)
                        .WithCurrency(Currency)
                        .WithAmount(Amount)
                        .WithAuthenticationSource(AuthenticationSource.MOBILE_SDK)
                        .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                        .WithOrderCreateDate(DateTime.Now)
                        .WithAddress(shippingAddress, AddressType.Shipping)
                        .WithMobileData(mobileData)
                        .WithBrowserData(browserData)
                        .Execute(Secure3dVersion.Two);

            Assert.IsNotNull(initAuth);
            Assert.AreEqual(Secure3dStatus.ENROLLED.ToString(), initAuth.Enrolled);
            Assert.AreEqual(Secure3dVersion.Two, initAuth.Version);
            Assert.AreEqual(Secure3dStatus.SUCCESS_AUTHENTICATED.ToString(), initAuth.Status);
            Assert.AreEqual("YES", initAuth.LiabilityShift);
            Assert.IsFalse(initAuth.ChallengeMandated);
            Assert.IsNotNull(initAuth.IssuerAcsUrl);
            Assert.IsNotNull(initAuth.PayerAuthenticationRequest);
            Assert.IsNotNull(initAuth.ChallengeReturnUrl);
            Assert.IsNotNull(initAuth.MessageType);
            Assert.IsNotNull(initAuth.SessionDataFieldName);
            Assert.AreEqual("05", initAuth.Eci);
        }

        #endregion

        #region Initiate Frictionless V2

        [TestMethod]
        public void CardHolderEnrolled_Frictionless_v2_Initiate() {
            card.Number = GpApi3DSTestCards.CARD_AUTH_SUCCESSFUL_V2_1;

            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());
            Assert.IsFalse(secureEcom.ChallengeMandated);

            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .Execute();

            Assert.IsNotNull(initAuth);
            Assert.AreEqual(Secure3dStatus.ENROLLED.ToString(), initAuth.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.Two, initAuth.Version);
            Assert.AreEqual(Secure3dStatus.SUCCESS_AUTHENTICATED.ToString(), initAuth.Status);
            Assert.IsFalse(initAuth.ChallengeMandated);
            Assert.IsNotNull(initAuth.IssuerAcsUrl);
            Assert.IsNotNull(initAuth.PayerAuthenticationRequest);
            Assert.IsNotNull(initAuth.ChallengeReturnUrl);
            Assert.IsNotNull(initAuth.MessageType);
            Assert.IsNotNull(initAuth.SessionDataFieldName);
            Assert.AreEqual("05", initAuth.Eci);
        }

        [DataTestMethod]
        [DataRow(ChallengeRequestIndicator.NO_PREFERENCE)]
        [DataRow(ChallengeRequestIndicator.NO_CHALLENGE_REQUESTED)]
        [DataRow(ChallengeRequestIndicator.NO_CHALLENGE_REQUESTED_TRANSACTION_RISK_ANALYSIS_PERFORMED)]
        [DataRow(ChallengeRequestIndicator.NO_CHALLENGE_REQUESTED_DATA_SHARE_ONLY)]
        [DataRow(ChallengeRequestIndicator.NO_CHALLENGE_REQUESTED_SCA_ALREADY_PERFORMED)]
        [DataRow(ChallengeRequestIndicator.NO_CHALLENGE_REQUESTED_WHITELIST)]
        public void CardHolderEnrolled_Frictionless_v2_Initiate_AllPreferenceValues(ChallengeRequestIndicator preference) {
            card.Number = GpApi3DSTestCards.CARD_AUTH_SUCCESSFUL_V2_1;

            var secureEcom = Secure3dService.CheckEnrollment(card)
                    .WithCurrency(Currency)
                    .WithAmount(Amount)
                    .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());
            Assert.IsFalse(secureEcom.ChallengeMandated);

            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithChallengeRequestIndicator(preference)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .Execute();

            Assert.IsNotNull(initAuth);
            Assert.AreEqual(Secure3dStatus.ENROLLED.ToString(), initAuth.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.Two, initAuth.Version);
            Assert.AreEqual(Secure3dStatus.SUCCESS_AUTHENTICATED.ToString(), initAuth.Status);
            Assert.IsFalse(initAuth.ChallengeMandated);
            Assert.IsNotNull(initAuth.IssuerAcsUrl);
            Assert.IsNotNull(initAuth.PayerAuthenticationRequest);
            Assert.IsNotNull(initAuth.ChallengeReturnUrl);
            Assert.IsNotNull(initAuth.MessageType);
            Assert.IsNotNull(initAuth.SessionDataFieldName);
            Assert.AreEqual("05", initAuth.Eci);
        }

        [DataTestMethod]
        [DataRow(ChallengeRequestIndicator.CHALLENGE_PREFERRED)]
        [DataRow(ChallengeRequestIndicator.CHALLENGE_MANDATED)]
        [DataRow(ChallengeRequestIndicator.CHALLENGE_REQUESTED_PROMPT_FOR_WHITELIST)]
        public void CardHolderEnrolled_Frictionless_v2_Initiate_AllPreferenceValues_ChallengeRequired(ChallengeRequestIndicator preference) {
            card.Number = GpApi3DSTestCards.CARD_AUTH_SUCCESSFUL_V2_1;

            var secureEcom = Secure3dService.CheckEnrollment(card)
                    .WithCurrency(Currency)
                    .WithAmount(Amount)
                    .Execute();

            AssertThreeDSResponse(secureEcom, Secure3dStatus.AVAILABLE.ToString());
            Assert.IsFalse(secureEcom.ChallengeMandated);

            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithChallengeRequestIndicator(preference)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .Execute();

            Assert.IsNotNull(initAuth);
            Assert.AreEqual(Secure3dStatus.ENROLLED.ToString(), initAuth.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.Two, initAuth.Version);
            Assert.AreEqual(Secure3dStatus.CHALLENGE_REQUIRED.ToString(), initAuth.Status);
            Assert.IsTrue(initAuth.ChallengeMandated);
        }

        #endregion

        private void AssertThreeDSResponse(ThreeDSecure secureEcom, string status) {
            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(Secure3dStatus.ENROLLED.ToString(), secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);
            Assert.AreEqual(status, secureEcom.Status);
            Assert.IsNotNull(secureEcom.IssuerAcsUrl);
            Assert.IsNotNull(secureEcom.PayerAuthenticationRequest);
            Assert.IsNotNull(secureEcom.ChallengeReturnUrl);
            Assert.IsNotNull(secureEcom.MessageType);
            Assert.IsNotNull(secureEcom.SessionDataFieldName);
            Assert.IsNull(secureEcom.Eci);
        }
    }
}