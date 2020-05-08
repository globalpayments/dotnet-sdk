using System;
using System.Collections.Generic;
using System.Text;
using GlobalPayments.Api.Terminals.Ingenico;
using GlobalPayments.Api.Utils;
using System.Linq;

namespace GlobalPayments.Api.Terminals.Ingenico {
    public class DataResponse {

        private string _authCode;
        private decimal? _finalAmount;
        private PaymentMethod? _paymentMethod;
        private decimal? _cashbackAmount;
        private decimal? _gratuityAmount;
        private decimal? _availableAmount;
        private string _dccCode;
        private decimal? _dccAmount;
        private TransactionSubTypes? _txnSubType;
        private decimal? _splitSaleAmount;
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
            get { return _finalAmount.ToString().ToAmount(); }
            set { _finalAmount = value; }
        }

        public PaymentMethod? PaymentMethod {
            get { return _paymentMethod; }
            set { _paymentMethod = value; }
        }

        public decimal? CashbackAmount {
            get { return _cashbackAmount.ToString().ToAmount(); }
            set { _cashbackAmount = value; }
        }

        public decimal? GratuityAmount {
            get { return _gratuityAmount.ToString().ToAmount(); }
            set { _gratuityAmount = value; }
        }

        public decimal? AvailableAmount {
            get { return _availableAmount.ToString().ToAmount(); }
            set { _availableAmount = value; }
        }
        public string DccCode {
            get { return _dccCode; }
            set { _dccCode = value; }
        }

        public decimal? DccAmount {
            get { return _dccAmount.ToString().ToAmount(); }
            set { _dccAmount = value; }
        }

        public TransactionSubTypes? TransactionSubType {
            get { return _txnSubType; }
            set { _txnSubType = value; }
        }

        public decimal? SplitSaleAmount {
            get { return _splitSaleAmount.ToString().ToAmount(); }
            set { _splitSaleAmount = value; }
        }

        public DynamicCurrencyStatus? DccStatus {
            get { return _dccStatus; }
            set { _dccStatus = value; }
        }

        #endregion

        private void ParseData() {

            var tlv = new TypeLengthValue(_buffer);

            Type stringType = typeof(string);
            Type decimalType = typeof(decimal?);

            _authCode = (string)tlv.GetValue((byte)RepFieldCode.AuthCode, stringType);
            _cashbackAmount = (decimal?)tlv.GetValue((byte)RepFieldCode.CashbackAmount, decimalType);
            _gratuityAmount = (decimal?)tlv.GetValue((byte)RepFieldCode.GratuityAmount, decimalType);
            _finalAmount = (decimal?)tlv.GetValue((byte)RepFieldCode.FinalTransactionAmount, decimalType);
            _availableAmount = (decimal?)tlv.GetValue((byte)RepFieldCode.AvailableAmount, decimalType);
            _dccCode = (string)tlv.GetValue((byte)RepFieldCode.DccCurrency, stringType);
            _dccAmount = (decimal?)tlv.GetValue((byte)RepFieldCode.DccConvertedAmount, decimalType);
            _txnSubType = (TransactionSubTypes?)tlv.GetValue((byte)RepFieldCode.TransactionSubType, typeof(TransactionSubTypes?));
            _dccStatus = (DynamicCurrencyStatus?)tlv.GetValue((byte)RepFieldCode.DccOperationStatus, typeof(DynamicCurrencyStatus?));
            _splitSaleAmount = (decimal?)tlv.GetValue((byte)RepFieldCode.SplitSalePaidAmount, decimalType);
            _paymentMethod = (PaymentMethod?)tlv.GetValue((byte)RepFieldCode.PaymentMethod, typeof(PaymentMethod?));
        }
    }
}