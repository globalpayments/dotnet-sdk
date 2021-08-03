using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void DisputeAcceptWrongId() {
            dispute = new DisputeSummary {
                CaseId = "DIS_SAND_bbbb1111"
            };
            var exceptionCaught = false;
            try {
                dispute.Accept()
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_DISPUTE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40067", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - 124," +
                                "Unable to accept for that id. Please check the Case id again.", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
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
        public void DisputeChallenge_MultipleDocuments() {
            dispute.CaseId = "DIS_SAND_abcd1241";

            var documents = new List<DisputeDocument>();
            documents.Add(new DisputeDocument {
                Type = "SALES_RECEIPT",
                Base64Content = "R0lGODlhigPCAXAAACwAAAAAigPCAYf///8AQnv",
            });
            documents.Add(new DisputeDocument {
                Type = "TERMS_AND_CONDITIONS",
                Base64Content = "R0lGODlhigPCAXAAACwAAAAAigPCAYf///8AQnv",
            });

            var response = dispute.Challenge(documents)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(DisputeStatus.UnderReview), response?.ResponseMessage);
        }

        [TestMethod]
        public void DisputeChallenge_MultipleDocuments_ClosedStatus() {
            var documents = new List<DisputeDocument>();
            documents.Add(new DisputeDocument {
                Type = "SALES_RECEIPT",
                Base64Content = "R0lGODlhigPCAXAAACwAAAAAigPCAYf///8AQnv",
            });
            documents.Add(new DisputeDocument {
                Type = "TERMS_AND_CONDITIONS",
                Base64Content = "R0lGODlhigPCAXAAACwAAAAAigPCAYf///8AQnv",
            });

            var exceptionCaught = false;
            try {
                dispute.Challenge(documents)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40072", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - 131," +
                                "The dispute stage, Retrieval, can be challenged with a single document only. Please correct the request and resubmit",
                    ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void DisputeChallenge_MissingOptional_Type() {
            var documents = new List<DisputeDocument>();
            documents.Add(new DisputeDocument {
                Base64Content = "R0lGODlhigPCAXAAACwAAAAAigPCAYf///8AQnv",
            });

            var response = dispute.Challenge(documents)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(DisputeStatus.Closed), response?.ResponseMessage);
        }

        [TestMethod]
        public void DisputeChallengeWrongId() {
            dispute = new DisputeSummary {
                CaseId = "DIS_SAND_bbbb1111"
            };
            var documents = new List<DisputeDocument>();
            documents.Add(new DisputeDocument {
                Type = "SALES_RECEIPT",
                Base64Content = "R0lGODlhigPCAXAAACwAAAAAigPCAYf///8AQnv",
            });

            var exceptionCaught = false;
            try {
                dispute.Challenge(documents)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_DISPUTE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40060", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - 117," +
                                "Unable to challenge for that id. Please check the Case id again.", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void DisputeChallenge_MissingDocument() {
            var exceptionCaught = false;

            try {
                dispute.Challenge(new List<DisputeDocument>())
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("40065", ex.ResponseMessage);
                Assert.AreEqual(
                    "Status Code: BadRequest - Unable to challenge as No document provided with the request",
                    ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
    }
}