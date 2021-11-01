namespace GlobalPayments.Api.Tests.GpApi
{
    public static class GpApi3DSTestCards
    {
        public const string CARDHOLDER_NOT_ENROLLED_V1 = "4917000000000087";
        public const string CARDHOLDER_ENROLLED_V1 = "4012001037141112";

        public const string CARD_AUTH_SUCCESSFUL_V2_1 = "4263970000005262";
        public const string CARD_AUTH_SUCCESSFUL_NO_METHOD_URL_V2_1 = "4222000006724235";
        public const string CARD_AUTH_ATTEMPTED_BUT_NOT_SUCCESSFUL_V2_1 = "4012001037167778";
        public const string CARD_AUTH_FAILED_V2_1 = "4012001037461114";
        public const string CARD_AUTH_ISSUER_REJECTED_V2_1 = "4012001038443335";
        public const string CARD_AUTH_COULD_NOT_BE_PREFORMED_V2_1 = "4012001037484447";
        public const string CARD_CHALLENGE_REQUIRED_V2_1 = "4012001038488884";

        public const string CARD_AUTH_SUCCESSFUL_V2_2 = "4222000006285344";
        public const string CARD_AUTH_SUCCESSFUL_NO_METHOD_URL_V2_2 = "4222000009719489";
        public const string CARD_AUTH_ATTEMPTED_BUT_NOT_SUCCESSFUL_V2_2 = "4222000005218627";
        public const string CARD_AUTH_FAILED_V2_2 = "4222000002144131";
        public const string CARD_AUTH_ISSUER_REJECTED_V2_2 = "4222000007275799";
        public const string CARD_AUTH_COULD_NOT_BE_PREFORMED_V2_2 = "4222000008880910";
        public const string CARD_CHALLENGE_REQUIRED_V2_2 = "4222000001227408";
    }
}