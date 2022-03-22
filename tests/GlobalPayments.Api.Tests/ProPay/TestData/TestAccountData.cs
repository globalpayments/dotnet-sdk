using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.PayFac;
using GlobalPayments.Api.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Tests.ProPay.TestData
{
    public class TestAccountData {
        public static BankAccountData GetBankAccountData() {
            var bankAccountInformation = new BankAccountData()
            {
                AccountCountryCode = "USA",
                AccountName = "MyBankAccount",
                AccountNumber = "123456789",
                AccountOwnershipType = "C",
                RoutingNumber = "102000076"
            };

            return bankAccountInformation;
        }

        public static BusinessData GetBusinessData() {
            var businessData = new BusinessData()
            {
                BusinessLegalName = "LegalName",
                DoingBusinessAs = "PPA",
                EmployerIdentificationNumber = new Random((int)DateTime.Now.Ticks).Next(100000000, 999999999).ToString(),
                BusinessDescription = "Accounting Services",
                WebsiteURL = "https://www.propay.com",
                MerchantCategoryCode = "5399",
                MonthlyBankCardVolume = "50000",
                AverageTicket = "100",
                HighestTicket = "300",
                BusinessAddress = new Address()
                {
                    StreetAddress1 = "123 Main St.",
                    City = "Downtown",
                    State = "NJ",
                    PostalCode = "12345",
                    Country = "USA"
                }
            };

            return businessData;
        }

        public static UserPersonalData GetUserPersonalData() {
            Random rand = new Random((int)DateTime.Now.Ticks);

            var accountPersonalInformation = new UserPersonalData()
            {
                DayPhone = "4464464464",
                EveningPhone = "4464464464",
                ExternalID = rand.Next(1000000, 999999999).ToString(),
                FirstName = "John",
                LastName = "Doe",
                PhonePIN = "1234",
                SourceEmail = string.Format($"user{rand.Next(1, 10000)}@user.com"), // user1@user.com through user99999@user.com
                SSN = "123456789",
                DateOfBirth = "01-01-1981",
                Tier = "TestEIN",
                UserAddress = new Address()
                {
                    StreetAddress1 = "123 Main St.",
                    City = "Downtown",
                    State = "NJ",
                    PostalCode = "12345",
                    Country = "USA"
                }
            };

            return accountPersonalInformation;
        }

        public static ThreatRiskData GetThreatRiskData() {
            var threatRiskData = new ThreatRiskData()
            {
                MerchantSourceIP = "8.8.8.8",
                ThreatMetrixPolicy = "Default",
                ThreatMetrixSessionID = "dad889c1-1ca4-4fq71-8f6f-807eb4408bc7"
            };

            return threatRiskData;
        }

        public static SignificantOwnerData GetSignificantOwnerData() {
            var significantOwnerData = new SignificantOwnerData()
            {
                AuthorizedSignerFirstName = "John",
                AuthorizedSignerLastName = "Doe",
                AuthorizedSignerTitle = "Director"
            };

            return significantOwnerData;
        }

        public static BeneficialOwnerData GetBeneficialOwnerData() {
            var ownersInformation = new BeneficialOwnerData()
            {
                OwnersCount = "2",
                OwnersList = new List<OwnersData>()
                {
                    // First Owner
                    new OwnersData()
                    {
                        FirstName = "First1",
                        LastName = "Last1",
                        Title = "CEO",
                        Email = "abc@qamail.com",
                        DateOfBirth = "11-11-1988",
                        SSN = "123545677",
                        OwnerAddress = new Address()
                        {
                            StreetAddress1 = "123 Main St.",
                            City = "Downtown",
                            State = "NJ",
                            PostalCode = "12345",
                            Country = "USA"
                        }
                    },
                    //Second Owner
                    new OwnersData()
                    {
                        FirstName = "First2",
                        LastName = "Last2",
                        Title = "Director",
                        Email = "abc1@qamail.com",
                        DateOfBirth = "11-11-1989",
                        SSN = "123545677",
                        OwnerAddress = new Address()
                        {
                            StreetAddress1 = "123 Main St.",
                            City = "Downtown",
                            State = "NJ",
                            PostalCode = "12345",
                            Country = "USA"
                        }
                    }
                }
            };

            return ownersInformation;
        }

        public static CreditCardData GetCreditCardData() {
            var card = new CreditCardData()
            {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123",
                CardHolderName = "Joe Smith"
            };

            return card;
        }

        public static BankAccountData GetACHData() {
            var bankAccountInformation = new BankAccountData()
            {
                AccountNumber = "123456789",
                AccountType = "C",
                RoutingNumber = "102000076"
            };

            return bankAccountInformation;
        }

        public static Address GetMailingAddress() {
            var address = new Address()
            {
                StreetAddress1 = "123 Main St.",
                City = "Downtown",
                State = "NJ",
                PostalCode = "12345",
                Country = "USA"
            };

            return address;
        }

        public static BankAccountData GetSecondaryBankAccountData() {
            var bankAccountInformation = new BankAccountData()
            {
                AccountCountryCode = "USA",
                AccountName = "MyBankAccount",
                AccountNumber = "123456788",
                AccountOwnershipType = "Personal",
                AccountType = "C",
                RoutingNumber = "102000076"
            };

            return bankAccountInformation;
        }

        public static GrossBillingInformation GetGrossBillingSettleData() {
            var grossBillingInformation = new GrossBillingInformation()
            {
                GrossSettleBankData = new BankAccountData()
                {
                    AccountCountryCode = "USA",
                    AccountName = "MyBankAccount",
                    AccountNumber = "123456788",
                    AccountOwnershipType = "Personal",
                    AccountType = "C",
                    RoutingNumber = "102000076",
                    AccountHolderName = "John"
                },
                GrossSettleAddress = new Address()
                {
                    StreetAddress1 = "123 Main St.",
                    City = "Downtown",
                    State = "NJ",
                    PostalCode = "12345",
                    Country = "USA"
                },
                GrossSettleCreditCardData = new CreditCardData()
                {
                    Number = "4111111111111111",
                    ExpMonth = 12,
                    ExpYear = 2025,
                    Cvn = "123",
                    CardHolderName = "Joe Smith"
                }
            };

            return grossBillingInformation;
        }

        public static AccountPermissions GetAccountPermissions() {
            var accountPermissions = new AccountPermissions()
            {
                CCProcessing = true
            };

            return accountPermissions;
        }

        public static RenewAccountData GetRenewAccountDetails(bool payByCC) {
            var renewAccountData = new RenewAccountData()
            {
                Tier = "TestEIN",
            };

            if (payByCC) {
                renewAccountData.ZipCode = "12345";
                renewAccountData.CreditCard = new CreditCardData()
                {
                    Number = "4111111111111111",
                    ExpMonth = 12,
                    ExpYear = 2025,
                    Cvn = "123"
                };
            }
            else {
                renewAccountData.PaymentBankAccountNumber = "123456789";
                renewAccountData.PaymentBankRoutingNumber = "102000076";
                renewAccountData.PaymentBankAccountType = "Checking";
            }

            return renewAccountData;
        }

        public static string GetDocumentBase64String(string filepath) {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(filepath));
        }
    }
}
