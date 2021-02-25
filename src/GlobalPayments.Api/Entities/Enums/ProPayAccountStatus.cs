using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Enums
{
    public enum ProPayAccountStatus {
        ReadyToProcess,
        FraudAccount,
        RiskwiseDeclined,
        Hold,
        Canceled,
        FraudVictim,
        ClosedEula,
        ClosedExcessiveChargeback
    }
}
