using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.TestData;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.Network;
using GlobalPayments.Api.Utils;
using System;

namespace GlobalPayments.Api.Tests.Network
{
    [TestClass]
    public class NWSAdministrativeTests
    {
        AdminService _adminService;
        private CreditCardData card;
        private CreditTrackData track;
        private CreditCardData readylinkCard;
        private DebitTrackData readylinkTrack;

        public NWSAdministrativeTests()
        {
            AcceptorConfig acceptorConfig = new AcceptorConfig
            {
                // data code values
                CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe_KeyEntry,
                CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.None,
                CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.ByMerchant,
                TerminalOutputCapability = TerminalOutputCapability.Printing_Display,
                //OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Unattended,
                //
                // hardware software config values
                HardwareLevel = "34",
                SoftwareLevel = "21205710",

                // pos configuration values
                SupportsPartialApproval = true,
                SupportsShutOffAmount = true,
                SupportsReturnBalance = true,
                SupportsDiscoverNetworkReferenceId = true,
                SupportsAvsCnvVoidReferrals = true,
                SupportsEmvPin = true,

                // DE 43-34 Message Data
                EchoSettlementData = true
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
                UniqueDeviceId = "0001",
                AcceptorConfig = acceptorConfig,
                EnableLogging = true,
                StanProvider = StanGenerator.GetInstance(),
                BatchProvider = BatchProvider.GetInstance()
            };

            _adminService = new AdminService(config);

            ServicesContainer.ConfigureService(config);

            // with merchant type
            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "ICR");

            // VISA
            card = TestCards.VisaManual(true, true);
            track = TestCards.VisaSwipe();
        }

        [TestMethod]
        public void Test_000_batch_close()
        {
            BatchSummary summary = BatchService.CloseBatch(BatchCloseType.Forced);
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary.IsBalanced);
        }

        [TestMethod]
        public void Test_001_site_configuration_message()
        {
            var record = new RecordDataEntry()
            {
                MessageVersion = "001",
                TransactionDate = DateTime.Now.ToString("yyMMdd"),
                TransactionTime = DateTime.Now.ToString("hhmmss"),
                CompanyName = "ShaunCorp",
                HeartlandCompanyId = "SPSA",
                MerchantName = "Test",
                MerchantId = "NWSDOTNET01",
                MerchantStreet = "123 Main St",
                MerchantCity = "North Olmsted",
                MerchantState = "OH",
                MerchantZIP = "44070",
                MerchantPhoneNumber = "123-456-7890",
                SiteBrand = "",
                MerchantType = "5542",
                POSApplicationType = "S",
                OperationMethod = "A",
                POSVendor = "VERIFONE",
                POSModel = "OMNI 3750",
                POSTerminalType = "32",
                SoftwareVersion = "53",
                POSSpecification = "P",
                POSSpecificationVersion = "1.1",
                PaymentEngine = "N",
                PaymentVertical = "C",
                HardwareVersion = "3750",
                POSSoftwareVersion = "05050012",
                FirmwareLevel = "Q50015A2",
                MiddlewareVendor = "",
                MiddlewareModel = "",
                MiddlewareType = "",
                MiddlewareSoftwareVersion = "",
                ReceiptPrinterType = "I",
                ReceiptPrinterModel = "",
                JournalPrinterType = "S",
                JournalPrinterModel = "",
                MultiLaneDeviceType = "P",
                MultiLaneDeviceVendor = "VERIFONE",
                MultiLaneDeviceModel = "SE1000",
                InsideKeyManagementScheme = "D",
                InsidePINEncryption = "T",
                OutsidePEDType = "",
                OutsidePEDVendor = "",
                OutsidePEDModel = "",
                OutsideKeyManagementScheme = "",
                OutsidePINEncryption = "",
                CheckReaderVendor = "VIVOTECH",
                CheckReaderModel = "VIVOpay 4000",
                InsideContactlessReaderType = "I",
                InsideContactlessReaderVendor = "",
                InsideContactlessReaderModel = "",
                OutsideContactlessReaderType = "I",
                OutsideContactlessReaderVendor = "",
                OutsideContactlessReaderModel = "",
                CommunicationMedia = "I",
                CommunicationProtocol = "T",
                BroadbandUse = "S",
                DataWireAccess = "A",
                MicroNodeModelNumber = "",
                MicroNodeSoftwareVersion = "",
                RouterType = "E",
                RouterVendor = "LINKSYS",
                RouterProductModel = "BEFSR41",
                ModemPhoneNumber = "",
                PrimaryDialNumberOrIPAddress = "vf1.datawire.net",
                SecondaryDialNumberOrIPAddress = "800-239-9089",
                DispenserInterfaceVendor = "",
                DispenserInterfaceModel = "",
                DispenserInterfaceSoftwareVersion = "",
                DispenserVendor = "",
                DispenserModel = "",
                DispenserSoftwareVersion = "",
                DispenserQuantity = "",
                NumberOfScannersOrPeripherals = "",
                Scanner1Vendor = "",
                Scanner1Model = "",
                Scanner1SoftwareVersion = "",
                Peripheral2Vendor = "",
                Peripheral2Model = "",
                Peripheral2SoftwareVersion = "",
                Peripheral3Vendor = "",
                Peripheral3Model = "",
                Peripheral3SoftwareVersion = "",
                Peripheral4Vendor = "",
                Peripheral4Model = "",
                Peripheral4SoftwareVersion = "",
                Peripheral5Vendor = "",
                Peripheral5Model = "",
                Peripheral5SoftwareVersion = ""
            };

            Transaction response = _adminService.POSSiteConfig(record).Execute();

            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);

            // check response
            Assert.AreEqual("600", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_time_request_message()
        {
            
            Transaction response = _adminService.TimeRequest().Execute();

            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.IsNotNull(response.TimeResponseFromHeartland);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }
    }
}
