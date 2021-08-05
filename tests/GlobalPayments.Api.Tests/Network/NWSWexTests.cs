using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using UnitOfMeasure = GlobalPayments.Api.Network.Entities.UnitOfMeasure;

namespace GlobalPayments.Api.Tests.Network {
    [TestClass]
    public class NWSWexTests {
        public NWSWexTests() {
            AcceptorConfig acceptorConfig = new AcceptorConfig();

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
            config.TerminalId = "JOSHUA3";
            config.AcceptorConfig = acceptorConfig;
            config.EnableLogging = true;
            config.StanProvider = StanGenerator.GetInstance();
            config.BatchProvider = BatchProvider.GetInstance();

            ServicesContainer.ConfigureService(config);

            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "outside");
        }

        [TestMethod]
        public void Test_000_batch_close() {
            BatchSummary summary = BatchService.CloseBatch(BatchCloseType.Forced);
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary.IsBalanced);
        }

        /*
            3000-V202-MA-I-WX2-1200
            IPT Manually Keyed Gen Merch (400)- Approved
        */
        [TestMethod]
        public void Test_001_3000_V202_MA_I_WX2_1200() {
            CreditCardData card = new CreditCardData();
            card.Number = "6900460430001234566";
            card.ExpMonth = 12;
            card.ExpYear = 2021;

            FleetData fleetData = new FleetData
            {
                PurchaseDeviceSequenceNumber = "30002",
                ServicePrompt = "0",
                DriverId = "273368",
                //OdometerReading = "123456",
                VehicleNumber = "22001"
            };

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("400", Api.Network.Entities.UnitOfMeasure.Units, 1, 3);

            Transaction response = card.Charge(3m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3000-V202-ON-O-WX2-1100
            OPT Unleaded pre-authorization (074) - Approved
            3000-V202-ON-O-WX2-1220
            OPT Unleaded (001) - Approved
        */
        [TestMethod]
        public void Test_002_3000_V202_ON_O_WX2_1100() {
            CreditTrackData track = new CreditTrackData();
            track.Value = ";6900460430001234566=21121002200100000?";
            //card.Value = ";6900460420006149231=19023013031200001?";

            CreditCardData card = new CreditCardData();
            card.CardPresent = true;
            card.ReaderPresent = true;
            card.Number = "6900460430001234566";
            card.ExpMonth = 12;
            card.ExpYear = 2021;
            card.Cvn = "123";

            FleetData fleetData = new FleetData
            {
                PurchaseDeviceSequenceNumber = "22001",
                ServicePrompt = "0",
                DriverId = "273368",
                OdometerReading = "123456",
                VehicleNumber = "22001"
            };

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 1m, 3m, 3m);

            Transaction response = card.Charge(3m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    //.Execute();
                    .Execute("outside");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);

            //productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            //productData.Add("001", UnitOfMeasure.Gallons, 1m, 3m, 3m);

            //Transaction capture = response.Capture(3m)
            //        .WithCurrency("USD")
            //        .WithProductData(productData)
            //        .WithFleetData(fleetData)
            //        .Execute("outside");
            //Assert.IsNotNull(capture);
            //System.Diagnostics.Debug.WriteLine(capture.HostResponseDate);
            //System.Diagnostics.Debug.WriteLine(capture.SystemTraceAuditNumber);
            //Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3010-V202-ON-I-WX2-1200
            IPT Gen Merch (400) - Approved
        */
        [TestMethod]
        public void Test_003_3010_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            //card.Value = ";6900460430001234566=21121012202100000?";
            card.Value = ";6900460420006149231=19023013031200001?";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "0";
            //fleetData.OdometerReading = "125630";
            fleetData.DriverId = "273368";
            fleetData.VehicleNumber = "22021";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("102", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3010-V202-ON-O-WX2-1100
            OPT preauthorization fuel (074) Say No to Carwash - Approved
            3010-V202-ON-O-WX2-1220
            OPT Unleaded Super (003) Say No to Carwash - Approved
        */
        [TestMethod]
        public void Test_004_3010_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = ";6900460430001234566=21121012202100000?";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "0";
            //fleetData.OdometerReading = "125630";
            fleetData.DriverId = "273368";
            fleetData.VehicleNumber = "22021";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("003", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3011-V202-MA-I-WX2-RTRN
            Manually Keyed Return Test General Merchandise (400)
        */
        [TestMethod]
        public void Test_005_3011_V202_MA_I_WX2_RTRN() {
            CreditCardData card = new CreditCardData();
            card.Number = "6900460420006149231";
            card.ExpMonth = 19;
            card.ExpYear = 2022;

            FleetData fleetData = new FleetData();
            fleetData.PurchaseDeviceSequenceNumber = "30112";
            fleetData.ServicePrompt = "11";
            fleetData.OdometerReading = "100";
            fleetData.DriverId = "745212";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("400", UnitOfMeasure.Units, 1m, 53.72m, 53.72m);

            Transaction response = card.Refund(53.72m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTransactionMatchingData(new TransactionMatchingData("0000040067", "0114"))
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3011-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Declined invalid vehicle (181)
        */
        [TestMethod]
        public void Test_006_3011_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19021013011200001";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "11";
            fleetData.OdometerReading = "100";
            fleetData.VehicleNumber = "1";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("100", response.ResponseCode);
        }


        /*
            3012-V202-ON-I-WX2-1200
            "IPT Unleaded & Car Wash (001, 102) - Approved with WEX override from fleet manager"
        */
        [TestMethod]
        public void Test_007_3012_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19021003012200002";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "12";
            fleetData.OdometerReading = "4";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("102", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3013-V202-ON-I-WX2-1200
            IPT Unl PLUS (002) - Approved
        */
        [TestMethod]
        public void Test_008_3013_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19021013013200003";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "13";
            fleetData.DriverId = "698520";
            fleetData.VehicleNumber = "76543210";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("002", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3013-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3013-V202-ON-O-WX2-1220
            OPT Regular Diesel (019) - Approved
        */
        [TestMethod]
        public void Test_009_3013_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19021013013200003";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "13";
            fleetData.DriverId = "698520";
            fleetData.VehicleNumber = "76543210";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("019", UnitOfMeasure.Gallons, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3014-V202-ON-I-WX2-1200
            IPT Off Road Diesel (318) - Approved
        */
        [TestMethod]
        public void Test_010_3014_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19021013014200004";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "14";
            fleetData.DriverId = "896523";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("318", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3014-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3014-V202-ON-O-WX2-1220
            OPT E-85 (026) - Approved
        */
        [TestMethod]
        public void Test_011_3014_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19021013014200004";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "14";
            fleetData.DriverId = "896523";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("026", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3015-V202-ON-I-WX2-1200
            IPT Motor Oil (101) - Approved
        */
        [TestMethod]
        public void Test_012_3015_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19021043015200005";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "15";
            fleetData.VehicleNumber = "45850";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("101", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3015-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3015-V202-ON-O-WX2-1220
            OPT Unleaded (001) - Approved
        */
        [TestMethod]
        public void Test_013_3015_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19021043015200005";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "15";
            fleetData.VehicleNumber = "45850";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3016-V202-ON-I-WX2-1100
            IPT fuel (074) preauthorization
        */
        [TestMethod]
        public void Test_014_3016_V202_ON_I_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19021013016200006";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "16";
            fleetData.DriverId = "456320";
            fleetData.JobNumber = "123456";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3016-V202-ON-I-WX2-1200
            IPT Unleaded (001) - Approved
        */
        [TestMethod]
        public void Test_015_3016_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19021013016200006";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "16";
            fleetData.DriverId = "456320";
            fleetData.JobNumber = "4789650";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3016-V202-ON-I-WX2-1220
            IPT Completion - UNL (001) - Approved
        */
        [TestMethod]
        public void Test_016_3016_V202_ON_I_WX2_1220() {
            //CreditTrackData card = new CreditTrackData();
            //card.Value = "6900460420006149231=1902";

            CreditTrackData track = new CreditTrackData();
            track.Value = ";6900460430001234566=21121002200100000?";

            FleetData fleetData = new FleetData();
            fleetData.PurchaseDeviceSequenceNumber = "22001";
            fleetData.ServicePrompt = "0";
            fleetData.DriverId = "273368";
            fleetData.VehicleNumber = "22001";
            fleetData.OdometerReading = "12345";
            //fleetData.JobNumber = "123456";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction trans = Transaction.FromNetwork(
                    10m,
                    "TYPE04",
                    new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized),
                    track
            );

            Transaction response = trans.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3017-V202-ON-I-WX2-1200
            IPT General Merchandise (400)
        */
        [TestMethod]
        public void Test_017_3017_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19021013017200007";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "17";
            fleetData.VehicleNumber = "456320";
            fleetData.JobNumber = "4789650";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("400", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3017-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization with car wash on Unrestricted card - Approved

            3017-V202-ON-O-WX2-1220
            "OPT UNL Super & car wash (003,102) - Approved"
        */
        [TestMethod]
        public void Test_018_3017_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19021013017200007";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "17";
            fleetData.VehicleNumber = "89650";
            fleetData.JobNumber = "123456";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("003", UnitOfMeasure.Units, 1m, 10m, 10m);
            productData.Add("102", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(20m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3018-V202-ON-I-WX2-1200
            IPT Fuel Only Card Purchase Gen Merch (400) - Decline
        */
        [TestMethod]
        public void Test_019_3018_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19021003018200008";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "18";
            fleetData.OdometerReading = "896520";
            fleetData.VehicleNumber = "78563";
            fleetData.DriverId = "745212";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("400", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("100", response.ResponseCode);
        }


        /*
            3018-V202-ON-O-WX2-1100
            "OPT PRE-AUTH for UNL & car wash (001,102)  - Approved"
            3018-V202-ON-O-WX2-1220
            "OPT Completion for UNL & car wash (001,102) - Approved"
        */
        [TestMethod]
        public void Test_020_3018_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19021003018200008";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "18";
            fleetData.OdometerReading = "896520";
            fleetData.VehicleNumber = "78563";
            fleetData.DriverId = "745212";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);
            productData.Add("102", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(20m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3019-V202-ON-I-WX2-1100
            IPT Pre-authorization for fuel (074) - Approved
        */
        [TestMethod]
        public void Test_021_3019_V202_ON_I_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19021013019200009";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "19";
            fleetData.OdometerReading = "85236";
            fleetData.DriverId = "0051";
            fleetData.JobNumber = "875236";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3019-V202-ON-I-WX2-1200
            IPT Sale Max Product Codes Test (See Prod Code column) - Approved
        */
        [TestMethod]
        public void Test_022_3019_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19021013019200009";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "19";
            fleetData.OdometerReading = "85236";
            fleetData.DriverId = "0051";
            fleetData.JobNumber = "875236";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);
            productData.Add("100", UnitOfMeasure.Units, 1m, 10m, 10m);
            productData.Add("400", UnitOfMeasure.Units, 1m, 10m, 10m);
            productData.Add("950", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(40m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3019-V202-ON-I-WX2-1220
            IPT Completion for CNG (022) - Approved
        */
        [TestMethod]
        public void Test_023_3019_V202_ON_I_WX2_1220() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=1902";

            FleetData fleetData = new FleetData();
            fleetData.PurchaseDeviceSequenceNumber = "30192";
            fleetData.ServicePrompt = "19";
            fleetData.OdometerReading = "85236";
            fleetData.DriverId = "0051";
            fleetData.JobNumber = "875236";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("022", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction trans = Transaction.FromNetwork(
                    10m,
                    "TYPE04",
                    new NtsData(FallbackCode.None, AuthorizerCode.Host_Authorized),
                    card
            );

            Transaction response = trans.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3022-V202-MA-I-WX2-1200
            "IPT Manually Keyed Gen Auto & Mdse w/tax (100, 400, 950) Unrestricted Card - Approved"
        */
        [TestMethod]
        public void Test_024_3022_V202_MA_I_WX2_1200() {
            CreditCardData card = new CreditCardData();
            card.Number = "6900460420006149231";
            card.ExpMonth = 19;
            card.ExpYear = 2022;

            FleetData fleetData = new FleetData();
            fleetData.PurchaseDeviceSequenceNumber = "30222";
            //fleetData.VehicleNumber = "30222");
            fleetData.ServicePrompt = "22";
            fleetData.DriverId = "785423";
            fleetData.OdometerReading = "520310";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("100", UnitOfMeasure.Units, 1m, 10m, 10m);
            productData.Add("400", UnitOfMeasure.Units, 1m, 10m, 10m);
            productData.Add("950", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(30m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3022-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3022-V202-ON-O-WX2-1220
            OPT Unleaded (001) - Approved
        */
        [TestMethod]
        public void Test_025_3022_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19022013022200002";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "22";
            fleetData.OdometerReading = "785423";
            fleetData.DriverId = "520310";
            fleetData.EnteredData = "1234";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3031-V202-ON-I-WX2-1200
            IPT Gen Merch w/Discount (400 & 904) - Approved
        */
        [TestMethod]
        public void Test_026_3031_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19023013031200001";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "31";
            fleetData.OdometerReading = "125430";
            fleetData.EnteredData = "126542";
            fleetData.UserId = "152430";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("400", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3031-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3031-V202-ON-O-WX2-1220
            OPT Unleaded (001) - Approved
        */
        [TestMethod]
        public void Test_027_3031_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19023013031200001";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "31";
            fleetData.OdometerReading = "125430";
            fleetData.EnteredData = "126542";
            fleetData.UserId = "152430";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3032-V202-ON-I-WX2-1200
            IPT Unleaded Super (003) - Approved
        */
        [TestMethod]
        public void Test_028_3032_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19023013032200002";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "32";
            fleetData.UserId = "8533";
            fleetData.JobNumber = "1298765432";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("003", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3032-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3032-V202-ON-O-WX2-1220
            OPT Unleaded (001) - Approved
        */
        [TestMethod]
        public void Test_029_3032_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19023013032200002";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "32";
            fleetData.UserId = "8533";
            fleetData.JobNumber = "1298765432";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3033-V202-ON-I-WX2-1200
            IPT General Merchandise w/discount (400 & 904) - Approved
        */
        [TestMethod]
        public void Test_030_3033_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19023013033200003";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "33";
            fleetData.VehicleNumber = "52365";
            fleetData.UserId = "23652";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("400", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3033-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3033-V202-ON-O-WX2-1220
            OPT DEF container (600) - Approved
        */
        [TestMethod]
        public void Test_031_3033_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19023013033200003";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "33";
            fleetData.VehicleNumber = "52365";
            fleetData.UserId = "23652";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("600", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3034-V202-ON-I-WX2-1200
            IPT Auto Parts (100) - Approved
        */
        [TestMethod]
        public void Test_032_3034_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19023013034200004";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "34";
            fleetData.DriverId = "0123";
            fleetData.UserId = "52361";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("100", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3034-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3034-V202-ON-O-WX2-1220
            OPT Unleaded (001) - Approved
        */
        [TestMethod]
        public void Test_033_3034_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19023013034200004";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "34";
            fleetData.DriverId = "0123";
            fleetData.UserId = "52361";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3035-V202-ON-I-WX2-1200
            IPT Unl Plus (002) - Invalid Driver ID (180) - Decline
        */
        [TestMethod]
        public void Test_034_3035_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19023013035200005";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "35";
            fleetData.DriverId = "001234";
            fleetData.Department = "1298765432";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("002", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("100", response.ResponseCode);
        }


        /*
            3035-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3035-V202-ON-O-WX2-1220
            OPT Unleaded (001) - Approved
        */
        [TestMethod]
        public void Test_035_3035_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19023013035200005";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "35";
            fleetData.DriverId = "009865";
            fleetData.Department = "1298765432";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3036-V202-ON-I-WX2-1200
            IPT General Automotive (100) - Approved
        */
        [TestMethod]
        public void Test_036_3036_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19023013036200006";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "36";
            fleetData.UserId = "1298765432";
            fleetData.Department = "20";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("100", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3036-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3036-V202-ON-O-WX2-1220
            OPT DEF (pumped) (062) - Approved
        */
        [TestMethod]
        public void Test_037_3036_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19023013036200006";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "36";
            fleetData.UserId = "1298765432";
            fleetData.Department = "20";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("062", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3037-V202-ON-I-WX2-1200
            IPT Off Road Diesel & Gen Merch (317 400) - Approved
        */
        [TestMethod]
        public void Test_038_3037_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19023013037200007";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "37";
            fleetData.VehicleNumber = "52369";
            fleetData.Department = "11";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("317", UnitOfMeasure.Units, 1m, 10m, 10m);
            productData.Add("400", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(20m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3037-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3037-V202-ON-O-WX2-1220
            OPT Unleaded (001) - Approved
        */
        [TestMethod]
        public void Test_039_3037_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19023013037200007";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "37";
            fleetData.VehicleNumber = "52369";
            fleetData.Department = "11";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3038-V202-ON-I-WX2-1200
            "IPT Gen Merch w/tax (400,950) - Approved"
        */
        [TestMethod]
        public void Test_040_3038_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460430006149231=19023013038200008";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "38";
            fleetData.OdometerReading = "152310";
            fleetData.DriverId = "523690";
            fleetData.Department = "9";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("400", UnitOfMeasure.Units, 1m, 10m, 10m);
            productData.Add("950", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(20m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3038-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3038-V202-ON-O-WX2-1220
            OPT Unleaded (001) - Approved
        */
        [TestMethod]
        public void Test_041_3038_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460430006149231=19023013038200008";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "38";
            fleetData.OdometerReading = "152310";
            fleetData.DriverId = "523690";
            fleetData.Department = "9";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3039-V202-ON-I-WX2-1200
            "IPT Gen Auto w/discount & tax (100,904,950) - Approved"
        */
        [TestMethod]
        public void Test_042_3039_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19023013039200009";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "39";
            fleetData.OdometerReading = "100236";
            fleetData.UserId = "456320";
            fleetData.Department = "1";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("100", UnitOfMeasure.Units, 1m, 10m, 10m);
            productData.Add("950", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(20m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3039-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3039-V202-ON-O-WX2-1220
            OPT Off Road Diesel (317) - Approved
        */
        [TestMethod]
        public void Test_043_3039_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19023013039200009";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "39";
            fleetData.OdometerReading = "100236";
            fleetData.UserId = "456320";
            fleetData.Department = "1";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("317", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3040-V202-ON-I-WX2-1200
            IPT Gen Merch (400) - Approved
        */
        [TestMethod]
        public void Test_044_3040_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19024023040200000";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "40";
            fleetData.OdometerReading = "100023";
            fleetData.VehicleNumber = "5236";
            fleetData.Department = "8";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("400", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3040-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3040-V202-ON-O-WX2-1220
            OPT Unleaded (001) - Approved
        */
        [TestMethod]
        public void Test_045_3040_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19024023040200000";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "40";
            fleetData.OdometerReading = "100023";
            fleetData.VehicleNumber = "5263";
            fleetData.Department = "8";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3041-V202-ON-I-WX2-1200
            IPT General Merchandise w/Discount (400 & 904) - Approved
        */
        [TestMethod]
        public void Test_046_3041_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19024013041200001";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "41";
            fleetData.Department = "96";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("400", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3041-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3041-V202-ON-O-WX2-1220
            OPT Unleaded (001) - Approved
        */
        [TestMethod]
        public void Test_047_3041_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19024013041200001";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "41";
            fleetData.Department = "96";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3042-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3042-V202-ON-O-WX2-1220
            OPT Unleaded (001) - Approved
        */
        [TestMethod]
        public void Test_048_3042_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19024013042200002";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "42";
            fleetData.EnteredData = "1298765432";
            fleetData.UserId = "8564";
            fleetData.Department = "3";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3043-V202-ON-I-WX2-1200
            Gen Merch (400) Declined - Invalid Site
        */
        [TestMethod]
        public void Test_049_3043_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19024013043200003";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "43";
            fleetData.EnteredData = "878623";
            fleetData.VehicleNumber = "85214";
            fleetData.Department = "3";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("400", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("100", response.ResponseCode);
        }


        /*
            3043-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3043-V202-ON-O-WX2-1220
            OPT Off Road Diesel (317) - Approved
        */
        [TestMethod]
        public void Test_050_3043_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19024013043200003";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "43";
            fleetData.EnteredData = "878623";
            fleetData.VehicleNumber = "85214";
            fleetData.Department = "3";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("317", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3044-V202-ON-I-WX2-1200
            IPT Premium Diesel (020) - Approved
        */
        [TestMethod]
        public void Test_051_3044_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19024013044200004";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "44";
            fleetData.EnteredData = "543210";
            fleetData.DriverId = "123456";
            fleetData.Department = "1230";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("020", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3044-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3044-V202-ON-O-WX2-1220
            OPT Unleaded (001) - Approved
        */
        [TestMethod]
        public void Test_052_3044_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19024013044200004";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "44";
            fleetData.EnteredData = "753214";
            fleetData.DriverId = "908765";
            fleetData.Department = "5";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3045-V202-ON-I-WX2-1200
            IPT General Merchandise w/Coupon (400 & 904) - Approved
        */
        [TestMethod]
        public void Test_053_3045_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19024013045200005";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "45";
            fleetData.EnteredData = "8523641";
            fleetData.DriverId = "012361";
            fleetData.UserId = "52369";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("400", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3045-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3045-V202-ON-O-WX2-1220
            OPT Unleaded  (001) - Approved
        */
        [TestMethod]
        public void Test_054_3045_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19024013045200005";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "45";
            fleetData.EnteredData = "753214";
            fleetData.DriverId = "908765";
            fleetData.UserId = "52369";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3046-V202-ON-I-WX2-1200
            IPT General Merchandise w/discount (400 & 904) - Approved
        */
        [TestMethod]
        public void Test_055_3046_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19024013046200006";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "46";
            fleetData.EnteredData = "52361";
            fleetData.UserId = "9876543210";
            fleetData.DriversLicenseNumber = "520123";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("400", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3046-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3046-V202-ON-O-WX2-1220
            OPT Unleaded (001) - Approved
        */
        [TestMethod]
        public void Test_056_3046_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19024013046200006";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "46";
            fleetData.EnteredData = "52361";
            fleetData.DriversLicenseNumber = "520123";
            fleetData.UserId = "9876543210";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3047-V202-ON-I-WX2-1200
            IPT Unleaded & Gen Merch (001 & 400) - Approved
        */
        [TestMethod]
        public void Test_057_3047_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19024013047200007";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "47";
            fleetData.EnteredData = "1230";
            fleetData.VehicleNumber = "1230";
            fleetData.DriversLicenseNumber = "1";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);
            productData.Add("400", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(20m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3047-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3047-V202-ON-O-WX2-1220
            OPT Unleaded (001) - Approved
        */
        [TestMethod]
        public void Test_058_3047_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19024013047200007";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "47";
            fleetData.EnteredData = "1230";
            fleetData.VehicleNumber = "1230";
            fleetData.DriversLicenseNumber = "1";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3048-V202-ON-I-WX2-1200
            IPT Unl PLUS (002) - Approved
        */
        [TestMethod]
        public void Test_059_3048_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19024013048200008";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "48";
            fleetData.EnteredData = "1230";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("002", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3048-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3048-V202-ON-O-WX2-1220
            OPT Unleaded Plus (002) - Approved
        */
        [TestMethod]
        public void Test_060_3048_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19024013048200008";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "48";
            fleetData.EnteredData = "1230";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("002", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3049-V202-ON-I-WX2-1200
            IPT Regular Diesel (019) - Approved
        */
        [TestMethod]
        public void Test_061_3049_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19024013049200009";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "49";
            fleetData.DriverId = "023450";
            fleetData.EnteredData = "1234";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("019", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3049-V202-ON-O-WX2-1100
            OPT Unleaded preauthorization (074) - Approved
            3049-V202-ON-O-WX2-1220
            OPT Unleaded (001) - Approved
        */
        [TestMethod]
        public void Test_062_3049_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19024013049200009";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "49";
            fleetData.DriverId = "023450";
            fleetData.EnteredData = "1234";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3050-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Approved
            3050-V202-ON-O-WX2-1220
            OPT Unleaded (001) - Approved
        */
        [TestMethod]
        public void Test_063_3050_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19025013050200000";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "50";
            fleetData.UserId = "16548";
            fleetData.EnteredData = "254630";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3051-V202-ON-I-WX2-1200
            Gen Merch (400)
        */
        [TestMethod]
        public void Test_064_3051_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19025013051200001";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "51";
            fleetData.VehicleNumber = "5365";
            fleetData.EnteredData = "63255";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("400", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3051-V202-ON-I-WX2-RTRN
            "Return Test Gen Merch with discount (400, 904)"
        */
        [TestMethod]
        public void Test_065_3051_V202_ON_I_WX2_RTRN() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=1902";

            FleetData fleetData = new FleetData();
            fleetData.PurchaseDeviceSequenceNumber = "30512";
            fleetData.ServicePrompt = "10";
            //        fleetData.setVehicleTag("5365");
            //        fleetData.EnteredData = "63255");
            fleetData.DriverId = "123456";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("400", UnitOfMeasure.Units, 1, 52.73);

            Transaction response = card.Refund(52.73m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTransactionMatchingData(new TransactionMatchingData("0000040067", "0114"))
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3051-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Decline (Invalid Driver ID - 180)
        */
        [TestMethod]
        public void Test_066_3051_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19025013051200001";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "51";
            fleetData.VehicleNumber = "5365";
            fleetData.EnteredData = "63255";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("100", response.ResponseCode);
        }


        /*
            3052-V202-ON-O-WX2-1100
            OPT fuel (074) preauthorization - Declined Exp Card (101)
        */
        [TestMethod]
        public void Test_067_3052_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=11021003052200000";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "10";
            fleetData.OdometerReading = "4";
            fleetData.DriverId = "130031";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("100", response.ResponseCode);
        }


        /*
            3053-V202-ON-O-WX2-1100
            OPT Unleaded (001) - Approved (mod 10 test)
            3053-V202-ON-O-WX2-1220
            OPT Unleaded (001) - Approved (mod 10 test)
        */
        [TestMethod]
        public void Test_068_3053_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460430006149231=19020013053200000";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "00";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3069-V202-SF-O-WX2-1200
            Off-line Keyed Manual - Pretend you called VRU (on Verifone) and receive auth # 123456 to key in manually.
        */
        [TestMethod]
        public void Test_069_3069_V202_SF_O_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19020003069200000";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "00";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithOfflineAuthCode("123456")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3070-V202-ON-O-WX2-1100
            OPT Action Code 002 ($999.99 returned in 1110 and POS shuts off at $150.00 Floor Limit) Purchase UNL (001)
            3070-V202-ON-O-WX2-1220
            OPT Action Code 002 ($999.99 returned in 1110 and POS shuts off at $150.00 Floor Limit) Purchase UNL (001)
        */
        [TestMethod]
        public void Test_070_3070_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19021013070200000";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "10";
            fleetData.OdometerReading = "125630";
            fleetData.DriverId = "0330";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3072-V202-ON-O-WX2-1100
            "OPT Action Code 002 ($10.00 returned on 1110 and POS shuts off at $10.00, which is less than $150.00 Floor Limit) Purc UNL (001)"
            3072-V202-ON-O-WX2-1220
            "OPT Action Code 002 ($10.00 returned on 1110 and POS shuts off at $10.00, which is less than $20.00 Floor Limit) Purch UNL(001)"
        */
        [TestMethod]
        public void Test_071_3072_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19021003072200000";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "10";
            fleetData.OdometerReading = "123456";
            fleetData.DriverId = "123456";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }


        /*
            3073-V202-ON-I-WX2-1200
            IPT Fuel Only Card Purchase Gen Merch (400) - WEX override approves
        */
        [TestMethod]
        public void Test_072_3073_V202_ON_I_WX2_1200() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19021043073200002";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "12";
            fleetData.OdometerReading = "123456";

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("400", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        /*
            3073-V202-ON-O-WX2-1100
            OPT Action Code 002 ($150.00 returned on 1110 and POS shuts off at $150.00 which is also the Floor Limit) Purch UNL(001)
            3073-V202-ON-O-WX2-1220
            OPT Action Code 002 ($150.00 returned on 1110 and POS shuts off at $150.00 which is also the Floor Limit) Purch UNL(001)
        */
        [TestMethod]
        public void Test_073_3073_V202_ON_O_WX2_1100() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=19021003073200002";

            FleetData fleetData = new FleetData();
            fleetData.ServicePrompt = "12";
            fleetData.OdometerReading = "123456";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute("outside");
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }
    }
}
