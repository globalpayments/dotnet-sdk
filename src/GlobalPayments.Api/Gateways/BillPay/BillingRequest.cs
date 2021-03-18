using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Logging;
using GlobalPayments.Api.Utils;
using System.Net;

namespace GlobalPayments.Api.Gateways.BillPay {
    /// <summary>
    /// Factory method to create and return the request object based on the transaction type
    /// </summary>
    internal class BillingRequest : GatewayRequestBase {
        public BillingRequest(Credentials credentials, string serviceUrl, int timeout)
        {
            this.Credentials = credentials;
            this.ServiceUrl = serviceUrl;
            this.Timeout = timeout;
        }

        internal BillingRequest WithRequestLogger(IRequestLogger requestLogger) {
            RequestLogger = requestLogger;
            return this;
        }

        internal BillingRequest WithWebProxy(IWebProxy webProxy) {
            WebProxy = webProxy;
            return this;
        }

        internal BillingResponse Execute(BillingBuilder builder) {
            switch (builder.TransactionType) {
                case TransactionType.Activate:
                    return CommitPreloadBills();
                case TransactionType.Create:
                    if (builder.BillingLoadType == BillingLoadType.Bills) {
                        return PreloadBills(builder);
                    } else if (builder.BillingLoadType == BillingLoadType.SecurePayment) {
                        return LoadSecurePay(builder);
                    } else {
                        throw new UnsupportedTransactionException();
                    }
                case TransactionType.Fetch:
                    return GetConvenienceFee(builder);
                case TransactionType.Delete:
                    return ClearLoadedBills();
                default:
                    throw new UnsupportedTransactionException();
            }
        }

        private ConvenienceFeeResponse GetConvenienceFee(BillingBuilder builder) {
            var et = new ElementTree();
            var envelope = CreateSOAPEnvelope(et, "GetConvenienceFee");
            var request = new GetConvenienceFeeRequest(et)
                .Build(envelope, builder, Credentials);

            var response = DoTransaction(request, publicEndpoint);
            var result = new ConvenienceFeeRequestResponse()
                .WithResponseTagName("GetConvenienceFeeResponse")
                .WithResponse(response)
                .Map();

            if (result.IsSuccessful) {
                return result;
            }

            throw new GatewayException(message: "An error occurred attempting to retrieve the payment fee", responseCode: result.ResponseCode, responseMessage: result.ResponseMessage);
        }

        private BillingResponse PreloadBills(BillingBuilder builder) {
            var et = new ElementTree();
            var envelope = CreateSOAPEnvelope(et, "PreloadBills");
            var request = new PreloadBillsRequest(et)
                .Build(envelope, builder, Credentials);

            var response = DoTransaction(request, publicEndpoint);
            var result = new PreloadBillsResponse()
                .WithResponseTagName("PreloadBillsResponse")
                .WithResponse(response)
                .Map();

            if (result.IsSuccessful) {
                return result;
            }

            throw new GatewayException(message: "An error occurred attempting to load the hosted bills", responseCode: result.ResponseCode, responseMessage: result.ResponseMessage);
        }

        private BillingResponse CommitPreloadBills() {
            var et = new ElementTree();
            var envelope = CreateSOAPEnvelope(et, "CommitPreloadedBills");
            var request = new CommitPreloadedBillsRequest(et)
                .Build(envelope, Credentials);

            var response = DoTransaction(request, publicEndpoint);
            var result = new BillingRequestResponse()
                .WithResponseTagName("CommitPreloadedBillsResponse")
                .WithResponse(response)
                .Map();

            if (result.IsSuccessful) {
                return result;
            }

            throw new GatewayException(message: "An error occurred attempting to commit the preloaded bills", responseCode: result.ResponseCode, responseMessage: result.ResponseMessage);
        }

        private BillingResponse ClearLoadedBills() {
            var et = new ElementTree();
            var envelope = CreateSOAPEnvelope(et, "ClearLoadedBills");
            var request = new ClearLoadedBillsRequest(et)
                .Build(envelope, Credentials);

            var response = DoTransaction(request, publicEndpoint);

            return new BillingRequestResponse()
                .WithResponseTagName("ClearLoadedBillsResponse")
                .WithResponse(response)
                .Map();
        }

        private LoadSecurePayResponse LoadSecurePay(BillingBuilder builder) {
            var et = new ElementTree();
            var envelope = CreateSOAPEnvelope(et, "LoadSecurePayDataExtended");
            var request = new LoadSecurePayRequest(et)
                .Build(envelope, builder, Credentials);

            var response = DoTransaction(request, publicEndpoint);
            var result = new SecurePayResponse()
                .WithResponseTagName("LoadSecurePayDataExtendedResponse")
                .WithResponse(response)
                .Map();

            if (result.IsSuccessful) {
                return result;
            }

            throw new GatewayException(message: "An error occurred attempting to load the hosted bill", responseCode: result.ResponseCode, responseMessage: result.ResponseMessage);
        }
    }
}
