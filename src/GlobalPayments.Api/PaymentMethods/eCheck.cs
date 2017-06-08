using System;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.PaymentMethods {
    public class eCheck : IPaymentMethod, IChargable {
        public string AccountNumber { get; set; }
        public AccountType? AccountType { get; set; }
        public bool AchVerify { get; set; }
        public int BirthYear { get; set; }
        public string CheckHolderName { get; set; }
        public string CheckName { get; set; }
        public string CheckNumber { get; set; }
        public CheckType? CheckType { get; set; }
        public bool CheckVerify { get; set; }
        public string DriversLicenseNumber { get; set; }
        public string DriversLicenseState { get; set; }
        public EntryMethod EntryMode { get; set; }
        public string MicrNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string RoutingNumber { get; set; }
        public string SecCode { get; set; }
        public string SsnLast4 { get; set; }
        public string Token { get; set; }

        public PaymentMethodType PaymentMethodType { get { return PaymentMethodType.ACH; } }

        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale, this).WithAmount(amount);
        }
    }
}
