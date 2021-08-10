using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests {
    [TestClass]
    public class CountryUtilTests {
        [TestMethod]
        public void GetCountryCodeExact() {
            string result = CountryUtils.GetCountryCodeByCountry("Ireland");
            Assert.IsNotNull(result);
            Assert.AreEqual("IE", result);
        }

        [TestMethod]
        public void GetCountryCodeMisspelled() {
            string result = CountryUtils.GetCountryCodeByCountry("Afganistan");
            Assert.IsNotNull(result);
            Assert.AreEqual("AF", result);
        }

        [TestMethod]
        public void GetCountryCodeFromPartial() {
            string result = CountryUtils.GetCountryCodeByCountry("Republic of Congo");
            Assert.IsNotNull(result);
            Assert.AreEqual("CD", result);
        }

        [TestMethod]
        public void GetCountryCodeByExactCode() {
            string result = CountryUtils.GetCountryCodeByCountry("IE");
            Assert.IsNotNull(result);
            Assert.AreEqual("IE", result);
        }

        [TestMethod]
        public void GetCountryCodeByPartialCode() {
            string result = CountryUtils.GetCountryCodeByCountry("USA");
            Assert.IsNotNull(result);
            Assert.AreEqual("US", result);
        }

        [TestMethod]
        public void GetCountryCodeNullDoesNotError() {
            CountryUtils.GetCountryCodeByCountry(null);
        }

        [TestMethod]
        public void GetCountryCodeFakeCountry() {
            string result = CountryUtils.GetCountryCodeByCountry("FakeCountry");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetCountryCodeFakeCountry2() {
            string result = CountryUtils.GetCountryCodeByCountry("Fakeistan");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetCountryCodeFakeCountry3() {
            string result = CountryUtils.GetCountryCodeByCountry("MyRussia");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetCountryByCodeExact() {
            string result = CountryUtils.GetCountryByCode("IE");
            Assert.IsNotNull(result);
            Assert.AreEqual("Ireland", result);
        }

        [TestMethod]
        public void GetCountryByThreeDigitCode() {
            string result = CountryUtils.GetCountryByCode("USA");
            Assert.IsNotNull(result);
            Assert.AreEqual("United States of America", result);
        }

        [TestMethod]
        public void GetCountryCodeByExactNumericCode() {
            string result = CountryUtils.GetCountryCodeByCountry("840");
            Assert.IsNotNull(result);
            Assert.AreEqual("US", result);
        }

        [TestMethod]
        public void GetNumericCodeByTwoDigitCode() {
            string result = CountryUtils.GetNumericCodeByCountry("US");
            Assert.IsNotNull(result);
            Assert.AreEqual("840", result);
        }

        [TestMethod]
        public void GetNumericCodeByThreeDigitCode() {
            string result = CountryUtils.GetNumericCodeByCountry("USA");
            Assert.IsNotNull(result);
            Assert.AreEqual("840", result);
        }

        [TestMethod]
        public void GetNumericCodeByCountryName() {
            string result = CountryUtils.GetNumericCodeByCountry("United States of America");
            Assert.IsNotNull(result);
            Assert.AreEqual("840", result);
        }

        [TestMethod]
        public void GetNumericCodeByNumericCode() {
            string result = CountryUtils.GetNumericCodeByCountry("840");
            Assert.IsNotNull(result);
            Assert.AreEqual("840", result);
        }

        [TestMethod]
        public void GetNetherlandsAntillesNumericCodeByCountryCode()
        {
            string result = CountryUtils.GetNumericCodeByCountry("AN");
            Assert.IsNotNull(result);
            Assert.AreEqual("530", result);
        }

        [TestMethod]
        public void GetNetherlandsAntillesCountryCodeByCountryName()
        {
            string result = CountryUtils.GetCountryCodeByCountry("Netherlands Antilles");
            Assert.IsNotNull(result);
            Assert.AreEqual("AN", result);
        }

        [TestMethod]
        public void GetNumericCodeByNonExistingCountryName() {
            string result = CountryUtils.GetNumericCodeByCountry("Fake Country Name");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetCountryByCodeNullDoesNotError() {
            CountryUtils.GetCountryCodeByCountry(null);
        }

        [TestMethod]
        public void CheckAddressCodeFromCountryExact() {
            Address address = new Address();
            address.Country = "United States of America";
            Assert.IsNotNull(address.CountryCode);
            Assert.AreEqual("US", address.CountryCode);
        }

        [TestMethod]
        public void CheckAddressCountryFromCodeExact() {
            Address address = new Address();
            address.CountryCode = "US";
            Assert.IsNotNull(address.Country);
            Assert.IsNotNull("United States of America", address.Country);
        }

        [TestMethod]
        public void CheckAddressCodeFromCountryFuzzy() {
            Address address = new Address();
            address.Country = "Afganistan";
            Assert.IsNotNull(address.CountryCode);
            Assert.AreEqual("AF", address.CountryCode);
        }

        [TestMethod]
        public void CheckAddressCountryFromCodeFuzzy() {
            Address address = new Address();
            address.CountryCode = "USA";
            Assert.IsNotNull(address.Country);
            Assert.IsNotNull("United States of America", address.Country);
        }

        [TestMethod]
        public void AddressIsCountryExactMatch() {
            Address address = new Address();
            address.Country = "United States of America";
            Assert.IsTrue(address.IsCountry("US"));
        }

        [TestMethod]
        public void CheckAddressCodeFromNumericCodeExact() {
            Address address = new Address();
            address.Country = "056";
            Assert.IsNotNull(address.CountryCode);
            Assert.AreEqual("BE", address.CountryCode);
        }

        [TestMethod]
        public void AddressIsCountryExactMisMatch() {
            Address address = new Address();
            address.Country = "United States of America";
            Assert.IsFalse(address.IsCountry("GB"));
        }

        [TestMethod]
        public void AddressIsCountryFuzzyMatch() {
            Address address = new Address();
            address.Country = "Afganistan";
            Assert.IsTrue(address.IsCountry("AF"));
        }

        [TestMethod]
        public void AddressIsCountryFuzzyMisMatch() {
            Address address = new Address();
            address.Country = "Afganistan";
            Assert.IsFalse(address.IsCountry("GB"));
        }

        [TestMethod]
        public void CountryIsGB_NoStreetAddress1() {
            Address address = new Address {
                Country = "GB",
                PostalCode = "E77 4Qj"
            };
            Assert.IsNotNull(address.CountryCode);
            Assert.IsTrue(address.IsCountry("GB"));

            var card = new CreditCardData {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123",
                CardHolderName = "Joe Smith"
            };

            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RebatePassword = "rebate",
                RefundPassword = "refund"
            });

            var response = card.Charge(10m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
    }
}
