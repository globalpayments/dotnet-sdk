using System;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Billing;

namespace GlobalPayments.Api.PaymentMethods {
    /// <summary>
    /// Use eCheck/ACH as a payment method.
    /// </summary>
    public class eCheck : IPaymentMethod, IChargable, ITokenizable, IAuthable, IRefundable {
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
        public bool CheckGuarantee { get; set; }
        public string CheckReference { get; set; }
        public string MerchantNotes { get; set; }
        public Address BankAddress { get; set; }

        /// <summary>
        /// Set to `PaymentMethodType.ACH` for internal methods.
        /// </summary>
        public PaymentMethodType PaymentMethodType { get { return PaymentMethodType.ACH; } }

        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale, this).WithAmount(amount);
        }

        /// <summary>
        /// Gets token information for the specified token
        /// </summary>
        public Transaction GetTokenInformation(string configName = "default") {
            var response = new AuthorizationBuilder(TransactionType.GetTokenInfo, this).Execute(configName);
            return response;
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
            return Tokenize(true, configName);
        }
        public string Tokenize(bool validateCard, string configName) {
            TransactionType type = validateCard ? TransactionType.Verify : TransactionType.Tokenize;

            var response = new AuthorizationBuilder(type, this)
                .WithRequestMultiUseToken(true)
                .Execute(configName);
            return response.Token;
        }

        public string Tokenize(bool validateCard, Address billingAddress, Customer customerData, string configName = "default") {
            TransactionType type = validateCard ? TransactionType.Verify : TransactionType.Tokenize;

            var builder = new AuthorizationBuilder(type, this)
                .WithRequestMultiUseToken(validateCard)
                .WithPaymentMethodUsageMode(PaymentMethodUsageMode.Multiple);

            if (billingAddress != null) {
                builder = builder.WithAddress(billingAddress);
            }
            if (customerData != null) {
                builder = builder.WithCustomerData(customerData);
            }

            var response = builder.Execute(configName);
            return response.Token;
        }

        public bool UpdateTokenExpiry(string configName = "default") {
            throw new UnsupportedTransactionException();
        }       

        public AuthorizationBuilder Authorize(decimal? amount = null, bool isEstimated = false) {
            return new AuthorizationBuilder(TransactionType.Auth, this)
                    .WithAmount(amount);
        }

        public AuthorizationBuilder Refund(decimal? amount = null)
        {
            return new AuthorizationBuilder(TransactionType.Refund, this)
                .WithAmount(amount);
        }

        string ITokenizable.Tokenize(string configName, PaymentMethodUsageMode paymentMethodUsageMode) {
            throw new NotImplementedException();
        }

        string ITokenizable.Tokenize(bool validateCard, string configName, PaymentMethodUsageMode paymentMethodUsageMode) {
            throw new NotImplementedException();
        }
    }
}
