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
    }
}
