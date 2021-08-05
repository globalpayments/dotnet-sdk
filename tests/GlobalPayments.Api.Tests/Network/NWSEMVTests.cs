using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.TestData;
using GlobalPayments.Api.Network.Enums;

namespace GlobalPayments.Api.Tests.Network
{
    [TestClass]
    public class NWSEMVTests
    {
        public NWSEMVTests()
        {
            Address address = new Address
            {
                Name = "My STORE",
                StreetAddress1 = "1 MY STREET",
                City = "MYTOWN",
                PostalCode = "90210",
                State = "KY",
                Country = "USA"
            };

            AcceptorConfig acceptorConfig = new AcceptorConfig
            {
                Address = address,

                // data code values
                CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe_KeyEntry, // Inside
                //CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe, // Outside

                CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.PIN,
                CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.ByMerchant,
                TerminalOutputCapability = TerminalOutputCapability.Printing_Display,
                //OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Unattended,
                //
                // hardware software config values
                HardwareLevel = "34",
                SoftwareLevel = "21205710",

                // pos configuration values
                SupportsPartialApproval = false,
                SupportsShutOffAmount = true,
                SupportsReturnBalance = true,
                SupportsDiscoverNetworkReferenceId = true,
                SupportsAvsCnvVoidReferrals = true,
                SupportsEmvPin = true,

                // DE 43-34 Message Data
                EchoSettlementData = true
            };

            // gateway config

            NetworkGatewayConfig config = new NetworkGatewayConfig(NetworkGatewayType.NWS)
            {
                ServiceUrl = "test.txns-c.secureexchange.net",
                PrimaryPort = 15031,
                SecondaryEndpoint = "test.txns-e.secureexchange.net",
                SecondaryPort = 15031,
                CompanyId = "SPSA",
                TerminalId = "NWSDOTNET21",
                UniqueDeviceId = "0001",
                AcceptorConfig = acceptorConfig,
                EnableLogging = true,
                StanProvider = StanGenerator.GetInstance(),
                BatchProvider = BatchProvider.GetInstance()
            };

            ServicesContainer.ConfigureService(config);

            // with merchant type
            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "ICR");
        }

        // Indoor tests
        // Make sure AcceptorConfig.CardDataInputCapability is set to ContactlessEmv_ContactEmv_MagStripe_KeyEntry (Inside)
        [TestMethod]
        public void Test_Indoor_MC_MTip40_Test01_Scenario01() {
            CreditTrackData track = new CreditTrackData
            {
                Value = ";5567630000088409=49126010793608000024?",
                PinBlock = "62968D2481D231E1A504010024A00014"
            };

            FleetData fleetData = new FleetData
            {
                VehicleNumber = "987654",
                OdometerReading = "123456"
            };
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Heartland);
            productData.Add("01", Api.Network.Entities.UnitOfMeasure.Gallons, 4m, 5m, 20m);

            Transaction response = track.Charge(20m)
                    .WithCurrency("USD")
                    .WithFleetData(fleetData)
                    .WithProductData(productData)
                    .WithTagData("5F280208409F080200029F1101015A0855673000000000164F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Indoor_Visa_ADVT_5A_1() {
            CreditTrackData track = new CreditTrackData
            {
                Value = ";4761739001010143=241220111478183?",
                PinBlock = "62968D2481D231E1A504010024A00014"
                //PinBlock = "93001E8971E7C20F"
            };

            Transaction response = track.Charge(31m)
                    .WithCurrency("USD")
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Indoor_Visa_ADVT_1A_2()
        {
            DebitTrackData track = new DebitTrackData
            {
                Value = ";4761739001010135=241220119559045?",
                PinBlock = "62968D2481D231E1A504010024A00014"
                //PinBlock = "93001E8971E7C20F"
            };

            Transaction response = track.Charge(20m)
                    .WithCurrency("USD")
                    .WithCashBack(10m)
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Indoor_Amex_AXP_EMV_002() {
            CreditTrackData track = new CreditTrackData
            {
                Value = ";374245002741006=241222117101234500000?",
                PinBlock = "62968D2481D231E1A504010024A00014"
            };

            Transaction response = track.Charge(31m)
                    .WithCurrency("USD")
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Indoor_Discover_E2E_19() {
            CreditTrackData track = new CreditTrackData
            {
                Value = ";6510000000000182=23122011000079900000?",
                PinBlock = "62968D2481D231E1A504010024A00014"
            };

            Transaction response = track.Charge(20m)
                    .WithCurrency("USD")
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Indoor_Discover_E2E_50() {
            DebitTrackData track = new DebitTrackData
            {
                Value = ";6510000000000018=23122011000010600000?",
                PinBlock = "62968D2481D231E1A504010024A00014"
            };

            Transaction response = track.Charge(79m)
                    .WithCurrency("USD")
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        // Outdoor tests
        // Make sure AcceptorConfig.CardDataInputCapability is set to ContactlessEmv_ContactEmv_MagStripe (Outside)
        [TestMethod]
        public void Test_Outdoor_MC_MTip15_Test01_Scenario02() {
            CreditTrackData track = new CreditTrackData
            {
                Value = ";5413330089020094=2512201029570106?"
            };

            Transaction response = track.Charge(40m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200029F1101015A0854133300890200944F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000040009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Outdoor_MC_MTip13_USM_Test01_Scenario01() {
            DebitTrackData track = new DebitTrackData
            {
                Value = ";5413330089099056=2512?",
                PinBlock = "62968D2481D231E1A504010024A00014"
                //PinBlock = "73C41D219768F591"
            };

            Transaction response = track.Charge(10m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200025A0854133300890990564F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000010009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34034203009F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Outdoor_MC_MTip13_USM_Test01_Scenario01f()
        {
            DebitTrackData track = new DebitTrackData
            {
                Value = ";5413330089099056=2512?",
                PinBlock = "62968D2481D231E1A504010024A00014"
                //PinBlock = "73C41D219768F591"
            };

            Transaction response = track.Charge(10m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200025A0854133300890990564F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000000009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34034203009F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Outdoor_Visa_Test_Case_21() {
            CreditTrackData track = new CreditTrackData
            {
                Value = ";4761739001010440=22122011966186589?"
            };

            Transaction response = track.Charge(20m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200029F1101014F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401119F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Outdoor_Amex_AXP_AFD_013() {
            CreditTrackData track = new CreditTrackData
            {
                Value = ";374245003741005=241220115041234500000?"
            };

            Transaction response = track.Authorize(50m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Outdoor_Discover_E2E_04() {
            CreditTrackData track = new CreditTrackData
            {
                Value = ";6510000000000133=23122011000036200000?"
            };

            Transaction response = track.Authorize(1m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Outdoor_Discover_E2E_09() {
            CreditTrackData track = new CreditTrackData
            {
                Value = ";6510000000000091=23122011000052400000?"
            };

            Transaction response = track.Authorize(1m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }
    }
}
