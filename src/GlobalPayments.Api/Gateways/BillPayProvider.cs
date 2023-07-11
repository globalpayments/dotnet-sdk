using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.Gateways.BillPay;
using GlobalPayments.Api.Logging;
using System.Net;

namespace GlobalPayments.Api.Gateways {
    internal class BillPayProvider : IBillingProvider, IPaymentGateway, IRecurringService, IReportingService {
        /// <summary>
        /// Gets or sets the merchant credentials to be used in each SOAP request
        /// </summary>
        public Credentials Credentials { get; set; }
        public IRequestLogger RequestLogger { get; set; }
        public IWebProxy WebProxy { get; set; }

        /// <summary>
        /// Gets or sets whether or not the bill data is loaded into our system.
        /// </summary>
        public bool IsBillDataHosted { get; set; }
        public int Timeout { get; set; }
        public string ServiceUrl { get; set; }

        public bool SupportsOpenBanking => false;
        public bool SupportsHostedPayments => true;
        public bool SupportsRetrieval => false;
        public bool SupportsUpdatePaymentDetails => false;

        /// <summary>
        /// Invokes a request against the BillPay gateway using the AuthorizationBuilder
        /// </summary>
        /// <param name="builder">The <see cref="AuthorizationBuilder">AuthroizationBuilder</see> containing the required information to build the request</param>
        /// <returns>A Transaction response</returns>
        public Transaction ProcessAuthorization(AuthorizationBuilder builder) {
            return new AuthorizationRequest(Credentials, ServiceUrl, Timeout)
                .WithRequestLogger(RequestLogger)
                .WithWebProxy(WebProxy)
                .Execute(builder, IsBillDataHosted);
        }

        /// <summary>
        /// Invokes a request against the BillPay gateway using the ManagementBuilder
        /// </summary>
        /// <param name="builder">The <see cref="ManagementBuilder">ManagementBuilder</see> containing the required information to build the request</param>
        /// <returns>A Transaction response</returns>
        public Transaction ManageTransaction(ManagementBuilder builder) {
            return new ManagementRequest(Credentials, ServiceUrl, Timeout)
                .WithRequestLogger(RequestLogger)
                .WithWebProxy(WebProxy)
                .Execute(builder, IsBillDataHosted);
        }

        public BillingResponse ProcessBillingRequest(BillingBuilder builder) {
            return new BillingRequest(Credentials, ServiceUrl, Timeout)
                .WithRequestLogger(RequestLogger)
                .WithWebProxy(WebProxy)
                .Execute(builder);
        }

        public T ProcessRecurring<T>(RecurringBuilder<T> builder) where T : class {
            return new RecurringRequest<T>(Credentials, ServiceUrl, Timeout)
                .WithRequestLogger(RequestLogger)
                .WithWebProxy(WebProxy)
                .Execute(builder);
        }

        public T ProcessReport<T>(ReportBuilder<T> builder) where T : class {
            return new ReportRequest<T>(Credentials, ServiceUrl, Timeout)
                .WithRequestLogger(RequestLogger)
                .WithWebProxy(WebProxy)
                .Execute(builder);
        }

        public string SerializeRequest(AuthorizationBuilder builder) {
            throw new UnsupportedTransactionException();
        }
    }
}
