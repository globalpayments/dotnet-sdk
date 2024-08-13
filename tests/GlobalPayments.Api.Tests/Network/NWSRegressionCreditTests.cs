using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitOfMeasure = GlobalPayments.Api.Network.Entities.UnitOfMeasure;

namespace GlobalPayments.Api.Tests.Network
{
    [TestClass]
    public class NWSRegressionCreditTests {
        private CreditCardData card;
        private CreditTrackData track;
        private FleetData fleetData;

        public NWSRegressionCreditTests() {
            Address address = new Address {
                Name = "My STORE",
                StreetAddress1 = "1 MY STREET",
                City = "MYTOWN",
                PostalCode = "90210",
                State = "KY",
                Country = "USA"
            };

            AcceptorConfig acceptorConfig = new AcceptorConfig {
                Address = address,

                // data code values
                CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe_KeyEntry, // Inside

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

                // DE 43-34 Message Data
                EchoSettlementData = true
            };

            // gateway config
            NetworkGatewayConfig config = new NetworkGatewayConfig(NetworkGatewayType.NWS) {
                ServiceUrl = "test.txns-c.secureexchange.net",
                PrimaryPort = 15031,
                SecondaryEndpoint = "test.txns-e.secureexchange.net",
                SecondaryPort = 15031,
                CompanyId = "SPSA",
                TerminalId = "NWSDOTNET01",// "NWSBATCH2002",// "NWSDOTNET01",
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

            fleetData = new FleetData {
                //ServicePrompt = "0",
                DriverId = "11411",
                VehicleNumber = "22031",
                OdometerReading = "1256"
            };

            // VISA
            card = TestCards.VisaManual(true, true);
            track = TestCards.VisaSwipe();
        }

        [TestMethod]
        public void Test_Credit_Auth_Visa() {
            Transaction response = track.Authorize(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Credit_Auth_Capture_Visa() {
            Transaction response = track.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            // test_019
            Transaction captureResponse = response.Capture()
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Credit_MS_Refund() {
            track = TestCards.MasterCardSwipe();
            Transaction response = track.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Credit_Sale_Void_Discover() {
            track = TestCards.DiscoverSwipe();
            Transaction response = track.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            // void the transaction test case #7
            Transaction voidResponse = response.Void().Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Credit_Sale_Reversal_Amex() {
            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);

            track.CardType = "Amex";
            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            response.NTSData = ntsData;

            // void the transaction test case #8
            Transaction reverseResponse = response.Reverse().Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_FleetCor_Sale() {
            track = TestCards.FleetWideSwipe();
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Heartland);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Debit_Sale_Response_Tests() {
            // debit card
            DebitTrackData debitTrack = new DebitTrackData {
                Value = "4355567063338=2012101HJNw/ewskBgnZqkL",
                PinBlock = "62968D2481D231E1A504010024A00014",
                EncryptionData = EncryptionData.Version2("/wECAQEEAoFGAgEH4gcOTDT6jRZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0m+/d4SO9TEshhRGUUQzVBrBvP/Os1qFx+6zdQp1ejjUCoDmzoUMbil9UG73zBxxTOy25f3Px0p8joyCh8PEWhADz1BkROJT3q6JnocQE49yYBHuFK0obm5kqUcYPfTY09vPOpmN+wp45gJY9PhkJF5XvPsMlcxX4/JhtCshegz4AYrcU/sFnI+nDwhy295BdOkVN1rn00jwCbRcE900kj3UsFfyc", "2")
            };

            Transaction response = debitTrack.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
        }

        //EMV Related Test Cases
        [TestMethod]
        public void Test_EMV_MasterCard_Auth() {
            CreditTrackData track = new CreditTrackData {
                Value = "%B2223000010005780^TEST CARD/EMV BIN-2^19121010000000009210?;2223000010005780=19121010000000009210?"
            };

            Transaction response = track.Authorize(50m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_EMV_Mastercard_AuthCapture() {
            CreditTrackData track = new CreditTrackData {
                Value = "%B2223000010005780^TEST CARD/EMV BIN-2^19121010000000009210?;2223000010005780=19121010000000009210?"
            };

            Transaction response = track.Authorize(10m, true)
                    .WithCurrency("USD")
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
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

            Transaction captureResponse = response.Capture(response.AuthorizedAmount)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(captureResponse);

            // check message data
            pmi = captureResponse.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("1376", pmi.MessageReasonCode);
            Assert.AreEqual("201", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_EMV_MasterCard_Credit_Void() {
            CreditTrackData track = new CreditTrackData {
                Value = "%B2223000010005780^TEST CARD/EMV BIN-2^19121010000000009210?;2223000010005780=19121010000000009210?"
            };

            Transaction response = track.Charge(10m)
                        .WithCurrency("USD")
                        .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            // reverse the transaction
            Transaction reverseResponse = response.Void().Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);

        }

        [TestMethod]
        public void Test_EMV_Discover_Refund() {
            CreditTrackData track = new CreditTrackData {
                Value = ";374245003741005=241220115041234500000?"
            };

            Transaction response = track.Refund(50m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_EMV_Visa_Sale_Reversal() {
            CreditTrackData track = new CreditTrackData {
                Value = ";374245003741005=241220115041234500000?"
            };

            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);
            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute();
            Assert.IsNotNull(response);

            response.NTSData = ntsData;
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            Transaction reversal = response.Reverse(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(reversal);

            // check message data
            pmi = reversal.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("400", pmi.FunctionCode);
            Assert.AreEqual("4021", pmi.MessageReasonCode);

            // check response
            Assert.AreEqual("400", reversal.ResponseCode);
        }

        [TestMethod]
        public void Test_EMV_Debit_Sale() {
            DebitTrackData track = new DebitTrackData {
                Value = ";4761739001010135=241220119559045?",
                PinBlock = "62968D2481D231E1A504010024A00014"
            };

            Transaction response = track.Charge(20m)
                    .WithCurrency("USD")
                    .WithCashBack(10m)
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_EMV_AMex_Credit_Sale() {
            CreditTrackData rvalue = new CreditTrackData {
                Value = "%B3727 006992 51018^AMEX TEST CARD^2112990502700?;372700699251018=2112990502700?",
                EntryMethod = EntryMethod.Swipe
            };

            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
    }
}
