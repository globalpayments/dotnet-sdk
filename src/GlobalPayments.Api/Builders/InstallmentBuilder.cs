using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways.Interfaces;
using GlobalPayments.Api.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Builders {
    /// <summary>
    /// Used to create Installments
    /// </summary>
    public class InstallmentBuilder : BaseBuilder<Installment> {
        
        /// <summary>
        /// Represents the Installment entity
        /// </summary>
        internal IInstallmentEntity Entity;

        /// <summary>
        /// Represents the parameterized constructor to set the installment Entity value
        /// </summary>
        /// <param name="entity"></param>
        public InstallmentBuilder(IInstallmentEntity entity = null) {
            if (entity != null)
            {
                Entity = entity;
            }
        }

        /// <summary>
        /// Executes the Installment builder against the gateway.
        /// </summary>
        /// <returns>TResult</returns>
        public override Installment Execute(string configName = "default") {
            base.Execute(configName);

            var client = ServicesContainer.Instance.GetInstallmentClient(configName);
            return client.ProcessInstallment(this);
        }
    }
}
