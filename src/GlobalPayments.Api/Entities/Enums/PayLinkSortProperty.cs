using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Enums
{
    [MapTarget(Target.GP_API)]
    public enum PayLinkSortProperty {
        [Map(Target.GP_API, "TIME_CREATED")]
        TimeCreated
    }
}
