using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE62_CardIssuerEntryTag {
        [Map(Target.NWS, " ")]
        None,
        [Map(Target.NWS, "1xx")]
        StoredValueCards,
        [Map(Target.NWS, "2xx")]
        LoyaltyCards,
        [Map(Target.NWS, "3xx")]
        PrivateLabelCards,
        [Map(Target.NWS, "3SF")]
        SearsProprietaryDeferDate,
        [Map(Target.NWS, "3SL")]
        SearsProprietaryDelayDate,
        [Map(Target.NWS, "Bxx")]
        Bank_CreditCards,
        [Map(Target.NWS, "Cxx")]
        Checks,
        [Map(Target.NWS, "C00")]
        CheckInformation,
        [Map(Target.NWS, "C02")]
        CheckExpandedOrRawMICRData,
        [Map(Target.NWS, "Dxx")]
        PIN_DebitCards,
        [Map(Target.NWS, "Exx")]
        ElectronicBenefitsTransfer,
        [Map(Target.NWS, "EIS")]
        EwicIssuingEntity,
        [Map(Target.NWS, "EWN")]
        EwicMerchantId,
        [Map(Target.NWS, "Fxx")]
        FleetCards,
        [Map(Target.NWS, "F00")]
        Wex_SpecVersionSupport,
        [Map(Target.NWS, "F01")]
        Wex_PurchaseDeviceSequenceNumber,
        [Map(Target.NWS, "Gxx")]
        PrepaidServiceSystem,
            //Removed the tags as part of v19.1 compliance updates
        //[Map(Target.NWS, "IAD")]
        //CardIssuerAuthenticationData,

        [Map(Target.NWS, "IAM")]
        AmountSentToIssuerOnBehalfOfPos,
        [Map(Target.NWS, "IAN")]
        AccountFromCardIssuer,
        [Map(Target.NWS, "IAR")]
        CardIssuerAuthenticationResponseCode,
        [Map(Target.NWS, "IAT")]
        AccountTypeFromCardIssuer,
        [Map(Target.NWS, "IAV")]
        AvsResponseCode,
            //Removed the tags as part of v19.1 compliance updates
        //[Map(Target.NWS, "IAX")]
        //CardIssuerAuthenticationIdentifier,
        [Map(Target.NWS, "ICC")]
        ChipConditionCode,
        [Map(Target.NWS, "ICP")]
        CreditPlan,
        [Map(Target.NWS, "ICV")]
        CvnResponseCode,
        [Map(Target.NWS, "IDG")]
        DiagnosticMessage,
        [Map(Target.NWS, "IED")]
        ExtendedExpirationDate,
        [Map(Target.NWS, "IGS")]
        GiftCardPurchase,
        [Map(Target.NWS, "IID")]
        UniqueDeviceId,
        [Map(Target.NWS, "IPA")]
        PiggyBackActionCode,
        [Map(Target.NWS, "IPR")]
        ReceiptText,
        [Map(Target.NWS, "IRA")]
        OriginalResponse_ActionCode,
        [Map(Target.NWS, "IRC")]
        CenterCallNumber,
        [Map(Target.NWS, "IRR")]
        RetrievalReferenceNumber,
        [Map(Target.NWS, "ITM")]
        IssuerSpecificTransactionMatchData,
        [Map(Target.NWS, "ITX")]
        DisplayText,
        [Map(Target.NWS, "I41")]
        Alternate_DE41,
        [Map(Target.NWS, "I42")]
        Alternate_DE42,
        [Map(Target.NWS, "NDE")]
        DialError,
        [Map(Target.NWS, "ND2")]
        DiscoverNetworkReferenceId,
        [Map(Target.NWS, "NM1")]
        NTS_MastercardBanknet_ReferenceNumber,
        [Map(Target.NWS, "NM2")]
        NTS_MastercardBanknet_SettlementDate,
        [Map(Target.NWS, "NPC")]
        NTS_POS_Capability,
        [Map(Target.NWS, "NPS")]
        PetroleumSwitch,
        [Map(Target.NWS, "NSI")]
        SwipeIndicator,
        [Map(Target.NWS, "NTE")]
        TerminalError,
        [Map(Target.NWS, "NTS")]
        NTS_System,
        [Map(Target.NWS, "NV1")]
        VisaTransactionId,
        [Map(Target.NWS, "IAU")]
        MastercardUCAFData,
        [Map(Target.NWS, "IME")]
        MastercardECommerceIndicators,
        [Map(Target.NWS, "IMW")]
        MastercardWalletID
    }

    public class DE62_CardIssuerEntryTagClass {
        public static DE62_CardIssuerEntryTag FindPartial(string value) {
            switch (value.ToCharArray()[0]) {
                case '1':
                    return DE62_CardIssuerEntryTag.StoredValueCards;
                case '2':
                    return DE62_CardIssuerEntryTag.LoyaltyCards;
                case '3':
                    return DE62_CardIssuerEntryTag.PrivateLabelCards;
                case 'B':
                    return DE62_CardIssuerEntryTag.Bank_CreditCards;
                case 'C':
                    return DE62_CardIssuerEntryTag.Checks;
                case 'D':
                    return DE62_CardIssuerEntryTag.PIN_DebitCards;
                case 'E':
                    return DE62_CardIssuerEntryTag.ElectronicBenefitsTransfer;
                case 'F':
                    return DE62_CardIssuerEntryTag.FleetCards;
                case 'G':
                    return DE62_CardIssuerEntryTag.PrepaidServiceSystem;
                case 'N':
                    return DE62_CardIssuerEntryTag.NTS_System;
                default:
                    return DE62_CardIssuerEntryTag.None;
            }
        }
    }



}
