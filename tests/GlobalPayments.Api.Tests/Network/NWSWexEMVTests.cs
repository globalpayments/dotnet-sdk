using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using UnitOfMeasure = GlobalPayments.Api.Network.Entities.UnitOfMeasure;

namespace GlobalPayments.Api.Tests.Network {
    [TestClass]
    public class NWSWexEMVTests
    {
        public NWSWexEMVTests()
        {
            AcceptorConfig acceptorConfig = new AcceptorConfig();

            // data code values
            acceptorConfig.CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe_KeyEntry;
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.PIN;
            acceptorConfig.CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.ByMerchant;
            acceptorConfig.TerminalOutputCapability = TerminalOutputCapability.Printing_Display;

            // hardware software config values
            acceptorConfig.HardwareLevel = "34";
            acceptorConfig.SoftwareLevel = "21205710";

            // pos configuration values
            acceptorConfig.SupportsShutOffAmount = true;
            acceptorConfig.SupportsDiscoverNetworkReferenceId = true;
            acceptorConfig.SupportsAvsCnvVoidReferrals = true;
            acceptorConfig.SupportedEncryptionType = EncryptionType.TEP2;
            acceptorConfig.SupportsEmvPin = true;

            // gateway config
            NetworkGatewayConfig config = new NetworkGatewayConfig(NetworkGatewayType.NWS);
            config.ServiceUrl = "test.txns-c.secureexchange.net";
            config.PrimaryPort = 15031;
            config.SecondaryEndpoint = "test.txns-e.secureexchange.net";
            config.SecondaryPort = 15031;
            config.UniqueDeviceId = "2001";
            config.CompanyId = "SPSA";
            config.TerminalId = "JOSHUA3";
            config.AcceptorConfig = acceptorConfig;
            config.EnableLogging = true;
            config.StanProvider = StanGenerator.GetInstance();
            config.BatchProvider = BatchProvider.GetInstance();
            config.MerchantType = "5541";
            ServicesContainer.ConfigureService(config);
        }

        [TestMethod]
        public void test_wex_emv_4804_V412_ON_I_WX4_1200_1()
        {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=27121004804220000";

            FleetData fleetData = new FleetData()
            {
                OdometerReading = "9876",
                DriverId = "1234"
            };

            ProductData productData = new ProductData(ServiceLevel.Other_NonFuel, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 10.720m, 4.664m, 50m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void test_wex_emv_4847_V412_ON_I_WX4_1200_2()
        {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=27121004847120001";

            FleetData fleetData = new FleetData()
            {
                VehicleNumber = "123450",
                OdometerReading = "123450"
            };

            ProductData productData = new ProductData(ServiceLevel.Other_NonFuel, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 10.720m, 4.664m, 50m);


            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void test_wex_emv_4892_V412_ON_I_WX4_1200_3() {

            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=27124004892120000";

            FleetData fleetData = new FleetData()
            {
                VehicleNumber = "12345",
                OdometerReading = "42586",
                Department = "171"
            };

            ProductData productData = new ProductData(ServiceLevel.Other_NonFuel, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 10.720m, 4.664m, 50m);


            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void test_wex_emv_4918_V412_ON_I_WX4_1200_4() {

            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=27121004918120000";

            FleetData fleetData = new FleetData()
            {
                OdometerReading = "1234",
                DriverId = "1234",
                EnteredData = "1234"
            };

            ProductData productData = new ProductData(ServiceLevel.Other_NonFuel, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 10.720m, 4.664m, 50m);

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void test_wex_emv_4922_V412_ON_I_WX4_1200_5() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=27121004922120001";

            FleetData fleetData = new FleetData()
            {
                VehicleNumber = "123456",
                OdometerReading = "123456",
                TripNumber = "123456",
                UnitNumber = "123456"
            };

            ProductData productData = new ProductData(ServiceLevel.Other_NonFuel, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 10.720m, 4.664m, 50m);


            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void test_wex_emv_4924_V412_ON_I_WX4_1200_6() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=27121004924120000";

            FleetData fleetData = new FleetData()
            {
                DriverId = "123456",
                OdometerReading = "123456",
                EnteredData = "123456",
                TripNumber = "123456"
            };

            ProductData productData = new ProductData(ServiceLevel.Other_NonFuel, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 10.720m, 4.664m, 50m);


            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void test_wex_emv_4926_V412_ON_I_WX4_1200_7() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=27121004926120001";

            FleetData fleetData = new FleetData()
            {
                VehicleNumber = "123456",
                OdometerReading = "123456",
                TrailerReferHours = "123456",
                UnitNumber = "123456"
            };

            ProductData productData = new ProductData(ServiceLevel.Other_NonFuel, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 10.720m, 4.664m, 50m);


            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void test_wex_emv_5136_V412_ON_I_WX4_1200_8() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "7071380420006149231=27121005136120009";

            FleetData fleetData = new FleetData()
            {
                //VehicleNumber = "123456",
                OdometerReading = "123450",
                DriverId = "123450",
                JobNumber = "1234"
            };

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 10.720m, 4.664m, 50m);


            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void test_wex_emv_5193_V412_ON_I_WX4_1200_9() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149249=27121005193120000";

            FleetData fleetData = new FleetData()
            {
                //VehicleNumber = "123456",
                OdometerReading = "9876",
                DriverId = "1234",
                //JobNumber = "1234"
            };

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("400", UnitOfMeasure.Gallons, 10.720m, 4.664m, 50m);


            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void test_4844_V412_ON_I_WX4_1220_1() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=27120014844120000";

            ProductData productData = new ProductData(ServiceLevel.Other_NonFuel, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("400", UnitOfMeasure.Units, 1m, 10.00m, 10m);

            FleetData fleetData = new FleetData();

            Transaction response = card.Authorize(45m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    //.WithFleetData(fleetData)
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            //Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(45m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    //.WithFleetData(fleetData)
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .Execute();
            Assert.IsNotNull(captureResponse);
            System.Diagnostics.Debug.WriteLine(captureResponse.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(captureResponse.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("400", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void test_4846_V412_ON_I_WX4_1220_2() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=27120014844120000";

            FleetData fleetData = new FleetData()
            {
                OdometerReading = "123450",
                DriverId = "123450"
            };

            ProductData productData = new ProductData(ServiceLevel.Other_NonFuel, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("100", UnitOfMeasure.Units, 1m, 10.00m, 10m);

            Transaction response = card.Authorize(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            //Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(45m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .Execute();
            Assert.IsNotNull(captureResponse);
            System.Diagnostics.Debug.WriteLine(captureResponse.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(captureResponse.SystemTraceAuditNumber);
            // check response
            // Assert.AreEqual("400", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void test_4848_V412_ON_I_WX4_1220_3() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=27120014844120000";

            FleetData fleetData = new FleetData()
            {
                OdometerReading = "123450",
                DriverId = "123450"
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
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            //Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(45m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .Execute();
            Assert.IsNotNull(captureResponse);
            System.Diagnostics.Debug.WriteLine(captureResponse.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(captureResponse.SystemTraceAuditNumber);
            // check response
            //Assert.AreEqual("400", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void test_4873_V412_ON_I_WX4_1220_4() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "6900460420006149231=27120014844120000";

            FleetData fleetData = new FleetData()
            {
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
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            //Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(45m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .Execute();
            Assert.IsNotNull(captureResponse);
            System.Diagnostics.Debug.WriteLine(captureResponse.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(captureResponse.SystemTraceAuditNumber);
            // check response
            //Assert.AreEqual("400", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void test_5136_V412_ON_I_WX4_1220_5() {
            CreditTrackData card = new CreditTrackData();
            card.Value = "7071380420006149231=27121005136120009";

            FleetData fleetData = new FleetData()
            {
                OdometerReading = "123450",
                DriverId = "123450",
                JobNumber = "1234"
            };

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Units, 1m, 10.00m, 10m);

            Transaction response = card.Authorize(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            //Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(45m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .Execute();
            Assert.IsNotNull(captureResponse);
            System.Diagnostics.Debug.WriteLine(captureResponse.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(captureResponse.SystemTraceAuditNumber);
            // check response
            //Assert.AreEqual("400", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void test_5135_V412_ON_I_WX4_1100()
        {
            CreditTrackData card = new CreditTrackData();
            card.Value = ";6900460420006149231=18011005135120000?";

            FleetData fleetData = new FleetData();
            //fleetData.ServicePrompt = "0";
            fleetData.OdometerReading = "9876";
            fleetData.DriverId = "1234";
            //fleetData.VehicleNumber = "22021";

            ProductData productData = new ProductData(ServiceLevel.SelfServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("074", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = card.Authorize(1m, true)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F07A0000007681010820239008407A00000076810108A025A33950500800080009A032106039B02E8009C01005F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F410400000000")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }
    }
}
