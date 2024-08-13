using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Tests.TestData;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Network {
    [TestClass]
    public class NWSEncryption3DesTests {
        private CreditCardData card;
        private CreditCardData cardWithCvn;
        private CreditTrackData track;
        private DebitTrackData debit;
        private GiftCard giftCard;

        private NetworkGatewayConfig config;

        public NWSEncryption3DesTests() {
            Address address = new Address();
            address.Name = "My STORE";
            address.StreetAddress1 = "1 MY STREET";
            address.City = "MYTOWN";
            address.PostalCode = "90210";
            address.State = "KY";
            address.Country = "USA";

            AcceptorConfig acceptorConfig = new AcceptorConfig();
            acceptorConfig.Address = address;

            // data code values
            acceptorConfig.CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe_KeyEntry;
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.PIN;
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
            //DE 127
            acceptorConfig.SupportedEncryptionType = EncryptionType.TDES;
            acceptorConfig.ServiceType = ServiceType.GPN_API;
            acceptorConfig.OperationType = OperationType.Decrypt;

            // gateway config
            config = new NetworkGatewayConfig(NetworkGatewayType.NWS) {
                ServiceUrl = "test.txns-c.secureexchange.net",
                PrimaryPort = 15031,
                SecondaryEndpoint = "test.txns-e.secureexchange.net",
                SecondaryPort = 15031,
                CompanyId = "SPSA",
                TerminalId = "NWSDOTNET01",//"NWSBATCH2002",//"NWSDOTNET01",
                UniqueDeviceId = "0001",
                AcceptorConfig = acceptorConfig,
                EnableLogging = true,
                StanProvider = StanGenerator.GetInstance(),
                BatchProvider = BatchProvider.GetInstance()
            };


            ServicesContainer.ConfigureService(config);

            track = new CreditTrackData();
            track.EncryptionData = new EncryptionData();
            track.EntryMethod = EntryMethod.Swipe;
            track.EncryptionData.KTB = "3A2067D00508DBE43E3342CC77B0575E04D9191B380C88036DD82D54C834DCB4130F52560AA9551B";
            track.EncryptionData.KSN = "F000019990E00003";
            track.EncryptionData.TrackNumber = TrackNumber.TrackTwo.ToString();
            track.CardType = "MC";
            track.EntryMethod = EntryMethod.Swipe;
            track.TrackNumber = TrackNumber.TrackTwo;

            // VISA
            card = new CreditCardData();
            card.EncryptionData = new EncryptionData();
            card.CardPresent = true;
            card.ReaderPresent = true;
            card.EncryptionData.KTB = "3A2067D00508DBE43E3342CC77B0575E04D9191B380C88036DD82D54C834DCB4130F52560AA9551B";
            card.EncryptionData.KSN = "F000019990E00003";
            card.CardType = "MC";
            card.EncryptionData.TrackNumber = TrackNumber.TrackTwo.ToString();

            cardWithCvn = new CreditCardData();
            cardWithCvn.EncryptionData = new EncryptionData();
            cardWithCvn.Cvn = "103";
            cardWithCvn.EncryptionData.KTB = "3A2067D00508DBE43E3342CC77B0575E04D9191B380C88036DD82D54C834DCB4130F52560AA9551B";
            cardWithCvn.EncryptionData.KSN = "F000019990E00003";
            cardWithCvn.CardType = "MC";
            cardWithCvn.CardPresent = false;
            cardWithCvn.ReaderPresent = false;
            cardWithCvn.EncryptionData.TrackNumber = TrackNumber.TrackOne.ToString();

            // DEBIT
            debit = new DebitTrackData();
            debit.EncryptionData = new EncryptionData();
            debit.PinBlock = "62968D2481D231E1A504010024A00014";
            debit.EncryptionData.KTB = "3A2067D00508DBE43E3342CC77B0575E04D9191B380C88036DD82D54C834DCB4130F52560AA9551B";
            debit.EncryptionData.KSN = "F000019990E00003"; //"3D3F820E00003";
            debit.CardType = "PINDebitCard";
            debit.PinBlock = "62968D2481D231E1A504010024A00014";
            debit.TrackNumber = TrackNumber.TrackTwo;

            giftCard = TestCards.SvsSwipe();
            giftCard.EncryptionData = new EncryptionData();
            giftCard.EncryptionData.KTB = "3A2067D00508DBE43E3342CC77B0575E04D9191B380C88036DD82D54C834DCB4130F52560AA9551B";
            giftCard.EncryptionData.KSN = "F000019990E00003";
        }

        [TestMethod]
        public void Test_001_Credit_Manual_Auth_Cvn() {
            Transaction response = cardWithCvn.Authorize(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Credit_Manual_Auth() {
            Transaction response = card.Authorize(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Credit_Swipe_Auth() {
            Transaction response = track.Authorize(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_003_Credit_Manual_Sale() {
            Transaction response = track.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            // void the transaction test case #7
            Transaction voidResponse = response.Void()
                .Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_003_Credit_Sale_Void() {
            Transaction response = track.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            // void the transaction test case #7
            Transaction voidResponse = response.Void()
                .Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_004_Credit_Manual_Sale() {
            Transaction response = track.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            // void the transaction test case #8
            Transaction reverseResponse = response.Reverse()
                .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_004_Credit_Sale_Reversal() {
            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);

            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            response.NTSData = ntsData;

            // void the transaction test case #8
            Transaction reverseResponse = response.Reverse()
                    .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Credit_Auth_Capture() {
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
        public void Test_Credit_Swipe_Sale() {
            Transaction response = track.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Credit_Swipe_Voice_Capture() {
            Transaction transaction = Transaction.FromNetwork(10m, "123456",
                new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized),
                track
            );

            Transaction response = transaction.Capture(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

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
        public void Test_005_Credit_Manual_Refund_Cvn() {
            Transaction response = cardWithCvn.Refund(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_006_Credit_Manual_Refund() {
            Transaction response = card.Refund(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Credit_Balance_Inquiry() {
            Transaction response = track.BalanceInquiry()
                .Execute();

            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;

            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("313000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_007_Credit_Swipe_Auth() {
            CreditTrackData track = new CreditTrackData();
            track.EncryptionData = new EncryptionData();
            track.CardType = "MC";
            track.EncryptionData.KTB = "3A2067D00508DBE43E3342CC77B0575E04D9191B380C88036DD82D54C834DCB4130F52560AA9551B";
            track.EncryptionData.KSN = "F000019990E00003";
            track.EncryptionData.TrackNumber = "2";
            track.TrackNumber = TrackNumber.TrackTwo;
            track.EntryMethod = EntryMethod.Swipe;

            Transaction response = track.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction capture = response.Capture(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
        }

        //        [TestMethod]
        //        public void Test_009_Credit_Swipe_Sale() {
        //            Transaction response = track.Charge(10m)
        //                .WithCurrency("USD")
        //                .Execute();
        //            Assert.IsNotNull(response);
        //            Assert.AreEqual("000", response.ResponseCode);
        //            //Assert.AreEqual(response.ResponseMessage, "000", response.ResponseCode);

        //            string IRR_data = response.CardIssuerResponse.c().get(CardIssuerEntryTag.RetrievalReferenceNumber);
        //// reverse the transaction test case #40
        //Transaction voidResponse = response.voidTransaction()
        //        .withIssuerData(CardIssuerEntryTag.RetrievalReferenceNumber, IRR_data)
        //        .withCustomerInitiated(true)
        //        .execute();
        //assertNotNull(voidResponse);
        //assertAreEqual(response.getResponseMessage(), "400", voidResponse.getResponseCode());

        //// reverse the transaction test case #39
        //Transaction reverseResponse = response.reverse().execute();
        //assertNotNull(reverseResponse);
        //assertAreEqual(response.getResponseMessage(), "400", reverseResponse.getResponseCode());
        //    }

        [TestMethod]
        public void Test_010_Credit_Swipe_Refund() {
            Transaction response = track.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_013_Visa_Encrypted_Follow_On() {
            Transaction response = track.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction recreated = Transaction.FromNetwork(
                response.AuthorizedAmount,
                response.AuthorizationCode,
                new NtsData(FallbackCode.Received_IssuerTimeout, AuthorizerCode.Terminal_Authorized),
                track,
                response.MessageTypeIndicator,
                response.SystemTraceAuditNumber,
                response.OriginalTransactionTime,
                response.ProcessingCode
            );

            Transaction reversal = recreated.Reverse(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("400", reversal.ResponseCode);
        }

        [TestMethod]
        public void Test_014_Visa_Encrypted_Refund() {
            Transaction response = track.Refund(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_015_Credit_Swipe_Void() {
            Transaction response = track.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            // reverse the transaction test case #40
            //        Transaction voidResponse = response.voidTransaction()
            //                .withCustomerInitiated(true)
            //                .execute();
            //        assertNotNull(voidResponse);
            //        assertAreEqual(response.getResponseMessage(), "400", voidResponse.getResponseCode());

            // reverse the transaction test case #39
            Transaction reverseResponse = response.Reverse()
                    .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }

        //-----------------------------------Visa Credit-----------------------------------------------
        [TestMethod]
        public void Test_001_Visa_Credit_Manual_Auth_Cvn() {
            cardWithCvn.CardType = "Visa";
            Transaction response = cardWithCvn.Authorize(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Visa_Credit_Manual_Auth() {
            card.CardType = "Visa";
            Transaction response = card.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Visa_Credit_Swipe_Auth() {
            CreditTrackData track = new CreditTrackData();
            track.EncryptionData = new EncryptionData();
            track.CardType = "Visa";
            track.EncryptionData.KTB = "49AB0D7DF39F4EAA3ADEB107CCCC03D0";
            track.EncryptionData.KSN = "F000019990E00003";
            track.EncryptionData.TrackNumber = "3";
            track.TrackNumber = TrackNumber.PAN;
            track.EntryMethod = EntryMethod.Swipe;

            Transaction response = track.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Visa_Credit_Auth_Capture() {
            //track.CardType = "Visa";
            CreditTrackData track = new CreditTrackData();
            track.EncryptionData = new EncryptionData();
            track.CardType = "Visa";
            track.EncryptionData.KTB = "49AB0D7DF39F4EAA3ADEB107CCCC03D0";
            track.EncryptionData.KSN = "F000019990E00003";
            track.EncryptionData.TrackNumber = "3";
            track.TrackNumber = TrackNumber.PAN;
            track.EntryMethod = EntryMethod.Swipe;

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
        public void Test_Visa_Credit_Swipe_Sale() {
            //track.CardType = "Visa";
            CreditTrackData track = new CreditTrackData();
            track.EncryptionData = new EncryptionData();
            track.CardType = "Visa";
            track.EncryptionData.KTB = "49AB0D7DF39F4EAA3ADEB107CCCC03D0";
            track.EncryptionData.KSN = "F000019990E00003";
            track.EncryptionData.TrackNumber = "3";
            track.TrackNumber = TrackNumber.PAN;
            track.EntryMethod = EntryMethod.Swipe;

            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Visa_Credit_Swipe_Voice_Capture() {
            CreditTrackData track = new CreditTrackData();
            track.EncryptionData = new EncryptionData();
            track.CardType = "Visa";
            track.EncryptionData.KTB = "49AB0D7DF39F4EAA3ADEB107CCCC03D0";
            track.EncryptionData.KSN = "F000019990E00003";
            track.EncryptionData.TrackNumber = "3";
            track.TrackNumber = TrackNumber.PAN;
            track.EntryMethod = EntryMethod.Swipe;
            Transaction transaction = Transaction.FromNetwork(10m, "123456",
                new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized),
                track
                );

            Transaction response = transaction.Capture(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

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
        public void Test_010_Visa_Credit_Swipe_Refund() {
            //track.CardType = "Visa";
            CreditTrackData track = new CreditTrackData();
            track.EncryptionData = new EncryptionData();
            track.CardType = "Visa";
            track.EncryptionData.KTB = "49AB0D7DF39F4EAA3ADEB107CCCC03D0";
            track.EncryptionData.KSN = "F000019990E00003";
            track.EncryptionData.TrackNumber = "3";
            track.TrackNumber = TrackNumber.PAN;
            track.EntryMethod = EntryMethod.Swipe;
            Transaction response = track.Refund(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_003_Visa_Credit_Sale_Void() {
            //track.CardType = "Visa";
            CreditTrackData track = new CreditTrackData();
            track.EncryptionData = new EncryptionData();
            track.CardType = "Visa";
            track.EncryptionData.KTB = "49AB0D7DF39F4EAA3ADEB107CCCC03D0";
            track.EncryptionData.KSN = "F000019990E00003";
            track.EncryptionData.TrackNumber = "3";
            track.TrackNumber = TrackNumber.PAN;
            track.EntryMethod = EntryMethod.Swipe;
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
        public void Test_004_Visa_Credit_Sale_Reversal() {
            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);

            //track.CardType = "Visa";
            CreditTrackData track = new CreditTrackData();
            track.EncryptionData = new EncryptionData();
            track.CardType = "Visa";
            track.EncryptionData.KTB = "49AB0D7DF39F4EAA3ADEB107CCCC03D0";
            track.EncryptionData.KSN = "F000019990E00003";
            track.EncryptionData.TrackNumber = "3";
            track.TrackNumber = TrackNumber.PAN;
            track.EntryMethod = EntryMethod.Swipe;
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
        public void Test_Visa_Credit_Balance_Inquiry() {
            //track.CardType = "Visa";
            CreditTrackData track = new CreditTrackData();
            track.EncryptionData = new EncryptionData();
            track.CardType = "Visa";
            track.EncryptionData.KTB = "49AB0D7DF39F4EAA3ADEB107CCCC03D0";
            track.EncryptionData.KSN = "F000019990E00003";
            track.EncryptionData.TrackNumber = "3";
            track.TrackNumber = TrackNumber.PAN;
            track.EntryMethod = EntryMethod.Swipe;
            Transaction response = track.BalanceInquiry()
                    .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("313000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        //-----------------------------------VisaCorporate Credit-----------------------------------------------
        [TestMethod]
        public void Test_001_VisaCorporate_Credit_Manual_Auth_Cvn() {
            cardWithCvn.CardType = "VisaCorporate";
            Transaction response = cardWithCvn.Authorize(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_VisaCorporate_Credit_Manual_Auth() {
            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config);

            card.CardType = "VisaCorporate";
            Transaction response = card.Authorize(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_VisaCorporate_Credit_Swipe_Auth() {
            track.CardType = "VisaCorporate";
            Transaction response = track.Authorize(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_VisaCorporate_Credit_Auth_Capture() {
            track.CardType = "VisaCorporate";
            Transaction response = track.Authorize(10)
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
        public void Test_VisaCorporate_Credit_Swipe_Sale() {
            track.CardType = "VisaCorporate";
            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_VisaCorporate_Credit_Swipe_Voice_Capture() {
            track.CardType = "VisaCorporate";
            Transaction transaction = Transaction.FromNetwork(10, "TYPE04",
                new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized),
                track
            );

            Transaction response = transaction.Capture(10)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

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
        public void Test_010_VisaCorporate_Credit_Swipe_Refund() {
            track.CardType = "VisaCorporate";
            Transaction response = track.Refund(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_003_VisaCorporate_Credit_Sale_Void() {
            track.CardType = "VisaCorporate";
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
        public void Test_004_VisaCorporate_Credit_Sale_Reversal() {
            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);

            track.CardType = "VisaCorporate";
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
        public void Test_VisaCorporate_Credit_Balance_Inquiry() {
            track.CardType = "VisaCorporate";
            Transaction response = track.BalanceInquiry()
                    .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("313000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        //-----------------------------------VisaPurchasing Credit-----------------------------------------------
        [TestMethod]
        public void Test_001_VisaPurchasing_Credit_Manual_Auth_Cvn() {
            cardWithCvn.CardType = "VisaPurchasing";
            Transaction response = cardWithCvn.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_VisaPurchasing_Credit_Manual_Auth() {
            card.CardType = "VisaPurchasing";
            Transaction response = card.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_VisaPurchasing_Credit_Swipe_Auth() {
            track.CardType = "VisaPurchasing";
            Transaction response = track.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_VisaPurchasing_Credit_Auth_Capture() {
            track.CardType = "VisaPurchasing";
            Transaction response = track.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            // test_019
            Transaction captureResponse = response.Capture()
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_VisaPurchasing_Credit_Swipe_Sale() {
            track.CardType = "VisaPurchasing";
            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_VisaPurchasing_Credit_Swipe_Voice_Capture() {
            track.CardType = "VisaPurchasing";
            Transaction transaction = Transaction.FromNetwork(10m, "TYPE04",
                new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized),
                track
            );

            Transaction response = transaction.Capture(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);

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
        public void Test_010_VisaPurchasing_Credit_Swipe_Refund() {
            track.CardType = "VisaPurchasing";
            Transaction response = track.Refund(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_003_VisaPurchasing_Credit_Sale_Void() {
            track.CardType = "VisaPurchasing";
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
        public void Test_004_VisaPurchasing_Credit_Sale_Reversal() {
            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);

            track.CardType = "VisaPurchasing";
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
        public void Test_VisaPurchasing_Credit_Balance_Inquiry() {
            track.CardType = "VisaPurchasing";
            Transaction response = track.BalanceInquiry()
                        .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("313000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        //-----------------------------------MCPurchasing Credit-----------------------------------------------
        [TestMethod]
        public void Test_001_McPurchasing_Credit_Manual_Auth_Cvn() {
            cardWithCvn.CardType = "MCPurchasing";
            Transaction response = cardWithCvn.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_McPurchasing_Credit_Manual_Auth() {
            card.CardType = "MCPurchasing";
            Transaction response = card.Authorize(10)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_McPurchasing_Credit_Swipe_Auth() {
            track.CardType = "MCPurchasing";
            Transaction response = track.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_McPurchasing_Credit_Auth_Capture() {
            track.CardType = "MCPurchasing";
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
            // check response
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_McPurchasing_Credit_Swipe_Sale() {
            track.CardType = "MCPurchasing";
            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_McPurchasing_Credit_Swipe_Voice_Capture() {
            track.CardType = "MCPurchasing";
            Transaction transaction = Transaction.FromNetwork(10m, "TYPE04",
                new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized),
                track
            );

            Transaction response = transaction.Capture(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);

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
        public void Test_010_McPurchasing_Credit_Swipe_Refund() {
            track.CardType = "MCPurchasing";
            Transaction response = track.Refund(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_003_McPurchasing_Credit_Sale_Void() {
            track.CardType = "MCPurchasing";
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
        public void Test_004_McPurchasing_Credit_Sale_Reversal() {
            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);

            track.CardType = "MCPurchasing";
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
        public void Test_McPurchasing_Credit_Balance_Inquiry() {
            track.CardType = "MCPurchasing";
            Transaction response = track.BalanceInquiry()
                    .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("313000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        //-----------------------------------Amex Credit-----------------------------------------------
        [TestMethod]
        public void Test_001_Amex_Credit_Manual_Auth_Cvn() {
            cardWithCvn.CardType = "Amex";
            Transaction response = cardWithCvn.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Amex_Credit_Manual_Auth() {
            card.CardType = "Amex";
            Transaction response = card.Authorize(10)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Amex_Credit_Swipe_Auth() {
            track.CardType = "Amex";
            Transaction response = track.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Amex_Credit_Auth_Capture() {
            track.CardType = "Amex";
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
            // check response
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Amex_Credit_Swipe_Sale() {
            track.CardType = "Amex";
            Transaction response = track.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Amex_Credit_Swipe_Voice_Capture() {
            track.CardType = "Amex";
            Transaction transaction = Transaction.FromNetwork(10m, "TYPE04",
                new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized),
                track
            );

            Transaction response = transaction.Capture(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);

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
        public void Test_010_Amex_Credit_Swipe_Refund() {
            track.CardType = "Amex";
            Transaction response = track.Refund(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_003_Amex_Credit_Sale_Void() {
            track.CardType = "Amex";
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
        public void Test_004_Amex_Credit_Sale_Reversal() {
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
        public void Test_Amex_Credit_Balance_Inquiry() {
            track.CardType = "Amex";
            Transaction response = track.BalanceInquiry()
                    .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("313000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        //-----------------------------------Discover Credit-----------------------------------------------
        [TestMethod]
        public void Test_001_Discover_Credit_Manual_Auth_Cvn() {
            cardWithCvn.CardType = "Discover";
            Transaction response = cardWithCvn.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Discover_Credit_Manual_Auth() {
            card.CardType = "Discover";
            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Discover_Credit_Swipe_Auth() {
            track.CardType = "Discover";
            Transaction response = track.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Discover_Credit_Auth_Capture() {
            track.CardType = "Discover";
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
            // check response
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Discover_Credit_Swipe_Sale() {
            track.CardType = "Discover";
            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Discover_Credit_Swipe_Voice_Capture() {
            track.CardType = "Discover";
            Transaction transaction = Transaction.FromNetwork(10m, "TYPE04",
                new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized),
                track
            );

            Transaction response = transaction.Capture(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

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
        public void Test_010_Discover_Credit_Swipe_Refund() {
                track.CardType = "Discover";
                Transaction response = track.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_003_Discover_Credit_Sale_Void() {
            track.CardType = "Discover";
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
        public void Test_004_Discover_Credit_Sale_Reversal() {
            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);

            track.CardType = "Discover";
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
        public void Test_Discover_Credit_Balance_Inquiry() {
            track.CardType = "Discover";
            Transaction response = track.BalanceInquiry()
                .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("313000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);
                  
            Assert.AreEqual("000", response.ResponseCode);
        }                 

        //-----------------------------------PayPal Credit-----------------------------------------------
        [TestMethod]
        public void Test_001_PayPal_Credit_Manual_Auth_Cvn() {
            cardWithCvn.CardType = "PayPal";
            Transaction response = cardWithCvn.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_PayPal_Credit_Manual_Auth() {
            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config);
    
            card.CardType = "PayPal";
            Transaction response = card.Authorize(10)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_PayPal_Credit_Swipe_Auth() {
            track.CardType = "PayPal";
            Transaction response = track.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_PayPal_Credit_Auth_Capture() {
            track.CardType = "PayPal";
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
            // check response
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_PayPal_Credit_Swipe_Sale() {
            track.CardType = "PayPal";
            Transaction response = track.Charge(10m)
                     .WithCurrency("USD")
                     .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_PayPal_Credit_Swipe_Voice_Capture() {
            track.CardType = "PayPal";
            Transaction transaction = Transaction.FromNetwork(10m, "TYPE04",
                new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized),
                track
            );

            Transaction response = transaction.Capture(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

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
        public void Test_010_PayPal_Credit_Swipe_Refund() {
            track.CardType = "PayPal";
            Transaction response = track.Refund(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_003_PayPal_Credit_Sale_Void() {
            track.CardType = "PayPal";
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
        public void Test_004_PayPal_Credit_Sale_Reversal() {
            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);

            track.CardType = "PayPal";
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
        public void Test_PayPal_Credit_Balance_Inquiry() {
            track.CardType = "PayPal";
            Transaction response = track.BalanceInquiry()
                    .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("313000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //------------------------------------debit-----------------------------------------------------
        [TestMethod]
        public void Test_001_Debit_Auth() {
            Transaction response = debit.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }


        [TestMethod]
        public void Test_Debit_Auth_Capture() {
            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config);

            Transaction response = debit.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            Transaction captureResponse = response.Capture()
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(captureResponse);

            // check response
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Swipe_Voice_Capture() {
            Transaction transaction = Transaction.FromNetwork(10m, "TYPE04",
                new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized),
                debit
            );

            Transaction response = transaction.Capture(10)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("008000", pmi.ProcessingCode);
            Assert.AreEqual("201", pmi.FunctionCode);
            
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Sale_With_CashBack() {
            Transaction response = debit.Charge(10)
                    .WithCurrency("USD")
                    .WithCashBack(3m)
                    .Execute();
            Assert.IsNotNull(response);
            
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("090800", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_008_Debit_Sale() {
            Transaction response = debit.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_014_Debit_Encrypted_Refund() {
            Transaction response = debit.Refund(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Balance_Inquiry() {
            Transaction response = track.BalanceInquiry()
                    .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("303000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_015_Debit_Swipe_Void() {
            Transaction response = debit.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            // reverse the transaction test case #40
            Transaction voidResponse = response.Void()
                    .WithCustomerInitiated(true)
                    .Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("400", voidResponse.ResponseCode);
            
            // reverse the transaction test case #39
            Transaction reverseResponse = response.Reverse().Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_013_Debit_Encrypted_Follow_On() {
            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction recreated = Transaction.FromNetwork(
                    response.AuthorizedAmount,
                    response.AuthorizationCode,
                    new NtsData(FallbackCode.Received_IssuerTimeout, AuthorizerCode.Terminal_Authorized),
                    track,
                    response.MessageTypeIndicator,
                    response.SystemTraceAuditNumber,
                    response.OriginalTransactionTime,
                    response.ProcessingCode
            );
        
            Transaction reversal = recreated.Reverse(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("400", reversal.ResponseCode);
        }

        [TestMethod]
        public void Test_003_Debit_Sale_Void() {
            Transaction response = debit.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            // void the transaction test case #7
            Transaction voidResponse = response.Void().Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_004_Debit_Sale_Reversal() {
            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);

            Transaction response = debit.Charge(10)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            
            response.NTSData = ntsData;
            
            // void the transaction test case #8
            Transaction reverseResponse = response.Reverse().Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }

        //------------------------------------------------Gift Card--------------------------------------------------------
        [TestMethod]
        public void Test_001_Gift_Auth() {
            Transaction response = giftCard.Authorize(50m, true)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_GiftCard_Sale() {
            Transaction response = giftCard.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_GiftCard_Activate() {
            Transaction response = giftCard.Activate(25m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_GiftCard_BalanceInquiry() {
            Transaction response = giftCard.BalanceInquiry().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_GiftCard_Return() {
            Transaction response = giftCard.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_GiftCard_Void() {
            Transaction response = giftCard.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction voidResponse = response.Void().Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_GiftCard_Voice_Capture() {
            Transaction trans = Transaction.FromNetwork(1m, "TYPE04", NtsData.VoiceAuthorized(), giftCard);

            Transaction response = trans.Capture()
                    .WithReferenceNumber("12345")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_GiftCard_SaleCashBack() {
            Transaction response = giftCard.Charge(41m)
                        .WithCurrency("USD")
                        .WithCashBack(40m)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_GiftCard_CashOut() {
            Transaction response = giftCard.CashOut()
                        .WithClerkId("41256")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        [TestMethod]
        public void Test_GiftCard_Reversal() {
            try {
                var re = giftCard.Charge(11m)
                        .WithCurrency("USD")
                        .WithForceGatewayTimeout(true)
                        .Execute();
                Assert.Fail("No exception thrown");
            }
            catch (GatewayTimeoutException) {

            }
        }
    }
}
