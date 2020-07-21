using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE44_ActionReasonCode {
        [Map(Target.NWS, "0000")]
        NoActionReason,
        [Map(Target.NWS, "0001")]
        DataElementMissing,
        [Map(Target.NWS, "0002")]
        DataElementAttributeInvalid,
        [Map(Target.NWS, "0003")]
        ValueIsInvalid,
        [Map(Target.NWS, "0004")]
        ValueNotSupportedForCardType,
        [Map(Target.NWS, "0005")]
        ValueGreaterThanMaxValue,
        [Map(Target.NWS, "0006")]
        ValueLessThanMinValue,
        [Map(Target.NWS, "0007")]
        BatchNumberMissing,
        [Map(Target.NWS, "0008")]
        InvalidMicrData,
        [Map(Target.NWS, "0009")]
        InvalidDriversLicense,
        [Map(Target.NWS, "0010")]
        InvalidSocialSecurityNumber,
        [Map(Target.NWS, "0011")]
        InvalidBirthDate,
        [Map(Target.NWS, "0012")]
        InvalidStateCode,
        [Map(Target.NWS, "0013")]
        InvalidDenomination,
        [Map(Target.NWS, "0020")]
        DeactivatedCard,
        [Map(Target.NWS, "0021")]
        ProductRestriction,
        [Map(Target.NWS, "0022")]
        AmountNotWithinIssuerLimits,
        [Map(Target.NWS, "0023")]
        CardStripeDataIsBad,
        [Map(Target.NWS, "0024")]
        CannotActivateCard,
        [Map(Target.NWS, "0025")]
        CardAlreadyActive,
        [Map(Target.NWS, "0026")]
        CardNotActive,
        [Map(Target.NWS, "0027")]
        CardAlreadyRedeemed,
        //[Map(Target.NWS, "0028")]
        //Value is greater than the maximum value,
        //[Map(Target.NWS, "0029")]
        //Value is less than the minimum value,
        [Map(Target.NWS, "0033")]
        BusinessOrMerchantTypeRestriction,
        [Map(Target.NWS, "0034")]
        CardNotAcceptedByMerchant,
        [Map(Target.NWS, "0035")]
        AmountPresentedAmountRequiredMismatch,
        [Map(Target.NWS, "0036")]
        OverDailyFuelLimit,
        [Map(Target.NWS, "0037")]
        OverMonthlyFuelLimit,
        [Map(Target.NWS, "0038")]
        OverDailyMerchandiseAmountLimit,
        [Map(Target.NWS, "0039")]
        OverMonthlyMerchandiseAmountLimit,
        [Map(Target.NWS, "0040")]
        OverDailyServiceLimit,
        [Map(Target.NWS, "0041")]
        OverMonthlyServiceLimit,
        [Map(Target.NWS, "0042")]
        InvalidTransTimeCaptureMethod,
        [Map(Target.NWS, "0043")]
        InvalidVolume,
        [Map(Target.NWS, "0044")]
        TransValueOverLimit,
        [Map(Target.NWS, "0045")]
        InvalidProductCode,
        [Map(Target.NWS, "0046")]
        AvsMismatch,
        [Map(Target.NWS, "0047")]
        CvnMismatch,
        [Map(Target.NWS, "0048")]
        VelocityViolation,
        [Map(Target.NWS, "0049")]
        ProductProgramNotFound,
        [Map(Target.NWS, "0050")]
        ProductProgramOutOfStock,
        [Map(Target.NWS, "0511")]
        VehicleNotOnFile,
        [Map(Target.NWS, "0512")]
        DriverNotOnFile,
        [Map(Target.NWS, "0513")]
        SocialSecurityNumberNotOnFile,
        [Map(Target.NWS, "0514")]
        AddressNotOnFile,
        [Map(Target.NWS, "0600")]
        DuplicateBadStatus,
        [Map(Target.NWS, "0601")]
        DuplicateNeedReact,
        [Map(Target.NWS, "0602")]
        DuplicateNeedCCA,
        [Map(Target.NWS, "0603")]
        SuspectedFraud,
        [Map(Target.NWS, "0604")]
        PotentialDuplicateAccount,
        [Map(Target.NWS, "0605")]
        DoNotSolicit,
        [Map(Target.NWS, "0606")]
        NotifyByMail,
        [Map(Target.NWS, "0607")]
        NoHit,
        [Map(Target.NWS, "0608")]
        Deactivated,
        [Map(Target.NWS, "0609")]
        ExpirationDateNotSupported,
        [Map(Target.NWS, "0610")]
        DuplicatePurchaseOverLimit,
        [Map(Target.NWS, "0611")]
        PotentialBadContractualAge,
        [Map(Target.NWS, "0700")]
        TransmissionError,
        [Map(Target.NWS, "0701")]
        ThirdPartyProcessorTimeout,
        [Map(Target.NWS, "0702")]
        TransactionDateTimeInvalid,
        [Map(Target.NWS, "0703")]
        TerminalUnknown,
        [Map(Target.NWS, "0704")]
        TerminalInactive,
        [Map(Target.NWS, "0705")]
        TerminalShutdown,
        [Map(Target.NWS, "0706")]
        InternalRouterTimeout,
        [Map(Target.NWS, "0707")]
        EnqFailedDelayFailure,
        [Map(Target.NWS, "0708")]
        BitmapConstructionError_Host,
        [Map(Target.NWS, "0709")]
        NWSSystem_ABEND,
        [Map(Target.NWS, "0710")]
        InternalRouterEditError,
        [Map(Target.NWS, "0711")]
        IoError_ResourceUnavailable,
        [Map(Target.NWS, "0712")]
        TransactionNotRecognized,
        [Map(Target.NWS, "0713")]
        UnmatchedPreAuthorizationCompletion,
        [Map(Target.NWS, "0714")]
        DuplicateHeartlandGiftCardTransaction,
        [Map(Target.NWS, "0715")]
        TransactionNotAllowed,
        [Map(Target.NWS, "0716")]
        UnexpectedTrackDataReceived,
        [Map(Target.NWS, "0717")]
        ServiceProviderError,
        [Map(Target.NWS, "0718")]
        ConfigurationError,
        [Map(Target.NWS, "0719")]
        TransactionDisabled,
        [Map(Target.NWS, "0720")]
        ProductProgramDisabled,
        [Map(Target.NWS, "0721")]
        TransactionCannotBeVoided,
        [Map(Target.NWS, "0722")]
        VoidTimeLimitExpired,
        [Map(Target.NWS, "0723")]
        AdditionalE3ErrorCode,
        [Map(Target.NWS, "0941")]
        HostDetectedDuplicate_ReturningOriginalApproval,
        [Map(Target.NWS, "1000")]
        Approval,
        [Map(Target.NWS, "1046")]
        DuplicateAcct_CCA
    }
}
