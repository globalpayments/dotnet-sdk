using System;
using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpEcom
{
    [TestClass]
    public class GpEcomRecurringTest {
        private readonly Customer newCustomer;
        private readonly CreditCardData card;
        private const decimal Amount = 18.5m;
        private const string Currency = "USD";

        private static string CustomerId {
            get { return $"{DateTime.Now.ToString("yyyyMMddhhmm")}-Realex"; }
        }

        private static string PaymentId(string type) {
            return $"{DateTime.Now.AddDays(1):yyyyMMdd}-Realex-{type}";
        }

        public GpEcomRecurringTest() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "3dsecure",
                RefundPassword = "refund",
                SharedSecret = "secret",
                Channel = "ECOM",
                RequestLogger = new RequestConsoleLogger()
            });

            newCustomer = new Customer {
                Key = CustomerId,
                Id = "E8953893489",
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

            card = new CreditCardData {
                Number = "5425230000004415",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.AddYears(2).Year,
                CardHolderName = "James New Mason"
            };
        }

        [TestMethod]
        public void Test_001a_CreateCustomer() {
            try {
                var customer = newCustomer.Create();
                Assert.IsNotNull(customer);
            } catch (GatewayException exc) {
                // check for already created
                if (exc.ResponseCode != "501")
                    throw;
            }
        }

        [TestMethod]
        public void Test_001b_CreatePaymentMethod() {
            try {
                var paymentMethod = newCustomer.AddPaymentMethod(PaymentId("Credit"), new CreditCardData {
                    Number = "4263970000005262",
                    ExpMonth = DateTime.Now.Month,
                    ExpYear = DateTime.Now.AddYears(2).Year,
                    CardHolderName = "James Mason"
                }).Create();
                Assert.IsNotNull(paymentMethod);
            } catch (GatewayException exc) {
                // check for already created
                if (exc.ResponseCode != "520")
                    throw;
            }
        }

        [TestMethod]
        public void Test_001c_CreatePaymentMethodWithStoredCredential() {
            try {
                var storedCredential = new StoredCredential {
                    SchemeId = "YOUR_DESIRED_SCHEME_ID_NEW1"
                };

                var paymentMethod =
                    newCustomer
                        .AddPaymentMethod(PaymentId("Credit"), card, storedCredential)
                        .Create();

                Assert.IsNotNull(paymentMethod);
            } catch (GatewayException exc) {
                if (!exc.ResponseCode.Equals("520"))
                    throw;
            }
        }

        [TestMethod]
        public void Test_002c_EditPaymentMethodWithStoredCredential() {
            var paymentMethod = new RecurringPaymentMethod(CustomerId, PaymentId("Credit"));
            card.CardHolderName = "Philip Marlowe";

            paymentMethod.PaymentMethod = card;
            var storedCredential = new StoredCredential {
                SchemeId = "YOUR_DESIRED_SCHEME_ID"
            };
            paymentMethod.StoredCredential = storedCredential;

            try {
                paymentMethod.SaveChanges();
            } catch (GatewayException exc) {
                // check for already created
                if (exc.ResponseCode != "520")
                    throw;
            }
        }

        [TestMethod]
        public void Test_002a_EditCustomer() {
            var customer = new Customer {
                Key = CustomerId,
                FirstName = "Perry"
            };
            
            customer.SaveChanges();
        }

        [TestMethod]
        public void Test_002b_EditPaymentMethod() {
            var paymentMethod = new RecurringPaymentMethod(CustomerId, PaymentId("Credit")) {
                PaymentMethod = new CreditCardData {
                    Number = "5425230000004415",
                    ExpMonth = DateTime.Now.Month,
                    ExpYear = DateTime.Now.AddYears(2).Year,
                    CardHolderName = "Philip Marlowe"
                }
            };
            
            paymentMethod.SaveChanges();
        }

        [TestMethod]
        public void Test_002c_EditPaymentMethodExpOnly() {
            var paymentMethod = new RecurringPaymentMethod(CustomerId, PaymentId("Credit")) {
                PaymentMethod = new CreditCardData {
                    CardType = "MC",
                    ExpMonth = DateTime.Now.Month,
                    ExpYear = DateTime.Now.AddYears(2).Year,
                    CardHolderName = "Philip Marlowe"
                }
            };
            
            paymentMethod.SaveChanges();
        }

        [TestMethod, ExpectedException(typeof(UnsupportedTransactionException))]
        public void Test_003_FindOnRealex() {
            Customer.Find(CustomerId);
        }

        [TestMethod]
        public void Test_004a_ChargeStoredCard() {
            var paymentMethod = new RecurringPaymentMethod(CustomerId, PaymentId("Credit"));
            var response = paymentMethod.Charge(Amount)
                .WithCurrency(Currency)
                .WithCvn("123")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_004b_VerifyStoredCard() {
            var paymentMethod = new RecurringPaymentMethod(CustomerId, PaymentId("Credit"));
            var response = paymentMethod.Verify()
                .WithCvn("123")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_004c_RefundStoredCard() {
            var paymentMethod = new RecurringPaymentMethod(CustomerId, PaymentId("Credit"));
            var response = paymentMethod.Refund(Amount)
                .WithCurrency(Currency)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_RecurringPayment() {
            var paymentMethod = new RecurringPaymentMethod(CustomerId, PaymentId("Credit"));
            var response = paymentMethod.Charge(Amount)
                .WithRecurringInfo(RecurringType.Fixed, RecurringSequence.First)
                .WithCurrency(Currency)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_006_DeletePaymentMethod() {
            var paymentMethod = new RecurringPaymentMethod(CustomerId, PaymentId("Credit"));
            
            paymentMethod.Delete();
        }

        [TestMethod]
        public void Test_007_ChargeStoredCard_from_different_configs() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "3dsecure",
                RefundPassword = "refund",
                SharedSecret = "secret",
                RequestLogger = new RequestConsoleLogger()
            });

            var paymentMethod = newCustomer.AddPaymentMethod(PaymentId("Credit"), new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.AddYears(2).Year,
                CardHolderName = "James Mason"
            }).Create();
            
            Assert.IsNotNull(paymentMethod);
            
            var response = paymentMethod.Charge(Amount)
                .WithRecurringInfo(RecurringType.Fixed, RecurringSequence.First)
                .WithCurrency(Currency)
                .Execute();
            
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var pm = new RecurringPaymentMethod(CustomerId, PaymentId("Credit"));
            
            pm.Delete();

            var paymentMethod2 = newCustomer.AddPaymentMethod(PaymentId("Credit"), new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.AddYears(2).Year,
                CardHolderName = "James Mason 2"
            }).Create();
            
            Assert.IsNotNull(paymentMethod2);
            
            var response2 = paymentMethod2.Charge(Amount)
                .WithRecurringInfo(RecurringType.Fixed, RecurringSequence.First)
                .WithCurrency(Currency)
                .Execute();
            
            Assert.IsNotNull(response2);
            Assert.AreEqual("00", response2.ResponseCode);

            var pm2 = new RecurringPaymentMethod(CustomerId, PaymentId("Credit"));
            
            pm2.Delete();
        }

        /**************** Payment Scheduler Test ****************/

        #region Payment Scheduler Test

        [TestMethod]
        public void CardStorageAddSchedule() {
            try {
                var response = newCustomer.Create();
                Assert.IsNotNull(response);
                Assert.IsInstanceOfType(response, typeof(Customer));
            } catch (GatewayException exc) {
                if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                    throw;
                }
            }

            var paymentMethod = newCustomer.AddPaymentMethod(PaymentId("Credit"), card);
            try {
                var response = paymentMethod.Create();
                Assert.IsNotNull(response);
                Assert.IsInstanceOfType(response, typeof(RecurringPaymentMethod));
            } catch (GatewayException exc) {
                if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                    throw;
                }
            }

            var scheduleId = GenerationUtils.GenerateScheduleId();
            try {
                var response = paymentMethod.AddSchedule(scheduleId)
                    .WithStartDate(DateTime.Now)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithFrequency(ScheduleFrequency.SEMI_ANNUALLY)
                    .WithReprocessingCount(1)
                    .WithNumberOfPayments(12)
                    .WithCustomerNumber("E8953893489")
                    .WithOrderPrefix("gym")
                    .WithName("Gym Membership")
                    .WithDescription("Social Sign-Up")
                    .Create();

                Assert.IsInstanceOfType(response, typeof(Schedule));
                Assert.AreEqual("00", response.ResponseCode);
                Assert.AreEqual("Schedule created successfully", response.ResponseMessage);

                var schedule = RecurringService.Get<Schedule>(scheduleId);

                Assert.AreEqual(scheduleId, schedule.Id);
                Assert.AreEqual(12, schedule.NumberOfPayments);
                // Assert.AreEqual(ScheduleFrequency.SEMI_ANNUALLY, schedule.Frequency);
            } catch (GatewayException exc) {
                if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                    throw;
                }
            }
        }
        
        [TestMethod]
        public void CardStorageAddSchedule_AllFrequency() {
            try {
                var response = newCustomer.Create();
                Assert.IsNotNull(response);
                Assert.IsInstanceOfType(response, typeof(Customer));
            } catch (GatewayException exc) {
                if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                    throw;
                }
            }

            var paymentMethod = newCustomer.AddPaymentMethod(PaymentId("Credit"), card);
            try {
                var response = paymentMethod.Create();
                Assert.IsNotNull(response);
                Assert.IsInstanceOfType(response, typeof(RecurringPaymentMethod));
            } catch (GatewayException exc) {
                if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                    throw;
                }
            }

            var scheduleFrequency = new[] {
                ScheduleFrequency.WEEKLY, ScheduleFrequency.BI_MONTHLY, ScheduleFrequency.MONTHLY,
                ScheduleFrequency.QUARTERLY, ScheduleFrequency.SEMI_ANNUALLY, ScheduleFrequency.ANNUALLY
            };

            foreach (var scheduleFreq in scheduleFrequency) {
                var scheduleId = GenerationUtils.GenerateScheduleId();
                try {
                    var response = paymentMethod.AddSchedule(scheduleId)
                        .WithStartDate(DateTime.Now)
                        .WithAmount(Amount)
                        .WithCurrency(Currency)
                        .WithFrequency(scheduleFreq)
                        .WithReprocessingCount(1)
                        .WithNumberOfPayments(12)
                        .WithCustomerNumber("E8953893489")
                        .WithOrderPrefix("gym")
                        .WithName("Gym Membership")
                        .WithDescription("Social Sign-Up")
                        .Create();

                    Assert.IsInstanceOfType(response, typeof(Schedule));
                    Assert.AreEqual("00", response.ResponseCode);
                    Assert.AreEqual("Schedule created successfully", response.ResponseMessage);

                    var schedule = RecurringService.Get<Schedule>(scheduleId);

                    Assert.AreEqual(scheduleId, schedule.Id);
                    Assert.AreEqual(12, schedule.NumberOfPayments);
                }
                catch (GatewayException exc) {
                    if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                        throw;
                    }
                }
            }
        }

        [TestMethod]
        public void CardStorageAddSchedule_WithIndefinitelyRun() {
            try {
                var response = newCustomer.Create();
                Assert.IsNotNull(response);
                Assert.IsInstanceOfType(response, typeof(Customer));
            } catch (GatewayException exc) {
                if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                    throw;
                }
            }

            var paymentMethod = newCustomer.AddPaymentMethod(PaymentId("Credit"), card);
            try {
                var response = paymentMethod.Create();
                Assert.IsNotNull(response);
                Assert.IsInstanceOfType(response, typeof(RecurringPaymentMethod));
            } catch (GatewayException exc) {
                if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                    throw;
                }
            }

            var scheduleId = GenerationUtils.GenerateScheduleId();
            try {
                var response = paymentMethod.AddSchedule(scheduleId)
                    .WithStartDate(DateTime.Now)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithFrequency(ScheduleFrequency.QUARTERLY)
                    .WithReprocessingCount(1)
                    .WithNumberOfPayments(-1)
                    .WithCustomerNumber("E8953893489")
                    .WithOrderPrefix("gym")
                    .WithName("Gym Membership")
                    .WithDescription("Social Sign-Up")
                    .Create();

                Assert.AreEqual("00", response.ResponseCode);
                Assert.AreEqual("Schedule created successfully", response.ResponseMessage);

                var schedule = RecurringService.Get<Schedule>(scheduleId);

                Assert.AreEqual(scheduleId, schedule.Id);
                Assert.AreEqual(-1, schedule.NumberOfPayments);
            }
            catch (GatewayException exc) {
                if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                    throw;
                }
            }
        }

        [TestMethod]
        public void CardStorageAddSchedule_With999Runs() {
            try {
                var response = newCustomer.Create();
                Assert.IsNotNull(response);
            } catch (GatewayException exc) {
                if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                    throw;
                }
            }

            var paymentMethod = newCustomer.AddPaymentMethod(PaymentId("Credit"), card);
            try {
                var response = paymentMethod.Create();
                Assert.IsNotNull(response);
            } catch (GatewayException exc) {
                if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                    throw;
                }
            }

            var scheduleId = GenerationUtils.GenerateScheduleId();
            try {
                var response = paymentMethod.AddSchedule(scheduleId)
                    .WithStartDate(DateTime.Now)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithFrequency(ScheduleFrequency.QUARTERLY)
                    .WithReprocessingCount(1)
                    .WithNumberOfPayments(999)
                    .WithCustomerNumber("E8953893489")
                    .WithOrderPrefix("gym")
                    .WithName("Gym Membership")
                    .WithDescription("Social Sign-Up")
                    .Create();

                Assert.AreEqual("00", response.ResponseCode);
                Assert.AreEqual("Schedule created successfully", response.ResponseMessage);

                var schedule = RecurringService.Get<Schedule>(scheduleId);

                Assert.AreEqual(scheduleId, schedule.Id);
                Assert.AreEqual(999, schedule.NumberOfPayments);
            } catch (GatewayException exc) {
                if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                    throw;
                }
            }
        }

        [TestMethod]
        public void CardStorageAddSchedule_WithoutScheduleRef() {
            var paymentMethod = newCustomer.AddPaymentMethod(PaymentId("Credit"), card);
            var exceptionCaught = false;

            try {
                paymentMethod.AddSchedule(null)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithReprocessingCount(1)
                    .WithNumberOfPayments(12)
                    .WithFrequency(ScheduleFrequency.QUARTERLY)
                    .WithCustomerNumber("E8953893489")
                    .WithOrderPrefix("gym")
                    .WithName("Gym Membership")
                    .WithDescription("Social Sign-Up")
                    .Create();
            } catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("502", e.ResponseCode);
                Assert.AreEqual(
                    "Unexpected Gateway Response: 502 - Mandatory Fields missing: [/request/scheduleref]. See Developers Guide",
                    e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardStorageAddSchedule_WithoutFrequency() {
            var paymentMethod = newCustomer.AddPaymentMethod(PaymentId("Credit"), card);
            var scheduleId = GenerationUtils.GenerateScheduleId();
            var exceptionCaught = false;

            try {
                paymentMethod.AddSchedule(scheduleId)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithReprocessingCount(1)
                    .WithNumberOfPayments(12)
                    .WithCustomerNumber("E8953893489")
                    .WithOrderPrefix("gym")
                    .WithName("Gym Membership")
                    .WithDescription("Social Sign-Up")
                    .Create();
            } catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("502", e.ResponseCode);
                Assert.AreEqual(
                    "Unexpected Gateway Response: 502 - Mandatory Fields missing: [/request/schedule]. See Developers Guide",
                    e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardStorageAddSchedule_WithoutCustomerRef() {
            var paymentMethod = newCustomer.AddPaymentMethod(PaymentId("Credit"), card);
            var scheduleId = GenerationUtils.GenerateScheduleId();

            paymentMethod.CustomerKey = null;

            var exceptionCaught = false;

            try {
                paymentMethod.AddSchedule(scheduleId)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithFrequency(ScheduleFrequency.QUARTERLY)
                    .WithReprocessingCount(1)
                    .WithNumberOfPayments(12)
                    .WithCustomerNumber("E8953893489")
                    .WithOrderPrefix("gym")
                    .WithName("Gym Membership")
                    .WithDescription("Social Sign-Up")
                    .Create();
            } catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("502", e.ResponseCode);
                Assert.AreEqual(
                    "Unexpected Gateway Response: 502 - Mandatory Fields missing: [/request/payerref]. See Developers Guide",
                    e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardStorageAddSchedule_WithoutPaymentMethod() {
            var paymentMethod = newCustomer.AddPaymentMethod(null, card);
            var scheduleId = GenerationUtils.GenerateScheduleId();

            var exceptionCaught = false;

            try {
                paymentMethod.AddSchedule(scheduleId)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithFrequency(ScheduleFrequency.QUARTERLY)
                    .WithReprocessingCount(1)
                    .WithNumberOfPayments(12)
                    .WithCustomerNumber("E8953893489")
                    .WithOrderPrefix("gym")
                    .WithName("Gym Membership")
                    .WithDescription("Social Sign-Up")
                    .Create();
            } catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("502", e.ResponseCode);
                Assert.AreEqual(
                    "Unexpected Gateway Response: 502 - Mandatory Fields missing: [/request/paymentmethod]. See Developers Guide",
                    e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardStorageAddSchedule_WithoutAmount() {
            var paymentMethod = newCustomer.AddPaymentMethod(PaymentId("Credit"), card);
            var scheduleId = GenerationUtils.GenerateScheduleId();
            var exceptionCaught = false;

            try {
                paymentMethod.AddSchedule(scheduleId)
                    .WithCurrency(Currency)
                    .WithFrequency(ScheduleFrequency.QUARTERLY)
                    .WithReprocessingCount(1)
                    .WithNumberOfPayments(12)
                    .WithCustomerNumber("E8953893489")
                    .WithOrderPrefix("gym")
                    .WithName("Gym Membership")
                    .WithDescription("Social Sign-Up")
                    .Create();
            } catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("502", e.ResponseCode);
                Assert.AreEqual(
                    "Unexpected Gateway Response: 502 - Mandatory Fields missing: [/request/amount/@currency]. See Developers Guide",
                    e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardStorageAddSchedule_WithoutCurrency() {
            var paymentMethod = newCustomer.AddPaymentMethod(PaymentId("Credit"), card);
            var scheduleId = GenerationUtils.GenerateScheduleId();
            var exceptionCaught = false;

            try {
                paymentMethod.AddSchedule(scheduleId)
                    .WithAmount(Amount)
                    .WithFrequency(ScheduleFrequency.QUARTERLY)
                    .WithReprocessingCount(1)
                    .WithNumberOfPayments(12)
                    .WithCustomerNumber("E8953893489")
                    .WithOrderPrefix("gym")
                    .WithName("Gym Membership")
                    .WithDescription("Social Sign-Up")
                    .Create();
            } catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("506", e.ResponseCode);
                Assert.IsTrue(e.Message.Contains("currency=\"\"] ' does not conform to the schema"));
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardStorageAddSchedule_WithoutNumberOfPayments() {
            var paymentMethod = newCustomer.AddPaymentMethod(PaymentId("Credit"), card);
            var scheduleId = GenerationUtils.GenerateScheduleId();
            var exceptionCaught = false;

            try {
                paymentMethod.AddSchedule(scheduleId)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithFrequency(ScheduleFrequency.QUARTERLY)
                    .Create();
            } catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("502", e.ResponseCode);
                Assert.AreEqual(
                    "Unexpected Gateway Response: 502 - Mandatory Fields missing: [/request/numtimes]. See Developers Guide",
                    e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardStorageAddSchedule_WithNumberOfPaymentsInvalid() {
            try {
                var response = newCustomer.Create();
                Assert.IsNotNull(response);
                Assert.IsInstanceOfType(response, typeof(Customer));
            } catch (GatewayException exc) {
                if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                    throw;
                }
            }

            var paymentMethod = newCustomer.AddPaymentMethod(PaymentId("Credit"), card);
            try {
                var response = paymentMethod.Create();
                Assert.IsNotNull(response);
                Assert.IsInstanceOfType(response, typeof(RecurringPaymentMethod));
            } catch (GatewayException exc) {
                if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                    throw;
                }
            }

            var exceptionCaught = false;
            var scheduleId = GenerationUtils.GenerateScheduleId();
            try {
                paymentMethod.AddSchedule(scheduleId)
                    .WithStartDate(DateTime.Now)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithFrequency(ScheduleFrequency.QUARTERLY)
                    .WithReprocessingCount(1)
                    .WithNumberOfPayments(1000)
                    .Create();
            } catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual(
                    "Unexpected Gateway Response: 535 - Invalid value, numtimes cannot be greater than 999.",
                    e.Message);
                Assert.AreEqual("535", e.ResponseCode);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardStorageAddSchedule_WithNumberOfPaymentsZero() {
            try {
                var response = newCustomer.Create();
                Assert.IsNotNull(response);
                Assert.IsInstanceOfType(response, typeof(Customer));
            } catch (GatewayException exc) {
                if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                    throw;
                }
            }

            var paymentMethod = newCustomer.AddPaymentMethod(PaymentId("Credit"), card);
            try {
                var response = paymentMethod.Create();
                Assert.IsNotNull(response);
                Assert.IsInstanceOfType(response, typeof(RecurringPaymentMethod));
            } catch (GatewayException exc) {
                if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                    throw;
                }
            }

            var exceptionCaught = false;
            var scheduleId = GenerationUtils.GenerateScheduleId();
            try {
                paymentMethod.AddSchedule(scheduleId)
                    .WithStartDate(DateTime.Now)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithFrequency(ScheduleFrequency.QUARTERLY)
                    .WithReprocessingCount(1)
                    .WithNumberOfPayments(0)
                    .Create();
            } catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual(
                    "Unexpected Gateway Response: 535 - Invalid value, numtimes cannot be greater than 999.",
                    e.Message);
                Assert.AreEqual("535", e.ResponseCode);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void GetListOfPaymentSchedules() {
            try {
                var response = newCustomer.Create();
                Assert.IsNotNull(response);
            } catch (GatewayException exc) {
                if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                    throw;
                }
            }

            var paymentMethod = newCustomer.AddPaymentMethod(PaymentId("Credit"), card);
            try {
                var response = paymentMethod.Create();
                Assert.IsNotNull(response);
            } catch (GatewayException exc) {
                if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                    throw;
                }
            }

            for (var i = 0; i < 3; i++) {
                var scheduleId = GenerationUtils.GenerateScheduleId();
                try {
                    var response = paymentMethod.AddSchedule(scheduleId)
                        .WithStartDate(DateTime.Now)
                        .WithAmount(Amount)
                        .WithCurrency(Currency)
                        .WithFrequency(ScheduleFrequency.SEMI_ANNUALLY)
                        .WithReprocessingCount(1)
                        .WithNumberOfPayments(12)
                        .WithCustomerNumber("E8953893489")
                        .WithOrderPrefix("gym")
                        .WithName("Gym Membership")
                        .WithDescription("Social Sign-Up")
                        .Create();

                    Assert.IsInstanceOfType(response, typeof(Schedule));
                    Assert.AreEqual("00", response.ResponseCode);
                    Assert.AreEqual("Schedule created successfully", response.ResponseMessage);
                } catch (GatewayException exc) {
                    if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                        throw;
                    }
                }
            }

            var schedules = RecurringService.Search<List<Schedule>>()
                .AddSearchCriteria(SearchCriteria.PaymentMethodKey.ToString(), PaymentId("Credit"))
                .AddSearchCriteria(SearchCriteria.CustomerId.ToString(), newCustomer.Key)
                .Execute();

            Assert.IsNotNull(schedules);

            foreach (var schedule in schedules) {
                Assert.IsNotNull(schedule.Key);
            }
        }

        [TestMethod]
        public void GetListOfPaymentSchedules_RandomDetails() {
            var customerId = Guid.NewGuid().ToString();
            try {
                var schedules = RecurringService.Search<List<Schedule>>()
                    .AddSearchCriteria(SearchCriteria.PaymentMethodKey.ToString(), PaymentId("Credit"))
                    .AddSearchCriteria(SearchCriteria.CustomerId.ToString(), customerId)
                    .Execute();

                Assert.IsNull(schedules);
            } catch (GatewayException e) {
                Assert.AreEqual("520", e.ResponseCode);
                Assert.AreEqual($"Unexpected Gateway Response: 520 - This Payer Ref [{customerId}] does not exist",
                    e.Message);
            }
        }

        [TestMethod]
        public void GetListOfPaymentSchedules_WithoutPayer() {
            try {
                var response = RecurringService.Search<List<Schedule>>()
                    .AddSearchCriteria(SearchCriteria.PaymentMethodKey.ToString(), PaymentId("Credit"))
                    .Execute();

                Assert.IsNull(response);
            } catch (GatewayException e) {
                Assert.AreEqual("502", e.ResponseCode);
                Assert.AreEqual(
                    $"Unexpected Gateway Response: 502 - Mandatory Fields missing: [/request/payerref]. See Developers Guide",
                    e.Message);
            }
        }

        [TestMethod]
        public void GetListOfPaymentSchedules_WithoutPaymentMethod() {
            try {
                var response = RecurringService.Search<List<Schedule>>()
                    .AddSearchCriteria(SearchCriteria.CustomerId.ToString(), CustomerId)
                    .Execute();

                Assert.IsNull(response);
            } catch (GatewayException e) {
                Assert.AreEqual("502", e.ResponseCode);
                Assert.AreEqual(
                    $"Unexpected Gateway Response: 502 - Mandatory Fields missing: [/request/paymentmethod]. See Developers Guide",
                    e.Message);
            }
        }

        [TestMethod]
        public void DeleteSchedule() {
            try {
                var response = newCustomer.Create();
                Assert.IsNotNull(response);
            } catch (GatewayException exc) {
                if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                    throw;
                }
            }

            var paymentMethod = newCustomer.AddPaymentMethod(PaymentId("Credit"), card);
            try {
                var response = paymentMethod.Create();
                Assert.IsNotNull(response);
                //Assert.AreEqual("00", response.ResponseCode);
            } catch (GatewayException exc) {
                if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                    throw;
                }
            }

            var scheduleId = GenerationUtils.GenerateScheduleId();
            try {
                var responseSchedule = paymentMethod.AddSchedule(scheduleId)
                    .WithStartDate(DateTime.Now)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithFrequency(ScheduleFrequency.SEMI_ANNUALLY)
                    .WithReprocessingCount(1)
                    .WithNumberOfPayments(12)
                    .WithCustomerNumber("E8953893489")
                    .WithOrderPrefix("gym")
                    .WithName("Gym Membership")
                    .WithDescription("Social Sign-Up")
                    .Create();

                Assert.IsInstanceOfType(responseSchedule, typeof(Schedule));
                Assert.AreEqual("00", responseSchedule.ResponseCode);
                Assert.AreEqual("Schedule created successfully", responseSchedule.ResponseMessage);
            } catch (GatewayException exc) {
                if (exc.ResponseCode != "501" && exc.ResponseCode != "520") {
                    throw;
                }
            }

            var schedules = RecurringService.Search<List<Schedule>>()
                .AddSearchCriteria(SearchCriteria.PaymentMethodKey.ToString(), PaymentId("Credit"))
                .AddSearchCriteria(SearchCriteria.CustomerId.ToString(), newCustomer.Key)
                .Execute();

            Assert.IsNotNull(schedules);
            var schedule = schedules[0];

            schedule.Delete();
            Assert.AreEqual("00", schedule.ResponseCode);
            Assert.AreEqual("OK", schedule.ResponseMessage);
        }

        [TestMethod]
        public void Delete_RandomSchedule() {
            var schedule = new Schedule {
                Key = GenerationUtils.GenerateScheduleId()
            };
            var exceptionCaught = false;

            try {
                schedule.Delete();
            } catch (ApiException e) {
                exceptionCaught = true;
                Assert.AreEqual("Failed to delete record, see inner exception for more details", e.Message);
                Assert.AreEqual("Unexpected Gateway Response: 508 - The Scheduled Payment does not exist.",
                    e.InnerException?.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void GetPaymentScheduleById() {
            var schedule = new Schedule {
                Key = "bopinslfouil39vfmkqg"
            };
            schedule = RecurringService.Get<Schedule>(schedule.Key);

            Assert.IsNotNull(schedule);
            Assert.AreEqual(schedule.Id, schedule.Key);
            Assert.IsNotNull(schedule.StartDate);
        }

        [TestMethod]
        public void GetPaymentScheduleById_RandomId() {
            var schedule = new Schedule {
                Key = GenerationUtils.GenerateScheduleId()
            };

            try {
                var response = RecurringService.Get<Schedule>(schedule.Key);

                Assert.IsNotNull(response);
                Assert.IsNull(response.Key);
            } catch (ApiException e) {
                Assert.AreEqual("Unexpected Gateway Response: 508 - The Scheduled Payment does not exist.", e.Message);
            }
        }

        [TestMethod]
        public void GetPaymentScheduleById_NullId() {
            try {
                RecurringService.Get<Schedule>(null);
            } catch (ApiException e) {
                Assert.AreEqual("Key cannot be null for this transaction type.", e.Message);
            }
        }
        
        #endregion
    }
}