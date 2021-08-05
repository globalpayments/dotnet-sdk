using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.Network {
    [TestClass]
    public class NWSEbtTests {

        private EBTTrackData cashCard;
        private EBTTrackData foodCard;

        public NWSEbtTests() {
            Address address = new Address {
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
                CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe_KeyEntry,
                CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.PIN,
                TerminalOutputCapability = TerminalOutputCapability.Printing_Display,
                CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.AuthorizingAgent,
                //OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Unattended,

                // hardware software config values
                HardwareLevel = "34",
                SoftwareLevel = "21205710",

                // pos configuration values
                SupportsPartialApproval = true,
                SupportsShutOffAmount = true,
                SupportsReturnBalance = true,
                SupportsDiscoverNetworkReferenceId = true,
                SupportsAvsCnvVoidReferrals = true
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
                MerchantType = "5541",
                AcceptorConfig = acceptorConfig,
                EnableLogging = true,
                StanProvider = StanGenerator.GetInstance(),
                BatchProvider = BatchProvider.GetInstance()
            };

            ServicesContainer.ConfigureService(config);

            // cash card
            cashCard = new EBTTrackData(EbtCardType.CashBenefit);
            cashCard.Value = "4355567063338=2012101HJNw/ewskBgnZqkL";
            cashCard.PinBlock = "62968D2481D231E1A504010024A00014";
            cashCard.EncryptionData = EncryptionData.Version2("/wECAQEEAoFGAgEH4gcOTDT6jRZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0m+/d4SO9TEshhRGUUQzVBrBvP/Os1qFx+6zdQp1ejjUCoDmzoUMbil9UG73zBxxTOy25f3Px0p8joyCh8PEWhADz1BkROJT3q6JnocQE49yYBHuFK0obm5kqUcYPfTY09vPOpmN+wp45gJY9PhkJF5XvPsMlcxX4/JhtCshegz4AYrcU/sFnI+nDwhy295BdOkVN1rn00jwCbRcE900kj3UsFfyc", "2");

            // Food card
            foodCard = new EBTTrackData(EbtCardType.FoodStamp);
            foodCard.Value = "4355567063338=2012101HJNw/ewskBgnZqkL";
            foodCard.PinBlock = "62968D2481D231E1A504010024A00014";
            foodCard.EncryptionData = EncryptionData.Version2("/wECAQEEAoFGAgEH4gcOTDT6jRZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0m+/d4SO9TEshhRGUUQzVBrBvP/Os1qFx+6zdQp1ejjUCoDmzoUMbil9UG73zBxxTOy25f3Px0p8joyCh8PEWhADz1BkROJT3q6JnocQE49yYBHuFK0obm5kqUcYPfTY09vPOpmN+wp45gJY9PhkJF5XvPsMlcxX4/JhtCshegz4AYrcU/sFnI+nDwhy295BdOkVN1rn00jwCbRcE900kj3UsFfyc", "2");
        }
        [TestMethod]
        public void Test_000_batch_close() {
            BatchSummary summary = BatchService.CloseBatch();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary.IsBalanced);
        }

        [TestMethod]
        public void Test_213_manual_sale() {
            EBTCardData ebtCard = new EBTCardData(EbtCardType.CashBenefit);
            ebtCard.Number = "4355560000033338";
            ebtCard.ExpMonth = 12;
            ebtCard.ExpYear = 2020;
            ebtCard.PinBlock = "62968D2481D231E1A504010024A00014";
            ebtCard.ReaderPresent = true;

            Transaction response = ebtCard.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("008100", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check result
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_214_manual_balance_inquiry() {
            try
            {
                EBTCardData ebtCard = new EBTCardData(EbtCardType.CashBenefit);
                ebtCard.Number = "4012002000060016";
                ebtCard.ExpMonth = 12;
                ebtCard.ExpYear = 2025;
                ebtCard.PinBlock = "32539F50C245A6A93D123412324000AA";

                ebtCard.BalanceInquiry(InquiryType.CASH)
                        .WithUniqueDeviceId("0001")
                        .Execute();
            }
            catch (BuilderException)
            {

            }
        }

        [TestMethod]
        public void Test_215_swipe_authorization() {
            try {
                cashCard.Authorize(10m)
                        .WithCurrency("USD")
                        .Execute();
            }
            catch(UnsupportedTransactionException ex) {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void Test_216_swipe_balance_inquiry() {
            Transaction response = cashCard.BalanceInquiry(InquiryType.CASH)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("318100", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            // check result
            Assert.AreEqual("000", response.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_217_swipe_sale() {
            Transaction response = cashCard.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("008100", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check result
            Assert.AreEqual("000", response.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_218_swipe_sale_surcharge() {
            Transaction response = cashCard.Charge(10m)
                    .WithCurrency("USD")
                    .WithFee(FeeType.Surcharge, 1m)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("008100", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check result
            Assert.AreEqual("000", response.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_219_swipe_sale_cash_back() {
            Transaction response = cashCard.Charge(10m)
                    .WithCurrency("USD")
                    .WithCashBack(5m)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("098100", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check result
            Assert.AreEqual("000", response.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_220_swipe_benefit_withdrawal() {
            Transaction response = cashCard.BenefitWithdrawal(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("018100", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check result
            Assert.AreEqual("000", response.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_221_swipe_refund() {
            try {
                cashCard.Refund(10m)
                        .WithCurrency("USD")
                        .Execute();
            }
            catch(Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex.Message);

            }
        }

        [TestMethod]
        public void Test_222_swipe_void() {
            //Transaction transaction = Transaction.FromNetwork(
            //        10m,
            //        "TYPE04",
            //        new NtsData(),
            //        cashCard,
            //        "1200",
            //        "000791",
            //        "181126125809"
            //);
            Transaction transaction = cashCard.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();

            Transaction response = transaction.Void()
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("008100", pmi.ProcessingCode);
            Assert.AreEqual("441", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);

            // check result
            Assert.AreEqual("400", response.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_223_swipe_reversal() {
            try {
                cashCard.Charge(10m)
                        .WithCurrency("USD")
                        .WithForceGatewayTimeout(true)
                        .Execute();
                Assert.Fail("Did not timeout.");
            }
            catch (GatewayTimeoutException exc) {
                Assert.AreEqual(1, exc.ReversalCount);
                Assert.AreEqual("400", exc.ReversalResponseCode);
            }
        }

        [TestMethod]
        public void Test_224_swipe_reversal_cashBack() {
            try {
                cashCard.Charge(13m)
                        .WithCurrency("USD")
                        .WithCashBack(3m)
                        .WithForceGatewayTimeout(true)
                        .Execute();
                Assert.Fail("Did not timeout.");
            }
            catch (GatewayTimeoutException exc) {
                Assert.AreEqual(1, exc.ReversalCount);
                Assert.AreEqual("400", exc.ReversalResponseCode);
            }
        }

        [TestMethod]
        public void Test_225_manual_foodStamp_sale() {
            EBTCardData ebtCard = new EBTCardData(EbtCardType.FoodStamp);
            ebtCard.Number = "4355560000033338";
            ebtCard.ExpMonth = 12;
            ebtCard.ExpYear = 2025;
            ebtCard.PinBlock = "62968D2481D231E1A504010024A00014";
            ebtCard.ReaderPresent = true;

            Transaction response = ebtCard.Charge(13m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("008000", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check result
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_226_manual_foodStamp_balance() {
            EBTCardData ebtCard = new EBTCardData(EbtCardType.FoodStamp);
            ebtCard.Number = "4012002000060016";
            ebtCard.ExpMonth = 12;
            ebtCard.ExpYear = 2025;
            ebtCard.PinBlock = "32539F50C245A6A93D123412324000AA";

            ebtCard.BalanceInquiry(InquiryType.FOODSTAMP)
                    .WithUniqueDeviceId("0001")
                    .Execute();
        }

        [TestMethod]
        public void Test_227_swipe_foodStamp_authorization() {
            foodCard.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute();
        }

        [TestMethod]
        public void Test_228_swipe_foodStamp_balance() {
            Transaction response = foodCard.BalanceInquiry(InquiryType.FOODSTAMP)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("318000", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            // check result
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_229_swipe_foodStamp_sale() {
            Transaction response = foodCard.Charge(100m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("008000", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check result
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_230_swipe_foodStamp_sale_cashBack() {
            try {
                foodCard.Charge(10m)
                        .WithCurrency("USD")
                        .WithCashBack(5m)
                        .Execute();
            }
            catch(BuilderException ex) {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void Test_231_swipe_foodStamp_return() {
            EBTTrackData track = new EBTTrackData(EbtCardType.FoodStamp);
            track.Value = ";4012002000060016=25121011803939600000?";
            track.PinBlock = "32539F50C245A6A93D123412324000AA";

            Transaction response = foodCard.Refund(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("200080", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);

            // check result
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_232_swipe_foodStamp_voice_capture() {
            Transaction transaction = Transaction.FromNetwork(
                    1m,
                    "TYPE04",
                    new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized, DebitAuthorizerCode.UnknownAuthorizer),
                    foodCard,
                    "1200",
                    "001853",
                    "008000"
            );
            //Transaction transaction = foodCard.Charge(1m)
            //        .WithCurrency("USD")
            //        .Execute();

            Transaction response = transaction.Capture()
                    .WithReferenceNumber("123456789012345")
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("008000", pmi.ProcessingCode);
            Assert.AreEqual("201", pmi.FunctionCode);
            Assert.AreEqual("1378", pmi.MessageReasonCode);

            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);

            // check result
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_233_swipe_foodStamp_void() {
            //Transaction transaction = Transaction.FromNetwork(
            //        10m,
            //        "TYPE04",
            //        new NtsData(),
            //        foodCard,
            //        "1200",
            //        "001857",
            //        "200604101259"
            //);
            Transaction transaction = foodCard.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();


            Transaction response = transaction.Void()
                    .WithReferenceNumber("015610000549")
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("008000", pmi.ProcessingCode);
            Assert.AreEqual("441", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check result
            Assert.AreEqual("400", response.ResponseCode);
        }

        [TestMethod]
        public void Test_234_swipe_foodStamp_reverse_sale() {
            try {
                foodCard.Charge(10m)
                        .WithCurrency("USD")
                        .WithForceGatewayTimeout(true)
                        .Execute();
                Assert.Fail("Did not timeout.");
            }
            catch (GatewayTimeoutException exc) {
                Assert.AreEqual(1, exc.ReversalCount);
                Assert.AreEqual("400", exc.ReversalResponseCode);
            }
        }

        [TestMethod]
        public void Test_235_swipe_foodStamp_return_reverse() {
            EBTTrackData track = new EBTTrackData(EbtCardType.FoodStamp);
            track.Value = ";4012002000060016=25121011803939600000?";
            track.PinBlock = "32539F50C245A6A93D123412324000AA";

            //Transaction transaction = Transaction.FromNetwork(
            //        100m,
            //        "TYPE04",
            //        new NtsData(),
            //        foodCard,
            //        "1200",
            //        "001286",
            //        "200520091722"
            //);
            Transaction transaction = foodCard.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();

            try {
                Transaction response = transaction.Refund(10m)
                        .WithCurrency("USD")
                        .WithForceGatewayTimeout(true)
                        .Execute();
                Assert.IsNotNull(response);
            }
            catch(GatewayTimeoutException exc) {
                Assert.AreEqual(1, exc.ReversalCount);
                Assert.AreEqual("400", exc.ReversalResponseCode);
            }
            
        }
    }
}
