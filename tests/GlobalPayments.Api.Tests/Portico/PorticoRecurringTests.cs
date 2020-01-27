using System;
using System.Linq;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Portico {
    [TestClass]
    public class PorticoRecurringTests {
        //public static readonly string _seed = System.IO.Path.GetRandomFileName().Replace(".", "");
        public static readonly string _seed = "STATIC";

        public string CustomerId {
            get {
                return string.Format("{0}-GlobalApi", DateTime.Now.ToString("yyyyMMdd"), _seed);
            }
        }
        public string PaymentId(string type) {
            return string.Format("{0}-GlobalApi-{1}", DateTime.Now.ToString("yyyyMMdd"), type);
        }

        public PorticoRecurringTests() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
            });
        }

        [TestMethod]
        public void Test_000_CleanUp() {
            // Remove Schedules
            try {
                var schResults = Schedule.FindAll();
                foreach (var schedule in schResults) {
                    schedule.Delete(true);
                }
            }
            catch (ApiException exc) {
                Assert.IsNotNull(exc);
            }

            // Remove Payment Methods
            try {
                var pmResults = RecurringPaymentMethod.FindAll();
                foreach (var pm in pmResults) {
                    pm.Delete(true);
                }
            }
            catch (ApiException exc) {
                Assert.IsNotNull(exc);
            }

            // Remove Customers
            try {
                var custResults = Customer.FindAll();
                foreach (var c in custResults) {
                    c.Delete(true);
                }
            }
            catch (ApiException exc) {
                Assert.IsNotNull(exc);
            }
        }

        [TestMethod]
        public void Test_001a_CreateCustomer() {
            var cust = Customer.Find(CustomerId);
            if (cust != null)
                Assert.Inconclusive("Customer already exists.");

            var customer = new Customer {
                Id = CustomerId,
                Status = "Active",
                FirstName = "Bill",
                LastName = "Johnson",
                Company = "Heartland Payment Systems",
                Address = new Address {
                    StreetAddress1 = "987 Elm St",
                    City = "Princeton",
                    Province = "NJ",
                    PostalCode = "12345",
                    Country = "USA"
                },
                HomePhone = "9876543210",
                WorkPhone = "9876543210",
                Fax = "9876543210",
                MobilePhone = "9876543210",
                Email = "text@example.com"
            }.Create();
            Assert.IsNotNull(customer);
            Assert.IsNotNull(customer.Key);
        }

        [TestMethod]
        public void Test_001b_CreatePaymentMethod_Credit() {
            var customer = Customer.Find(CustomerId);
            Assert.IsNotNull(customer);

            var payment = customer.AddPaymentMethod(
                PaymentId("Credit"),
                new CreditCardData {
                    Number = "4111111111111111",
                    ExpMonth = 12,
                    ExpYear = 2025
                }).Create();
            Assert.IsNotNull(payment);
            Assert.IsNotNull(payment.Key);

            customer = Customer.Find(CustomerId);
            Assert.IsNotNull(customer);
            Assert.IsTrue(customer.PaymentMethods.Count > 0);

            Assert.IsNotNull(customer.PaymentMethods.First(p => p.Key == payment.Key));
        }

        [TestMethod, Ignore]
        public void Test_001c_CreatePaymentMethod_CreditEncrypted() {
            var customer = Customer.Find(CustomerId);
            Assert.IsNotNull(customer);

            var payment = customer.AddPaymentMethod(
                PaymentId("CreditEncrypted"),
                new CreditCardData {
                    Number = "+++++++AbcdNLNs0",
                    ExpMonth = 12,
                    ExpYear = 2025,
                    EncryptionData = EncryptionData.Version2("/cRAQECAoFGAgEH2wcOUedz2RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0EPrPpSLjzrwxIyMkh+IHZ1G2iathRcU+SHp6/lw+IyzMVGhhAjmQ3n9I6BK0YHL2OPMg6oVM4/Dj9W0DT/XNLSdl8y7nZew7ep+mbIAkBDQmagI2WIQCB1XOku0veyQ2Pf")
                });
            Assert.IsNotNull(payment);
            Assert.IsNotNull(payment.Key);
        }

        [TestMethod]
        public void Test_001d_CreatePaymentMethod_CreditTrack() {
            var customer = Customer.Find(CustomerId);
            Assert.IsNotNull(customer);

            var payment = customer.AddPaymentMethod(
                PaymentId("Track"),
                new CreditTrackData {
                    Value = "%B4012110000000011^TEST CUSTOMER^250510148888000000000074800000?;",
                    EntryMethod = EntryMethod.Swipe
                }).Create();
            Assert.IsNotNull(payment);
            Assert.IsNotNull(payment.Key);
        }

        [TestMethod, Ignore]
        public void Test_001e_CreatePaymentMethod_CreditTrackEncrypted() {
            var customer = Customer.Find(CustomerId);
            Assert.IsNotNull(customer);

            var payment = customer.AddPaymentMethod(
                PaymentId("CreditTrackEncrypted"),
                new CreditTrackData {
                    Value = "aAb1AB2k3nfxqbq+vcbEu0a6MVSZ9/0DXLhxroKUZfnrRtjVGjO9cS6se/wNCUkO5qX",
                    EntryMethod = EntryMethod.Swipe,
                    EncryptionData = EncryptionData.Version2("/aBCDEFGoABCdEF3wABCDE4jRZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0oJonV1aAbDFOsmdGObCs+OAlW5Zvw2Fx3YRkijfNHt2I7D8XwP4Uh0nSdfDhSVrTZXj5mX9frrKQoj0C7k+TunlfFgMN1aPa6fgYN/zHCsG3Zae4W798DP/Mi7vOVadze+EaUkX+H4ereXRPdCzYgqODWSEgse0O1fc6iJg3cvEAXo14FOLG/exampleiCThEpkrZtuvndLrI+O9ZY5UCQ1G3l9O")
                });
            Assert.IsNotNull(payment);
            Assert.IsNotNull(payment.Key);
        }

        [TestMethod, Ignore]
        public void Test_001f_CreatePaymentMethod_Token() {
            var token = new CreditCardData {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2025,
            }.Tokenize();

            var customer = Customer.Find(CustomerId);
            Assert.IsNotNull(customer);

            var payment = customer.AddPaymentMethod(
                PaymentId("Token"),
                new CreditCardData { Token = token }
            );
            Assert.IsNotNull(payment);
            Assert.IsNotNull(payment.Key);
        }

        [TestMethod]
        public void Test_001g_CreatePaymentMethod_ACH() {
            var customer = Customer.Find(CustomerId);
            Assert.IsNotNull(customer, "Customer does not exist.");

            var payment = customer.AddPaymentMethod(
                PaymentId("ACH"),
                new eCheck {
                    AccountNumber = "24413815",
                    RoutingNumber = "490000018",
                    CheckType = CheckType.PERSONAL,
                    SecCode = SecCode.PPD,
                    AccountType = AccountType.CHECKING,
                    DriversLicenseNumber = "7418529630",
                    DriversLicenseState = "TX",
                    BirthYear = 1989
                }).Create();
            Assert.IsNotNull(payment);
            Assert.IsNotNull(payment.Key);
        }

        [TestMethod]
        public void Test_001h_CreateSchedule_Credit() {
            var paymentMethod = RecurringPaymentMethod.Find(PaymentId("Credit"));
            Assert.IsNotNull(paymentMethod);

            var schedule = paymentMethod.AddSchedule(PaymentId("Credit"))
                .WithAmount(30.02m)
                .WithCurrency("USD")
                .WithStartDate(DateTime.Parse("02/01/2027"))
                .WithFrequency(ScheduleFrequency.WEEKLY)
                .WithStatus("Active")
                .WithReprocessingCount(2)
                .WithEndDate(DateTime.Parse("04/01/2027"))
                .Create();
            Assert.IsNotNull(schedule);
            Assert.IsNotNull(schedule.Key);
        }

        [TestMethod]
        public void Test_001i_CreateSchedule_ACH() {
            var paymentMethod = RecurringPaymentMethod.Find(PaymentId("ACH"));
            Assert.IsNotNull(paymentMethod, "Payment method does not exist.");

            var schedule = paymentMethod.AddSchedule(PaymentId("ACH"))
                .WithAmount(11m)
                .WithStartDate(DateTime.Now.AddDays(7))
                .WithFrequency(ScheduleFrequency.MONTHLY)
                .WithStatus("Active")
                .Create();
            Assert.IsNotNull(schedule);
            Assert.IsNotNull(schedule.Key);
        }

        [TestMethod]
        public void Test_002a_FindCustomer() {
            var customer = Customer.Find(CustomerId);
            Assert.IsNotNull(customer);
            Assert.IsNotNull(customer.Key);
        }

        [TestMethod]
        public void Test_002b_FindPaymentMethod() {
            var paymentMethod = RecurringPaymentMethod.Find(PaymentId("Credit"));
            Assert.IsNotNull(paymentMethod);
        }

        [TestMethod]
        public void Test_002c_FindSchedule() {
            var schedule = Schedule.Find(PaymentId("Credit"));
            Assert.IsNotNull(schedule);
        }

        [TestMethod]
        public void Test_002d_FindCustomerFindNullKey() {
            var customer = Customer.Find(null);
            Assert.IsNull(customer);
        }

        [TestMethod]
        public void Test_0023_FindCustomerFindInvalidKey() {
            var customer = Customer.Find("1");
            Assert.IsNull(customer);
        }

        [TestMethod]
        public void Test_003a_FindAllCustomers() {
            var customers = Customer.FindAll();
            Assert.IsNotNull(customers);
        }

        [TestMethod]
        public void Test_003b_FindAllPaymentMethods() {
            var paymentMethods = RecurringPaymentMethod.FindAll();
            Assert.IsNotNull(paymentMethods);
        }

        [TestMethod]
        public void Test_003c_FindAllSchedules() {
            var schedule = Schedule.FindAll();
            Assert.IsNotNull(schedule);
        }

        [TestMethod]
        public void Test_004a_EditCustomer() {
            var customer = Customer.Find(CustomerId);
            Assert.IsNotNull(customer);

            customer.FirstName = "Bob";
            customer.SaveChanges();
        }

        [TestMethod]
        public void Test_004b_EditPaymentMethod() {
            var paymentMethod = RecurringPaymentMethod.Find(PaymentId("Credit"));
            Assert.IsNotNull(paymentMethod);

            paymentMethod.PreferredPayment = false;
            paymentMethod.SaveChanges();
        }

        //[TestMethod, ExpectedException(typeof(UnsupportedTransactionException))]
        //public void Test_004c_EditPaymentMethodsMethod() {
        //    var paymentMethod = RecurringPaymentMethod.Find(PaymentId("Credit"));
        //    paymentMethod.PaymentMethod = new CreditCardData();
        //}

        [TestMethod]
        public void Test_004d_EditSchedule() {
            var schedule = Schedule.Find(PaymentId("Credit"));
            Assert.IsNotNull(schedule);

            schedule.Status = "Inactive";
            schedule.SaveChanges();
        }

        [TestMethod, ExpectedException(typeof(ApiException))]
        public void Test_005a_EditCustomerBadData() {
            var customer = new Customer { Key = "00000000" };
            customer.SaveChanges();
        }

        [TestMethod, ExpectedException(typeof(ApiException))]
        public void Test_005b_EditPaymentMethodBadData() {
            var paymentMethod = new RecurringPaymentMethod("000000", "000000");
            paymentMethod.SaveChanges();
        }

        [TestMethod]
        public void Test_006a_GetCustomer() {
            var customer = Customer.Find(CustomerId);
            Assert.IsNotNull(customer);

            customer = RecurringService.Get<Customer>(customer.Key);
            Assert.IsNotNull(customer);
        }

        [TestMethod]
        public void Test_006b_GetPaymentMethod() {
            var paymentMethod = RecurringPaymentMethod.Find(PaymentId("Credit"));
            Assert.IsNotNull(paymentMethod);

            paymentMethod = RecurringService.Get<RecurringPaymentMethod>(paymentMethod.Key);
            Assert.IsNotNull(paymentMethod);
        }

        [TestMethod]
        public void Test_006c_GetSchedule() {
            var schedule = Schedule.Find(PaymentId("Credit"));
            Assert.IsNotNull(schedule);

            schedule = RecurringService.Get<Schedule>(schedule.Key);
            Assert.IsNotNull(schedule);
        }

        [TestMethod]
        public void Test_007a_CreditCharge_OneTime() {
            var paymentMethod = RecurringPaymentMethod.Find(PaymentId("Credit"));
            Assert.IsNotNull(paymentMethod, "Payment method missing.");

            var response = paymentMethod.Charge(9m)
                .WithCurrency("USD")
                .WithShippingAmt(5m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_007b_CreditCharge_ScheduleId() {
            var paymentMethod = RecurringPaymentMethod.Find(PaymentId("Credit"));
            Assert.IsNotNull(paymentMethod, "Payment method missing.");

            var schedule = Schedule.Find(PaymentId("Credit"));
            Assert.IsNotNull(schedule, "Schedule is missing.");

            var response = paymentMethod.Charge(10m)
                .WithCurrency("USD")
                .WithScheduleId(schedule.Key)
                .WithOneTimePayment(false)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_007c_ACHCharge_OneTime() {
            var paymentMethod = RecurringPaymentMethod.Find(PaymentId("ACH"));
            Assert.IsNotNull(paymentMethod, "Payment method missing.");

            var response = paymentMethod.Charge(11m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void Test_007d_ACHCharge_ScheduleId() {
            var paymentMethod = RecurringPaymentMethod.Find(PaymentId("ACH"));
            Assert.IsNotNull(paymentMethod, "Payment method missing.");

            var schedule = Schedule.Find(PaymentId("ACH"));
            Assert.IsNotNull(schedule, "Schedule is missing.");

            var response = paymentMethod.Charge(12m)
                .WithCurrency("USD")
                .WithScheduleId(schedule.Key)
                .WithOneTimePayment(false)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_007e_CreditCharge_Declined() {
            var paymentMethod = RecurringPaymentMethod.Find(PaymentId("Credit"));
            Assert.IsNotNull(paymentMethod, "Payment method missing.");

            var response = paymentMethod.Charge(10.08m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("51", response.ResponseCode);
        }

        [TestMethod]
        public void Test_008a_DeleteSchedule_Credit() {
            var schedule = Schedule.Find(PaymentId("Credit"));
            Assert.IsNotNull(schedule);
            schedule.Delete();
        }

        [TestMethod]
        public void Test_008b_DeleteSchedule_ACH() {
            var schedule = Schedule.Find(PaymentId("ACH"));
            Assert.IsNotNull(schedule);
            schedule.Delete();
        }

        [TestMethod]
        public void Test_008c_DeletePaymentMethod() {
            var paymentMethod = RecurringPaymentMethod.Find(PaymentId("Credit"));
            Assert.IsNotNull(paymentMethod);
            paymentMethod.Delete();
        }

        [TestMethod]
        public void Test_008d_DeletePaymentMethod() {
            var paymentMethod = RecurringPaymentMethod.Find(PaymentId("Track"));
            Assert.IsNotNull(paymentMethod);
            paymentMethod.Delete();
        }

        [TestMethod]
        public void Test_008e_DeletePaymentMethod() {
            var paymentMethod = RecurringPaymentMethod.Find(PaymentId("ACH"));
            Assert.IsNotNull(paymentMethod);
            paymentMethod.Delete();
        }

        [TestMethod]
        public void Test_008f_DeleteCustomer() {
            var customer = Customer.Find(CustomerId);
            Assert.IsNotNull(customer);

            customer.Delete();
        }

        [TestMethod]
        public void Test_008g_CreditCharge_WithNewCryptoURL() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
            });
            var paymentMethod = RecurringPaymentMethod.Find(PaymentId("Credit"));
            Assert.IsNotNull(paymentMethod);

            var response = paymentMethod.Charge(17.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("51", response.ResponseCode);

        }
    }
}
