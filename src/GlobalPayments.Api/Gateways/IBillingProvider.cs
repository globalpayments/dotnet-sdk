using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities.Billing;

namespace GlobalPayments.Api.Gateways {
    internal interface IBillingProvider {
        /// <summary>
        /// Indicates if the Billing Gateway hosts merchant bill records
        /// </summary>
        bool IsBillDataHosted { get; }
        BillingResponse ProcessBillingRequest(BillingBuilder builder);
    }
}
