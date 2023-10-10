using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Enums
{
    public enum DocumentCategory
    {
        [Map(Target.GP_API, "IDENTITY_VERIFICATION")]
        IDENTITY_VERIFICATION,
        [Map(Target.GP_API, "RISK_REVIEW")]
        RISK_REVIEW,
        [Map(Target.GP_API, "UNDERWRITING")]
        UNDERWRITING,

        VERIFICATION
    }
}
