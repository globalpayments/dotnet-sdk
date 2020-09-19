using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System;
using System.Text;

namespace GlobalPayments.Api.Network {
    public class Iso8583Element {
        public DataElementId Id{ get; private set; }
        public DataElementType Type{ get; private set; }
        public string Description{ get; private set; }
        public int Length{ get; private set; }
        public byte[] Buffer{ get; private set; }
        internal byte[] GetSendBuffer() {
            if (Buffer == null) {
                return new byte[0];
            }
            switch (Type) {
                case DataElementType.LVAR:
                case DataElementType.LLVAR:
                case DataElementType.LLLVAR:
                    string length = StringUtils.PadLeft(Buffer.Length, Type.Equals(DataElementType.LVAR) ? 1 : Type.Equals(DataElementType.LLVAR) ? 2 : 3, '0');
                    MessageWriter mw = new MessageWriter();
                    mw.AddRange(Encoding.UTF8.GetBytes(length));
                    mw.AddRange(Buffer);
                    return mw.ToArray();
                default:
                    return Buffer;
            }
        }

        private Iso8583Element() { }

        internal static Iso8583Element Inflate(DataElementId id, DataElementType type, string description, int length, byte[] buffer) {
            Iso8583Element element = new Iso8583Element {
                Id = id,
                Type = type,
                Description = description,
                Length = length,
                Buffer = buffer
            };

            return element;
        }

        internal static Iso8583Element Inflate(DataElementId id, DataElementType type, string description, int length, MessageReader mr) {
            Iso8583Element element = new Iso8583Element {
                Id = id,
                Type = type,
                Description = description,
                Length = length
            };

            switch (type) {
                case DataElementType.LVAR:
                case DataElementType.LLVAR:
                case DataElementType.LLLVAR:
                    string lengthStr = mr.ReadString(type.Equals(DataElementType.LVAR) ? 1 : type.Equals(DataElementType.LLVAR) ? 2 : 3);
                    int actualLength = int.Parse(lengthStr);
                    element.Buffer = mr.ReadBytes(actualLength);
                    break;
                default:
                    element.Buffer = mr.ReadBytes(length);
                    break;
            }
            return element;
        } 

        internal TResult GetConcrete<TResult>() where TResult : IDataElement<TResult> {
            try {
                TResult rvalue = (TResult)Activator.CreateInstance(typeof(TResult));
                return rvalue.FromByteArray(Buffer);
            }
            catch (Exception) {
                return default(TResult);
            }

        }
    }
}
