using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Extensions;
using System.IO;
using System.Text;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Utils;
using System;

namespace GlobalPayments.Api.Terminals.Ingenico {
    public abstract class IngenicoBaseResponse : DeviceResponse {
        protected byte[] _buffer;
        protected ParseFormat _parseFormat;
        internal DataResponse _respField;

        #region Added Properties Specific for Ingenico
        public string DccCurrency { get; set; }
        public DynamicCurrencyStatus? DccStatus { get; set; }
        public decimal? DccAmount { get; set; }
        public TransactionSubTypes? TransactionSubType { get; set; }
        public decimal? SplitSaleAmount { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public string CurrencyCode { get; set; }
        public string PrivateData { get; set; }
        public decimal? FinalTransactionAmount { get; set; }

        internal string Amount { get; set; }

        #endregion

        internal IngenicoBaseResponse(byte[] buffer, ParseFormat format = ParseFormat.Transaction) {
            _buffer = buffer;
            _parseFormat = format;
            ParseResponse(_buffer);
        }


        public virtual void ParseResponse(byte[] response) {
            if (response != null) {

                ReferenceNumber = Encoding.UTF8.GetString(response.SubArray(0, 2));
                Status = ((TransactionStatus)Encoding.UTF8.GetString(response.SubArray(2, 1)).ToInt32()).ToString();
                Amount = Encoding.UTF8.GetString(response.SubArray(3, 8));
                PaymentMode = (PaymentMode)Encoding.UTF8.GetString(response.SubArray(11, 1)).ToInt32();
                CurrencyCode = Encoding.UTF8.GetString(response.SubArray(67, 3));
                PrivateData = Encoding.UTF8.GetString(response.SubArray(70, response.Length - 70));

                // This is for parsing of Response field for Transaction request
                if (_parseFormat == ParseFormat.Transaction) {
                    _respField = new DataResponse(response.SubArray(12, 55));

                    DccAmount = _respField.DccAmount;
                    DccCurrency = _respField.DccCode;
                    DccStatus = _respField.DccStatus;
                }
            }
        }

        public override string ToString() {
            DeviceResponseText = Encoding.UTF8.GetString(_buffer, 0, _buffer.Length);
            return DeviceResponseText;
        }

    }

    public class IngenicoTerminalResponse : IngenicoBaseResponse, ITerminalResponse {

        internal IngenicoTerminalResponse(byte[] buffer, ParseFormat format = ParseFormat.Transaction) : base(buffer, format) { }

        #region Properties
        public decimal? TransactionAmount { get { return Amount.ToAmount(); } set { } }
        public decimal? BalanceAmount { get { return _respField.AvailableAmount; } set { } }
        public string AuthorizationCode { get { return _respField.AuthorizationCode ?? ""; } set { } }
        public decimal? TipAmount { get { return _respField.GratuityAmount; } set { } }
        public decimal? CashBackAmount { get { return _respField.CashbackAmount; } set { } }
        public string PaymentType { get { return _respField.PaymentMethod.ToString(); } set { } }
        public string TerminalRefNumber { get { return ReferenceNumber; } set { } }

        public string ResponseCode { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string SignatureStatus { get; set; }
        public byte[] SignatureData { get; set; }
        public string TransactionType { get; set; }
        public string MaskedCardNumber { get; set; }
        public string EntryMethod { get; set; }
        public string ApprovalCode { get; set; }
        public decimal? AmountDue { get; set; }
        public string CardHolderName { get; set; }
        public string CardBIN { get; set; }
        public bool CardPresent { get; set; }
        public string ExpirationDate { get; set; }
        public string AvsResponseCode { get; set; }
        public string AvsResponseText { get; set; }
        public string CvvResponseCode { get; set; }
        public string CvvResponseText { get; set; }
        public bool TaxExempt { get; set; }
        public string TaxExemptId { get; set; }
        public string TicketNumber { get; set; }
        public ApplicationCryptogramType ApplicationCryptogramType { get; set; }
        public string ApplicationCryptogram { get; set; }
        public string CardHolderVerificationMethod { get; set; }
        public string TerminalVerificationResults { get; set; }
        public string ApplicationPreferredName { get; set; }
        public string ApplicationLabel { get; set; }
        public string ApplicationId { get; set; }
        public decimal? MerchantFee { get; set; }
        public string ResponseText { get; set; }
        #endregion

    }

    public class IngenicoTerminalReportResponse : IngenicoBaseResponse, ITerminalReport {

        internal IngenicoTerminalReportResponse(byte[] buffer) : base(buffer) {
            _buffer = buffer;
            Status = _buffer.Length > 0 ? "SUCCESS" : "FAILED";
        }

        public override string ToString() {
            return Encoding.ASCII.GetString(_buffer);
        }
    }
}