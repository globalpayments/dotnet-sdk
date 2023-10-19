using System;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Indicates the transaction type.
    /// </summary>
    
    public enum TransactionType : byte {
        /// <summary>
        /// Indicates a decline.
        /// </summary>
        Decline = 0,

        /// <summary>
        /// Indicates an account verify.
        /// </summary>
        Verify = 1 ,

        /// <summary>
        /// Indicates a capture/add to batch.
        /// </summary>
        Capture = 2,

        /// <summary>
        /// Indicates an authorization without capture.
        /// </summary>
        Auth = 3,

        /// <summary>
        /// Indicates a refund/return.
        /// </summary>
        Refund = 4,

        /// <summary>
        /// Indicates a reversal.
        /// </summary>
        Reversal = 5,

        /// <summary>
        /// Indicates a sale/charge/authorization with capture.
        /// </summary>
        Sale = 6,

        /// <summary>
        /// Indicates an edit.
        /// </summary>
        Edit = 7,

        /// <summary>
        /// Indicates a void.
        /// </summary>
        Void = 8,

        /// <summary>
        /// Indicates value should be added.
        /// </summary>
        AddValue = 9,

        /// <summary>
        /// Indicates a balance inquiry.
        /// </summary>
        Balance = 10,

        /// <summary>
        /// Indicates an activation.
        /// </summary>
        Activate = 11,

        /// <summary>
        /// Indicates an alias should be added.
        /// </summary>
        Alias = 12,

        /// <summary>
        /// Indicates the payment method should be replaced.
        /// </summary>
        Replace = 13,

        /// <summary>
        /// Indicates a reward.
        /// </summary>
        Reward = 14,

        /// <summary>
        /// Indicates a deactivation.
        /// </summary>
        Deactivate = 15,

        /// <summary>
        /// Indicates a batch close.
        /// </summary>
        BatchClose = 16,

        /// <summary>
        /// Indicates a resource should be created.
        /// </summary>
        Create = 17,

        /// <summary>
        /// Indicates a resource should be deleted.
        /// </summary>
        Delete = 18,

        /// <summary>
        /// Indicates a benefit withdrawal.
        /// </summary>
        BenefitWithdrawal = 19,

        /// <summary>
        /// Indicates a resource should be fetched.
        /// </summary>
        Fetch = 20,

        /// <summary>
        /// Indicates a resource type should be searched.
        /// </summary>
        Search = 21,

        /// <summary>
        /// Indicates a hold.
        /// </summary>
        Hold = 22,

        /// <summary>
        /// Indicates a release.
        /// </summary>
        Release = 23,

        /// <summary>
        /// Indicates a verify 3d Secure enrollment transaction
        /// </summary>
        VerifyEnrolled = 24,

        /// <summary>
        /// Indicates a verify 3d secure verify signature transaction
        /// </summary>
        VerifySignature = 25,

        /// <summary>
        /// Indcates a TokenUpdateExpiry Transaction
        /// </summary>
        TokenUpdate = 26,

        /// <summary>
        /// Indicates a Token Delete Transaction
        /// </summary>
        TokenDelete = 27,

        /// <summary>
        /// Indicates a confirm call
        /// </summary>
        Confirm = 28,

        /// <summary>
        /// Indicates an Initiate Authentication 3DS2 call
        /// </summary>
        InitiateAuthentication = 29,

        /// <summary>
        /// Indicates a DataCollect.
        /// </summary>
        DataCollect = 30,

        /// <summary>
        /// Indicates a PreAuthCompletion.
        /// </summary>
        PreAuthCompletion = 31,

        /// <summary>
        /// Indicates a DccRateLookup.
        /// </summary>
        DccRateLookup = 32,

        /// <summary>
        /// 
        /// </summary>
        Increment = 33,

        /// <summary>
        /// Indicates a token only transaction
        /// </summary>
        /// 
        Tokenize = 34,

        /// <summary>
        ///
        /// </summary>
        CashOut = 35,

        /// <summary>
        ///
        /// </summary>
        Payment = 36,

        /// <summary>
        ///
        /// </summary>
        CashAdvance = 37,

        /// <summary>
        /// Indicates a dispute acceptance
        /// </summary>
        DisputeAcceptance = 38,

        /// <summary>
        /// Indicates a dispute challenge
        /// </summary>
        DisputeChallenge = 39,

        /// <summary>
        /// 
        /// ProPay: Create Account
        /// </summary>
        CreateAccount = 40,

        /// <summary>
        /// ProPay: Edit Account
        /// </summary>
        EditAccount = 41,

        /// <summary>
        /// ProPay: Reset Account Password
        /// </summary>
        ResetPassword = 42,

        /// <summary>
        /// ProPay: Renew Account
        /// </summary>
        RenewAccount = 43,

        /// <summary>
        /// ProPay: Update Beneficial Ownership Information
        /// </summary>
        UpdateBeneficialOwnership = 44,

        /// <summary>
        /// ProPay: Disown an account
        /// </summary>
        DisownAccount = 45,

        /// <summary>
        /// ProPay: Upload a document to a ProPay account related to a chargeback
        /// </summary>
        UploadDocumentChargeback = 46,

        /// <summary>
        /// ProPay: Upload a document to a ProPay account
        /// </summary>
        UploadDocument = 47,

        /// <summary>
        /// ProPay: Obtain a single-sign-on key
        /// </summary>
        ObtainSSOKey = 48,

        /// <summary>
        /// ProPay: Update bank account ownership information
        /// </summary>
        UpdateBankAccountOwnership = 49,

        /// <summary>
        /// ProPay: Add funds to a ProPay account (EFT)
        /// </summary>
        AddFunds = 50,

        /// <summary>
        /// ProPay: Sweep funds from a ProPay account (EFT)
        /// </summary>
        SweepFunds = 51,

        /// <summary>
        /// ProPay: Add a card for Flash Funds
        /// </summary>
        AddCardFlashFunds = 52,

        /// <summary>
        /// ProPay: Move money out via Flash Funds
        /// </summary>
        PushMoneyFlashFunds = 53,

        /// <summary>
        /// ProPay: Disburse funds to a ProPay account
        /// </summary>
        DisburseFunds = 54,

        /// <summary>
        /// ProPay: SpendBack Transaction
        /// </summary>
        SpendBack = 55,

        /// <summary>
        /// ProPay: Roll back a SplitPay transaction
        /// </summary>
        ReverseSplitPay = 56,

        /// <summary>
        /// ProPay: Split funds from an existing transaction
        /// </summary>
        SplitFunds = 57,

        /// <summary>
        /// ProPay: Get Account details
        /// </summary>
        GetAccountDetails = 58,

        /// <summary>
        /// ProPay: Get Account balance
        /// </summary>
        GetAccountBalance = 59,

        /// <summary>
        /// Indicates a transaction reauthorization
        /// </summary>
        Reauth = 60,

        /// <summary>
        /// 
        /// </summary>
        SiteConfig = 61,

        /// <summary>
        /// 
        /// </summary>
        TimeRequest = 62,

        /// <summary>
        /// Get Token Information for the given token
        /// </summary>
        GetTokenInfo = 63,

        PayByLinkUpdate = 64,

        RiskAssess = 65,

        OrderDevice = 66,

        TransferFunds = 67,        
       
    }
}
