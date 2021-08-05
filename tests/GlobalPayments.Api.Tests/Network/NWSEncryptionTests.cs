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

namespace GlobalPayments.Api.Tests.Network {
    [TestClass]
    public class NWSEncryptionTests {
        private CreditCardData card;
        private CreditCardData cardWithCvn;
        private CreditTrackData track;
        private DebitTrackData debit;

        public NWSEncryptionTests() {
            Address address = new Address();
            address.Name = "My STORE";
            address.StreetAddress1 = "1 MY STREET";
            address.City = "MYTOWN";
            address.PostalCode = "90210";
            address.State = "KY";
            address.Country = "USA";

            AcceptorConfig acceptorConfig = new AcceptorConfig();
            acceptorConfig.Address = address;

            // data code values
            acceptorConfig.CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe_KeyEntry;
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
            acceptorConfig.SupportedEncryptionType = EncryptionType.TEP2;

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

            // AMEX
            //card = TestCards.AmexManualEncrypted();
            ////cardWithCvn = TestCards.AmexManualEncrypted();
            ////cardWithCvn.Cvn = "9072488";
            //track = TestCards.AmexSwipeEncrypted();

            // DISCOVER
            card = TestCards.DiscoverManualEncrypted();
            //cardWithCvn = TestCards.DiscoverManualEncrypted();
            //cardWithCvn.Cvn = "703";
            track = TestCards.DiscoverSwipeEncryptedV2();

            // MASTERCARD
            //card = TestCards.MasterCardManualEncrypted();
            ////cardWithCvn = TestCards.MasterCardManualEncrypted();
            ////cardWithCvn.Cvn = "7803754";
            //track = TestCards.MasterCardSwipeEncryptedV2();

            // VISA
            //card = TestCards.VisaManualEncrypted(true, true);
            ////cardWithCvn = TestCards.VisaManualEncrypted();
            ////cardWithCvn.Cvn = "7803754";
            //track = TestCards.VisaSwipeEncryptedV2();

            // DEBIT
            debit = new DebitTrackData();
            debit.Value = ";6090001234567891=2112120000000000001?";
            debit.PinBlock = "62968D2481D231E1A504010024A00014";
            debit.EncryptionData = EncryptionData.Version2("/wECAQEEAoFGAgEH4gcOTDT6jRZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0m+/d4SO9TEshhRGUUQzVBrBvP/Os1qFx+6zdQp1ejjUCoDmzoUMbil9UG73zBxxTOy25f3Px0p8joyCh8PEWhADz1BkROJT3q6JnocQE49yYBHuFK0obm5kqUcYPfTY09vPOpmN+wp45gJY9PhkJF5XvPsMlcxX4/JhtCshegz4AYrcU/sFnI+nDwhy295BdOkVN1rn00jwCbRcE900kj3UsFfyc", "2");
        }

        [TestMethod]
        public void Test_000_batch_close() {
            BatchSummary summary = BatchService.CloseBatch();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary.IsBalanced);
        }

        [TestMethod]
        public void Test_001_credit_manual_auth_cvn() {
            Transaction response = cardWithCvn.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_credit_manual_auth() {
            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_003_credit_manual_sale_cvn() {
            Transaction response = cardWithCvn.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);

            // void the transaction test case #7
            Transaction voidResponse = response.Void().Execute();
            Assert.IsNotNull(voidResponse);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_004_credit_manual_sale() {
            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);

            // void the transaction test case #8
            Transaction reverseResponse = response.Void().Execute();
            Assert.IsNotNull(reverseResponse);
            System.Diagnostics.Debug.WriteLine("Void: " + reverseResponse.HostResponseDate);
            System.Diagnostics.Debug.WriteLine("Void: " + reverseResponse.SystemTraceAuditNumber);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_005_credit_manual_refund_cvn() {
            Transaction response = cardWithCvn.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_006_credit_manual_refund() {
            Transaction response = card.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_034_credit_swipe_auth() {
            Transaction response = track.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_036_credit_swipe_sale() {
            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);

            // reverse the transaction test case #40
            Transaction voidResponse = response.Void().Execute();
            Assert.IsNotNull(voidResponse);
            System.Diagnostics.Debug.WriteLine("Void: " + voidResponse.HostResponseDate);
            System.Diagnostics.Debug.WriteLine("Void: " + voidResponse.SystemTraceAuditNumber);
            Assert.AreEqual("400", voidResponse.ResponseCode);

            // reverse the transaction test case #39
            //Transaction reverseResponse = response.Reverse().Execute();
            //Assert.IsNotNull(reverseResponse);
            //System.Diagnostics.Debug.WriteLine("Reverse: " + reverseResponse.HostResponseDate);
            //System.Diagnostics.Debug.WriteLine("Reverse: " + reverseResponse.SystemTraceAuditNumber);
            //Assert.AreEqual("400", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_038_credit_swipe_refund() {
            Transaction response = track.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }
    }
}
