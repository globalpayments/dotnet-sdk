using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Builders;

namespace GlobalPayments.Api.PaymentMethods {
    public abstract class EBT : IPaymentMethod, IBalanceable, IChargable, IRefundable, IPinProtected {
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
    }

    public class EBTCardData : EBT, ICardData {
        public string ApprovalCode { get; set; }
        public bool CardPresent { get; set; }
        public string Cvn { get; set; }
        public CvnPresenceIndicator CvnPresenceIndicator { get; set; }
        public int? ExpMonth { get; set; }
        public int? ExpYear { get; set; }
        public string Number { get; set; }
        public bool ReaderPresent { get; set; }
        public string SerialNumber { get; set; }
    }

    public class EBTTrackData : EBT, ITrackData, IEncryptable {
        public EncryptionData EncryptionData { get; set; }
        public EntryMethod EntryMethod { get; set; }
        public string Value { get; set; }
    }
}
