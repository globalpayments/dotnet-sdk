using System;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.PaymentMethods
{
    /// <summary>
    /// Use cash as a payment method.
    /// </summary>
    public class Cash : IPaymentMethod, IChargable, IRefundable {
        /// <summary>
        /// Set to `PaymentMethodType.Cash` for internal methods.
        /// </summary>
        public PaymentMethodType PaymentMethodType { get { return PaymentMethodType.Cash; } }

        /// <summary>
        /// Sends a cash sale transaction to the gateway.
        /// </summary>
        /// <remarks>
        /// This transaction is purely informational. No contact with the issuer
        /// or settlement will occur as the cash exchange will happen directly
        /// between the merchant and the consumer.
        /// </remarks>
        /// <exception cref="NotImplementedException">
        /// Thrown when the configured gateway does not support cash transactions.
        /// </exception>
        /// <param name="amount">The amount of the transaction</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder Charge(decimal? amount = default(decimal?)) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends a cash refund transaction to the gateway.
        /// </summary>
        /// <remarks>
        /// This transaction is purely informational. No contact with the issuer
        /// or settlement will occur as the cash exchange will happen directly
        /// between the merchant and the consumer.
        /// </remarks>
        /// <exception cref="NotImplementedException">
        /// Thrown when the configured gateway does not support cash transactions.
        /// </exception>
        /// <param name="amount">The amount of the transaction</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder Refund(decimal? amount) {
            throw new NotImplementedException();
        }
    }
}
