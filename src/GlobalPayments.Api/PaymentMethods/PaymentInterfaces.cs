using System;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.PaymentMethods
{
    public interface IPaymentMethod {
        PaymentMethodType PaymentMethodType { get; }
    }

    public interface ICardData {
        bool CardPresent { get; set; }
        string Cvn { get; set; }
        CvnPresenceIndicator CvnPresenceIndicator { get; set; }
        string Number { get; set; }
        int? ExpMonth { get; set; }
        int? ExpYear { get; set; }
        bool ReaderPresent { get; set; }
    }

    public interface ITrackData {
        string Value { get; set; }
        EntryMethod EntryMethod { get; set; }
    }

    //public abstract class CardData : IPaymentMethod {
    //    public bool CardPresent { get; set; }
    //    private string cvn;
    //    public string Cvn {
    //        get { return cvn; }
    //        set {
    //            cvn = value;
    //            if (!string.IsNullOrEmpty(value))
    //                CvnPresenceIndicator = CvnPresenceIndicator.Present;
    //        }
    //    }
    //    public CvnPresenceIndicator CvnPresenceIndicator { get; set; }
    //    public string Number { get; set; }
    //    public int ExpMonth { get; set; }
    //    public int ExpYear { get; set; }
    //    public PaymentMethodType PaymentMethodType { get; private set; }
    //    public bool ReaderPresent { get; set; }

    //    public CardData(PaymentMethodType type) {
    //        PaymentMethodType = type;
    //    }
    //}

    //public abstract class TrackData : IPaymentMethod {
    //    public string Value { get; set; }
    //    public EntryMethod EntryMethod { get; set; }
    //    public PaymentMethodType PaymentMethodType { get; private set; }

    //    public TrackData(PaymentMethodType type) {
    //        PaymentMethodType = type;
    //    }
    //}

    interface IAuthable {
        AuthorizationBuilder Authorize(decimal? amount = null);
    }

    interface IChargable {
        AuthorizationBuilder Charge(decimal? amount = null);
    }

    interface IBalanceable {
        AuthorizationBuilder BalanceInquiry(InquiryType? inquiry);
    }

    interface IEditable {
        ManagementBuilder Edit(decimal? amount = null);
    }

    interface IEncryptable {
        EncryptionData EncryptionData { get; set; }
    }

    interface IPinProtected {
        string PinBlock { get; set; }
    }

    interface IPrePayable {
        AuthorizationBuilder AddValue(decimal? amount = null);
    }

    interface IRefundable {
        AuthorizationBuilder Refund(decimal? amount = null);
    }

    interface IReversable {
        AuthorizationBuilder Reverse(decimal? amount = null);
    }

    interface ITokenizable {
        string Token { get; set; }
        string Tokenize();
    }

    interface IVerifiable {
        AuthorizationBuilder Verify();
    }

    interface IVoidable {
        
    }
}
