using GlobalPayments.Api.Terminals;
using System;

namespace GlobalPayments.Api.Utils {
    public class MessageReader {
        byte[] buffer;
        int position = 0;
        long length = 0;

        public long GetLength() { return length; }
        public MessageReader(byte[] bytes) {
            buffer = bytes;
            length = bytes.Length;
        }
        public bool CanRead() {
            return position < length;
        }
        public byte Peek() {
            return buffer[position];
        }
        //public ControlCodes ReadCode() {
        //    return ReadEnum<ControlCodes>();
        //}
        //public T ReadEnum<T>() where T : System.Enum {
        //    ReverseByteEnumMap<T> map = new ReverseByteEnumMap<T>();
        //    return map.Get(buffer[position++]);
        //}
        public byte ReadByte() {
            return buffer[position++];
        }

        public byte[] ReadBytes(int length) {
            byte[] rvalue = new byte[length];

            try {
                for (int i = 0; i < length; i++)
                    rvalue[i] = buffer[position++];
            }
            catch (IndexOutOfRangeException) {
                // eat this exception and return what we have
            }
            return rvalue;
        }

        public char ReadChar() {
            return (char)buffer[position++];
        }

        public string ReadString(int length) {
            string rvalue = "";

            for (int i = 0; i < length; i++)
                rvalue += (char)buffer[position++];

            return rvalue;
        }

        public string ReadToCode(ControlCodes code) {
            return ReadToCode(code, true);
        }
        public string ReadToCode(ControlCodes code, bool removeCode) {
            string rvalue = "";

            try {
                byte value;
                while ((value = Peek()) != (byte)code) {
                    if (Enum.IsDefined(typeof(ControlCodes),value)) {
                        ControlCodes byteCode = (ControlCodes)(buffer[position++]);
                        if (byteCode == ControlCodes.ETX)
                            break;
                        else rvalue += byteCode.ToString();
                    }
                    else rvalue += (char)buffer[position++];
                }
            }
            catch (IndexOutOfRangeException) {
                removeCode = false;
            }

            // pop the code off
            if (removeCode)
                ReadByte();

            return rvalue;
        }

        public void Purge() {
            buffer = new byte[0];
            length = 0;
        }
    }
}
