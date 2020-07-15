using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Ingenico;
using System;
using System.Linq;
using System.Text;

namespace GlobalPayments.Api.Utils {
    internal class TypeLengthValue {

        private byte[] _data = new byte[0];
        private TLVFormat _format = TLVFormat.Standard;

        public TypeLengthValue() {

        }

        public TypeLengthValue(byte[] data) {
            _data = data;
        }

        // Add TLV Format since Ingenico has different format of length when it comes to TLV Standard.
        public TLVFormat TLVFormat {
            get { return _format; }
            set { _format = value; }
        }


        public object GetValue(byte type, Type returnType, TLVFormat? format = null) {
            if (_data.Length == 0) {
                throw new Exception("No data to parse.");
            }

            int typeIndexLocation = Array.FindIndex(_data, e => e == type);
            if (typeIndexLocation >= 0) {
                // Get the length based on Documentation (TLV).
                byte[] lengthBuffer = { _data[typeIndexLocation + 1], _data[typeIndexLocation + 2] };
                int length = 0;

                if ((format != null && format == TLVFormat.Standard) || _format == TLVFormat.Standard) {
                    length = Convert.ToInt32(Encoding.UTF8.GetString(lengthBuffer, 0, lengthBuffer.Length), 16);
                }
                else if ((format != null && format == TLVFormat.State) || _format == TLVFormat.State) {
                    length = Convert.ToInt32(Encoding.UTF8.GetString(lengthBuffer, 0, lengthBuffer.Length));
                }
                else {
                    throw new ApiException("Unsupported TLV format.");
                }

                // Get the value of type according to length limit.
                byte[] value = _data.SubArray(typeIndexLocation + 3, length);

                int endLength = typeIndexLocation + length + 3;

                // Remove field that have been parsed and successfully get the value.
                _data = _data.SubArray(0, typeIndexLocation).Concat(_data.SubArray(endLength, _data.Length - endLength)).ToArray();
                string strValue = Encoding.ASCII.GetString(value, 0, value.Length);


                if (returnType == typeof(decimal?)) {
                    return decimal.Parse(strValue);
                }
                else if (returnType == typeof(string)) {
                    return strValue;
                }
                else if (returnType == typeof(DynamicCurrencyStatus?)) {
                    return (DynamicCurrencyStatus)int.Parse(strValue);
                }
                else if (returnType == typeof(PaymentMethod?)) {
                    return (PaymentMethod)int.Parse(strValue);
                }
                else {
                    throw new Exception("Data type not supported in parsing of TLV data.");
                }
            }
            return null;
        }
    }
}
