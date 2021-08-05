using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Tests.Network {
    [TestClass]
    public class NWSAvsTests {
        public NWSAvsTests() {
            AcceptorConfig acceptorConfig = new AcceptorConfig();

            // data code values
            acceptorConfig.CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe_KeyEntry;
            acceptorConfig.CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.ByMerchant;
            acceptorConfig.TerminalOutputCapability = TerminalOutputCapability.Printing_Display;
            acceptorConfig.OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Unattended;

            // hardware software config values
            acceptorConfig.HardwareLevel = "34";
            acceptorConfig.SoftwareLevel = "21205710";

            // pos configuration values
            acceptorConfig.SupportsPartialApproval = true;
            acceptorConfig.SupportsShutOffAmount = true;
            acceptorConfig.SupportsReturnBalance = true;
            acceptorConfig.SupportsDiscoverNetworkReferenceId = true;
            acceptorConfig.SupportsAvsCnvVoidReferrals = true;
            acceptorConfig.SupportsEmvPin = true;

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

            // with merchant type
            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "ICR");
        }

        [TestMethod]
        public void Test_000_batch_close() {
            BatchSummary summary = BatchService.CloseBatch();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary.IsBalanced);
        }

        //001. Run a American Express MTI 1100 Authorization
        [TestMethod]
        public void Test_001_amex_auth() {
            CreditCardData card = TestCards.AmexManual(true, true);

            Transaction response = card.Authorize(0m)
                    .WithCurrency("USD")
                    .WithAddress(new Address("90071"))
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        //002. Run a Discover MTI 1100 Authorization
        [TestMethod]
        public void Test_002_discover_auth() {
            CreditCardData card = TestCards.DiscoverManual(true, true);

            Transaction response = card.Authorize(0m)
                        .WithCurrency("USD")
                        .WithAddress(new Address("90057"))
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        //007. Run a American Express MTI 1200 Sale
        [TestMethod]
        public void Test_007_amex_sale() {
            CreditCardData card = TestCards.AmexManual(true, true);

            Transaction response = card.Authorize(0m)
                        .WithCurrency("USD")
                        .WithAddress(new Address("90078"))
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //008. Run a Discover MTI 1200 Sale
        [TestMethod]
        public void Test_008_discover_sale() {
            CreditCardData card = TestCards.DiscoverManual(true, true);

            Transaction response = card.Authorize(0m)
                        .WithCurrency("USD")
                        .WithAddress(new Address("90050"))
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        //009. Run a MasterCard MTI 1200 Sale
        [TestMethod]
        public void Test_009_mastercard_sale() {
            CreditCardData card = TestCards.MasterCardManual(true, true);

            Transaction response = card.Authorize(0m)
                        .WithCurrency("USD")
                        .WithAddress(new Address("90031"))
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //010. Run a Visa MTI 1200 Sale
        [TestMethod]
        public void Test_010_visa_sale() {
            CreditCardData card = TestCards.VisaManual(true, true);

            Transaction response = card.Authorize(0m)
                        .WithCurrency("USD")
                        .WithAddress(new Address("90001"))
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //011. Run a MasterCard Fleet MTI 1200 Sale
        //[TestMethod]
        //public void Test_011_mastercard_fleet_sale() {
        //    CreditCardData card = TestCards.MasterCardFleetManual(true, true);

        //    Transaction response = card.Authorize(0m)
        //                .WithCurrency("USD")
        //                .WithAddress(new Address("90038"))
        //                .Execute();
        //    Assert.IsNotNull(response);
        //    Assert.AreEqual("000", response.ResponseCode);
        //}

        ////012. Run a Visa Fleet MTI 1200 Sale
        //[TestMethod]
        //public void Test_012_visa_fleet_sale() {
        //    CreditCardData card = TestCards.VisaFleetManual(true, true);

        //    Transaction response = card.Authorize(0m)
        //                .WithCurrency("USD")
        //                .WithAddress(new Address("100000"))
        //                .Execute();
        //    Assert.IsNotNull(response);
        //    Assert.AreEqual("000", response.ResponseCode);
        //}

        //    Input Mode 2 – Magnetic Stripe Read DE 45 Track 1 or DE 35 Track 2 Data (Inside)
        //019. Run a American Express MTI 1200 Sale

        [TestMethod]
        public void Test_019_amex_sale() {
            CreditTrackData card = TestCards.AmexSwipe();

            Transaction response = card.Authorize(0m)
                        .WithCurrency("USD")
                        .WithAddress(new Address("90085"))
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //020. Run a Discover MTI 1200 Sale
        [TestMethod]
        public void Test_020_discover_sale() {
            CreditTrackData card = TestCards.DiscoverSwipe();

            Transaction response = card.Authorize(0m)
                        .WithCurrency("USD")
                        .WithAddress(new Address("90058"))
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            //Assert.AreEqual("000", response.ResponseCode);
        }

        //021. Run a MasterCard MTI 1200 Sale
        [TestMethod]
        public void Test_021_mastercard_sale() {
            CreditTrackData card = TestCards.MasterCardSwipe();

            Transaction response = card.Authorize(0m)
                        .WithCurrency("USD")
                        .WithAddress(new Address("90039"))
                        .Execute();
            Assert.IsNotNull(response);
            //Assert.AreEqual("000", response.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        //022. Run a Visa MTI 1200 Sale
        [TestMethod]
        public void Test_022_visa_sale() {
            CreditTrackData card = TestCards.VisaSwipe();

            Transaction response = card.Authorize(0m)
                        .WithCurrency("USD")
                        //.WithAddress(new Address("90018"))
                        .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //023. Run a MasterCard Fleet MTI 1200 Sale
        //[TestMethod]
        //public void Test_023_mastercard_fleet_sale() {
        //    CreditTrackData card = TestCards.MasterCardFleetSwipe();

        //    Transaction response = card.Authorize(0m)
        //                .WithCurrency("USD")
        //                .WithAddress(new Address("90038"))
        //                .Execute();
        //    Assert.IsNotNull(response);
        //    Assert.AreEqual("000", response.ResponseCode);
        //}

        ////024. Run a Visa Fleet MTI 1200 Sale
        //[TestMethod]
        //public void Test_024_visa_fleet_sale() {
        //    CreditTrackData card = TestCards.VisaFleetSwipe();

        //    Transaction response = card.Authorize(0m)
        //                .WithCurrency("USD")
        //                .WithAddress(new Address("10000"))
        //                .Execute();
        //    Assert.IsNotNull(response);
        //    Assert.AreEqual("000", response.ResponseCode);
        //}

        //    Input Mode 2 – Magnetic Stripe Read DE 45 Track 1 or DE 35 Track 2 Data (ICR)
        //025. Run a American Express MTI 1100 Authorization

        [TestMethod]
        public void Test_025_amex_auth() {
            CreditTrackData card = TestCards.AmexSwipe();

            Transaction response = card.Authorize(0m)
                        .WithCurrency("USD")
                        .WithAddress(new Address("90085"))
                        .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //026. Run a Discover Card MTI 1100 Authorization
        [TestMethod]
        public void Test_026_discover_auth() {
            CreditTrackData card = TestCards.DiscoverSwipe();

            Transaction response = card.Authorize(0m)
                        .WithCurrency("USD")
                        .WithAddress(new Address("90053"))
                        .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            //Assert.AreEqual("000", response.ResponseCode);
        }

        //027. Run a MasterCard MTI 1100 Authorization
        [TestMethod]
        public void Test_027_mastercard_auth() {
            CreditTrackData card = TestCards.MasterCardSwipe();

            Transaction response = card.Authorize(0m)
                        .WithCurrency("USD")
                        .WithAddress(new Address("90034"))
                        .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            //Assert.AreEqual("000", response.ResponseCode);
        }

        //028. Run a Visa MTI 1100 Authorization
        //[TestMethod]
        //public void Test_028_visa_auth() {
        //    CreditTrackData card = TestCards.VisaSwipe();

        //    Transaction response = card.Authorize(0m)
        //                .WithCurrency("USD")
        //                .WithAddress(new Address("90009"))
        //                .Execute();
        //    Assert.IsNotNull(response);
        //    System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
        //    System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        //    Assert.AreEqual("107", response.ResponseCode);
        //}

        ////029. Run a MasterCard Fleet MTI 1100 Authorization
        //[TestMethod]
        //public void Test_029_mastercard_fleet_auth() {
        //    CreditTrackData card = TestCards.MasterCardFleetSwipe();

        //    Transaction response = card.Authorize(0m)
        //                .WithCurrency("USD")
        //                .WithAddress(new Address("90038"))
        //                .Execute("ICR");
        //    Assert.IsNotNull(response);
        //    Assert.AreEqual("000", response.ResponseCode);
        //}

        ////030. Run a Visa Fleet MTI 1100 Authorization
        //[TestMethod]
        //public void Test_030_visa_fleet_auth() {
        //    CreditTrackData card = TestCards.VisaFleetSwipe();

        //    Transaction response = card.Authorize(0m)
        //                .WithCurrency("USD")
        //                .WithAddress(new Address("10000"))
        //                .Execute("ICR");
        //    Assert.IsNotNull(response);
        //    Assert.AreEqual("000", response.ResponseCode);
        //}

        [TestMethod]
        public void Test_Visa_Account_Verification() {
            CreditTrackData track = TestCards.VisaSwipe(EntryMethod.Swipe);

            Transaction response = track.Authorize(0m, false)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }
    }
}
