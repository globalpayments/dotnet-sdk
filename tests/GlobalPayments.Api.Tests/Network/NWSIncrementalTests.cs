using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitOfMeasure = GlobalPayments.Api.Network.Entities.UnitOfMeasure;

namespace GlobalPayments.Api.Tests.Network {
    [TestClass]
    public class NWSIncrementalTests {
        private CreditCardData card;
        private CreditTrackData track;
        AcceptorConfig acceptorConfig;
        NetworkGatewayConfig config;
        private DebitTrackData readylinkTrack;

        public NWSIncrementalTests() {
            Address address = new Address {
                Name = "My STORE",
                StreetAddress1 = "1 MY STREET",
                City = "MYTOWN",
                PostalCode = "90210",
                State = "KY",
                Country = "USA"
            };

            acceptorConfig = new AcceptorConfig {
                Address = address,

                // data code values
                CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe_KeyEntry,
                CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.PIN,
                CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.ByMerchant,
                TerminalOutputCapability = TerminalOutputCapability.Printing_Display,
                
                // hardware software config values
                HardwareLevel = "34",
                SoftwareLevel = "21205710",

                // pos configuration values
                SupportsPartialApproval = true,
                SupportsShutOffAmount = true,
                SupportsReturnBalance = true,
                SupportsDiscoverNetworkReferenceId = true,
                SupportsAvsCnvVoidReferrals = true,
                SupportsEmvPin = true,
               
                //48-34.5
                IncrementalAuthIndicator = true,//Support incremental Authorization
                // DE 43-34 Message Data
                EchoSettlementData = true
            };

            // gateway config
            config = new NetworkGatewayConfig(NetworkGatewayType.NWS) {
                ServiceUrl = "test.txns-c.secureexchange.net",
                PrimaryPort = 15031,
                SecondaryEndpoint = "test.txns-e.secureexchange.net",
                SecondaryPort = 15031,
                CompanyId = "SPSA",
                TerminalId = "NWSDOTNET01",
                UniqueDeviceId = "0001",
                AcceptorConfig = acceptorConfig,
                EnableLogging = true,
                StanProvider = StanGenerator.GetInstance(),
                BatchProvider = BatchProvider.GetInstance()
            };
            config.MerchantType = "7523";// (Electric Vehicle Charging).
            ServicesContainer.ConfigureService(config);

            // VISA
            track = TestCards.VisaSwipe();
        }

        [TestMethod]
        public void Test_000_batch_close() {
            BatchSummary summary = BatchService.CloseBatch(BatchCloseType.Forced);
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary.IsBalanced);
        }

        //WEX DEVEXP-1055,DEVEXP-1053
        [TestMethod]
        public void Test_PreAuth_Completion_With_Wex_DE_55() {
            acceptorConfig.WexAdditionalProduct = true;//required to return IAC
            //  NPC .10 flag need to set for DE_062 IAC codeto return 
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=27120014844120000";

            FleetData fleetData = new FleetData() {
                OdometerReading = "23235",
                DriverId = "3887",
                JobNumber = "50"
            };

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("100", UnitOfMeasure.Units, 1m, 10.00m, 10m);

            Transaction response = card.Authorize(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .Execute();
            Assert.IsNotNull(captureResponse);
            System.Diagnostics.Debug.WriteLine(captureResponse.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(captureResponse.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("400", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Incremental_authorization_Visa() {
            
            Transaction response = track.Authorize(10m,true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);
            
            // check response
            Assert.AreEqual("000", response.ResponseCode);
           
            Transaction increresponse = response.Increment(5m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaAdditionalResponseData, response.IssuerData["B01"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaCustomPaymentService, response.IssuerData["B02"])
                .WithCurrency("USD")
                .Execute();
           
            Assert.IsNotNull(increresponse);
            Assert.AreEqual("000", increresponse.ResponseCode);
        }
        

        [TestMethod]
        public void Test_Incremental_authorization_Partial_Reverse_Visa() {
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);
            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction increresponse = response.Increment(5m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaAdditionalResponseData, response.IssuerData["B01"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaCustomPaymentService, response.IssuerData["B02"])
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(increresponse);
            Assert.AreEqual("000", increresponse.ResponseCode);

            Transaction partialResponse = response.Reverse(2m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaAdditionalResponseData, response.IssuerData["B01"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaCustomPaymentService, response.IssuerData["B02"])
                .WithCurrency("USD")
                .WithAuthAmount(15m)
                .Execute();
            Assert.IsNotNull(partialResponse);
            pmi = partialResponse.MessageInformation;
            Assert.IsNotNull(partialResponse);
            Assert.AreEqual("445", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);

            // check response
            Assert.AreEqual("400", partialResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Incremental_authorization_Full_Reverse_Visa() {
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);
            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction increresponse = response.Increment(5m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaAdditionalResponseData, response.IssuerData["B01"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaCustomPaymentService, response.IssuerData["B02"])
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(increresponse);
            Assert.AreEqual("000", increresponse.ResponseCode);

            Transaction partialResponse = response.Reverse(15m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaAdditionalResponseData, response.IssuerData["B01"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaCustomPaymentService, response.IssuerData["B02"])
                .WithCurrency("USD")
                .WithAuthAmount(15m)
                .Execute();
            Assert.IsNotNull(partialResponse);
            pmi = partialResponse.MessageInformation;
            Assert.IsNotNull(partialResponse);
            Assert.AreEqual("445", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);
            // check response
            Assert.AreEqual("400", partialResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Incremental_authorization_With_Reverse_Capture_Visa() {
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);
            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction increresponse = response.Increment(5m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaAdditionalResponseData, response.IssuerData["B01"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaCustomPaymentService, response.IssuerData["B02"])
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(increresponse);
            Assert.AreEqual("000", increresponse.ResponseCode);

            Transaction partialResponse = response.Reverse(2m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaAdditionalResponseData, response.IssuerData["B01"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaCustomPaymentService, response.IssuerData["B02"])
                .WithCurrency("USD")
                .Execute();
            pmi = partialResponse.MessageInformation;
            Assert.IsNotNull(partialResponse);
            Assert.AreEqual("445", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);

            Transaction captureResponse = response.Capture(13m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaAdditionalResponseData, response.IssuerData["B01"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaCustomPaymentService, response.IssuerData["B02"])
                 .WithCurrency("USD")
                 .WithAuthorizationCount(2)
                 .Execute();
            pmi = response.MessageInformation;
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Incremental_authorization_Capture_Visa() {
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);
            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(10m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaAdditionalResponseData, response.IssuerData["B01"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaCustomPaymentService, response.IssuerData["B02"])
                 .WithCurrency("USD")
                 .WithAuthorizationCount(1)
                 .Execute();
            pmi = response.MessageInformation;
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Incremental_authorization_Void_Visa() {
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);
            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction increresponse = response.Increment(5m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaAdditionalResponseData, response.IssuerData["B01"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaCustomPaymentService, response.IssuerData["B02"])
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(increresponse);
            Assert.AreEqual("000", increresponse.ResponseCode);

            Transaction voidResponse = response.Void()
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaAdditionalResponseData, response.IssuerData["B01"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaCustomPaymentService, response.IssuerData["B02"])
                .Execute();

            pmi = voidResponse.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("441", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);
            // check response
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Incremental_authorization_MC() {
            track = TestCards.MasterCardSwipe();
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);
            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction increresponse = response.Increment(5m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(increresponse);
            Assert.AreEqual("000", increresponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Incremental_authorization_Partial_Reverse_MC() {
            track = TestCards.VisaSwipe();
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);
            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction increresponse = response.Increment(5m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(increresponse);
            Assert.AreEqual("000", increresponse.ResponseCode);

            Transaction partialResponse = response.Reverse(2m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithCurrency("USD")
                .WithAuthAmount(15m)
                .Execute();
            Assert.IsNotNull(partialResponse);
            pmi = partialResponse.MessageInformation;
            Assert.IsNotNull(partialResponse);
            Assert.AreEqual("445", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);
            // check response
            Assert.AreEqual("400", partialResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Incremental_authorization_Full_Reverse_MC() {
            track = TestCards.MasterCardSwipe();
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);
            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction increresponse = response.Increment(5m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(increresponse);
            Assert.AreEqual("000", increresponse.ResponseCode);

            Transaction partialResponse = response.Reverse(15m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithCurrency("USD")
                .WithAuthAmount(15m)
                .Execute();
            Assert.IsNotNull(partialResponse);
            pmi = partialResponse.MessageInformation;
            Assert.IsNotNull(partialResponse);
            Assert.AreEqual("445", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);
            // check response
            Assert.AreEqual("400", partialResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Incremental_authorization_With_Reverse_Capture_MC() {
            track = TestCards.MasterCardSwipe();
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);
            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction increresponse = response.Increment(5m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(increresponse);
            Assert.AreEqual("000", increresponse.ResponseCode);

            Transaction partialResponse = response.Reverse(2m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithCurrency("USD")
                .Execute();
            pmi = partialResponse.MessageInformation;
            Assert.IsNotNull(partialResponse);
            Assert.AreEqual("445", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);

            Transaction captureResponse = response.Capture(13m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithCurrency("USD")
                .WithAuthorizationCount(2)
                .Execute();
            pmi = response.MessageInformation;
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Incremental_authorization_Capture_MC() {
            track = TestCards.MasterCardSwipe();
            Transaction response = track.Authorize(10m,true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);
            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(10m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithCurrency("USD")
                .WithAuthorizationCount(1)
                .Execute();
            pmi = response.MessageInformation;
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Incremental_authorization_Void_MC() {
            track = TestCards.MasterCardSwipe();
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);
            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction increresponse = response.Increment(5m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(increresponse);
            Assert.AreEqual("000", increresponse.ResponseCode);

            Transaction voidResponse = response.Void()
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .Execute();

            pmi = voidResponse.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("441", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);
            // check response
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        #region Inremental false DE_030 tag
        [TestMethod]
        public void Test_authorization_full_Capture_MC() {
            acceptorConfig.IncrementalAuthIndicator = false;
            track = TestCards.MasterCardSwipe();
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);
            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture()
                 .WithCurrency("USD")                
                 .Execute();
            pmi = captureResponse.MessageInformation;
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("201", pmi.FunctionCode);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_authorization_partial_Capture_MC() {
            acceptorConfig.IncrementalAuthIndicator = false;
            track = TestCards.MasterCardSwipe();
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);
            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(5m)
                 .WithCurrency("USD")
                 .Execute();
            pmi = captureResponse.MessageInformation;
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("202", pmi.FunctionCode);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_authorization_reversal_partial_Visa() {
            //A partial reversal advice message is sent by a POS application that supports incremental
            //authorizations when the final transaction amount(settlement amount) exceeds the total of the
            // estimated authorization amount plus incremental authorization amounts

            acceptorConfig.IncrementalAuthIndicator = false;
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);
            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction partialResponse = response.Reverse(5m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(partialResponse);
            pmi = partialResponse.MessageInformation;
            Assert.IsNotNull(partialResponse);
            Assert.AreEqual("445", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);
            // check response
            Assert.AreEqual("400", partialResponse.ResponseCode);
        }
        #endregion
    }
}
