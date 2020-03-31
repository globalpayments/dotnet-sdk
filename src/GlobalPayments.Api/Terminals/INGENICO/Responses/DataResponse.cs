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

        #region Byte Code
        // For less memory allocation;
        private byte _C = 67;
        private byte _Z = 90;
        private byte _Y = 89;
        private byte _M = 77;
        private byte _A = 65;
        private byte _U = 85;
        private byte _O = 79;
        private byte _P = 80;
        private byte _T = 84;
        private byte _S = 83;
        private byte _D = 68;
        #endregion


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

        /**
         * C = AuthCode
         * Z = Cashback Amount
         * Y = Gratuity Amount
         * M = Final Transaction Amount
         * A = Available Amount
         * U = DCC Currency
         * O = DCC Converted transaction amount
         * P = Payment Method
         * T = Transaction Sub-Type
         * S = Split Sale Paid Amount
         * D = DCC Operation Status
        */

        private void ParseData() {

            _authCode = (string)GetValueOfRespField(_C, typeof(string));
            _cashbackAmount = (decimal?)GetValueOfRespField(_Z, typeof(decimal?));
            _gratuityAmount = (decimal?)GetValueOfRespField(_Y, typeof(decimal?));
            _finalAmount = (decimal?)GetValueOfRespField(_M, typeof(decimal?));
            _availableAmount = (decimal?)GetValueOfRespField(_A, typeof(decimal?));
            _dccCode = (string)GetValueOfRespField(_U, typeof(string));
            _dccAmount = (decimal?)GetValueOfRespField(_O, typeof(decimal?));
            _txnSubType = (TransactionSubTypes?)GetValueOfRespField(_T, typeof(TransactionSubTypes?));
            _dccStatus = (DynamicCurrencyStatus?)GetValueOfRespField(_D, typeof(DynamicCurrencyStatus?));
            _splitSaleAmount = (decimal?)GetValueOfRespField(_S, typeof(decimal?));
            _paymentMethod = (PaymentMethod?)GetValueOfRespField(_P, typeof(PaymentMethod?));
        }

        private object GetValueOfRespField(byte toGet, Type returnType) {
            var index = Array.FindIndex(_buffer, e => e == toGet);
            if (index >= 0) {
                // Get the length based on Documention (TLV).
                byte[] lengthBuffer = { _buffer[index + 1], _buffer[index + 2] };
                var length = Convert.ToInt32(Encoding.UTF8.GetString(lengthBuffer, 0, lengthBuffer.Length), 16);

                var _arrValue = _buffer.SubArray(index + 3, length); ;
                var endLength = index + length + 3;
                _buffer = _buffer.SubArray(0, index).Concat(_buffer.SubArray(endLength, _buffer.Length - endLength)).ToArray();
                var strValue = Encoding.ASCII.GetString(_arrValue, 0, _arrValue.Length);


                if (returnType == typeof(decimal?)) {
                    return decimal.Parse(strValue);
                }
                else if (returnType == typeof(string)) {
                    return strValue;
                }
                else if (returnType == typeof(TransactionSubTypes?)) {
                    return  (TransactionSubTypes)int.Parse(Convert.ToInt64(_arrValue[0]).ToString("X2"), System.Globalization.NumberStyles.HexNumber);
                }
                else if (returnType == typeof(DynamicCurrencyStatus?)) {
                    return (DynamicCurrencyStatus)int.Parse(strValue);
                }
                else if (returnType == typeof(PaymentMethod?)) {
                    return (PaymentMethod)int.Parse(strValue);
                }
                else
                    throw new Exception("Data type not supported in parsing of response data.");
            }
            return null;
        }
    }
}