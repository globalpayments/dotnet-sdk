using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Tests.Network
{
    [TestClass]
    public class NWSCheckTests
    {
        private eCheck check;
        Address address;

        public NWSCheckTests() {
            AcceptorConfig acceptorConfig = new AcceptorConfig
            {
                // data code values
                CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe_KeyEntry,
                CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.PIN,
                CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.ByMerchant,
                TerminalOutputCapability = TerminalOutputCapability.Printing_Display,
                OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Unattended,

                // hardware software config values
                HardwareLevel = "34",
                SoftwareLevel = "21205710",

                // pos configuration values
                SupportsPartialApproval = true,
                SupportsShutOffAmount = true,
                SupportsReturnBalance = true,
                SupportsDiscoverNetworkReferenceId = true,
                SupportsAvsCnvVoidReferrals = true,
                SupportsEmvPin = true
            };

            // gateway config

            NetworkGatewayConfig config = new NetworkGatewayConfig(NetworkGatewayType.NWS)
            {
                ServiceUrl = "test.txns-c.secureexchange.net",
                PrimaryPort = 15031,
                SecondaryEndpoint = "test.txns-e.secureexchange.net",
                SecondaryPort = 15031,
                CompanyId = "SPSA",
                TerminalId = "NWSDOTNET01",
                //UniqueDeviceId = "0001",
                AcceptorConfig = acceptorConfig,
                EnableLogging = true,
                StanProvider = StanGenerator.GetInstance(),
                BatchProvider = BatchProvider.GetInstance()
            };

            ServicesContainer.ConfigureService(config);

            // with merchant type
            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "ICR");

            address = new Address
            {
                StreetAddress1 = "123 Main St.",
                City = "Downtown",
                State = "NJ",
                PostalCode = "12345"
            };

            //check = new eCheck { AccountNumber = "0273771700", AccountType = AccountType.CHECKING, AchVerify = false };
            check = TestChecks.Certification();

        }

        [TestMethod]
        public void Test_CheckGuarantee_FormattedMICR()
        {
            //check.DriversLicenseNumber = "";
            //check.DriversLicenseState = "";
            check.AccountNumber = "11111111";
            check.RoutingNumber = "111111111";


            check.CheckGuarantee = true;
            Transaction response = check.Authorize(101m, true)
                .WithCurrency("USD")
                .WithCheckCustomerId("0873629115")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("032000", pmi.ProcessingCode);
            Assert.AreEqual("100", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_CheckVerification_FormattedMICR() {
            check.AccountNumber = "11111111";
            check.RoutingNumber = "111111111";
            check.CheckVerify = true;

            Transaction response = check.Authorize(101m, true)
                .WithCurrency("USD")
                .WithCheckCustomerId("0873629115")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("042000", pmi.ProcessingCode);
            Assert.AreEqual("100", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_CheckGuarantee_FormattedMICR_Extended()
        {
            //check.DriversLicenseNumber = "";
            //check.DriversLicenseState = "";
            check.AccountNumber = "111111111111111";
            check.RoutingNumber = "111111111";
            check.CheckNumber = "11111";
            check.CheckGuarantee = true;

            Transaction response = check.Authorize(101m, true)
                .WithCurrency("USD")
                .WithCheckCustomerId("0873629115")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("032000", pmi.ProcessingCode);
            Assert.AreEqual("100", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_CheckVerification_FormattedMICR_Extended()
        {
            check.AccountNumber = "111111111111111";
            check.RoutingNumber = "111111111";
            check.CheckNumber = "11111";
            check.CheckVerify = true;

            Transaction response = check.Authorize(101m, true)
                .WithCurrency("USD")
                .WithCheckCustomerId("0873629115")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("042000", pmi.ProcessingCode);
            Assert.AreEqual("100", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_CheckGuarantee_RawMICR()
        {
            //check.DriversLicenseNumber = "";
            //check.DriversLicenseState = "";
            check.AccountNumber = "";
            check.RoutingNumber = "";
            check.CheckNumber = "";
            check.CheckGuarantee = true;

            Transaction response = check.Authorize(101m, true)
                .WithCurrency("USD")
                .WithCheckCustomerId("0873629115")
                .WithAddress(address)
                .WithRawMICRData("⑆111111111⑆11111111⑈00101")
                .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("032000", pmi.ProcessingCode);
            Assert.AreEqual("100", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_CheckVerification_RawMICR()
        {
            check.AccountNumber = "";
            check.RoutingNumber = "";
            check.CheckNumber = "";
            check.CheckVerify = true;

            Transaction response = check.Authorize(101m, true)
                .WithCurrency("USD")
                .WithCheckCustomerId("0873629115")
                .WithAddress(address)
                .WithRawMICRData("⑆111111111⑆11111111⑈00101")
                .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("042000", pmi.ProcessingCode);
            Assert.AreEqual("100", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }
    }
}
