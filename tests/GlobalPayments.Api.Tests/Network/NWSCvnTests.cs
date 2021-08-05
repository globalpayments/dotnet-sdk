using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using UnitOfMeasure = GlobalPayments.Api.Network.Entities.UnitOfMeasure;

namespace GlobalPayments.Api.Tests.Network {
    [TestClass]
    public class NWSCvnTests {
        public NWSCvnTests() {
            AcceptorConfig acceptorConfig = new AcceptorConfig();

            // data code values
            acceptorConfig.CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe_KeyEntry;
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.OnCardSecurityCode;
            acceptorConfig.TerminalOutputCapability = TerminalOutputCapability.Printing_Display;

            // hardware software config values
            acceptorConfig.HardwareLevel = "34";
            acceptorConfig.SoftwareLevel = "21205710";

            // pos configuration values
            acceptorConfig.SupportsPartialApproval = true;
            acceptorConfig.SupportsShutOffAmount = true;
            acceptorConfig.SupportsReturnBalance = true;
            acceptorConfig.SupportsDiscoverNetworkReferenceId = true;
            acceptorConfig.SupportsAvsCnvVoidReferrals = true;

            // gateway config
            NetworkGatewayConfig config = new NetworkGatewayConfig(NetworkGatewayType.NWS);
            config.ServiceUrl = "test.txns-c.secureexchange.net";
            config.PrimaryPort = 15031;
            config.SecondaryEndpoint = "test.txns-e.secureexchange.net";
            config.SecondaryPort = 15031;
            config.CompanyId = "SPSA";
            config.TerminalId = "NWSDOTNET01";
            config.AcceptorConfig = acceptorConfig;
            config.EnableLogging = true;
            config.StanProvider = StanGenerator.GetInstance();
            config.BatchProvider = BatchProvider.GetInstance();

            ServicesContainer.ConfigureService(config);
        }

        [TestMethod]
        public void Test_000_batch_close() {
            BatchSummary summary = BatchService.CloseBatch();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary.IsBalanced);
        }

        [TestMethod]
        public void Test_001_amex_match() {
            CreditCardData card = TestCards.AmexManual(false, false);
            card.Cvn = "7101";

            Transaction response = card.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
            Assert.AreEqual("Y", response.CvnResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_002_amex_mismatch() {
            CreditCardData card = TestCards.AmexManual(false, false);
            card.Cvn = "8102";

            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
            Assert.AreEqual("N", response.CvnResponseCode);

        }

        [TestMethod]
        public void Test_003_discover_match() {
            CreditCardData card = TestCards.DiscoverManual(false, false);
            card.Cvn = "703";

            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
            Assert.AreEqual("M", response.CvnResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_004_discover_mismatch() {
            CreditCardData card = TestCards.DiscoverManual(false, false);
            card.Cvn = "804";

            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
            Assert.AreEqual("N", response.CvnResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_005_discover_not_processed() {
            CreditCardData card = TestCards.DiscoverManual(false, false);
            card.Cvn = "105";

            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("107", response.ResponseCode);
        }

        [TestMethod]
        public void Test_006_discover_cid_on_card() {
            CreditCardData card = TestCards.DiscoverManual(false, false);
            card.Cvn = "106";

            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("107", response.ResponseCode);
        }

        [TestMethod]
        public void Test_007_mastercard_match() {
            CreditCardData card = TestCards.MasterCardManual(false, false);
            card.Cvn = "707";

            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
            Assert.AreEqual("M", response.CvnResponseCode);
        }

        [TestMethod]
        public void Test_008_mastercard_mismatch() {
            CreditCardData card = TestCards.MasterCardManual(false, false);
            card.Cvn = "808";

            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
            Assert.AreEqual("N", response.CvnResponseCode);
        }

        [TestMethod]
        public void Test_009_mastercard_not_processed() {
            CreditCardData card = TestCards.MasterCardManual(false, false);
            card.Cvn = "109";

            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("107", response.ResponseCode);
        }

        [TestMethod]
        public void Test_010_visa_match() {
            CreditCardData card = TestCards.VisaManual(false, false);
            card.Cvn = "710";

            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
            Assert.AreEqual("M", response.CvnResponseCode);
        }

        [TestMethod]
        public void Test_011_visa_mismatch() {
            CreditCardData card = TestCards.VisaManual(false, false);
            card.Cvn = "811";

            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
            Assert.AreEqual("N", response.CvnResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_012_visa_not_processed() {
            CreditCardData card = TestCards.VisaManual(false, false);
            card.Cvn = "112";

            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("107", response.ResponseCode);
        }

        [TestMethod]
        public void Test_013_visa_cid_on_card() {
            CreditCardData card = TestCards.VisaManual(false, false);
            card.Cvn = "113";

            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("107", response.ResponseCode);
        }

        [TestMethod]
        public void Test_014_mastercard_fleet_match() {
            CreditCardData card = TestCards.MasterCardFleetManual(false, false);
            card.Cvn = "107";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Heartland);
            productData.Add(ProductCode.Unleaded_Gas, UnitOfMeasure.Gallons, 1, 10);

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "00";

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_015_visa_fleet_match() {
            CreditCardData card = TestCards.VisaManual(false, false);
            card.Cvn = "110";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Heartland);
            productData.Add(ProductCode.Unleaded_Gas, UnitOfMeasure.Gallons, 1, 10);

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "00";

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Input Mode 6 - Key Entry (Manual Entry)
        //001. Run a American Express 1100 Authorization

        //002. Run a Discover 1100 Authorization
        //003. Run a MasterCard 1100 Authorization
        //007. Run a American Express MTI 1200 Sale
        //008. Run a Discover MTI 1200 Sale
        //009. Run a MasterCard MTI 1200 Sale
        //010. Run a Visa MTI 1200 Sale
        //011. Run a MasterCard Fleet MTI 1200 Sale
        //012. Run a Visa Fleet MTI 1200 Sale
        //Input Mode 2 – Magnetic Stripe Read DE 45 Track 1 or DE 35 Track 2 Data (Inside)
        //022. Run a Visa MTI 1200 Sale
    }
}
