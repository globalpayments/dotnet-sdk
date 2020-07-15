using System;
using System.Collections.Generic;
using System.Text;
using GlobalPayments.Api.Terminals.Ingenico;
using GlobalPayments.Api.Utils;
using System.Linq;

namespace GlobalPayments.Api.Terminals.Ingenico {
    public class DataResponse {

        private string _authCode;
        private string _finalAmount;
        private PaymentMethod? _paymentMethod;
        private string _cashbackAmount;
        private string _gratuityAmount;
        private string _availableAmount;
        private string _dccCode;
        private string _dccAmount;
        private TransactionSubTypes? _txnSubType;
        private string _splitSaleAmount;
        private DynamicCurrencyStatus? _dccStatus;

        private byte[] _buffer;

        public DataResponse(byte[] buffer) {
            _buffer = buffer;
            ParseData();
        }

        #region Property Fields

        public string AuthorizationCode {
            get { return _authCode ?? ""; }
            private set { }
        }

        public decimal? FinalAmount {
            get { return _finalAmount.ToAmount(); }
            set { _finalAmount = value.ToString(); }
        }

        public PaymentMethod? PaymentMethod {
            get { return _paymentMethod; }
            set { _paymentMethod = value; }
        }

        public decimal? CashbackAmount {
            get { return _cashbackAmount.ToAmount(); }
            set { _cashbackAmount = value.ToString(); }
        }

        public decimal? GratuityAmount {
            get { return _gratuityAmount.ToAmount(); }
            set { _gratuityAmount = value.ToString(); }
        }

        public decimal? AvailableAmount {
            get { return _availableAmount.ToAmount(); }
            set { _availableAmount = value.ToString(); }
        }
        public string DccCode {
            get { return _dccCode; }
            set { _dccCode = value; }
        }

        public decimal? DccAmount {
            get { return _dccAmount.ToAmount(); }
            set { _dccAmount = value.ToString(); }
        }

        public TransactionSubTypes? TransactionSubType {
            get { return _txnSubType; }
            set { _txnSubType = value; }
        }

        public decimal? SplitSaleAmount {
            get { return _splitSaleAmount.ToAmount(); }
            set { _splitSaleAmount = value.ToString(); }
        }

        public DynamicCurrencyStatus? DccStatus {
            get { return _dccStatus; }
            set { _dccStatus = value; }
        }

        #endregion

        private void ParseData() {

            var tlv = new TypeLengthValue(_buffer);

            Type stringType = typeof(string);

            _authCode = (string)tlv.GetValue((byte)RepFieldCode.AuthCode, stringType);
            _cashbackAmount = (string)tlv.GetValue((byte)RepFieldCode.CashbackAmount, stringType);
            _gratuityAmount = (string)tlv.GetValue((byte)RepFieldCode.GratuityAmount, stringType);
            _finalAmount = (string)tlv.GetValue((byte)RepFieldCode.FinalTransactionAmount, stringType);
            _availableAmount = (string)tlv.GetValue((byte)RepFieldCode.AvailableAmount, stringType);
            _dccCode = (string)tlv.GetValue((byte)RepFieldCode.DccCurrency, stringType);
            _dccAmount = (string)tlv.GetValue((byte)RepFieldCode.DccConvertedAmount, stringType);
            _txnSubType = EnumConverter.FromDescription<TransactionSubTypes>(tlv.GetValue((byte)RepFieldCode.TransactionSubType, stringType));
            _dccStatus = (DynamicCurrencyStatus?)tlv.GetValue((byte)RepFieldCode.DccOperationStatus, typeof(DynamicCurrencyStatus?));
            _splitSaleAmount = (string)tlv.GetValue((byte)RepFieldCode.SplitSalePaidAmount, stringType);
            _paymentMethod = (PaymentMethod?)tlv.GetValue((byte)RepFieldCode.PaymentMethod, typeof(PaymentMethod?));
        }
    }
}