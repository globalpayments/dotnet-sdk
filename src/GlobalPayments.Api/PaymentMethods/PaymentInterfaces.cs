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
        string ShortExpiry { get; }
    }

    public interface ITrackData {
        string Expiry { get; set; }
        string Pan { get; set; }
        TrackNumber TrackNumber { get; set; }
        string TrackData { get; set; }
        string DiscretionaryData { get; set; }
        string Value { get; set; }
        EntryMethod EntryMethod { get; set; }
    }

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
        string Tokenize(string configName = "default");
        bool UpdateTokenExpiry(string configName = "default");
        bool DeleteToken(string configName = "default");
    }

    interface IVerifiable {
        AuthorizationBuilder Verify();
    }

    interface ISecure3d {
        ThreeDSecure ThreeDSecure { get; set; }
    }

    interface IVoidable { }
}
