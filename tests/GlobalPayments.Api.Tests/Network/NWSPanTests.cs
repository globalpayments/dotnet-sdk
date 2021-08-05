using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Tests.Network {
    [TestClass]
    public class NWSPanTests {
        public NWSPanTests() {
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
        public void Test_001_amex_x416() {
            CreditTrackData track = TestCards.AmexSwipe();

            // Product Data
            ProductData productData = new ProductData(ServiceLevel.SelfServe);
            productData.Add(
                    ProductCode.Unleaded_Premium_Gas,
                    Api.Network.Entities.UnitOfMeasure.Gallons,
                        2.64m,
                        1.429m,
                        3.77m
                );

            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
    }
}
