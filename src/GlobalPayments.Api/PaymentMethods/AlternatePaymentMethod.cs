using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Builders;

namespace GlobalPayments.Api.PaymentMethods {
    public class AlternatePaymentMethod: IPaymentMethod, IChargable {
        /// <summary>
        /// Returns Payment Method Type.
        /// </summary>
        public PaymentMethodType PaymentMethodType { get { return PaymentMethodType.Credit; } }

        /// <summary>
        /// A AlternativePaymentMethodType value representing the
        /// AlternativePaymentMethodType Name.
        /// </summary>
        public AlternativePaymentType? AlternativePaymentMethodType { get; set; }

        /// <summary>
        /// A ReturnUrl is representing after the payment
        /// Where the transaction return to.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// A CancelUrl is representing during the payment
        /// Where the transaction cancels to .
        /// </summary>
        public string CancelUrl { get; set; }

        /// <summary>
        /// A StatusUpdateUrl is representing after the transaction
        /// Where the status response will come like SUCCESS/PENDING
        ///.
        /// </summary>
        public string StatusUpdateUrl { get; set; }

        /// <summary>
        /// A Descriptor value representing About Transaction.
        /// </summary>
        public string Descriptor { get; set; }

        /// <summary>
        /// A Country value representing the Country.
        /// </summary>
        public string Country { get; set; }

        public string AccountHolderName { get; set; }

        /// <summary>
        /// Creates a charge (sale) against the payment method.
        /// </summary>
        /// <param name="amount">The amount of the transaction</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale, this)
                 .WithAmount(amount);
        }
    }
}
