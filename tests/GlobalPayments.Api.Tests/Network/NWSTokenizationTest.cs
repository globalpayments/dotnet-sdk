using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Network
{
    [TestClass]
    public class NWSTokenizationTest {
        private CreditCardData card;
        private CreditTrackData track;

        public NWSTokenizationTest() {
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
            acceptorConfig.SupportsEmvPin = true;

            //DE 127
            acceptorConfig.ServiceType = ServiceType.GPN_API;
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.DeTokenize;
            acceptorConfig.TokenizationType = TokenizationType.MerchantTokenization;
            acceptorConfig.MerchantId = "650000011573667";

            //DE 127
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

        [TestMethod]
        public void Test_File_Action() {
            card = TestCards.MasterCardManual();
            card.TokenizationData = "5506740000004316";// "5473500000000014";
            Transaction response = card.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_001_Credit_Manual_Auth() {
            card = TestCards.MasterCardManual();
            card.TokenizationData = "4C3276D020FD1936C20921BBA0B7909B";// "8E4BDE85FCF1FD72A6CC9A8AC0EB740A";// "8E4BDE85FCF1FD72A6CC9A8AC0EB740A";
            Transaction response = card.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        
        [TestMethod]
        public void Test_002_Credit_Manual_Sale() {
            card = TestCards.MasterCardManual();
            card.TokenizationData = "4C3276D020FD1936C20921BBA0B7909B";// "8E4BDE85FCF1FD72A6CC9A8AC0EB740A";
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        //force draft capture
        [TestMethod]
        public void Test_016_AuthCapture() {
            card = TestCards.MasterCardManual();
            card.TokenizationData = "4C3276D020FD1936C20921BBA0B7909B";// "8E4BDE85FCF1FD72A6CC9A8AC0EB740A";
            Transaction response = card.Authorize(10m, true)
                    .WithCurrency("USD")
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
        public void Test_004_Credit_Refund() {
            card = TestCards.MasterCardManual();
            card.TokenizationData = "4C3276D020FD1936C20921BBA0B7909B";// "8E4BDE85FCF1FD72A6CC9A8AC0EB740A";
            Transaction response = card.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_Credit_Balance_Inquiry() {
            card = TestCards.MasterCardManual();
            card.TokenizationData = "4C3276D020FD1936C20921BBA0B7909B";// "8E4BDE85FCF1FD72A6CC9A8AC0EB740A";
            Transaction response = card.BalanceInquiry()
                        .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("313000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Sale_Reversal() {
            card = TestCards.MasterCardManual();
            card.TokenizationData = "4C3276D020FD1936C20921BBA0B7909B";// "8E4BDE85FCF1FD72A6CC9A8AC0EB740A";

            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
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
        public void Test_015_Credit_Void() {
            card = TestCards.MasterCardManual();
            card.TokenizationData = "4C3276D020FD1936C20921BBA0B7909B";// "8E4BDE85FCF1FD72A6CC9A8AC0EB740A";

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

        //mastercard purchasing
        [TestMethod]
        public void Test_File_Action_Mastercard_Purchasing() {
            card = TestCards.MasterCardPurchasingManual();
            card.TokenizationData = "5302490000004066";
            Transaction response = card.FileAction()
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_001_Credit_Manual_Auth_Mastercard_Purchasing() {
            card = TestCards.MasterCardPurchasingManual();
            card.TokenizationData = "4B76646C9A22E481DFB94CF1314E9301";
                Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Credit_Manual_Sale_Mastercard_Purchasing() {
            card = TestCards.MasterCardPurchasingManual();
            card.TokenizationData = "4B76646C9A22E481DFB94CF1314E9301";
            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
         
        //force draft capture
        [TestMethod]
        public void Test_016_AuthCapture_Mastercard_Purchasing() {
            card = TestCards.MasterCardPurchasingManual();
            card.TokenizationData = "4B76646C9A22E481DFB94CF1314E9301";
            Transaction response = card.Authorize(10m, true)
                        .WithCurrency("USD")
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
        public void Test_004_Credit_Refund_Mastercard_Purchasing() {
            card = TestCards.MasterCardPurchasingManual();
            card.TokenizationData = "4B76646C9A22E481DFB94CF1314E9301";
            Transaction response = card.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_Credit_Balance_Inquiry_Mastercard_Purchasing() {
            card = TestCards.MasterCardPurchasingManual();
            card.TokenizationData = "4B76646C9A22E481DFB94CF1314E9301";
            Transaction response = card.BalanceInquiry()
                        .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("313000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Sale_Reversal_Mastercard_Purchasing() {
            card = TestCards.MasterCardPurchasingManual();
            card.TokenizationData = "4B76646C9A22E481DFB94CF1314E9301";

            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);
            Transaction response = card.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);

            response.NTSData = ntsData;
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check response
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
        public void Test_015_Credit_Void_Mastercard_Purchasing() {
            card = TestCards.MasterCardPurchasingManual();
            card.TokenizationData = "4B76646C9A22E481DFB94CF1314E9301";

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

        //Visa Manual
        [TestMethod]
        public void Test_File_Action_Visa() {
            card = TestCards.VisaManual();
            card.TokenizationData = "4012002000060016";
            Transaction response = card.FileAction()
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_001_Credit_Manual_Auth_Visa() {
            card = TestCards.VisaManual();
            card.TokenizationData = "FBFE7A3F3AD34F8211E556327CA5E379";
            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Credit_Manual_Sale_Visa() {
            card = TestCards.VisaManual();
            card.TokenizationData = "FBFE7A3F3AD34F8211E556327CA5E379";
            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //force draft capture
        [TestMethod]
        public void Test_016_AuthCapture_Visa() {
            card = TestCards.VisaManual();
            card.TokenizationData = "FBFE7A3F3AD34F8211E556327CA5E379";
            Transaction response = card.Authorize(10m, true)
                        .WithCurrency("USD")
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
        public void Test_004_Credit_Refund_Visa() {
            card = TestCards.VisaManual();
            card.TokenizationData = "FBFE7A3F3AD34F8211E556327CA5E379";
            Transaction response = card.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_Credit_Balance_Inquiry_Visa() {
            card = TestCards.VisaManual();
            card.TokenizationData = "FBFE7A3F3AD34F8211E556327CA5E379";
            Transaction response = card.BalanceInquiry()
                        .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("313000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Sale_Reversal_Visa() {
            card = TestCards.VisaManual();
            card.TokenizationData = "FBFE7A3F3AD34F8211E556327CA5E379";

            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            response.NTSData = ntsData;
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check response
            //Assert.AreEqual("000", response.getResponseCode());

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
        public void Test_015_Credit_Void_Visa() {
            card = TestCards.VisaManual();
            card.TokenizationData = "FBFE7A3F3AD34F8211E556327CA5E379";

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

        //Visa Manual
        [TestMethod]
        public void Test_File_Action_Visa_Purchasing() {
            card = TestCards.VisaPurchasingManual();
            card.TokenizationData = "4012002000060016";
            Transaction response = card.FileAction()
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

        }

        [TestMethod]
        public void Test_001_Credit_Manual_Auth_Visa_Purchasing() {
            card = TestCards.VisaPurchasingManual();
            card.TokenizationData = "E099BF9FBFEA0A06FF7B7779241CAFDB";
            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        
        [TestMethod]
        public void Test_002_Credit_Manual_Sale_Visa_Purchasing() {
            card = TestCards.VisaPurchasingManual();
            card.TokenizationData = "E099BF9FBFEA0A06FF7B7779241CAFDB";
            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        
        //force draft capture
        [TestMethod]
        public void Test_016_AuthCapture_Visa_Purchasing() {
            card = TestCards.VisaPurchasingManual();
            card.TokenizationData = "E099BF9FBFEA0A06FF7B7779241CAFDB";
            Transaction response = card.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003800", pmi.ProcessingCode);
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
            Assert.AreEqual("003800", pmi.ProcessingCode);
            Assert.AreEqual("1376", pmi.MessageReasonCode);
            Assert.AreEqual("201", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }
        
        [TestMethod]
        public void Test_004_Credit_Refund_Visa_Purchasing() {
            card = TestCards.VisaPurchasingManual();
            card.TokenizationData = "E099BF9FBFEA0A06FF7B7779241CAFDB";
            Transaction response = card.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_Credit_Balance_Inquiry_Visa_Purchasing() {
            card = TestCards.VisaPurchasingManual();
            card.TokenizationData = "E099BF9FBFEA0A06FF7B7779241CAFDB";
            Transaction response = card.BalanceInquiry()
                        .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("313800", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Sale_Reversal_Visa_Purchasing() {
            card = TestCards.VisaPurchasingManual();
            card.TokenizationData = "E099BF9FBFEA0A06FF7B7779241CAFDB";

            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            response.NTSData = ntsData;
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003800", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            Transaction reversal = response.Reverse(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(reversal);

            // check message data
            pmi = reversal.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003800", pmi.ProcessingCode);
            Assert.AreEqual("400", pmi.FunctionCode);
            Assert.AreEqual("4021", pmi.MessageReasonCode);

            // check response
            Assert.AreEqual("400", reversal.ResponseCode);
        }

        [TestMethod]
        public void Test_015_Credit_Void_Visa_Purchasing() {
            card = TestCards.VisaPurchasingManual();
            card.TokenizationData = "E099BF9FBFEA0A06FF7B7779241CAFDB";

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

        //visa corporate
        [TestMethod]
        public void Test_File_Action_Visa_Corporate() {
            card = TestCards.VisaCorporateManual();
            card.TokenizationData = "4013872718148777";
            Transaction response = card.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }   

        [TestMethod]
        public void Test_001_Credit_Manual_Auth_Visa_Corporate() {
                card = TestCards.VisaCorporateManual();
                card.TokenizationData = "5F052EB94571A12965D2D6343525E9CA";
                Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Credit_Manual_Sale_Visa_Corporate() {
            card = TestCards.VisaCorporateManual();
            card.TokenizationData = "5F052EB94571A12965D2D6343525E9CA";
            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        
        //force draft capture
        [TestMethod]
        public void Test_016_AuthCapture_Visa_Corporate() {
            card = TestCards.VisaCorporateManual();
            card.TokenizationData = "5F052EB94571A12965D2D6343525E9CA";
            Transaction response = card.Authorize(10m, true)
                        .WithCurrency("USD")
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
        public void Test_004_Credit_Refund_Visa_Corporate() {
            card = TestCards.VisaCorporateManual();
            card.TokenizationData = "5F052EB94571A12965D2D6343525E9CA";
            Transaction response = card.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_Credit_Balance_Inquiry_Visa_Corporate() {
            card = TestCards.VisaCorporateManual();
            card.TokenizationData = "5F052EB94571A12965D2D6343525E9CA";
            Transaction response = card.BalanceInquiry()
                .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("313000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }
        
        [TestMethod]
        public void Test_Sale_Reversal_Visa_Corporate() {
            card = TestCards.VisaCorporateManual();
            card.TokenizationData = "5F052EB94571A12965D2D6343525E9CA";

            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
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
        public void Test_015_Credit_Void_Visa_Corporate() {
            card = TestCards.VisaCorporateManual();
            card.TokenizationData = "5F052EB94571A12965D2D6343525E9CA";

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


        //discover
        [TestMethod]
        public void Test_File_Action_Discover() {
            card = TestCards.DiscoverManual();
            card.TokenizationData = "6550006599174230";
            Transaction response = card.FileAction()
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

        }

        [TestMethod]
        public void Test_001_Credit_Manual_Auth_Discover() {
            card = TestCards.DiscoverManual();
            card.TokenizationData = "4D6B025705ADA3BC92392CB12D4C5A9E";
            Transaction response = card.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Credit_Manual_Sale_Discover() {
            card = TestCards.DiscoverManual();
            card.TokenizationData = "4D6B025705ADA3BC92392CB12D4C5A9E";
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

        }

        //force draft capture
        [TestMethod]
        public void Test_016_AuthCapture_Discover() {
            card = TestCards.DiscoverManual();
            card.TokenizationData = "4D6B025705ADA3BC92392CB12D4C5A9E";
            Transaction response = card.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);

            // check response
            //Assert.AreEqual("000", response.ResponseCode);
            //response.TransactionReference.AuthCode = "00479A";

            Transaction captureResponse = response.Capture(10m)
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
        public void Test_004_Credit_Refund_Discover() {
            card = TestCards.DiscoverManual();
            card.TokenizationData = "4D6B025705ADA3BC92392CB12D4C5A9E";
            Transaction response = card.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_Credit_Balance_Inquiry_Discover() {
            card = TestCards.DiscoverManual();
            card.TokenizationData = "4D6B025705ADA3BC92392CB12D4C5A9E";
            Transaction response = card.BalanceInquiry()
                        .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("313000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Sale_Reversal_Discover() {
            card = TestCards.DiscoverManual();
            card.TokenizationData = "4D6B025705ADA3BC92392CB12D4C5A9E";

            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
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
        public void Test_015_Credit_Void_Discover() {
            card = TestCards.DiscoverManual();
            card.TokenizationData = "4D6B025705ADA3BC92392CB12D4C5A9E";

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
        
        //Amex
        [TestMethod]
        public void Test_File_Action_Amex() {
            card = TestCards.AmexManual();
            card.TokenizationData = "372700699251018";
            Transaction response = card.FileAction()
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_001_Credit_Manual_Auth_Amex() {
            card = TestCards.AmexManual();
            card.TokenizationData = "4CCD57AAFF5477B986563BE1E70690B3";
            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Credit_Manual_Sale_Amex() {
            card = TestCards.AmexManual();
            card.TokenizationData = "4CCD57AAFF5477B986563BE1E70690B3";
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
    
        //force draft capture
        [TestMethod]
        public void Test_016_AuthCapture_Amex()  {
            card = TestCards.AmexManual();
            card.TokenizationData = "4CCD57AAFF5477B986563BE1E70690B3";
            Transaction response = card.Authorize(10m, true)
                .WithCurrency("USD")
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
        public void Test_004_Credit_Refund_Amex() {
            card = TestCards.AmexManual();
            card.TokenizationData = "4CCD57AAFF5477B986563BE1E70690B3";
            Transaction response = card.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_Credit_Balance_Inquiry_Amex()  {
            card = TestCards.AmexManual();
            card.TokenizationData = "4CCD57AAFF5477B986563BE1E70690B3";
            Transaction response = card.BalanceInquiry()
                        .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("313000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Sale_Reversal_Amex() {
            card = TestCards.AmexManual();
            card.TokenizationData = "4CCD57AAFF5477B986563BE1E70690B3";

            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            response.NTSData = ntsData;
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check response
            //Assert.AreEqual("000", response.getResponseCode());

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
        public void Test_015_Credit_Void_Amex() {
            card = TestCards.AmexManual();
            card.TokenizationData = "4CCD57AAFF5477B986563BE1E70690B3";

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

        //JCB
        [TestMethod]
        public void Test_File_Action_Jcb() {
            card = TestCards.JcbManual();
            card.TokenizationData = "3566007770007321";
            Transaction response = card.FileAction()
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_001_Credit_Manual_Auth_Jcb() {
            card = TestCards.JcbManual();
            card.TokenizationData = "1DD5C11868C9717809EDD0BB12ACF61C";
            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Credit_Manual_Sale_Jcb() {
            card = TestCards.JcbManual();
            card.TokenizationData = "1DD5C11868C9717809EDD0BB12ACF61C";
            Transaction response = card.Charge(10)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
    
        //force draft capture
        [TestMethod]
        public void Test_016_AuthCapture_Jcb() {
            card = TestCards.JcbManual();
            card.TokenizationData = "1DD5C11868C9717809EDD0BB12ACF61C";
            Transaction response = card.Authorize(10m, true)
                        .WithCurrency("USD")
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
        public void Test_004_Credit_Refund_Jcb() {
            card = TestCards.JcbManual();
            card.TokenizationData = "1DD5C11868C9717809EDD0BB12ACF61C";
            Transaction response = card.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_Credit_Balance_Inquiry_Jcb() {
            card = TestCards.JcbManual();
            card.TokenizationData = "1DD5C11868C9717809EDD0BB12ACF61C";
            Transaction response = card.BalanceInquiry()
                        .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("313000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Sale_Reversal_Jcb() {
            card = TestCards.JcbManual();
            card.TokenizationData = "1DD5C11868C9717809EDD0BB12ACF61C";

            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            response.NTSData = ntsData;
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check response
            //Assert.AreEqual("000", response.getResponseCode());

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
        public void Test_015_Credit_Void_Jcb() {
            card = TestCards.JcbManual();
            card.TokenizationData = "1DD5C11868C9717809EDD0BB12ACF61C";

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

        //Union Pay
        [TestMethod]
        public void Test_File_Action_Union_Pay() {
            card = TestCards.UnionPayManual();
            card.TokenizationData = "6221260012345674";
            Transaction response = card.FileAction()
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

        }

        [TestMethod]
        public void Test_001_Credit_Manual_Auth_Union_Pay() {
            card = TestCards.UnionPayManual();
            card.TokenizationData = "F1C96E421546E54F77CB74EDFCDDCE65";
            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Credit_Manual_Sale_Union_Pay() {
            card = TestCards.UnionPayManual();
            card.TokenizationData = "F1C96E421546E54F77CB74EDFCDDCE65";
            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //force draft capture
        [TestMethod]
        public void Test_016_AuthCapture_Union_Pay() {
            card = TestCards.UnionPayManual();
            card.TokenizationData = "F1C96E421546E54F77CB74EDFCDDCE65";
            Transaction response = card.Authorize(10m, true)
                        .WithCurrency("USD")
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
        public void Test_004_Credit_Refund_Union_Pay() {
            card = TestCards.UnionPayManual();
            card.TokenizationData = "F1C96E421546E54F77CB74EDFCDDCE65";
            Transaction response = card.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_Credit_Balance_Inquiry_Union_Pay() {
            card = TestCards.UnionPayManual();
            card.TokenizationData = "F1C96E421546E54F77CB74EDFCDDCE65";
            Transaction response = card.BalanceInquiry()
                        .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("313000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }
        
        [TestMethod]
        public void Test_Sale_Reversal_Union_Pay() {
            card = TestCards.UnionPayManual();
            card.TokenizationData = "F1C96E421546E54F77CB74EDFCDDCE65";

            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
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
        public void Test_015_Credit_Void_Union_Pay() {
            card = TestCards.UnionPayManual();
            card.TokenizationData  = "F1C96E421546E54F77CB74EDFCDDCE65";

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

        //Paypal
        [TestMethod]
        public void Test_File_Action_Paypal() {
            card = TestCards.Paypal();
            card.TokenizationData = "6506001000010029";
            Transaction response = card.FileAction()
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_001_Credit_Manual_Auth_Paypal() {
            card = TestCards.Paypal();
            card.TokenizationData = "77D2FFAEE984E740AC487E1E5A8E6726";
            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Credit_Manual_Sale_Paypal() {
            card = TestCards.Paypal();
            card.TokenizationData = "77D2FFAEE984E740AC487E1E5A8E6726";
            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        //force draft capture
        [TestMethod]
        public void Test_016_AuthCapture_Paypal() {
            card = TestCards.Paypal();
            card.TokenizationData = "77D2FFAEE984E740AC487E1E5A8E6726";
            Transaction response = card.Authorize(10m, true)
                        .WithCurrency("USD")
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
        public void Test_004_Credit_Refund_Paypal() {
            card = TestCards.Paypal();
            card.TokenizationData = "77D2FFAEE984E740AC487E1E5A8E6726";
            Transaction response = card.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_Credit_Balance_Inquiry_Paypal() {
            card = TestCards.Paypal();
            card.TokenizationData = "77D2FFAEE984E740AC487E1E5A8E6726";
            Transaction response = card.BalanceInquiry()
                        .Execute();
            Assert.IsNotNull(response);

            PriorMessageInformation pmi = response.MessageInformation;
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("313000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Sale_Reversal_Paypal() {
            card = TestCards.Paypal();
            card.TokenizationData = "77D2FFAEE984E740AC487E1E5A8E6726";

            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            response.NTSData = ntsData;
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("003000", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check response
            //Assert.AreEqual("000", response.getResponseCode());

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
        public void Test_015_Credit_Void_Paypal() {
            card = TestCards.Paypal();
            card.TokenizationData = "77D2FFAEE984E740AC487E1E5A8E6726";

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
