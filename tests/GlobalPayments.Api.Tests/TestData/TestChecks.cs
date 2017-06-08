using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Tests.TestData
{
    public static class TestChecks
    {
        public static eCheck Certification(string secCode = SecCode.PPD, CheckType checkType = CheckType.PERSONAL, AccountType accountType = AccountType.CHECKING, string checkName = null) {
            return new eCheck {
                AccountNumber = "24413815",
                RoutingNumber = "490000018",
                CheckType = checkType,
                SecCode = secCode,
                AccountType = accountType,
                EntryMode = EntryMethod.Manual,
                CheckHolderName = "John Doe",
                DriversLicenseNumber = "09876543210",
                DriversLicenseState = "TX",
                PhoneNumber = "8003214567",
                BirthYear = 1997,
                SsnLast4 = "4321",
                CheckName = checkName
            };
        }
    }
}
