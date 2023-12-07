using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApi3DSecureTest : BaseGpApiTests {
        #region Constants

        private const string AVAILABLE = "AVAILABLE";
        private const string FAILED = "FAILED";
        private const string NOT_AUTHENTICATED = "NOT_AUTHENTICATED";
        private const string SUCCESS_ATTEMPT_MADE = "SUCCESS_ATTEMPT_MADE";
        private const string CHALLENGE_REQUIRED = "CHALLENGE_REQUIRED";
        private const string ENROLLED = "ENROLLED";
        private const string NOT_ENROLLED = "NOT_ENROLLED";
        private const string SUCCESS_AUTHENTICATED = "SUCCESS_AUTHENTICATED";

        #endregion

        private CreditCardData card;
        private Address shippingAddress;
        private BrowserData browserData;

        private const string Currency = "GBP";
        private static readonly decimal Amount = new decimal(10.01);

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);
        }

        public GpApi3DSecureTest() {
            // Create card data
            card = new CreditCardData {
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
        }

        #region 3DS v1 tests

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_AuthenticationSuccessful_FullCycle_v1() {
            card.Number = GpApi3DSTestCards.CARDHOLDER_ENROLLED_V1;

            var errorFound = false;
            try {
                Secure3dService
                    .CheckEnrollment(card)
                    .WithCurrency(Currency)
                    .WithAmount(Amount)
                    .WithAuthenticationSource(AuthenticationSource.BROWSER)
                    .WithChallengeRequestIndicator(ChallengeRequestIndicator.CHALLENGE_MANDATED)
                    .WithStoredCredential(new StoredCredential {
                        Initiator = StoredCredentialInitiator.CardHolder,
                        Type = StoredCredentialType.Unscheduled,
                        Sequence = StoredCredentialSequence.First,
                        Reason = StoredCredentialReason.NoShow
                    })
                    .Execute(Secure3dVersion.One);
            } catch (BuilderException e) {
                errorFound = true;
                Assert.AreEqual("3D Secure One is no longer supported!", e.Message);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_AuthenticationSuccessful_FullCycle_v1_WithTokenizedPaymentMethod() {
            card.Number = GpApi3DSTestCards.CARDHOLDER_ENROLLED_V1;

            // Tokenize payment method
            var tokenizedCard = new CreditCardData {
                Token = card.Tokenize()
            };

            Assert.IsNotNull(tokenizedCard.Token);

            var errorFound = false;
            try {
                Secure3dService
                    .CheckEnrollment(tokenizedCard)
                    .WithCurrency(Currency)
                    .WithAmount(Amount)
                    .Execute(Secure3dVersion.One);
            } catch (BuilderException e) {
                errorFound = true;
                Assert.AreEqual("3D Secure One is no longer supported!", e.Message);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void CardHolderNotEnrolled_v1() {
            card.Number = GpApi3DSTestCards.CARDHOLDER_NOT_ENROLLED_V1;
            var errorFound = false;
            try {
                Secure3dService
                    .CheckEnrollment(card)
                    .WithCurrency(Currency)
                    .WithAmount(Amount)
                    .Execute(Secure3dVersion.One);
            } catch (BuilderException e) {
                errorFound = true;
                Assert.AreEqual("3D Secure One is no longer supported!", e.Message);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        #endregion

        #region 3DS v2 tests

        [DataTestMethod]
        [DataRow(GpApi3DSTestCards.CARD_AUTH_SUCCESSFUL_V2_1)]
        [DataRow(GpApi3DSTestCards.CARD_AUTH_SUCCESSFUL_NO_METHOD_URL_V2_1)]
        [DataRow(GpApi3DSTestCards.CARD_AUTH_SUCCESSFUL_V2_2)]
        [DataRow(GpApi3DSTestCards.CARD_AUTH_SUCCESSFUL_NO_METHOD_URL_V2_2)]
        public void FrictionlessFullCycle_v2(string cardNumber) {
            card.Number = cardNumber;

            // Check enrollment
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(ENROLLED, secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);
            Assert.AreEqual(AVAILABLE, secureEcom.Status);

            // Initiate authentication
            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .Execute();

            Assert.IsNotNull(initAuth);
            Assert.AreEqual(SUCCESS_AUTHENTICATED, initAuth.Status);

            // Get authentication data
            secureEcom = Secure3dService.GetAuthenticationData()
                .WithServerTransactionId(initAuth.ServerTransactionId)
                .Execute();

            Assert.AreEqual(SUCCESS_AUTHENTICATED, secureEcom.Status);
            Assert.AreEqual(secureEcom.LiabilityShift, "YES");

            card.ThreeDSecure = secureEcom;

            // Create transaction
            var response = card.Charge(Amount)
                .WithCurrency(Currency)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void FrictionlessFullCycle_v2_WithTokenizedPaymentMethod() {
            card.Number = GpApi3DSTestCards.CARD_AUTH_SUCCESSFUL_V2_1;

            // Tokenize payment method
            var tokenizedCard = new CreditCardData {
                Token = card.Tokenize()
            };

            Assert.IsNotNull(tokenizedCard.Token);

            // Check enrollment
            var secureEcom = Secure3dService.CheckEnrollment(tokenizedCard)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(ENROLLED, secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);
            Assert.AreEqual(AVAILABLE, secureEcom.Status);

            // Initiate authentication
            var initAuth = Secure3dService.InitiateAuthentication(tokenizedCard, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .Execute();

            Assert.IsNotNull(initAuth);
            Assert.AreEqual(SUCCESS_AUTHENTICATED, initAuth.Status);

            // Get authentication data
            secureEcom = Secure3dService.GetAuthenticationData()
                .WithServerTransactionId(initAuth.ServerTransactionId)
                .Execute();

            Assert.AreEqual(SUCCESS_AUTHENTICATED, secureEcom.Status);

            tokenizedCard.ThreeDSecure = secureEcom;

            // Create transaction
            var response = tokenizedCard.Charge(Amount)
                .WithCurrency(Currency)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }
        
        [TestMethod]
        public void FrictionlessFullCycle_v2_Verify3DS() {
            card.Number = GpApi3DSTestCards.CARD_AUTH_SUCCESSFUL_V2_1;
            
            // Check enrollment
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(ENROLLED, secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);
            Assert.AreEqual(AVAILABLE, secureEcom.Status);

            // Initiate authentication
            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .Execute();

            Assert.IsNotNull(initAuth);
            Assert.AreEqual(SUCCESS_AUTHENTICATED, initAuth.Status);

            // Get authentication data
            secureEcom = Secure3dService.GetAuthenticationData()
                .WithServerTransactionId(initAuth.ServerTransactionId)
                .Execute();

            Assert.AreEqual(SUCCESS_AUTHENTICATED, secureEcom.Status);

            card.ThreeDSecure = secureEcom;
            
            var verifyResponse = card.Verify()
                .WithCurrency(Currency)
                .Execute();

            Assert.IsNotNull(verifyResponse);
            Assert.AreEqual(Success, verifyResponse.ResponseCode);
            Assert.AreEqual("VERIFIED", verifyResponse.ResponseMessage);

            // Create transaction
            var response = card.Charge(Amount)
                .WithCurrency(Currency)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }
        
        [TestMethod]
        public void FrictionlessFullCycle_v2_Verify3DS_TokenizedCard() {
            card.Number = GpApi3DSTestCards.CARD_AUTH_SUCCESSFUL_V2_1;
            
            // Tokenize payment method
            var tokenizedCard = new CreditCardData {
                Token = card.Tokenize()
            };
            
            Assert.IsNotNull(tokenizedCard.Token);

            // Check enrollment
            var secureEcom = Secure3dService.CheckEnrollment(tokenizedCard)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(ENROLLED, secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);
            Assert.AreEqual(AVAILABLE, secureEcom.Status);

            // Initiate authentication
            var initAuth = Secure3dService.InitiateAuthentication(tokenizedCard, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .Execute();

            Assert.IsNotNull(initAuth);
            Assert.AreEqual(SUCCESS_AUTHENTICATED, initAuth.Status);

            // Get authentication data
            secureEcom = Secure3dService.GetAuthenticationData()
                .WithServerTransactionId(initAuth.ServerTransactionId)
                .Execute();

            Assert.AreEqual(SUCCESS_AUTHENTICATED, secureEcom.Status);

            tokenizedCard.ThreeDSecure = secureEcom;
            
            var verifyResponse = tokenizedCard.Verify()
                .WithCurrency(Currency)
                .Execute();

            Assert.IsNotNull(verifyResponse);
            Assert.AreEqual(Success, verifyResponse.ResponseCode);
            Assert.AreEqual("VERIFIED", verifyResponse.ResponseMessage);

            // Create transaction
            var response = tokenizedCard.Charge(Amount)
                .WithCurrency(Currency)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [DataTestMethod]
        [DataRow(GpApi3DSTestCards.CARD_AUTH_ATTEMPTED_BUT_NOT_SUCCESSFUL_V2_1, SUCCESS_ATTEMPT_MADE)]
        [DataRow(GpApi3DSTestCards.CARD_AUTH_FAILED_V2_1, NOT_AUTHENTICATED)]
        [DataRow(GpApi3DSTestCards.CARD_AUTH_ISSUER_REJECTED_V2_1, FAILED)]
        [DataRow(GpApi3DSTestCards.CARD_AUTH_COULD_NOT_BE_PREFORMED_V2_1, FAILED)]
        [DataRow(GpApi3DSTestCards.CARD_AUTH_ATTEMPTED_BUT_NOT_SUCCESSFUL_V2_2, SUCCESS_ATTEMPT_MADE)]
        [DataRow(GpApi3DSTestCards.CARD_AUTH_FAILED_V2_2, NOT_AUTHENTICATED)]
        [DataRow(GpApi3DSTestCards.CARD_AUTH_ISSUER_REJECTED_V2_2, FAILED)]
        [DataRow(GpApi3DSTestCards.CARD_AUTH_COULD_NOT_BE_PREFORMED_V2_2, FAILED)]
        public void FrictionlessFullCycle_v2_Failed(string cardNumber, string status) {
            card.Number = cardNumber;

            // Check enrollment
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(ENROLLED, secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);
            Assert.AreEqual(AVAILABLE, secureEcom.Status);

            // Initiate authentication
            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .Execute();

            Assert.IsNotNull(initAuth);
            Assert.AreEqual(status, initAuth.Status);

            // Get authentication data
            secureEcom = Secure3dService.GetAuthenticationData()
                .WithServerTransactionId(initAuth.ServerTransactionId)
                .Execute();

            Assert.AreEqual(status, secureEcom.Status);
            string liabilityShift = status == "SUCCESS_ATTEMPT_MADE" ? "YES" : "NO";
            Assert.AreEqual(liabilityShift, secureEcom.LiabilityShift);

            card.ThreeDSecure = secureEcom;

            // Create transaction
            var response = card.Charge(Amount)
                .WithCurrency(Currency)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [DataTestMethod]
        [DataRow(GpApi3DSTestCards.CARD_CHALLENGE_REQUIRED_V2_1)]
        [DataRow(GpApi3DSTestCards.CARD_CHALLENGE_REQUIRED_V2_2)]
        public void CardHolderEnrolled_ChallengeRequired_v2(string cardNumber) {
            card.Number = cardNumber;

            // Check enrollment
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(ENROLLED, secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);
            Assert.AreEqual(AVAILABLE, secureEcom.Status);

            // Initiate authentication
            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .Execute();

            Assert.IsNotNull(initAuth);
            Assert.AreEqual(CHALLENGE_REQUIRED, initAuth.Status);
            Assert.IsTrue(initAuth.ChallengeMandated);
            Assert.IsNotNull(initAuth.IssuerAcsUrl);
            Assert.IsNotNull(initAuth.PayerAuthenticationRequest);

            // Perform ACS authentication
            GpApi3DSecureAcsClient acsClient = new GpApi3DSecureAcsClient(initAuth.IssuerAcsUrl);
            string authResponse = acsClient.Authenticate_v2(initAuth);
            Assert.AreEqual("{\"success\":true}", authResponse);

            // Get authentication data
            secureEcom = Secure3dService.GetAuthenticationData()
                .WithServerTransactionId(initAuth.ServerTransactionId)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(SUCCESS_AUTHENTICATED, secureEcom.Status);
            Assert.AreEqual("YES", secureEcom.LiabilityShift);

            card.ThreeDSecure = secureEcom;

            // Create transaction
            var response = card.Charge(Amount)
                .WithCurrency(Currency)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void FrictionlessFullCycle_v2_DifferentAmount() {
            card.Number = GpApi3DSTestCards.CARD_AUTH_SUCCESSFUL_V2_2;

            // Check enrollment
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(15)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(ENROLLED, secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);
            Assert.AreEqual(AVAILABLE, secureEcom.Status);
            // Assert.AreEqual(1500, secureEcom.Amount);

            // Initiate authentication
            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(10)
                .WithCurrency("EUR")
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .Execute();

            Assert.IsNotNull(initAuth);
            Assert.AreEqual(SUCCESS_AUTHENTICATED, initAuth.Status);
            Assert.AreEqual("YES", secureEcom.LiabilityShift);
            // Assert.AreEqual(1500, secureEcom.Amount);
            // Assert.AreEqual(Currency, secureEcom.Currency);

            // Get authentication data
            secureEcom = Secure3dService.GetAuthenticationData()
                .WithServerTransactionId(initAuth.ServerTransactionId)
                .Execute();

            Assert.AreEqual(SUCCESS_AUTHENTICATED, secureEcom.Status);
            Assert.AreEqual("YES", secureEcom.LiabilityShift);
            // Assert.AreEqual(1500, secureEcom.Amount);
            // Assert.AreEqual(Currency, secureEcom.Currency);

            card.ThreeDSecure = secureEcom;

            // Create transaction
            var response = card.Charge(15)
                .WithCurrency(Currency)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2_DuplicateAcsRequest() {
            card.Number = GpApi3DSTestCards.CARD_CHALLENGE_REQUIRED_V2_1;

            // Check enrollment
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(ENROLLED, secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);
            Assert.AreEqual(AVAILABLE, secureEcom.Status);

            // Initiate authentication
            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .Execute();

            Assert.IsNotNull(initAuth);
            Assert.AreEqual(CHALLENGE_REQUIRED, initAuth.Status);
            Assert.IsTrue(initAuth.ChallengeMandated);
            Assert.IsNotNull(initAuth.IssuerAcsUrl);
            Assert.IsNotNull(initAuth.PayerAuthenticationRequest);

            // Perform ACS authentication
            GpApi3DSecureAcsClient acsClient = new GpApi3DSecureAcsClient(initAuth.IssuerAcsUrl);
            string authResponse = acsClient.Authenticate_v2(initAuth);
            Assert.AreEqual("{\"success\":true}", authResponse);

            GpApi3DSecureAcsClient acsClient2 = new GpApi3DSecureAcsClient(initAuth.IssuerAcsUrl);
            string authResponseSecond = acsClient2.Authenticate_v2(initAuth);
            Assert.AreEqual("{\"success\":true}", authResponseSecond);

            // Get authentication data
            secureEcom = Secure3dService.GetAuthenticationData()
                .WithServerTransactionId(initAuth.ServerTransactionId)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(SUCCESS_AUTHENTICATED, secureEcom.Status);
            Assert.AreEqual("YES", secureEcom.LiabilityShift);

            card.ThreeDSecure = secureEcom;

            // Create transaction
            var response = card.Charge(Amount)
                .WithCurrency(Currency)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        #endregion

        [TestMethod]
        public void DecoupledAuth()
        {
            card.Number = GpApi3DSTestCards.CARD_AUTH_SUCCESSFUL_V2_1;

            var tokenizedCard = new CreditCardData() {
                Token = card.Tokenize()
            };
        
             tokenizedCard.CardHolderName = "James Mason";

            var secureEcom = Secure3dService.CheckEnrollment(tokenizedCard)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .WithDecoupledNotificationUrl("https://www.example.com/decoupledNotification")
                .Execute();

                Assert.IsNotNull(secureEcom);
                Assert.AreEqual(ENROLLED, secureEcom.Enrolled);
                Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);
                Assert.AreEqual(AVAILABLE, secureEcom.Status);

            var initAuth = Secure3dService.InitiateAuthentication(tokenizedCard, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .WithDecoupledFlowRequest(true)
                .WithDecoupledFlowTimeout(9001)
                .WithDecoupledNotificationUrl("https://www.example.com/decoupledNotification")
                .Execute();

            Assert.IsNotNull(initAuth);
            Assert.AreEqual(SUCCESS_AUTHENTICATED, secureEcom.Status);
            Assert.AreEqual("YES", secureEcom.LiabilityShift);

             secureEcom = Secure3dService.GetAuthenticationData()
                .WithServerTransactionId(secureEcom.ServerTransactionId)
                .Execute();

            Assert.AreEqual(SUCCESS_AUTHENTICATED, secureEcom.Status);
            Assert.AreEqual("YES", secureEcom.LiabilityShift);

            tokenizedCard.ThreeDSecure = secureEcom;
            var response = tokenizedCard.Charge(Amount)
                    .WithCurrency(Currency)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), response.ResponseMessage);
        }

        [TestMethod]
        public void ExemptionSaleTransaction() {
            card.Number = GpApi3DSTestCards.CARD_CHALLENGE_REQUIRED_V2_2;

            ThreeDSecure threeDs = new ThreeDSecure {
                ExemptStatus = ExemptStatus.LOW_VALUE
            };
            card.ThreeDSecure = threeDs;

            var response = card.Charge(Amount)
                .WithCurrency(Currency)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), response.ResponseMessage.ToUpper());
        }

        [TestMethod]
        public void ChargeTransaction_WithRandom3DSValues() {
            card.Number = GpApi3DSTestCards.CARD_CHALLENGE_REQUIRED_V2_1;

            ThreeDSecure threeDs = new ThreeDSecure {
                AuthenticationValue = Guid.NewGuid().ToString(),
                DirectoryServerTransactionId = Guid.NewGuid().ToString(),
                ServerTransactionId = Guid.NewGuid().ToString(),
                Eci = Guid.NewGuid().ToString(),
                MessageVersion = Guid.NewGuid().ToString()
            };
            
            card.ThreeDSecure = threeDs;

            var response = card.Charge(Amount)
                    .WithCurrency(Currency)
                    .Execute();
            
            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), response.ResponseMessage);
        }
    }

    /// <summary>
    /// ACS Authentication Simulator result codes
    /// </summary>
    public enum AuthenticationResultCode : int {
        Successful = 0,
        Unavailable = 5,
        AttemptAcknowledge = 7,
        Failed = 9,
    }

    /// <summary>
    /// This 3DS ACS client mocks the ACS Authentication Simulator used for testing purposes
    /// </summary>
    public class GpApi3DSecureAcsClient {
        private string _redirectUrl;

        public GpApi3DSecureAcsClient(string redirectUrl) {
            _redirectUrl = redirectUrl;
        }

        private string SubmitFormData(string formUrl, List<KeyValuePair<string, string>> formData) {
            HttpClient httpClient = new HttpClient() {
                Timeout = TimeSpan.FromMilliseconds(6000)
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, formUrl);
            HttpResponseMessage response = null;
            try {
                request.Content = new FormUrlEncodedContent(formData);
                response = httpClient.SendAsync(request).Result;
                var rawResponse = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode != HttpStatusCode.OK) {
                    throw new Exception(rawResponse);
                }

                return rawResponse;
            }
            catch (Exception) {
                throw;
            }
            finally {
            }
        }

        private string GetFormAction(string rawHtml, string formName) {
            var searchString = $"name=\"{formName}\" action=\"";

            var index = rawHtml.IndexOf(searchString, comparisonType: StringComparison.InvariantCultureIgnoreCase);
            if (index > -1) {
                index = index + searchString.Length;
                var length = rawHtml.IndexOf("\"", index) - index;
                return rawHtml.Substring(index, length);
            }

            return null;
        }

        private string GetInputValue(string rawHtml, string inputName) {
            var searchString = $"name=\"{inputName}\" value=\"";

            var index = rawHtml.IndexOf(searchString, comparisonType: StringComparison.InvariantCultureIgnoreCase);
            if (index > -1) {
                index = index + searchString.Length;
                var length = rawHtml.IndexOf("\"", index) - index;
                return rawHtml.Substring(index, length);
            }

            return null;
        }

        /// <summary>
        /// Performs ACS authentication for 3DS v1
        /// </summary>
        /// <param name="secureEcom"></param>
        /// <param name="paRes"></param>
        /// <param name="authenticationResultCode"></param>
        /// <returns></returns>
        public string Authenticate_v1(ThreeDSecure secureEcom, out string paRes,
            AuthenticationResultCode authenticationResultCode = 0) {
            // Step 1
            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(
                new KeyValuePair<string, string>(secureEcom.MessageType, secureEcom.PayerAuthenticationRequest));
            formData.Add(new KeyValuePair<string, string>(secureEcom.SessionDataFieldName,
                secureEcom.ServerTransactionId));
            formData.Add(new KeyValuePair<string, string>("TermUrl", secureEcom.ChallengeReturnUrl));
            formData.Add(new KeyValuePair<string, string>("AuthenticationResultCode",
                authenticationResultCode.ToString("D")));
            string rawResponse = SubmitFormData(secureEcom.IssuerAcsUrl, formData);

            paRes = GetInputValue(rawResponse, "PaRes");

            // Step 2
            formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("MD", GetInputValue(rawResponse, "MD")));
            formData.Add(new KeyValuePair<string, string>("PaRes", paRes));
            rawResponse = SubmitFormData(GetFormAction(rawResponse, "PAResForm"), formData);

            return rawResponse;
        }

        /// <summary>
        /// Performs ACS authentication for 3DS v2
        /// </summary>
        /// <param name="secureEcom"></param>
        /// <returns></returns>
        public string Authenticate_v2(ThreeDSecure secureEcom) {
            // Step 1
            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>(secureEcom.MessageType, secureEcom.PayerAuthenticationRequest));
            //formData.Add(new KeyValuePair<string, string>(secureEcom.SessionDataFieldName, secureEcom.ServerTransactionId));
            string rawResponse = SubmitFormData(secureEcom.IssuerAcsUrl, formData);

            // Step 2
            formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("get-status-type", "true"));
            do {
                rawResponse = SubmitFormData(secureEcom.IssuerAcsUrl, formData);
                Thread.Sleep(2000);
            } while (rawResponse.Trim() == "IN_PROGRESS");

            // Step 3
            formData = new List<KeyValuePair<string, string>>();
            rawResponse = SubmitFormData(secureEcom.IssuerAcsUrl, formData);

            // Step 4
            formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("cres", GetInputValue(rawResponse, "cres")));
            rawResponse = SubmitFormData(GetFormAction(rawResponse, "ResForm"), formData);

            return rawResponse;
        }
    }
}