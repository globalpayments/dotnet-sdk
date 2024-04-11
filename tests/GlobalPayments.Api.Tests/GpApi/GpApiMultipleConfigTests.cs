using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiMultipleConfigTests : BaseGpApiReportingTest {
        private const string ConfigName1 = "config1";
        private const string ConfigName2 = "config2";

        [TestMethod]
        public void MultipleConfig() {
            ServicesContainer.ConfigureService(
                new GpApiConfig {
                    AppId = AppId,
                    AppKey = AppKey
                },
                ConfigName1
            );
            ServicesContainer.ConfigureService(
                new GpApiConfig {
                    AppId = "AzcKJwI7SzGGtd9IXCEir5VFPZ6kU8kH",
                    AppKey = "xv1bZxbRxFQtzhAo"
                },
                ConfigName2
            );

            var oldestTransactionForConfig1 = ReportingService.FindTransactionsPaged(1, 1)
                .OrderBy(TransactionSortProperty.TimeCreated)
                .Execute(ConfigName1);
            var oldestTransactionForConfig2 = ReportingService.FindTransactionsPaged(1, 1)
                .OrderBy(TransactionSortProperty.TimeCreated)
                .Execute(ConfigName2);
            Assert.IsTrue(oldestTransactionForConfig1.Results.Count == 1);
            Assert.IsTrue(oldestTransactionForConfig2.Results.Count == 1);
            Assert.AreNotEqual(
                oldestTransactionForConfig1.Results[0].MerchantId,
                oldestTransactionForConfig2.Results[0].MerchantId
            );
        }
    }
}