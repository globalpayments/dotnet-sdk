using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.PAX;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.Pax {
    [TestClass]
    public class PaxReportTests {
        static IDeviceInterface _device;
        static string authCode;
        static string transactionNumber;
        static string referenceNumber;
        static string searchText = string.Empty;

        [ClassInitialize]
        public static void Initialize(TestContext context) {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.PAX_DEVICE,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "10.12.220.172",
                //IpAddress = "192.168.0.31",
                Port = "10009",
                Timeout = 30000,
                RequestIdProvider = new RandomIdProvider()
            });
            Assert.IsNotNull(_device);

            _device.OnMessageSent += (message) => {
                Assert.IsTrue(message.Contains(searchText), message);
            };

            var card = new CreditCardData {
                Number = "4005554444444460",
                ExpMonth = 12,
                ExpYear = 20,
                Cvn = "123"
            };

            var address = new Address {
                StreetAddress1 = "1 Heartland Way",
                PostalCode = "95124"
            };

            var response = _device.Sale(11m)
                .WithAllowDuplicates(true)
                .WithPaymentMethod(card)
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            authCode = response.AuthorizationCode;
            transactionNumber = response.TerminalRefNumber;
            referenceNumber = response.ReferenceNumber;
        }

        [TestInitialize]
        public void TestInit() { searchText = string.Empty; }

        [TestMethod]
        public void LocalDetailReport_RecordNumber() {
            var report = _device.LocalDetailReport()
                .Where(PaxSearchCriteria.RecordNumber, 0)
                .Execute() as LocalDetailReport;
            Assert.IsNotNull(report);
            Assert.AreEqual(0, report.RecordNumber);
        }

        [TestMethod]
        public void LocalDetailReport_ReferenceNumber() {
            var report = _device.LocalDetailReport()
                    .Where(PaxSearchCriteria.ReferenceNumber, referenceNumber)
                    .Execute() as LocalDetailReport;
            Assert.IsNotNull(report);
            Assert.AreEqual(referenceNumber, report.ReferenceNumber);
        }

        [TestMethod]
        public void LocalDetailReport_TerminalReferenceNumber() {
            var report = _device.LocalDetailReport()
                    .Where(PaxSearchCriteria.TerminalReferenceNumber, transactionNumber)
                    .Execute() as LocalDetailReport;
            Assert.IsNotNull(report);
            Assert.AreEqual(transactionNumber, report.TerminalRefNumber);
        }

        [TestMethod]
        public void LocalDetailReport_TransactionType() {
            var report = _device.LocalDetailReport()
                    .Where(PaxSearchCriteria.TransactionType, TerminalTransactionType.SALE)
                    .Execute() as LocalDetailReport;
            Assert.IsNotNull(report);
            Assert.AreEqual("SALE", report.TransactionType);
        }

        [TestMethod]
        public void LocalDetailReport_AuthCode() {
            var report = _device.LocalDetailReport()
                    .Where(PaxSearchCriteria.AuthCode, authCode)
                    .Execute() as LocalDetailReport;
            Assert.IsNotNull(report);
            Assert.AreEqual(authCode, report.AuthorizationCode);
        }

        [TestMethod]
        public void LocalDetailReport_CardType() {
            var report = _device.LocalDetailReport()
                    .Where(PaxSearchCriteria.CardType, TerminalCardType.VISA)
                    .Execute() as LocalDetailReport;
            Assert.IsNotNull(report);
            Assert.AreEqual("VISA", report.CardType);
        }

        [TestMethod]
        public void LocalDetailReport_MerchantId() {
            searchText = "MM_ID=12345";

            var report = _device.LocalDetailReport()
                    .Where(PaxSearchCriteria.MerchantId, 12345)
                    .Execute() as LocalDetailReport;
            Assert.IsNotNull(report);
        }

        [TestMethod]
        public void LocalDetailReport_MerchantName() {
            searchText = "MM_NAME=CAS";

            var report = _device.LocalDetailReport()
                    .Where(PaxSearchCriteria.MerchantName, "CAS")
                    .Execute() as LocalDetailReport;
            Assert.IsNotNull(report);
        }
    }
}
