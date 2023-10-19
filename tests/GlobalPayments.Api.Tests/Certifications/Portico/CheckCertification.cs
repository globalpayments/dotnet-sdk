using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Certifications.Portico
{
    [TestClass]
    public class CheckCertification
    {
        Address address;

        public CheckCertification()
        {
            ServicesContainer.ConfigureService(new PorticoConfig
            {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
            });

            address = new Address
            {
                StreetAddress1 = "123 Main St.",
                City = "Downtown",
                State = "NJ",
                PostalCode = "12345"
            };
        }

        #region ACH Debit - Consumer

        [TestMethod]
        public void checks_001ConsumerPersonalChecking()
        {
            var check = TestChecks.Certification(SecCode.PPD, CheckType.PERSONAL, AccountType.CHECKING);

            var response = check.Charge(11.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // test case 25
            var voidResponse = response.Void().Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void checks_002ConsumerBusinessChecking()
        {
            var check = TestChecks.Certification(SecCode.PPD, CheckType.BUSINESS, AccountType.CHECKING);
            var response = check.Charge(12.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void checks_003ConsumerPersonalSavings()
        {
            var check = TestChecks.Certification(SecCode.PPD, CheckType.PERSONAL, AccountType.SAVINGS);
            var response = check.Charge(13.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void checks_004ConsumerBusinessSavings()
        {
            var check = TestChecks.Certification(SecCode.PPD, CheckType.BUSINESS, AccountType.SAVINGS);
            var response = check.Charge(14.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void checks_005CorporatePersonalChecking()
        {
            var check = TestChecks.Certification(SecCode.CCD, CheckType.PERSONAL, AccountType.CHECKING, "Heartland Pays");
            var response = check.Charge(15.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // test case 26
            var voidResponse = response.Void().Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", response.ResponseCode);
        }

        #endregion

        #region ACH Debit - Corporate

        [TestMethod]
        public void checks_006CorporateBuisnessChecking()
        {
            var check = TestChecks.Certification(SecCode.CCD, CheckType.BUSINESS, AccountType.CHECKING, "Heartland Pays");
            var response = check.Charge(16.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void checks_007CorporatePersonalSavings()
        {
            var check = TestChecks.Certification(SecCode.CCD, CheckType.PERSONAL, AccountType.SAVINGS, "Heartland Pays");
            var response = check.Charge(17.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void checks_008CorporateBuisnessSavings()
        {
            var check = TestChecks.Certification(SecCode.CCD, CheckType.BUSINESS, AccountType.SAVINGS, "Heartland Pays");
            var response = check.Charge(18.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        #endregion

        #region eGold Checking Tests

        [TestMethod]
        public void checks_009EgoldPersonalChecking()
        {
            var check = TestChecks.Certification(SecCode.POP, CheckType.PERSONAL, AccountType.CHECKING);
            var response = check.Charge(11.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void checks_010EgoldBuisnessChecking()
        {
            var check = TestChecks.Certification(SecCode.POP, CheckType.BUSINESS, AccountType.CHECKING);
            var response = check.Charge(12.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // test case 27
            var voidResponse = response.Void().Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void checks_011EgoldPersonalSavings()
        {
            var check = TestChecks.Certification(SecCode.POP, CheckType.PERSONAL, AccountType.SAVINGS);
            var response = check.Charge(13.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void checks_012EgoldBusinessSavings()
        {
            var check = TestChecks.Certification(SecCode.POP, CheckType.BUSINESS, AccountType.SAVINGS);
            var response = check.Charge(14.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        #endregion

        #region eSilver 

        [TestMethod]
        public void checks_013EsilverPersonalChecking()
        {
            var check = TestChecks.Certification(SecCode.POP, CheckType.PERSONAL, AccountType.CHECKING);
            var response = check.Charge(15.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void checks_014EsilverBuisnessChecking()
        {
            var check = TestChecks.Certification(SecCode.POP, CheckType.BUSINESS, AccountType.CHECKING);
            var response = check.Charge(16.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // test case 28
            var voidResponse = response.Void().Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void checks_015EsilverPersonalSavings()
        {
            var check = TestChecks.Certification(SecCode.POP, CheckType.PERSONAL, AccountType.SAVINGS);
            var response = check.Charge(17.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void checks_016EsilverBuisnessSavings()
        {
            var check = TestChecks.Certification(SecCode.POP, CheckType.BUSINESS, AccountType.SAVINGS);
            var response = check.Charge(18.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        #endregion

        #region Bronze

        [TestMethod, Ignore]
        public void checks_017EbronzePersonalChecking()
        {
            var check = TestChecks.Certification(SecCode.EBRONZE, CheckType.PERSONAL, AccountType.CHECKING);
            check.CheckVerify = true;
            var response = check.Charge(19.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void checks_018EbronzePersonalChecking()
        {
            var check = TestChecks.Certification(SecCode.EBRONZE, CheckType.PERSONAL, AccountType.CHECKING);
            check.CheckVerify = true;
            var response = check.Charge(20.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void checks_019EbronzePersonalChecking()
        {
            var check = TestChecks.Certification(SecCode.EBRONZE, CheckType.PERSONAL, AccountType.SAVINGS);
            check.CheckVerify = true;
            var response = check.Charge(21.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void checks_020EbronzeBusinessSavings()
        {
            var check = TestChecks.Certification(SecCode.EBRONZE, CheckType.BUSINESS, AccountType.SAVINGS);
            check.CheckVerify = true;
            var response = check.Charge(22.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        #endregion

        #region Checks-by-Web

        [TestMethod]
        public void checks_021WebPersonalChecking()
        {
            var check = TestChecks.Certification(SecCode.WEB, CheckType.PERSONAL, AccountType.CHECKING);
            var response = check.Charge(23.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void checks_022WebBuisnessChecking()
        {
            var check = TestChecks.Certification(SecCode.WEB, CheckType.BUSINESS, AccountType.CHECKING);
            var response = check.Charge(24.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void checks_023WebPersonalSavings()
        {
            var check = TestChecks.Certification(SecCode.WEB, CheckType.PERSONAL, AccountType.SAVINGS);
            var response = check.Charge(25.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // test case 29
            var voidResponse = response.Void().Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void checks_024WebBusinessSavings()
        {
            var check = TestChecks.Certification(SecCode.WEB, CheckType.BUSINESS, AccountType.SAVINGS);
            var response = check.Charge(5.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        #endregion

        #region Check Void
        // this is done inline
        #endregion
    }
}
