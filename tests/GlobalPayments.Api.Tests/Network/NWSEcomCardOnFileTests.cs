using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Network {
    [TestClass]
    public class NWSEcomCardOnFileTests {
        private CreditCardData card;
        private CreditTrackData track;
        public AcceptorConfig acceptorConfig;
        public NWSEcomCardOnFileTests() {
            acceptorConfig = new AcceptorConfig {
                // data code values
                CardDataInputCapability = CardDataInputCapability.Manual, // Inside
                CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.None,
                //CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.ElectronicAuthentication,
                CardCaptureCapability = false,
                OperatingEnvironment = OperatingEnvironment.Internet_With_SSL,

                CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.NotAuthenticated,
                CardDataOutputCapability = CardDataOutputCapability.Unknown,
                TerminalOutputCapability = TerminalOutputCapability.Unknown,
                PinCaptureCapability = PinCaptureCapability.None,

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
            };

            //DE 127
            acceptorConfig.ServiceType = ServiceType.GPN_API;
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.DeTokenize;
            acceptorConfig.TokenizationType = TokenizationType.MerchantTokenization;
            acceptorConfig.MerchantId = "650000011573667";

            // gateway config
            NetworkGatewayConfig config = new NetworkGatewayConfig(NetworkGatewayType.NWS) {
                ServiceUrl = "test.txns-c.secureexchange.net",
                PrimaryPort = 15031,
                SecondaryEndpoint = "test.txns-e.secureexchange.net",
                SecondaryPort = 15031,
                CompanyId = "SPSA",
                TerminalId = "NWSDOTNET05",
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

        #region MasterCard 
        //Use case 6
        [TestMethod]
        public void COFTOkenized() {
            card = TestCards.MasterCardManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            Transaction response = card.Charge(193m)
                    .WithCurrency("USD")
                    .WithCvn(card.Cvn)
                    .WithCardHolderPresence(DE22_CardHolderPresence.CardHolder_NotPresent_Internet)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.NotAuthenticated)
                        //.WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationIdentifier, "IAXTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardDSRPCryptogram, "IMDTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardECommerceIndicators, "247")
                    .WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 10
        [TestMethod]
        public void COFTOkenAnd3DSAuthenticationSuccessful() {
            card = TestCards.MasterCardManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.ElectronicAuthentication;
            acceptorConfig.CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.CardIssuer;
            Transaction response = card.Charge(194m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.ThreeD_Secure_Authenticated)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.Authenticated)
                    .WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationData, "IADTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationIdentifier, "IAXTag")
                    .WithIssuerData(DE62_CardIssuerEntryTag.MastercardUCAFData, "IAUTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardDSRPCryptogram, "IMDTag")
                    .WithIssuerData(DE62_CardIssuerEntryTag.MastercardECommerceIndicators, "247")
                    .WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 11
        [TestMethod]
        public void COFTOkenAnd3DSAuthenticationAttempted() {
            card = TestCards.MasterCardManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.ElectronicAuthentication;
            acceptorConfig.CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.CardIssuer;
            Transaction response = card.Charge(195m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.ThreeD_Secure_Authentication_Attempted)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.AuthenticationAttempted)
                    .WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationData, "IADTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationIdentifier, "IAXTag")
                    .WithIssuerData(DE62_CardIssuerEntryTag.MastercardUCAFData, "IAUTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardDSRPCryptogram, "IMDTag")
                        .WithIssuerData(DE62_CardIssuerEntryTag.MastercardECommerceIndicators, "247")
                    .WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 12
        [TestMethod]
        public void COFTOkenAnd3DSAuthenticationFailed() {
            card = TestCards.MasterCardManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            Transaction response = card.Charge(6m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.CardHolder_NotPresent_Internet)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.NotAuthenticated)
                    .WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use Case 5
        [TestMethod]
        public void CardHolderWithCOF() {
            card = TestCards.MasterCardManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            card.TokenizationData = "4C3276D020FD1936C20921BBA0B7909B";
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithCvn(card.Cvn)
                    .WithCardHolderPresence(DE22_CardHolderPresence.CardHolder_NotPresent_Internet)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.NotAuthenticated)
                    .WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator,"C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        
        //Use case 7
        [TestMethod]
        public void COFPanwith3DSSuccesful() {
            card = TestCards.MasterCardManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            card.TokenizationData = "4C3276D020FD1936C20921BBA0B7909B";
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.ElectronicAuthentication;
            acceptorConfig.CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.CardIssuer;
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.ThreeD_Secure_Authenticated)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.Authenticated)
                    //.WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationData,"IADTag")
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardUCAFData, "IAUTag")
                    .WithIssuerData(DE62_CardIssuerEntryTag.MastercardECommerceIndicators,"247")
                    .WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 8
        [TestMethod]
        public void COFPanwith3DSAttempted() {
            card = TestCards.MasterCardManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            card.TokenizationData = "4C3276D020FD1936C20921BBA0B7909B";
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.ElectronicAuthentication;
            acceptorConfig.CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.CardIssuer;
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.ThreeD_Secure_Authentication_Attempted)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.AuthenticationAttempted)
                    //.WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationData, "IADTag")
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardUCAFData, "IAUTag")
                    .WithIssuerData(DE62_CardIssuerEntryTag.MastercardECommerceIndicators, "247")
                    .WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 9
        [TestMethod]
        public void COFPanwith3DSFailed() {
            card = TestCards.MasterCardManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            card.TokenizationData = "4C3276D020FD1936C20921BBA0B7909B";
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.CardHolder_NotPresent_Internet)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.NotAuthenticated)
                    .WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        #endregion

        #region Discover
        //Use case 6
        [TestMethod]
        public void COFTOkenizedDiscover() {
            card = TestCards.DiscoverManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            Transaction response = card.Charge(193m)
                    .WithCurrency("USD")
                    .WithCvn(card.Cvn)
                    .WithCardHolderPresence(DE22_CardHolderPresence.CardHolder_NotPresent_Internet)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.NotAuthenticated)
                        .WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationIdentifier, "IAXTag")
                        .WithIssuerData(DE62_CardIssuerEntryTag.MastercardDSRPCryptogram, "IMDTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardECommerceIndicators, "IMETag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 10
        [TestMethod]
        public void COFTOkenAnd3DSAuthenticationSuccessfulDiscover() {
            card = TestCards.DiscoverManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.ElectronicAuthentication;
            acceptorConfig.CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.CardIssuer;
            Transaction response = card.Charge(5m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.ThreeD_Secure_Authenticated)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.Authenticated)
                    .WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationData, "45AB3994839NFDN930203N3N4B5B3J4NO7G6T8F7")// "IADTag")
                        .WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationIdentifier, "IAXTag")
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardUCAFData, "IAUTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardDSRPCryptogram, "IMDTag")
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardECommerceIndicators, "IMETag")
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 11
        [TestMethod]
        public void COFTOkenAnd3DSAuthenticationAttemptedDiscover() {
            card = TestCards.DiscoverManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.ElectronicAuthentication;
            acceptorConfig.CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.CardIssuer;
            Transaction response = card.Charge(5m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.ThreeD_Secure_Authentication_Attempted)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.AuthenticationAttempted)
                    .WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationData, "45AB3994839NFDN930203N3N4B5B3J4NO7G6T8F7")// "IADTag")
                        .WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationIdentifier, "IAXTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardUCAFData, "IAUTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardDSRPCryptogram, "IMDTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardECommerceIndicators, "IMETag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 12
        [TestMethod]
        public void COFTOkenAnd3DSAuthenticationFailedDiscover() {
            card = TestCards.DiscoverManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            Transaction response = card.Charge(5m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.CardHolder_NotPresent_Internet)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.NotAuthenticated)
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use Case 5
        [TestMethod]
        public void CardHolderWithCOFDiscover() {
            card = TestCards.DiscoverManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            card.TokenizationData = "4D6B025705ADA3BC92392CB12D4C5A9E";
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithCvn(card.Cvn)
                    .WithCardHolderPresence(DE22_CardHolderPresence.CardHolder_NotPresent_Internet)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.NotAuthenticated)
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 7
        [TestMethod]
        public void COFPanwith3DSSuccesfulDiscover() {
            card = TestCards.DiscoverManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            card.TokenizationData = "4D6B025705ADA3BC92392CB12D4C5A9E";
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.ElectronicAuthentication;
            acceptorConfig.CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.CardIssuer;
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.ThreeD_Secure_Authenticated)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.Authenticated)
                    .WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationData, "45AB3994839NFDN930203N3N4B5B3J4NO7G6T8F7")// "IADTag")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 8
        [TestMethod]
        public void COFPanwith3DSAttemptedDiscover() {
            card = TestCards.DiscoverManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            card.TokenizationData = "4D6B025705ADA3BC92392CB12D4C5A9E";
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.ElectronicAuthentication;
            acceptorConfig.CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.CardIssuer;
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.ThreeD_Secure_Authentication_Attempted)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.AuthenticationAttempted)
                    .WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationData, "45AB3994839NFDN930203N3N4B5B3J4NO7G6T8F7")// "IADTag")
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardUCAFData, "IAUTag")
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardECommerceIndicators, "IMETag")
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 9
        [TestMethod]
        public void COFPanwith3DSFailedDiscover() {
            card = TestCards.DiscoverManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            card.TokenizationData = "4D6B025705ADA3BC92392CB12D4C5A9E";
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.CardHolder_NotPresent_Internet)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.NotAuthenticated)
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        #endregion

        #region Amex 
        //Use case 6
        [TestMethod]
        public void COFTOkenizedAmex() {
            card = TestCards.AmexManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            Transaction response = card.Charge(193m)
                    .WithCurrency("USD")
                    .WithCvn(card.Cvn)
                    .WithCardHolderPresence(DE22_CardHolderPresence.CardHolder_NotPresent_Internet)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.NotAuthenticated)
                        .WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationIdentifier, "IAXTag")
                        .WithIssuerData(DE62_CardIssuerEntryTag.MastercardDSRPCryptogram, "IMDTag")
                        .WithIssuerData(DE62_CardIssuerEntryTag.MastercardECommerceIndicators, "IMETag")
                    .WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 10
        [TestMethod]
        public void COFTOkenAnd3DSAuthenticationSuccessfulAmex() {
            card = TestCards.AmexManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.ElectronicAuthentication;
            acceptorConfig.CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.CardIssuer;
            Transaction response = card.Charge(194m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.ThreeD_Secure_Authenticated)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.Authenticated)
                    .WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationData, "45AB3994839NFDN930203N3N4B5B3J4NO7G6T8F7")// "IADTag")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 11
        [TestMethod]
        public void COFTOkenAnd3DSAuthenticationAttemptedAmex() {
            card = TestCards.AmexManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.ElectronicAuthentication;
            acceptorConfig.CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.CardIssuer;
            Transaction response = card.Charge(195m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.ThreeD_Secure_Authentication_Attempted)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.AuthenticationAttempted)
                    .WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationData, "45AB3994839NFDN930203N3N4B5B3J4NO7G6T8F7")//"IADTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationIdentifier, "IAXTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardUCAFData, "IAUTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardDSRPCryptogram, "IMDTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardECommerceIndicators, "IMETag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 12
        [TestMethod]
        public void COFTOkenAnd3DSAuthenticationFailedAmex() {
            card = TestCards.AmexManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            Transaction response = card.Charge(195m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.CardHolder_NotPresent_Internet)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.NotAuthenticated)
                    .WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 12
        [TestMethod]
        [Ignore]
        public void COFTOkenAnd3DSAuthenticationFailedAmex1() {
            card = TestCards.AmexManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            Transaction response = card.Charge(196m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.CardHolder_NotPresent_Internet)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.NotAuthenticated)
                    .WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use Case 5
        [TestMethod]
        public void CardHolderWithCOFAmex() {
            card = TestCards.AmexManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            card.TokenizationData = "4CCD57AAFF5477B986563BE1E70690B3";
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithCvn(card.Cvn)
                    .WithCardHolderPresence(DE22_CardHolderPresence.CardHolder_NotPresent_Internet)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.NotAuthenticated)
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 7
        [TestMethod]
        public void COFPanwith3DSSuccesfulAmex() {
            card = TestCards.AmexManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            card.TokenizationData = "4CCD57AAFF5477B986563BE1E70690B3";
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.ElectronicAuthentication;
            acceptorConfig.CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.CardIssuer;
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.ThreeD_Secure_Authenticated)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.Authenticated)
                    .WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationData, "45AB3994839NFDN930203N3N4B5B3J4NO7G6T8F7")// "IADTag")
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardUCAFData, "IAUTag")
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardECommerceIndicators, "IMETag")
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 8
        [TestMethod]
        public void COFPanwith3DSAttemptedAmex() {
            card = TestCards.AmexManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            card.TokenizationData = "4CCD57AAFF5477B986563BE1E70690B3";
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.ElectronicAuthentication;
            acceptorConfig.CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.CardIssuer;
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.ThreeD_Secure_Authentication_Attempted)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.AuthenticationAttempted)
                    .WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationData, "45AB3994839NFDN930203N3N4B5B3J4NO7G6T8F7")// "IADTag")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 9
        [TestMethod]
        public void COFPanwith3DSFailedAmex() {
            card = TestCards.AmexManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            card.TokenizationData = "4CCD57AAFF5477B986563BE1E70690B3";
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.CardHolder_NotPresent_Internet)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.NotAuthenticated)
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        #endregion

        #region Visa
        //Use case 6
        [TestMethod]
        public void COFTOkenizedVisa() {
            card = TestCards.VisaManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            Transaction response = card.Charge(193m)
                    .WithCurrency("USD")
                    .WithCvn(card.Cvn)
                    .WithCardHolderPresence(DE22_CardHolderPresence.CardHolder_NotPresent_Internet)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.NotAuthenticated)
                        //.WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationIdentifier, "IAXTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardDSRPCryptogram, "IMDTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardECommerceIndicators, "IMETag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 10
        [TestMethod]
        public void COFTOkenAnd3DSAuthenticationSuccessfulVisa() {
            card = TestCards.VisaManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.ElectronicAuthentication;
            acceptorConfig.CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.CardIssuer;
            Transaction response = card.Charge(194m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.ThreeD_Secure_Authenticated)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.Authenticated)
                    .WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationData, "45AB3994839NFDN930203N3N4B5B3J4NO7G6T8F7")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationIdentifier, "IAXTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardUCAFData, "IAUTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardDSRPCryptogram, "IMDTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardECommerceIndicators, "IMETag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 11
        [TestMethod]
        public void COFTOkenAnd3DSAuthenticationAttemptedVisa() {
            card = TestCards.VisaManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.ElectronicAuthentication;
            acceptorConfig.CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.CardIssuer;
            Transaction response = card.Charge(195m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.ThreeD_Secure_Authentication_Attempted)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.AuthenticationAttempted)
                    .WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationData, "45AB3994839NFDN930203N3N4B5B3J4NO7G6T8F7")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationIdentifier, "IAXTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardUCAFData, "IAUTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardDSRPCryptogram, "IMDTag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardECommerceIndicators, "IMETag")
                        //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 12
        [TestMethod]
        public void COFTOkenAnd3DSAuthenticationFailedVisa() {
            card = TestCards.VisaManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            Transaction response = card.Charge(5m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.CardHolder_NotPresent_Internet)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.NotAuthenticated)
                    .WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use Case 5
        [TestMethod]
        public void CardHolderWithCOFVisa() {
            card = TestCards.VisaManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            card.TokenizationData = "FBFE7A3F3AD34F8211E556327CA5E379";
            Transaction response = card.Charge(5m)
                    .WithCurrency("USD")
                    .WithCvn(card.Cvn)
                    .WithCardHolderPresence(DE22_CardHolderPresence.CardHolder_NotPresent_Internet)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.NotAuthenticated)
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 7
        [TestMethod]
        public void COFPanwith3DSSuccesfulVisa() {
            card = TestCards.VisaManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            card.TokenizationData = "FBFE7A3F3AD34F8211E556327CA5E379";
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.ElectronicAuthentication;
            acceptorConfig.CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.CardIssuer;
            Transaction response = card.Charge(7m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.ThreeD_Secure_Authenticated)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.Authenticated)
                    .WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationData, "45AB3994839NFDN930203N3N4B5B3J4NO7G6T8F7")
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardUCAFData, "IAUTag")
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardECommerceIndicators, "IMETag")
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 8
        [TestMethod]
        public void COFPanwith3DSAttemptedVisa() {
            card = TestCards.VisaManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            card.TokenizationData = "FBFE7A3F3AD34F8211E556327CA5E379";
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.ElectronicAuthentication;
            acceptorConfig.CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.CardIssuer;
            Transaction response = card.Charge(8m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.ThreeD_Secure_Authentication_Attempted)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.AuthenticationAttempted)
                    .WithIssuerData(DE62_CardIssuerEntryTag.CardIssuerAuthenticationData, "45AB3994839NFDN930203N3N4B5B3J4NO7G6T8F7")
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardUCAFData, "IAUTag")
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardECommerceIndicators, "IMETag")
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //Use case 9
        [TestMethod]
        public void COFPanwith3DSFailedVisa() {
            card = TestCards.VisaManual();
            card.EntryMethod = ManualEntryMethod.CardOnFile;
            card.TokenizationData = "FBFE7A3F3AD34F8211E556327CA5E379";
            Transaction response = card.Charge(9m)
                    .WithCurrency("USD")
                    .WithCardHolderPresence(DE22_CardHolderPresence.CardHolder_NotPresent_Internet)
                    .WithAuthenticationMethod(CardHolderAuthenticationMethod.NotAuthenticated)
                    //.WithIssuerData(DE62_CardIssuerEntryTag.MastercardCITMITIndicator, "C101")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        #endregion
    }
}
