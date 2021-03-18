using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Logging;
using System.Net;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal sealed class RecurringRequest<T> : GatewayRequestBase where T: class {
        public RecurringRequest(Credentials credentials, string serviceUrl, int timeout) {
            this.Credentials = credentials;
            this.ServiceUrl = serviceUrl;
            this.Timeout = timeout;
        }

        internal RecurringRequest<T> WithRequestLogger(IRequestLogger requestLogger) {
            RequestLogger = requestLogger;
            return this;
        }

        internal RecurringRequest<T> WithWebProxy(IWebProxy webProxy) {
            WebProxy = webProxy;
            return this;
        }

        internal T Execute(RecurringBuilder<T> builder) {
            if (builder.Entity is Customer customer) {
                return CustomerRequest(customer, builder.TransactionType);
            } else if (builder.Entity is RecurringPaymentMethod recurringPaymentMethod) {
                return CustomerAccountRequest(recurringPaymentMethod, builder.TransactionType);
            } else {
                throw new UnsupportedTransactionException();
            }
        }

        private T CustomerRequest(Customer customer, TransactionType type) {
            switch (type) {
                case TransactionType.Create:
                    return CreateSingleSignOnAccount(customer);
                case TransactionType.Edit:
                    return UpdateSingleSignOnAccount(customer);
                case TransactionType.Delete:
                    return DeleteSingleSignOnAccount(customer);
                default:
                    throw new UnsupportedTransactionException();
            }
        }

        private T CustomerAccountRequest(RecurringPaymentMethod paymentMethod, TransactionType type) {
            switch (type) {
                case TransactionType.Create:
                    return CreateCustomerAccount(paymentMethod);
                case TransactionType.Edit:
                    return UpdateCustomerAccount(paymentMethod);
                case TransactionType.Delete:
                    return DeleteCustomerAccount(paymentMethod);
                default:
                    throw new UnsupportedTransactionException();
            }
        }

        private T CreateSingleSignOnAccount(Customer customer) {
            var et = new ElementTree();
            var envelope = CreateSOAPEnvelope(et, "CreateSingleSignOnAccount");
            var request = new CreateSingleSignOnAccountRequest(et)
                .Build(envelope, Credentials, customer);

            var response = DoTransaction(request, publicEndpoint);

            var result = new SingleSignOnAccountResponse()
                .WithResponseTagName("CreateSingleSignOnAccountResponse")
                .WithResponse(response)
                .Map();

            if (result.IsSuccessful) {
                customer.Key = customer.Id;
                return customer as T;
            }

            throw new GatewayException(message: "An error occurred while creating the customer", responseCode: result.ResponseMessage, responseMessage: result.ResponseMessage);
        }

        private T UpdateSingleSignOnAccount(Customer customer) {
            var et = new ElementTree();
            var envelope = CreateSOAPEnvelope(et, "UpdateSingleSignOnAccount");
            var request = new UpdateSingleSignOnAccountRequest(et)
                .Build(envelope, Credentials, customer);

            var response = DoTransaction(request, publicEndpoint);

            var result = new SingleSignOnAccountResponse()
                .WithResponseTagName("UpdateSingleSignOnAccountResponse")
                .WithResponse(response)
                .Map();

            if (result.IsSuccessful) {
                return customer as T;
            }

            throw new GatewayException(message: "An error occurred while updating the customer", responseCode: result.ResponseMessage, responseMessage: result.ResponseMessage);
        }

        private T DeleteSingleSignOnAccount(Customer customer) {
            var et = new ElementTree();
            var envelope = CreateSOAPEnvelope(et, "DeleteSingleSignOnAccount");
            var request = new DeleteSingleSignOnAccountRequest(et)
                .Build(envelope, Credentials, customer);

            var response = DoTransaction(request, publicEndpoint);

            var result = new SingleSignOnAccountResponse()
                .WithResponseTagName("DeleteSingleSignOnAccountResponse")
                .WithResponse(response)
                .Map();

            if (result.IsSuccessful) {
                return customer as T;
            }

            throw new GatewayException(message: "An error occurred while deleting the customer", responseCode: result.ResponseMessage, responseMessage: result.ResponseMessage);
        }

        private T CreateCustomerAccount(RecurringPaymentMethod paymentMethod) {
            var et = new ElementTree();
            var envelope = CreateSOAPEnvelope(et, "SaveCustomerAccount");
            var request = new CreateCustomerAccountRequest(et)
                .Build(envelope, Credentials, paymentMethod);

            var response = DoTransaction(request, publicEndpoint);

            var result = new CreateCustomerAccountResponse()
                .WithResponseTagName("SaveCustomerAccountResponse")
                .WithResponse(response)
                .Map();

            if (result.IsSuccessful) {
                paymentMethod.Key = paymentMethod.Id;
                return paymentMethod as T;
            }

            throw new GatewayException(message: "An error occurred while creating the customer account", responseCode: result.ResponseMessage, responseMessage: result.ResponseMessage);
        }

        private T UpdateCustomerAccount(RecurringPaymentMethod paymentMethod) {
            var et = new ElementTree();
            var envelope = CreateSOAPEnvelope(et, "UpdateCustomerAccount");
            var request = new UpdateCustomerAccountRequest(et)
                .Build(envelope, Credentials, paymentMethod);

            var response = DoTransaction(request, publicEndpoint);

            var result = new CustomerAccountResponse()
                .WithResponseTagName("UpdateCustomerAccountResponse")
                .WithResponse(response)
                .Map();

            if (result.IsSuccessful) {
                return paymentMethod as T;
            }

            throw new GatewayException(message: "An error occurred while updating the customer account", responseCode: result.ResponseMessage, responseMessage: result.ResponseMessage);
        }

        private T DeleteCustomerAccount(RecurringPaymentMethod paymentMethod) {
            var et = new ElementTree();
            var envelope = CreateSOAPEnvelope(et, "DeleteCustomerAccount");
            var request = new DeleteCustomerAccountRequest(et)
                .Build(envelope, Credentials, paymentMethod);

            var response = DoTransaction(request, publicEndpoint);

            var result = new SingleSignOnAccountResponse()
                .WithResponseTagName("DeleteCustomerAccountResponse")
                .WithResponse(response)
                .Map();

            if (result.IsSuccessful) {
                return paymentMethod as T;
            }

            throw new GatewayException(message: "An error occurred while deleting the customer account", responseCode: result.ResponseMessage, responseMessage: result.ResponseMessage);
        }
    }
}
