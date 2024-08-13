using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Network {
    [TestClass]
    public class NWSCombined3DesTokenizationTest {
        private CreditCardData card;
        private CreditTrackData track;

        public NWSCombined3DesTokenizationTest() {
            AcceptorConfig acceptorConfig = new AcceptorConfig();

            //DE 127
            acceptorConfig.ServiceType = ServiceType.GPN_API;
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.Tokenize;
            acceptorConfig.TokenizationType = TokenizationType.MerchantTokenization;
            acceptorConfig.MerchantId = "650000011573667";
            acceptorConfig.SupportedEncryptionType = Api.Network.Entities.EncryptionType.TDES;
            acceptorConfig.ServiceType = ServiceType.GPN_API;
            acceptorConfig.OperationType = OperationType.Decrypt;

            // gateway config
            NetworkGatewayConfig config = new NetworkGatewayConfig(Api.Network.Enums.NetworkGatewayType.NWS);
            config.ServiceUrl = "test.txns-c.secureexchange.net";
            config.PrimaryPort = 15031;
            config.SecondaryEndpoint = "test.txns-e.secureexchange.net";
            config.SecondaryPort = 15031;
            config.CompanyId = "SPSA";
            config.TerminalId = "NWSDOTNET01";
            config.UniqueDeviceId = "0001";
            config.AcceptorConfig = acceptorConfig;
            config.EnableLogging = true;
            config.StanProvider = StanGenerator.GetInstance();
            config.BatchProvider = BatchProvider.GetInstance();

            ServicesContainer.ConfigureService(config);
        }

        #region Track 2 
        [TestMethod]
        public void Test_CombinedFile_Action_Mastercard() {
            var encryptionData = new EncryptionData();
            encryptionData.KTB = "3A2067D00508DBE43E3342CC77B0575E04D9191B380C88036DD82D54C834DCB4130F52560AA9551B";
            encryptionData.KSN = "F000019990E00003";
            encryptionData.TrackNumber = TrackNumber.TrackTwo.ToString();

            card = TestCards.MasterCardManual();
            card.TokenizationData = "5506740000004316";
            card.EncryptionData = encryptionData;

            Transaction response = card.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_CombinedFile_Action_Visa() {
            var encryptionData = new EncryptionData();
            encryptionData.KTB = "3A2067D00508DBE43E3342CC77B0575E04D9191B380C88036DD82D54C834DCB4130F52560AA9551B";
            encryptionData.KSN = "F000019990E00003";
            encryptionData.TrackNumber = TrackNumber.TrackTwo.ToString();


            card = TestCards.VisaManual();
            card.TokenizationData = "4012002000060016";
            card.EncryptionData = encryptionData;

            Transaction response = card.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_CombinedFile_Action_Discover() {
            var encryptionData = new EncryptionData();
            encryptionData.KTB = "3A2067D00508DBE43E3342CC77B0575E04D9191B380C88036DD82D54C834DCB4130F52560AA9551B";
            encryptionData.KSN = "F000019990E00003";
            encryptionData.TrackNumber = TrackNumber.TrackTwo.ToString();

            card = TestCards.DiscoverManual();
            card.TokenizationData = "6550006599174230";
            card.EncryptionData = encryptionData;


            Transaction response = card.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_CombinedFile_Action_Amex() {
            var encryptionData = new EncryptionData();
            encryptionData.KTB = "3A2067D00508DBE43E3342CC77B0575E04D9191B380C88036DD82D54C834DCB4130F52560AA9551B";
            encryptionData.KSN = "F000019990E00003";
            encryptionData.TrackNumber = TrackNumber.TrackTwo.ToString();

            card = TestCards.AmexManual();
            card.TokenizationData = "372700699251018";
            card.EncryptionData = encryptionData;
            Transaction response = card.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        #endregion

        #region Pan Data
        [TestMethod]
        public void Test_CombinedFile_Action_MastercardPAN() {
            var encryptionData = new EncryptionData();
            encryptionData.KTB = "3A2067D00508DBE43E3342CC77B0575E17401487FC0B377F";
            encryptionData.KSN = "F000019990E00003";
            encryptionData.TrackNumber = TrackNumber.PAN.ToString();

            card = TestCards.MasterCardManual();
            card.TokenizationData = "5506740000004316";
            card.EncryptionData = encryptionData;

            Transaction response = card.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_CombinedFile_Action_VisaPAN() {
            var encryptionData = new EncryptionData();
            encryptionData.KTB = "49AB0D7DF39F4EAA3ADEB107CCCC03D0";
            encryptionData.KSN = "F000019990E00003";
            encryptionData.TrackNumber = TrackNumber.PAN.ToString();

            card = TestCards.VisaManual();
            card.TokenizationData = "4012002000060016";
            card.EncryptionData = encryptionData;

            Transaction response = card.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_CombinedFile_Action_DiscoverPAN() {
            var encryptionData = new EncryptionData();
            encryptionData.KTB = "3A2067D00508DBE43E3342CC77B0575E17401487FC0B377F";
            encryptionData.KSN = "F000019990E00003";
            encryptionData.TrackNumber = TrackNumber.PAN.ToString();

            card = TestCards.DiscoverManual();
            card.TokenizationData = "6550006599174230";
            card.EncryptionData = encryptionData;


            Transaction response = card.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_CombinedFile_Action_AmexPAN() {
            var encryptionData = new EncryptionData();
            encryptionData.KTB = "3A2067D00508DBE43E3342CC77B0575E17401487FC0B377F";
            encryptionData.KSN = "F000019990E00003";
            encryptionData.TrackNumber = TrackNumber.PAN.ToString();

            card = TestCards.AmexManual();
            card.TokenizationData = "372700699251018";
            card.EncryptionData = encryptionData;
            Transaction response = card.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        #endregion

        #region Track 1
        [TestMethod]
        public void Test_CombinedFile_Action_MastercardTrack1() {
            var encryptionData = new EncryptionData();
            encryptionData.KTB = "EC7EB2F7BD67A2784F1AD9270EFFD90DD121B8653623911C6BC7B427F726A49F834CA051A6C1CC9CBB17910A1DBA209796BB6D08B8C374A2912AB018A679FA5A0A0EDEADF349FED3 ";
            encryptionData.KSN = "F000019990E00003";
            encryptionData.TrackNumber = TrackNumber.TrackOne.ToString();

            card = TestCards.MasterCardManual();
            card.TokenizationData = "5506740000004316";
            card.EncryptionData = encryptionData;

            Transaction response = card.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_CombinedFile_Action_VisaTrack1() {
            var encryptionData = new EncryptionData();
            encryptionData.KTB = "EC7EB2F7BD67A2784F1AD9270EFFD90DD121B8653623911C6BC7B427F726A49F834CA051A6C1CC9CBB17910A1DBA209796BB6D08B8C374A2912AB018A679FA5A0A0EDEADF349FED3";
            encryptionData.KSN = "F000019990E00003";
            encryptionData.TrackNumber = TrackNumber.TrackOne.ToString();


            card = TestCards.VisaManual();
            card.TokenizationData = "4012002000060016";
            card.EncryptionData = encryptionData;

            Transaction response = card.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_CombinedFile_Action_DiscoverTrack1() {
            var encryptionData = new EncryptionData();
            encryptionData.KTB = "EC7EB2F7BD67A2784F1AD9270EFFD90DD121B8653623911C6BC7B427F726A49F834CA051A6C1CC9CBB17910A1DBA209796BB6D08B8C374A2912AB018A679FA5A0A0EDEADF349FED3";
            encryptionData.KSN = "F000019990E00003";
            encryptionData.TrackNumber = TrackNumber.TrackOne.ToString();

            card = TestCards.DiscoverManual();
            card.TokenizationData = "6550006599174230";
            card.EncryptionData = encryptionData;


            Transaction response = card.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_CombinedFile_Action_AmexTrack1() {
            var encryptionData = new EncryptionData();
            encryptionData.KTB = "EC7EB2F7BD67A2784F1AD9270EFFD90DD121B8653623911C6BC7B427F726A49F834CA051A6C1CC9CBB17910A1DBA209796BB6D08B8C374A2912AB018A679FA5A0A0EDEADF349FED3";
            encryptionData.KSN = "F000019990E00003";
            encryptionData.TrackNumber = TrackNumber.TrackOne.ToString();

            card = TestCards.AmexManual();
            card.TokenizationData = "372700699251018";
            card.EncryptionData = encryptionData;
            Transaction response = card.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        #endregion
    }
}
