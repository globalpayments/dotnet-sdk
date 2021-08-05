using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.PaymentMethods {
    public class Ewic : IPaymentMethod, IBalanceable, IChargable, IPinProtected {
        public PaymentMethodType PaymentMethodType { get { return PaymentMethodType.Ewic; } }
        public string PinBlock { get; set; }

        public AuthorizationBuilder BalanceInquiry(InquiryType? inquiry = null) {
            return new AuthorizationBuilder(TransactionType.Balance, this).WithBalanceInquiryType(inquiry);            
        }

        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale, this).WithAmount(amount);
        }

    }
}
