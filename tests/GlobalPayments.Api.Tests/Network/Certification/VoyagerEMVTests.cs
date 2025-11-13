using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitOfMeasure = GlobalPayments.Api.Network.Entities.UnitOfMeasure;

namespace GlobalPayments.Api.Tests.Network.Certification
{
    [TestClass]
    public class VoyagerEMVTests
    {
        private CreditCardData card;
        private CreditTrackData track;
        private ProductData productData;
        private FleetData fleetData;
        NetworkGatewayConfig config;

        public VoyagerEMVTests()
        {
            AcceptorConfig acceptorConfig = new AcceptorConfig
            {
                // data code values
                CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe_KeyEntry,
                CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.None,
                TerminalOutputCapability = TerminalOutputCapability.Printing_Display,
                CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.ByMerchant,
                //OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Unattended,
                // hardware software config values
                HardwareLevel = "34",
                SoftwareLevel = "21205710",

                // pos configuration values
                SupportsPartialApproval = false,
                SupportsShutOffAmount = true,
                SupportsReturnBalance = true,
                SupportsDiscoverNetworkReferenceId = true,
                SupportsAvsCnvVoidReferrals = true,
                SupportedEncryptionType = EncryptionType.TEP2
            };

            // gateway config
            config = new NetworkGatewayConfig(NetworkGatewayType.NWS)
            {
                ServiceUrl = "test.txns-c.secureexchange.net",
                PrimaryPort = 15031,
                SecondaryEndpoint = "test.txns-e.secureexchange.net",
                SecondaryPort = 15031,
                CompanyId = "SPSA",
                TerminalId = "DOTNET25",
                UniqueDeviceId = "0001",
                AcceptorConfig = acceptorConfig,
                EnableLogging = true,
                StanProvider = StanGenerator.GetInstance(),
                BatchProvider = BatchProvider.GetInstance()
            };


            ServicesContainer.ConfigureService(config);


            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "ICR");
            // MASTERCARD FLEET
            //card = TestCards.MasterCardFleetManual(true, true);
            //track = TestCards.MasterCardFleetSwipe();
            //productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            //productData.Add("03", UnitOfMeasure.Gallons, 1, 10);

            // VOYAGER FLEET
            card = TestCards.VoyagerFleetManual(true, true);
            track = TestCards.VoyagerFleetSwipe();

            fleetData = new FleetData
            {
                //ServicePrompt = "0",
                DriverId = "11411",
                VehicleNumber = "22031",
                OdometerReading = "1256"
            };

            // VISA
            //card = TestCards.VisaFleetManual(true, true);
            //track = TestCards.VisaFleetSwipe();
            //productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            //productData.Add("01", UnitOfMeasure.Gallons, 1, 10);

            //FleetOne
            //card = TestCards.FleetOneManual(true, true);
            //track = TestCards.FleetOneSwipe();

            //FleetCor Fuelman
            //card = TestCards.FuelmanManual(true, true);
            //track = TestCards.FuelmanSwipe();

            // FleetCor Fleetwide
            //card = TestCards.FleetWideManual(true, true);
            //track = TestCards.FleetWideSwipe();
        }

        #region Voyager EMV
        [TestMethod]
        public void Voyager_EMV_Contact_Refund_05()
        {
            track.Value = ";7088850950270000131=32010000010100600?";
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.IssuerSpecific);
            productData.Add("01", UnitOfMeasure.Gallons, 5m, 5m, 10m);
            fleetData = new FleetData
            {
                //only 3 and 4 for voyager
                DriverId = "11411",
                OdometerReading = "1256"
            };
            config.MerchantType = "5541";
            ServicesContainer.ConfigureService(config);
            Transaction response = track.Refund(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .WithTagData("4F07A0000000041010500A4D617374657243617264820238008407A00000000410108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000000410109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10120110A0800F22000065C800000000000000FF9F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                        .Execute();
            Assert.IsNotNull(response);
            // DE_063: 02102F00101\G05\05\1000\
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("200009", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }
        //[TestMethod]
        //public void Voyager_EMV_Contactless_Refund()
        //{
        //    ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
        //    productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
        //   // track.EntryMethod = EntryMethod.Proximity;
        //    track.Value = ";7088850950270000131=32010000010100600?";
        //   // track.PinBlock = "73C41D219768F591";
        //    Transaction response = track.Refund(10m)
        //                .WithCurrency("USD")
        //                .WithFleetData(fleetData)
        //                .WithProductData(productData)
        //                .WithTagData("4F0AA0000000049999C0001682023900840AA0000000049999C000168A025A33950500800080009A032021039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F4104000000009F6E1308400003030001012059876123400987612340")
        //                .Execute();
        //    Assert.IsNotNull(response);

        //    // check message data
        //    PriorMessageInformation pmi = response.MessageInformation;
        //    Assert.IsNotNull(pmi);
        //    Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
        //    Assert.AreEqual("200009", pmi.ProcessingCode);
        //    Assert.AreEqual("200", pmi.FunctionCode);
        //    System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
        //    System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        //    // check response
        //    Assert.AreEqual("000", response.ResponseCode);
        //}
        [TestMethod]
        public void Voyager_EMV_Authorization_9F6E()
        {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 5m, 5m, 10m);
            fleetData = new FleetData
            {
                //only 3 and 4 for voyager
                DriverId = "11411",
                OdometerReading = "1256"
            };
            config.MerchantType = "5541";
            ServicesContainer.ConfigureService(config);

            track.Value = ";7088850950270000131=32010000010100600?";

            Transaction response = track.Authorize(10m)
                   .WithCurrency("USD")
                   .WithFleetData(fleetData)
                   .WithTagData("4F0AA0000000049999C0001682023900840AA0000000049999C000168A025A33950500800080009A032021039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000000410109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F4104000000009F6E1308400003030001012059876123400987612340")
                   .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            //Assert.AreEqual("101", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);

            // check response
            Assert.AreEqual("000", response.ResponseCode);

            //partial approval cancellation
            //    Transaction reversal = response.Cancel()
            //            .WithReferenceNumber(response.ReferenceNumber)
            //            .Execute();
            //Assert.IsNotNull(reversal);

            //    pmi = reversal.MessageInformation;
            //    Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            //    Assert.AreEqual("000900", pmi.ProcessingCode);
            //    Assert.AreEqual("441", pmi.FunctionCode);
            //    Assert.AreEqual("4352", pmi.MessageReasonCode);

            //    Assert.AreEqual("400", reversal.ResponseCode);
            //    System.Diagnostics.Debug.WriteLine(reversal.HostResponseDate);
            //    System.Diagnostics.Debug.WriteLine(reversal.SystemTraceAuditNumber);
            // Test_009


            //Transaction captureResponse = response.Capture(10m)
            //        .WithCurrency("USD")
            //        .WithProductData(productData)
            //        .WithFleetData(fleetData)
            //        //.Execute();
            //        .Execute("ICR");
            //Assert.IsNotNull(captureResponse);

            //// check message data
            //pmi = captureResponse.MessageInformation;
            //Assert.IsNotNull(pmi);
            //Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            //Assert.AreEqual("000900", pmi.ProcessingCode);
            ////Assert.AreEqual("201", pmi.FunctionCode);
            //System.Diagnostics.Debug.WriteLine(captureResponse.HostResponseDate);
            //System.Diagnostics.Debug.WriteLine(captureResponse.SystemTraceAuditNumber);
            //// check response
            //Assert.AreEqual("000", captureResponse.ResponseCode);
        }
        [TestMethod]
        public void Voyager_EMV_Sale()
        {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 25.000m, 1.000m, 25.00m);
            fleetData = new FleetData
            {
                //only 3 and 4 for voyager
                DriverId = "11411",
                OdometerReading = "1256"
            };
            config.MerchantType = "5541";
            ServicesContainer.ConfigureService(config);
            track.Value = ";7088850950270000131=32010000010100600?";
            Transaction sale = track.Charge(25.00m)
                   .WithCurrency("USD")
                   .WithProductData(productData)
                   .WithFleetData(fleetData)
                   .WithTagData("4F0AA0000000049999C0001682023900840AA0000000049999C000168A025A33950500800080009A032021039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000000410109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F4104000000009F6E1308400003030001012059876123400987612340")
                   .Execute();
            Assert.IsNotNull(sale);
            Assert.AreEqual("000", sale.ResponseCode);


        }
        [TestMethod]
        public void Voyager_EMV_Authorization_Capture()
        {
            //Auth capture will be outdoor,Auth does not require productdata
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.IssuerSpecific);
            productData.Add("01", UnitOfMeasure.Gallons, 10.720m, 3.4664m, 37.15m);
            fleetData = new FleetData
            {
                //only 3 and 4 for voyager
                DriverId = "11411",
                OdometerReading = "1256"
            };
            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "ICR");
            Transaction response = track.Authorize(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithTagData("4F0AA0000000049999C0001682023900840AA0000000049999C000168A025A33950500800080009A032021039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F4104000000009F6E1308400003030001012059876123400987612340")
                        .Execute();
            //.Execute("ICR");
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            //Assert.AreEqual("101", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);

            // partial approval cancellation
            //Transaction reversal = response.Cancel()
            //        .WithReferenceNumber(response.ReferenceNumber)
            //        .Execute();
            //Assert.IsNotNull(reversal);

            //pmi = reversal.MessageInformation;
            //Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            //Assert.AreEqual("000900", pmi.ProcessingCode);
            //Assert.AreEqual("441", pmi.FunctionCode);
            //Assert.AreEqual("4352", pmi.MessageReasonCode);

            //Assert.AreEqual("400", reversal.ResponseCode);
            //System.Diagnostics.Debug.WriteLine(reversal.HostResponseDate);
            //System.Diagnostics.Debug.WriteLine(reversal.SystemTraceAuditNumber);
            // Test_009


            Transaction captureResponse = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F0AA0000000049999C0001682023900840AA0000000049999C000168A025A33950500800080009A032021039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F4104000000009F6E1308400003030001012059876123400987612340")
                    .Execute();
            //.Execute("ICR");
            Assert.IsNotNull(captureResponse);

            // check message data
            pmi = captureResponse.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            //Assert.AreEqual("201", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(captureResponse.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(captureResponse.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }
        [TestMethod]
        public void Voyager_EMV_Sale_Void()
        {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 5m, 5m, 10m);
            fleetData = new FleetData
            {
                //only 3 and 4 for voyager
                DriverId = "11411",
                OdometerReading = "1256"
            };
            config.MerchantType = "5541";
            ServicesContainer.ConfigureService(config);
            Transaction sale = track.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F0AA0000000049999C0001682023900840AA0000000049999C000168A025A33950500800080009A032021039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F4104000000009F6E1308400003030001012059876123400987612340")
                    .Execute();
            Assert.IsNotNull(sale);
            Assert.AreEqual("000", sale.ResponseCode);

            Transaction response = sale.Void(amount: sale.AuthorizedAmount)
                    .WithReferenceNumber("12345")
                    .WithTagData("4F0AA0000000049999C0001682023900840AA0000000049999C000168A025A33950500800080009A032021039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F4104000000009F6E1308400003030001012059876123400987612340")
                    //.WithAuthenticatioNMethod(CardHolderAuthenticationMethod.ManualSignatureVerification)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("441", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("400", response.ResponseCode);
        }
        [TestMethod]
        
        public void Voyager_EMV_Authorization_Timeout_with_Reversal_02()
        {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 5m, 5m, 10m);
            fleetData = new FleetData
            {
                //only 3 and 4 for voyager
                DriverId = "11411",
                OdometerReading = "1256"
            };
            config.MerchantType = "5541";
            ServicesContainer.ConfigureService(config);

            track.Value = ";7088850950270000131=32010000010100600?";

            Transaction response = track.Authorize(10m)
                   .WithCurrency("USD")
                   .WithFleetData(fleetData)
                   .WithTagData("4F0AA0000000049999C0001682023900840AA0000000049999C000168A025A33950500800080009A032021039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000000410109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F4104000000009F6E1308400003030001012059876123400987612340")
                   .Execute();
            Assert.IsNotNull(response);

            // check message data
            //PriorMessageInformation pmi = response.MessageInformation;
            //Assert.IsNotNull(pmi);
            //Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            //Assert.AreEqual("000900", pmi.ProcessingCode);
            ////Assert.AreEqual("101", pmi.FunctionCode);
            //System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            //System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);

            // check response
            //  Assert.AreEqual("000", response.ResponseCode);
            Transaction transaction = Transaction.FromNetwork(
                        10m,
                        "TYPE04",
                        new NtsData(FallbackCode.Received_IssuerTimeout, AuthorizerCode.Terminal_Authorized),
                        track,
                        "1220",
                        "0000",
                        "0000",
                       "000900"

                );
            Transaction reversal=transaction.Reverse(10m)
                .WithCurrency("USD")
                .WithFleetData(fleetData)
               // .WithProductData(productData)
                .WithTagData("4F0AA0000000049999C0001682023900840AA0000000049999C000168A025A33950500800080009A032021039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000000410109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F4104000000009F6E1308400003030001012059876123400987612340")
                .Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("400", reversal.ResponseCode);
        }
        [TestMethod]
        public void Voyager_EMV_Sale_Reverse()
        {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 5m, 5m, 10m);
            fleetData = new FleetData
            {
                //only 3 and 4 for voyager
                DriverId = "11411",
                OdometerReading = "1256"
            };
            config.MerchantType = "5541";
            ServicesContainer.ConfigureService(config);
            Transaction sale = track.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTagData("4F0AA0000000049999C0001682023900840AA0000000049999C000168A025A33950500800080009A032021039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F4104000000009F6E1308400003030001012059876123400987612340")
                    .Execute();
            Assert.IsNotNull(sale);
           // Assert.AreEqual("000", sale.ResponseCode);

            Transaction response = sale.Reverse()
                   // .WithReferenceNumber("12345")
                   .WithFleetData(fleetData)
                    .WithTagData("4F0AA0000000049999C0001682023900840AA0000000049999C000168A025A33950500800080009A032021039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000076810109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F4104000000009F6E1308400003030001012059876123400987612340")
                    //.WithAuthenticatioNMethod(CardHolderAuthenticationMethod.ManualSignatureVerification)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("441", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("400", response.ResponseCode);
        }
        [TestMethod]
        public void Voyager_EMV_Reverse() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 5m, 5m, 10m);
            fleetData = new FleetData
            {
                //only 3 and 4 for voyager
                DriverId = "11411",
                OdometerReading = "1256"
            };
            config.MerchantType = "5541";
            ServicesContainer.ConfigureService(config);

            track.Value = ";7088850950270000131=32010000010100600?";
            Transaction transaction = Transaction.FromNetwork(
                        10m,
                        "TYPE04",
                        new NtsData(FallbackCode.Received_IssuerTimeout, AuthorizerCode.Terminal_Authorized),
                        track,
                        "1220",
                        "0000",
                        "0000",
                       "000900"
                );
            Transaction reversal = transaction.Reverse(10m)
                .WithCurrency("USD")
                .WithFleetData(fleetData)
                .WithProductData(productData)
                .WithTagData("4F0AA0000000049999C0001682023900840AA0000000049999C000168A025A33950500800080009A032021039B02E8009C01005F24032212315F280208405F2A0208405F3401029F02060000000001009F03060000000000009F0607A00000000410109F07023D009F080201539F090200019F0D05BC308088009F1A0208409F0E0500400000009F0F05BCB08098009F10200FA502A830B9000000000000000000000F0102000000000000000000000000009F2103E800259F2608DD53340458AD69B59F2701809F34031E03009F3501169F3303E0F8C89F360200019F37045876B0989F3901009F4005F000F0A0019F4104000000009F6E1308400003030001012059876123400987612340")
                .Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("400", reversal.ResponseCode);
        }
        #endregion
    }
}
