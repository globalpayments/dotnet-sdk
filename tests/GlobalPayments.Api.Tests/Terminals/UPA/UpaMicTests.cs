using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.UPA {
    [TestClass]
    public class UpaMicTests {
        IDeviceInterface _device;

        public UpaMicTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.UPA_DEVICE,
                ConnectionMode = ConnectionModes.MIC,
                GatewayConfig = new GpApiConfig {
                    Environment = Environment.QA,
                    MerchantId = "MER_3651eab3530d4c8b8909d9844bfdbffa",
                    AppId = "83cdNQ0YBmzxzkLpFHpDGn2ir0WKTW0N",
                    AppKey = "1ASrcQZb0AEqR6ZT",
                    Channel = Channel.CardPresent,
                    Country = "US",
                    DeviceCurrency = "USD",
                    EnableLogging = true,
                    MethodNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                    AccessTokenInfo = new AccessTokenInfo {
                        TransactionProcessingAccountID = "TRA_4dca2d890e914f7da0790a70947b98c8",
                        TransactionProcessingAccountName = "DublinTransAccount_Nucleus01"
                    }
                },
                Timeout = 30000,
                RequestIdProvider = new RandomIdProvider()
            });
        }

        [TestMethod]
        public void CreditSale() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            try {
                var response = _device.Sale(10m)
                    .WithEcrId(13)
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("SUCCESS", response.ResponseText);
                Assert.AreEqual("INITIATED", response.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }
    }
}
