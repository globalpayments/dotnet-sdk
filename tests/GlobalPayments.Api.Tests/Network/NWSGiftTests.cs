using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.Network
{
    [TestClass]
    public class NWSGiftTests {
        private GiftCard giftCard;
        AcceptorConfig acceptorConfig;
        public NWSGiftTests() {
            Address address = new Address();
            address.Name = "My STORE";
            address.StreetAddress1 = "1 MY STREET";
            address.City = "MYTOWN";
            address.PostalCode = "90210";
            address.State = "KY";
            address.Country = "USA";

            acceptorConfig = new AcceptorConfig();
            acceptorConfig.Address = address;


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


            //DE 127
            acceptorConfig.ServiceType = ServiceType.GPN_API;
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.DeTokenize;
            acceptorConfig.TokenizationType = TokenizationType.MerchantTokenization;
            acceptorConfig.MerchantId = "650000011573667";

            //DE 127
            acceptorConfig.SupportedEncryptionType = Api.Network.Entities.EncryptionType.TDES;
            acceptorConfig.ServiceType = ServiceType.GPN_API;
            acceptorConfig.OperationType = OperationType.Decrypt;

            // gateway config
            NetworkGatewayConfig config = new NetworkGatewayConfig(NetworkGatewayType.NWS);
            config.ServiceUrl = "test.txns-c.secureexchange.net";
            config.PrimaryPort = 15031;
            config.SecondaryEndpoint = "test.txns-e.secureexchange.net";
            config.SecondaryPort = 15031;
            config.CompanyId = "SPSA";
            config.TerminalId = "NWSDOTNET01";
            config.AcceptorConfig = acceptorConfig;
            config.UniqueDeviceId = "0001";
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

        //    ------------------------------------------Tokenization------------------------------------------

        [TestMethod]
        public void Test_File_Action() {
            GiftCard giftCard = TestCards.SvsSwipe();
            giftCard.TokenizationData = "6006491260550227406";
            Transaction response = giftCard.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_001_Gift_Manual_Auth() {
            GiftCard giftCard = TestCards.SvsSwipe();
            giftCard.TokenizationData = "FFE7A63D91B8FEEC4602C91F63623A81";
            Transaction response = giftCard.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Gift_Manual_Sale() {
            GiftCard giftCard = TestCards.SvsSwipe();
            giftCard.TokenizationData = "FFE7A63D91B8FEEC4602C91F63623A81";
            Transaction response = giftCard.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
            
        //force draft capture
        [TestMethod]
        public void Test_016_AuthCapture() {
            GiftCard giftCard = TestCards.SvsSwipe();
            giftCard.TokenizationData = "FFE7A63D91B8FEEC4602C91F63623A81";

            Transaction response = giftCard.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("006000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(response.AuthorizedAmount)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(captureResponse);

            // check message data
            pmi = captureResponse.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("006000", pmi.ProcessingCode);
            Assert.AreEqual("1376", pmi.MessageReasonCode);
            Assert.AreEqual("201", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_004_GiftCard_Refund() {
            GiftCard giftCard = TestCards.SvsManual();
            giftCard.TokenizationData = "44F03FB6B07AF9734BEC9F9F30D56587";// "44F03FB6B07AF9734BEC9F9F30D56587";

            Transaction response = giftCard.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_Balance_Inquiry() {
            GiftCard giftCard = TestCards.SvsSwipe();
            giftCard.TokenizationData = "FFE7A63D91B8FEEC4602C91F63623A81";
            Transaction response = giftCard.BalanceInquiry()
                        .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("316000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Sale_Reversal() {
            GiftCard giftCard = TestCards.SvsSwipe();
            giftCard.TokenizationData = "FFE7A63D91B8FEEC4602C91F63623A81";

            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);
            Transaction response = giftCard.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            response.NTSData = ntsData;
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("006000", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction reversal = response.Reverse(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(reversal);

            // check message data
            pmi = reversal.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("006000", pmi.ProcessingCode);
            Assert.AreEqual("400", pmi.FunctionCode);
            Assert.AreEqual("4021", pmi.MessageReasonCode);

            // check response
            Assert.AreEqual("400", reversal.ResponseCode);
        }

        [TestMethod]
        public void Test_015_Void() {
            GiftCard giftCard = TestCards.SvsSwipe();
            giftCard.TokenizationData = "FFE7A63D91B8FEEC4602C91F63623A81";

            Transaction response = giftCard.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            // reverse the transaction
            Transaction reverseResponse = response.Void().Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }

        //--------------------------------------Global Payments Gift Cards Tokenization------------------------------------------//
        [TestMethod]
        public void Test_File_ActionHMs() {
            GiftCard giftCard = TestCards.HmsGiftCard();
            giftCard.TokenizationData = "7083559900008168903";
            Transaction response = giftCard.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_001_GiftHMS_Manual_Auth() {
            GiftCard giftCard = TestCards.HmsGiftCard();
            giftCard.TokenizationData = "E04FB5B80626E23B932B88B92CB94235";
            Transaction response = giftCard.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_GiftHMS_Manual_Sale() {
            GiftCard giftCard = TestCards.HmsGiftCard();
            giftCard.TokenizationData = "E04FB5B80626E23B932B88B92CB94235";
            Transaction response = giftCard.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //force draft capture
        [TestMethod]
        public void Test_016_HMSAuthCapture() {
            GiftCard giftCard = TestCards.HmsGiftCard();
            giftCard.TokenizationData = "58817C9AB274F0CE22FE7B725972BAFD";

            Transaction response = giftCard.Authorize(3m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("006000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(response.AuthorizedAmount)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(captureResponse);

            // check message data
            pmi = captureResponse.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("006000", pmi.ProcessingCode);
            Assert.AreEqual("1376", pmi.MessageReasonCode);
            Assert.AreEqual("201", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_004_GiftCardHMS_Refund() {
            GiftCard giftCard = TestCards.HmsGiftCard();
            giftCard.TokenizationData = "E04FB5B80626E23B932B88B92CB94235";

            Transaction response = giftCard.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_Balance_InquiryHMS() {
            GiftCard giftCard = TestCards.HmsGiftCard();
            giftCard.TokenizationData = "E04FB5B80626E23B932B88B92CB94235";
            Transaction response = giftCard.BalanceInquiry()
                        .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("316000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Sale_ReversalHMS() {
            GiftCard giftCard = TestCards.HmsGiftCard();
            giftCard.TokenizationData = "58817C9AB274F0CE22FE7B725972BAFD";

            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);
            Transaction response = giftCard.Charge(3m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            response.NTSData = ntsData;
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("006000", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction reversal = response.Reverse(3m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(reversal);

            // check message data
            pmi = reversal.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("006000", pmi.ProcessingCode);
            Assert.AreEqual("400", pmi.FunctionCode);
            Assert.AreEqual("4021", pmi.MessageReasonCode);

            // check response
            Assert.AreEqual("400", reversal.ResponseCode);
        }

        [TestMethod]
        public void Test_015_VoidHMS() {
            GiftCard giftCard = TestCards.HmsGiftCard();
            giftCard.TokenizationData = "58817C9AB274F0CE22FE7B725972BAFD";

            Transaction response = giftCard.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            // reverse the transaction
            Transaction reverseResponse = response.Void().Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }
    }
}
