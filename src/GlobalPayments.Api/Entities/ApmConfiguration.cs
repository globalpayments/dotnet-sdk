using GlobalPayments.Api.Entities.Enums;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Represents an alternative payment method (APM) provider configuration for a hosted payment page,
    /// including the provider and the payment plans it offers (e.g. Cashpresso BNPL).
    /// </summary>
    public class ApmConfiguration {

        /// <summary>
        /// The alternative payment method provider (e.g. <see cref="AlternativePaymentType.CASHPRESSO"/>).
        /// </summary>
        public AlternativePaymentType? Provider { get; set; }

        /// <summary>
        /// The Cashpresso payment plans offered by the provider on the hosted payment page.
        /// </summary>
        public CashpressoPaymentPlan[] PaymentPlans { get; set; }
    }
}
