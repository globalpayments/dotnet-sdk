using GlobalPayments.Api.Builders;

namespace GlobalPayments.Api.Gateways {
    interface IRecurringService {
        bool SupportsRetrieval { get; }
        bool SupportsUpdatePaymentDetails { get; }
        T ProcessRecurring<T>(RecurringBuilder<T> builder) where T : class;
    }
}
