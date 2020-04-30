using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.TransIT {
    [TestClass]
    public class TransitAuthenticationTests {
        public TransitAuthenticationTests() {
            //ServicesContainer.ConfigureService(new TransitConfig {
            //    DeviceId = "88700000322601",
            //    MerchantId = "887000003226",
            //    TransactionKey = "H1O8QTS2JVA9OFMQ3FGEH6D5E28X1KS9"
            //});
        }

        [TestMethod]
        public void GenerateKey_Manual() {
            // TODO: I have verified this works and am awaiting for guidance on how to handle the transaction keys before finalizing this piece
            string transactionKey = TransitService.GenerateTransactionKey(
                Environment.TEST,
                "887000003282",
                "TA5654114",
                "TransitCert2020!"
                //"3RDHBVQOYIGTC9WQ32CPBFMPGX5OTWN3"
            );
            Assert.IsNotNull(transactionKey);
        }
    }
}
