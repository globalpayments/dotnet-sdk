using System;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Services {
    public class CheckService {
        public CheckService(GatewayConfig config, string configName= "default") {
            ServicesContainer.ConfigureService(config, configName);
        }

        // Recurring


        // Charge
        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale).WithAmount(amount);
        }

        // Void using gateway transaction ID
        public ManagementBuilder Void(string transactionId) {
            return Void(transactionId);
        }

        // Void using client transaction ID
        public ManagementBuilder Void(string transactionId, bool isClientTxnId = false) {
            if (isClientTxnId) {
                return new ManagementBuilder(TransactionType.Void)
                    .WithPaymentMethod(new TransactionReference {
                        PaymentMethodType = PaymentMethodType.ACH,
                        ClientTransactionId = transactionId
                    });
            } else {
                return new ManagementBuilder(TransactionType.Void)
                    .WithPaymentMethod(new TransactionReference {
                        PaymentMethodType = PaymentMethodType.ACH,
                        TransactionId = transactionId
                    });
            }            
        }
    }
}
