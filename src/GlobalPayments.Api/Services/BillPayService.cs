using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPayments.Api.Services {
    public class BillPayService {
        /// <summary>
        /// Returns the fee for the given payment method and amount
        /// </summary>
        /// <param name="paymentMethod">The payment method that will be used to make the charge against</param>
        /// <param name="amount">The total amount to be charged</param>
        /// <param name="configName">The name of the registered configuration to retrieve. This defaults to 'default'</param>
        /// <returns></returns>
        public decimal CalculateConvenienceAmount(IPaymentMethod paymentMethod, decimal amount, string configName = "default") {
            var response = new BillingBuilder(TransactionType.Fetch)
                .WithPaymentMethod(paymentMethod)
                .WithAmount(amount)
                .Execute(configName);

            return ((ConvenienceFeeResponse)response).ConvenienceFee;
        }

        /// <summary>
        /// Loads one or more bills for a specific customer and returns an identifier that can be used by the customer to retrieve their bills
        /// </summary>
        /// <param name="hostedPaymentData">The payment data to be hosted</param>
        /// <param name="configName">The name of the registered configuration to retrieve. This defaults to 'default'</param>
        /// <returns></returns>
        public LoadSecurePayResponse LoadHostedPayment(HostedPaymentData hostedPaymentData, string configName = "default") {
            var response = new BillingBuilder(TransactionType.Create)
                .WithBillingLoadType(BillingLoadType.SecurePayment)
                .WithHostedPaymentData(hostedPaymentData)
                .Execute(configName);

            return ((LoadSecurePayResponse)response);
        }

        /// <summary>
        /// Loads one or more bills for one or many customers
        /// </summary>
        /// <param name="bills">The collection of bills to load</param>
        /// <param name="configName">The name of the registered configuration to retrieve. This defaults to 'default'</param>
        public void LoadBills(IEnumerable<Bill> bills, string configName = "default") {
            int maxBillsPerUpload = 1000;
            int billCount = bills.Count();
            int numberOfCalls = billCount < maxBillsPerUpload ? 1 : billCount / maxBillsPerUpload;

            for (int i = 0; i < numberOfCalls; i++) {
                new BillingBuilder(TransactionType.Create)
                    .WithBillingLoadType(BillingLoadType.Bills)
                    .WithBills(bills.Skip(i * maxBillsPerUpload).Take(maxBillsPerUpload))
                    .Execute(configName);
            }
        }

        /// <summary>
        /// Removes all bills that have been loaded and have not been committed
        /// </summary>
        /// <param name="configName">The name of the registered configuration to retrieve. This defaults to 'default'</param>
        /// <returns></returns>
        public BillingResponse ClearBills(string configName = "default") {
            return new BillingBuilder(TransactionType.Delete)
                .WithBillingLoadType(BillingLoadType.Bills)
                .ClearPreloadedBills()
                .Execute(configName);
        }

        /// <summary>
        /// Commits all bills that have been preloaded
        /// </summary>
        /// <param name="configName">The name of the registered configuration to retrieve. This defaults to 'default'</param>
        /// <returns></returns>
        public BillingResponse CommitPreloadedBills(string configName = "default") {
            return new BillingBuilder(TransactionType.Activate)
                .WithBillingLoadType(BillingLoadType.Bills)
                .CommitPreloadedBills()
                .Execute(configName);
        }
    }
}
