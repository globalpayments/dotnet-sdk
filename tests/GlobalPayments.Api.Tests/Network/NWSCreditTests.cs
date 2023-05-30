using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.TestData;
using GlobalPayments.Api.Network.Enums;

namespace GlobalPayments.Api.Tests.Network {
    [TestClass]
    public class NWSCreditTests {
        private CreditCardData card;
        private CreditTrackData track;
        //private CreditCardData readylinkCard;
        private DebitTrackData readylinkTrack;

        public NWSCreditTests() {
            Address address = new Address
            {
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
                TerminalId = "NWSDOTNET01",
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

            // VISA
            card = TestCards.VisaManual(true, true);
            track = TestCards.VisaSwipe();

            // VISA CORPORATE
            //card = TestCards.VisaCorporateManual(true, true);
            //track = TestCards.VisaCorporateSwipe();

            // VISA PURCHASING
            //card = TestCards.VisaPurchasingManual(true, true);
            //track = TestCards.VisaPurchasingSwipe();

            // MASTERCARD
            //card = TestCards.MasterCardManual(true, true);
            //track = TestCards.MasterCardSwipe();

            // MASTERCARD PURCHASING
            //card = TestCards.MasterCardPurchasingManual(true, true);
            //track = TestCards.MasterCardPurchasingSwipe();

            // MASTERCARD 2
            //card = TestCards.MasterCard2Manual(true, true);
            //track = TestCards.MasterCard2Swipe();

            // AMEX
            //card = TestCards.AmexManual(true, true);
            //track = TestCards.AmexSwipe();

            // DISCOVER
            //card = TestCards.DiscoverManual(true, true);
            //track = TestCards.DiscoverSwipe();

            //Visa ReadyLink
            //readylinkCard = TestCards.VisaReadyLinkManual(true, true);
            readylinkTrack = TestCards.VisaReadyLinkSwipe();
        }

        [TestMethod]
        public void Test_000_batch_close() {
            BatchSummary summary = BatchService.CloseBatch(BatchCloseType.Forced);
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary.IsBalanced);
        }

        [TestMethod]
        public void Test_003_manual_authorization() {
            Transaction response = card.Authorize(10m, true)
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
            
        }

        [TestMethod]
        public void Test_004_manual_sale() {
            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            
            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction reversal = response.Reverse(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(reversal);
            System.Diagnostics.Debug.WriteLine("Reversal:");
            System.Diagnostics.Debug.WriteLine(reversal.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(reversal.SystemTraceAuditNumber);

        }

        [TestMethod]
        public void Test_005_swipe_verify() {
            Transaction response = track.Verify().Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("313000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_006_swipe_authorization() {
            Transaction response = track.Authorize(10m, false)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            //Assert.AreEqual("003000", pmi.ProcessingCode);
            //Assert.AreEqual("000", response.ResponseCode);

            //Assert.AreEqual("101", pmi.FunctionCode);

            //Transaction recreated = Transaction.FromNetwork(
            //    response.AuthorizedAmount,
            //    response.AuthorizationCode,
            //    response.NTSData,
            //    track,
            //    response.MessageTypeIndicator,
            //    response.SystemTraceAuditNumber,
            //    response.OriginalTransactionTime,
            //    response.ProcessingCode
            //);
            //// check response
            //Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_007_swipe_sale() {
            Transaction response = track.Charge(100m)
                .WithCurrency("USD")
                .WithAmountTaxed(1m)
                .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_008_refund()
        {
            Transaction response = track.Refund(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("200030", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_009_swipe_stand_in_Capture() {
            Transaction transaction = Transaction.FromNetwork(
                10m,
                "TYPE04",
                new NtsData(FallbackCode.Received_IssuerTimeout, AuthorizerCode.Host_Authorized),
                track);

            Transaction response = transaction.Capture(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("201", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_010_swipe_voice_Capture() {
            Transaction transaction = Transaction.FromNetwork(
                    10m,
                    "TYPE04",
                    new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized),
                    track
            );

            Transaction response = transaction.Capture(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("201", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_011_swipe_void() {
            Transaction sale = track.Charge(100m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(sale);
            Assert.AreEqual("000", sale.ResponseCode);

            Transaction response = sale.Void()
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("441", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);

            // check response
            Assert.AreEqual("400", response.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_012_swipe_partial_void() {
            Transaction sale = track.Charge(100m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(sale);
            System.Diagnostics.Debug.WriteLine(sale.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(sale.SystemTraceAuditNumber);
            Assert.AreEqual("002", sale.ResponseCode);

            Transaction response = sale.Void(amount: sale.AuthorizedAmount)
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("441", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);

            // check response
            Assert.AreEqual("400", response.ResponseCode);
        }

        //[TestMethod]
        //public void Test_013_swipe_reverse_force_draft_capture() {
        //    Transaction transaction = Transaction.FromNetwork(
        //            10m,
        //            "TYPE04",
        //            new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized),
        //            track
        //    );

        //    try
        //    {
        //        Transaction response = transaction.Capture(10m)
        //                .WithCurrency("USD")
        //                //.WithForceGatewayTimeout(true)
        //                .Execute();

        //        //Assert.IsNotNull(response);

        //    }
        //    catch (GatewayTimeoutException exc)
        //    {
        //        Assert.AreEqual(1, exc.ReversalCount);
        //        Assert.AreEqual("400", exc.ReversalResponseCode);
        //    }
        //}

        [TestMethod]
        public void Test_014_swipe_reverse_sale() {
            try {
                track.Charge(10m)
                        .WithCurrency("USD")
                        .WithForceGatewayTimeout(true)
                        .Execute();
                Assert.Fail("Did not throw a timeout");
            }
            catch (GatewayTimeoutException exc) {
                Assert.AreEqual(1, exc.ReversalCount);
                Assert.AreEqual("400", exc.ReversalResponseCode);
            }
        }

        //[TestMethod]
        //public void Test_015_swipe_reverse_return() {
        //    try {
        //        track.Refund(10m)
        //            .WithCurrency("USD")
        //            .WithForceGatewayTimeout(true)
        //            .Execute();
        //        Assert.Fail("Did not throw a timeout");
        //    }
        //    catch (GatewayTimeoutException exc) {
        //        Assert.AreEqual(1, exc.ReversalCount);
        //        Assert.AreEqual("400", exc.ReversalResponseCode);
        //    }
        //}

        [TestMethod]
        public void Test_016_ICR_authorization() {
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute("ICR");
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
            
            // test_017
            Transaction CaptureResponse = response.Capture(12m)
                    .WithCurrency("USD")
                    .Execute("ICR");
            Assert.IsNotNull(CaptureResponse);

            // check message data
            pmi = CaptureResponse.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("202", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(CaptureResponse.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(CaptureResponse.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", CaptureResponse.ResponseCode);            
        }

        [TestMethod]
        public void Test_018_ICR_partial_authorization() {
            Transaction response = track.Authorize(40m, true)
                        .WithCurrency("USD")
                        .Execute("ICR");
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);

            // check response
            Assert.AreEqual("002", response.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);

            // test_019
            //Transaction CaptureResponse = response.Capture(response.AuthorizedAmount)
            //        .WithCurrency("USD")
            //        .Execute("ICR");
            //Assert.IsNotNull(CaptureResponse);

            //// check message data
            //pmi = CaptureResponse.MessageInformation;
            //Assert.IsNotNull(pmi);
            //Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            //Assert.AreEqual("003000", pmi.ProcessingCode);
            //Assert.AreEqual("202", pmi.FunctionCode);

            //// check response
            //Assert.AreEqual("000", CaptureResponse.ResponseCode);
            //System.Diagnostics.Debug.WriteLine(CaptureResponse.HostResponseDate);
            //System.Diagnostics.Debug.WriteLine(CaptureResponse.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_020_ICR_auth_reversal() {
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute("ICR");
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction reversal = response.Reverse(10m)
                    .WithCurrency("USD")
                    .Execute("ICR");
            Assert.IsNotNull(reversal);

            // check message data
            pmi = reversal.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("400", pmi.FunctionCode);
            Assert.AreEqual("4021", pmi.MessageReasonCode);
            System.Diagnostics.Debug.WriteLine(reversal.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(reversal.SystemTraceAuditNumber);
            // check response
            //Assert.AreEqual("400", reversal.ResponseCode);
        }

        [TestMethod]
        public void Test_021_EMV_sale() {
            CreditCardData card = TestCards.MasterCardFleetManual(true, true);

            CreditTrackData track = TestCards.AmexSwipe();
            track.Value = ";374245002741006=241222117101234500000?";

            //track.Value = ";5567630000088409=49126010793608000024?"; //MTip40 Test 01 Scenario 01
            track.PinBlock = "62968D2481D231E1A504010024A00014";

            FleetData fleetData = new FleetData {
                //ServicePrompt = "0",
                //DriverId = "11411",
                VehicleNumber = "987654",
                OdometerReading = "123456"
            };
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Heartland);
            productData.Add("01", Api.Network.Entities.UnitOfMeasure.Gallons, 4m, 5m, 20m);

            Transaction response = track.Charge(31m)
                    .WithCurrency("USD")
                    //.WithFleetData(fleetData)
                    //.WithProductData(productData)
                    .WithTagData("4F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void WFRC_Response_Tests_Auth() {
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.ResponseMessage);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void WFRC_Response_Tests_Sale()
        {
            Transaction response = track.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.ResponseMessage);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void WFRC_Response_Tests_Void()
        {
            Transaction sale = track.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(sale);

            // Run up to this point and break - Wait for analyst to set response before continuing
            Transaction response = sale.Void().Execute();
            Assert.IsNotNull(response);

            System.Diagnostics.Debug.WriteLine(response.ResponseMessage);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        //[TestMethod]
        //public void Test_021_EMV_Proximity_sale() {
        //    CreditTrackData track = new CreditTrackData {
        //        Value = "6550006599174230=25121001001210012",
        //        EntryMethod = EntryMethod.Proximity
        //    };

        //    Transaction response = track.Charge(6m)
        //            .WithCurrency("USD")
        //            .WithTagData("4F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
        //            .Execute();
        //    Assert.IsNotNull(response);
        //    Assert.AreEqual("000", response.ResponseCode);
        //}

        //[TestMethod]
        //public void Test_022_EMV_authorization() {
        //    CreditTrackData track = new CreditTrackData {
        //        Value = "4012002000060016=25121011803939600000"
        //    };

        //    Transaction response = track.Authorize(6m)
        //            .WithCurrency("USD")
        //            .WithTagData("4F07A0000001523010500A4D617374657243617264820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10120110A0800F22000065C800000000000000FF9F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
        //            .Execute();
        //    Assert.IsNotNull(response);
        //    Assert.AreEqual("000", response.ResponseCode);
        //}

        //[TestMethod]
        //public void Test_023_EMV_02() {
        //    CreditTrackData track = new CreditTrackData {
        //        Value = "4012002000060016=25121011803939600000"
        //    };

        //    Transaction response = track.Charge(6m)
        //            .WithCurrency("USD")
        //            .WithTagData("4F07A0000001523010500A4D617374657243617264820258008407A00000015230108E0A00000000000000000100950542400080009A031901199B02E8009C01005F24031812315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000003009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05FC50A000009F0E0500000000009F0F05F870A498009F10120210A5000F040000000000000000000000FF9F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030622599F26080AB5BD8D4719AEEA9F2701809F330360F0C89F34030100029F3501219F360200059F3704DADCC7CB9F3901059F4005F000A0B0019F4104000000989F4E0D54657374204D65726368616E74")
        //            .Execute();
        //    Assert.IsNotNull(response);
        //    Assert.AreEqual("000", response.ResponseCode);

        //    Transaction reversal = response.Reverse().Execute();
        //    Assert.IsNotNull(reversal);
        //    Assert.AreEqual("400", reversal.ResponseCode);
        //}

        //[TestMethod]
        //public void Test_024_EMV_03() {
        //    CreditTrackData track = new CreditTrackData {
        //        Value = "4012002000060016=25121011803939600000"
        //    };

        //    Transaction response = track.Charge(6m)
        //            .WithCurrency("USD")
        //            .WithTagData("4F07A000000152301050104D415354455243415244204445424954820218008407A00000015230108E120000000000000000420102055E0342031F00950580000080009A031901099B0268009C01405F24032212315F25031711015F2A0208405F300202015F3401119F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FFC09F090200029F0D05B0509C88009F0E0500000000009F0F05B0709C98009F10120110A00003220000000000000000000000FF9F12104D6173746572636172642044656269749F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030647199F26084233C50A9D5D7FA29F2701809F330360F0C89F34035E03009F3501219F360201259F3704FF4CA1CD9F3901059F4005F000A0B0019F4104000000809F4E0D54657374204D65726368616E74")
        //            .Execute();
        //    Assert.IsNotNull(response);
        //    Assert.AreEqual("000", response.ResponseCode);

        //    Transaction voidResponse = response.Void().Execute();
        //    Assert.IsNotNull(voidResponse);
        //    Assert.AreEqual("400", voidResponse.ResponseCode);
        //}

        //[TestMethod]
        //public void Test_025_EMV_04() {
        //    CreditTrackData track = new CreditTrackData {
        //        Value = "4427802641004797=4427802641004797"
        //    };

        //    Transaction response = track.Authorize(100m, true)
        //            .WithCurrency("USD")
        //            .WithTagData("4F07A0000001523010500A4D61737465724361726457135413330089010434D22122019882803290000F5A085413330089010434820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F201A546573742F4361726420313020202020202020202020202020205F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10120110A0800F22000065C800000000000000FF9F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
        //            .Execute("ICR");
        //    Assert.IsNotNull(response);
        //    Assert.AreEqual("002", response.ResponseCode);

        //    Transaction Capture = response.Capture(response.AuthorizedAmount)
        //            .WithCurrency("USD")
        //            .Execute("ICR");
        //    Assert.IsNotNull(Capture);
        //    Assert.AreEqual("000", Capture.ResponseCode);
        //}

        //[TestMethod]
        //public void Test_025_EMV_credit_online_pin() {
        //    CreditTrackData track = new CreditTrackData();
        //    track.Value = "4355567063338=2012101HJNw/ewskBgnZqkL";
        //    track.PinBlock = "62968D2481D231E1A504010024A00014";

        //    Transaction response = track.Authorize(40m, true)
        //            .WithCurrency("USD")
        //            .WithTagData("4F07A0000001523010500A4D61737465724361726457135413330089010434D22122019882803290000F5A085413330089010434820238008407A00000015230108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F201A546573742F4361726420313020202020202020202020202020205F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10120110A0800F22000065C800000000000000FF9F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
        //            .Execute("ICR");
        //    Assert.IsNotNull(response);
        //    Assert.AreEqual("002", response.ResponseCode);

        //    Transaction capture = response.Capture(response.AuthorizedAmount)
        //            .WithCurrency("USD")
        //            .Execute("ICR");
        //    Assert.IsNotNull(capture);
        //    Assert.AreEqual("000", capture.ResponseCode);
        //}

        [TestMethod]
        public void Test_026_balance_inquiry() {
            Transaction response = track.BalanceInquiry()
                .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("313000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void SwipeCashAdvance() {
            Transaction response = track.CashAdvance(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("013000", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void SwipePayment() {
            Transaction response = track.Payment(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("500039", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        //[TestMethod]
        //public void Test_tor_on_stand_in_capture() {
        //    Transaction transaction = Transaction.FromNetwork(
        //        1m,
        //        "TYPE04",
        //        new NtsData(FallbackCode.Received_IssuerTimeout, AuthorizerCode.Host_Authorized),
        //        track
        //    );

        //    try {
        //        transaction.Capture(10m)
        //                .WithCurrency("USD")
        //                .WithForceGatewayTimeout(true)
        //                .Execute("ICR");
        //        Assert.Fail("No timeout exception thrown");
        //    }
        //    catch(GatewayTimeoutException exc) {
        //        Assert.AreEqual(1, exc.ReversalCount);
        //    }
        //}

        [TestMethod]
        public void Test_ReadyLink_Load() {
            Transaction response = readylinkTrack.AddValue(750m)
                .WithFee(FeeType.TransactionFee, 2m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("600008", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_WFRC_001_bank_card_swipe_inside_standin_capture_1220()
        {
            track = TestCards.VisaSwipe(EntryMethod.Swipe);

            Transaction transaction = Transaction.FromNetwork(
                10m,
                "TYPE04",
                new NtsData(FallbackCode.Received_IssuerTimeout, AuthorizerCode.Host_Authorized),
                track);

            Transaction response = transaction.Capture(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("201", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_WFRC_002_fleet_card_swipe_inside_voice_capture_1220()
        {
            CreditTrackData fleetTrack = TestCards.VoyagerFleetSwipe(EntryMethod.Swipe);

            Transaction transaction = Transaction.FromNetwork(
                        10m,
                        "TYPE04",
                        new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized),
                        fleetTrack
                );

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Heartland);
            productData.Add("01", Api.Network.Entities.UnitOfMeasure.Gallons, 1m, 10m, 10m);

            FleetData fleetData = new FleetData
            {
                //ServicePrompt = "0",
                DriverId = "11411",
                //VehicleNumber = "22031",
                OdometerReading = "1256"
            };

            Transaction response = transaction.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithAuthenticatioNMethod(CardHolderAuthenticationMethod.ManualSignatureVerification)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("201", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_WFRC_003_discover_swipe_inside_sale_1220_cash_over()
        {
     
        }

        [TestMethod]
        public void Test_WFRC_004_discover_swipe_inside_sale_1220_partial_approval_cash_over()
        {

        }

        [TestMethod]
        public void Test_WFRC_005_bank_card_swipe_inside_void_1420()
        {
            track = TestCards.VisaSwipe(EntryMethod.Swipe);

            Transaction sale = track.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(sale);
            Assert.AreEqual("000", sale.ResponseCode);

            Transaction response = sale.Void()
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("441", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);

            // check response
            Assert.AreEqual("400", response.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_WFRC_006_visa_chargeback_considerations()
        {
            track = new CreditTrackData
            {
                Value = ";4761739001010143=241222111478183?",
                EntryMethod = EntryMethod.Swipe
            };

            Transaction response = track.Charge(1m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_WFRC_007_visa_balance_return ()
        {
            track = TestCards.VisaSwipe(EntryMethod.Swipe);

            Transaction response = track.Charge(100m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            //Assert.AreEqual("000", response.ResponseCode);

            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_WFRC_008_MC_balance_return()
        {
            track = TestCards.MasterCardSwipe(EntryMethod.Swipe);

            Transaction response = track.Charge(100m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            //Assert.AreEqual("000", response.ResponseCode);

            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_WFRC_009_amex_balance_return()
        {
            track = TestCards.AmexSwipe(EntryMethod.Swipe);

            Transaction response = track.Charge(100m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            //Assert.AreEqual("000", response.ResponseCode);

            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_WFRC_010_discover_balance_return()
        {
            track = TestCards.DiscoverSwipe(EntryMethod.Swipe);

            Transaction response = track.Charge(100m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            //Assert.AreEqual("000", response.ResponseCode);

            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }
    }
}
