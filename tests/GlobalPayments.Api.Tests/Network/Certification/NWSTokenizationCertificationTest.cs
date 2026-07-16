using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Network.Certification {
    [TestClass]
    public class NWSTokenizationCertificationTest {
        private CreditCardData card;
        private CreditTrackData track;
        AcceptorConfig acceptorConfig = new AcceptorConfig();

        public NWSTokenizationCertificationTest() {
            Address address = new Address();
            address.Name = "My STORE";
            address.StreetAddress1 = "1 MY STREET";
            address.City = "MYTOWN";
            address.PostalCode = "90210";
            address.State = "KY";
            address.Country = "USA";
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
            acceptorConfig.SupportsEmvPin = true;
            acceptorConfig.EchoSettlementData = true;
            acceptorConfig.IncrementalAuthIndicator = false;
            //DE 127
            acceptorConfig.ServiceType = ServiceType.GPN_API;
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.DeTokenize;
            acceptorConfig.TokenizationType = TokenizationType.MerchantTokenization;
            acceptorConfig.MerchantId = "650000011573667";

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

        /// <summary>
        /// Tokenization transaction using opertation type 1.
        /// </summary>
        [TestMethod]
        public void Test_File_Action() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.Tokenize;
            card = TestCards.MasterCardManual();
            card.TokenizationData = "5506740000004316";
            var response = card.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        /// <summary>
        /// Tokenization transaction using operation type 5 (single use token)
        /// </summary>
        [TestMethod]
        public void Test_File_Action_Mastercard_SingleUseToken() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.SingleUseToken;
            card = TestCards.MasterCardManual();
            card.TokenizationData = "5506740000004316";
            card.Cvn = "123";
            Transaction response = card.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        /// <summary>
        /// Tokenization transaction using operation type 6 (converting Single use to Multi-use token)
        /// </summary>
        [TestMethod]
        public void Test_CombinedFile_Action_Mastercard_SingleToMultiUseToken() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.SingleToMultiUseToken;
            card = TestCards.MasterCardManual();
            card.TokenizationData = "FCB87D22C00D15F19A18991289E32732";
            card.Cvn = "123";
            Transaction response = card.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        /// <summary>
        /// Tokenization Sale transaction, Return Tokenization Transaction
        /// </summary>
        [TestMethod]
        public void Test_Sale_Reversal() {
            card = TestCards.MasterCardManual();
            card.TokenizationData = "FCB87D22C00D15F19A18991289E32732";
            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);

            Transaction response = card.Charge(11m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
            response.NTSData = ntsData;

            Transaction reversal = response.Reverse(11m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("400", reversal.ResponseCode);
        }

        /// <summary>
        /// Tokenization Sale transaction, Refund Tokenization Transaction
        /// </summary>
        [TestMethod]
        public void Test_Sale_Refund() {
            card = TestCards.MasterCardManual();
            card.TokenizationData = "FCB87D22C00D15F19A18991289E32732";
            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);

            Transaction response = card.Charge(11m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction refundTrans = response.Refund(11m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(refundTrans);
            Assert.AreEqual("000", refundTrans.ResponseCode);
        }

        /// <summary>
        /// Tokenization sale transaction and then void (1420) it.
        /// </summary>
        [TestMethod]
        public void Test_015_Credit_Void() {
            card = TestCards.MasterCardManual();
            card.TokenizationData = "FCB87D22C00D15F19A18991289E32732";

            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            // reverse the transaction
            Transaction reverseResponse = response.Void().Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }
    }
}
