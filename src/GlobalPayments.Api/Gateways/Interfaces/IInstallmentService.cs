using GlobalPayments.Api.Builders;
using GlobalPayments.Api.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Gateways.Interfaces {
    public interface IInstallmentService {
        Installment ProcessInstallment(InstallmentBuilder builder);
    }
}
