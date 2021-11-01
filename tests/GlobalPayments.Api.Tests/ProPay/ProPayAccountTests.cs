﻿using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlobalPayments.Api.Entities;
using System;
using Environment = GlobalPayments.Api.Entities.Environment;
using GlobalPayments.Api.Tests.ProPay.TestData;
using GlobalPayments.Api.Entities.PayFac;

namespace GlobalPayments.Api.Tests.ProPay {
    [TestClass]
    public class ProPayAccountTests {
        private PayFacService _service;

        public ProPayAccountTests() {
            _service = new PayFacService();
            ServicesContainer.ConfigureService(new PorticoConfig()
            {
                CertificationStr = "5dbacb0fc504dd7bdc2eadeb7039dd",
                TerminalID = "7039dd",
                Environment = Environment.TEST,
                X509CertificatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ProPay\TestData\testCertificate.crt"),
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
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.PayFacData.AccountNumber);
            Assert.IsNotNull(response.PayFacData.Password);
            Assert.IsNotNull(response.PayFacData.SourceEmail);
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
                .WithAccountNumber("718134233") // The account being "disowned" needs to have another affiliation set. Contact propayimplementations@tsys.com and they will set one if necessary
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
                DocCategory = "Verification",
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
