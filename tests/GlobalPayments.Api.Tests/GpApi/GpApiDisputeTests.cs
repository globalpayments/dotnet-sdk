using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiDisputeTests : BaseGpApiTests {
        DisputeSummary dispute;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
            });
        }

        [TestInitialize]
        public void TestInitialize() {
            dispute = new DisputeSummary {
                CaseId = "DIS_SAND_abcd1234"
            };
        }

        [TestMethod]
        public void DisputeAccept() {
            var response = dispute.Accept()
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(DisputeStatus.Closed), response?.ResponseMessage);
        }

        [TestMethod]
        public void DisputeAcceptWrongId()
        {
            dispute = new DisputeSummary
            {
                CaseId = "DIS_SAND_bbbb1111"
            };
            try
            {
                dispute.Accept()
                .Execute();
            } catch(GatewayException ex)
            {
                Assert.AreEqual("INVALID_DISPUTE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40067", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - 124," +
                    "Unable to accept for that id. Please check the Case id again.", ex.Message);
            }
        }

        [TestMethod]
        public void DisputeChallenge() {
            var documents = new List<DisputeDocument>();
            documents.Add(new DisputeDocument {
                Type = "SALES_RECEIPT",
                Base64Content = "R0lGODlhigPCAXAAACwAAAAAigPCAYf///8AQnv",
            });
            
            var response = dispute.Challenge(documents)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(DisputeStatus.Closed), response?.ResponseMessage);
        }

        [TestMethod]
        public void DisputeChallengeWrongId()
        {
            dispute = new DisputeSummary
            {
                CaseId = "DIS_SAND_bbbb1111"
            };
            var documents = new List<DisputeDocument>();
            documents.Add(new DisputeDocument
            {
                Type = "SALES_RECEIPT",
                Base64Content = "R0lGODlhigPCAXAAACwAAAAAigPCAYf///8AQnv",
            });

            try
            {
                dispute.Challenge(documents)
                .Execute();
            }
            catch (GatewayException ex)
            {
                Assert.AreEqual("INVALID_DISPUTE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40060", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - 117," +
                    "Unable to challenge for that id. Please check the Case id again.", ex.Message);
            }
        }
    }
}
