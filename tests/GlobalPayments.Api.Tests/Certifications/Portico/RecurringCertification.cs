using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Certifications.Portico
{
    [TestClass]
    public class RecurringCertification
    {
        private static Customer _customerPerson;
        private static Customer _customerBusiness;
        private static RecurringPaymentMethod _paymentMethodVisa;
        private static RecurringPaymentMethod _paymentMethodMasterCard;
        private static RecurringPaymentMethod _paymentMethodCheckPpd;
        private static RecurringPaymentMethod _paymentMethodCheckCcd;
        private static Schedule _scheduleVisa;
        private static Schedule _scheduleMasterCard;
        private static Schedule _scheduleCheckPpd;
        private static Schedule _scheduleCheckCcd;

        private static readonly string TodayDate = DateTime.Today.ToString("yyyyMMdd");
        private static readonly string IdentifierBase = "{0}-{1}" + Guid.NewGuid().ToString().Substring(0, 10);

        private static string GetIdentifier(string identifier)
        {
            return string.Format(IdentifierBase, TodayDate, identifier);
        }

        public RecurringCertification()
        {
            ServicesContainer.ConfigureService(new PorticoConfig
            {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
            });
        }

        [TestMethod, Ignore]
        public void recurring_000_CloseBatch()
        {
            try
            {
                var response = BatchService.CloseBatch();
                Assert.IsNotNull(response);
                //Console.WriteLine(string.Format("Batch ID: {0}", response.Id));
                //Console.WriteLine(string.Format("Sequence Number: {0}", response.SequenceNumber));
            }
            catch (GatewayException exc)
            {
                if (exc.ResponseMessage != "Transaction was rejected because it requires a batch to be open.")
                    Assert.Fail(exc.Message);
            }
        }

        [TestMethod]
        public void recurring_000_CleanUp()
        {
            // Remove Schedules
            try
            {
                var schResults = Schedule.FindAll();
                foreach (var schedule in schResults)
                {
                    schedule.Delete(true);
                }
            }
            catch (ApiException exc)
            {
                Assert.IsNotNull(exc);
            }

            // Remove Payment Methods
            try
            {
                var pmResults = RecurringPaymentMethod.FindAll();
                foreach (var pm in pmResults)
                {
                    pm.Delete(true);
                }
            }
            catch (ApiException exc)
            {
                Assert.IsNotNull(exc);
            }

            // Remove Customers
            try
            {
                var custResults = Customer.FindAll();
                foreach (var c in custResults)
                {
                    c.Delete(true);
                }
            }
            catch (ApiException exc)
            {
                Assert.IsNotNull(exc);
            }
        }

        // CUSTOMER SETUP

        [TestMethod]
        public void recurring_001_AddCustomerPerson()
        {
            var customer = new Customer
            {
                Id = GetIdentifier("Person"),
                FirstName = "John",
                LastName = "Doe",
                Status = "Active",
                Email = "john.doe@email.com",
                Address = new Address
                {
                    StreetAddress1 = "123 Main St",
                    City = "Dallas",
                    State = "TX",
                    PostalCode = "98765",
                    Country = "USA"
                },
                WorkPhone = "5551112222"
            }.Create();
            Assert.IsNotNull(customer);
            Assert.IsNotNull(customer.Key);
            _customerPerson = customer;
        }

        [TestMethod]
        public void recurring_002_AddCustomerBusiness()
        {
            var customer = new Customer
            {
                Id = GetIdentifier("Business"),
                Company = "AcmeCo",
                Status = "Active",
                Email = "acme@email.com",
                Address = new Address
                {
                    StreetAddress1 = "987 Elm St",
                    City = "Princeton",
                    State = "NJ",
                    PostalCode = "12345",
                    Country = "USA"
                },
                WorkPhone = "5551112222"
            }.Create();
            Assert.IsNotNull(customer);
            Assert.IsNotNull(customer.Key);
            _customerBusiness = customer;
        }

        // PAYMENT METHOD SETUP

        [TestMethod]
        public void recurring_003_AddPaymentCreditVisa()
        {
            if (_customerPerson == null)
                Assert.Inconclusive();

            var paymentMethod = _customerPerson.AddPaymentMethod(GetIdentifier("CreditV"), new CreditCardData
            {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025
            }).Create();
            Assert.IsNotNull(paymentMethod);
            Assert.IsNotNull(paymentMethod.Key);
            _paymentMethodVisa = paymentMethod;
        }

        [TestMethod]
        public void recurring_004_AddPaymentCreditMasterCard()
        {
            if (_customerPerson == null)
                Assert.Inconclusive();

            var paymentMethod = _customerPerson.AddPaymentMethod(
                GetIdentifier("CreditMC"),
                new CreditCardData
                {
                    Number = "5473500000000014",
                    ExpMonth = 12,
                    ExpYear = 2025
                }).Create();
            Assert.IsNotNull(paymentMethod);
            Assert.IsNotNull(paymentMethod.Key);
            _paymentMethodMasterCard = paymentMethod;
        }

        [TestMethod]
        public void recurring_005_AddPaymentCheckPPD()
        {
            if (_customerPerson == null)
                Assert.Inconclusive();

            var paymentMethod = _customerPerson.AddPaymentMethod(
                GetIdentifier("CheckPPD"),
                new eCheck
                {
                    AccountType = AccountType.CHECKING,
                    CheckType = CheckType.PERSONAL,
                    SecCode = SecCode.PPD,
                    RoutingNumber = "490000018",
                    DriversLicenseNumber = "7418529630",
                    DriversLicenseState = "TX",
                    AccountNumber = "24413815",
                    BirthYear = 1989
                }).Create();
            Assert.IsNotNull(paymentMethod);
            Assert.IsNotNull(paymentMethod.Key);
            _paymentMethodCheckPpd = paymentMethod;
        }

        [TestMethod]
        public void recurring_006_AddPaymentCheckCCD()
        {
            if (_customerBusiness == null)
                Assert.Inconclusive();

            var paymentMethod = _customerBusiness.AddPaymentMethod(
                    GetIdentifier("CheckCCD"),
                    new eCheck
                    {
                        AccountType = AccountType.CHECKING,
                        CheckType = CheckType.BUSINESS,
                        SecCode = SecCode.CCD,
                        RoutingNumber = "490000018",
                        DriversLicenseNumber = "3692581470",
                        DriversLicenseState = "TX",
                        AccountNumber = "24413815",
                        BirthYear = 1989
                    }
                ).Create();
            Assert.IsNotNull(paymentMethod);
            Assert.IsNotNull(paymentMethod.Key);
            _paymentMethodCheckCcd = paymentMethod;
        }

        // PAYMENT SETUP - DECLINED

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void recurring_007_AddPaymentCheckPPD()
        {
            if (_customerPerson == null)
                Assert.Inconclusive();

            var paymentMethod = _customerPerson.AddPaymentMethod(
                    GetIdentifier("CheckPPD"),
                    new eCheck
                    {
                        AccountType = AccountType.CHECKING,
                        CheckType = CheckType.PERSONAL,
                        SecCode = SecCode.PPD,
                        RoutingNumber = "490000018",
                        DriversLicenseNumber = "7418529630",
                        DriversLicenseState = "TX",
                        AccountNumber = "24413815",
                        BirthYear = 1989
                    }
                ).Create();
        }

        // Recurring Billing using PayPlan - Managed Schedule

        [TestMethod]
        public void recurring_008_AddScheduleCreditVisa()
        {
            if (_paymentMethodVisa == null)
                Assert.Inconclusive();

            var schedule = _paymentMethodVisa.AddSchedule(GetIdentifier("CreditV"))
                .WithStartDate(DateTime.Parse("02/01/2027"))
                .WithAmount(30.01m)
                .WithFrequency(ScheduleFrequency.WEEKLY)
                .WithReprocessingCount(1)
                .WithStatus("Active")
                .Create();
            Assert.IsNotNull(schedule);
            Assert.IsNotNull(schedule.Key);
            _scheduleVisa = schedule;
        }

        [TestMethod]
        public void recurring_009_AddScheduleCreditMasterCard()
        {
            if (_paymentMethodMasterCard == null)
                Assert.Inconclusive();

            var schedule = _paymentMethodMasterCard.AddSchedule(GetIdentifier("CreditMC"))
                .WithStatus("Active")
                .WithAmount(30.02m)
                .WithStartDate(DateTime.Parse("02/01/2027"))
                .WithFrequency(ScheduleFrequency.WEEKLY)
                .WithEndDate(DateTime.Parse("04/01/2027"))
                .WithReprocessingCount(2)
                .Create();
            Assert.IsNotNull(schedule);
            Assert.IsNotNull(schedule.Key);
            _scheduleMasterCard = schedule;
        }

        [TestMethod]
        public void recurring_010_AddScheduleCheckPPD()
        {
            if (_paymentMethodCheckPpd == null)
                Assert.Inconclusive();

            var schedule = _paymentMethodCheckPpd.AddSchedule(GetIdentifier("CheckPPD"))
                .WithStatus("Active")
                .WithAmount(30.03m)
                .WithStartDate(DateTime.Parse("02/01/2027"))
                .WithFrequency(ScheduleFrequency.MONTHLY)
                .WithReprocessingCount(1)
                .WithNumberOfPayments(2)
                .Create();
            Assert.IsNotNull(schedule);
            Assert.IsNotNull(schedule.Key);
            _scheduleCheckPpd = schedule;
        }

        [TestMethod]
        public void recurring_011_AddScheduleCheckCCD()
        {
            if (_paymentMethodCheckCcd == null)
                Assert.Inconclusive();

            var schedule = _paymentMethodCheckCcd.AddSchedule(GetIdentifier("CheckCCD"))
                .WithStatus("Active")
                .WithAmount(30.04m)
                .WithStartDate(DateTime.Parse("02/01/2027"))
                .WithFrequency(ScheduleFrequency.BI_WEEKLY)
                .WithReprocessingCount(1)
                .Create();
            Assert.IsNotNull(schedule);
            Assert.IsNotNull(schedule.Key);
            _scheduleCheckCcd = schedule;
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void recurring_012_AddScheduleCreditVisa()
        {
            if (_paymentMethodVisa == null)
                Assert.Inconclusive();

            var schedule = _paymentMethodVisa.AddSchedule(GetIdentifier("CreditV"))
                .WithStartDate(DateTime.Parse("02/01/2027"))
                .WithAmount(30.01m)
                .WithFrequency(ScheduleFrequency.WEEKLY)
                .WithReprocessingCount(1)
                .WithStatus("Active")
                .Create();
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void recurring_013_AddScheduleCCheckPPD()
        {
            if (_paymentMethodCheckPpd == null)
                Assert.Inconclusive();

            var schedule = _paymentMethodCheckPpd.AddSchedule(GetIdentifier("CheckPPD"))
                .WithStatus("Active")
                .WithAmount(30.03m)
                .WithStartDate(DateTime.Parse("02/01/2027"))
                .WithFrequency(ScheduleFrequency.MONTHLY)
                .WithReprocessingCount(1)
                .WithNumberOfPayments(2)
                .Create();
        }

        // Recurring Billing using PayPlan - Managed Schedule

        [TestMethod]
        public void recurring_014_RecurringBillingVisa()
        {
            if (_paymentMethodVisa == null || _scheduleVisa == null)
                Assert.Inconclusive();

            var response = _paymentMethodVisa.Charge(20.01m)
                .WithCurrency("USD")
                .WithScheduleId(_scheduleVisa.Key)
                .WithOneTimePayment(false)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void recurring_015_RecurringBillingMasterCard()
        {
            if (_paymentMethodMasterCard == null || _scheduleMasterCard == null)
                Assert.Inconclusive();

            var response = _paymentMethodMasterCard.Charge(20.02m)
                .WithCurrency("USD")
                .WithScheduleId(_scheduleVisa.Key)
                .WithOneTimePayment(false)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void recurring_016_RecurringBillingCheckPPD()
        {
            if (_paymentMethodCheckPpd == null || _scheduleCheckPpd == null)
                Assert.Inconclusive();

            var response = _paymentMethodCheckPpd.Charge(20.03m)
                .WithCurrency("USD")
                .WithScheduleId(_scheduleVisa.Key)
                .WithOneTimePayment(false)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void recurring_017_RecurringBillingCheckCCD()
        {
            if (_paymentMethodCheckCcd == null || _scheduleCheckCcd == null)
                Assert.Inconclusive();

            var response = _paymentMethodCheckCcd.Charge(20.04m)
                .WithCurrency("USD")
                .WithScheduleId(_scheduleVisa.Key)
                .WithOneTimePayment(false)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // One time bill payment

        [TestMethod]
        public void recurring_018_RecurringBillingVisa()
        {
            if (_paymentMethodVisa == null)
                Assert.Inconclusive();

            var response = _paymentMethodVisa.Charge(20.06m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void recurring_019_RecurringBillingMasterCard()
        {
            if (_paymentMethodMasterCard == null)
                Assert.Inconclusive();

            var response = _paymentMethodMasterCard.Charge(20.07m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void recurring_020_RecurringBillingCheckPPD()
        {
            if (_paymentMethodCheckPpd == null)
                Assert.Inconclusive();

            var response = _paymentMethodCheckPpd.Charge(20.08m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void recurring_021_RecurringBillingCheckCCD()
        {
            if (_paymentMethodCheckCcd == null)
                Assert.Inconclusive();

            var response = _paymentMethodCheckCcd.Charge(20.09m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Onetime bill payment - declined

        [TestMethod]
        public void recurring_022_RecurringBillingVisa_Decline()
        {
            if (_paymentMethodVisa == null)
                Assert.Inconclusive();

            var response = _paymentMethodVisa.Charge(10.08m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("51", response.ResponseCode);
        }

        [TestMethod]
        public void recurring_023_RecurringBillingCheckPPD_Decline()
        {
            if (_paymentMethodCheckPpd == null)
                Assert.Inconclusive();

            var response = _paymentMethodCheckPpd.Charge(25.02m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("1", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void recurring_999_CloseBatch()
        {
            try
            {
                var response = BatchService.CloseBatch();
                Assert.IsNotNull(response);
                //Console.WriteLine(string.Format("Batch ID: {0}", response.Id));
                //Console.WriteLine(string.Format("Sequence Number: {0}", response.SequenceNumber));
            }
            catch (GatewayException exc)
            {
                if (exc.ResponseMessage != "Transaction was rejected because it requires a batch to be open.")
                    Assert.Fail(exc.Message);
            }
        }
    }
}
