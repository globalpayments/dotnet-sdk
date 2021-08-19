using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Logging;
using System.Net;

namespace GlobalPayments.Api.Gateways.BillPay {
    /// <summary>
    /// Factory method to create and return the request object based on the transaction type
    /// </summary>
    internal class ManagementRequest : GatewayRequestBase {
        public ManagementRequest(Credentials credentials, string serviceUrl, int timeout) {
            this.Credentials = credentials;
            this.ServiceUrl = serviceUrl;
            this.Timeout = timeout;
        }

        internal ManagementRequest WithRequestLogger(IRequestLogger requestLogger) {
            RequestLogger = requestLogger;
            return this;
        }

        internal ManagementRequest WithWebProxy(IWebProxy webProxy) {
            WebProxy = webProxy;
            return this;
        }

        internal Transaction Execute(ManagementBuilder builder, bool isBillDataHosted) {
            switch (builder.TransactionType) {
                case TransactionType.Refund:
                case TransactionType.Reversal:
                case TransactionType.Void:
                        return ReversePayment(builder);
                case TransactionType.TokenUpdate:
                    if (builder.PaymentMethod is CreditCardData card) {
                        return UpdateTokenExpirationDate(card);
                    }

                    throw new UnsupportedTransactionException();
                default:
                    throw new UnsupportedTransactionException();
            }
        }

        private Transaction ReversePayment(ManagementBuilder builder) {
            var et = new ElementTree();
            var envelope = CreateSOAPEnvelope(et, "ReversePayment");
            var request = new ReversePaymentRequest(et)
                .Build(envelope, builder, Credentials);

            var response = DoTransaction(request, publicEndpoint);
            var result = new ReversalResponse()
                .WithResponseTagName("ReversePaymentResponse")
                .WithResponse(response)
                .Map();

            if (result.ResponseCode == "0") {
                return result;
            }

            throw new GatewayException(message: "There was an error attempting to reverse the payment", responseCode: result.ResponseCode, responseMessage: result.ResponseMessage);
        }

        private Transaction UpdateTokenExpirationDate(CreditCardData card) {
            var et = new ElementTree();
            var envelope = CreateSOAPEnvelope(et, "UpdateTokenExpirationDate");
            var request = new UpdateTokenRequest(et)
                .Build(envelope, card, Credentials);

            var response = DoTransaction(request, publicEndpoint);
            var result = new UpdateTokenResponse()
                .WithResponseTagName("UpdateTokenExpirationDateResponse")
                .WithResponse(response)
                .Map();

            if (result.ResponseCode == "0") {
                return result;
            }

            throw new GatewayException(message: "There was an error attempting to the token expiry information", responseCode: result.ResponseCode, responseMessage: result.ResponseMessage);
        }
    }
}
