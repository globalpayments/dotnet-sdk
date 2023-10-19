using System;
using System.Diagnostics;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Certifications.Portico
{
    [TestClass]
    public class EcomCheckCertification
    {
        Address address;

        public EcomCheckCertification()
        {
            ServicesContainer.ConfigureService(new PorticoConfig
            {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
            });

            address = new Address
            {
                StreetAddress1 = "6860 Dallas Pkwy",
                City = "Dallas",
                State = "TX",
                PostalCode = "75024"
            };
        }

        [TestMethod]
        public void ecomm_001_PersonalChecking()
        {
            eCheck check = new eCheck
            {
                RoutingNumber = "490000018",
                AccountNumber = "24413815",
                AccountType = AccountType.CHECKING,
                CheckType = CheckType.PERSONAL,
                SecCode = SecCode.WEB,
                DriversLicenseNumber = "09876543210",
                DriversLicenseState = "NJ",
                CheckHolderName = "John Doe",
                PhoneNumber = "5558675309"
            };

            Transaction response = check.Charge(19m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            Debug.WriteLine("Test 1: " + response.TransactionId);
        }

        [TestMethod]
        public void ecomm_002_BusinessChecking()
        {
            eCheck check = new eCheck
            {
                RoutingNumber = "490000018",
                AccountNumber = "24413815",
                AccountType = AccountType.CHECKING,
                CheckType = CheckType.BUSINESS,
                SecCode = SecCode.WEB,
                DriversLicenseNumber = "09876543210",
                DriversLicenseState = "NJ",
                CheckHolderName = "Acme Unlimited",
                PhoneNumber = "5558675309"
            };

            Transaction response = check.Charge(20m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            Debug.WriteLine("Test 2: " + response.TransactionId);
        }

        [TestMethod]
        public void ecomm_003_PersonalSavings()
        {
            eCheck check = new eCheck
            {
                RoutingNumber = "490000018",
                AccountNumber = "24413815",
                AccountType = AccountType.SAVINGS,
                CheckType = CheckType.PERSONAL,
                SecCode = SecCode.WEB,
                DriversLicenseNumber = "09876543210",
                DriversLicenseState = "NJ",
                CheckHolderName = "John Doe",
                PhoneNumber = "5558675309"
            };

            Transaction response = check.Charge(21m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            Debug.WriteLine("Test 3: " + response.TransactionId);

            Transaction voidResponse = response.Void().Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
            Debug.WriteLine("Test 5: " + voidResponse.TransactionId);
        }

        [TestMethod]
        public void ecomm_004_BusinessSavings()
        {
            eCheck check = new eCheck
            {
                RoutingNumber = "490000018",
                AccountNumber = "24413815",
                AccountType = AccountType.SAVINGS,
                CheckType = CheckType.BUSINESS,
                SecCode = SecCode.WEB,
                DriversLicenseNumber = "09876543210",
                DriversLicenseState = "NJ",
                CheckHolderName = "Acme Unlimited",
                PhoneNumber = "5558675309"
            };

            Transaction response = check.Charge(22m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            Debug.WriteLine("Test 4: " + response.TransactionId);
        }
    }
}
