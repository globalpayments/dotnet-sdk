using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    public enum StoredCredentialType {
        /// <summary>
        /// A single payment using a stored credential.
        /// </summary>
        OneOff,
        /// <summary>
        /// A payment made as part of an installment plan.
        /// </summary>
        [Map(Target.GP_API, "INSTALLMENT")]
        Installment,
        /// <summary>
        /// A payment made on a recurring schedule.
        /// </summary>
        [Map(Target.GP_API, "RECURRING")]
        Recurring,
        /// <summary>
        /// A merchant-initiated payment made outside a fixed schedule.
        /// </summary>
        [Map(Target.GP_API, "UNSCHEDULED")]
        Unscheduled,
        /// <summary>
        /// A payment made under a subscription agreement.
        /// </summary>
        [Map(Target.GP_API, "SUBSCRIPTION")]
        Subscription,
        /// <summary>
        /// A request to store or update a payment method without processing a payment.
        /// </summary>
        [Map(Target.GP_API, "MAINTAIN_PAYMENT_METHOD")]
        MaintainPaymentMethod,
        /// <summary>
        /// A verification request to maintain a stored payment credential.
        /// </summary>
        [Map(Target.GP_API, "MAINTAIN_PAYMENT_VERIFICATION")]
        MaintainPaymentVerification,
        /// <summary>
        /// Add a stored payment method.
        /// </summary>
        [Map(Target.GP_API, "ADD_PAYMENT_METHOD")]
        AddPaymentMethod,
        /// <summary>
        /// Split or delayed shipment processing.
        /// </summary>
        [Map(Target.GP_API, "SPLIT_OR_DELAYED_SHIPMENT")]
        SplitOrDelayedShipment,
        /// <summary>
        /// Top up a balance.
        /// </summary>
        [Map(Target.GP_API, "TOP_UP")]
        TopUp,
        /// <summary>
        /// Mail order processing.
        /// </summary>
        [Map(Target.GP_API, "MAIL_ORDER")]
        MailOrder,
        /// <summary>
        /// Telephone order processing.
        /// </summary>
        [Map(Target.GP_API, "TELEPHONE_ORDER")]
        TelephoneOrder,
        /// <summary>
        /// Check trusted-beneficiary status.
        /// </summary>
        [Map(Target.GP_API, "WHITELIST_STATUS_CHECK")]
        WhitelistStatusCheck,
        /// <summary>
        /// Other stored-credential payment processing.
        /// </summary>
        [Map(Target.GP_API, "OTHER_PAYMENT")]
        OtherPayment,
        /// <summary>
        /// Billing agreement processing.
        /// </summary>
        [Map(Target.GP_API, "BILLING_AGREEMENT")]
        BillingAgreement
    }
}
