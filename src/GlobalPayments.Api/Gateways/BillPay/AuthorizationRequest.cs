using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.Logging;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System.Net;

namespace GlobalPayments.Api.Gateways.BillPay {
    /// <summary>
    /// Factory method to create and return the request object based on the transaction type
    /// </summary>
    internal class AuthorizationRequest : GatewayRequestBase {
        public AuthorizationRequest(Credentials credentials, string serviceUrl, int timeout) {
            this.Credentials = credentials;
            this.ServiceUrl = serviceUrl;
            this.Timeout = timeout;
        }

        internal AuthorizationRequest WithRequestLogger(IRequestLogger requestLogger) {
            RequestLogger = requestLogger;
            return this;
        }

        internal AuthorizationRequest WithWebProxy(IWebProxy webProxy) {
            WebProxy = webProxy;
            return this;
        }

        internal Transaction Execute(AuthorizationBuilder builder, bool isBillDataHosted) {
            switch (builder.TransactionType) {
                case TransactionType.Sale:
                    if (isBillDataHosted) {
                        if (builder.RequestMultiUseToken) {
                            return MakePaymentReturnToken(builder);
                        }

                        return MakePayment(builder);
                    } else {
                        if (builder.RequestMultiUseToken) {
                            return MakeBlindPaymentReturnToken(builder);
                        }

                        return MakeBlindPayment(builder);
                    }
                case TransactionType.Verify:
                    if (!builder.RequestMultiUseToken) {
                        throw new UnsupportedTransactionException();
                    }

                    if (builder.PaymentMethod is eCheck) {
                        return GetAchToken(builder);
                    }

                    return GetToken(builder);
                default:
                    throw new UnsupportedTransactionException();
            }
        }

        private Transaction MakePaymentReturnToken(AuthorizationBuilder builder) {
            var et = new ElementTree();
            var envelope = CreateSOAPEnvelope(et, "MakePaymentReturnToken");
            var request = new MakePaymentReturnTokenRequest(et)
                .Build(envelope, builder, Credentials);

            var response = DoTransaction(request, publicEndpoint);
            var result = new TransactionResponse()
                .WithResponseTagName("MakePaymentReturnTokenResponse")
                .WithResponse(response)
                .Map();

            if (result.ResponseCode == "0") {
                return result;
            }

            throw new GatewayException(message: "An error occurred attempting to make the payment", responseCode: result.ResponseCode, responseMessage: result.ResponseMessage);
        }

        private Transaction MakeBlindPaymentReturnToken(AuthorizationBuilder builder) {
            var et = new ElementTree();
            var envelope = CreateSOAPEnvelope(et, "MakeBlindPaymentReturnToken");
            var request = new MakeBlindPaymentReturnTokenRequest(et)
                .Build(envelope, builder, Credentials);
            var response = DoTransaction(request, publicEndpoint);
            var result = new TransactionResponse()
                .WithResponseTagName("MakeBlindPaymentReturnTokenResponse")
                .WithResponse(response)
                .Map();

            if (result.ResponseCode == "0") {
                return result;
            }

            throw new GatewayException(message: "An error occurred attempting to make the payment", responseCode: result.ResponseCode, responseMessage: result.ResponseMessage);
        }

        private Transaction MakeBlindPayment(AuthorizationBuilder builder) {
            var et = new ElementTree();
            var envelope = CreateSOAPEnvelope(et, "MakeBlindPayment");
            var request = new MakeBlindPaymentRequest(et)
                .Build(envelope, builder, Credentials);
            var response = DoTransaction(request, publicEndpoint);
            var result = new TransactionResponse()
                .WithResponseTagName("MakeBlindPaymentResponse")
                .WithResponse(response)
                .Map();

            if (result.ResponseCode == "0") {
                return result;
            }

            throw new GatewayException(message: "An error occurred attempting to make the payment", responseCode: result.ResponseCode, responseMessage: result.ResponseMessage);
        }

        private Transaction MakePayment(AuthorizationBuilder builder) {
            var et = new ElementTree();
            var envelope = CreateSOAPEnvelope(et, "MakePayment");
            var request = new MakePaymentRequest(et)
                .Build(envelope, builder, Credentials);
            var response = DoTransaction(request, publicEndpoint);
            var result = new TransactionResponse()
                .WithResponseTagName("MakePaymentResponse")
                .WithResponse(response)
                .Map();

            if (result.ResponseCode == "0") {
                return result;
            }

            throw new GatewayException(message: "An error occurred attempting to make the payment", responseCode: result.ResponseCode, responseMessage: result.ResponseMessage);
        }

        private Transaction GetToken(AuthorizationBuilder builder) {
            var et = new ElementTree();
            var envelope = CreateSOAPEnvelope(et, "GetToken");
            var request = new GetTokenRequest(et)
                .Build(envelope, builder, Credentials);
            var response = DoTransaction(request, publicEndpoint);
            var result = new TokenRequestResponse()
                .WithResponseTagName("GetTokenResponse")
                .WithResponse(response)
                .Map();

            if (result.ResponseCode == "0") {
                return result;
            }

            throw new GatewayException(message: "An error occurred attempting to create the token", responseCode: result.ResponseCode, responseMessage: result.ResponseMessage);
        }

        private Transaction GetAchToken(AuthorizationBuilder builder) {
            var et = new ElementTree();
            var envelope = CreateSOAPEnvelope(et, "GetToken");
            var request = new GetAchTokenRequest(et)
                .Build(envelope, builder, Credentials);
            var response = DoTransaction(request, publicEndpoint);
            var result = new TokenRequestResponse()
                            .WithResponseTagName("GetTokenResponse")
                            .WithResponse(response)
                            .Map();

            if (result.ResponseCode == "0") {
                return result;
            }

            throw new GatewayException(message: "An error occurred attempting to create the token", responseCode: result.ResponseCode, responseMessage: result.ResponseMessage);
        }
    }
}
