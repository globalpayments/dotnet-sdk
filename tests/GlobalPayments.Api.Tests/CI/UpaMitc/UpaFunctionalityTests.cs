using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Tests.Utils;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.CI {
    [TestClass]
    public class UpaFunctionalityTests {
        private const string MitcUpaAppId = "6l8Xr23kHL9tGmAtXUvCEXKskvF7aLGq";
        private const string MitcUpaAppKey = "z0ApiLDfXrKmrlNa";
        private const CiTestingHarness.CacheMode CurrentCacheMode = CiTestingHarness.CacheMode.Locked;

        private static CiTestingHarness _harness;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            _harness = new CiTestingHarness(
                "https://apis.sandbox.globalpay.com/ucp",
                CurrentCacheMode,
                "UpaFunctionalityTests");
        }

        private GpApiConfig GetGpApiConfig() {
            return new GpApiConfig {
                AppId = MitcUpaAppId,
                AppKey = MitcUpaAppKey,
                Channel = Channel.CardPresent,
                Country = "US",
                DeviceCurrency = "USD",
                EnableLogging = true,
                RequestLogger = new RequestConsoleLogger(),
                AccessTokenInfo = new AccessTokenInfo {
                    TransactionProcessingAccountName = "90916726"
                },
                ServiceUrl = _harness.GetTestingUrl()
            };
        }

        private IDeviceInterface CreateDevice(string testKey) {
            var device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.UPA_DEVICE,
                ConnectionMode = ConnectionModes.MIC,
                Timeout = 30000,
                GatewayConfig = GetGpApiConfig(),
                RequestIdProvider = _harness.CreateRequestIdProvider(testKey)
            });
            device.EcrId = "12";
            return device;
        }
        

        private void AssertMitcUpaResponse(ITerminalResponse response) {
            Assert.AreEqual("COMPLETE", response.Status);
            Assert.AreEqual("COMPLETE", response.DeviceResponseText);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void CreditSale() {
            _harness.SetFunction("UPA|Functionality|Credit Sale");
            var device = CreateDevice("CreditSale");
            var amount = _harness.GenerateRandomDecimal("CreditSale_amount", 1m, 10m, 2);
        
            var response = device.Sale(amount).WithEcrId(device.EcrId).Execute();
            Assert.IsNotNull(response);
            AssertMitcUpaResponse(response);
        }
        
        [TestMethod]
        public void RefundAgainstCard() {
            _harness.SetFunction("UPA|Functionality|Refund against Card");
            var device = CreateDevice("RefundAgainstCard");
            var amount = _harness.GenerateRandomDecimal("RefundAgainstCard_amount", 1m, 10m, 2);
        
            var response = device.Refund(amount).WithEcrId(device.EcrId).Execute();
            Assert.IsNotNull(response);
            AssertMitcUpaResponse(response);
        }

        [TestMethod]
        public void RefundAgainstTransactionId() {
            _harness.SetFunction("UPA|Functionality|Refund against Transaction ID");
            var device = CreateDevice("RefundAgainstTransactionId");
            var amount = _harness.GenerateRandomDecimal("RefundAgainstTransactionId_amount", 1m, 10m, 2);

            var sale = device.Sale(amount).WithEcrId(device.EcrId).Execute();
            Assert.IsNotNull(sale);
            AssertMitcUpaResponse(sale);

            Thread.Sleep(CurrentCacheMode == CiTestingHarness.CacheMode.Unlocked ? 15000 : 0);

            var refund = device.Refund(amount)
                .WithEcrId(device.EcrId)
                .WithTransactionId(sale.TransactionId)
                .Execute();
            Assert.IsNotNull(refund);
            AssertMitcUpaResponse(refund);
        }
    }
}
