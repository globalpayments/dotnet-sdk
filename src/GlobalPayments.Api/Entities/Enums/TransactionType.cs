using System;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Indicates the transaction type.
    /// </summary>
    [Flags]
    public enum TransactionType : long {
        /// <summary>
        /// Indicates a decline.
        /// </summary>
        Decline = 0,

        /// <summary>
        /// Indicates an account verify.
        /// </summary>
        Verify = 1 << 0,

        /// <summary>
        /// Indicates a capture/add to batch.
        /// </summary>
        Capture = 1 << 1,

        /// <summary>
        /// Indicates an authorization without capture.
        /// </summary>
        Auth = 1 << 2,

        /// <summary>
        /// Indicates a refund/return.
        /// </summary>
        Refund = 1 << 3,

        /// <summary>
        /// Indicates a reversal.
        /// </summary>
        Reversal = 1 << 4,

        /// <summary>
        /// Indicates a sale/charge/authorization with capture.
        /// </summary>
        Sale = 1 << 5,

        /// <summary>
        /// Indicates an edit.
        /// </summary>
        Edit = 1 << 6,

        /// <summary>
        /// Indicates a void.
        /// </summary>
        Void = 1 << 7,

        /// <summary>
        /// Indicates value should be added.
        /// </summary>
        AddValue = 1 << 8,

        /// <summary>
        /// Indicates a balance inquiry.
        /// </summary>
        Balance = 1 << 9,

        /// <summary>
        /// Indicates an activation.
        /// </summary>
        Activate = 1 << 10,

        /// <summary>
        /// Indicates an alias should be added.
        /// </summary>
        Alias = 1 << 11,

        /// <summary>
        /// Indicates the payment method should be replaced.
        /// </summary>
        Replace = 1 << 12,

        /// <summary>
        /// Indicates a reward.
        /// </summary>
        Reward = 1 << 13,

        /// <summary>
        /// Indicates a deactivation.
        /// </summary>
        Deactivate = 1 << 14,

        /// <summary>
        /// Indicates a batch close.
        /// </summary>
        BatchClose = 1 << 15,

        /// <summary>
        /// Indicates a resource should be created.
        /// </summary>
        Create = 1 << 16,

        /// <summary>
        /// Indicates a resource should be deleted.
        /// </summary>
        Delete = 1 << 17,

        /// <summary>
        /// Indicates a benefit withdrawal.
        /// </summary>
        BenefitWithdrawal = 1 << 18,

        /// <summary>
        /// Indicates a resource should be fetched.
        /// </summary>
        Fetch = 1 << 19,

        /// <summary>
        /// Indicates a resource type should be searched.
        /// </summary>
        Search = 1 << 20,

        /// <summary>
        /// Indicates a hold.
        /// </summary>
        Hold = 1 << 21,

        /// <summary>
        /// Indicates a release.
        /// </summary>
        Release = 1 << 22,

        /// <summary>
        /// Indicates a verify 3d Secure enrollment transaction
        /// </summary>
        VerifyEnrolled = 1 << 23,

        /// <summary>
        /// Indicates a verify 3d secure verify signature transaction
        /// </summary>
        VerifySignature = 1 << 24,

        /// <summary>
        /// Indcates a TokenUpdateExpiry Transaction
        /// </summary>
        TokenUpdate = 1 << 25,

        /// <summary>
        /// Indicates a Token Delete Transaction
        /// </summary>
        TokenDelete = 1 << 26,

        /// <summary>
        /// Indicates a verify authentication 3DS2 call
        /// </summary>
        VerifyAuthentication = 1 << 27,

        /// <summary>
        /// Indicates an Initiate Authentication 3DS2 call
        /// </summary>
        InitiateAuthentication = 1 << 28,

        /// <summary>
        /// Indicates a DataCollect.
        /// </summary>
        DataCollect = 1 << 29,

        /// <summary>
        /// Indicates a PreAuthCompletion.
        /// </summary>
        PreAuthCompletion = 1 << 30,

        /// <summary>
        /// Indicates a DccRateLookup.
        /// </summary>
        DccRateLookup = 1 << 31,

        /// <summary>
        /// 
        /// </summary>
        Increment = 1L << 32,

        /// <summary>
        /// Indicates a token only transaction
        /// </summary>
        Tokenize = 1L << 33
    }
}
