using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Enums
{
    public enum FraudFilterResult {
        [Map(Target.GP_API, "PENDING_REVIEW")]
        HOLD,

        [Map(Target.GP_API, "ACCEPTED")]
        PASS,

        [Map(Target.GP_API, "REJECTED")]
        BLOCK,

        [Map(Target.GP_API, "NOT_EXECUTED")]
        NOT_EXECUTED,

        [Map(Target.GP_API, "ERROR")]
        ERROR,

        [Map(Target.GP_API, "RELEASE_SUCCESSFUL")]
        RELEASE_SUCCESSFUL,

        [Map(Target.GP_API, "HOLD_SUCCESSFUL")]
        HOLD_SUCCESSFUL
    }
}
