using GlobalPayments.Api.Gateways.Events;
using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// A general error occurred.
    /// </summary>
    public class ApiException : Exception {
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public ApiException(string message = null, Exception innerException = null) : base(message, innerException) { }
    }

    /// <summary>
    /// A builder error occurred. Check the method calls against the builder.
    /// </summary>
    public class BuilderException : ApiException {
        /// <param name="message">The exception message</param>
        public BuilderException(string message = null) : base(message) { }
    }

    /// <summary>
    /// An account or SDK configuration error occurred.
    /// </summary>
    public class ConfigurationException : ApiException {
        /// <param name="message">The exception message</param>
        public ConfigurationException(string message) : base(message) { }
    }

    /// <summary>
    /// An error occurred on the gateway.
    /// </summary>
    public class GatewayException : ApiException {
        /// <summary>
        /// The gateway response code.
        /// </summary>
        public string ResponseCode { get; private set; }

        /// <summary>
        /// The gateway response message.
        /// </summary>
        public string ResponseMessage { get; private set; }
        public List<IGatewayEvent> GatewayEvents { get; set; }

        internal GatewayException(string message, string responseCode = null, string responseMessage = null) : base(message) {
            ResponseCode = responseCode;
            ResponseMessage = responseMessage;
        }

        internal GatewayException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// A message to/from the device caused an error.
    /// </summary>
    public class MessageException : ApiException {
        /// <param name="message">The exception message</param>
        public MessageException(string message, Exception innerException = null) : base(message, innerException) { }
    }

    /// <summary>
    /// The built transaction is not supported for the gateway or payment method.
    /// </summary>
    public class UnsupportedTransactionException : ApiException {
        /// <param name="message">The exception message</param>
        public UnsupportedTransactionException(string message = null) : base(message ?? "Transaction type not supported for this payment method.") { }
    }

    public class ValidationException : ApiException {
        public List<string> ValidationErrors { get; private set; }

        public ValidationException(List<string> validationErrors) : base("The application failed validation. Please see the validation errors for specific details.") {
            ValidationErrors = validationErrors;
        }
    }
    public class GatewayTimeoutException : GatewayException {
        public string Host { get; set; }
        public string MessageTypeIndicator { get; set; }
        public string ProcessingCode { get; set; }
        public int ReversalCount { get; set; }
        public string ReversalResponseCode { get; set; }
        public string ReversalResponseText { get; set; }
        public GatewayTimeoutException() : base("The gateway did not respond within the given timeout.") { }
        public GatewayTimeoutException(Exception innerException) : base("The gateway did not respond within the given timeout.", innerException) { }
    }
}
