using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.TestData;
using GlobalPayments.Api.Network.Enums;

namespace GlobalPayments.Api.Tests.Network.Certification
{
    [TestClass]
    public class ContactLessEMVTests
    {
        AcceptorConfig acceptorConfig;
        NetworkGatewayConfig config;
        public ContactLessEMVTests()
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

           acceptorConfig = new AcceptorConfig
            {
                Address = address,

                // data code values
                CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe_KeyEntry, // Inside
                //CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe, // Outside

                CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.PIN,
                CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.ByMerchant,
                TerminalOutputCapability = TerminalOutputCapability.Printing_Display,
                //OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Unattended,//outside
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

            config = new NetworkGatewayConfig(NetworkGatewayType.NWS)
            {
                ServiceUrl = "test.txns-c.secureexchange.net",
                PrimaryPort = 15031,
                SecondaryEndpoint = "test.txns-e.secureexchange.net",
                SecondaryPort = 15031,
                CompanyId = "SPSA",
                TerminalId = "DOTNET25", //"DOTNET25",//"NWSDOTNET01",
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
        public void Test_000_batch_close()
        {
            BatchSummary summary = BatchService.CloseBatch();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary.IsBalanced);
        }
        
        #region Indoor
        [TestMethod]
        public void Test_EMV_Contactless_Credit_Visa_Sale_Indoor_02()
        {
            config.MerchantType = "5541";   //indoor    
            ServicesContainer.ConfigureService(config);
            CreditTrackData track = new CreditTrackData
            {
                Value = ";4761739001010440=22122011966186589?",//Visa
                //Value = ";374245003741005=241220115041234500000?",//Visa Fleet
               // Value = ";374245003741005=321220115041234500000?",//Amex
               
                EntryMethod = EntryMethod.Proximity

            };

            Transaction response = track.Charge(20m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200029F1101014F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401119F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }
        [TestMethod]
        public void Test_EMV_Contactless_Credit_MC_PreAuth_Indoor_03()
        {
            config.MerchantType = "5541";   //indoor    
            ServicesContainer.ConfigureService(config);
            CreditTrackData track = new CreditTrackData
            {
                Value = ";2221100000000122=2512101123456789?",
                EntryMethod = EntryMethod.Proximity

            };

            Transaction response = track.Authorize(50m)
                 .WithCurrency("USD")
                 .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                 .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_EMV_Contactless_Credit_Discover_PreAuth_Completion_Indoor_04()
        {
            config.MerchantType = "5541";   //indoor    
            ServicesContainer.ConfigureService(config);
            CreditTrackData track = new CreditTrackData
            {
                Value = ";6510000000000893=26052011000093100000?",
                EntryMethod = EntryMethod.Proximity

            };

            Transaction response = track.Authorize(50m)
                 .WithCurrency("USD")
                 .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A032406189B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000050009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                 .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);

           Transaction responsePreAuth= response.PreAuthCompletion(50m)
                .WithCurrency("USD")
                 .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A032406189B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000050009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                 .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(responsePreAuth.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(responsePreAuth.SystemTraceAuditNumber);
            Assert.AreEqual("000", responsePreAuth.ResponseCode);
        }
        [TestMethod]
        public void Test_EMV_Contactless_Credit_Indoor_Amex_Refund_04()
        {
            config.MerchantType = "5541";   //indoor
            ServicesContainer.ConfigureService(config);
            CreditTrackData track = new CreditTrackData
            {
                Value = ";374245003741005=321220115041234500000?",
               // PinBlock = "73C41D219768F591",
                EntryMethod = EntryMethod.Proximity
            };

            Transaction response = track.Refund(10m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }
        [TestMethod]
        public void Test_EMV_Contactless_Credit_Visa_Fleet_Sale_Indoor_05()
        {
            config.MerchantType = "5541";   //indoor    
            ServicesContainer.ConfigureService(config);
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Heartland);
            productData.Add("01", Api.Network.Entities.UnitOfMeasure.Gallons, 2m, 5m, 10m);
            FleetData fleetData = new FleetData
            {
                VehicleNumber = "987654",
                OdometerReading = "123456"
            };

            CreditTrackData track = new CreditTrackData
            {
                Value = ";4485580000080017=311220115886224023?",
                EntryMethod = EntryMethod.Proximity

            };

            Transaction response = track.Charge(10m)
                 .WithCurrency("USD")
                 .WithFleetData(fleetData)
                 .WithProductData(productData)
                 .WithTagData("5F280208409F080200024F07A0000000041010500A4D617374657243617264820238008407A00000000410108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                 .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }
        [TestMethod]
        public void Test_EMV_Contactless_Credit_MC_Fleet_Void_Indoor_06()
        {
            config.MerchantType = "5541";   //indoor    
            ServicesContainer.ConfigureService(config);
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Heartland);
            productData.Add("01", Api.Network.Entities.UnitOfMeasure.Gallons, 2m, 5m, 10m);
            FleetData fleetData = new FleetData
            {
                VehicleNumber = "987654",
                OdometerReading = "123456"
            };

           
            CreditTrackData track = new CreditTrackData
            {
                Value = ";5567630000087401=26122019000990000024?",//need MC fleet
                EntryMethod = EntryMethod.Proximity

            };

            Transaction response = track.Charge(10m)
                 .WithCurrency("USD")
                 .WithFleetData(fleetData)
                 .WithProductData(productData)
                 .WithTagData("5F280208409F080200024F07A0000000041010500A4D617374657243617264820238008407A00000000410108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000000410109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                 .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            //Assert.AreEqual("000", response.ResponseCode);

            Transaction responseVoid = response.Void()
                 .WithReferenceNumber("12345")
                 .WithCurrency("USD")
                 .WithTagData("5F280208409F080200024F07A0000000041010500A4D617374657243617264820238008407A00000000410108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000000410109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                 .Execute();
            Assert.IsNotNull(responseVoid);

            // check message data
            PriorMessageInformation pmi = responseVoid.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            //Assert.AreEqual("000900", pmi.ProcessingCode);
            //Assert.AreEqual("441", pmi.FunctionCode);
           // Assert.AreEqual("4351", pmi.MessageReasonCode);
            System.Diagnostics.Debug.WriteLine(responseVoid.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(responseVoid.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("400", responseVoid.ResponseCode);
        }
        [TestMethod]
        public void Test_EMV_Contactless_Credit_Visa_Fleet2_PreAuth_Indoor_07()
        {
            config.MerchantType = "5541";   //indoor    
            ServicesContainer.ConfigureService(config);
            //ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Heartland);
            //productData.Add("01", Api.Network.Entities.UnitOfMeasure.Gallons, 2m, 5m, 10m);
            //FleetData fleetData = new FleetData
            //{
            //    VehicleNumber = "987654",
            //    OdometerReading = "123456"
            //};
            CreditTrackData track = new CreditTrackData
            {
                Value = ";4485580000080017=311220115886224023?",
                EntryMethod = EntryMethod.Proximity

            };

            Transaction response = track.Authorize(10m)
                 .WithCurrency("USD")
                 //.WithFleetData(fleetData)
                 //.WithProductData(productData)
                 .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                 .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }
        [TestMethod]
        public void Test_EMV_Contactless_Visa_Debit_Sale_Cashback_Indoor_08()
        {
            config.MerchantType = "5541";   //indoor     
            ServicesContainer.ConfigureService(config);
            DebitTrackData track = new DebitTrackData
            {
                Value = ";4761739001010135=241220119559045?",
                PinBlock = "73C41D219768F591", //"62968D2481D231E1A504010024A00014",//"73C41D219768F591",
                EntryMethod = EntryMethod.Proximity

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
        public void Test_EMV_Contactless_Credit_Visa_BalanceInquiry_Indoor_09()
        {
            config.MerchantType = "5541";   //indoor    
            ServicesContainer.ConfigureService(config);
            CreditTrackData track = new CreditTrackData
            {
                Value = ";4761739001010440=22122011966186589?",
                EntryMethod = EntryMethod.Proximity

            };

            Transaction response = track.BalanceInquiry()
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200029F1101014F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401119F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }
        [TestMethod]
        public void Test_EMV_Contactless_Credit_MC_CashAdvance_Indoor_10()
        {
            config.MerchantType = "6010";   //indoor    
            ServicesContainer.ConfigureService(config);
            CreditTrackData track = new CreditTrackData
            {
                Value = ";2221100000000122=2512101123456789?",
                EntryMethod = EntryMethod.Proximity

            };

            Transaction response = track.CashAdvance(10m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200029F1101014F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401119F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }
        [TestMethod]
        public void Test_EMV_Contactless_Credit_Amex_Sale_Indoor_11()
        {
            config.MerchantType = "5541";   //indoor    
            ServicesContainer.ConfigureService(config);
            CreditTrackData track = new CreditTrackData
            {
               
                Value = ";374245003741005=321220115041234500000?",//Amex
                EntryMethod = EntryMethod.Proximity

            };

            Transaction response = track.Charge(20m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200029F1101014F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401119F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }
        [TestMethod]
        public void Test_EMV_Contactless_Credit_Indoor_Amex_reversal_Indoor_12()
        {
            config.MerchantType = "5541";   //indoor
            ServicesContainer.ConfigureService(config);
            CreditTrackData track = new CreditTrackData
            {
                Value = ";374245003741005=241220115041234500000?",                
                EntryMethod = EntryMethod.Proximity
            };

            Transaction response = track.Charge(20m)
                 .WithCurrency("USD")
                 .WithTagData("5F280208409F080200029F1101014F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401119F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                 .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction reverse=response.Reverse()
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute();
            Assert.IsNotNull(reverse);
            Assert.AreEqual("400", reverse.ResponseCode);
        }

        [TestMethod]
        public void Test_EMV_Contactless_Credit_Indoor_Visa_Void_Indoor_13()
        {
            config.MerchantType = "5541";   //indoor
            ServicesContainer.ConfigureService(config);
            CreditTrackData track = new CreditTrackData
            {
                Value = ";4761739001010440=22122011966186589?",               
                EntryMethod = EntryMethod.Proximity
            };

            Transaction response = track.Charge(50m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction responseVoid = response.Void()
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute();
            Assert.IsNotNull(responseVoid);
            Assert.AreEqual("400", responseVoid.ResponseCode);
        }

        [TestMethod]
        public void Test_EMV_Contactless_Credit_MC_Voice_Capture_Indoor_14()
        {
            config.MerchantType = "5541";   //indoor    
            ServicesContainer.ConfigureService(config);
            CreditTrackData track = new CreditTrackData
            {
                Value = ";2221100000000122=2512101123456789?",
                EntryMethod = EntryMethod.Proximity

            };


            Transaction transaction = Transaction.FromNetwork(
                        10m,
                        "TYPE04",
                        new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized),
                        track
                );
            Transaction response = transaction.Capture(50m)
                 .WithCurrency("USD")
                 .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                 .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }
        #endregion Indoor
        #region Outdoor
        [TestMethod]
        public void Test_EMV_Contactless_Visa_Credit_Outdoor_16()
        {
            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "ICR");
            acceptorConfig.CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe;
            acceptorConfig.OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Unattended;
            CreditTrackData track = new CreditTrackData
            {
                Value = ";4761739001010440=22122011966186589?",
                EntryMethod = EntryMethod.Proximity
            };

            Transaction response = track.Authorize(50m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200029F1101014F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401119F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);

            //Transaction responseCapture = response.Capture(response.AuthorizedAmount)
            //    .WithCurrency("USD")
            //    .WithTagData("5F280208409F080200029F1101014F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401119F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
            //    .Execute("ICR");
            //Assert.IsNotNull(response);
            //System.Diagnostics.Debug.WriteLine(responseCapture.HostResponseDate);
            //System.Diagnostics.Debug.WriteLine(responseCapture.SystemTraceAuditNumber);
            //Assert.AreEqual("000", responseCapture.ResponseCode);
        }
        [TestMethod]
        public void Test_EMV_Contactless_MC_Credit_PreAuth_Outdoor_17()
        {
            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "ICR");
            acceptorConfig.CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe;
            acceptorConfig.OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Unattended;
            CreditTrackData track = new CreditTrackData
            {
                Value = ";2221100000000122=2512101123456789?",
                EntryMethod = EntryMethod.Proximity
            };

            Transaction response = track.Authorize(50m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200029F1101014F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401119F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }
        [TestMethod]
        public void Test_EMV_Contactless_Discover_Credit_PreAuth_capture_Outdoor_18()
        {
            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "ICR");
            acceptorConfig.CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe;
            acceptorConfig.OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Unattended;
            CreditTrackData track = new CreditTrackData
            {
                Value = ";6510000000000810=26052011000088400000?",
                EntryMethod = EntryMethod.Proximity
            };

            Transaction response = track.Authorize(50m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A032406189B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000050009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction responsePreAuth = response.Capture(50m)
                .WithCurrency("USD")
                 .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A032406189B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000050009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                 .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(responsePreAuth.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(responsePreAuth.SystemTraceAuditNumber);
            Assert.AreEqual("000", responsePreAuth.ResponseCode);
        }
        [TestMethod]
        public void Test_EMV_Contactless_Amex_Credit_PreAuth_Outdoor_19()
        {
            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "ICR");
            acceptorConfig.CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe;
            acceptorConfig.OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Unattended;
            CreditTrackData track = new CreditTrackData
            {
                Value = ";374245003741005=321220115041234500000?",
                EntryMethod = EntryMethod.Proximity
            };

            Transaction response = track.Authorize(50m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200029F1101014F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401119F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }
        [TestMethod]
        public void Test_EMV_Contactless_Visa_Fleet_Credit_PreAuth_Outdoor_20()
        {
            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "ICR");
            acceptorConfig.CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe;
            acceptorConfig.OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Unattended;
            CreditTrackData track = new CreditTrackData
            {
                Value = ";4485580000080017=311220115886224023",
                EntryMethod = EntryMethod.Proximity
            };

            Transaction response = track.Authorize(50m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200029F1101014F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401119F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }
        [TestMethod]
        public void Test_EMV_Contactless_MC_Credit_PreAuth_Completion_Outdoor_21()
        {
            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "ICR");
            acceptorConfig.CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe;
            acceptorConfig.OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Unattended;
            CreditTrackData track = new CreditTrackData
            {
               // 5567300000000016 = 25121019999888877724

                Value = ";2221100000000122=2512101123456789?",//need MC fleet
                EntryMethod = EntryMethod.Proximity
            };

            Transaction response = track.Authorize(50m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000000041010500A4D617374657243617264820238008407A00000000410108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000000410109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction responsePreAuth = response.Capture(50m)
                .WithCurrency("USD")
                 .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A032406189B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000050009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                 .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(responsePreAuth.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(responsePreAuth.SystemTraceAuditNumber);
            Assert.AreEqual("000", responsePreAuth.ResponseCode);
        }
        [TestMethod]
        public void Test_EMV_Contactless_Visa_Fleet2_Credit_PreAuth_Outdoor_22()
        {
            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "ICR");
            acceptorConfig.CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe;
            acceptorConfig.OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Unattended;
            CreditTrackData track = new CreditTrackData
            {
                Value = ";4485580000080017=311220115886224023?",
                EntryMethod = EntryMethod.Proximity
            };

            Transaction response = track.Authorize(50m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200029F1101014F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401119F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }
        //[TestMethod]
        //public void Test_EMV_Contactless_MC_Debit_PreAuth_Outdoor_24()
        //{
        //    config.MerchantType = "5542";
        //    ServicesContainer.ConfigureService(config, "ICR");
        //    acceptorConfig.CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe;
        //    acceptorConfig.OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Unattended;
        //    DebitTrackData track = new DebitTrackData
        //    {
        //        Value = "720002123456789=2512120000000000001?",
        //        PinBlock = "73C41D219768F591",
        //        EntryMethod = EntryMethod.Proximity
        //    };

        //    Transaction response = track.Authorize(50m)
        //        .WithCurrency("USD")
        //        .WithTagData("5F280208409F080200029F1101014F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401119F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
        //        .Execute("ICR");
        //    Assert.IsNotNull(response);
        //    System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
        //    System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        //    Assert.AreEqual("000", response.ResponseCode);
        //}
        //[TestMethod]
        //public void Test_EMV_Contactless_MC_Debit_PreAuth_Completion_Outdoor_23()
        //{
        //    config.MerchantType = "5542";
        //    ServicesContainer.ConfigureService(config, "ICR");
        //    acceptorConfig.CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe;
        //    acceptorConfig.OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Unattended;
        //    DebitTrackData track = new DebitTrackData
        //    {
        //        Value = "720002123456789=2512120000000000001?",
        //        PinBlock = "73C41D219768F591",
        //        EntryMethod = EntryMethod.Proximity
        //    };

        //    Transaction response = track.Authorize(50m)
        //        .WithCurrency("USD")
        //        .WithTagData("5F280208409F080200029F1101014F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401119F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
        //        .Execute("ICR");
        //    Assert.IsNotNull(response);
        //    System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
        //    System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        //    Assert.AreEqual("000", response.ResponseCode);

        //    Transaction responsePreAuth = response.Capture(50m)
        //        .WithCurrency("USD")
        //         .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
        //         .Execute();
        //    Assert.IsNotNull(response);
        //    System.Diagnostics.Debug.WriteLine(responsePreAuth.HostResponseDate);
        //    System.Diagnostics.Debug.WriteLine(responsePreAuth.SystemTraceAuditNumber);
        //    Assert.AreEqual("000", responsePreAuth.ResponseCode);
        //}
        [TestMethod]
        public void Test_EMV_Contactless_MC_Debit_PreAuth_Capture_Outdoor_23()
        {
            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "ICR");
            acceptorConfig.CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe;
            acceptorConfig.OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Unattended;
            DebitTrackData track = new DebitTrackData
            {
                Value = "720002123456789=2512120000000000001?",
                PinBlock = "73C41D219768F591",
                EntryMethod = EntryMethod.Proximity
            };

            Transaction response = track.Authorize(50m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200029F1101014F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401119F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction responseCapture = response.Capture(response.AuthorizedAmount)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200029F1101014F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401119F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(responseCapture.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(responseCapture.SystemTraceAuditNumber);
            Assert.AreEqual("000", responseCapture.ResponseCode);
        }
        [TestMethod]
        public void Test_EMV_Contactless_Visa_Fleet_Credit_PreAuth_Completion_Outdoor_24()
        {
            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "ICR");
            acceptorConfig.CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe;
            acceptorConfig.OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Unattended;
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Heartland);
            productData.Add("01", Api.Network.Entities.UnitOfMeasure.Gallons, 2m, 5m, 10m);
            FleetData fleetData = new FleetData
            {
                VehicleNumber = "987654",
                OdometerReading = "123456"
            };

            CreditTrackData track = new CreditTrackData
            {
                Value = ";4485580000080017=311220115886224023?",
                EntryMethod = EntryMethod.Proximity

            };

            Transaction response = track.Authorize(10m)
                .WithCurrency("USD")
                .WithFleetData(fleetData)
                .WithTagData("5F280208409F080200024F07A0000000041010500A4D617374657243617264820238008407A00000000410108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000000410109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction responsePreAuth = response.Capture(10m)

                .WithCurrency("USD")
                .WithFleetData(fleetData)
                 .WithProductData(productData)
                 .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A032406189B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000050009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                 .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(responsePreAuth.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(responsePreAuth.SystemTraceAuditNumber);
            Assert.AreEqual("000", responsePreAuth.ResponseCode);
        }
        [TestMethod]
        public void Test_EMV_Contactless_Amex_Credit_PreAuth_Completion_Outdoor_25()
        {
            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "ICR");
            acceptorConfig.CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe;
            acceptorConfig.OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Unattended;
            CreditTrackData track = new CreditTrackData
            {
                Value = ";374245003741005=321220115041234500000?",
                EntryMethod = EntryMethod.Proximity
            };

            Transaction response = track.Authorize(50m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000000041010500A4D617374657243617264820238008407A00000000410108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000000410109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction responsePreAuth = response.Capture(50m)
                .WithCurrency("USD")
                 .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A032406189B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000050009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(responsePreAuth.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(responsePreAuth.SystemTraceAuditNumber);
            Assert.AreEqual("000", responsePreAuth.ResponseCode);
        }
        #endregion Outdoor
    }
}
