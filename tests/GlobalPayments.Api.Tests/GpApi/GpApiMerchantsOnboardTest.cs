using System;
using System.Collections.Generic;
using System.Linq;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.PayFac;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi
{
    [TestClass]
    public class GpApiMerchantsOnboardTest : BaseGpApiTests
    {
        private PayFacService _service;
        private ReportingService _reportingService;
        private CreditCardData card;

        [TestInitialize]
        public void TestInitialize()
        {
            _service = new PayFacService();
            _reportingService = new ReportingService();

            ServicesContainer.RemoveConfig();

            var gpApiConfig = GpApiConfigSetup(AppIdForMerchant, AppKeyForMerchant, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);

            card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "123",
                CardPresent = true
            };
        }

        #region Onboard Merchant
        
        [TestMethod]
        public void BoardMerchant() {
            var merchantData = GetMerchantData();
            var productData = GetProductList();
            var persons = GetPersonList();
            var creditCardInformation = card; // TestAccountData.CreditCardData();
            var bankAccountInformation = GetBankAccountData(); 
            var paymentStatistics = GetPaymentStatistics();

            var merchant = _service.CreateMerchant()
                .WithUserPersonalData(merchantData)
                .WithDescription("Merchant Business Description")
                .WithProductData(productData)
                .WithCreditCardData(creditCardInformation, PaymentMethodFunction.PRIMARY_PAYOUT)
                .WithBankAccountData(bankAccountInformation, PaymentMethodFunction.SECONDARY_PAYOUT)
                .WithPersonsData(persons)
                .WithPaymentStatistics(paymentStatistics)
                .Execute();

            Assert.IsInstanceOfType(merchant, typeof(User));
            Assert.AreEqual("SUCCESS", merchant.ResponseCode);
            Assert.AreEqual(UserStatus.UNDER_REVIEW, merchant.UserReference.UserStatus);
            Assert.IsNotNull(merchant.UserReference.UserId);
        }
        

        [TestMethod]
        public void BoardMerchant_OnlyMandatory() {
            var merchantData = GetMerchantData();
            var productData = GetProductList();
            var persons = GetPersonList();
            var paymentStatistics = GetPaymentStatistics();

            var merchant = _service.CreateMerchant()
                .WithUserPersonalData(merchantData)
                .WithDescription("Merchant Business Description")
                .WithProductData(productData)
                .WithPersonsData(persons)
                .WithPaymentStatistics(paymentStatistics)
                .Execute();

            Assert.IsInstanceOfType(merchant, typeof(User));
            Assert.AreEqual("SUCCESS", merchant.ResponseCode);
            Assert.AreEqual(UserStatus.UNDER_REVIEW, merchant.UserReference.UserStatus);
            Assert.IsNotNull(merchant.UserReference.UserId);
        }

        [TestMethod]
        public void BoardMerchant_WithIdempotency() {
            var idempotencyKey = Guid.NewGuid().ToString();
            var merchantData = GetMerchantData();
            var productData = GetProductList();
            var persons = GetPersonList();
            var paymentStatistics = GetPaymentStatistics();

            var merchant = _service.CreateMerchant()
                .WithUserPersonalData(merchantData)
                .WithDescription("Merchant Business Description")
                .WithProductData(productData)
                .WithPersonsData(persons)
                .WithPaymentStatistics(paymentStatistics)
                .WithIdempotencyKey(idempotencyKey)
                .Execute();

            Assert.IsInstanceOfType(merchant, typeof(User));
            Assert.AreEqual("SUCCESS", merchant.ResponseCode);
            Assert.AreEqual(UserStatus.UNDER_REVIEW, merchant.UserReference.UserStatus);
            Assert.IsNotNull(merchant.UserReference.UserId);

            merchantData.FirstName = $"James {DateTime.Now.ToString("yyyyMMddmmss")}";

            var exceptionCaught = false;
            try
            {
                _service.CreateMerchant()
                    .WithUserPersonalData(merchantData)
                    .WithDescription("Merchant Business Description")
                    .WithProductData(productData)
                    .WithPersonsData(persons)
                    .WithPaymentStatistics(paymentStatistics)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: Conflict - Idempotency Key seen before: id={merchant.UserReference.UserId}, status={merchant.UserReference.UserStatus}",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void BoardMerchant_DuplicateMerchantName() {
            var idempotencyKey = Guid.NewGuid().ToString();
            var merchantData = GetMerchantData();
            var productData = GetProductList();
            var persons = GetPersonList();
            var paymentStatistics = GetPaymentStatistics();

            var merchant = _service.CreateMerchant()
                .WithUserPersonalData(merchantData)
                .WithDescription("Merchant Business Description")
                .WithProductData(productData)
                .WithPersonsData(persons)
                .WithPaymentStatistics(paymentStatistics)
                //.WithIdempotencyKey(idempotencyKey)
                .Execute();

            Assert.IsInstanceOfType(merchant, typeof(User));
            Assert.AreEqual("SUCCESS", merchant.ResponseCode);
            Assert.AreEqual(UserStatus.UNDER_REVIEW, merchant.UserReference.UserStatus);
            Assert.IsNotNull(merchant.UserReference.UserId);

            var exceptionCaught = false;
            try {
                _service.CreateMerchant()
                    .WithUserPersonalData(merchantData)
                    .WithDescription("Merchant Business Description")
                    .WithProductData(productData)
                    .WithPersonsData(persons)
                    .WithPaymentStatistics(paymentStatistics)
                    //.WithIdempotencyKey(idempotencyKey)
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40041", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Duplicate Merchant Name",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        #endregion

        [TestMethod]
        public void GetMerchantInfo() {
            var merchantId = "MER_34123992f0c84d70b30f8371202bd4e2";

            var merchant = _service.GetMerchantInfo(merchantId)
                .Execute();

            Assert.IsNotNull(merchant);
            Assert.AreEqual(merchantId, merchant.UserReference.UserId);
        }

        [TestMethod]
        public void GetMerchantInfo_RandomId() {
            var merchantId = $"MER_{Guid.NewGuid().ToString().Replace("-", "")}";

            var exceptionCaught = false;
            try {
                _service.GetMerchantInfo(merchantId)
                    .Execute();
            } catch (GatewayException ex)
            {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual(
                    $"Status Code: BadRequest - Merchant configuration does not exist for the following combination: MMA_1595ca59906346beae43d92c24863430 , {merchantId}",
                    ex.Message);
                Assert.AreEqual("40041", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void GetMerchantInfo_InvalidId() {
            var merchantId = $"MER_{Guid.NewGuid()}";

            var exceptionCaught = false;
            try {
                _service.GetMerchantInfo(merchantId)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_TRANSACTION_ACTION", ex.ResponseCode);
                Assert.AreEqual("Status Code: NotFound - Retrieve information about this transaction is not supported",
                    ex.Message);
                Assert.AreEqual("40042", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void SearchMerchants() {
            var merchants = _reportingService.FindMerchants(1, 10)
                .Execute();

            Assert.IsTrue(merchants.TotalRecordCount > 0);
            Assert.IsTrue(merchants.Results.Count <= 10);
        }

        [TestMethod]
        public void SearchMerchantAccounts()
        {
            var config = GpApiConfigSetup(AppIdForMerchant, AppKeyForMerchant, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(config);

            var merchants = _reportingService.FindMerchants(1, 10)
                .Where(SearchCriteria.MerchantStatus, MerchantAccountStatus.ACTIVE)
                .Execute();

            Assert.IsTrue(merchants.TotalRecordCount > 0);
            Assert.IsTrue(merchants.Results.Count <= 10);

            var merchantId = merchants.Results.FirstOrDefault().Id;

            config.MerchantId = merchantId;
            ServicesContainer.ConfigureService(config, "accounts");

            var accounts = ReportingService.FindAccounts(1, 10)
                .Where(SearchCriteria.AccountStatus, MerchantAccountStatus.ACTIVE)                
                .Execute("accounts");

            Assert.IsNotNull(accounts);
            Assert.IsTrue(accounts.Results.Count > 0);
        }

        #region Edit Merchant
        
        [TestMethod]
        public void EditMerchantApplicantInfo() {
            var merchants = _reportingService.FindMerchants(1, 1)
                .Execute();

            Assert.IsTrue(merchants.TotalRecordCount > 0);
            Assert.IsTrue(merchants.Results.Count == 1);

            var merchant = User.FromId(merchants.Results[0].Id, UserType.MERCHANT);
            var persons = GetPersonList("Update");
            var response = merchant.Edit()
                .WithPersonsData(persons)
                //.WithDescription("Update merchant applicant info")
                .Execute();

            Assert.IsInstanceOfType(response, typeof(User));
            Assert.AreEqual("PENDING", response.ResponseCode);
        }

        [TestMethod]
        public void EditMerchantPaymentProcessing() {
            var merchants = _reportingService.FindMerchants(1, 1)
                .Execute();

            Assert.IsTrue(merchants.TotalRecordCount > 0);
            Assert.IsTrue(merchants.Results.Count == 1);

            var paymentStatistics = new PaymentStatistics();
            paymentStatistics.TotalMonthlySalesAmount = 1111;
            paymentStatistics.HighestTicketSalesAmount = 2222;

            var merchant = User.FromId(merchants.Results[0].Id, UserType.MERCHANT);

            var response = merchant.Edit()
                .WithPaymentStatistics(paymentStatistics)
                //.WithDescription("Update merchant payment processing")
                .Execute();

            Assert.IsInstanceOfType(response, typeof(User));
            Assert.AreEqual("PENDING", response.ResponseCode);
        }

        [TestMethod]
        public void EditMerchantBusinessInformation() {
            var merchants = _reportingService.FindMerchants(1, 1)
                .Execute();

            Assert.IsTrue(merchants.TotalRecordCount > 0);
            Assert.IsTrue(merchants.Results.Count == 1);

            var merchant = User.FromId(merchants.Results[0].Id, UserType.MERCHANT);
            merchant.UserReference.UserStatus = UserStatus.ACTIVE;

            var merchantData = new UserPersonalData();
            merchantData.FirstName = "Username";
            merchantData.DBA = "Doing Business As";
            merchantData.Website = "https://abcd.com";
            merchantData.TaxIdReference = "987654321";
            var businessAddress = new Address();
            businessAddress.StreetAddress1 = "Apartment 852";
            businessAddress.StreetAddress2 = "Complex 741";
            businessAddress.StreetAddress3 = "Unit 4";
            businessAddress.City = "Chicago";
            businessAddress.State = "IL";
            businessAddress.PostalCode = "50001";
            businessAddress.CountryCode = "840";
            merchantData.UserAddress = businessAddress;

            /** @var User $response */
            var response = merchant.Edit()
                .WithUserPersonalData(merchantData)
                //.WithDescription("Sample Data for description")
                .Execute();

            //print_r($response);
            Assert.IsInstanceOfType(response, typeof(User));
            Assert.AreEqual("PENDING", response.ResponseCode);
            Assert.AreEqual(UserStatus.UNDER_REVIEW, response.UserReference.UserStatus);
            Assert.AreEqual(merchants.Results[0].Name, response.Name);
        }

        [TestMethod]
        public void EditMerchantApplicantInfo_RemoveMerchantFromPartner_FewArguments() {
            var merchants = _reportingService.FindMerchants(1, 1)
                .Execute();

            Assert.IsTrue(merchants.TotalRecordCount > 0);
            Assert.IsTrue(merchants.Results.Count == 1);

            var merchant = User.FromId(merchants.Results[0].Id, UserType.MERCHANT);

            var exceptionCaught = false;
            try {
                merchant.Edit()
                    .WithStatusChangeReason(StatusChangeReason.REMOVE_PARTNERSHIP)
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadRequest - Required field is missing.",
                    ex.Message);
                Assert.AreEqual("40241", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void EditMerchantApplicantInfo_RemoveMerchantFromPartner_TooManyArguments() {
            var merchants = _reportingService.FindMerchants(1, 1)
                .Execute();

            Assert.IsTrue(merchants.TotalRecordCount > 0);
            Assert.IsTrue(merchants.Results.Count == 1);

            var merchant = User.FromId(merchants.Results[0].Id, UserType.MERCHANT);
            var merchantData = GetMerchantData();

            var exceptionCaught = false;
            try {
                merchant.Edit()
                    .WithUserPersonalData(merchantData)
                    .WithStatusChangeReason(StatusChangeReason.REMOVE_PARTNERSHIP)
                    .Execute();
            } catch (GatewayException ex)
            {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual(
                    "Status Code: BadRequest - Bad Request. The request has extra tags which are not required.",
                    ex.Message);
                Assert.AreEqual("40268", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        #endregion

        #region Board Merchant error scenarios
        
        [TestMethod]
        public void BoardMerchant_WithoutMerchantData() {
            var productData = GetProductList();
            var persons = GetPersonList();
            var paymentStatistics = GetPaymentStatistics();

            var exceptionCaught = false;
            try {
                _service.CreateMerchant()
                    .WithDescription("Merchant Business Description")
                    .WithProductData(productData)
                    .WithPersonsData(persons)
                    .WithPaymentStatistics(paymentStatistics)
                    .Execute();
            } catch (BuilderException ex) {
                exceptionCaught = true;
                Assert.AreEqual("UserPersonalData cannot be null for this transaction type.", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void BoardMerchant_WithoutUserName() {
            var merchantData = GetMerchantData();
            merchantData.UserName = null;

            var productData = GetProductList();
            var persons = GetPersonList();
            var paymentStatistics = GetPaymentStatistics();

            var exceptionCaught = false;
            try {
                _service.CreateMerchant()
                    .WithUserPersonalData(merchantData)
                    .WithDescription("Merchant Business Description")
                    .WithProductData(productData)
                    .WithPersonsData(persons)
                    .WithPaymentStatistics(paymentStatistics)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields name",
                    ex.Message);
                Assert.AreEqual("40005", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void BoardMerchant_WithoutLegalName() {
            var merchantData = GetMerchantData();
            merchantData.LegalName = null;

            var productData = GetProductList();
            var persons = GetPersonList();
            var paymentStatistics = GetPaymentStatistics();

            var merchant = _service.CreateMerchant()
                .WithUserPersonalData(merchantData)
                .WithDescription("Merchant Business Description")
                .WithProductData(productData)
                .WithPersonsData(persons)
                .WithPaymentStatistics(paymentStatistics)
                .Execute();
            Assert.IsInstanceOfType(merchant, typeof(User));
            Assert.AreEqual("SUCCESS", merchant.ResponseCode);
            Assert.AreEqual(UserStatus.UNDER_REVIEW, merchant.UserReference.UserStatus);
            Assert.IsNotNull(merchant.UserReference.UserId);
        }

        [TestMethod]
        public void BoardMerchant_WithoutMerchantType() {
            var merchantData = GetMerchantData();
            merchantData.Type = null;

            var productData = GetProductList();
            var persons = GetPersonList();
            var paymentStatistics = GetPaymentStatistics();

            var exceptionCaught = false;
            try {
                _service.CreateMerchant()
                    .WithUserPersonalData(merchantData)
                    .WithDescription("Merchant Business Description")
                    .WithProductData(productData)
                    .WithPersonsData(persons)
                    .WithPaymentStatistics(paymentStatistics)
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields type",
                    ex.Message);
                Assert.AreEqual("40005", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void BoardMerchant_WithoutDba() {
            var merchantData = GetMerchantData();
            merchantData.DBA = null;

            var productData = GetProductList();
            var persons = GetPersonList();
            var paymentStatistics = GetPaymentStatistics();

            var merchant = _service.CreateMerchant()
                .WithUserPersonalData(merchantData)
                .WithDescription("Merchant Business Description")
                .WithProductData(productData)
                .WithPersonsData(persons)
                .WithPaymentStatistics(paymentStatistics)
                .Execute();

            Assert.IsInstanceOfType(merchant, typeof(User));
            Assert.AreEqual("SUCCESS", merchant.ResponseCode);
            Assert.AreEqual(UserStatus.UNDER_REVIEW, merchant.UserReference.UserStatus);
            Assert.IsNotNull(merchant.UserReference.UserId);
        }

        [TestMethod]
        public void BoardMerchant_WithoutWebsite() {
            var merchantData = GetMerchantData();
            merchantData.Website = null;

            var productData = GetProductList();
            var persons = GetPersonList();
            var paymentStatistics = GetPaymentStatistics();

            var merchant = _service.CreateMerchant()
                .WithUserPersonalData(merchantData)
                .WithDescription("Merchant Business Description")
                .WithProductData(productData)
                .WithPersonsData(persons)
                .WithPaymentStatistics(paymentStatistics)
                .Execute();

            Assert.IsInstanceOfType(merchant, typeof(User));
            Assert.AreEqual("SUCCESS", merchant.ResponseCode);
            Assert.AreEqual(UserStatus.UNDER_REVIEW, merchant.UserReference.UserStatus);
            Assert.IsNotNull(merchant.UserReference.UserId);
        }

        [TestMethod]
        public void BoardMerchant_WithoutNotificationStatusUrl() {
            var merchantData = GetMerchantData();
            merchantData.NotificationStatusUrl = null;

            var productData = GetProductList();
            var persons = GetPersonList();
            var paymentStatistics = GetPaymentStatistics();

            var merchant = _service.CreateMerchant()
                .WithUserPersonalData(merchantData)
                .WithDescription("Merchant Business Description")
                .WithProductData(productData)
                .WithPersonsData(persons)
                .WithPaymentStatistics(paymentStatistics)
                .Execute();

            Assert.IsInstanceOfType(merchant, typeof(User));
            Assert.AreEqual("SUCCESS", merchant.ResponseCode);
            Assert.AreEqual(UserStatus.UNDER_REVIEW, merchant.UserReference.UserStatus);
            Assert.IsNotNull(merchant.UserReference.UserId);
        }

        [TestMethod]
        public void BoardMerchant_WithoutPersons() {
            var merchantData = GetMerchantData();
            var productData = GetProductList();
            var paymentStatistics = GetPaymentStatistics();

            var exceptionCaught = false;
            try {
                _service.CreateMerchant()
                    .WithUserPersonalData(merchantData)
                    .WithDescription("Merchant Business Description")
                    .WithProductData(productData)
                    .WithPaymentStatistics(paymentStatistics)
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields : email",
                    ex.Message);
                Assert.AreEqual("40005", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void BoardMerchant_WithoutPaymentStatistics() {
            var merchantData = GetMerchantData();
            var productData = GetProductList();
            var persons = GetPersonList();

            var merchant = _service.CreateMerchant()
                .WithUserPersonalData(merchantData)
                .WithDescription("Merchant Business Description")
                .WithProductData(productData)
                .WithPersonsData(persons)
                .Execute();

            Assert.IsInstanceOfType(merchant, typeof(User));
            Assert.AreEqual("SUCCESS", merchant.ResponseCode);
            Assert.AreEqual(UserStatus.UNDER_REVIEW, merchant.UserReference.UserStatus);
            Assert.IsNotNull(merchant.UserReference.UserId);
        }

        [TestMethod]
        public void BoardMerchant_WithoutTotalMonthlySalesAmount() {
            var merchantData = GetMerchantData();
            var productData = GetProductList();
            var persons = GetPersonList();
            var paymentStatistics = GetPaymentStatistics();
            paymentStatistics.TotalMonthlySalesAmount = null;

            var merchant = _service.CreateMerchant()
                .WithUserPersonalData(merchantData)
                .WithDescription("Merchant Business Description")
                .WithProductData(productData)
                .WithPersonsData(persons)
                .WithPaymentStatistics(paymentStatistics)
                .Execute();

            Assert.IsInstanceOfType(merchant, typeof(User));
            Assert.AreEqual("SUCCESS", merchant.ResponseCode);
            Assert.AreEqual(UserStatus.UNDER_REVIEW, merchant.UserReference.UserStatus);
            Assert.IsNotNull(merchant.UserReference.UserId);
        }

        [TestMethod]
        public void BoardMerchant_WithoutAverageTicketSalesAmount() {
            var merchantData = GetMerchantData();
            var productData = GetProductList();
            var persons = GetPersonList();
            var paymentStatistics = GetPaymentStatistics();
            paymentStatistics.AverageTicketSalesAmount = null;

            var merchant = _service.CreateMerchant()
                .WithUserPersonalData(merchantData)
                .WithDescription("Merchant Business Description")
                .WithProductData(productData)
                .WithPersonsData(persons)
                .WithPaymentStatistics(paymentStatistics)
                .Execute();

            Assert.IsInstanceOfType(merchant, typeof(User));
            Assert.AreEqual("SUCCESS", merchant.ResponseCode);
            Assert.AreEqual(UserStatus.UNDER_REVIEW, merchant.UserReference.UserStatus);
            Assert.IsNotNull(merchant.UserReference.UserId);
        }

        [TestMethod]
        public void BoardMerchant_WithoutHighestTicketSalesAmount() {
            var merchantData = GetMerchantData();
            var productData = GetProductList();
            var persons = GetPersonList();
            var paymentStatistics = GetPaymentStatistics();
            paymentStatistics.HighestTicketSalesAmount = null;

            var merchant = _service.CreateMerchant()
                .WithUserPersonalData(merchantData)
                .WithDescription("Merchant Business Description")
                .WithProductData(productData)
                .WithPersonsData(persons)
                .WithPaymentStatistics(paymentStatistics)
                .Execute();

            Assert.IsInstanceOfType(merchant, typeof(User));
            Assert.AreEqual("SUCCESS", merchant.ResponseCode);
            Assert.AreEqual(UserStatus.UNDER_REVIEW, merchant.UserReference.UserStatus);
            Assert.IsNotNull(merchant.UserReference.UserId);
        }

        [TestMethod]
        public void BoardMerchant_WithoutDescription() {
            var merchantData = GetMerchantData();
            var productData = GetProductList();
            var persons = GetPersonList();
            var paymentStatistics = GetPaymentStatistics();

            var merchant = _service.CreateMerchant()
                .WithUserPersonalData(merchantData)
                .WithProductData(productData)
                .WithPersonsData(persons)
                .WithPaymentStatistics(paymentStatistics)
                .Execute();

            Assert.IsInstanceOfType(merchant, typeof(User));
            Assert.AreEqual("SUCCESS", merchant.ResponseCode);
            Assert.AreEqual(UserStatus.UNDER_REVIEW, merchant.UserReference.UserStatus);
            Assert.IsNotNull(merchant.UserReference.UserId);
        }
        
        #endregion

        #region Upload Documents
        
        [TestMethod]
        public void UploadMerchantDocs()
        {
            var documentDetail = new DocumentUploadData();
            documentDetail.Document = "VGVzdGluZw==";

            documentDetail.DocType = FileType.TIF;
            documentDetail.DocCategory = DocumentCategory.IDENTITY_VERIFICATION;            
           
            var merchant = User.FromId("MER_5096d6b88b0b49019c870392bd98ddac", UserType.MERCHANT);
            var response = merchant.UploadDocument(documentDetail)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.IsNotNull(response.Document);
            Assert.IsNotNull(response.Document.Id);
            Assert.AreEqual(FileType.TIF, response.Document.Format);
            Assert.AreEqual(DocumentCategory.IDENTITY_VERIFICATION, response.Document.Category);            
        }

        [TestMethod]
        public void UploadMerchantDocs_AllDocumentFormats()
        {
            var documentDetail = new DocumentUploadData {
                Document = "VGVzdGluZw==",
                DocCategory = DocumentCategory.IDENTITY_VERIFICATION
            };

            foreach (FileType fileType in Enum.GetValues(typeof(FileType))) {
                var merchant = User.FromId("MER_5096d6b88b0b49019c870392bd98ddac", UserType.MERCHANT);
                
                documentDetail.DocType = fileType;

                var response = merchant.UploadDocument(documentDetail)
                 .Execute();

                Assert.IsNotNull(response);
                Assert.AreEqual("SUCCESS", response.ResponseCode);
                Assert.IsNotNull(response.Document);
                Assert.IsNotNull(response.Document.Id);
                Assert.AreEqual(fileType, response.Document.Format);
                Assert.AreEqual(DocumentCategory.IDENTITY_VERIFICATION, response.Document.Category);
            }
        }

        [TestMethod]
        public void UploadMerchantDocs_AllDocumentCategories()
        {
            var documentDetail = new DocumentUploadData {
                Document = "VGVzdGluZw==",
                DocType = FileType.TIF,
                DocCategory = DocumentCategory.IDENTITY_VERIFICATION
            };

            foreach (DocumentCategory documentCategory in Enum.GetValues(typeof(DocumentCategory))) {
                var merchant = User.FromId("MER_5096d6b88b0b49019c870392bd98ddac", UserType.MERCHANT);
                var categoryEnum = EnumConverter.GetMapping(Target.GP_API, documentCategory);
                if (string.IsNullOrEmpty(categoryEnum))
                    continue;

                documentDetail.DocCategory = documentCategory;

                var response = merchant.UploadDocument(documentDetail)
                .Execute();

                Assert.IsNotNull(response);
                Assert.AreEqual("SUCCESS", response.ResponseCode);
                Assert.IsNotNull(response.Document);
                Assert.IsNotNull(response.Document.Id);
                Assert.AreEqual(FileType.TIF, response.Document.Format);
                Assert.AreEqual(documentCategory, response.Document.Category);
            }
        }
        
        [TestMethod]
        public void UploadMerchantDocs_MissingDocFormat()
        {
            var documentDetail = new DocumentUploadData {
                Document = "VGVzdGluZw==",
                DocCategory = DocumentCategory.IDENTITY_VERIFICATION
            };

            var merchant = User.FromId("MER_5096d6b88b0b49019c870392bd98ddac", UserType.MERCHANT);
            var exceptionCaught = false;
            try {
               merchant.UploadDocument(documentDetail)
                .Execute();
            } catch (BuilderException e) {
                exceptionCaught = true;
                Assert.AreEqual("DocType cannot be null for this transaction type.", e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        [TestMethod]
        public void UploadMerchantDocs_MissingDocCategory()
        {
            var documentDetail = new DocumentUploadData {
                Document = "VGVzdGluZw==",
                DocType = FileType.TIF,
            };

            var merchant = User.FromId("MER_5096d6b88b0b49019c870392bd98ddac", UserType.MERCHANT);
            var exceptionCaught = false;
            try {
                merchant.UploadDocument(documentDetail)
                .Execute();
            } catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest -  Request expects the following fields: function", e.Message);
                Assert.AreEqual("MANDATORY_DATA_MISSING", e.ResponseCode);
                Assert.AreEqual("40251", e.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        [TestMethod]
        public void UploadMerchantDocs_MissingDocBaseContent()
        {
            var documentDetail = new DocumentUploadData {
                DocType = FileType.TIF,
                DocCategory = DocumentCategory.IDENTITY_VERIFICATION
            };

            var merchant = User.FromId("MER_5096d6b88b0b49019c870392bd98ddac", UserType.MERCHANT);
            var exceptionCaught = false;
            try {
                merchant.UploadDocument(documentDetail)
                  .Execute();
            } catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest -  Request expects the following fields: b64_content", e.Message);
                Assert.AreEqual("MANDATORY_DATA_MISSING", e.ResponseCode);
                Assert.AreEqual("40251", e.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        #endregion
        
        private List<Person> GetPersonList(string type = null) {
            var persons = new List<Person>();

            var person = new Person();
            person.Functions = PersonFunctions.APPLICANT;
            person.FirstName = "James " + type;
            person.MiddleName = "Mason " + type;
            person.LastName = "Doe " + " " + type;
            person.Email = "uniqueemail@address.com";
            person.DateOfBirth = DateTime.Parse("1982-02-23").ToString("yyyy-MM-dd");
            person.NationalIdReference = "123456789";
            person.JobTitle = "CEO";
            person.EquityPercentage = "25";
            person.Address = new Address();
            person.Address.StreetAddress1 = "1 Business Address";
            person.Address.StreetAddress2 = "Suite 2";
            person.Address.StreetAddress3 = "1234";
            person.Address.City = "Atlanta";
            person.Address.State = "GA";
            person.Address.PostalCode = "30346";
            person.Address.Country = "US";
            person.HomePhone = new PhoneNumber { CountryCode = "01", Number = "8008675309" };
            person.WorkPhone = new PhoneNumber { CountryCode = "01", Number = "8008675309" };

            persons.Add(person);

            return persons;
        }

        private List<Product> GetProductList() {
            return new List<Product>()
            {
                new Product { ProductId = "PRO_TRA_CP-US-CARD-A920_SP" },
                new Product { ProductId = "PRO_TRA_CNP_US_BANK-TRANSFER_PP" },
                new Product { ProductId = "PRO_FMA_PUSH-FUNDS_PP" },
                new Product { ProductId = "PRO_TRA_CNP-US-CARD_PP" }
            };
        }

        private PaymentStatistics GetPaymentStatistics() {
            var paymentStatistics = new PaymentStatistics {
                TotalMonthlySalesAmount = 3000000,
                AverageTicketSalesAmount = 50000,
                HighestTicketSalesAmount = 60000
            };

            return paymentStatistics;
        }

        private BankAccountData GetBankAccountData()
        {
            var bankAccountInformation = new BankAccountData {
                AccountHolderName = "Bank Account Holder Name",
                AccountNumber = "123456788",
                AccountOwnershipType = "Personal",
                AccountType = AccountType.SAVINGS.ToString(),
                RoutingNumber = "102000076"
            };

            return bankAccountInformation;
        }

        private UserPersonalData GetMerchantData() {
            var merchantData = new UserPersonalData();
            merchantData.UserName = $"CERT_Propay_{DateTime.Now.ToString("yyyyMMddmmss")}";
            merchantData.LegalName = "Business Legal Name";
            merchantData.DBA = "Doing Business As";
            merchantData.MerchantCategoryCode = 5999;
            merchantData.Website = "https://example.com/";
            merchantData.NotificationEmail = "merchant@example.com";
            merchantData.CurrencyCode = "USD";
            merchantData.TaxIdReference = "123456789";
            merchantData.Tier = "test";
            merchantData.Type = UserType.MERCHANT;

            var businessAddress = new Address();
            businessAddress.StreetAddress1 = "Apartment 852";
            businessAddress.StreetAddress2 = "Complex 741";
            businessAddress.StreetAddress3 = "Unit 4";
            businessAddress.City = "Chicago";
            businessAddress.State = "IL";
            businessAddress.PostalCode = "50001";
            businessAddress.CountryCode = "840";

            merchantData.UserAddress = businessAddress;

            var shippingAddress = new Address();
            shippingAddress.StreetAddress1 = "Flat 456";
            shippingAddress.StreetAddress2 = "House 789";
            shippingAddress.StreetAddress3 = "Basement Flat";
            shippingAddress.City = "Halifax";
            shippingAddress.PostalCode = "W5 9HR";
            shippingAddress.CountryCode = "826";

            merchantData.MailingAddress = shippingAddress;
            merchantData.NotificationStatusUrl = "https://www.example.com/notifications/status";

            return merchantData;
        }
    }
}