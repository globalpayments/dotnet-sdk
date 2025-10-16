using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitOfMeasure = GlobalPayments.Api.Network.Entities.UnitOfMeasure;

namespace GlobalPayments.Api.Tests.Network.Certification {
    [TestClass]
    public class NWSSpec21_1CertificationTests {
        private CreditTrackData track;
        AcceptorConfig acceptorConfig;
        NetworkGatewayConfig config;

        public NWSSpec21_1CertificationTests() {
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
                //MobileDevice = true,
                
                //48-34.5
                IncrementalAuthIndicator = true,//Support incremental Authorization
                WexAdditionalProduct = true,

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

            // with merchant type
            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "ICR");

            // VISA
            track = TestCards.VisaSwipe();
        }

        //Test Case #1
        [TestMethod]
        public void Test_Incremental_authorization_Partial_Reverse_Visa() {
            Transaction response = track.Authorize(10m, true)
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
            Assert.AreEqual("400", partialResponse.ResponseCode);
        }

        //Test Case #2
        [TestMethod]
        public void Test_007_Swipe_Sale() {
            Transaction response = track.Charge(100m)
                .WithCurrency("USD")
                .WithAmountTaxed(1m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Test Case #3
        [TestMethod]
        public void Test_Wex_EMV_Without_DE55_Auth_1100() {
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
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Test Case #4
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
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .WithFleetData(fleetData)
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
            Assert.AreEqual("400", captureResponse.ResponseCode);
        }

        //Test Case #6
        [TestMethod]
        public void Test_Incremental_Authorization_With_Reverse_Capture_MC() {
            track = TestCards.MasterCardSwipe();
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);
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

        //Test Case #7
        [TestMethod]
        public void Test_Incremental_Authorization_Partial_Reverse_Vi(){
            track = TestCards.VisaSwipe();
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
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
            Assert.AreEqual("400", partialResponse.ResponseCode);
        }

        //Test Case #8
        [TestMethod]
        public void Test_Incremental_Authorization_Partial_Reverse_PartiallyApproved_Vi() {
            track = TestCards.VisaSwipe();
            Transaction response = track.Authorize(142.00m, true)
                        .WithCurrency("USD")
                        .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("002", response.ResponseCode);

            Transaction increresponse = response.Increment(5m)
                .WithIssuerData(DE62_CardIssuerEntryTag.ActualStan, response.IssuerData["IST"])
                .WithIssuerData(DE62_CardIssuerEntryTag.IssuerReferenceNumber, response.IssuerData["IRN"])
                .WithIssuerData(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, response.IssuerData["IRR"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaAdditionalResponseData, response.IssuerData["B01"])
                .WithIssuerData(DE62_CardIssuerEntryTag.VisaCustomPaymentService, response.IssuerData["B02"])
                .WithCurrency("USD")
                .Execute("ICR");
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
                .Execute("ICR");
            Assert.IsNotNull(partialResponse);
            Assert.AreEqual("400", partialResponse.ResponseCode);
        }

        //Test Case #9
        [TestMethod]
        public void Test_Partial_Reversal_Discover() {
            track = TestCards.DiscoverSwipe();
            Transaction response = track.Charge(10m)
                        .WithCurrency("USD")
                        .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction reversal = response.Reverse(5m)
                .Execute("ICR");
            Assert.IsNotNull(reversal);
            Assert.AreEqual("400", reversal.ResponseCode);
        }

        //Test Case #10
        [TestMethod]
        public void Test_Mobile_Transaction_With_4833_5() {
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Test Case #11
        [TestMethod]
        public void Test_Initial_Authorization_Vi() {
            track = TestCards.VisaSwipe();
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Test Case #12
        [TestMethod]
        public void Test_Incremental_Authorization_MC() {
            track = TestCards.MasterCardSwipe();
            Transaction response = track.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
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

        //Test Case #13
        [TestMethod]
        public void Test_Sale_With_ICR_Discover() {
            track = TestCards.DiscoverSwipe();
            Transaction increresponse = track.Authorize(1m)
                .WithCurrency("USD")
                .Execute("ICR");
            Assert.IsNotNull(increresponse);
            Assert.AreEqual("000", increresponse.ResponseCode);
        }

        //Test Case #14
        [TestMethod]
        public void Test_Sale_With_IAUTag_MC() {
            track = TestCards.MasterCardSwipe();
            Transaction increresponse = track.Charge(5m)
                .WithIssuerData(DE62_CardIssuerEntryTag.MastercardUCAFData, "IAU")
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(increresponse);
            Assert.AreEqual("000", increresponse.ResponseCode);
        }

        //Test Case #15
        [TestMethod]
        public void Test_Sale_With_IMDTag_MC() {
            track = TestCards.MasterCardSwipe();
            Transaction increresponse = track.Charge(5m)
                .WithIssuerData(DE62_CardIssuerEntryTag.MastercardDSRPCryptogram, "IMD")
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(increresponse);
            Assert.AreEqual("000", increresponse.ResponseCode);
        }

        //Test Case #16
        [TestMethod]
        public void Test_Sale_With_IMWTag_MC() {
            track = TestCards.MasterCardSwipe();
            Transaction increresponse = track.Charge(5m)
                .WithIssuerData(DE62_CardIssuerEntryTag.MastercardWalletID, "IMW")
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(increresponse);
            Assert.AreEqual("000", increresponse.ResponseCode);
        }

        //Test Case #17
        [TestMethod]
        public void Test_Wex_Manual_With_F01_1200() {
            CreditCardData card = new CreditCardData();
            card.Number = "6900460430001234566";
            card.ExpMonth = 12;
            card.ExpYear = 2026;

            FleetData fleetData = new FleetData {
                PurchaseDeviceSequenceNumber = "30002",
                ServicePrompt = "0",
                DriverId = "273368",
                VehicleNumber = "22001"
            };

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("400", UnitOfMeasure.Units, 1, 3);

            Transaction response = card.Charge(3m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Test Case #18
        [TestMethod]
        public void Test_Wex_NonOriginal_With_F01_1100_1200() {
            //CreditTrackData card = new CreditTrackData();
            //card.Value = ";6900460430001234566=21121012202100000?";
            CreditCardData card = new CreditCardData();
            card.Number = "6900460430001234566";
            card.ExpMonth = 12;
            card.ExpYear = 2026;

            FleetData fleetData = new FleetData {
                PurchaseDeviceSequenceNumber = "30002",
                ServicePrompt = "0",
                DriverId = "273368",
                VehicleNumber = "22001"
            };

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            //Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("003", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }

        //Test Case #1
        [TestMethod]
        public void Test_Incremental_authorization_With_OriginalAmount_Visa() {
            Transaction response = track.Authorize(5m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
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
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }
        
        [TestMethod]
        public void Test_authorization_Full_Capture_WithoutDE30_MC() {
            acceptorConfig.IncrementalAuthIndicator = false;
            track = TestCards.MasterCardSwipe();
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture()
                 .WithCurrency("USD")
                 .Execute();
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Authorization_WithoutIncremental_Partial_Capture() {
            acceptorConfig.IncrementalAuthIndicator = false;
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(5m)
                 .WithCurrency("USD")
                 .Execute();
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Authorization_WithoutIncremental_Partial_Reversal_MC() {
            acceptorConfig.IncrementalAuthIndicator = false;
            track = TestCards.MasterCardSwipe();
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction partialResponse = response.Reverse(5m)
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(partialResponse);
            Assert.AreEqual("400", partialResponse.ResponseCode);
        }
    }
}
