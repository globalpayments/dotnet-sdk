using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways.Events;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Tests.Network {
    [TestClass]
    public class NWSTimeoutTests {
        public NWSTimeoutTests() {
            AcceptorConfig acceptorConfig = new AcceptorConfig();
            acceptorConfig.HardwareLevel = "3750";
            acceptorConfig.SoftwareLevel = "04010031";
            acceptorConfig.OperatingSystemLevel = "Q50016A6";

            NetworkGatewayConfig config = new NetworkGatewayConfig(NetworkGatewayType.NWS);
            config.ServiceUrl = "test.txns-c.secureexchange.net";
            config.PrimaryPort = 15031;
            config.SecondaryEndpoint = "test.txns-e.secureexchange.net";
            config.SecondaryPort = 15031;
            config.CompanyId = "SPSA";
            config.TerminalId = "NWSDOTNET01";
            config.AcceptorConfig = acceptorConfig;
            config.StanProvider = StanGenerator.GetInstance();
            config.BatchProvider = BatchProvider.GetInstance();
            config.EnableLogging = true;

            ServicesContainer.ConfigureService(config);
        }

        [TestMethod]
        public void Test_001_authorization_timeout() {
            CreditTrackData track = new CreditTrackData();
            track.Value = "4012002000060016=25121011803939600000";

            try {
                track.Authorize(10m)
                        .WithCurrency("USD")
                        .WithForceGatewayTimeout(true)
                        .Execute();
                Assert.Fail("No timeout detected");
            }
            catch (GatewayTimeoutException exc) {
                Assert.AreEqual(1, exc.ReversalCount);
                Assert.IsNotNull("primary", exc.Host);
                Assert.AreEqual("400", exc.ReversalResponseCode);
                Assert.AreEqual("Accepted", exc.ReversalResponseText);

                StringBuilder sb = new StringBuilder();
                //            foreach (IGatewayEvent event in exc.GatewayEvents) {
                //        sb.append(event.getEventMessage()).append("\r\n");
                //}
                //Console.WriteLine(sb.ToString());
            }
            catch (GatewayException exc) {
                Assert.IsNotNull(exc.GatewayEvents);
            }
        }
    }
}
