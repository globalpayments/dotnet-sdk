using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static GlobalPayments.Api.Tests.GpApi.GpApi3DSTestCards;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApi3DSecure1Tests : BaseGpApiTests {
        #region Constants

        private const string AUTHENTICATION_SUCCESSFUL = "AUTHENTICATION_SUCCESSFUL";
        private const string CHALLENGE_REQUIRED = "CHALLENGE_REQUIRED";
        private const string ENROLLED = "ENROLLED";
        private const string NOT_ENROLLED = "NOT_ENROLLED";

        #endregion

        private static CreditCardData card;
        private const string Currency = "GBP";
        private static readonly decimal Amount = new decimal(10.01);

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "P3LRVjtGRGxWQQJDE345mSkEh2KfdAyg",
                AppKey = "ockJr6pv6KFoGiZA",
                Country = "GB",
                ChallengeNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                MethodNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
            });
        }

        public GpApi3DSecure1Tests() {
            card = new CreditCardData {
                Number = CARDHOLDER_ENROLLED_V1,
                ExpMonth = 12,
                ExpYear = 2025,
                CardHolderName = "John Smith"
            };
        }

        #region Check Enrollment

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v1() {
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .Execute();

            AssertThreeDSResponse(secureEcom, CHALLENGE_REQUIRED);
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v1_WithIdempotencyKey() {
            var idempotencyKey = Guid.NewGuid().ToString();

            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithIdempotencyKey(idempotencyKey)
                .Execute();

            AssertThreeDSResponse(secureEcom, CHALLENGE_REQUIRED);

            var exceptionCaught = false;
            try {
                Secure3dService.CheckEnrollment(card)
                    .WithCurrency(Currency)
                    .WithAmount(Amount)
                    .WithAuthenticationSource(AuthenticationSource.BROWSER)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual(
                    "Status Code: Conflict - Idempotency Key seen before: id=" + secureEcom.ServerTransactionId +
                    ", status=CHALLENGE_REQUIRED", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v1_TokenizedCard() {
            var tokenizedCard = new CreditCardData() {
                Token = card.Tokenize()
            };

            Assert.IsNotNull(tokenizedCard.Token);

            var secureEcom = Secure3dService.CheckEnrollment(tokenizedCard)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            AssertThreeDSResponse(secureEcom, CHALLENGE_REQUIRED);
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v1_AllPreferenceValues() {
            foreach (ChallengeRequestIndicator preference in Enum.GetValues(typeof(ChallengeRequestIndicator))) {
                var secureEcom = Secure3dService.CheckEnrollment(card)
                    .WithCurrency(Currency)
                    .WithAmount(Amount)
                    .WithAuthenticationSource(AuthenticationSource.BROWSER)
                    .WithChallengeRequestIndicator(preference)
                    .Execute();

                AssertThreeDSResponse(secureEcom, CHALLENGE_REQUIRED);
            }
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v1_AllSourceValues() {
            foreach (AuthenticationSource source in Enum.GetValues(typeof(AuthenticationSource))) {
                var secureEcom = Secure3dService.CheckEnrollment(card)
                    .WithCurrency(Currency)
                    .WithAmount(Amount)
                    .WithAuthenticationSource(source)
                    .Execute();

                AssertThreeDSResponse(secureEcom, CHALLENGE_REQUIRED);
            }
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v1_StoredCredential() {
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithStoredCredential(new StoredCredential {
                    Initiator = StoredCredentialInitiator.CardHolder,
                    Type = StoredCredentialType.Unscheduled,
                    Sequence = StoredCredentialSequence.First,
                    Reason = StoredCredentialReason.NoShow
                })
                .Execute();

            AssertThreeDSResponse(secureEcom, CHALLENGE_REQUIRED);
        }

        [TestMethod]
        [Ignore]
        public void CardHolderEnrolled_ChallengeRequired_v1_Refund() {
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .WithTransactionType(TransactionType.Refund)
                .Execute();

            AssertThreeDSResponse(secureEcom, CHALLENGE_REQUIRED);

            var saleSecureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .WithTransactionType(TransactionType.Sale)
                .Execute();

            AssertThreeDSResponse(saleSecureEcom, CHALLENGE_REQUIRED);

            Assert.AreNotSame(secureEcom, saleSecureEcom);
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v1_WithNullPaymentMethod() {
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

        [TestMethod]
        public void CardHolderNotEnrolled_v1() {
            card.Number = CARDHOLDER_NOT_ENROLLED_V1;

            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(NOT_ENROLLED, secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.One, secureEcom.Version);
            Assert.AreEqual(NOT_ENROLLED, secureEcom.Status);
            Assert.IsNotNull(secureEcom.IssuerAcsUrl);
            Assert.IsNotNull(secureEcom.PayerAuthenticationRequest);
            Assert.IsNotNull(secureEcom.ChallengeReturnUrl);
            Assert.IsNotNull(secureEcom.MessageType);
            Assert.IsNotNull(secureEcom.SessionDataFieldName);
            Assert.AreEqual(6, secureEcom.Eci);
        }

        #endregion

        #region PostResult

        [TestMethod]
        public void CardHolderEnrolled_v1_PostResult() {
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .Execute();

            AssertThreeDSResponse(secureEcom, CHALLENGE_REQUIRED);

            // Perform ACS authetication
            var acsClient = new GpApi3DSecureAcsClient(secureEcom.IssuerAcsUrl);
            string payerAuthenticationResponse;
            string authResponse = acsClient.Authenticate_v1(secureEcom, out payerAuthenticationResponse);
            Assert.AreEqual("{\"success\":true}", authResponse);

            // Get authentication data
            secureEcom = Secure3dService.GetAuthenticationData()
                .WithServerTransactionId(secureEcom.ServerTransactionId)
                .WithPayerAuthenticationResponse(payerAuthenticationResponse)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(AUTHENTICATION_SUCCESSFUL, secureEcom.Status);
        }

        [TestMethod]
        public void CardHolderEnrolled_v1_PostResult_WithIdempotencyKey() {
            var idempotencyKey = Guid.NewGuid().ToString();

            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .Execute();

            AssertThreeDSResponse(secureEcom, CHALLENGE_REQUIRED);

            // Perform ACS authetication
            var acsClient = new GpApi3DSecureAcsClient(secureEcom.IssuerAcsUrl);
            string payerAuthenticationResponse;
            string authResponse = acsClient.Authenticate_v1(secureEcom, out payerAuthenticationResponse);
            Assert.AreEqual("{\"success\":true}", authResponse);

            // Get authentication data
            secureEcom = Secure3dService.GetAuthenticationData()
                .WithServerTransactionId(secureEcom.ServerTransactionId)
                .WithPayerAuthenticationResponse(payerAuthenticationResponse)
                .WithIdempotencyKey(idempotencyKey)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(AUTHENTICATION_SUCCESSFUL, secureEcom.Status);

            var exceptionCaught = false;
            try {
                secureEcom = Secure3dService.GetAuthenticationData()
                    .WithServerTransactionId(secureEcom.ServerTransactionId)
                    .WithPayerAuthenticationResponse(payerAuthenticationResponse)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual(
                    "Status Code: Conflict - Idempotency Key seen before: id=" + secureEcom.ServerTransactionId +
                    ", status=AUTHENTICATION_SUCCESSFUL", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardHolderEnrolled_v1_PostResult_NonExistentId() {
            var transId = Guid.NewGuid().ToString();
            var exceptionCaught = false;
            try {
                Secure3dService.GetAuthenticationData()
                    .WithServerTransactionId(transId)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("40118", ex.ResponseMessage);
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual(
                    "Status Code: NotFound - Authentication " + transId + " not found at this location.", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardHolderEnrolled_v1_PostResult_AcsNotPerformed() {
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .Execute();

            AssertThreeDSResponse(secureEcom, CHALLENGE_REQUIRED);

            var exceptionCaught = false;
            try {
                Secure3dService.GetAuthenticationData()
                    .WithServerTransactionId(secureEcom.ServerTransactionId)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("50027", ex.ResponseMessage);
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual(
                    "Status Code: BadRequest - Undefined element in Message before PARes", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardHolderNotEnrolled_v1_PostResult() {
            card.Number = CARDHOLDER_NOT_ENROLLED_V1;

            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(NOT_ENROLLED, secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.One, secureEcom.Version);
            Assert.AreEqual(NOT_ENROLLED, secureEcom.Status);
            Assert.IsNotNull(secureEcom.IssuerAcsUrl);
            Assert.IsNotNull(secureEcom.PayerAuthenticationRequest);
            Assert.IsNotNull(secureEcom.ChallengeReturnUrl);
            Assert.IsNotNull(secureEcom.MessageType);
            Assert.IsNotNull(secureEcom.SessionDataFieldName);
            Assert.AreEqual(6, secureEcom.Eci);

            var exceptionCaught = false;
            try {
                Secure3dService.GetAuthenticationData()
                    .WithServerTransactionId(secureEcom.ServerTransactionId)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("50027", ex.ResponseMessage);
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual(
                    "Status Code: BadRequest - Undefined element in Message before PARes", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        #endregion

        private void AssertThreeDSResponse(ThreeDSecure secureEcom, string status) {
            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(ENROLLED, secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.One, secureEcom.Version);
            Assert.AreEqual(status, secureEcom.Status);
            Assert.IsTrue(secureEcom.ChallengeMandated);
            Assert.IsNotNull(secureEcom.IssuerAcsUrl);
            Assert.IsNotNull(secureEcom.PayerAuthenticationRequest);
            Assert.IsNotNull(secureEcom.ChallengeReturnUrl);
            Assert.IsNotNull(secureEcom.MessageType);
            Assert.IsNotNull(secureEcom.SessionDataFieldName);
            Assert.IsNull(secureEcom.Eci);
        }
    }
}