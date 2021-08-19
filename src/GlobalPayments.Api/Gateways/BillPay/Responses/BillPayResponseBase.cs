using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal abstract class BillPayResponseBase<T> : IBillPayResponse<T> {
        internal Element response;
        internal string responseTagName;

        public IBillPayResponse<T> WithResponseTagName(string tagName) {
            this.responseTagName = tagName;

            return this;
        }

        public IBillPayResponse<T> WithResponse(string response) {
            this.response = new ElementTree(response)
                .Get(responseTagName);

            return this;
        }

        public abstract T Map();

        /// <summary>
        /// Retrieves the first BillPay message
        /// </summary>
        /// <param name="response">The XML Element of the billPay response</param>
        /// <returns></returns>
        internal string GetFirstResponseCode(Element response) {
            var message = response.Get("a:Messages");

            return message.GetValue<string>("a:Code");
        }

        /// <summary>
        /// Retrieves the first BillPay message
        /// </summary>
        /// <param name="response">The XML Element of the billPay response</param>
        /// <returns></returns>
        internal string GetFirstResponseMessage(Element response) {
            var message = response.Get("a:Messages");

            return message.GetValue<string>("a:MessageDescription");
        }

        /// <summary>
        /// Convert a string value payment method to a PaymentMethodType enum value
        /// </summary>
        /// <param name="paymentMethod">A string representing the payment method type</param>
        /// <returns>The enumeration value of the specified payment method, if supported</returns>
        internal PaymentMethodType SetPaymentMethodType(string paymentMethod) {
            PaymentMethodType paymentMethodType;
            if (paymentMethod.Contains("Credit"))
                paymentMethodType = PaymentMethodType.Credit;
            else if (paymentMethod.Contains("Debit"))
                paymentMethodType = PaymentMethodType.Debit;
            else if (paymentMethod.Contains("ACH"))
                paymentMethodType = PaymentMethodType.ACH;
            else
                throw new UnsupportedTransactionException();

            return paymentMethodType;
        }
    }
}
