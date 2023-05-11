using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.PaymentMethods
{
    public class AccountFunds : IPaymentMethod
    {
        public PaymentMethodType PaymentMethodType => PaymentMethodType.Account_Funds;
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string MerchantId { get; set; }
        public string RecipientAccountId { get; set; }
        public UsableBalanceMode? UsableBalanceMode { get; set; }

        /// <summary>
        /// Transfers specific funds to another merchant account.
        /// </summary>
        /// <param name="amount">The amount to transfer/return</param>
        public AuthorizationBuilder Transfer(decimal? amount = null)
        {
            return new AuthorizationBuilder(TransactionType.TransferFunds)
                .WithAmount(amount)
                .WithPaymentMethod(this);
        }
    }
}
