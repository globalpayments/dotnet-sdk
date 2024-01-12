using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Enums
{
    public enum BlockCardType
    {
        [Description("consumercredit")]
        CONSUMER_CREDIT,
        [Description("consumerdebit")]
        CONSUMER_DEBIT,
        [Description("commercialdebit")]
        COMMERCIAL_DEBIT,
        [Description("commercialcredit")]
        COMMERCIAL_CREDIT
    }
}
