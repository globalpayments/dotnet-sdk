using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Network {
    [TestClass]
    public class NWSPinDebitCertificationTests {
        private DebitTrackData debit;
        private NetworkGatewayConfig config;

        public NWSPinDebitCertificationTests() {
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

            // with merchant type
            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "ICR");

            // debit card
            debit = new DebitTrackData();
            debit.Value = ";4761739001010135=241220119559045?";
            debit.PinBlock = "62968D2481D231E1A504010024A00014";
            debit.EntryMethod = EntryMethod.Swipe;
        }

        #region Pin Debit Approval Transaction
        [TestMethod]
        public void Test_Debit_Purchase() {
            Transaction response = debit.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Purchase_With_CashBack() {
            debit.EntryMethod = EntryMethod.Proximity;
            Transaction response = debit.Charge(10)
                    .WithCurrency("USD")
                    .WithCashBack(3m)
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("090800", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Return() {
            Transaction response = debit.Refund(10m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
       
        [TestMethod]
        public void Test_Debit_Auth() {
            Transaction response = debit.Authorize(10m, true)
                    .WithCurrency("USD")
                    .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Auth_Capture() {
            debit.EntryMethod = EntryMethod.Proximity;
            Transaction response = debit.Authorize(10m)
                    .WithCurrency("USD")
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute("ICR");
            Assert.IsNotNull(response);
            //Assert.AreEqual("000", response.ResponseCode);
            
            Transaction captureResponse = response.Capture()
                    .WithCurrency("USD")
                    .WithAuthenticatioNMethod(CardHolderAuthenticationMethod.PIN)
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute("ICR");
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_AuthCancellation() {
            Transaction response = debit.Authorize(10m, true)
                    .WithCurrency("USD")
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute("ICR");
            Assert.IsNotNull(response);
            //Assert.AreEqual("000", response.ResponseCode);

            Transaction voidResponse = response.Void()
                    .WithCurrency("USD")
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .WithAuthenticatioNMethod(CardHolderAuthenticationMethod.PIN)
                    .Execute("ICR");
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_PurchaseVoid() {
            Transaction response = debit.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            //Assert.AreEqual("000", response.ResponseCode);

            Transaction voidResponse = response.Void()
                   .WithCurrency("USD")
                   .WithAuthenticatioNMethod(CardHolderAuthenticationMethod.PIN)
                   .Execute("ICR");
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_PurchaseReverse() {
            debit.EntryMethod = EntryMethod.Proximity;
            Transaction response = debit.Charge(10m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute();
            Assert.IsNotNull(response);
            //Assert.AreEqual("000", response.ResponseCode);

            Transaction reverseResponse = response.Reverse()
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .WithAuthenticatioNMethod(CardHolderAuthenticationMethod.PIN)
                .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Purchase_With_CashBackReverse() {
            Transaction response = debit.Charge(10)
                    .WithCurrency("USD")
                    .WithCashBack(3m)
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute();
            Assert.IsNotNull(response);
            //Assert.AreEqual("000", response.ResponseCode);

            //// check message data
            //PriorMessageInformation pmi = response.MessageInformation;
            //Assert.IsNotNull(pmi);
            //Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            //Assert.AreEqual("090800", pmi.ProcessingCode);
            //Assert.AreEqual("200", pmi.FunctionCode);

            Transaction reverseResponse = response.Reverse()
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .WithAuthenticatioNMethod(CardHolderAuthenticationMethod.PIN)
                .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_ReturnReverse() {
            Transaction response = debit.Refund(10m)
                .WithCurrency("USD")
                .Execute("ICR");
            Assert.IsNotNull(response);
            //Assert.AreEqual("000", response.ResponseCode);

            Transaction reverseResponse = response.Reverse()
                .WithAuthenticatioNMethod(CardHolderAuthenticationMethod.PIN)
                .Execute("ICR");
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }

        #endregion
        
        #region Pin Debit Incorrect Pin Transaction
        [TestMethod]
        public void Test_Debit_PurchaseIncorretPin() {
            Transaction response = debit.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Purchase_With_CashBackIncorretPin() {
            debit.EntryMethod = EntryMethod.Proximity;
            Transaction response = debit.Charge(10)
                    .WithCurrency("USD")
                    .WithCashBack(3m)
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("090800", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_ReturnIncorretPin() {
            Transaction response = debit.Refund(10m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_AuthIncorretPin() {
            Transaction response = debit.Authorize(10m, true)
                    .WithCurrency("USD")
                    .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        #endregion

        #region Pin Debit Pin Tries Exceeded
        [TestMethod]
        public void Test_Debit_Purchase_PinTriesExceeded(){
            Transaction response = debit.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Purchase_With_CashBack_PinTriesExceeded() {
            debit.EntryMethod = EntryMethod.Proximity;
            Transaction response = debit.Charge(10)
                    .WithCurrency("USD")
                    .WithCashBack(3m)
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("090800", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Return_PinTriesExceeded() {
            Transaction response = debit.Refund(10m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Auth_PinTriesExceeded(){
            Transaction response = debit.Authorize(10m, true)
                    .WithCurrency("USD")
                    .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        #endregion

        #region Pin Debit Denial
        [TestMethod]
        public void Test_Debit_Purchase_Denial() {
            Transaction response = debit.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Purchase_With_CashBack_Denial() {
            debit.EntryMethod = EntryMethod.Proximity;
            Transaction response = debit.Charge(10)
                    .WithCurrency("USD")
                    .WithCashBack(3m)
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("090800", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Return_Denial() {
            Transaction response = debit.Refund(10m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Auth_Denial(){
            Transaction response = debit.Authorize(10m, true)
                    .WithCurrency("USD")
                    .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        #endregion

        #region Pin Debit Pick Up Card
        [TestMethod]
        public void Test_Debit_PickUpCard() {
            Transaction response = debit.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Purchase_With_CashBack_PickUpCard() {
            debit.EntryMethod = EntryMethod.Proximity;
            Transaction response = debit.Charge(10)
                    .WithCurrency("USD")
                    .WithCashBack(3m)
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("090800", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Return_PickUpCard() {
            Transaction response = debit.Refund(10m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Auth_PickUpCard() {
            Transaction response = debit.Authorize(10m, true)
                    .WithCurrency("USD")
                    .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        #endregion

        #region Pin Debit Host System Failure
        [TestMethod]
        public void Test_Debit_Purchase_HostSystemFailure() {
            Transaction response = debit.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Purchase_With_CashBack_HostSystemFailure() {
            debit.EntryMethod = EntryMethod.Proximity;
            Transaction response = debit.Charge(10)
                    .WithCurrency("USD")
                    .WithCashBack(3m)
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("090800", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Return_HostSystemFailure() {
            Transaction response = debit.Refund(10m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Auth_HostSystemFailure() {
            Transaction response = debit.Authorize(10m, true)
                    .WithCurrency("USD")
                    .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        #endregion
    }
}
