using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
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
                AccountOwnershipType = "Personal",
                RoutingNumber = "102000076",
                AccountType = "Checking",
                BankName = "First Union"
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
                BusinessType = "D",
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

        public static UserPersonalData GetUserPersonalData(string dob = "01-01-1981") {
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
                DateOfBirth = dob,
                Tier = "test",
                IpSignup = "4.14.150.145",
                USCitizen = true,
                BOAttestation = true,
                TermsAcceptanceIP = "4.14.150.145",
                TermsAcceptanceTimeStamp = "2022-10-27 12:57:08.2021237",
                TermsVersion = ((int)PropayTermsVersion.MerchantUS).ToString(),
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
                Cvn = "999",
                CardHolderName = "Sylvester Stallone"
            };

            return card;
        }

        public static BankAccountData GetACHData() {
            var bankAccountInformation = new BankAccountData()
            {
                AccountNumber = "123456789",
                AccountType = "Checking",
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
                RoutingNumber = "102000076",
                BankName = "My Bank"
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

        public static DeviceData GetDeviceData(int numDeviceTypes = 1, bool withAttributes = true) {
            List<string> deviceTypes = new List<string>() { "Secure Submit", "TestDevice" };

            var deviceData = new DeviceData();
            deviceData.Devices = new List<DeviceInfo>();

            for (int i = 0; i < numDeviceTypes; i++) {
                var deviceInfo = new DeviceInfo();
                if (i >= deviceTypes.Count)
                    break;
                deviceInfo.Name = deviceTypes[i];
                deviceInfo.Quantity = 1;
                if (withAttributes)
                {
                    deviceInfo.Attributes = new List<DeviceAttributeInfo>()
                        {
                            new DeviceAttributeInfo()
                            {
                                Name = "Heartland.AMD.OfficeKey",
                                Value = "123456"
                            }
                        };
                }
                deviceData.Devices.Add(deviceInfo);
            }

            return deviceData;
        }
        public static DeviceData GetDeviceForPhysicalDevice(int numDeviceTypes = 1, bool withAttributes = true) {
            List<string> deviceTypes = new List<string>() { "TestDevice", "Secure Submit" };

            var deviceData = new DeviceData();
            deviceData.Devices = new List<DeviceInfo>();

            for (int i = 0; i < numDeviceTypes; i++) {
                var deviceInfo = new DeviceInfo();
                if (i >= deviceTypes.Count)
                    break;
                deviceInfo.Name = deviceTypes[i];
                deviceInfo.Quantity = 1;
                if (withAttributes) {
                    deviceInfo.Attributes = new List<DeviceAttributeInfo>() {
                            new DeviceAttributeInfo() {
                                Name = "Canada.CP.Language",
                                Value = "en"
                            }
                        };
                }
                deviceData.Devices.Add(deviceInfo);
            }

            return deviceData;
        }


        public static DeviceData GetDeviceDataForOrderDevice(int numDeviceTypes = 1, bool withAttributes = true) {
            List<string> deviceTypes = new List<string>() { "Secure Submit" };

            var deviceData = new DeviceData();
            deviceData.Devices = new List<DeviceInfo>();

            for (int i = 0; i < numDeviceTypes; i++) {
                var deviceInfo = new DeviceInfo();
                if (i >= deviceTypes.Count)
                    break;
                deviceInfo.Name = deviceTypes[i];
                deviceInfo.Quantity = 1;
                if (withAttributes) {
                    deviceInfo.Attributes = new List<DeviceAttributeInfo>() {
                            new DeviceAttributeInfo() {
                                Name = "Heartland.AMD.OfficeKey",
                                Value = "123456"
                            }
                        };
                }
                deviceData.Devices.Add(deviceInfo);
            }

            return deviceData;
        }

        public static OrderDevice GetOrderNewDeviceData() {
            var orderDevice = new OrderDevice() {
                AccountNum = 718580389,//718576800,
                ShipTo = "Test Company",
                ShipToContact = "John Q. Public",
                ShipToAddress = "2675 W 600 N",
                ShipToAddress2 = "Apt G",
                ShipToCity = "Lindon",
                ShipToState = "UT",
                ShipToZip = "84042",
                ShipToPhone = "801-555-1212",
                CardholderName = "Johnny Cage",
                CcNum = "4111111111111111",
                ExpDate = "0427",
                CVV2 = "999",
                BillingZip = "84003"
            };
        
            return orderDevice;
        }

    public static string GetDocumentBase64String(string filepath) {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(filepath));
        }
    }
}
