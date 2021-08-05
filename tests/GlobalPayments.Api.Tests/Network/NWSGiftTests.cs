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
    public class NWSGiftTests {
        private GiftCard giftCard;
        public NWSGiftTests() {
            AcceptorConfig acceptorConfig = new AcceptorConfig();

            // data code values
            acceptorConfig.CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe_KeyEntry;
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.PIN;
            acceptorConfig.TerminalOutputCapability = TerminalOutputCapability.Printing_Display;
            acceptorConfig.CardDataOutputCapability = CardDataOutputCapability.Unknown;
            acceptorConfig.PinCaptureCapability = PinCaptureCapability.Unknown;

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

            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "ICR");

            config.NodeIdentification = "VLK2";
            ServicesContainer.ConfigureService(config, "ValueLink");

            // VALUE LINK
            //giftCard = TestCards.ValueLinkManual();
            //giftCard = TestCards.ValueLinkSwipe();

            // SVS
            //giftCard = TestCards.SvsManual();
            giftCard = TestCards.SvsSwipe();

            //GIFT CARD
            //giftCard = TestCards.GiftCardManual();
            //giftCard = TestCards.GiftCardSwipe();
        }

        [TestMethod]
        public void Test_000_batch_close() {
            BatchSummary summary = BatchService.CloseBatch();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary.IsBalanced);
        }

        // Test 172
        [TestMethod]
        public void giftCard_activate() {
            Transaction response = giftCard.Activate(25m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);      
        }

        // Test 170
        [TestMethod]
        public void giftCard_add_value() {
            Transaction response = giftCard.AddValue(100m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        // Test 165 (manual)
        // Test 168 (swipe)
        [TestMethod]
        public void giftCard_sale() {
            Transaction response = giftCard.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);            
        }

        // Test 175 (Reverse Sale)
        // Test 176 (Reverse Return)
        [TestMethod]
        public void giftCard_reversal() {
            try {
                giftCard.Charge(11m)
                        .WithCurrency("USD")
                        .WithForceGatewayTimeout(true)
                        .Execute();
                Assert.Fail("No exception thrown");
            }
            catch (GatewayTimeoutException) {

            }
        }

        [TestMethod]
        public void giftCard_sale_cashBack() {
            Transaction response = giftCard.Charge(41m)
                        .WithCurrency("USD")
                        .WithCashBack(40m)
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        // Test 171
        [TestMethod]
        public void giftCard_cash_out() {
            Transaction response = giftCard.CashOut()
                        .WithClerkId("41256")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        // Test 167
        // Test 177 (ICR) & 178 (ICR Auth-Capture)
        [TestMethod]
        public void giftCard_auth_capture() {
            Transaction response = giftCard.Authorize(50m, true)
                        .WithCurrency("USD")
                        .Execute();
                        //.Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction transaction = Transaction.FromNetwork(
                    50m,
                    response.AuthorizationCode,
                    response.NTSData,
                    giftCard,
                    response.MessageTypeIndicator,
                    response.SystemTraceAuditNumber,
                    response.OriginalTransactionTime
            );

            Transaction captureResponse = transaction.Capture(new Decimal(35.24))
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(captureResponse);
            System.Diagnostics.Debug.WriteLine(captureResponse.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(captureResponse.SystemTraceAuditNumber);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        // Test 166
        [TestMethod]
        public void giftCard_balance_inquiry() {
            Transaction response = giftCard.BalanceInquiry()
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        // Test 169
        [TestMethod]
        public void giftCard_return() {
            Transaction response = giftCard.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        // Test 174
        [TestMethod]
        public void giftCard_void() {
            Transaction response = giftCard.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);

            Transaction voidResponse = response.Void().Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("400", voidResponse.ResponseCode);
            System.Diagnostics.Debug.WriteLine(voidResponse.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(voidResponse.SystemTraceAuditNumber);
        }

        // Test 173
        [TestMethod]
        public void giftCard_voice_capture() {
            Transaction trans = Transaction.FromNetwork(
                    10m,
                    "TYPE04",
                    NtsData.VoiceAuthorized(),
                    giftCard
                );

            Transaction response = trans.Capture()
                    .WithReferenceNumber("12345")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void value_link_card_type_test() {
            GiftCard card = new GiftCard();
            card.SetValue("6010567085878703=25010004000070779628");

            Transaction response = card.Authorize(new Decimal(1.00), true)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }
    }
}
