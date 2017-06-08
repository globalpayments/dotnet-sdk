using System;

namespace GlobalPayments.Api.Entities
{
    public class ApiException : Exception {
        public ApiException(string message = null, Exception innerException = null) : base(message, innerException) { }
    }

    public class BuilderException : ApiException {
        public BuilderException(string message = null) : base(message) { }
    }

    public class ConfigurationException : ApiException {
        public ConfigurationException(string message) : base(message) { }
    }

    public class GatewayException : ApiException {
        public string ResponseCode { get; private set; }
        public string ResponseMessage { get; private set; }

        internal GatewayException(string message, string responseCode = null, string responseMessage = null) : base(message) {
            ResponseCode = responseCode;
            ResponseMessage = responseMessage;
        }

        internal GatewayException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class MessageException : ApiException {
        public MessageException(string message, Exception innerException = null) : base(message, innerException) { }
    }

    public class UnsupportedTransactionException : ApiException {
        public UnsupportedTransactionException(string message = null) : base(message ?? "Transaction type not supported for this payment method.") { }
    }
}
