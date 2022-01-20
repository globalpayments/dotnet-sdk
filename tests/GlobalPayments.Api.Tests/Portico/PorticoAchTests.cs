using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests {
    [TestClass]
    public class PorticoAchTests
    {
        eCheck check;
        Address address;

        [TestInitialize]
        public void Init() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
            });

            check = new eCheck {
                AccountNumber = "24413815",
                RoutingNumber = "490000018",
                CheckType = CheckType.PERSONAL,
                SecCode = SecCode.PPD,
                AccountType = AccountType.CHECKING,
                EntryMode = EntryMethod.Manual,
                CheckHolderName = "John Doe",
                DriversLicenseNumber = "09876543210",
                DriversLicenseState = "TX",
                PhoneNumber = "8003214567",
                BirthYear = 1997,
                SsnLast4 = "4321"
            };

            address = new Address {
                StreetAddress1 = "123 Main St.",
                City = "Downtown",
                State = "NJ",
                PostalCode = "12345"
            };
        }

        [TestMethod, Ignore]
        public void CheckSaleWithSingleUseToken() {
            var check = new eCheck {
                Token = "supt_bUNE7MtGwN0hdDcElNijZ83A",
                CheckType = CheckType.PERSONAL,
                SecCode = SecCode.PPD,
                AccountType = AccountType.CHECKING,
                EntryMode = EntryMethod.Manual,
                CheckHolderName = "John Doe",
                DriversLicenseNumber = "09876543210",
                DriversLicenseState = "TX",
                PhoneNumber = "8003214567",
                BirthYear = 1997,
                SsnLast4 = "4321"
            };

            var response = check.Charge(11m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void CheckSale() {
            var response = check.Charge(11m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void CheckVoidFromTransactionId() {
            var response = check.Charge(10.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var voidResponse = Transaction.FromId(response.TransactionId, PaymentMethodType.ACH)
                .Void()
                .Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void CheckNewCryptoUrl() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
            });

            var response = check.Charge(10.00m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void SupportTestCase() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                    SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A",
                    Environment = Environment.TEST,
                    RequestLogger = new RequestFileLogger(@"C:\temp\portico\requestlog.txt")
                }
            );

            var check = new eCheck {
                AccountNumber = "24413815",
                RoutingNumber = "490000018",
                AccountType = AccountType.CHECKING,
                CheckType = CheckType.PERSONAL,
                EntryMode = EntryMethod.Manual,
                SecCode = SecCode.PPD,
                CheckHolderName = "John Doe"
            };

            var address = new Address {
                StreetAddress1 = "123 Main St.",
                City = "Downtown",
                State = "NJ",
                PostalCode = "12345"
            };

            var response = check.Charge(10.00m)
               .WithCurrency("USD")
               .WithAddress(address)
               .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
    }
}
