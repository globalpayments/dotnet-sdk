using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Enums
{
    [MapTarget(Target.GP_API)]
    public enum BNPLType {
        [Map(Target.GP_API, "AFFIRM")]
        AFFIRM,

        [Map(Target.GP_API, "CLEARPAY")]
        CLEARPAY,

        [Map(Target.GP_API, "KLARNA")]
        KLARNA
    }
}
