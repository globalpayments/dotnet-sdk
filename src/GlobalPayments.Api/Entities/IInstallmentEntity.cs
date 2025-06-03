using GlobalPayments.Api.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities {
    public interface IInstallmentEntity {
        Installment Create(string configName);
    }
}
