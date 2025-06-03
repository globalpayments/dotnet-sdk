using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiCreateInstallmentTest : BaseGpApiTests {
        private Installment installment;
        private CreditCardData visaCard;
        private CreditCardData masterCard;
        [TestInitialize]
        public void TestInitialize() {

            ServicesContainer.RemoveConfig();
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            gpApiConfig.AppId = "bcTDtE6wV2iCfWPqXv0FMpU86YDqvTnc";
            gpApiConfig.AppKey = "jdf2vlLCA13A3Fsz";
            gpApiConfig.ServiceUrl = "https://apis-sit.globalpay.com/ucp";
            gpApiConfig.Country = "MX";
            gpApiConfig.AccessTokenInfo = new AccessTokenInfo
            {
                TransactionProcessingAccountName = "IPP_Processing"
            };
            ServicesContainer.ConfigureService(gpApiConfig);

            installment = new Installment
            {
                Channel = "CNP",
                Amount = 11,
                Country = "MX",
                Currency = "MXN",
                Program = "SIP",
                AccountName = "transaction_processing",
                Reference = "becf9f3e-4d33-459c-8ed2-0c4affc95"
            };

            installment.EntryMode = "ECOM";

            visaCard = new CreditCardData
            {
                Number = "4213168058314147",
                ExpMonth = 07,
                ExpYear = 2027,
                Cvn = "123",
                CardPresent = false,
                ReaderPresent = false
            };

            masterCard = new CreditCardData
            {
                Number = "5546259023665054",
                ExpMonth = 05,
                ExpYear = 2025,
                Cvn = "123",
                CardPresent = false,
                ReaderPresent = false
            };
        }

        [TestMethod]
        public void CreateInstallment_WithMasterCardAndValidProgramSIP() {

            installment.CardDetails = masterCard;

            Installment response = installment.Create();

            Assert.IsNotNull(response);
            Assert.AreEqual(installment.Program,response.Program);
            Assert.AreEqual("APPROVAL", response.Message);
            Assert.IsNotNull(response.AuthCode);
            Assert.AreEqual("00", response.Result);
            Assert.AreEqual("SUCCESS", response.Action.ResultCode);
        }

        [TestMethod]
        public void CreateInstallment_WithMasterCardAndValidProgramMIPP() {
            installment.Program = "mIPP";
            installment.CardDetails = masterCard;

            Installment response = installment.Create();

            Assert.IsNotNull(response);
            Assert.AreEqual(installment.Program, response.Program);
            Assert.AreEqual("APPROVAL", response.Message);
            Assert.IsNotNull(response.AuthCode);
            Assert.AreEqual("00", response.Result);
            Assert.AreEqual("SUCCESS", response.Action.ResultCode);
        }

        [TestMethod]
        public void CreateInstallment_WithVisaAndValidProgramSIP() {

            installment.CardDetails = visaCard;

            Installment response = installment.Create();

            Assert.IsNotNull(response);
            Assert.AreEqual(installment.Program, response.Program);
            Assert.AreEqual("APPROVAL", response.Message);
            Assert.AreEqual("00", response.Result);
            Assert.IsNotNull(response.AuthCode);
            Assert.AreEqual("SUCCESS", response.Action.ResultCode);
        }

        [TestMethod]
        public void CreateInstallment_WithVisaAndValidProgramMIPP() {
            installment.Program = "mIPP";
            installment.CardDetails = visaCard;

            Installment response = installment.Create();

            Assert.IsNotNull(response);
            Assert.AreEqual(installment.Program, response.Program);
            Assert.AreEqual("APPROVAL", response.Message);
            Assert.AreEqual("00", response.Result);
            Assert.IsNotNull(response.AuthCode);
            Assert.AreEqual("SUCCESS", response.Action.ResultCode);
        }

        [TestMethod]
        public void CreateInstallment_WithExpiredVisaCardAndValidProgramMIPP() {
            installment.Program = "mIPP";
            installment.CardDetails = new CreditCardData
            {
                Number = "4213168058314147",
                ExpMonth = 07,
                ExpYear = 2022,
                Cvn = "123",
                CardPresent = false,
                ReaderPresent = false
            };

            Installment response = installment.Create();

            Assert.IsNotNull(response);
            Assert.AreEqual("54", response.Result);
            Assert.AreEqual("EXPIRED CARD", response.Message);
            Assert.AreEqual("DECLINED", response.Action.ResultCode);
        }

        [TestMethod]
        public void CreateInstallment_WithExpiredMasterCardAndValidProgramMIPP() {
            installment.Program = "mIPP";
            installment.CardDetails = new CreditCardData
            {
                Number = "5546259023665054",
                ExpMonth = 05,
                ExpYear = 2021,
                Cvn = "123",
                CardPresent = false,
                ReaderPresent = false
            };

            Installment response = installment.Create();

            Assert.IsNotNull(response);
            Assert.AreEqual("54", response.Result);
            Assert.AreEqual("EXPIRED CARD", response.Message);
            Assert.AreEqual("DECLINED", response.Action.ResultCode);
        }

        [TestMethod]
        public void CreateInstallment_WithVisaAndInvalidProgram() {
            installment.CardDetails = visaCard;
            installment.Program = "InCorrectProgram";
            try
            {
                Installment response = installment.Create();
            }
            catch (GatewayException Ex)
            {
                Assert.AreEqual(Ex.ResponseCode, "INVALID_REQUEST_DATA");
                Assert.AreEqual(Ex.ResponseMessage, "40213");
                Assert.AreEqual(Ex.Message, "Status Code: BadRequest - program contains unexpected data");
            }
        }

        [TestMethod]
        public void CreateInstallment_WithMasterCardAndInvalidProgram() {
            installment.CardDetails = visaCard;
            installment.Program = "InCorrectProgram";
            try
            {
                Installment response = installment.Create();
            }
            catch (GatewayException Ex)
            {
                Assert.AreEqual(Ex.ResponseCode, "INVALID_REQUEST_DATA");
                Assert.AreEqual(Ex.ResponseMessage, "40213");
                Assert.AreEqual(Ex.Message, "Status Code: BadRequest - program contains unexpected data");
            }
        }
    }
}
