using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GlobalPayments.Api.Tests.BillPay {
    [TestClass]
    public class BillPayTests {
        eCheck ach;
        CreditCardData clearTextCredit;
        Address address;
        Customer customer;
        Bill bill;
        Bill[] bills;
        Bill billLoad;
        Bill blindBill;

        [TestInitialize]
        public void Init() {

            ServicesContainer.ConfigureService(new BillPayConfig
            {
                MerchantName = "Dev_Exp_Team_Merchant",
                Username = "DevExpTeam",
                Password = "devexpteam_R0cks!",
                ServiceUrl = ServiceEndpoints.BILLPAY_CERTIFICATION
            });

            ach = new eCheck() {
                AccountNumber = "12345",
                RoutingNumber = "064000017",
                AccountType = AccountType.CHECKING,
                CheckType = CheckType.BUSINESS,
                SecCode = "WEB",
                CheckHolderName = "Test Tester",
                BankName = "Regions"
            };

            clearTextCredit = new CreditCardData() {
                Number = "4444444444444448",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year,
                Cvn = "123",
                CardHolderName = "Test Tester",
            };

            address = new Address() {
                StreetAddress1 = "1234 Test St",
                StreetAddress2 = "Apt 201",
                City = "Auburn",
                State = "AL",
                Country = "US",
                PostalCode = "12345"
            };

            customer = new Customer() {
                Address = address,
                Email = "testemailaddress@e-hps.com",
                FirstName = "Test",
                LastName = "Tester",
                HomePhone = "555-555-4444",
                Company = "Test Company",
                MiddleName = "Testing",
            };

            bill = new Bill() {
                Amount = 50M,
                Identifier1 = "12345"
            };

            bills = new Bill[] {
                new Bill() {
                    BillType = "Tax Payments",
                    Identifier1 = "123",
                    Amount = 10M
                },
                new Bill() {
                    BillType = "Tax Payments",
                    Identifier1 = "321",
                    Amount = 10M
                }
            };

            billLoad = new Bill() {
                Amount = 50M,
                BillType = "Tax Payments",
                Identifier1 = "12345",
                Identifier2 = "23456",
                BillPresentment = BillPresentment.Full,
                DueDate = DateTime.Now.AddDays(3),
                Customer = customer
            };

            blindBill = new Bill() {
                Amount = 50M,
                BillType = "Tax Payments",
                Identifier1 = "12345",
                Identifier2 = "23456",
                BillPresentment = BillPresentment.Full,
                DueDate = DateTime.Now.AddDays(1),
                Customer = customer
            };
        }

        #region Authorization Builder Cases

        [TestMethod]
        public void Charge_WithSingleBill_ReturnsSuccessfulTransaction() {
            var service = new BillPayService();
            var fee = service.CalculateConvenienceAmount(clearTextCredit, bill.Amount);

            RunAndValidateTransaction(() => {
                return clearTextCredit
                    .Charge(bill.Amount)
                    .WithAddress(address)
                    .WithBills(bill)
                    .WithConvenienceAmount(fee)
                    .WithCurrency("USD")
                    .Execute();
            });
        }

        [TestMethod]
        public void Charge_WithMultipleBills_ReturnsSuccessfulTransaction() {
            var service = new BillPayService();
            var totalAmount = bills.Sum(x => x.Amount);
            var fee = service.CalculateConvenienceAmount(clearTextCredit, totalAmount);

            RunAndValidateTransaction(() => {
                return clearTextCredit
                    .Charge(totalAmount)
                    .WithAddress(address)
                    .WithBills(bills)
                    .WithConvenienceAmount(fee)
                    .WithCurrency("USD")
                    .Execute();
            });
        }

        [TestMethod]
        public void Tokenize_UsingCreditCard_ReturnsTokenInformation() {
            var tokenResponse = clearTextCredit.Verify()
                .WithAddress(address)
                .WithCustomerData(customer)
                .WithRequestMultiUseToken(true)
                .Execute();

           clearTextCredit.Token = tokenResponse.Token;
           var tokenInfoResponse = clearTextCredit.GetTokenInformation();

           Assert.IsNotNull(tokenInfoResponse);
           Assert.IsNotNull(tokenInfoResponse.TokenData);
        }

        [TestMethod]
        public void Tokenize_UsingCreditCard_ReturnsCardType()
        {
            string cardTypeVisa = "VISA";
            string cardTypeDiscover = "DISC";
            string cardTypeMasterCard = "MC";
            string cardTypeAmericanExpress = "AMEX";

            var clearTextCreditVisa = new CreditCardData()
            {
                Number = "4444444444444448",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year,
                Cvn = "123",
                CardHolderName = "Test Tester",
            };

            var clearTextCreditDiscover = new CreditCardData()
            {
                Number = "6011000000000087",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year,
                Cvn = "123",
                CardHolderName = "Test Tester",
            };

            var clearTextCreditMasterCard = new CreditCardData()
            {
                Number = "5425230000004415",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year,
                Cvn = "123",
                CardHolderName = "Test Tester",
            };

            var clearTextCreditAmericanExpress = new CreditCardData()
            {
                Number = "374101000000608",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year,
                Cvn = "123",
                CardHolderName = "Test Tester",
            };

            // VISA
            var tokenResponseVisa = clearTextCreditVisa.Verify()
                .WithAddress(address)
                .WithCustomerData(customer)
                .WithRequestMultiUseToken(true)
                .Execute();

            clearTextCreditVisa.Token = tokenResponseVisa.Token;
            var tokenInfoResponseVisa = clearTextCreditVisa.GetTokenInformation();

            Assert.IsNotNull(tokenInfoResponseVisa);
            Assert.IsNotNull(tokenInfoResponseVisa.TokenData);
            Assert.AreEqual(cardTypeVisa, tokenInfoResponseVisa.CardType);

            //Discover
            var tokenResponseDiscover = clearTextCreditDiscover.Verify()
                .WithAddress(address)
                .WithCustomerData(customer)
                .WithRequestMultiUseToken(true)
                .Execute();

            clearTextCreditDiscover.Token = tokenResponseDiscover.Token;
            var tokenInfoResponseDiscover = clearTextCreditDiscover.GetTokenInformation();

            Assert.IsNotNull(tokenInfoResponseDiscover);
            Assert.IsNotNull(tokenInfoResponseDiscover.TokenData);
            Assert.AreEqual(cardTypeDiscover, tokenInfoResponseDiscover.CardType);

            //Master Card
            var tokenResponseMasterCard = clearTextCreditMasterCard.Verify()
                .WithAddress(address)
                .WithCustomerData(customer)
                .WithRequestMultiUseToken(true)
                .Execute();

            clearTextCreditMasterCard.Token = tokenResponseMasterCard.Token;
            var tokenInfoResponseMasterCard = clearTextCreditMasterCard.GetTokenInformation();

            Assert.IsNotNull(tokenInfoResponseMasterCard);
            Assert.IsNotNull(tokenInfoResponseMasterCard.TokenData);
            Assert.AreEqual(cardTypeMasterCard, tokenInfoResponseMasterCard.CardType);

            //America Express
            var tokenResponseAmericanExpress = clearTextCreditAmericanExpress.Verify()
                .WithAddress(address)
                .WithCustomerData(customer)
                .WithRequestMultiUseToken(true)
                .Execute();

            clearTextCreditAmericanExpress.Token = tokenResponseAmericanExpress.Token;
            var tokenInfoResponseAmericanExpress = clearTextCreditAmericanExpress.GetTokenInformation();

            Assert.IsNotNull(tokenInfoResponseAmericanExpress);
            Assert.IsNotNull(tokenInfoResponseAmericanExpress.TokenData);
            Assert.AreEqual(cardTypeAmericanExpress, tokenInfoResponseAmericanExpress.CardType);
        }


        [TestMethod]
        public void Tokenize_UsingCreditCard_ReturnsToken() {
            var response = clearTextCredit.Verify()
                .WithAddress(new Address { PostalCode = "12345" })
                .WithRequestMultiUseToken(true)
                .Execute();

            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Token));
        }

        [TestMethod]
        public void Tokenize_UsingCreditCard_WithCustomerData_ReturnsToken() {
            var token = clearTextCredit.Tokenize(true, address, customer);
            Assert.IsFalse(string.IsNullOrWhiteSpace(token));

            // Tokenize via Verify() call
            //var response = clearTextCredit.Verify()
            //    // Billing address will be prioritized if specified separately
            //    .WithAddress(address)
            //    // or Customer.Address will be used as a backup if a billing address isn't provided via builder method
            //    .WithCustomerData(customer)
            //    .WithRequestMultiUseToken(true)
            //    .Execute();

            //Assert.IsFalse(string.IsNullOrWhiteSpace(response.Token));
        }

        [TestMethod]
        public void UpdateTokenExpiry_UsingCreditCardToken_DoesNotThrow() {
            var response = clearTextCredit.Verify()
                .WithAddress(new Address { PostalCode = "12345" })
                .WithRequestMultiUseToken(true)
                .Execute();

            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Token));

            try {
                clearTextCredit.Token = response.Token;
                clearTextCredit.ExpMonth = 12;
                clearTextCredit.ExpYear = 2022;

                clearTextCredit.UpdateTokenExpiry();
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void Tokenize_UsingACH_ReturnsToken() {
            var token = ach.Tokenize();

            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
        }

        [TestMethod]
        public void Tokenize_UsingACH_WithCustomerData_ReturnsToken() {
            var token = ach.Tokenize(true, address, customer);

            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
        }

        [TestMethod]
        public void Tokenize_UsingACH_ReturnsTokenInformation() {
            var token = ach.Tokenize();

            ach.Token = token;

            var tokenInfoResponse = ach.GetTokenInformation();

            Assert.IsNotNull(tokenInfoResponse);
            Assert.IsNotNull(tokenInfoResponse.TokenData);
        }

        [TestMethod]
        public void Charge_UsingTokenizedCreditCard_ReturnsSuccessfulTransaction() {
            var service = new BillPayService();
            var response = clearTextCredit.Verify()
                .WithAddress(new Address { PostalCode = "12345" })
                .WithRequestMultiUseToken(true)
                .Execute();

            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Token));

            var token = response.Token;
            var fee = service.CalculateConvenienceAmount(clearTextCredit, bill.Amount);

            var paymentMethod = new CreditCardData() {
                Token = token,
                ExpMonth = clearTextCredit.ExpMonth,
                ExpYear = clearTextCredit.ExpYear
            };

            Assert.IsFalse(string.IsNullOrWhiteSpace(token));

            RunAndValidateTransaction(() => {
                return paymentMethod
                    .Charge(bill.Amount)
                    .WithAddress(address)
                    .WithBills(bill)
                    .WithConvenienceAmount(fee)
                    .WithCurrency("USD")
                    .Execute();
            });
        }

        [TestMethod]
        public void Charge_UsingSingleUseToken_ReturnsSuccessfulTransaction() {
            var service = new BillPayService();
            var fee = service.CalculateConvenienceAmount(clearTextCredit, bill.Amount);

            var paymentMethod = new CreditCardData()
            {
                Token = "ENTER SINGLE USE TOKEN VALUE",
                ExpMonth = 01,
                ExpYear = 2025
            };

            RunAndValidateTransaction(() => {
                return paymentMethod
                    .Charge(bill.Amount)
                    .WithAddress(address)
                    .WithBills(bill)
                    .WithConvenienceAmount(fee)
                    .WithCurrency("USD")
                    .WithPaymentMethodUsageMode(PaymentMethodUsageMode.Single)
                    .Execute();
            });
        }

        [TestMethod]
        public void Charge_UsingSingleUseToken_ReturnsSuccessfulTransactionWithToken()
        {
            var service = new BillPayService();
            var fee = service.CalculateConvenienceAmount(clearTextCredit, bill.Amount);

            var paymentMethod = new CreditCardData()
            {
                Token = "ENTER SINGLE USE TOKEN VALUE",
                ExpMonth = 01,
                ExpYear = 25
            };

            var transaction = RunAndValidateTransaction(() => {
                return paymentMethod
                    .Charge(bill.Amount)
                    .WithAddress(address)
                    .WithBills(bill)
                    .WithConvenienceAmount(fee)
                    .WithCurrency("USD")
                    .WithRequestMultiUseToken(true)
                    .WithPaymentMethodUsageMode(PaymentMethodUsageMode.Single)
                    .Execute();
            });

            Assert.IsNotNull(transaction.Token);
        }

        [TestMethod]
        public void Charge_UsingTokenizedACH_ReturnsSuccessfulTransaction() {
            var service = new BillPayService();
            var result = ach.Tokenize();
            var fee = service.CalculateConvenienceAmount(ach, bill.Amount);
            var paymentMethod = new eCheck() {
                AccountType = AccountType.CHECKING,
                CheckType = CheckType.BUSINESS,
                SecCode = "WEB",
                CheckHolderName = "Tester",
                Token = result
            };

            Assert.IsFalse(string.IsNullOrWhiteSpace(result));

            RunAndValidateTransaction(() => {
                return paymentMethod
                    .Charge(bill.Amount)
                    .WithBills(bill)
                    .WithConvenienceAmount(fee)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .Execute();
            });
        }

        [TestMethod]
        public void Charge_UsingTokenFromPreviousPayment_ReturnsSuccessfulTransaction() {
            var service = new BillPayService();
            var fee = service.CalculateConvenienceAmount(clearTextCredit, bill.Amount);

            var transaction = RunAndValidateTransaction(() => {
                return clearTextCredit
                    .Charge(bill.Amount)
                    .WithAddress(address)
                    .WithBills(bill)
                    .WithConvenienceAmount(fee)
                    .WithCurrency("USD")
                    .WithRequestMultiUseToken(true)
                    .Execute();
            });

            Assert.IsFalse(string.IsNullOrWhiteSpace(transaction.Token));

            var transaction2 = RunAndValidateTransaction(() => {
                var tokenizedCard = new CreditCardData() {
                    Token = transaction.Token,
                    ExpYear = clearTextCredit.ExpYear,
                    ExpMonth = clearTextCredit.ExpMonth
                };
                return tokenizedCard.Charge(bill.Amount)
                    .WithBills(bill)
                    .WithConvenienceAmount(fee)
                    .WithCurrency("USD")
                    .Execute();
            });
        }

        [TestMethod]
        public void Charge_WithoutAddingBills_ThrowsValidationException() {
            Assert.ThrowsException<ValidationException>(() => {
                clearTextCredit
                    .Charge(50M)
                    .WithCurrency("USD")
                    .WithConvenienceAmount(3M)
                    .Execute();
            });
        }

        [TestMethod]
        public void Charge_WithMismatchingAmounts_ThrowsValidationException() {
            Assert.ThrowsException<ValidationException>(() => {
                clearTextCredit
                    .Charge(60M)
                    .WithBills(bill)
                    .WithCurrency("USD")
                    .Execute();
            });
        }

        #endregion

        #region Management Builder Cases

        [TestMethod]
        public void ReversePayment_WithPreviousTransaction_ReturnsSuccessfulTransaction() {
            var service = new BillPayService();
            var fee = service.CalculateConvenienceAmount(clearTextCredit, bill.Amount);

            // Make transaction to reverse
            var transaction = RunAndValidateTransaction(() => {
                return clearTextCredit
                    .Charge(bill.Amount)
                    .WithAddress(address)
                    .WithBills(bill)
                    .WithConvenienceAmount(fee)
                    .WithCurrency("USD")
                    .Execute();
            });

            // Now reverse it
            var reversal = RunAndValidateTransaction(() => {
                return Transaction.FromId(transaction.TransactionId)
                    .Reverse(bill.Amount)
                    .WithConvenienceAmount(fee)
                    .Execute();
            });
        }

        [TestMethod]
        public void ReversePayment_WithPreviousMultiBillTransaction_ReturnsSuccessfulTransaction() {
            var service = new BillPayService();
            var totalAmount = bills.Sum(x => x.Amount);
            var fee = service.CalculateConvenienceAmount(clearTextCredit, totalAmount);

            // Make transaction to reverse
            var transaction = RunAndValidateTransaction(() => {
                return clearTextCredit
                    .Charge(totalAmount)
                    .WithAddress(address)
                    .WithBills(bills)
                    .WithConvenienceAmount(fee)
                    .WithCurrency("USD")
                    .Execute();
            });

            // Now reverse it
            var reversal = RunAndValidateTransaction(() => {
                return Transaction.FromId(transaction.TransactionId)
                    .Reverse(totalAmount)
                    .WithConvenienceAmount(fee)
                    .Execute();
            });
        }

        [TestMethod]
        public void PartialReversal_WithCreditCard_ReturnsSuccessfulTransaction() {
            var service = new BillPayService();
            var totalAmount = bills.Sum(x => x.Amount);
            var fee = service.CalculateConvenienceAmount(clearTextCredit, totalAmount);

            // Make transaction to reverse
            var transaction = RunAndValidateTransaction(() => {
                return clearTextCredit
                    .Charge(totalAmount)
                    .WithAddress(address)
                    .WithBills(bills)
                    .WithPaymentMethod(clearTextCredit)
                    .WithConvenienceAmount(fee)
                    .WithCurrency("USD")
                    .Execute();
            });

            // Now reverse it
            var reversal = RunAndValidateTransaction(() => {
                var billsToPariallyReverse = bills.Select(x => new Bill {
                    BillType = x.BillType,
                    Identifier1 = x.Identifier1,
                    Amount = x.Amount - 5
                }).ToArray();

                var newFees = service.CalculateConvenienceAmount(clearTextCredit, totalAmount - 10);

                return Transaction.FromId(transaction.TransactionId)
                    .Reverse(totalAmount - 10)
                    .WithBills(billsToPariallyReverse)
                    .WithConvenienceAmount(fee - newFees)
                    .Execute();
            });
        }

        #endregion

        #region Billing Builder Cases

        [TestMethod]
        public void LoadHostedPayment_WithMakePaymentType_ReturnsIdentifier() {
            var service = new BillPayService();
            var data = new HostedPaymentData() {
                Bills = new List<Bill>() { blindBill },
                CustomerAddress = new Address { StreetAddress1 = "123 Drive", PostalCode = "12345" },
                CustomerEmail = "test@tester.com",
                CustomerFirstName = "Test",
                CustomerLastName = "Tester",
                HostedPaymentType = HostedPaymentType.MakePayment
            };
            var response = service.LoadHostedPayment(data);

            Assert.IsTrue(!string.IsNullOrEmpty(response.PaymentIdentifier));
        }

        [TestMethod]
        public void LoadHostedPayment_WithMakePaymentReturnToken_ReturnsIdentifier() {
            var service = new BillPayService();
            var hostedPaymentData = new HostedPaymentData() {
                Bills = new List<Bill>() { blindBill },
                CustomerAddress = new Address {
                    StreetAddress1 = "123 Drive",
                    City = "Auburn",
                    State = "AL",
                    PostalCode = "36830",
                    CountryCode = "US",
                },
                CustomerEmail = "test@tester.com",
                CustomerFirstName = "Test",
                CustomerLastName = "Tester",
                CustomerPhoneMobile = "800-555-5555",
                CustomerIsEditable = true,
                HostedPaymentType = HostedPaymentType.MakePaymentReturnToken
            };
            var response = service.LoadHostedPayment(hostedPaymentData);

            Assert.IsTrue(!string.IsNullOrEmpty(response.PaymentIdentifier));
        }

        [TestMethod]
        public void LoadHostedPayment_WithoutBills_ThrowsValidationException() {
            var service = new BillPayService();
            var hostedPaymentData = new HostedPaymentData() {
                CustomerAddress = new Address { StreetAddress1 = "123 Drive" },
                CustomerEmail = "alexander.molbert@e-hps.com",
                CustomerFirstName = "Alex",
                HostedPaymentType = HostedPaymentType.MakePayment
            };

            Assert.ThrowsException<ValidationException>(() => {
                var response = service.LoadHostedPayment(hostedPaymentData);
            });
        }

        [TestMethod]
        public void LoadHostedPayment_WithoutPaymentType_ThrowsValidationException() {
            var service = new BillPayService();
            var hostedPaymentData = new HostedPaymentData() {
                Bills = new List<Bill>() { blindBill },
                CustomerAddress = new Address { StreetAddress1 = "123 Drive" },
                CustomerEmail = "alexander.molbert@e-hps.com",
                CustomerFirstName = "Alex",
            };

            Assert.ThrowsException<ValidationException>(() => {
                var response = service.LoadHostedPayment(hostedPaymentData);
            });
        }

        [TestMethod]
        public void Load_WithOneBill_DoesNotThrow() {
            try {
                var service = new BillPayService();

                service.LoadBills(new List<Bill>() { billLoad });
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void Load_WithOneThousandBills_DoesNotThrow() {
            try {
                var service = new BillPayService();

                service.LoadBills(Enumerable.Range(0, 1000).Select(x => new Bill() {
                    Amount = billLoad.Amount,
                    BillPresentment = billLoad.BillPresentment,
                    BillType = billLoad.BillType,
                    Customer = billLoad.Customer,
                    DueDate = billLoad.DueDate,
                    Identifier1 = x.ToString(),
                    Identifier2 = x.ToString()
                }));
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void Load_WithFiveThousandBills_DoesNotThrow() {
            try {
                var service = new BillPayService();

                service.LoadBills(Enumerable.Range(0, 5000).Select(x => new Bill() {
                    Amount = billLoad.Amount,
                    BillPresentment = billLoad.BillPresentment,
                    BillType = billLoad.BillType,
                    Customer = billLoad.Customer,
                    DueDate = billLoad.DueDate,
                    Identifier1 = x.ToString(),
                    Identifier2 = x.ToString()
                }));
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void Load_WithDuplicateBills_ThrowsGatewayException() {
            Assert.ThrowsException<GatewayException>(() => {
                var service = new BillPayService();
                var bills = new List<Bill>() {
                    billLoad,
                    billLoad
                };

                service.LoadBills(bills);
            });
        }

        [TestMethod]
        public void Load_WithInvalidBillType_ThrowsGatewayException() {
            Assert.ThrowsException<GatewayException>(() => {
                var service = new BillPayService();
                var bills = new List<Bill>() {
                    billLoad,
                    new Bill() {
                        Amount = billLoad.Amount,
                        BillPresentment = billLoad.BillPresentment,
                        BillType = "InvalidBillType",
                        Customer = billLoad.Customer,
                        DueDate = billLoad.DueDate,
                        Identifier1 = billLoad.Identifier1
                    }
                };

                service.LoadBills(bills);
            });
        }

        #endregion

        #region Recurring Builder Cases

        [TestMethod]
        public void Create_Customer_ReturnsCustomer() {
            try {
                customer = new Customer() {
                    FirstName = "IntegrationCreate",
                    LastName = "Customer",
                    Email = "test.test@test.com",
                    Id = Guid.NewGuid().ToString()
                }.Create();

                Assert.AreEqual("IntegrationCreate", customer.FirstName);
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void Update_Customer_ReturnsCustomer() {
            try {
                customer = new Customer() {
                    FirstName = "IntegrationUpdate",
                    LastName = "Customer",
                    Email = "test.test@test.com",
                    Id = Guid.NewGuid().ToString()
                }.Create();

                Assert.AreEqual("IntegrationUpdate", customer.FirstName);

                customer.FirstName = "Updated";

                customer.SaveChanges();

                Assert.AreEqual("Updated", customer.FirstName);
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void Delete_Customer_ReturnsCustomer() {
            var id = Guid.NewGuid().ToString();

            try {
                customer = new Customer() {
                    FirstName = "IntegrationDelete",
                    LastName = "Customer",
                    Email = "test.test@test.com",
                    Id = id
                }.Create();

                Assert.AreEqual("IntegrationDelete", customer.FirstName);

                customer.Delete();

                // Bill pay currently does not support retrieval of customer, so there is no true
                // way to validate the customer was deleted other than no exception was thrown
                Assert.AreEqual("IntegrationDelete", customer.FirstName);
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void Create_CustomerAccount_ReturnsPaymentMethod() {
            try {
                var customer = new Customer() {
                    FirstName = "Integration",
                    LastName = "Customer",
                    Email = "test.test@test.com",
                    Id = Guid.NewGuid().ToString()
                }.Create();

                var paymentMethod = customer.AddPaymentMethod(Guid.NewGuid().ToString(), clearTextCredit).Create();

                Assert.IsFalse(string.IsNullOrWhiteSpace(paymentMethod.Key));
            } catch (Exception ex) {
                Assert.Fail((ex.InnerException ?? ex).Message);
            }
        }

        [TestMethod]
        public void Update_CustomerAccount_ReturnsSuccess() {
            try {
                var customer = new Customer() {
                    FirstName = "Account",
                    LastName = "Update",
                    Email = "account.update@test.com",
                    Id = Guid.NewGuid().ToString()
                }.Create();

                var paymentMethod = customer.AddPaymentMethod(Guid.NewGuid().ToString(), clearTextCredit).Create();

                Assert.IsFalse(string.IsNullOrWhiteSpace(paymentMethod.Key));

                ((CreditCardData)paymentMethod.PaymentMethod).ExpYear = 2026;

                paymentMethod.SaveChanges();
            } catch (Exception ex) {
                Assert.Fail((ex.InnerException ?? ex).Message);
            }
        }

        [TestMethod]
        public void Delete_CustomerAccount_ReturnsSuccess() {
            try {
                var customer = new Customer() {
                    FirstName = "Account",
                    LastName = "Delete",
                    Email = "account.delete@test.com",
                    Id = Guid.NewGuid().ToString()
                }.Create();

                var paymentMethod = customer.AddPaymentMethod(Guid.NewGuid().ToString(), clearTextCredit).Create();

                Assert.IsFalse(string.IsNullOrWhiteSpace(paymentMethod.Key));

                paymentMethod.Delete();
            } catch (Exception ex) {
                Assert.Fail((ex.InnerException ?? ex).Message);
            }
        }

        [TestMethod]
        public void Delete_NonexistingCustomer_ThrowsApiException() {
            Assert.ThrowsException<ApiException>(() => {
                new Customer() {
                    FirstName = "Incog",
                    LastName = "Anony",
                    Id = "DoesntExist"
                }.Delete();
            });
        }

        #endregion

        #region Report Builder Cases
        [TestMethod]
        public void GetTransactionByOrderID_SingleBill() {
            var service = new BillPayService();
            var response = clearTextCredit.Verify()
                .WithAddress(new Address { PostalCode = "12345" })
                .WithRequestMultiUseToken(true)
                .Execute();

            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Token));

            var token = response.Token;
            var fee = service.CalculateConvenienceAmount(clearTextCredit, bill.Amount);

            var paymentMethod = new CreditCardData()
            {
                Token = token,
                ExpMonth = clearTextCredit.ExpMonth,
                ExpYear = clearTextCredit.ExpYear
            };

            Assert.IsFalse(string.IsNullOrWhiteSpace(token));

            var orderID = Guid.NewGuid().ToString();
            var transactionResponse = RunAndValidateTransaction(() => {
                return paymentMethod
                    .Charge(bill.Amount)
                    .WithAddress(address)
                    .WithBills(bill)
                    .WithConvenienceAmount(fee)
                    .WithOrderId(orderID)
                    .WithCurrency("USD")
                    .Execute();
            });

            TransactionSummary summary = ReportingService.TransactionDetail(orderID).Execute();
            Assert.IsNotNull(summary);
        }

        [TestMethod]
        public void GetTransactionByOrderID_MultipleBills()
        {
            var service = new BillPayService();
            var totalAmount = bills.Sum(x => x.Amount);
            var fee = service.CalculateConvenienceAmount(clearTextCredit, totalAmount);

            var orderID = Guid.NewGuid().ToString();
            var transactionResponse = RunAndValidateTransaction(() => {
                return clearTextCredit
                    .Charge(totalAmount)
                    .WithAddress(address)
                    .WithBills(bills)
                    .WithConvenienceAmount(fee)
                    .WithOrderId(orderID)
                    .WithCurrency("USD")
                    .Execute();
            });

            TransactionSummary summary = ReportingService.TransactionDetail(orderID).Execute();
            Assert.IsNotNull(summary);
        }
        #endregion

        #region Helpers

        /// <summary>
        /// Encapsulates the standard test framework for running a succesful transaction.
        /// </summary>
        /// <param name="transactionAction">A method that executes and returns a payment</param>
        /// <returns>The transaction generated by transactionAction</returns>
        private Transaction RunAndValidateTransaction(Func<Transaction> transactionAction) {
            Transaction transaction = null;
            try {
                transaction = transactionAction.Invoke();

                ValidateSuccesfulTransaction(transaction);
            } catch (GatewayException gex) {
                Assert.Fail($"{gex.ResponseCode} - {gex.ResponseMessage}");
            } catch (ValidationException vex) {
                Assert.Fail(vex.ValidationErrors.First());
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }

            return transaction;
        }

        private void ValidateSuccesfulTransaction(Transaction transaction) {
            var isValidTransactionId = int.TryParse(transaction.TransactionId, out int transactionId);

            Assert.IsTrue(isValidTransactionId, "Transaction Id is not an integer");

            Assert.AreNotEqual(transactionId, 0, transaction.ResponseMessage);
        }

        #endregion
    }
}
