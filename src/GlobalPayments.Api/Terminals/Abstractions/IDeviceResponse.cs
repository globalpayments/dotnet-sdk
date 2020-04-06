using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Terminals.Abstractions {
    public interface IDeviceResponse {
        string Status { get; set; }
        string Command { get; set; }
        string Version { get; set; }
        string DeviceResponseCode { get; set; }
        string DeviceResponseText { get; set; }
        string ReferenceNumber { get; set; }
        string ToString();
    }

    public interface ITerminalResponse : IDeviceResponse {
        string ResponseCode { get; set; }
        string ResponseText { get; set; }
        string TransactionId { get; set; }
        string TerminalRefNumber { get; set; }
        string Token { get; set; }
        string SignatureStatus { get; set; }
        byte[] SignatureData { get; set; }
        string TransactionType { get; set; }
        string MaskedCardNumber { get; set; }
        string EntryMethod { get; set; }
        string AuthorizationCode { get; set; }
        string ApprovalCode { get; set; }
        decimal? TransactionAmount { get; set; }
        decimal? AmountDue { get; set; }
        decimal? BalanceAmount { get; set; }
        string CardHolderName { get; set; }
        string CardBIN { get; set; }
        bool CardPresent { get; set; }
        string ExpirationDate { get; set; }
        decimal? TipAmount { get; set; }
        decimal? CashBackAmount { get; set; }
        string AvsResponseCode { get; set; }
        string AvsResponseText { get; set; }
        string CvvResponseCode { get; set; }
        string CvvResponseText { get; set; }
        bool TaxExempt { get; set; }
        string TaxExemptId { get; set; }
        string TicketNumber { get; set; }
        string PaymentType { get; set; }
        string ApplicationPreferredName { get; set; }
        string ApplicationLabel { get; set; }
        string ApplicationId { get; set; }
        ApplicationCryptogramType ApplicationCryptogramType { get; set; }
        string ApplicationCryptogram { get; set; }
        string CardHolderVerificationMethod { get; set; }
        string TerminalVerificationResults { get; set; }
        decimal? MerchantFee { get; set; }
    }

    public interface ITerminalReport : IDeviceResponse { }
}
