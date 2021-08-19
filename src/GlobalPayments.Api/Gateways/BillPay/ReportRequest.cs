using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.Logging;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GlobalPayments.Api.Gateways.BillPay
{
    internal sealed class ReportRequest<T> : GatewayRequestBase where T: class {
        public ReportRequest(Credentials credentials, string serviceUrl, int timeout) {
            this.Credentials = credentials;
            this.ServiceUrl = serviceUrl;
            this.Timeout = timeout;
        }

        internal ReportRequest<T> WithRequestLogger(IRequestLogger requestLogger) {
            RequestLogger = requestLogger;
            return this;
        }

        internal ReportRequest<T> WithWebProxy(IWebProxy webProxy) {
            WebProxy = webProxy;
            return this;
        }

        internal T Execute(ReportBuilder<T> builder) {
            var et = new ElementTree();
            var envelope = CreateSOAPEnvelope(et, "GetTransactionByOrderID");
            var request = new GetTransactionByOrderIDRequest(et)
                .Build(envelope, builder, Credentials);
            var response = DoTransaction(request, publicEndpoint);
            var result = new TransactionByOrderIDRequestResponse()
                            .WithResponseTagName("GetTransactionByOrderIDResponse")
                            .WithResponse(response)
                            .Map();

            return result as T;
        }
    }
}
