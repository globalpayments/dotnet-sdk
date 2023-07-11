using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Enums
{
    public enum RiskAssessmentStatus {
        ACCEPTED,
        REJECTED,
        CHALLENGE,
        PENDING_REVIEW
    }
}
