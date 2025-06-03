using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Services {
    /// <summary>
    /// Represents the Installment Service to create the installment
    /// </summary>
    public class InstallmentService {
        /// <summary>
        /// Creates the Installment
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="configName"></param>
        /// <returns>Installment</returns>
        public static Installment Create(Installment entity, string configName = "default") {

            var response = new InstallmentBuilder(entity).Execute(configName);
            return response;
        }
    }
}
