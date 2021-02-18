using System;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.PaymentMethods {
    /// <summary>
    /// Use eCheck/ACH as a payment method.
    /// </summary>
    public class eCheck : IPaymentMethod, IChargable {
        public string AccountNumber { get; set; }
        public AccountType? AccountType { get; set; }
        public bool AchVerify { get; set; }

        /// <summary>
        /// The name of the issuing Bank
        /// </summary>
        public string BankName { get; set; }
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

        /// <summary>
        /// Set to `PaymentMethodType.ACH` for internal methods.
        /// </summary>
        public PaymentMethodType PaymentMethodType { get { return PaymentMethodType.ACH; } }

        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale, this).WithAmount(amount);
        }

        public bool DeleteToken(string configName = "default") {
            try {
                new ManagementBuilder(TransactionType.TokenDelete)
                    .WithPaymentMethod(this)
                    .Execute(configName);
                return true;
            } catch (ApiException) {
                return false;
            }
        }

        public string Tokenize(string configName = "default") {
            return Tokenize(true, "", configName);
        }
        public string Tokenize(bool verifyCard, string billingPostalCode = "", string configName = "default") {
            TransactionType type = verifyCard ? TransactionType.Verify : TransactionType.Tokenize;

            var response = new AuthorizationBuilder(type, this)
                .WithRequestMultiUseToken(verifyCard)
                .Execute(configName);
            return response.Token;
        }

        public bool UpdateTokenExpiry(string configName = "default") {
            throw new UnsupportedTransactionException();
        }
    }
}
