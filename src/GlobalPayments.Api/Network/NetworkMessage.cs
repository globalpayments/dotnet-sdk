using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GlobalPayments.Api.Network {
    public class NetworkMessage {
        public string MessageTypeIndicator { get; set; }
        private Dictionary<DataElementId, Iso8583Element> elements;
        public Iso8583Bitmap Bitmap { get; private set; }
        private Iso8583Bitmap secondaryBitmap;
        private Iso8583MessageType messageType;
        public Iso8583ElementFactory Factory { get; private set; }
        public bool IsDataCollect(PaymentMethodType paymentMethodType) {
            string functionCode = GetString(DataElementId.DE_024);
            string reasonCode = GetString(DataElementId.DE_025);
            if (MessageTypeIndicator.Equals("1200") && functionCode.Equals("200")) {
                return true;
            }
            else if (MessageTypeIndicator.Equals("1220") || MessageTypeIndicator.Equals("1221")) {
                if (functionCode.Equals("200")) {
                    return (paymentMethodType.Equals(PaymentMethodType.Credit));
                }
                else if (functionCode.Equals("201") || functionCode.Equals("202")) {
                    if (paymentMethodType != default(PaymentMethodType) && (paymentMethodType.Equals(PaymentMethodType.Debit) || paymentMethodType.Equals(PaymentMethodType.EBT))) {
                        return reasonCode.Equals("1379");
                    }
                    return (reasonCode.Equals("1376") || reasonCode.Equals("1377") || reasonCode.Equals("1378") || reasonCode.Equals("1381"));
                }
                return false;
            }
            else if (MessageTypeIndicator.Equals("1420")) {
                return true;
            }
            return false;
        }

        public NetworkMessage(Iso8583MessageType messageType = Iso8583MessageType.CompleteMessage) {
            this.messageType = messageType;
            elements = new Dictionary<DataElementId, Iso8583Element>();
            Factory = Iso8583ElementFactory.GetConfiguredFactory(messageType);
        }

        public bool Has(DataElementId id) {
            return elements.ContainsKey(id);
        }

        public decimal GetAmount(DataElementId id) {
            if (elements.ContainsKey(id)) {
                Iso8583Element element = elements[id];
                return StringUtils.ToAmount(Encoding.ASCII.GetString(element.Buffer));
            }
            return 0;
        }

        public byte[] GetByteArray(DataElementId id) {
            if (elements.ContainsKey(id)) {
                Iso8583Element element = elements[id];
                return element.Buffer;
            }
            return null;
        }

        public DateTime? GetDate(DataElementId id, string formatter) {
            string value = GetString(id);
            if (!string.IsNullOrEmpty(value)) {
                try {
                    return DateTime.ParseExact(value,formatter, CultureInfo.InvariantCulture);
                }
                catch (Exception) {
                    return null;
                }
            }
            return null;
        }

        public string GetString(DataElementId id) {
            if (elements.ContainsKey(id)) {
                Iso8583Element element = elements[id];
                return Encoding.ASCII.GetString(element.Buffer);
            }
            return null;
        }

        public TResult GetDataElement<TResult>(DataElementId id) where TResult : IDataElement<TResult> {
            if (elements.ContainsKey(id)) {
                Iso8583Element element = elements[id];
                return element.GetConcrete<TResult>();
            }
            return default(TResult);
        }
        
        public DE48_CardType GetStringConstant(DataElementId id) {
            if (elements.ContainsKey(id)) {
                Iso8583Element element = elements[id];
                string value = Encoding.ASCII.GetString(element.Buffer);

                //DE48_CardType rvalue = (DE48_CardType)Enum.Parse(typeof(DE48_CardType),StringUtils.Trim(value));
                DE48_CardType rvalue = EnumConverter.FromMapping<DE48_CardType>(Target.NWS, StringUtils.Trim(value));
                if (rvalue == default(DE48_CardType)) {
                    rvalue = EnumConverter.FromMapping<DE48_CardType>(Target.NWS, value);
                }
                return rvalue;
            }
            return default(DE48_CardType);
        }

        public DE48_AdministrativelyDirectedTaskCode GetByteConstant(DataElementId id) {
            if (elements.ContainsKey(id)) {
                Iso8583Element element = elements[id];
                return (DE48_AdministrativelyDirectedTaskCode)(Object)(element.Buffer[0]);
            }
            return default(DE48_AdministrativelyDirectedTaskCode);
        }        

        public NetworkMessage Set(DataElementId id, string value) {
            if (value != null) {
                return Set(id, Encoding.UTF8.GetBytes(value));
            }
            return this;
        }

        /// <summary>
        /// commented the below 2 methods as it implemets the same as set method acceptiong string as 2nd param
        /// </summary>        

        public NetworkMessage Set<T>(DataElementId id, IDataElement<T> element) where  T : class {
            if (element != null) {
                return Set(id, element.ToByteArray());
            }
            return this;
        }

        public NetworkMessage Set(DataElementId id, byte[] buffer) {
            if (buffer != null) {
                Iso8583Element element = Factory.CreateElement(id, buffer);
                elements[id] = element;
            }
            return this;
        }

        public byte[] BuildMessage() {
            return BuildMessage(false);
        }

        public byte[] BuildMessage(bool addBitmapAsString) {
            MessageWriter mw = new MessageWriter();

            // put the MTI
            if (!string.IsNullOrEmpty(MessageTypeIndicator)) {
                mw.AddRange(Encoding.UTF8.GetBytes(MessageTypeIndicator));
            }

            // deal with the bitmaps
            GenerateBitmaps();
            if (addBitmapAsString) {
                mw.AddRange(Encoding.UTF8.GetBytes(Bitmap.ToHexString()));
            }
            else {
                mw.AddRange(Bitmap.ToByteArray());
            }

            // primary bitmap
            DataElementId currentElement = Bitmap.GetNextDataElement();
            do {
                if (elements.ContainsKey(currentElement)) {
                    Iso8583Element element = elements[currentElement];
                    mw.AddRange(element.GetSendBuffer());
                }
                currentElement = Bitmap.GetNextDataElement();
            }
            while (currentElement != 0);

            // secondary bitmap
            if (messageType.Equals(Iso8583MessageType.CompleteMessage)) {
                currentElement = secondaryBitmap.GetNextDataElement();
                while (currentElement != 0) {
                    Iso8583Element element = elements[currentElement];
                    mw.AddRange(element.GetSendBuffer());

                    currentElement = secondaryBitmap.GetNextDataElement();
                }
            }

            return mw.ToArray();
        }

        private void GenerateBitmaps() {
            Bitmap = new Iso8583Bitmap(new byte[8]);

            // check if we need a secondary bitmap
            if (messageType.Equals(Iso8583MessageType.CompleteMessage)) {
                secondaryBitmap = new Iso8583Bitmap(new byte[8], 64);
                Bitmap.SetDataElement(DataElementId.DE_001);
            }

            foreach (DataElementId elementType in elements.Keys) {
                if (secondaryBitmap != null && elementType.GetHashCode() > 64) {
                    secondaryBitmap.SetDataElement(elementType);
                }
                else {
                    Bitmap.SetDataElement(elementType);
                }
            }

            // put the finished secondary bitmap to the elements
            if (messageType.Equals(Iso8583MessageType.CompleteMessage)) {
                Set(DataElementId.DE_001, secondaryBitmap.ToByteArray());
            }
        }

        public static NetworkMessage Parse(string input, Iso8583MessageType messageType) {
            MessageReader mr = new MessageReader(Encoding.UTF8.GetBytes(input));
            Iso8583Bitmap bitmap = new Iso8583Bitmap(StringUtils.BytesFromHex(mr.ReadString(16)));

            return ParseMessage(bitmap, mr, messageType);
        }

        public static NetworkMessage Parse(byte[] input, Iso8583MessageType messageType) {
            MessageReader mr = new MessageReader(input);
            Iso8583Bitmap bitmap = new Iso8583Bitmap(mr.ReadBytes(8));

            return ParseMessage(bitmap, mr, messageType);
        }

        private static NetworkMessage ParseMessage(Iso8583Bitmap bitmap, MessageReader mr, Iso8583MessageType messageType) {
            NetworkMessage message = new NetworkMessage(messageType);
            message.Bitmap = bitmap;

            // initialize the factory
            message.Factory = Iso8583ElementFactory.GetConfiguredFactory(mr, messageType);

            // read the primary bitmap
            DataElementId currentElement = bitmap.GetNextDataElement();
            do {
                message.elements[currentElement] = message.Factory.CreateElement(currentElement);
                currentElement = bitmap.GetNextDataElement();
            }
            while (currentElement != 0);

            // check for secondary bitmap
            if (message.Has(DataElementId.DE_001)) {
                byte[] secondaryBuffer = message.GetByteArray(DataElementId.DE_001);
                Iso8583Bitmap secondaryMap = new Iso8583Bitmap(secondaryBuffer, 64);

                currentElement = secondaryMap.GetNextDataElement();
                while (currentElement != 0) {
                    message.elements[currentElement] = message.Factory.CreateElement(currentElement);
                    currentElement = secondaryMap.GetNextDataElement();
                }
            }

            // return the document
            return message;
        }

        public new string ToString() {
            StringBuilder sb = new StringBuilder();

            // put the MTI
            if (!string.IsNullOrEmpty(MessageTypeIndicator)) {
                sb.Append(string.Format("MTI: {0}\r\n", MessageTypeIndicator));
            }

            // deal with the bitmaps
            GenerateBitmaps();
            sb.Append(string.Format("P_BITMAP: {0}\r\n", Bitmap.ToHexString()));

            // primary bitmap
            DataElementId currentElement = Bitmap.GetNextDataElement();
            do {
                Iso8583Element element = elements[currentElement];
                if (currentElement.Equals(DataElementId.DE_001)) {
                    sb.Append(string.Format("S_BITMAP: {0}\r\n", secondaryBitmap.ToHexString()));
                }
                else {
                    // special handling for DE 55
                    if (element.Id.Equals(DataElementId.DE_055)) {
                        byte[] buffer = element.Buffer;
                        sb.Append(string.Format("{0}: {1}{2}\r\n", element.Id, StringUtils.PadLeft(buffer.Length, 3, '0'), StringUtils.HexFromBytes(buffer)));
                    }
                    else sb.Append(string.Format("{0}: {1}\r\n", element.Id, Encoding.UTF8.GetString(element.GetSendBuffer())));
                }

                currentElement = Bitmap.GetNextDataElement();
            }
            while (currentElement != 0);

            // secondary bitmap
            if (messageType.Equals(Iso8583MessageType.CompleteMessage)) {
                currentElement = secondaryBitmap.GetNextDataElement();
                while (currentElement != 0) {
                    Iso8583Element element = elements[currentElement];
                    sb.Append(string.Format("{0}: {1}\r\n", element.Id, Encoding.ASCII.GetString(element.GetSendBuffer())));

                    currentElement = secondaryBitmap.GetNextDataElement();
                }
            }

            return sb.ToString();
        }
    }
}
