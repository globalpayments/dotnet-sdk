using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Enums
{
    [MapTarget(Target.GP_API)]
    public enum MerchantAccountsSortProperty {
        [Map(Target.GP_API, "TIME_CREATED")]
        TIME_CREATED
    }
}
