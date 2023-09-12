using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GlobalPayments.Api.Entities.Enums
{
    public enum PaymentMethodFunction {
        [Description("PRIMARY_PAYOUT")]
        PRIMARY_PAYOUT,

        [Description("SECONDARY_PAYOUT")]
        SECONDARY_PAYOUT,

        [Description("ACCOUNT_ACTIVATION_FEE")]
        ACCOUNT_ACTIVATION_FEE
    }
}
