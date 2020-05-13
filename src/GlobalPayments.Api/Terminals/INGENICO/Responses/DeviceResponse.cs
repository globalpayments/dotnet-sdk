using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Extensions;
using System.IO;
using System.Text;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Utils;
using System;

namespace GlobalPayments.Api.Terminals.Ingenico {
    public abstract class IngenicoBaseResponse : IDeviceResponse {
        public string Status { get; set; }
        public string Command { get; set; }
        public string Version { get; set; }
        public string DeviceResponseCode { get; set; }
        public string DeviceResponseText { get; set; }
        public string ReferenceNumber { get; set; }
    }

    public class IngenicoTerminalResponse : IngenicoBaseResponse, ITerminalResponse {

        internal string _transactionStatus;
        internal decimal _amount;
        internal PaymentMode _paymentMode;
        internal string _privateData;
        internal string terminalRawData;
        internal string _currencyCode;
        internal DataResponse _respField;
        
        private byte[] _buffer;
        private ParseFormat _parseFormat;

        internal IngenicoTerminalResponse(byte[] buffer, ParseFormat format = ParseFormat.Transaction) {
            _buffer = buffer;
            _parseFormat = format;
            ParseResponse(buffer);
        }

        public override string ToString() {
            base.DeviceResponseText = Encoding.UTF8.GetString(_buffer, 0, _buffer.Length);
            return base.DeviceResponseText;
        }

        #region Added Properties Specific for Ingenico
        public string DccCurrency { get { return _respField.DccCode; } set { } }
        public DynamicCurrencyStatus? DccStatus { get { return _respField.DccStatus; } set { } }
        public TransactionSubTypes? TransactionSubType { get { return _respField.TransactionSubType; } set { } }
        public decimal SplitSaleAmount { get { return 0; } set { } }
        public PaymentMode PaymentMode { get { return _paymentMode; } set { } }
        public string DynamicCurrencyCode { get { return _respField.DccCode; } }
        public string CurrencyCode { get { return _currencyCode; } set { } }
        public string PrivateData { get { return _privateData; } }
        public decimal? FinalTransactionAmount { get { return _respField.FinalAmount; } }
        #endregion

        #region Properties
        public string ResponseText { get { return terminalRawData; } set { } }
        public decimal? TransactionAmount { get { return _amount.ToString().ToAmount(); } set { } }
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
        #endregion

        /** Index
        0 - 1 = Epos Number
        2 - Transaction Status
        3 - 10 = Amount
        11 = Payment Mode
        12 - 20 = AuthCode or starts with (C)
        21 - 34 = Final Transaction Amount or starts with (M)
        35 - 38 = Payment Method or starts with (P)
        67 - 69 = Currency
        70 - 79 = Private Data
        */
        public virtual void ParseResponse(byte[] response) {
            if (response != null) {

                ReferenceNumber = Encoding.UTF8.GetString(response.SubArray(0, 2));
                _transactionStatus = ((TransactionStatus)Encoding.UTF8.GetString(response.SubArray(2, 1)).ToInt32()).ToString();
                decimal.TryParse(Encoding.UTF8.GetString(response.SubArray(3, 8)), out _amount);
                _paymentMode = (PaymentMode)Encoding.UTF8.GetString(response.SubArray(11, 1)).ToInt32();
                _currencyCode = Encoding.UTF8.GetString(response.SubArray(67, 3));
                _privateData = Encoding.UTF8.GetString(response.SubArray(70, response.Length - 70));
                Status = _transactionStatus;

                // This is for parsing of Response field for Transaction request
                if (_parseFormat == ParseFormat.Transaction) {
                    _respField = new DataResponse(response.SubArray(12, 55));
                }
            }
        }
    }

    public class IngenicoTerminalReportResponse : IngenicoBaseResponse, ITerminalReport {
        private byte[] _buffer;

        internal IngenicoTerminalReportResponse(byte[] buffer) {
            _buffer = buffer;
            Status = _buffer.Length > 0 ? "SUCCESS" : "FAILED";
        }

        public override string ToString() {
            return Encoding.ASCII.GetString(_buffer);
        }


    }
}