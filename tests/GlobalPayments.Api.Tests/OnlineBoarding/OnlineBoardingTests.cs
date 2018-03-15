using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.OnlineBoarding;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.OnlineBoarding {
    [TestClass]
    public class OnlineBoardingTests {
        BoardingService _service;

        public OnlineBoardingTests() {
            _service = new BoardingService(new ServicesConfig {
                BoardingConfig = new BoardingConfig {
                    Portal = "Galardi"
                }
            });
        }

        [TestMethod]
        public void NewApplication() {
            BoardingApplication application = _service.NewApplication();

            // DBA Information
            application.MerchantInfo = new MerchantInfo {
                MerchantDbaName = "Automated Application",
                MerchantEmail = "name@somedomain.com",
                MerchantPhone = "1234567890",
                MerchantEmailFirstName = "Russell",
                MerchantEmailLastName = "Everett",
                MerchantPrimaryContactPhone = "1234567890",
                MerchantStoreNumber = "123",
                MerchantNumberOfLocations = 1,
                MerchantStreet = "1 Heartland Way",
                MerchantCity = "Jeffersonville",
                MerchantStatesSelect = States.Indiana,
                MerchantZip = "12345",
                FederalTaxId = "123456789"
            };
            application.LegalInfoSameAsMerchant = true;

            // business info
            application.BusinessInfo = new BusinessInfo {
                OwnershipTypeSelect = TypeofOwnershipSelect.SoleProprietorship,
                IsFederalIdSignersSsn = true,
                DataCompromiseOrComplianceInvestigation = false,
                EverFiledBankrupt = false,
                DateBusinessAcquired = new DateTime(2000, 1, 1),
                DataStorageOrMerchantServicer = false,
                DateAcceptingCreditCardsStarted = new DateTime(2000, 12, 12)
            };

            // owners
            var owner = new OwnerOfficer {
                FirstName = "Russell",
                LastName = "Everett",
                Title = "Developer",
                DateOfBirth = new DateTime(1977, 09, 12),
                HomePhone = "1234567890",
                SSN = "123456789",
                OwnershipTypeSelect = OwnerOfficerSelect.Owner,
                Street = "1 Heartland Way",
                City = "Jeffersonville",
                Zip = "12345",
                StateSelect = States.Indiana,
                EquityOwnership = "100",
                EmailAddress = "russell.everett@e-hps.com"
            };
            application.OwnerOfficers.Add(owner);

            // banking information
            application.BankingInfo = new BankingInfo {
                BankName = "Wells Fargo",
                BankCity = "St. Louis",
                BankStatesSelect = States.Missouri,
                BankZip = "12345"
            };

            // bank accounts
            application.BankingInfo.BankAccounts.Add(new BankAccount {
                AccountNumber = "12345678901234",
                TransitRouterAbaNumber = "123456789",
                AccountTypeSelect = BankAccountTypeSelect.Checking,
                TransferMethodTypeSelect = FundsTransferMethodSelect.DepositsAndFees
            });

            _service.SubmitApplication("D9E5EEB0-7709-4E60-B0CE-0ABABC1EBACE", application);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void BadInvitationToken() {
            _service.SubmitApplication("BAD-TOKEN", _service.NewApplication());
        }
    }
}
