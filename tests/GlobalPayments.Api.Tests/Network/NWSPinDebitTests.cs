using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Network {
    [TestClass]
    public class NWSPinDebitTests {
        private DebitTrackData debit;
        private NetworkGatewayConfig config;

        public NWSPinDebitTests() {
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
                TerminalId = "NWSDOTNET01",//"NWSBATCH2002",//"NWSDOTNET01",
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

            // forced timeout
            config.ForceGatewayTimeout = true;
            ServicesContainer.ConfigureService(config, "timeout");

            // debit card
            debit = new DebitTrackData();
            debit.Value = ";4761739001010135=241220119559045?";
            debit.PinBlock = "62968D2481D231E1A504010024A00014";
            debit.EntryMethod = EntryMethod.Proximity;
        }

        #region Pin Debit Transaction
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
            Transaction response = debit.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture()
                    .WithCurrency("USD")
                    .Execute("ICR");
            Assert.IsNotNull(captureResponse);

            // check response
            Assert.AreEqual("000", captureResponse.ResponseCode);
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
        public void Test_Debit_Sale() {
            Transaction response = debit.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Sale_With_Refund() {
            Transaction response = debit.Charge(10)
                    .WithCurrency("USD")
                    .WithCashBack(3m)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
            
            Transaction refundResponse = response.Refund(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(refundResponse);
            Assert.AreEqual("000", refundResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Refund() {
            Transaction response = debit.Refund(10m)
                .WithCurrency("USD")
                .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Swipe_Void() {
            Transaction response = debit.Charge(10m)
                    .WithCurrency("USD")
                    .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction voidResponse = response.Void()
                    .WithCustomerInitiated(true)
                    .Execute("ICR");
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Sale_Void() {
            Transaction response = debit.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            Transaction voidResponse = response.Void().Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Sale_Reversal() {
            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);

            Transaction response = debit.Charge(10)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            
            response.NTSData = ntsData;
            
            Transaction reverseResponse = response.Reverse().Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Sale_ReversalWithCashback() {
            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);

            Transaction response = debit.Charge(10)
                    .WithCurrency("USD")
                    .WithCashBack(2)
                    .Execute();
            Assert.IsNotNull(response);

            response.NTSData = ntsData;

            Transaction reverseResponse = response.Reverse().Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }
        #endregion

        #region Pin Debit Transaction With EMV
        [TestMethod]
        public void Test_Debit_AuthWithEMV() {
            Transaction response = debit.Authorize(10m, true)
                    .WithCurrency("USD")
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Auth_CaptureWithEMV() {
            Transaction response = debit.Authorize(10m)
                    .WithCurrency("USD")
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute("ICR");
            Assert.IsNotNull(response);

            Transaction captureResponse = response.Capture()
                    .WithCurrency("USD")
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute("ICR");
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Sale_With_CashBackWithEMV() {
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
        public void Test_Debit_SaleWithEMV() {
            Transaction response = debit.Charge(10m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_RefundWithEMV() {
            Transaction response = debit.Refund(10m)
                .WithCurrency("USD")
                .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Swipe_VoidWithEMV() {
            Transaction response = debit.Charge(10m)
                    .WithCurrency("USD")
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute("ICR");
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction voidResponse = response.Void()
                    .WithCustomerInitiated(true)
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute("ICR");
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Sale_VoidWithEMV() {
            Transaction response = debit.Charge(10m)
                    .WithCurrency("USD")
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute();
            Assert.IsNotNull(response);

            Transaction voidResponse = response.Void().Execute("ICR");
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_Debit_Sale_ReversalWithEMV() {
            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);

            Transaction response = debit.Charge(10)
                    .WithCurrency("USD")
                    .WithTagData("5F280208409F080200024F07A0000001523010500A4D617374657243617264820238008407A00000000310108E0A00000000000000001F00950500008080009A031901099B02E8009C01405F24032212315F25030401015F2A0208405F300202015F3401009F01060000000000019F02060000000006009F03060000000000009F0607A00000015230109F0702FF009F090200029F0D05B8508000009F0E0500000000009F0F05B8708098009F10080105A000030000009F120A4D6173746572436172649F160F3132333435363738393031323334359F1A0208409F1C0831313232333334349F1E0831323334353637389F21030710109F26080631450565A30B759F2701809F330360F0C89F34033F00019F3501219F360200049F3704C6B1A04F9F3901059F4005F000A0B0019F4104000000869F4C0865C862608A23945A9F4E0D54657374204D65726368616E74")
                    .Execute();
            Assert.IsNotNull(response);

            response.NTSData = ntsData;

            Transaction reverseResponse = response.Reverse().Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }
        #endregion
    }
}
