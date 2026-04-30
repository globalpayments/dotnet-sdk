using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {

    /// <summary>
    /// Test class for Visa Installment features using GP API.
    /// Contains integration and negative test cases for:
    ///   - Credit sale transactions with Visa Installment program
    ///   - Querying and retrieving installment plans
    ///   - Hosted Payment Page (HPP) link creation with installments
    ///   - Validation of error scenarios (missing/invalid data)
    ///   - Regular transactions without installments
    /// </summary>
    [TestClass]
    public class GpApiVisaInstallmentTest : BaseGpApiTests {

        private const string VISA_INSTALLMENT_APP_ID = "hkjrcsGDhWiDt8GEhoDMKy3pzFz5R0Bo";
        private const string VISA_INSTALLMENT_APP_KEY = "cQOKHoAAvNIcEN8s";
        private const string INSTALLMENT_ACCOUNT_NAME = "GPECOM_Installments_Processing";
        private const string INSTALLMENT_CONFIG_NAME = "installments";
        private const string CURRENCY = "GBP";
        private const decimal AMOUNT = 1000m;

        private CreditCardData visaCard;
        private Address address;

        [TestInitialize]
        public void TestInitialize() {
            ServicesContainer.RemoveConfig();

            // Default config for sale transactions
            var installmentConfig = new GpApiConfig {
                AppId = VISA_INSTALLMENT_APP_ID,
                AppKey = VISA_INSTALLMENT_APP_KEY,
                Channel = Channel.CardNotPresent,
                Environment = Entities.Environment.TEST,
                ServiceUrl = ServiceEndpoints.GP_API_BOI_TEST,
                Country = "GB",
                RequestLogger = new RequestConsoleLogger(),
                EnableLogging = true,
                AccessTokenInfo = new AccessTokenInfo {
                    TransactionProcessingAccountName = INSTALLMENT_ACCOUNT_NAME
                }
            };
            ServicesContainer.ConfigureService(installmentConfig, INSTALLMENT_CONFIG_NAME);

            // Config for installment query/GET endpoints
            var config = new GpApiConfig {
                AppId = VISA_INSTALLMENT_APP_ID,
                AppKey = VISA_INSTALLMENT_APP_KEY,
                Channel = Channel.CardNotPresent,
                Environment = Entities.Environment.TEST,
                Country = "GB",
                RequestLogger = new RequestConsoleLogger(),
                EnableLogging = true,
                ServiceUrl = ServiceEndpoints.GP_API_TEST,
                AccessTokenInfo = new AccessTokenInfo {
                    TransactionProcessingAccountName = INSTALLMENT_ACCOUNT_NAME
                }
            };
            ServicesContainer.ConfigureService(config);

            var configSetup = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(configSetup, "defaultConfig");

            address = new Address {
                StreetAddress1 = "123 Main St.",
                City = "Downtown",
                State = "NJ",
                PostalCode = "12345",
                Country = "US"
            };

            visaCard = new CreditCardData {
                Number = "4622943127052828",
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "999",
                CardPresent = false,
                ReaderPresent = false
            };
        }

        #region Credit Sale with VIS Program

        /// <summary>
        /// Tests a credit sale transaction with Visa Installment program.
        /// </summary>
        [TestMethod]
        public void CreditSale_WithVisProgram() {
            var response = visaCard.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithInstallmentData(new InstallmentData {
                    Program = "VIS",
                    Count = "12",
                    Reference = "109bbbf5-c027-de5e-50c1-153dafa9ac03",
                    Terms = new Terms {
                        Language = "fre",
                        Version = "2"
                    }
                })
                .WithAllowDuplicates(true)
                .Execute(INSTALLMENT_CONFIG_NAME);

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
            Assert.AreEqual("VIS", response.InstallmentData?.Program);
        }

        /// <summary>
        /// Tests a credit sale transaction with Visa Installment program and storage mode set to ALWAYS.
        /// </summary>
        [TestMethod]
        public void CreditSale_WithVisProgram_WithStorageMode() {
            var response = visaCard.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithStorageMode(StorageMode.ALWAYS)
                .WithInstallmentData(new InstallmentData {
                    Program = "VIS",
                    Count = "12",
                    Reference = "109bbbf5-c027-de5e-50c1-153dafa9ac03",
                    Terms = new Terms {
                        Language = "fre",
                        Version = "2"
                    }
                })
                .WithAllowDuplicates(true)
                .Execute(INSTALLMENT_CONFIG_NAME);

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
            Assert.AreEqual("VIS", response.InstallmentData?.Program);
        }

        /// <summary>
        /// Tests a regular card transaction without installments (no installment data set).
        /// </summary>
        [TestMethod]
        public void RegularTransaction_WithoutInstallments_WithInstallmentConfig() {
            var response = visaCard.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .Execute(INSTALLMENT_CONFIG_NAME);

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
            Assert.IsNull(response.InstallmentData);
        }

        /// <summary>
        /// Tests a regular card transaction without installments (no installment data set).
        /// </summary>
        [TestMethod]
        public void RegularTransaction_WithoutInstallments_WithDefaultConfig() {   
            var response = visaCard.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .Execute("defaultConfig");

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
            Assert.IsNull(response.InstallmentData);
        }

        #endregion

        #region Query Installment Plans

        /// <summary>
        /// Tests querying available Visa installment plans.
        /// </summary>
        [TestMethod]
        public void QueryInstallmentPlans() {
            var response = new Installment {
                AccountName = INSTALLMENT_ACCOUNT_NAME,
                Channel = "CNP",
                Amount = AMOUNT,
                Currency = CURRENCY,
                Country = "GB",
                Program = "VIS",
                Reference = "QUERY-" + Guid.NewGuid().ToString(),
                FundingMode = FundingMode.CONSUMER_FUNDED,
                EligiblePlans = EligiblePlans.LIMITED,
                EntryMode = "ECOM",
                InstallmentTerms = new Terms {
                    MaxTimeUnitNumber = 24,
                    MaxAmount = 1000
                },
                CardDetails = new CreditCardData {
                    Number = "4263970000005262",
                    ExpMonth = ExpMonth,
                    ExpYear = ExpYear,
                    CardPresent = false,
                    ReaderPresent = false
                }
            }.Create(INSTALLMENT_CONFIG_NAME);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Id);
            Assert.IsNotNull(response.Terms);
            Assert.IsTrue(response.Terms.Count > 0);
        }

        #endregion

        #region GET Installment by ID

        /// <summary>
        /// Tests retrieving an installment by its ID.
        /// </summary>
        [TestMethod]
        public void GetInstallment_ByInstallmentId() {
            var queryResponse = new Installment {
                AccountName = INSTALLMENT_ACCOUNT_NAME,
                Channel = "CNP",
                Amount = AMOUNT,
                Currency = CURRENCY,
                Country = "GB",
                Program = "VIS",
                Reference = "QUERY-" + Guid.NewGuid().ToString(),
                FundingMode = FundingMode.CONSUMER_FUNDED,
                EligiblePlans = EligiblePlans.LIMITED,
                EntryMode = "ECOM",
                InstallmentTerms = new Terms {
                    MaxTimeUnitNumber = 24,
                    MaxAmount = 1000
                },
                CardDetails = new CreditCardData {
                    Number = "4263970000005262",
                    ExpMonth = ExpMonth,
                    ExpYear = ExpYear,
                    CardPresent = false,
                    ReaderPresent = false
                }
            }.Create(INSTALLMENT_CONFIG_NAME);
            Assert.IsNotNull(queryResponse);
            Assert.IsNotNull(queryResponse.Id);

            WaitForGpApiReplication();

            var installmentDetails = InstallmentService.Get(queryResponse.Id);

            Assert.IsNotNull(installmentDetails);
            Assert.IsNotNull(installmentDetails.Id);
            Assert.AreEqual(queryResponse.Id, installmentDetails.Id);
        }

        #endregion

        #region GET Transaction with Installment Data

        /// <summary>
        /// Tests retrieving a transaction with associated installment data.
        /// </summary>
        [TestMethod]
        public void GetTransaction_WithInstallmentData() {
            var transactionResponse = visaCard.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithInstallmentData(new InstallmentData {
                    Program = "VIS",
                    Count = "12",
                    Reference = "109bbbf5-c027-de5e-50c1-153dafa9ac03",
                    Terms = new Terms {
                        Language = "fre",
                        Version = "2"
                    }
                })
                .WithAllowDuplicates(true)
                .Execute(INSTALLMENT_CONFIG_NAME);

            Assert.IsNotNull(transactionResponse);
            Assert.AreEqual(Success, transactionResponse.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transactionResponse.ResponseMessage);

            WaitForGpApiReplication();

            var retrievedTransaction = ReportingService.TransactionDetail(transactionResponse.TransactionId)
                .Execute(INSTALLMENT_CONFIG_NAME);

            Assert.IsNotNull(retrievedTransaction);
            Assert.AreEqual(transactionResponse.TransactionId, retrievedTransaction.TransactionId);
            Assert.IsNotNull(retrievedTransaction.InstallmentData);
            Assert.IsNotNull(retrievedTransaction.InstallmentData.Id);
            Assert.IsNotNull(retrievedTransaction.InstallmentData.Program);
        }

        #endregion

        #region HPP Link with Installments

        /// <summary>
        /// Tests creating a Hosted Payment Page link with Visa installment data.
        /// </summary>
        [TestMethod]
        public void CreateHPPLink_WithInstallments() {
            var installmentData = new InstallmentData {
                Program = "VIS",
                Count = "12",
                FundingMode = FundingMode.CONSUMER_FUNDED,
                Terms = new Terms {
                    MaxTimeUnitNumber = 24,
                    MaxAmount = 200000
                }
            };

            var payByLink = new PayByLinkData {
                Type = PayByLinkType.HOSTED_PAYMENT_PAGE,
                UsageMode = PaymentMethodUsageMode.Single,
                AllowedPaymentMethods = new PaymentMethodName[] { PaymentMethodName.Card },
                UsageLimit = 1,
                Name = "Mobile",
                IsShippable = false,
                ShippingAmount = 1.23m,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                CancelUrl = "https://www.example.com/cancelUrl",
                IsDccEnabled = false,
                ExpirationDate = DateTime.UtcNow.AddDays(10),
                InstallmentData = installmentData,
                Configuration = new PaymentMethodConfiguration {
                    StorageMode = StorageMode.OFF,
                    ExemptStatus = ExemptStatus.LOW_VALUE,
                    IsBillingAddressRequired = false,
                    IsShippingAddressEnabled = true,
                    IsAddressOverrideAllowed = false,
                    ChallengeRequestIndicator = ChallengeRequestIndicator.NO_CHALLENGE_REQUESTED
                }
            };

            var shippingAddress = new Address {
                StreetAddress1 = "Apartment 852",
                StreetAddress2 = "Complex 741",
                StreetAddress3 = "no",
                City = "Chicago",
                State = "IL",
                PostalCode = "5001",
                Country = "US"
            };

            var billingAddress = new Address {
                StreetAddress1 = "Apartment 852",
                StreetAddress2 = "Complex 741",
                StreetAddress3 = "no",
                City = "Chicago",
                PostalCode = "5001",
                Country = "US"
            };

            var newCustomer = new Customer {
                FirstName = "James",
                LastName = "Mason",
                Email = "jamesmason@example.com",
                Status = "NEW"
            };

            var response = PayByLinkService.Create(payByLink, AMOUNT)
                .WithCurrency(CURRENCY)
                .WithClientTransactionId("TestOrder-123")
                .WithDescription("HPP_Links_Test")
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithCustomerData(newCustomer)
                .WithPayByLinkData(payByLink)
                .Execute(INSTALLMENT_CONFIG_NAME);

            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(PayByLinkStatus.ACTIVE.ToString().ToUpper(), response.ResponseMessage.ToUpper());
            Assert.IsNotNull(response.PayByLinkResponse?.Url);
            Assert.IsNotNull(response.PayByLinkResponse?.Id);
        }

        #endregion

        #region Negative Tests

        /// <summary>
        /// Tests that missing card details throws an ApiException.
        /// </summary>
        [TestMethod]
        public void QueryInstallment_MissingCardDetails_ThrowsException() {
            var installment = new Installment {
                AccountName = INSTALLMENT_ACCOUNT_NAME,
                Amount = AMOUNT,
                Currency = CURRENCY,
                Program = "VIS"
                // CardDetails intentionally omitted
            };

            var ex = Assert.ThrowsException<ApiException>(() => {
                installment.Create(INSTALLMENT_CONFIG_NAME);
            });

            Assert.IsNotNull(ex.Message);
        }

        /// <summary>
        /// Tests that missing terms throws a GatewayException.
        /// </summary>
        [TestMethod]
        public void QueryInstallment_MissingTerms_ThrowsException() {
            var installment = new Installment {
                AccountName = INSTALLMENT_ACCOUNT_NAME,
                Channel = "CNP",
                Amount = AMOUNT,
                Currency = CURRENCY,
                Program = "VIS",
                // InstallmentTerms intentionally omitted
                CardDetails = new CreditCardData {
                    Number = "4263970000005262",
                    ExpMonth = ExpMonth,
                    ExpYear = ExpYear
                }
            };

            var ex = Assert.ThrowsException<GatewayException>(() => {
                installment.Create(INSTALLMENT_CONFIG_NAME);
            });

            Assert.IsNotNull(ex.ResponseCode);
            Assert.IsNotNull(ex.ResponseMessage);
            Assert.IsNotNull(ex.Message);
        }

        /// <summary>
        /// Tests that an invalid currency throws a GatewayException.
        /// </summary>
        [TestMethod]
        public void QueryInstallment_InvalidCurrency_ThrowsException() {
            var installment = new Installment {
                AccountName = INSTALLMENT_ACCOUNT_NAME,
                Amount = AMOUNT,
                Currency = "INVALID_CURRENCY",
                Program = "VIS",
                InstallmentTerms = new Terms { MaxTimeUnitNumber = 24, MaxAmount = 1000 },
                CardDetails = new CreditCardData {
                    Number = "4263970000005262",
                    ExpMonth = ExpMonth,
                    ExpYear = ExpYear
                }
            };

            var ex = Assert.ThrowsException<GatewayException>(() => {
                installment.Create(INSTALLMENT_CONFIG_NAME);
            });

            Assert.IsNotNull(ex.ResponseCode);
            Assert.IsNotNull(ex.ResponseMessage);
            Assert.IsNotNull(ex.Message);
        }

        /// <summary>
        /// Tests that a negative amount throws a GatewayException.
        /// </summary>
        [TestMethod]
        public void QueryInstallment_NegativeAmount_ThrowsException() {
            var installment = new Installment {
                AccountName = INSTALLMENT_ACCOUNT_NAME,
                Amount = -100m,
                Currency = CURRENCY,
                Program = "VIS",
                InstallmentTerms = new Terms { MaxTimeUnitNumber = 24, MaxAmount = 1000 },
                CardDetails = new CreditCardData {
                    Number = "4263970000005262",
                    ExpMonth = ExpMonth,
                    ExpYear = ExpYear
                }
            };

            var ex = Assert.ThrowsException<GatewayException>(() => {
                installment.Create(INSTALLMENT_CONFIG_NAME);
            });

            Assert.IsNotNull(ex.ResponseCode);
            Assert.IsNotNull(ex.ResponseMessage);
            Assert.IsNotNull(ex.Message);
        }

        /// <summary>
        /// Tests that a null program throws a GatewayException.
        /// </summary>
        [TestMethod]
        public void QueryInstallment_NullProgram_ThrowsException() {
            var installment = new Installment {
                AccountName = INSTALLMENT_ACCOUNT_NAME,
                Amount = AMOUNT,
                Currency = CURRENCY,
                // Program intentionally null
                InstallmentTerms = new Terms { MaxTimeUnitNumber = 24, MaxAmount = 1000 },
                CardDetails = new CreditCardData {
                    Number = "4263970000005262",
                    ExpMonth = ExpMonth,
                    ExpYear = ExpYear
                }
            };

            var ex = Assert.ThrowsException<GatewayException>(() => {
                installment.Create(INSTALLMENT_CONFIG_NAME);
            });

            Assert.IsNotNull(ex.ResponseCode);
            Assert.IsNotNull(ex.ResponseMessage);
            Assert.IsNotNull(ex.Message);
        }

        /// <summary>
        /// Tests that an expired card throws a GatewayException.
        /// </summary>
        [TestMethod]
        public void QueryInstallment_ExpiredCard_ThrowsException() {
            var installment = new Installment {
                AccountName = INSTALLMENT_ACCOUNT_NAME,
                Amount = AMOUNT,
                Currency = CURRENCY,
                Program = "VIS",
                InstallmentTerms = new Terms { MaxTimeUnitNumber = 24, MaxAmount = 1000 },
                CardDetails = new CreditCardData {
                    Number = "4263970000005262",
                    ExpMonth = 1,
                    ExpYear = 2020  // expired
                }
            };

            var ex = Assert.ThrowsException<GatewayException>(() => {
                installment.Create(INSTALLMENT_CONFIG_NAME);
            });

            Assert.IsNotNull(ex.ResponseCode);
            Assert.IsNotNull(ex.ResponseMessage);
            Assert.IsNotNull(ex.Message);
        }

        /// <summary>
        /// Tests that a null installment ID throws an ApiException.
        /// </summary>
        [TestMethod]
        public void GetInstallment_NullId_ThrowsApiException() {
            var ex = Assert.ThrowsException<ApiException>(() => {
                InstallmentService.Get(null, INSTALLMENT_CONFIG_NAME);
            });

            Assert.AreEqual("Installment id is mandatory and cannot be null", ex.Message);
        }

        /// <summary>
        /// Tests that a non-existent installment ID throws a GatewayException.
        /// </summary>
        [TestMethod]
        public void GetInstallment_NonExistentId_ThrowsGatewayException() {
            var ex = Assert.ThrowsException<GatewayException>(() => {
                InstallmentService.Get("NON_EXISTENT_ID_12345", INSTALLMENT_CONFIG_NAME);
            });

            Assert.IsNotNull(ex.ResponseCode);
            Assert.IsNotNull(ex.ResponseMessage);
            Assert.IsNotNull(ex.Message);
        }

        /// <summary>
        /// Tests that a zero amount transaction throws a GatewayException.
        /// </summary>
        [TestMethod]
        public void Transaction_ZeroAmount_ThrowsGatewayException() {
            var ex = Assert.ThrowsException<GatewayException>(() => {
                visaCard.Charge(0m)
                    .WithCurrency(CURRENCY)
                    .WithAddress(address)
                    .WithInstallmentData(new InstallmentData {
                        Program = "VIS",
                        Count = "12",
                        Terms = new Terms {
                            Language = "fre",
                            Version = "2"
                        }
                    })
                    .Execute();
            });

            Assert.IsNotNull(ex.ResponseCode);
            Assert.IsNotNull(ex.ResponseMessage);
            Assert.IsNotNull(ex.Message);
        }

        #endregion
    }
}
