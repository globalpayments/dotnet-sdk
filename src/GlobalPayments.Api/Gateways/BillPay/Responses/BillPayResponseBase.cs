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
    }
}
