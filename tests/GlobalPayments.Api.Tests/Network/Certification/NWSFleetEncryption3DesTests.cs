using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitOfMeasure = GlobalPayments.Api.Network.Entities.UnitOfMeasure;

namespace GlobalPayments.Api.Tests.Network.Certification {
    [TestClass]
    public class NWSFleetEncryption3DesTests {
        private CreditTrackData trackMC;
        private CreditTrackData trackVisa;
        private CreditTrackData trackVoyager;
        private CreditTrackData trackWexFleet;
        private CreditTrackData trackFleetcor;

        private NetworkGatewayConfig config;
        private FleetData fleetData;

        public NWSFleetEncryption3DesTests() {
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
                TerminalId = "NWSDOTNET01",
                UniqueDeviceId = "0001",
                AcceptorConfig = acceptorConfig,
                EnableLogging = true,
                StanProvider = StanGenerator.GetInstance(),
                BatchProvider = BatchProvider.GetInstance()
            };
            ServicesContainer.ConfigureService(config);
            
            #region Test Cards
            trackMC = GetMastercardFleetTrack1Detail();
            trackVisa = GetVisaFleetDetails();
            trackVoyager = GetVoyagerFleetDetails();
            trackWexFleet = GetWexFleetDetails();
            trackFleetcor = GetFleetcorTrack2Details();
            #endregion

            fleetData = new FleetData {
                DriverId = "11411",
                OdometerReading = "1256"
            };
        }

        #region Test Cards Details
        public CreditTrackData GetMastercardFleetTrack1Detail() {
            trackMC = new CreditTrackData();
            trackMC.EncryptionData = new EncryptionData();
            trackMC.CardType = "MC";
            trackMC.FleetCard = true;
            trackMC.EntryMethod = EntryMethod.Swipe;
            trackMC.EncryptionData.KTB = "79FF82DA5C4FF957FC72F615F67239A5A261DA9FD86A8354684F456875C090B1D9ACDA3ACEB02F1E632BB61BF3C8D622973893A1E6A969B8E8BA0761E61E955A6B306FE4681306AC";
            trackMC.EncryptionData.KSN = "F000019990E00003";
            trackMC.EncryptionData.TrackNumber = TrackNumber.TrackOne.ToString();
            trackMC.TrackNumber = TrackNumber.TrackOne;
            return trackMC;
        }
        public CreditTrackData GetMastercardFleetTrack2Detail() {
            trackMC = new CreditTrackData();
            trackMC.CardType = "MC";
            trackMC.FleetCard = true;
            trackMC.EncryptionData = new EncryptionData();
            trackMC.EncryptionData.KTB = "0E37C579CDDB7838B7894782EFA229072FE396A09C59F917249B63E56DA51AA5C6F87862E63398A0";
            trackMC.EncryptionData.KSN = "F000019990E00003";
            trackMC.EncryptionData.TrackNumber = TrackNumber.TrackTwo.ToString();
            trackMC.TrackNumber = TrackNumber.TrackTwo;
            return trackMC;
        }
        public CreditTrackData GetMasterCardFleetPanDetails() {
            trackMC = new CreditTrackData();
            trackMC.CardType = "MC";
            trackMC.FleetCard = true;
            trackMC.EncryptionData = new EncryptionData();
            trackMC.EncryptionData.KTB = "6F4C54230C45E0DDD7DF6AD380881A1C";
            trackMC.EncryptionData.KSN = "F000019990E00003";
            trackMC.EncryptionData.TrackNumber = TrackNumber.PAN.ToString();
            trackMC.TrackNumber = TrackNumber.PAN;
            return trackMC;
        }
        public CreditTrackData GetVisaFleetDetails() {
            trackVisa = new CreditTrackData();
            trackVisa.EncryptionData = new EncryptionData();
            trackVisa.EncryptionData.KTB = "EDCAC8D0A62C80A7D85419AEA8A246F0C4980437475DCC3EEF25B0BC972CBD5161FCD9297AA4C6CB";
            trackVisa.EncryptionData.KSN = "F000019990E00003";
            trackVisa.EncryptionData.TrackNumber = TrackNumber.TrackTwo.ToString();
            trackVisa.CardType = "VISA";
            trackVisa.FleetCard = true;
            trackVisa.EntryMethod = EntryMethod.Swipe;
            trackVisa.TrackNumber = TrackNumber.TrackTwo;
            return trackVisa;
        }
        public CreditTrackData GetVoyagerFleetDetails() {
            trackVoyager = new CreditTrackData();
            trackVoyager.EncryptionData = new EncryptionData();
            trackVoyager.EncryptionData.KTB = "38AE7BBA2740AE21F0EBAF4DBF1A28260D3C9416F25FC3CC1125F8D8EE80D8D40E2D8A7CFCF43D5C";
            trackVoyager.EncryptionData.KSN = "F000019990E00003";
            trackVoyager.EncryptionData.TrackNumber = TrackNumber.TrackTwo.ToString();
            trackVoyager.CardType = "Voyager";
            trackVoyager.FleetCard = true;
            trackVoyager.EntryMethod = EntryMethod.Swipe;
            trackVoyager.TrackNumber = TrackNumber.TrackTwo;
            return trackVoyager;
        }
        public CreditTrackData GetFleetcorTrack2Details() {
            trackFleetcor = new CreditTrackData();
            trackFleetcor.EncryptionData = new EncryptionData();
            trackFleetcor.EncryptionData.KTB = "39BB9BB27C338BAE02814A668CD730176730A137B3F7CF4A";
            trackFleetcor.EncryptionData.KSN = "F000019990E00003";
            trackFleetcor.EncryptionData.TrackNumber = TrackNumber.TrackTwo.ToString();
            trackFleetcor.CardType = "Fleetcor";
            trackFleetcor.FleetCard = true;
            trackFleetcor.EntryMethod = EntryMethod.Swipe;
            trackFleetcor.TrackNumber = TrackNumber.TrackTwo;
            return trackFleetcor;
        }
        public CreditTrackData GetWexFleetDetails() {
            trackWexFleet = new CreditTrackData();
            trackWexFleet.EncryptionData = new EncryptionData();
            trackWexFleet.EncryptionData.KTB = "72C650103845AE125A4A75C17604950E18EC43B828EED9E4C85A45B2173F1F2394E12E729DD42565";
            trackWexFleet.EncryptionData.KSN = "F000019990E00003";
            trackWexFleet.EncryptionData.TrackNumber = TrackNumber.TrackTwo.ToString();
            trackWexFleet.CardType = "WexFleet";
            trackWexFleet.FleetCard = true;
            trackWexFleet.EntryMethod = EntryMethod.Swipe;
            trackWexFleet.TrackNumber = TrackNumber.TrackTwo;
            return trackWexFleet;
        }
        #endregion

        #region 3Des Mastercard Fleet Test cases
        [TestMethod]
        public void Test_MCFleet_Credit_Swipe_Auth_Track1() {
            trackMC = GetMastercardFleetTrack1Detail();
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackMC.Authorize(10m)
                .WithCurrency("USD")
                .WithProductData(productData)
                .WithFleetData(fleetData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_MCFleet_Credit_Swipe_Auth_Track2() {
            trackMC = GetMastercardFleetTrack2Detail();
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackMC.Authorize(10m)
                .WithCurrency("USD")
                .WithProductData(productData)
                .WithFleetData(fleetData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_MCFleet_Credit_Swipe_Auth_Pan() {
            trackMC = GetMasterCardFleetPanDetails();
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackMC.Authorize(10m)
                .WithCurrency("USD")
                .WithProductData(productData)
                .WithFleetData(fleetData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_MCFleet_Credit_Sale_Track1() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackMC.Charge(10m)
                .WithCurrency("USD")
                .WithProductData(productData)
                .WithFleetData(fleetData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_MCFleet_Credit_Auth_Capture() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);

            Transaction response = trackMC.Authorize(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture()
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }
        
        [TestMethod]
        public void Test_MCFleet_Credit_Swipe_Voice_Capture_Track1() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);

            Transaction transaction = Transaction.FromNetwork(10m, "123456",
                new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized),
                trackMC
            );

            Transaction response = transaction.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_MCFleet_Credit_Sale_Void() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackMC.Charge(10m)
                .WithCurrency("USD")
                .WithProductData(productData)
                .WithFleetData(fleetData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction voidResponse = response.Void()
                .WithProductData(productData)
                .WithReferenceNumber("123456")
                .WithFleetData(fleetData)
                .Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_MCFleet_Credit_Manual_Sale_Reverse() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackMC.Charge(10m)
                .WithCurrency("USD")
                .WithProductData(productData)
                .WithFleetData(fleetData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction reverseResponse = response.Reverse(10m)
                .WithProductData(productData)
                .WithReasonCode(ReasonCode.OTHER)
                .WithFleetData(fleetData)
                .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_MCFleet_Credit_Refund() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);

            Transaction response = trackMC.Refund(10m)
                .WithCurrency("USD")
                .WithProductData(productData)
                .WithFleetData(fleetData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_MCFleet_Credit_Balance_Inquiry() {
            Transaction response = trackMC.BalanceInquiry().Execute();

            Assert.IsNotNull(response);
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("310900", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);
            Assert.AreEqual("000", response.ResponseCode);
        }
        #endregion

        #region 3Des VisaFleet Test Cases
        [TestMethod]
        public void Test_VisaFleet_Credit_Swipe_Auth() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackVisa.Authorize(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_VisaFleet_Credit_Auth_Capture() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackVisa.Authorize(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture()
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_VisaFleet_Credit_Swipe_Voice_Capture() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction transaction = Transaction.FromNetwork(10m, "123456",
                new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized),
                trackVisa
                );

            Transaction response = transaction.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_VisaFleet_Credit_Swipe_Sale() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackVisa.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_VisaFleet_Credit_Swipe_Refund() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackVisa.Refund(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_VisaFleet_Credit_Sale_Void() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackVisa.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction voidResponse = response.Void()
                .WithProductData(productData)
                .WithFleetData(fleetData)
                .WithReferenceNumber("123456")
                .Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_VisaFleet_Credit_Sale_Reversal() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);

            Transaction response = trackVisa.Charge(10m)
                .WithCurrency("USD")
                .WithProductData(productData)
                .WithFleetData(fleetData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            response.NTSData = ntsData;
            Transaction reverseResponse = response.Reverse(10m)
                .WithProductData(productData)
                .WithFleetData(fleetData)
                .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_VisaFleet_Credit_Balance_Inquiry() {
            Transaction response = trackVisa.BalanceInquiry()
                    .Execute();
            Assert.IsNotNull(response);
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("310900", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);
            Assert.AreEqual("000", response.ResponseCode);
        }
        #endregion

        #region 3Des VoyagerFleet Test Cases
        [TestMethod]
        public void Test_VoyagerFleet_Credit_Swipe_Auth() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackVoyager.Authorize(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_VoyagerFleet_Credit_Auth_Capture() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackVoyager.Authorize(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_VoyagerFleet_Credit_Swipe_Voice_Capture() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction transaction = Transaction.FromNetwork(10m, "123456",
                new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized),
                trackVoyager
                );

            Transaction response = transaction.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_VoyagerFleet_Credit_Swipe_Sale() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackVoyager.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_VoyagerFleet_Credit_Swipe_Refund() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackVoyager.Refund(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_VoyagerFleet_Credit_Sale_Void() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackVoyager.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction voidResponse = response.Void()
                .WithProductData(productData)
                .WithFleetData(fleetData)
                .WithReferenceNumber("12345")
                .Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_VoyagerFleet_Credit_Sale_Reversal() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);

            Transaction response = trackVoyager.Charge(10m)
                .WithCurrency("USD")
                .WithProductData(productData)
                .WithFleetData(fleetData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            response.NTSData = ntsData;
            Transaction reverseResponse = response.Reverse(10m)
                .WithProductData(productData)
                .WithReasonCode(ReasonCode.OTHER)
                .WithFleetData(fleetData)
                .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_VoyagerFleet_Credit_Balance_Inquiry() {
            Transaction response = trackVoyager.BalanceInquiry()
                    .Execute();
            Assert.IsNotNull(response);
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("310900", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);
            Assert.AreEqual("000", response.ResponseCode);
        }
        #endregion

        #region 3Des WexFleet Test Cases
        [TestMethod]
        public void Test_WexFleet_Credit_Swipe_Auth() {
            fleetData.PurchaseDeviceSequenceNumber = "12345";
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackWexFleet.Authorize(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_WexFleet_Credit_Auth_Capture() {
            fleetData.PurchaseDeviceSequenceNumber = "12345";
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackWexFleet.Authorize(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture()
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_WexFleet_Credit_Swipe_Voice_Capture() {
            fleetData.PurchaseDeviceSequenceNumber = "12345";
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction transaction = Transaction.FromNetwork(10m, "123456",
                new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized),
                trackWexFleet
                );

            Transaction response = transaction.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_WexFleet_Credit_Swipe_Sale() {
            fleetData.PurchaseDeviceSequenceNumber = "12345";
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackWexFleet.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_WexFleet_Credit_Swipe_Refund() {
            fleetData.PurchaseDeviceSequenceNumber = "12345";
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackWexFleet.Refund(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithTransactionMatchingData(new TransactionMatchingData("0000040067", "0114"))
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_WexFleet_Credit_Sale_Void() {
            fleetData.PurchaseDeviceSequenceNumber = "12345";
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackWexFleet.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction voidResponse = response.Void()
                .WithProductData(productData)
                .WithReferenceNumber("67890")
                .WithFleetData(fleetData)
                .Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_WexFleet_Credit_Sale_Reversal() {
            fleetData.PurchaseDeviceSequenceNumber = "12345";
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);

            Transaction response = trackWexFleet.Charge(10m)
                .WithCurrency("USD")
                .WithProductData(productData)
                .WithFleetData(fleetData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            response.NTSData = ntsData;
            Transaction reverseResponse = response.Reverse(10m)
                .WithProductData(productData)
                .WithReferenceNumber("67890")
                .WithReasonCode(ReasonCode.OTHER)
                .WithFleetData(fleetData)
                .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_WexFleet_Credit_Balance_Inquiry() {
            fleetData.PurchaseDeviceSequenceNumber = "12345";
            Transaction response = trackWexFleet.BalanceInquiry()
                    .Execute();
            Assert.IsNotNull(response);
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("310900", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);
            Assert.AreEqual("000", response.ResponseCode);
        }
        #endregion

        #region 3Des FleetcorFleet Test Cases
        [TestMethod]
        public void Test_FleetcorFleet_Credit_Swipe_Auth() {
            fleetData.JobNumber = "22031";
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackFleetcor.Authorize(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_FleetcorFleet_Credit_Swipe_Sale() {
            fleetData.JobNumber = "22031";
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackFleetcor.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_FleetcorFleet_Credit_Auth_Capture() {
            fleetData.JobNumber = "22031";
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackFleetcor.Authorize(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_FleetcorFleet_Credit_Swipe_Voice_Capture() {
            fleetData.JobNumber = "22031";
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction transaction = Transaction.FromNetwork(10m, "123456",
                new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized),
                trackFleetcor
                );

            Transaction response = transaction.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_FleetcorFleet_Credit_Swipe_Refund() {
            fleetData.JobNumber = "22031";
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackFleetcor.Refund(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_FleetcorFleet_Credit_Sale_Void() {
            fleetData.JobNumber = "22031";
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = trackFleetcor.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction voidResponse = response.Void(null, 10m)
                           .WithProductData(productData)
                           .WithFleetData(fleetData)
                           .WithReferenceNumber("123456")
                           .Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_FleetcorFleet_Credit_Sale_Reversal() {
            fleetData.JobNumber = "22031";
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            NtsData ntsData = new NtsData(FallbackCode.CouldNotCommunicateWithHost, AuthorizerCode.Terminal_Authorized);

            Transaction response = trackFleetcor.Charge(10m)
                .WithCurrency("USD")
                .WithProductData(productData)
                .WithFleetData(fleetData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction reverseResponse = response.Reverse(10m)
                .WithProductData(productData)
                .WithFleetData(fleetData)
                .WithReasonCode(ReasonCode.OTHER)
                .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_FleetcorFleet_Credit_Balance_Inquiry() {
            Transaction response = trackFleetcor.BalanceInquiry()
                    .Execute();
            Assert.IsNotNull(response);
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("310900", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);
            Assert.AreEqual("000", response.ResponseCode);
        }
        #endregion
    }
}
