using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Enums
{
    public enum Secure3dStatus {
        SUCCESS_AUTHENTICATED,
        SUCCESS_ATTEMPT_MADE,
        NOT_AUTHENTICATED,
        FAILED,
        NOT_ENROLLED,
        AVAILABLE,
        ENROLLED,
        CHALLENGE_REQUIRED
    }
}
