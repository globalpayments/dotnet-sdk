using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Ingenico;
using System;
using System.Linq;
using System.Text;

namespace GlobalPayments.Api.Utils {
    internal class TypeLengthValue {

        private byte[] _data = new byte[0];

        public TypeLengthValue() {

        }

        public TypeLengthValue(byte[] data) {
            _data = data;
        }

        // Add TLV Format since Ingenico has different format of length when it comes to TLV Standard.
        public TLVFormat TLVFormat { get; private set; }

        public object GetValue(byte type, Type returnType, TLVFormat format = TLVFormat.Standard) {
            if (_data.Length == 0) {
                throw new Exception("No data to parse.");
            }

            TLVFormat = format;
            int typeIndexLocation = Array.FindIndex(_data, e => e == type);
            if (typeIndexLocation >= 0) {
                // Get the length based on Documentation (TLV).
                byte[] lengthBuffer = { _data[typeIndexLocation + 1], _data[typeIndexLocation + 2] };
                int length = 0;

                if (TLVFormat == TLVFormat.Standard) {
                    length = Convert.ToInt32(Encoding.GetEncoding(28591).GetString(lengthBuffer, 0, lengthBuffer.Length), 16);
                } else if (TLVFormat == TLVFormat.State || TLVFormat == TLVFormat.PayAtTable) {
                    length = Convert.ToInt32(Encoding.GetEncoding(28591).GetString(lengthBuffer, 0, lengthBuffer.Length));
                } else {
                    throw new ApiException("Unsupported TLV format.");
                }

                // Get the value of type according to length limit.
                byte[] value = _data.SubArray(typeIndexLocation + 3, length);

                int endLength = typeIndexLocation + length + 3;

                // Remove field that have been parsed and successfully get the value.
                _data = _data.SubArray(0, typeIndexLocation).Concat(_data.SubArray(endLength, _data.Length - endLength)).ToArray();
                string strValue = Encoding.GetEncoding(28591).GetString(value, 0, value.Length);


                if (returnType == typeof(decimal?)) {
                    return decimal.Parse(strValue);
                } else if (returnType == typeof(string)) {
                    return strValue;
                } else if (returnType == typeof(DynamicCurrencyStatus?)) {
                    return (DynamicCurrencyStatus)int.Parse(strValue);
                } else if (returnType == typeof(PaymentMethod?)) {
                    return (PaymentMethod)int.Parse(strValue);
                } else {
                    throw new Exception("Data type not supported in parsing of TLV data.");
                }
            }
            return null;
        }
    }
}
