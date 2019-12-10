using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class OpenPathResultModel
    {
        public enum ResponseTypes : int
        {
            Approved = 1,
            Declined = 2,
            Rejected = 3,
            Error = 4,
            Pending = 5,
            Information = 6
        }

        public enum VarietyTypes : int
        {
            Soft = 1,
            Action = 2,
            Hard = 3,
            NotApplicable = 15,
            Unknown = 16,
        }

        public enum SpecificTypes : int
        {
            Unknown = 000000,
            Approved = 111100,
            Success = 111101,
            PartialApproval = 111102,
            Pending = 111150,
            Declined = 111200,
            CustomerIsATeapot = 111201,
            InsufficientFunds = 111202,
            OverLimit = 111203,
            TransactionnotAllowed = 111204,
            RejectedByGateway = 111300,
            BlockedbyOpenPathSiteFirewall = 111302,
            DuplicateTransaction = 111330,
            ErrorReturnedFromProcessor = 111400,
            OpenPathSiteInactive = 111401,
            OpenPathSiteUnauthorized = 111402,
            OpenPathSiteDeactivated = 111403,
            OpenPathSiteDelinquent = 111404,
            OpenPathFilterInactive = 111405,
            InvalidMerchantConfiguration = 111410,
            MerchantAccountInactive = 111411,
            CommunicationError = 111420,
            CommunicationErrorWithProcessor = 111421,
            ApiError = 111422,
            ProcessorFormatError = 111440,
            InvalidTransactionInformation = 111441,
            ProcessorFeaturenotAvailable = 111460,
            UnsupportedCardType = 111461,
            IncorrectPaymentInformation = 112220,
            ExpiredCard = 112223,
            InvalidExpiration = 112224,
            InvalidSecurityCode = 112225,
            UpdatedCardholderInformationAvailable = 112263,
            RetryInAFewDays = 112264,
            CallIssuer = 112540,
            DeclinedWithInstruction = 112560,
            NotHonored = 113201,
            IssuerDoesNotExist = 113221,
            CardNumberDoesNotExistWithIssuer = 113222,
            PickUpCard = 113250,
            LostCard = 113251,
            StolenCard = 113252,
            FraudulentCard = 113253,
            SecViolation = 113254,

            StopAllRecurringPayments = 113261,
            StopThisRecurringProgram = 113262,
            AvsReferral = 113263,
            StopPayment = 113264,

            InvalidCreditCardNumber = 113301,
            HasPreviousHardDecline = 113302,
            InvalidCheckNumber = 113303,

            // cvv results
            Match = 131101,
            NotProcessed = 131103,
            SecurityCodeNotOnCard = 131104,
            IssuerNotCertified = 131105,
            NoMatch = 131210,
            UnknownCVVResponse = 131211,
            CvvShouldHaveBeenPresent = 131212,

            ThreeDSecureIssue = 140000,
            RejectedByFilterRule = 912301,
            RejectedByRouteDefaultPath = 912302,
            AmountCannotBeNegitive = 912200,

            // avs results
            AddressAndFullPostalCodeMatch = 921101,
            AddressAndPartialPostalCodeMatch = 921102,
            AddressMatch = 921103,
            FullPostalCodeMatch = 921104,
            PartialPostalCodeMatch = 921105,
            CountryDoesNotParticipate = 921108,
            IssuerSystemUnavailable = 921109,
            NotACardNotPresentOrder = 921110,
            ServiceNotSupported = 921111,
            AddressVerificationSystemnotAvailable = 921112,
            NoAddressMatch = 921206,
            AddressUnavailable = 921207,
            AddressAndPostalCodeNotVerifiedDueToIncompatibleFormats = 921208,
            RetryAvs = 921209,
            UknownAVSResponse = 921299,

            CloverGatewayResponse = 941600,
            NMIResponse = 941601,
            EBanxResponse = 941602,
            PayOnResponse = 941603,
            StripeResponse = 941604,
            PayEngineResponse = 941605,
            PayEngineRequest = 941606,
            WorldPayRequest = 941607,
            WorldPayResponse = 941608,
            SagePayRequest = 941609,
            SagePayResponse = 941610,
            HeartlandRequest = 941611,
            HeartlandResponse = 941612,
            AuthorizeDotNetRequest = 941613,
            AuthorizeDotNetResponse = 941614,

            InvalidAmount = 941213,
            InvalidCardHolderName = 141214,
            InvalidAuthorizationNo = 941215,
            InvalidTransactionCode = 941216,
            InvalidReferenceNumber = 941217,
            InvalidCustomerReferenceNumber = 941218,
            InvalidRefund = 941219,
            InvalidTerminalId = 941220,

            RestrictedCardNumber = 141220,
            InvalidTransactionTag = 941221,
            DatawithinthetransactionIsIncorrect = 941222,
            InvalidTransarmorToken = 941223,
            InvalidCurrencyRequested = 941224,
            InvalidSequenceNo = 941225,
            MessageTimedoutatHost = 941226,
            BCEFunctionError = 941227,
            InvalidDateFromHost = 941228,
            InvalidGatewayID = 941229,
            InvalidTransactionNumber = 941230,
            ConnectionInactive = 942231,
            UnmatchedTransaction = 941232,
            InvalidReversalResponse = 941233,
            UnabletoSendSocketTransaction = 942234,
            UnableToWriteTransactionToFile = 942235,
            UnabletoVoid = 942236,
            UnabletoConnect = 942237,
            DatabaseUnavailable = 942235,
            CardOrCheckNumberFailed = 943236,
            PaymentTicketGenerated = 611100,

            ServicesNotProvidedOrMerchandiseNotReceived = 182101,
            CancelledRecurringTransactionInstallmentBillingDispute = 182102,
            NotAsDescribedOrDefectiveMerchandise = 182103,
            TransactionNotRecognized = 182104,
            FraudCardNotPresentNoCardholderAuthorization = 182105,
            RequestedOrRequiredItemIllegibleOrMissing = 182106,
            DocumentationReceivedWasInvalidOrIncomplete = 182107,
            CounterfeitTransactionChipLiabilityShiftChipPINLiabilityShift = 182108,
            DeclinedAuthorizationAuthorizationRelatedChargeback = 182109,
            NoAuthorizationAuthorizationRelatedChargeback = 182110,
            UnathorizedExpiredCard = 182111,
            LatePresentment = 182112,
            IncorrectCurrencyOrTransactionCode = 182113,
            NonMatchingAccountNumberAuthorizationRelatedChargeback = 182114,
            IncorrectTransactionAmountOrAccountNumberTransactionAmountDiffers = 182115,
            FraudCardPresentNoCardholderAuthorization = 182116,
            DuplicateProcessing = 182117,
            CreditNotProcessed = 182118,
            PaidByOtherMeansTransactionAmountDiffers = 182119,
            QuestionableMerchantActivity = 182120,
            CardholderDisputeNotElsewhereClassified = 182121,
            Chargeback = 182122,
            FraudulentMultipleTransactionsFraudulentProcessingOfTransactions = 182123,
            CantVerifyPin = 182124,

            // off suite
            GatewayUpdateError = 999901,
            SubscriptionCreated = 999902,
            SubscriptionUpdated = 999903,
            SubscriptionDeleted = 999904

        }

        public enum ResultTypes : int
        {
            Transaction = 1,
            Address = 2,
            SecurityCode = 3,
            Gateway = 4,
            Processor = 5,
            Response = 6,
            Information = 7,
            Chargeback = 8,
        }

        // Result properties
        public ResultTypes? ResultType { get; set; }
        public SpecificTypes? Specific { get; set; }
        public ResponseTypes? Response { get; set; }
        public VarietyTypes? Variety { get; set; }
        public string Value { get; set; }

        // Path properties
        public string DestinationName { get; set; }
        public string Error { get; set; }
        public string Result { get; set; }
        public string SourceName { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}
