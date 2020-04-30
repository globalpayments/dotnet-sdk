using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.PaymentMethods {
    interface IBalanceable {
        AuthorizationBuilder BalanceInquiry(InquiryType? inquiry);
    }
}
