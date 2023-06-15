using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Enums
{
    public enum StatusChangeReason {
        ACTIVE,
        CLOSED_BY_MERCHANT,
        CLOSED_BY_RISK,
        APPLICATION_DENIED,
        PENDING_REVIEW,
        PENDING_MERCHANT_CONSENT,
        PENDING_IDENTITY_VALIDATION,
        PENDING_IDENTITY_VALIDATION_AND_PAYMENT,
        PENDING_PAYMENT,
        UNKNOWN_STATUS,
        REMOVE_PARTNERSHIP
    }
}
