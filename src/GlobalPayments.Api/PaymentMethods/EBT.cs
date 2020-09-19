using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Builders;

namespace GlobalPayments.Api.PaymentMethods {
    /// <summary>
    /// Use EBT as a payment method.
    /// </summary>
    public abstract class EBT : IPaymentMethod, IBalanceable, IChargable, IRefundable, IPinProtected {
        /// <summary>
        /// Set to `PaymentMethodType.EBT` for internal methods.
        /// </summary>
        public EbtCardType EbtCardType { get; set; }
        public PaymentMethodType PaymentMethodType { get { return PaymentMethodType.EBT; } }
        public string PinBlock { get; set; }

        public AuthorizationBuilder BalanceInquiry(InquiryType? inquiry = InquiryType.FOODSTAMP) {
            return new AuthorizationBuilder(TransactionType.Balance, this).WithBalanceInquiryType(inquiry).WithAmount(0m);
        }

        public AuthorizationBuilder BenefitWithdrawal(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.BenefitWithdrawal, this).WithAmount(amount).WithCashBack(0m);
        }

        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale, this).WithAmount(amount);
        }

        public AuthorizationBuilder Refund(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Refund, this).WithAmount(amount);
        }
        public AuthorizationBuilder Authorize(decimal? amount = null, bool isEstimated = true) {
            return new AuthorizationBuilder(TransactionType.Auth, this)
                    .WithAmount(amount)
                    .WithAmountEstimated(true);
        }
    }

    ///// <summary>
    ///// Use EBT manual entry data as a payment method.
    ///// </summary>
    //public class EBTCardData : EBT, ICardData {
    //    public string ApprovalCode { get; set; }
    //    public bool CardPresent { get; set; }
    //    public string Cvn { get; set; }
    //    public CvnPresenceIndicator CvnPresenceIndicator { get; set; }
    //    public int? ExpMonth { get; set; }
    //    public int? ExpYear { get; set; }
    //    public string Number { get; set; }
    //    public bool ReaderPresent { get; set; }
    //    public string SerialNumber { get; set; }
    //    public string ShortExpiry {
    //        get {
    //            var month = (ExpMonth.HasValue) ? ExpMonth.ToString().PadLeft(2, '0') : string.Empty;
    //            var year = (ExpYear.HasValue) ? ExpYear.ToString().PadLeft(4, '0').Substring(2, 2) : string.Empty;
    //            return month + year;
    //        }
    //    }
    //    public EBTCardData() { }
    //    public EBTCardData(EbtCardType cardType) {
    //        EbtCardType = cardType;
    //    }
    //}

    ///// <summary>
    ///// Use EBT track data as a payment method.
    ///// </summary>
    //public class EBTTrackData : EBT, ITrackData, IEncryptable {
    //    public EncryptionData EncryptionData { get; set; }
    //    public EntryMethod EntryMethod { get; set; }
    //    public string Value { get; set; }
    //    public TrackNumber TrackNumber { get; set; }
    //    public string TrackData { get; set; }
    //    public string Pan { get; set; }
    //    public string DiscretionaryData { get; set; }
    //    public string Expiry { get; set; }
    //    public EBTTrackData() { }
    //    public EBTTrackData(EbtCardType cardType) {
    //        EbtCardType = cardType;
    //    }
    //}
}
