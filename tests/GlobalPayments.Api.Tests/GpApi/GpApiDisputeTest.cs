using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiDisputeTest : BaseGpApiTests {
        private DisputeSummary dispute;
        private List<DisputeDocument> document;
        private List<DisputeDocument> documents;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);
        }

        [TestInitialize]
        public void TestInitialize() {
            dispute = new DisputeSummary {
                CaseId = "DIS_SAND_abcd1234"
            };
            
            document = new List<DisputeDocument> {
                new DisputeDocument{
                    Type = "SALES_RECEIPT",
                    Base64Content = "R0lGODlhigPCAXAAACwAAAAAigPCAYf///8AQnv",
                    }
            };
            
            documents = new List<DisputeDocument> {
                new DisputeDocument {
                    Type = "SALES_RECEIPT",
                    Base64Content = "R0lGODlhigPCAXAAACwAAAAAigPCAYf///8AQnv",
                },
                new DisputeDocument {
                    Type = "TERMS_AND_CONDITIONS",
                    Base64Content = "R0lGODlhigPCAXAAACwAAAAAigPCAYf///8AQnv",
                }
            };
        }

        [TestMethod]
        public void DisputeAccept() {
            var response = dispute.Accept()
                .Execute();
            
            AssertDisputeResponse(response, DisputeStatus.Closed);
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
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void DisputeChallenge() {
            var response = dispute.Challenge(document)
                .Execute();
            
            AssertDisputeResponse(response, DisputeStatus.Closed);
        }

        [TestMethod]
        public void DisputeChallenge_MultipleDocuments() {
            dispute.CaseId = "DIS_SAND_abcd1241";

            var response = dispute.Challenge(documents)
                .Execute();
            
            AssertDisputeResponse(response, DisputeStatus.UnderReview);
        }

        [TestMethod]
        public void DisputeChallenge_MultipleDocuments_ClosedStatus() {
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
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void DisputeChallenge_MissingOptional_Type() {
            var response = dispute.Challenge(document)
                .Execute();
            
            AssertDisputeResponse(response, DisputeStatus.Closed);
        }

        [TestMethod]
        public void DisputeChallengeWrongId() {
            dispute = new DisputeSummary {
                CaseId = "DIS_SAND_bbbb1111"
            };

            var exceptionCaught = false;
            try {
                dispute.Challenge(document)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_DISPUTE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40060", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - 117," +
                                "Unable to challenge for that id. Please check the Case id again.", ex.Message);
            } finally {
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
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        private void AssertDisputeResponse(Transaction transaction, DisputeStatus disputeStatus) {
            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual(GetMapping(disputeStatus), transaction.ResponseMessage);
        }
    }
}