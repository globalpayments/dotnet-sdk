using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Services
{
    public class FraudService
    {
        public static FraudBuilder<RiskAssessment> RiskAssess(IPaymentMethod paymentMethod)
        {
            return (new FraudBuilder<RiskAssessment>(TransactionType.RiskAssess))
                .WithPaymentMethod<FraudBuilder<RiskAssessment>>(paymentMethod);
                
        }
    }
}
