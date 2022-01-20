using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Realex {
    [TestClass]
    public class GpEcomRecurringTests {
        Customer new_customer;

        public string CustomerId {
            get {
                return string.Format("{0}-Realex", DateTime.Now.ToString("yyyyMMdd"));
            }
        }
        public string PaymentId(string type) {
            return string.Format("{0}-Realex-{1}", DateTime.Now.ToString("yyyyMMdd"), type);
        }

        public GpEcomRecurringTests() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                RefundPassword = "refund",
                SharedSecret = "secret"
            },"test");

            new_customer = new Customer {
                Key = CustomerId,
                Title = "Mr.",
                FirstName = "James",
                LastName = "Mason",
                Company = "Realex Payments",
                Address = new Address {
                    StreetAddress1 = "Flat 123",
                    StreetAddress2 = "House 456",
                    StreetAddress3 = "The Cul-De-Sac",
                    City = "Halifax",
                    Province = "West Yorkshire",
                    PostalCode = "W6 9HR",
                    Country = "United Kingdom"
                },
                HomePhone = "+35312345678",
                WorkPhone = "+3531987654321",
                Fax = "+124546871258",
                MobilePhone = "+25544778544",
                Email = "text@example.com",
                Comments = "Campaign Ref E7373G"
            };

            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 5,
                ExpYear = DateTime.Now.AddYears(2).Year,
                CardHolderName = "James Mason"
            };
        }

        [TestMethod]
        public void Test_001a_CreateCustomer() {
            try {
                var customer = new_customer.Create("test");
                Assert.IsNotNull(customer);
            }
            catch (GatewayException exc) {
                // check for already created
                if (exc.ResponseCode != "501")
                    throw;
            }
        }

        [TestMethod]
        public void Test_001b_CreatePaymentMethod() {
            try {
                var paymentMethod = new_customer.AddPaymentMethod(PaymentId("Credit"), new CreditCardData {
                    Number = "4263970000005262",
                    ExpMonth = 5,
                    ExpYear = DateTime.Now.AddYears(2).Year,
                    CardHolderName = "James Mason"
                }).Create("test");
                Assert.IsNotNull(paymentMethod);
            }
            catch (GatewayException exc) {
                // check for already created
                if(exc.ResponseCode != "520")
                    throw;
            }
        }

        [TestMethod]
        public void Test_002a_EditCustomer() {
            var customer = new Customer { Key = CustomerId };
            customer.FirstName = "Perry";
            customer.SaveChanges("test");
        }

        [TestMethod]
        public void Test_002b_EditPaymentMethod() {
            var paymentMethod = new RecurringPaymentMethod(CustomerId, PaymentId("Credit"));
            paymentMethod.PaymentMethod = new CreditCardData {
                Number = "5425230000004415",
                ExpMonth = 10,
                ExpYear = DateTime.Now.AddYears(2).Year,
                CardHolderName = "Philip Marlowe"
            };
            paymentMethod.SaveChanges("test");
        }

        [TestMethod]
        public void Test_002c_EditPaymentMethodExpOnly() {
            var paymentMethod = new RecurringPaymentMethod(CustomerId, PaymentId("Credit"));
            paymentMethod.PaymentMethod = new CreditCardData {
                CardType = "MC",
                ExpMonth = 10,
                ExpYear = DateTime.Now.AddYears(2).Year,
                CardHolderName = "Philip Marlowe"
            };
            paymentMethod.SaveChanges("test");
        }

        [TestMethod, ExpectedException(typeof(UnsupportedTransactionException))]
        public void Test_003_FindOnRealex() {
            Customer.Find(CustomerId, "test");
        }

        [TestMethod]
        public void Test_004a_ChargeStoredCard() {
            var paymentMethod = new RecurringPaymentMethod(CustomerId, PaymentId("Credit"));
            var response = paymentMethod.Charge(10m)
                .WithCurrency("USD")
                .WithCvn("123")
                .Execute("test");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_004b_VerifyStoredCard() {
            var paymentMethod = new RecurringPaymentMethod(CustomerId, PaymentId("Credit"));
            var response = paymentMethod.Verify()
                .WithCvn("123")
                .Execute("test");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_004c_RefundStoredCard() {
            var paymentMethod = new RecurringPaymentMethod(CustomerId, PaymentId("Credit"));
            var response = paymentMethod.Refund(10.01m)
                .WithCurrency("USD")
                .Execute("test");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_RecurringPayment() {
            var paymentMethod = new RecurringPaymentMethod(CustomerId, PaymentId("Credit"));
            var response = paymentMethod.Charge(12m)
                .WithRecurringInfo(RecurringType.Fixed, RecurringSequence.First)
                .WithCurrency("USD")
                .Execute("test");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_006_DeletePaymentMethod() {
            var paymentMethod = new RecurringPaymentMethod(CustomerId, PaymentId("Credit"));
            paymentMethod.Delete(configName:"test");
        }

        [TestMethod]
        public void Test_007_ChargeStoredCard_from_different_configs()
        {

            ServicesContainer.ConfigureService(new GpEcomConfig
            {
                MerchantId = "heartlandgpsandbox",
                AccountId = "3dsecure",
                RefundPassword = "refund",
                SharedSecret = "secret",
                RequestLogger = new RequestConsoleLogger()
            });

            var paymentMethod = new_customer.AddPaymentMethod(PaymentId("Credit"), new CreditCardData
            {
                Number = "4263970000005262",
                ExpMonth = 5,
                ExpYear = DateTime.Now.AddYears(2).Year,
                CardHolderName = "James Mason"
            }).Create("test");
            Assert.IsNotNull(paymentMethod);
            var response = paymentMethod.Charge(12m)
                .WithRecurringInfo(RecurringType.Fixed, RecurringSequence.First)
                .WithCurrency("USD")
                .Execute("test");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var pm = new RecurringPaymentMethod(CustomerId, PaymentId("Credit"));
            pm.Delete(configName: "test");

            var paymentMethod2 = new_customer.AddPaymentMethod(PaymentId("Credit"), new CreditCardData
            {
                Number = "4263970000005262",
                ExpMonth = 5,
                ExpYear = DateTime.Now.AddYears(2).Year,
                CardHolderName = "James Mason 2"
            }).Create();
            Assert.IsNotNull(paymentMethod2);
            var response2 = paymentMethod2.Charge(12m)
                .WithRecurringInfo(RecurringType.Fixed, RecurringSequence.First)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response2);
            Assert.AreEqual("00", response2.ResponseCode);

            var pm2 = new RecurringPaymentMethod(CustomerId, PaymentId("Credit"));
            pm2.Delete();
        }
    }
}
