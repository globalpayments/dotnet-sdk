using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Network {
    [TestClass]
    public class NWSDebitTokenizationTests {
        private DebitTrackData track;
        private CreditCardData card;
        private AcceptorConfig acceptorConfig;

        public NWSDebitTokenizationTests() {
            Address address = new Address();
            address.Name = "My STORE";
            address.StreetAddress1 = "1 MY STREET";
            address.City = "MYTOWN";
            address.PostalCode = "90210";
            address.State = "KY";
            address.Country = "USA";

            acceptorConfig = new AcceptorConfig();
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

            //DE 127
            acceptorConfig.ServiceType = ServiceType.GPN_API;
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.DeTokenize;
            acceptorConfig.TokenizationType = TokenizationType.MerchantTokenization;
            acceptorConfig.MerchantId = "650000011573667";

            //DE 127
            acceptorConfig.SupportedEncryptionType = EncryptionType.TDES;
            acceptorConfig.ServiceType = ServiceType.GPN_API;
            acceptorConfig.OperationType = OperationType.Decrypt;

            // gateway config
            NetworkGatewayConfig config = new NetworkGatewayConfig(NetworkGatewayType.NWS);
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

            // debit card
            track = new DebitTrackData {
                TokenizationData = "0E86DCF53051FE3AB75F223525C26DA3",
                Value = "4355567063338=2012101HJNw/ewskBgnZqkL",
                PinBlock = "62968D2481D231E1A504010024A00014"
            };
        }

        [TestMethod]
        public void Test_File_Action() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.Tokenize;
            DebitTrackData track = new DebitTrackData();
            track.Value = "4355567063338=2012101HJNw/ewskBgnZqkL";
            track.PinBlock = "62968D2481D231E1A504010024A00014";
            track.TokenizationData = "4355567063338";
            Transaction response = track.FileAction()
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_001_Debit_Manual_Auth() {
                Transaction response = track.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.Equals("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Debit_Manual_Sale() {
            Transaction response = track.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.Equals("000", response.ResponseCode);

        }
            //force draft capture
        [TestMethod]
        public void Test_016_AuthCapture() {
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.Equals("1100", pmi.MessageTransactionIndicator);
            Assert.Equals("000800", pmi.ProcessingCode);
            Assert.Equals("101", pmi.FunctionCode);

            // check response
            Assert.Equals("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(response.AuthorizedAmount)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(captureResponse);

            // check message data
            pmi = captureResponse.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.Equals("1220", pmi.MessageTransactionIndicator);
            Assert.Equals("000800", pmi.ProcessingCode);
            Assert.Equals("1376", pmi.MessageReasonCode);
            Assert.Equals("201", pmi.FunctionCode);

            // check response
            Assert.Equals("000", captureResponse.ResponseCode);
        }
        
        [TestMethod]
        public void Test_004_Debit_Refund() {
            Transaction response = track.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.Equals("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_Debit_Balance_Inquiry(){
            Transaction response = track.BalanceInquiry()
                        .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.Equals("1100", pmi.MessageTransactionIndicator);
            Assert.Equals("310800", pmi.ProcessingCode);
            Assert.Equals("108", pmi.FunctionCode);
            Assert.Equals("000", response.ResponseCode);
        }
        
        [TestMethod]
        public void Test_Sale_Reversal() {
            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);
            Transaction response = track.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);

            response.NTSData = ntsData;
        
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.Equals("1200", pmi.MessageTransactionIndicator);
            Assert.Equals("000800", pmi.ProcessingCode);
            Assert.Equals("200", pmi.FunctionCode);

            Transaction reversal = response.Reverse(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(reversal);

            // check message data
            pmi = reversal.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.Equals("1420", pmi.MessageTransactionIndicator);
            Assert.Equals("000800", pmi.ProcessingCode);
            Assert.Equals("400", pmi.FunctionCode);
            Assert.Equals("4021", pmi.MessageReasonCode);

            // check response
            Assert.Equals("400", reversal.ResponseCode);
        }

    }
}
