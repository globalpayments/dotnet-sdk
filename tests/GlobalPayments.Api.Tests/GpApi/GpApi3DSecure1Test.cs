using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static GlobalPayments.Api.Tests.GpApi.GpApi3DSTestCards;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApi3DSecure1Test : BaseGpApiTests {
        
        private static CreditCardData card;
        private const string Currency = "GBP";
        private static readonly decimal Amount = new decimal(10.01);
        private const string NOT_ENROLLED = "NOT_ENROLLED";

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);
        }       

        public GpApi3DSecure1Test() {
            card = new CreditCardData {
                Number = CARDHOLDER_ENROLLED_V1,
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                CardHolderName = "John Smith"
            };
        }

        [TestMethod]
        public void CheckEnrollment_V1() {           
            var exceptionCaught = false;
            try {
                Secure3dService.CheckEnrollment(card)
                    .WithCurrency(Currency)
                    .WithAmount(Amount)
                    .Execute(Secure3dVersion.One);
            } catch (BuilderException ex) {
                exceptionCaught = true;
                Assert.AreEqual("3D Secure One is no longer supported!", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardHolderEnrolled_v1() {
            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            AssertThreeDsResponse(secureEcom);
        }

        [TestMethod]
        public void CardHolderNotEnrolled_v1() {
            card.Number = CARDHOLDER_NOT_ENROLLED_V1;

            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .Execute();

            AssertThreeDsResponse(secureEcom);
        }

        [TestMethod]
        public void GetAuthenticationData_V1() {
            var exceptionCaught = false;
            try {
                Secure3dService.GetAuthenticationData()
                    .Execute(Secure3dVersion.One);
            } catch (BuilderException ex) {
                exceptionCaught = true;
                Assert.AreEqual("3D Secure One is no longer supported!", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        private static void AssertThreeDsResponse(ThreeDSecure secureEcom) {
            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(NOT_ENROLLED, secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(NOT_ENROLLED, secureEcom.Status);
            Assert.IsNotNull(secureEcom.IssuerAcsUrl);
            Assert.IsNotNull(secureEcom.PayerAuthenticationRequest);
            Assert.IsNotNull(secureEcom.ChallengeReturnUrl);
            Assert.IsNotNull(secureEcom.MessageType);
            Assert.IsNotNull(secureEcom.SessionDataFieldName);
            Assert.IsNull(secureEcom.Eci);
            Assert.AreEqual("NO",secureEcom.LiabilityShift);
        }

    }
}