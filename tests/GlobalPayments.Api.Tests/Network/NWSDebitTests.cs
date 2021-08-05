using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Tests.Network {
    [TestClass]
    public class NWSDebitTests {
        private DebitTrackData track;
        public NWSDebitTests() {
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
                TerminalOutputCapability = TerminalOutputCapability.Printing_Display,
                CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.PIN,

                //CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.AuthorizingAgent,
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
            
            track = new DebitTrackData
            {
                Value = "4355567063338=2012101HJNw/ewskBgnZqkL",
                PinBlock = "62968D2481D231E1A504010024A00014",
                EncryptionData = EncryptionData.Version2("/wECAQEEAoFGAgEH4gcOTDT6jRZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0m+/d4SO9TEshhRGUUQzVBrBvP/Os1qFx+6zdQp1ejjUCoDmzoUMbil9UG73zBxxTOy25f3Px0p8joyCh8PEWhADz1BkROJT3q6JnocQE49yYBHuFK0obm5kqUcYPfTY09vPOpmN+wp45gJY9PhkJF5XvPsMlcxX4/JhtCshegz4AYrcU/sFnI+nDwhy295BdOkVN1rn00jwCbRcE900kj3UsFfyc", "2")
            };
        }

        [TestMethod]
        public void Test_000_batch_close() {
            BatchSummary summary = BatchService.CloseBatch(BatchCloseType.Forced);
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary.IsBalanced);
        }

        [TestMethod]
        public void WFRC_Response_Tests_Auth()
        {
            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.ResponseMessage);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void WFRC_Response_Tests_Sale()
        {
            Transaction response = track.Charge(10m)
                        .WithCurrency("USD")
                        .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.ResponseMessage);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_149_pre_authorization() {
            Transaction response = track.Authorize(1m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000800", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_150_sale() {
            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000800", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_151_sale_with_surCharge() {
            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .WithFee(FeeType.Surcharge, 2m)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000800", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_152_sale_with_cashBack() {
            Transaction response = track.Charge(10m)
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
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_153_refund() {
            Transaction response = track.Refund(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("200008", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_154_void() {
            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            Transaction voidResponse = response.Void().Execute();

            // check message data
            PriorMessageInformation pmi = voidResponse.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000800", pmi.ProcessingCode);
            Assert.AreEqual("441", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);

            // check response
            Assert.AreEqual("400", voidResponse.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_155_reverse_authorization() {
            try {
                track.Authorize(10m)
                        .WithCurrency("USD")
                        .WithForceGatewayTimeout(true)
                        .Execute();
                Assert.Fail("Did not throw a timeout");
            }
            catch (GatewayTimeoutException exc) {
                Assert.AreEqual(1, exc.ReversalCount);
                Assert.AreEqual("400", exc.ReversalResponseCode);
            }
        }

        [TestMethod]
        public void Test_156_reverse_sale() {
            try {
                track.Charge(10m)
                        .WithCurrency("USD")
                        .WithForceGatewayTimeout(true)
                        .Execute();
                Assert.Fail("Did not throw a timeout");
            }
            catch (GatewayTimeoutException exc) {
                Assert.AreEqual(1, exc.ReversalCount);
                Assert.AreEqual("400", exc.ReversalResponseCode);
            }
        }

        [TestMethod]
        public void Test_157_reverse_sale_cashBack() {
            try {
                track.Charge(10m)
                        .WithCurrency("USD")
                        .WithCashBack(3m)
                        .WithForceGatewayTimeout(true)
                        .Execute();
                Assert.Fail("Did not throw a timeout");
            }
            catch (GatewayTimeoutException exc) {
                Assert.AreEqual(1, exc.ReversalCount);
                Assert.AreEqual("400", exc.ReversalResponseCode);
            }
        }

        [TestMethod]
        public void Test_158_refund_reverse() {
            try {
                track.Refund(10m)
                        .WithCurrency("USD")
                        .WithForceGatewayTimeout(true)
                        .Execute();
                Assert.Fail("Did not throw a timeout");
            }
            catch (GatewayTimeoutException exc) {
                Assert.AreEqual(1, exc.ReversalCount);
                Assert.AreEqual("400", exc.ReversalResponseCode);
            }
        }

        [TestMethod]
        public void Test_159_ICR_authorization() {
            Transaction response = track.Authorize(10m)
                    .WithCurrency("USD")
                    .Execute("ICR");
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000800", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);

            // test case 160
            Transaction capture = response.Capture(response.AuthorizedAmount)
                    .Execute("ICR");
            Assert.IsNotNull(capture);
            // check message data
            pmi = capture.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000800", pmi.ProcessingCode);
            Assert.AreEqual("201", pmi.FunctionCode);
            Assert.AreEqual("1376", pmi.MessageReasonCode);

            // check response
            Assert.AreEqual("000", capture.ResponseCode);
            System.Diagnostics.Debug.WriteLine(capture.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(capture.SystemTraceAuditNumber);
        }

        [TestMethod]
        public void Test_161_ICR_reverse_authorization() {
            //try {
            //    track.Authorize(10m)
            //            .WithCurrency("USD")
            //            .WithForceGatewayTimeout(true)
            //            .Execute("ICR");
            //    Assert.Fail("Did not throw a timeout");
            //}
            //catch (GatewayTimeoutException exc) {
            //    Assert.AreEqual(1, exc.ReversalCount);
            //    Assert.AreEqual("400", exc.ReversalResponseCode);
            //}

            Transaction response = track.Authorize(10m, true)
                        .WithCurrency("USD")
                        .Execute("ICR");
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000800", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);

            Transaction reversal = response.Reverse(10m)
                    .WithCurrency("USD")
                    .Execute("ICR");
            Assert.IsNotNull(reversal);

            // check message data
            pmi = reversal.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000800", pmi.ProcessingCode);
            Assert.AreEqual("400", pmi.FunctionCode);
            Assert.AreEqual("4021", pmi.MessageReasonCode);
            System.Diagnostics.Debug.WriteLine(reversal.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(reversal.SystemTraceAuditNumber);
            // check response
            //Assert.AreEqual("400", reversal.ResponseCode);
        }

        [TestMethod]
        public void Test_162_ICR_partial_authorization() {
            Transaction response = track.Authorize(110m, true)
                    .WithCurrency("USD")
                    .Execute("ICR");
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000800", pmi.ProcessingCode);
            Assert.AreEqual("101", pmi.FunctionCode);

            // check response
            Assert.AreEqual("002", response.ResponseCode);
            Assert.IsNotNull(response.AuthorizedAmount);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);

            // test case 163
            Transaction capture = response.Capture(response.AuthorizedAmount)
                    .Execute("ICR");
            Assert.IsNotNull(capture);

            // check message data
            pmi = capture.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000800", pmi.ProcessingCode);
            Assert.AreEqual("202", pmi.FunctionCode);
            Assert.AreEqual("1376", pmi.MessageReasonCode);

            // check response
            Assert.AreEqual("000", capture.ResponseCode);
            System.Diagnostics.Debug.WriteLine(capture.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(capture.SystemTraceAuditNumber);
        }

        //[TestMethod]
        //public void Test_164_ICR_void_authorization() {

        //}

        [TestMethod]
        public void ReadyLinkLoad() {
            track = new DebitTrackData {
                Value = ";4110651122223331=21121010000012345678?"
            };

            Transaction response = track.AddValue(50m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("600008", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        }

        //[TestMethod]
        //public void Test_164_ICR_auth_reversal() {
        //    Transaction response = track.Authorize(1m, true)
        //            .WithCurrency("USD")
        //            .Execute("ICR");
        //    Assert.IsNotNull(response);

        //    // check message data
        //    PriorMessageInformation pmi = response.MessageInformation;
        //    Assert.IsNotNull(pmi);
        //    Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
        //    Assert.AreEqual("000800", pmi.ProcessingCode);
        //    Assert.AreEqual("101", pmi.FunctionCode);

        //    // check response
        //    //Assert.AreEqual("000", response.ResponseCode);

        //    Transaction reversal = response.Reverse(1m)
        //            .WithCurrency("USD")
        //            .Execute("ICR");
        //    Assert.IsNotNull(reversal);

        //    // check message data
        //    pmi = reversal.MessageInformation;
        //    Assert.IsNotNull(pmi);
        //    Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
        //    Assert.AreEqual("000800", pmi.ProcessingCode);
        //    Assert.AreEqual("400", pmi.FunctionCode);
        //    Assert.AreEqual("4021", pmi.MessageReasonCode);

        //    // check response
        //    Assert.AreEqual("400", reversal.ResponseCode);
        //    System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
        //    System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
        //}

        //[TestMethod]
        //public void Test_165_emv_debit_sale() {
        //    DebitTrackData track = new DebitTrackData {
        //        Value = ";4024720012345671=18125025432198712345?",
        //        PinBlock = "AFEC374574FC90623D010000116001EE"
        //    };

        //    Transaction response = track.Charge(10m)
        //            .WithCurrency("USD")
        //            .WithTagData("82021C008407A0000002771010950580000000009A031709289C01005F280201245F2A0201245F3401019F02060000000010009F03060000000000009F080200019F090200019F100706010A03A420009F1A0201249F26089CC473F4A4CE18D39F2701809F3303E0F8C89F34030100029F3501229F360200639F370435EFED379F410400000019")
        //            .Execute();
        //    Assert.IsNotNull(response);
        //    Assert.AreEqual("000", response.ResponseCode);
        //}
    }
}
