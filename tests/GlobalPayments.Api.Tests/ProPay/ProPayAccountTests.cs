using System.Linq;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlobalPayments.Api.Entities;
using System;
using Environment = GlobalPayments.Api.Entities.Environment;
using GlobalPayments.Api.Tests.ProPay.TestData;
using GlobalPayments.Api.Entities.PayFac;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils.Logging;
using GlobalPayments.Api.Entities.Enums;

namespace GlobalPayments.Api.Tests.ProPay {
    [TestClass]
    public class ProPayAccountTests {
        private PayFacService _service;

        public ProPayAccountTests() {
            _service = new PayFacService();
            ServicesContainer.ConfigureService(new PorticoConfig()
            {
                CertificationStr = "ba7988a23134c54b95e55aee761351",
                TerminalID = "761351",
                Environment = Environment.TEST,
                EnableLogging = true,
                RequestLogger = new RequestFileLogger(@"C:\temp\profac\requestlog.txt"),
                X509CertificatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ProPay\TestData\testCertificate.crt"),
                //X509CertificateBase64String = "MIICpDCCAYygAwIBAgIIS7Y5fijJytIwDQYJKoZIhvcNAQENBQAwETEPMA0GA1UEAwwGUFJPUEFZMB4XDTE5MDkxOTAwMDAwMFoXDTI5MDkxOTAwMDAwMFowEzERMA8GA1UEAwwIMTI3LjAuMDEwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCCwvq2ho43oeeGX3L9+2aD7bna7qjdLwWumeIpwhPZLa44MeQ5100wy4W2hKk3pOb5yaHqyhzoHDriveQnq/EpZJk9m7sizXsxZtBHtt+wghSZjdNhnon3R54SH5J7oEPybRSAKXSEzHjN+kCu7W3TmXSLve6YuODnjUpbOcAsHG2wE+zpCoEbe8toH5Tt7g8HzEc5mJYkkILTq6j9pwDE50r2NVbV3SXwmQ1ifxf54Z9EFB5bQv5cI3+GL/VwlQeJdiKMGj1rs8zTR8TjbAjVlJbz6bBkFItUsqexgwAHIJZAaU7an8ZamGRlPjf6dp3mOEu4B47igNj5KOSgCNdRAgMBAAEwDQYJKoZIhvcNAQENBQADggEBAF88u367yrduqd3PfEIo2ClaI2QPRIIWKKACMcZDl3z1BzVzNFOZNG2vLcSuKnGRH89tJPCjyxdJa0RyDTkXMSLqb5FgUseEjmj3ULAvFqLZNW35PY9mmlmCY+S3CC/bQR4iyPLo8lsRq0Nl6hlvB440+9zS8UQjtc2957QgcXfD427UJb698gXzsfQcNeaQWy8pNm7FzDfHTJbo/t6FOpmfR+RMZky9FrlWabInkrkf3w2XJL0uUAYU9jGQa+l/vnZD2KNzs1mO1EqkS6yB/fsn85mkgGe4Vfbo9GQ/S+KmDujewFA0ma7O03fy1W5v6Amn/nAcFTCddVL3BDNEtOM=",
                ProPayUS = true
            });
        }

        [TestMethod]
        public void CreateAccount() {
            var bankAccountInfo = TestAccountData.GetBankAccountData();
            var userBusinessInfo = TestAccountData.GetBusinessData();
            var accountPersonalInfo = TestAccountData.GetUserPersonalData();
            var threatRiskData = TestAccountData.GetThreatRiskData();
            var significantOwnerData = TestAccountData.GetSignificantOwnerData();
            var ownersInfo = TestAccountData.GetBeneficialOwnerData();
            var creditCardInfo = TestAccountData.GetCreditCardData();
            var achInfo = TestAccountData.GetACHData();
            var mailingAddressInfo = TestAccountData.GetMailingAddress();
            var secondaryBankInformation = TestAccountData.GetSecondaryBankAccountData();
            var deviceData = TestAccountData.GetDeviceData(1, false);

            var response = _service.CreateAccount()
                .WithBankAccountData(bankAccountInfo)
                .WithBusinessData(userBusinessInfo)
                .WithUserPersonalData(accountPersonalInfo)
                .WithThreatRiskData(threatRiskData)
                .WithSignificantOwnerData(significantOwnerData)
                .WithBeneficialOwnerData(ownersInfo)
                .WithCreditCardData(creditCardInfo)
                .WithACHData(achInfo)
                .WithMailingAddress(mailingAddressInfo)
                .WithSecondaryBankAccountData(secondaryBankInformation)
                .WithDeviceData(deviceData)
                .WithTimeZone("ET")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.PayFacData.AccountNumber);
            Assert.IsNotNull(response.PayFacData.Password);
            Assert.IsNotNull(response.PayFacData.SourceEmail);
        }

        [TestMethod]
        public void CreateAccountForDeviceOrder() {
            var bankAccountInfo = TestAccountData.GetBankAccountData();
            var userBusinessInfo = TestAccountData.GetBusinessData();
            var accountPersonalInfo = TestAccountData.GetUserPersonalData();
            var ownersInfo = TestAccountData.GetBeneficialOwnerData();
            var mailingAddressInfo = TestAccountData.GetMailingAddress();
            var deviceData = TestAccountData.GetDeviceData(1, false);
            var creditCardInfo = TestAccountData.GetCreditCardData();

            var response = _service.CreateAccount()
                .WithBankAccountData(bankAccountInfo)
                .WithBusinessData(userBusinessInfo)
                .WithUserPersonalData(accountPersonalInfo)
                .WithBeneficialOwnerData(ownersInfo)
                .WithCreditCardData(creditCardInfo)
                .WithMailingAddress(mailingAddressInfo)
                .WithDeviceData(deviceData)
                .WithTimeZone("ET")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.PayFacData.AccountNumber);
            Assert.IsNotNull(response.PayFacData.Password);
            Assert.IsNotNull(response.PayFacData.SourceEmail);
        }

        [TestMethod]
        public void OrderNewDevice() {
            var orderDeviceInfo = TestAccountData.GetOrderNewDeviceData();
            var deviceData = TestAccountData.GetDeviceDataForOrderDevice(1, false);
            var response = _service.OrderDevice()
              .WithOrderDevice(orderDeviceInfo)
              .WithDeviceData(deviceData)
              .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreateAccountPhysicalDevice() {
            var bankAccountInfo = TestAccountData.GetBankAccountData();
            var accountPersonalInfo = TestAccountData.GetUserPersonalData();
            var deviceData = TestAccountData.GetDeviceForPhysicalDevice(1, true);
            var userBusinessInfo = TestAccountData.GetBusinessData();
            var ownersInfo = TestAccountData.GetBeneficialOwnerData();
            var mailingAddressInfo = TestAccountData.GetMailingAddress();
            var creditCardInfo = TestAccountData.GetCreditCardData();

            var response = _service.CreateAccount()
              .WithBankAccountData(bankAccountInfo)
              .WithUserPersonalData(accountPersonalInfo)
              .WithBusinessData(userBusinessInfo)
              .WithDeviceData(deviceData)
              .WithMailingAddress(mailingAddressInfo)
              .WithBeneficialOwnerData(ownersInfo)
              .WithCreditCardData(creditCardInfo)
              .WithTimeZone("UTC")
              .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.PayFacData.AccountNumber);
            Assert.IsNotNull(response.PayFacData.Password);
            Assert.IsNotNull(response.PayFacData.SourceEmail);
        }

        [TestMethod]
        public void TestFailedKYCStatus66() {
            var bankAccountInfo = TestAccountData.GetBankAccountData();
            var accountPersonalInfo = TestAccountData.GetUserPersonalData("01-01-1971");
            var deviceData = TestAccountData.GetDeviceData(1, false);
            var userBusinessInfo = TestAccountData.GetBusinessData();
            var ownersInfo = TestAccountData.GetBeneficialOwnerData();
            var mailingAddressInfo = TestAccountData.GetMailingAddress();
            var creditCardInfo = TestAccountData.GetCreditCardData();

            var response = _service.CreateAccount()
              .WithBankAccountData(bankAccountInfo)
              .WithUserPersonalData(accountPersonalInfo)
              .WithBusinessData(userBusinessInfo)
              .WithDeviceData(deviceData)
              .WithMailingAddress(mailingAddressInfo)
              .WithBeneficialOwnerData(ownersInfo)
              .WithCreditCardData(creditCardInfo)
              .WithTimeZone("UTC")
              .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("66", response.ResponseCode);
        }

        #region EDIT ACCOUNT TESTS
        [TestMethod]
        public void EditAccountInformation() {
            Random rand = new Random((int)DateTime.Now.Ticks);

            var accountPersonalInfo = new UserPersonalData()
            {
                DayPhone = "4464464464",
                EveningPhone = "4464464464",
                ExternalID = rand.Next(1000000, 999999999).ToString(),
                FirstName = "John",
                LastName = "Doe",
                MiddleInitial = "A",
                SourceEmail = string.Format($"user{rand.Next(1, 10000)}@user.com")
            };

            var response = _service.EditAccount()
                .WithAccountNumber("718135662")
                .WithUserPersonalData(accountPersonalInfo)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EditAccountPassword() {
            Random rand = new Random((int)DateTime.Now.Ticks);

            var response = _service.EditAccount()
                .WithAccountNumber("718135662")
                .WithPassword("testPwd_" + rand.Next(1, 100))
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EditAccountAddress() {
            var personalInfo = new UserPersonalData()
            {
                UserAddress = new Address()
                {
                    StreetAddress1 = "124 Main St.",
                    City = "Downtown",
                    State = "NJ",
                    PostalCode = "12345",
                    Country = "USA"
                },
                MailingAddress = new Address()
                {
                    StreetAddress1 = "125 Main St.",
                    City = "Downtown",
                    State = "NJ",
                    PostalCode = "12345",
                    Country = "USA"
                }
            };

            var response = _service.EditAccount()
                .WithAccountNumber("718135662")
                .WithUserPersonalData(personalInfo)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EditAccountPermissions() {
            var response = _service.EditAccount()
                .WithAccountNumber("718135662")
                .WithAccountPermissions(TestAccountData.GetAccountPermissions())
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EditAccountTimeZone() {
            // In ProPay, the TimeZone is edited as part of the User Personal Data. This value is not part of the UserPersonalData object in the SDK, though.

            // First, populate the UserPersonalData object
            // All items originally sent during account creation must be provided or they will be overwritten with blank spaces.
            var userPersonalData = new UserPersonalData()
            {
                DayPhone = "4464464464",
                EveningPhone = "4464464464",
                FirstName = "John",
                LastName = "Doe",
                MiddleInitial = "A",
                SourceEmail = "user840@user.com"
            };

            // Now call EditAccount, and in addition to sending the UserPersonalData, also send the TimeZone with one of the approved values.
            // See comment on WithTimeZone for values, or the ProPay documentation for further elaboration.
            var response = _service.EditAccount()
                .WithAccountNumber("718216467")
                .WithUserPersonalData(userPersonalData)
                .WithTimeZone("ET")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        #endregion

        [TestMethod]
        public void ResetPassword() {
            var response = _service.ResetPassword()
                .WithAccountNumber("718135662")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.PayFacData.Password);
        }

        #region RENEW ACCOUNT TESTS
        [TestMethod]
        public void RenewAccount() {
            var response = _service.RenewAccount()
                .WithAccountNumber("718135662")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void RenewAccountByCreditCard()
        {
            var response = _service.RenewAccount()
                .WithAccountNumber("718135662")
                .WithRenewalAccountData(TestAccountData.GetRenewAccountDetails(true))
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void RenewAccountByBankAccount()
        {
            var response = _service.RenewAccount()
                .WithAccountNumber("718135662")
                .WithRenewalAccountData(TestAccountData.GetRenewAccountDetails(false))
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
        #endregion

        [TestMethod]
        public void UpdateAccountBeneficialOwnership() {
            var beneficialOwners = TestAccountData.GetBeneficialOwnerData();

            var response = _service.UpdateBeneficialOwnershipInfo()
                .WithAccountNumber("718134589") // This account must have been created with a beneficial owner count specified, but no owner details passed
                .WithBeneficialOwnerData(beneficialOwners)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.PayFacData.BeneficialOwnerDataResults);
        }

        [TestMethod]
        public void DisownAccount() {
            var response = _service.DisownAccount()
                .WithAccountNumber("718581037") // The account being "disowned" needs to have another affiliation set. Contact propayimplementations@tsys.com and they will set one if necessary
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void UploadDocumentChargeback() {
            var docUploadData = new DocumentUploadData()
            {
                DocumentName = "TestDocCB_12345",
                TransactionReference = "2",
                DocumentPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ProPay\TestData\TestDocChargeback.docx")
            };

            var response = _service.UploadDocumentChargeback()
                .WithAccountNumber("718134204")
                .WithDocumentUploadData(docUploadData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void UploadDocument() {
            var docUploadData = new DocumentUploadData()
            {
                DocumentName = "TestDoc_12345",
                DocCategory = DocumentCategory.VERIFICATION,
                DocumentPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ProPay\TestData\TestDoc.docx")
            };

            var response = _service.UploadDocument()
                .WithAccountNumber("718134204")
                .WithDocumentUploadData(docUploadData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void UploadDocumentChargebackByDocumentString()
        {
            var docUploadData = new DocumentUploadData()
            {
                DocumentName = "TestDocCB_12345",
                TransactionReference = "2",
                Document = TestAccountData.GetDocumentBase64String(@"ProPay\TestData\TestDocChargeback.docx"),
                DocType = FileType.DOCX
            };

            var response = _service.UploadDocumentChargeback()
                .WithAccountNumber("718134204")
                .WithDocumentUploadData(docUploadData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void UploadDocumentByDocumentString() {
            var docUploadData = new DocumentUploadData()
            {
                DocumentName = "TestDoc_12345",
                DocCategory = DocumentCategory.VERIFICATION,
                Document = TestAccountData.GetDocumentBase64String(@"ProPay\TestData\TestDoc.docx"),
                DocType = FileType.DOCX
            };

            var response = _service.UploadDocument()
                .WithAccountNumber("718134204")
                .WithDocumentUploadData(docUploadData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void ObtainSSOKey() {
            var ssoRequestData = new SSORequestData()
            {
                ReferrerURL = "https://www.globalpaymentsinc.com/",
                IPAddress = "40.81.11.219",
                IPSubnetMask = "255.255.255.0"
            };

            var response = _service.ObtainSSOKey()
                .WithAccountNumber("718134204")
                .WithSSORequestData(ssoRequestData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.PayFacData.AuthToken);
        }

        // This ProPay method only works with Canadian affiliations
        [TestMethod]
        public void UpdateBankAccountOwnershipInfo() {
            var primaryOwner = new BankAccountOwnershipData()
            {
                FirstName = "Style",
                LastName = "Stallone",
                OwnerAddress = new Address()
                {
                    StreetAddress1 = "3400 N Ashton Blvd",
                    StreetAddress2 = "3401 M Ashton Clad",
                    StreetAddress3 = "3402 L Ashton Blvd",
                    City = "Orlando",
                    State = "FL",
                    PostalCode = "X0A 0H0",
                    Country = "CAN"
                },
                PhoneNumber = "123456789"
            };
            var secondaryOwner = new BankAccountOwnershipData()
            {
                FirstName = "Thomas",
                LastName = "Hanks",
                OwnerAddress = new Address()
                {
                    StreetAddress1 = "1970 Diamond Blvd",
                    StreetAddress2 = "1971 Diamond Blvd",
                    StreetAddress3 = "1972 Diamond Blvd",
                    City = "Orlando",
                    State = "FL",
                    PostalCode = "X0A 0H0",
                    Country = "CAN"
                },
                PhoneNumber = "123456789"
            };

            var response = _service.UpdateBankAccountOwnershipInfo()
                .WithAccountNumber("718134204")
                .WithPrimaryBankAccountOwner(primaryOwner)
                .WithSecondaryBankAccountOwner(secondaryOwner)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
    }
}
