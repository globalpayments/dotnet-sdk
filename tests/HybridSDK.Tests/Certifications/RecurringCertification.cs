using System;
using HybridSDK.Entities;
using HybridSDK.PaymentMethods;
using HybridSDK.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HybridSDK.Tests.Certifications {
    [TestClass]
    public class RecurringCertification {
        private static string _customerPersonKey;
        private static string _customerCompanyKey;
        private static string _paymentMethodKeyVisa;
        private static string _paymentMethodKeyMasterCard;
        private static string _paymentMethodKeyCheckPpd;
        private static string _paymentMethodKeyCheckCcd;
        private static string _scheduleKeyVisa;
        private static string _scheduleKeyMasterCard;
        private static string _scheduleKeyCheckPpd;
        private static string _scheduleKeyCheckCcd;

        private static readonly string TodayDate = DateTime.Today.ToString("yyyyMMdd");
        private static readonly string IdentifierBase = "{0}-{1}" + Guid.NewGuid().ToString().Substring(0, 10);

        private static string GetIdentifier(string identifier) {
            var rValue = string.Format(IdentifierBase, TodayDate, identifier);
            Console.WriteLine(rValue);

            return rValue;
        }

        public RecurringCertification() {
            ServicesContainer.Configure(new ServicesConfig {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A",
                ServiceUrl = "https://cert.api2.heartlandportico.com"
            });
        }

        [TestMethod, Ignore]
        public void recurring_000_CloseBatch() {
            try {
                var response = BatchService.CloseBatch();
                Assert.IsNotNull(response);
                Console.WriteLine(string.Format("Batch ID: {0}", response.Id));
                Console.WriteLine(string.Format("Sequence Number: {0}", response.SequenceNumber));
            }
            catch (GatewayException exc) {
                if (exc.ResponseMessage != "Transaction was rejected because it requires a batch to be open.")
                    Assert.Fail(exc.Message);
            }
        }

        [TestMethod, Ignore]
        public void recurring_000_CleanUp() {
            // Remove Schedules
            try {
                var schResults = Schedule.FindAll();
                foreach (var schedule in schResults) {
                    schedule.Delete(true);
                }
            }
            catch { }

            // Remove Payment Methods
            try {
                var pmResults = RecurringPaymentMethod.FindAll();
                foreach (var pm in pmResults) {
                    pm.Delete(true);
                }
            }
            catch { }

            // Remove Customers
            try {
                var custResults = Customer.FindAll();
                foreach (var c in custResults) {
                    c.Delete(true);
                }
            }
            catch { }
        }

        // CUSTOMER SETUP

        [TestMethod]
        public void recurring_001_AddCustomerPerson() {
            var customer = RecurringService.Create(new Customer {
                Id = GetIdentifier("Person"),
                FirstName = "John",
                LastName = "Doe",
                Status = "Active",
                Email = "john.doe@email.com",
                Address = new Address {
                    StreetAddress1 = "123 Main St",
                    City = "Dallas",
                    State = "TX",
                    PostalCode = "98765",
                    Country = "USA"
                },                
                WorkPhone = "5551112222"
            });
            Assert.IsNotNull(customer);
            Assert.IsNotNull(customer.Key);
            _customerPersonKey = customer.Key;
        }

        [TestMethod]
        public void recurring_002_AddCustomerBusiness() {
            var customer = RecurringService.Create(new Customer {
                Id = GetIdentifier("Business"),
                Company = "AcmeCo",
                Status = "Active",
                Email = "acme@email.com",
                Address = new Address {
                    StreetAddress1 = "987 Elm St",
                    City = "Princeton",
                    State = "NJ",
                    PostalCode = "12345",
                    Country = "USA"
                },
                WorkPhone = "5551112222"
            };

            var response = _payPlanService.AddCustomer(customer);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.CustomerKey);

            _customerCompanyKey = response.CustomerKey;
        }

        // PAYMENT METHOD SETUP

        [TestMethod]
        public void recurring_003_AddPaymentCreditVisa() {
            if (_customerPersonKey == null)
                Assert.Inconclusive();

            var paymentMethod = new HpsPayPlanPaymentMethod {
                PaymentMethodIdentifier = GetIdentifier("CreditV"),
                PaymentMethodType = HpsPayPlanPaymentMethodType.CreditCard,
                NameOnAccount = "John Doe",
                AccountNumber = "4012002000060016",
                ExpirationDate = "1225",
                CustomerKey = _customerPersonKey,
                Country = "USA"
            };

            var response = _payPlanService.AddPaymentMethod(paymentMethod);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.PaymentMethodKey);
            Assert.IsNotNull(response.CreationDate);

            _paymentMethodKeyVisa = response.PaymentMethodKey;
        }

        [TestMethod]
        public void recurring_004_AddPaymentCreditMasterCard() {
            if (_customerPersonKey == null)
                Assert.Inconclusive();

            var paymentMethod = new HpsPayPlanPaymentMethod {
                PaymentMethodIdentifier = GetIdentifier("CreditMC"),
                PaymentMethodType = HpsPayPlanPaymentMethodType.CreditCard,
                NameOnAccount = "John Doe",
                AccountNumber = "5473500000000014",
                ExpirationDate = "1225",
                CustomerKey = _customerPersonKey,
                Country = "USA"
            };

            var response = _payPlanService.AddPaymentMethod(paymentMethod);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.PaymentMethodKey);
            Assert.IsNotNull(response.CreationDate);

            _paymentMethodKeyMasterCard = response.PaymentMethodKey;
        }

        [TestMethod]
        public void recurring_005_AddPaymentCheckPPD() {
            if (_customerPersonKey == null)
                Assert.Inconclusive();

            var paymentMethod = new HpsPayPlanPaymentMethod {
                PaymentMethodIdentifier = GetIdentifier("CheckPPD"),
                PaymentMethodType = HpsPayPlanPaymentMethodType.Ach,
                AchType = "Checking",
                AccountType = "Personal",
                TelephoneIndicator = false,
                RoutingNumber = "490000018",
                NameOnAccount = "John Doe",
                DriversLicenseNumber = "7418529630",
                DriversLicenseState = "TX",
                AccountNumber = "24413815",
                AddressLine1 = "123 Main St",
                City = "Dallas",
                StateProvince = "TX",
                ZipPostalCode = "98765",
                CustomerKey = _customerPersonKey,
                Country = "USA",
                AccountHolderYob = "1989"
            };

            var response = _payPlanService.AddPaymentMethod(paymentMethod);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.PaymentMethodKey);
            Assert.IsNotNull(response.CreationDate);

            _paymentMethodKeyCheckPpd = response.PaymentMethodKey;
        }

        [TestMethod]
        public void recurring_006_AddPaymentCheckCCD() {
            if (_customerCompanyKey == null)
                Assert.Inconclusive();

            var paymentMethod = new HpsPayPlanPaymentMethod {
                PaymentMethodIdentifier = GetIdentifier("CheckCCD"),
                PaymentMethodType = HpsPayPlanPaymentMethodType.Ach,
                AchType = "Checking",
                AccountType = "Business",
                TelephoneIndicator = false,
                RoutingNumber = "490000018",
                NameOnAccount = "Acme Co",
                DriversLicenseNumber = "3692581470",
                DriversLicenseState = "TX",
                AccountNumber = "24413815",
                AddressLine1 = "987 Elm St",
                City = "Princeton",
                StateProvince = "NJ",
                ZipPostalCode = "13245",
                CustomerKey = _customerCompanyKey,
                Country = "USA",
                AccountHolderYob = "1989"
            };

            var response = _payPlanService.AddPaymentMethod(paymentMethod);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.PaymentMethodKey);
            Assert.IsNotNull(response.CreationDate);

            _paymentMethodKeyCheckCcd = response.PaymentMethodKey;
        }

        // PAYMENT SETUP - DECLINED

        [TestMethod]
        public void recurring_007_AddPaymentCheckPPD() {
            if (_customerPersonKey == null)
                Assert.Inconclusive();

            try {
                var paymentMethod = new HpsPayPlanPaymentMethod {
                    PaymentMethodIdentifier = GetIdentifier("CheckPPD"),
                    PaymentMethodType = HpsPayPlanPaymentMethodType.Ach,
                    AchType = "Checking",
                    AccountType = "Personal",
                    TelephoneIndicator = false,
                    RoutingNumber = "490000018",
                    NameOnAccount = "John Doe",
                    DriversLicenseNumber = "7418529630",
                    DriversLicenseState = "TX",
                    AccountNumber = "24413815",
                    AddressLine1 = "123 Main St",
                    City = "Dallas",
                    StateProvince = "TX",
                    ZipPostalCode = "98765",
                    CustomerKey = _customerPersonKey,
                    Country = "USA",
                    AccountHolderYob = "1989"
                };

                _payPlanService.AddPaymentMethod(paymentMethod);
            }
            catch (HpsException) { }
        }

        // Recurring Billing using PayPlan - Managed Schedule

        [TestMethod]
        public void recurring_008_AddScheduleCreditVisa() {
            if (_customerPersonKey == null || _paymentMethodKeyVisa == null)
                Assert.Inconclusive();

            var schedule = new HpsPayPlanSchedule {
                ScheduleIdentifier = GetIdentifier("CreditV"),
                CustomerKey = _customerPersonKey,
                ScheduleStatus = HpsPayPlanScheduleStatus.Active,
                PaymentMethodKey = _paymentMethodKeyVisa,
                SubtotalAmount = new HpsPayPlanAmount("3001"),
                StartDate = "02012027",
                Frequency = HpsPayPlanScheduleFrequency.Weekly,
                Duration = HpsPayPlanScheduleDuration.Ongoing,
                ReprocessingCount = 1
            };

            var response = _payPlanService.AddSchedule(schedule);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ScheduleKey);

            _scheduleKeyVisa = response.ScheduleKey;
        }

        [TestMethod]
        public void recurring_009_AddScheduleCreditMasterCard() {
            if (_customerPersonKey == null || _paymentMethodKeyMasterCard == null)
                Assert.Inconclusive();

            var schedule = new HpsPayPlanSchedule {
                ScheduleIdentifier = GetIdentifier("CreditMC"),
                CustomerKey = _customerPersonKey,
                ScheduleStatus = HpsPayPlanScheduleStatus.Active,
                PaymentMethodKey = _paymentMethodKeyMasterCard,
                SubtotalAmount = new HpsPayPlanAmount("3002"),
                StartDate = "02012027",
                Frequency = HpsPayPlanScheduleFrequency.Weekly,
                Duration = HpsPayPlanScheduleDuration.EndDate,
                EndDate = "04012027",
                ReprocessingCount = 2
            };

            var response = _payPlanService.AddSchedule(schedule);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ScheduleKey);

            _scheduleKeyMasterCard = response.ScheduleKey;
        }

        [TestMethod]
        public void recurring_010_AddScheduleCheckPPD() {
            if (_customerPersonKey == null || _paymentMethodKeyCheckPpd == null)
                Assert.Inconclusive();

            var schedule = new HpsPayPlanSchedule {
                ScheduleIdentifier = GetIdentifier("CheckPPD"),
                CustomerKey = _customerPersonKey,
                ScheduleStatus = HpsPayPlanScheduleStatus.Active,
                PaymentMethodKey = _paymentMethodKeyCheckPpd,
                SubtotalAmount = new HpsPayPlanAmount("3003"),
                StartDate = "02012027",
                Frequency = HpsPayPlanScheduleFrequency.Monthly,
                Duration = HpsPayPlanScheduleDuration.LimitedNumber,
                ReprocessingCount = 1,
                NumberOfPayments = 2,
                ProcessingDateInfo = "1"
            };

            var response = _payPlanService.AddSchedule(schedule);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ScheduleKey);

            _scheduleKeyCheckPpd = response.ScheduleKey;
        }

        [TestMethod]
        public void recurring_011_AddScheduleCheckCCD() {
            if (_customerCompanyKey == null || _paymentMethodKeyCheckCcd == null)
                Assert.Inconclusive();

            var schedule = new HpsPayPlanSchedule {
                ScheduleIdentifier = GetIdentifier("CheckCCD"),
                CustomerKey = _customerCompanyKey,
                ScheduleStatus = HpsPayPlanScheduleStatus.Active,
                PaymentMethodKey = _paymentMethodKeyCheckCcd,
                SubtotalAmount = new HpsPayPlanAmount("3004"),
                StartDate = "02012027",
                Frequency = HpsPayPlanScheduleFrequency.Biweekly,
                Duration = HpsPayPlanScheduleDuration.Ongoing,
                ReprocessingCount = 1
            };

            var response = _payPlanService.AddSchedule(schedule);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ScheduleKey);

            _scheduleKeyCheckCcd = response.ScheduleKey;
        }

        [TestMethod]
        [ExpectedException(typeof(HpsException))]
        public void recurring_012_AddScheduleCreditVisa() {
            if (_customerPersonKey == null || _paymentMethodKeyVisa == null)
                Assert.Inconclusive();

            var schedule = new HpsPayPlanSchedule {
                ScheduleIdentifier = GetIdentifier("CreditV"),
                CustomerKey = _customerPersonKey,
                ScheduleStatus = HpsPayPlanScheduleStatus.Active,
                PaymentMethodKey = _paymentMethodKeyVisa,
                SubtotalAmount = new HpsPayPlanAmount("3001"),
                StartDate = "02012027",
                Frequency = HpsPayPlanScheduleFrequency.Weekly,
                Duration = HpsPayPlanScheduleDuration.Ongoing,
                ReprocessingCount = 1
            };

            _payPlanService.AddSchedule(schedule);
        }

        [TestMethod]
        [ExpectedException(typeof(HpsException))]
        public void recurring_013_AddScheduleCCheckPPD() {
            if (_customerPersonKey == null || _paymentMethodKeyCheckPpd == null)
                Assert.Inconclusive();

            var schedule = new HpsPayPlanSchedule {
                ScheduleIdentifier = GetIdentifier("CheckPPD"),
                CustomerKey = _customerPersonKey,
                ScheduleStatus = HpsPayPlanScheduleStatus.Active,
                PaymentMethodKey = _paymentMethodKeyCheckPpd,
                SubtotalAmount = new HpsPayPlanAmount("3003"),
                StartDate = "02012027",
                Frequency = HpsPayPlanScheduleFrequency.Monthly,
                Duration = HpsPayPlanScheduleDuration.LimitedNumber,
                ReprocessingCount = 1,
                NumberOfPayments = 2,
                ProcessingDateInfo = "1"
            };

            _payPlanService.AddSchedule(schedule);
        }

        // Recurring Billing using PayPlan - Managed Schedule

        [TestMethod]
        public void recurring_014_RecurringBillingVisa() {
            if (_paymentMethodKeyVisa == null || _scheduleKeyVisa == null)
                Assert.Inconclusive();

            var response = _creditService.Recurring(20.01m)
                .WithPaymentMethodKey(_paymentMethodKeyVisa)
                .WithScheduleId(_scheduleKeyVisa)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void recurring_015_RecurringBillingMasterCard() {
            if (_paymentMethodKeyMasterCard == null || _scheduleKeyMasterCard == null)
                Assert.Inconclusive();

            var response = _creditService.Recurring(20.02m)
                .WithPaymentMethodKey(_paymentMethodKeyMasterCard)
                .WithScheduleId(_scheduleKeyMasterCard)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void recurring_016_RecurringBillingCheckPPD() {
            if (_paymentMethodKeyCheckPpd == null || _scheduleKeyCheckPpd == null)
                Assert.Inconclusive();

            var response = _checkService.Recurring(20.03m)
                .WithPaymentMethodKey(_paymentMethodKeyCheckPpd)
                .WithScheduleId(_scheduleKeyCheckPpd)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void recurring_017_RecurringBillingCheckCCD() {
            if (_paymentMethodKeyCheckCcd == null || _scheduleKeyCheckCcd == null)
                Assert.Inconclusive();

            var response = _checkService.Recurring(20.04m)
                .WithPaymentMethodKey(_paymentMethodKeyCheckCcd)
                .WithScheduleId(_scheduleKeyCheckCcd)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // One time bill payment

        [TestMethod]
        public void recurring_018_RecurringBillingVisa() {
            if (_paymentMethodKeyVisa == null)
                Assert.Inconclusive();

            var response = _creditService.Recurring(20.06m)
                .WithPaymentMethodKey(_paymentMethodKeyVisa)
                .WithOneTime(true).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void recurring_019_RecurringBillingMasterCard() {
            if (_scheduleKeyMasterCard == null)
                Assert.Inconclusive();

            var response = _creditService.Recurring(20.07m)
                .WithPaymentMethodKey(_paymentMethodKeyMasterCard)
                .WithOneTime(true).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void recurring_020_RecurringBillingCheckPPD() {
            if (_paymentMethodKeyCheckPpd == null)
                Assert.Inconclusive();

            var response = _checkService.Recurring(20.08m)
                .WithPaymentMethodKey(_paymentMethodKeyCheckPpd)
                .WithOneTime(true).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void recurring_021_RecurringBillingCheckCCD() {
            if (_paymentMethodKeyCheckCcd == null)
                Assert.Inconclusive();

            var response = _checkService.Recurring(20.09m)
                .WithPaymentMethodKey(_paymentMethodKeyCheckCcd)
                .WithOneTime(true).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // Onetime bill payment - declined

        [TestMethod]
        [ExpectedException(typeof(HpsCreditException))]
        public void recurring_022_RecurringBillingVisa() {
            if (_paymentMethodKeyVisa == null)
                Assert.Inconclusive();

            _creditService.Recurring(10.08m)
                .WithPaymentMethodKey(_paymentMethodKeyVisa)
                .WithOneTime(true).Execute();
        }

        [TestMethod]
        [ExpectedException(typeof(HpsCheckException))]
        public void recurring_023_RecurringBillingCheckPPD() {
            if (_paymentMethodKeyCheckPpd == null)
                Assert.Inconclusive();

            _checkService.Recurring(25.02m)
                .WithPaymentMethodKey(_paymentMethodKeyCheckPpd)
                .WithOneTime(true).Execute();
        }

        [TestMethod, Ignore]
        public void recurring_999_CloseBatch() {
            var response = _batchService.CloseBatch();
            Assert.IsNotNull(response);
            Console.WriteLine(@"Batch ID: {0}", response.Id);
            Console.WriteLine(@"Sequence Number: {0}", response.SequenceNumber);
        }
    }
}
