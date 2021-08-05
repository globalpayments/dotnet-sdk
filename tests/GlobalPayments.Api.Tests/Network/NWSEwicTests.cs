using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Elements;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Tests.Network
{
    [TestClass]
    public class NWSEwicTests
    {
        private EwicTrackData track;
        private EwicCardData card;
        private EWICData eWICData;
        public NWSEwicTests() {
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
                EWICMerchantID = "123456789",
                AcceptorConfig = acceptorConfig,
                EnableLogging = true,
                StanProvider = StanGenerator.GetInstance(),
                BatchProvider = BatchProvider.GetInstance()
            };

            ServicesContainer.ConfigureService(config);

            track = new EwicTrackData {
                Value = "6103189999903663=49121200001234",
                PinBlock = "12348D2481D231E1A504010024A00014",
                EntryMethod = EntryMethod.Swipe,
                //EncryptionData = EncryptionData.Version2("/wECAQEEAoFGAgEH4gcOTDT6jRZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0m+/d4SO9TEshhRGUUQzVBrBvP/Os1qFx+6zdQp1ejjUCoDmzoUMbil9UG73zBxxTOy25f3Px0p8joyCh8PEWhADz1BkROJT3q6JnocQE49yYBHuFK0obm5kqUcYPfTY09vPOpmN+wp45gJY9PhkJF5XvPsMlcxX4/JhtCshegz4AYrcU/sFnI+nDwhy295BdOkVN1rn00jwCbRcE900kj3UsFfyc", "2")
            };

            card = new EwicCardData
            {
                Number = "6103189999903663",
                ExpMonth = 12,
                ExpYear = 2049,
                PinBlock = "12348D2481D231E1A504010024A00014"
            };

            eWICData = new EWICData();
            DE117_WIC_Data_Field_EA eaData = new DE117_WIC_Data_Field_EA()
            {
                CategoryCode = "2",
                SubCategoryCode = "2",
                BenefitQuantity = "500"
            };
            //eWICData.Add(eaData);

            DE117_WIC_Data_Field_PS psData = new DE117_WIC_Data_Field_PS()
            {
                UPCData = "11110583000",
                ItemPrice = "100",
                PurchaseQuantity = "1500"
            };
            eWICData.Add(psData);
        }

        [TestMethod]
        public void Test_001_swipe_balance_inquiry() {

            Transaction response = track.BalanceInquiry()
                //.WithEWICData(eWICData)
                .WithEWICIssuingEntity("1122334455")
                .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("319700", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            // check result
            Assert.AreEqual("000", response.ResponseCode);

        }

        [TestMethod]
        public void Test_003_swipe_sale() {
            //eWICData = new EWICData();
            //DE117_WIC_Data_Field_PS psData = new DE117_WIC_Data_Field_PS() {
            //    UPCData = "123456789012",
            //    ItemPrice = "285",
            //    PurchaseQuantity = "100",
            //    ItemActionCode = "12"
            //};
            //eWICData.Add(psData);
            Transaction response = track.Charge(10m)
                .WithEWICData(eWICData)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);

            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("009700", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check result
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_008_swipe_merchant_initiated_void() {
            eWICData = new EWICData();
            DE117_WIC_Data_Field_PS psData = new DE117_WIC_Data_Field_PS()
            {
                UPCData = "123456789012",
                ItemPrice = "285",
                PurchaseQuantity = "100",
                ItemActionCode = "12"
            };
            eWICData.Add(psData);
            Transaction sale = track.Charge(10m)
                .WithEWICData(eWICData)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(sale);
            System.Diagnostics.Debug.WriteLine("Sale STAN:");
            System.Diagnostics.Debug.WriteLine(sale.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(sale.SystemTraceAuditNumber);

            // check result
            //Assert.AreEqual("000", response.ResponseCode);

            Transaction voidResponse = sale.Void().Execute();
            Assert.IsNotNull(voidResponse);
            System.Diagnostics.Debug.WriteLine("Void STAN:");
            System.Diagnostics.Debug.WriteLine(voidResponse.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(voidResponse.SystemTraceAuditNumber);

            // check message data
            PriorMessageInformation pmi = voidResponse.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("009700", pmi.ProcessingCode);
            Assert.AreEqual("441", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);

            // check response
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_009_swipe_customer_initiated_void()
        {
            eWICData = new EWICData();
            DE117_WIC_Data_Field_PS psData = new DE117_WIC_Data_Field_PS()
            {
                UPCData = "123456789012",
                ItemPrice = "285",
                PurchaseQuantity = "100",
                ItemActionCode = "12"
            };
            eWICData.Add(psData);
            Transaction sale = track.Charge(10m)
                .WithEWICData(eWICData)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(sale);
            System.Diagnostics.Debug.WriteLine("Sale STAN:");
            System.Diagnostics.Debug.WriteLine(sale.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(sale.SystemTraceAuditNumber);

            // check result
            //Assert.AreEqual("000", response.ResponseCode);

            Transaction voidResponse = sale.Void()
                .WithCustomerInitiated(true)
                .Execute();
            Assert.IsNotNull(voidResponse);
            System.Diagnostics.Debug.WriteLine("Void STAN:");
            System.Diagnostics.Debug.WriteLine(voidResponse.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(voidResponse.SystemTraceAuditNumber);

            // check message data
            PriorMessageInformation pmi = voidResponse.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("009700", pmi.ProcessingCode);
            Assert.AreEqual("441", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);

            // check response
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_010_manual_balance_inquiry()
        {
            eWICData = new EWICData();
            DE117_WIC_Data_Field_EA eaData = new DE117_WIC_Data_Field_EA()
            {
                CategoryCode = "2",
                SubCategoryCode = "2",
                BenefitQuantity = "500"
            };
            eWICData.Add(eaData);
            Transaction response = card.BalanceInquiry()
                //.WithEWICData(eWICData)
                .WithEWICIssuingEntity("1122334455")
                .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("319700", pmi.ProcessingCode);
            Assert.AreEqual("108", pmi.FunctionCode);

            // check result
            Assert.AreEqual("000", response.ResponseCode);

        }

        [TestMethod]
        public void Test_012_manual_sale()
        {
            eWICData = new EWICData();
            DE117_WIC_Data_Field_PS psData = new DE117_WIC_Data_Field_PS()
            {
                UPCData = "123456789012",
                ItemPrice = "285",
                PurchaseQuantity = "100",
                ItemActionCode = "12"
            };
            eWICData.Add(psData);
            Transaction response = card.Charge(10m)
                .WithEWICData(eWICData)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);

            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("009700", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check result
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_017_manual_merchant_initiated_void()
        {
            eWICData = new EWICData();
            DE117_WIC_Data_Field_PS psData = new DE117_WIC_Data_Field_PS()
            {
                UPCData = "123456789012",
                ItemPrice = "285",
                PurchaseQuantity = "100",
                ItemActionCode = "12"
            };
            eWICData.Add(psData);
            Transaction sale = card.Charge(10m)
                .WithEWICData(eWICData)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(sale);
            System.Diagnostics.Debug.WriteLine("Sale STAN:");
            System.Diagnostics.Debug.WriteLine(sale.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(sale.SystemTraceAuditNumber);

            // check result
            //Assert.AreEqual("000", response.ResponseCode);

            Transaction voidResponse = sale.Void().Execute();
            Assert.IsNotNull(voidResponse);
            System.Diagnostics.Debug.WriteLine("Void STAN:");
            System.Diagnostics.Debug.WriteLine(voidResponse.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(voidResponse.SystemTraceAuditNumber);

            // check message data
            PriorMessageInformation pmi = voidResponse.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("009700", pmi.ProcessingCode);
            Assert.AreEqual("441", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);

            // check response
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_018_manual_customer_initiated_void()
        {
            eWICData = new EWICData();
            DE117_WIC_Data_Field_PS psData = new DE117_WIC_Data_Field_PS()
            {
                UPCData = "123456789012",
                ItemPrice = "285",
                PurchaseQuantity = "100",
                ItemActionCode = "12"
            };
            eWICData.Add(psData);
            Transaction sale = card.Charge(10m)
                .WithEWICData(eWICData)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(sale);
            System.Diagnostics.Debug.WriteLine("Sale STAN:");
            System.Diagnostics.Debug.WriteLine(sale.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(sale.SystemTraceAuditNumber);

            // check result
            //Assert.AreEqual("000", response.ResponseCode);

            Transaction voidResponse = sale.Void()
                .WithCustomerInitiated(true)
                .Execute();
            Assert.IsNotNull(voidResponse);
            System.Diagnostics.Debug.WriteLine("Void STAN:");
            System.Diagnostics.Debug.WriteLine(voidResponse.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(voidResponse.SystemTraceAuditNumber);

            // check message data
            PriorMessageInformation pmi = voidResponse.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("009700", pmi.ProcessingCode);
            Assert.AreEqual("441", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);

            // check response
            Assert.AreEqual("400", voidResponse.ResponseCode);
        }
    }
}
