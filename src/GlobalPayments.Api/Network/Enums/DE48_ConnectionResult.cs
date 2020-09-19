using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE48_ConnectionResult {
        [Map(Target.NWS, "00")]
        NormalCompletion,
        [Map(Target.NWS, "01")]
        NoCarrierDetected,
        [Map(Target.NWS, "02")]
        BusySignal,
        [Map(Target.NWS, "03")]
        RingNoAnswer,
        [Map(Target.NWS, "04")]
        No_ENQ_FromHost,
        [Map(Target.NWS, "05")]
        GenericReceiveError,
        [Map(Target.NWS, "06")]
        GenericTransmitError,
        [Map(Target.NWS, "07")]
        NoResponseFromHostAfterTransmit,
        [Map(Target.NWS, "08")]
        DialingAlternate_01,
        [Map(Target.NWS, "09")]
        DialingAlternate_02,
        [Map(Target.NWS, "10")]
        RedialingPrimary_OrAlternate,
        [Map(Target.NWS, "11")]
        TransactionSentDirectToAlternate_NoError,
        [Map(Target.NWS, "12")]
        GenericLeasedLineError,
        [Map(Target.NWS, "13")]
        TooMany_NAKs_Sending,
        [Map(Target.NWS, "14")]
        TooMany_NAKs_Receiving,
        [Map(Target.NWS, "15")]
        Received_EOT_AwaitingResponse,
        [Map(Target.NWS, "16")]
        Parity_Framing_Overrun_ErrorOnReceive,
        [Map(Target.NWS, "17")]
        ResponseBufferOverflow,
        [Map(Target.NWS, "18")]
        LostCarrierAwaitingResponse,
        [Map(Target.NWS, "20")]
        Redialing_InvalidCustomerData,
        [Map(Target.NWS, "21")]
        Redialing_FailedErrorCheckResponse,
        [Map(Target.NWS, "50")]
        NoPollOnLeasedLine_AttemptingDialUp,
        [Map(Target.NWS, "51")]
        TimeoutOnLeadedLine_AttemptingDialUp,
        [Map(Target.NWS, "52")]
        TransmitNotReadyOnLeadedLine_AttemptingDialUp,
        [Map(Target.NWS, "53")]
        TimeoutOnLeasedLineAwaitingResponse_AttemptingDialUp,
        [Map(Target.NWS, "60")]
        ConnectionError_ISDN,
        [Map(Target.NWS, "61")]
        No_ENQ_FromHost_ISDN,
        [Map(Target.NWS, "62")]
        Received_EOT_AwaitingResponse_ISDN,
        [Map(Target.NWS, "70")]
        Hardware_OS_Error,
        [Map(Target.NWS, "71")]
        SPS_LinkCommunicationTest,
        [Map(Target.NWS, "90")]
        LostCarrier_Before_ENQ,
        [Map(Target.NWS, "91")]
        LostCarrier_After_1_ENQ,
        [Map(Target.NWS, "92")]
        LostCarrier_After_2_ENQ,
        [Map(Target.NWS, "93")]
        LostCarrier_After_3_ENQ,
        [Map(Target.NWS, "94")]
        LostCarrier_After_4_ENQ,
        [Map(Target.NWS, "95")]
        LostCarrier_After_5_ENQ,
        [Map(Target.NWS, "96")]
        LostCarrier_After_6_ENQ,
        [Map(Target.NWS, "99")]
        CommunicationError

    }
}
