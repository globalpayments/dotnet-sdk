﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Enums
{
    public enum BankPaymentStatus {
        PAYMENT_INITIATED,
        REQUEST_CONSUMER_CONSENT,
        PROCESSING,
        UNKNOWN,
        SUCCESS,
        INVALID_STATUS,
        FAILURE_INSUFFICIENT_FUNDS,
        FAILURE_INVALID_CURRENCY,
        FAILURE_GENERIC,
        FAILURE_PERMISSION_DENIED,
        FAILURE_CANCELED,
        FAILURE_QUOTE_EXPIRED,
        FAILURE_INVALID_AMOUNT,
        FAILURE_INVALID_QUOTE,
        FAILURE_EXPIRED,
        PENDING_EXTERNAL_AUTHORIZATION,
        FAILURE_DECLINED,
        STATUS_NOT_AVAILABLE,
        PAYMENT_NOT_COMPLETED,
        INITIATION_PROCESSING,
        INITIATION_REJECTED,
        INITIATION_FAILED
    }
}
