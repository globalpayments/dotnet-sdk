using GlobalPayments.Api.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Utils {

    public class StringParser {
        private int position = 0;
        private string buffer;

        public StringParser(byte[] buffer) : this(Encoding.UTF8.GetString(buffer)) {

        }
        public StringParser(string str) {
            this.buffer = str;
        }

        public bool? ReadBoolean() {
            return ReadBoolean("1");
        }
        public bool? ReadBoolean(string indicator) {
            string value = ReadString(1);
            if (value == null) {
                return null;
            }
            return value.Equals(indicator);
        }

        //public TResult ReadByteConstant<TResult>() where TResult : System.Enum
        //{
        //    string value = ReadString(1);
        //    if (value != null)
        //    {
        //        return (TResult)Enum.Parse(typeof(TResult),(Encoding.UTF8.GetBytes(ReadString(1))[0]).ToString());
        //    }
        //    return default(TResult);
        //}

        public byte ReadByte() {
            byte[] bytes = ReadBytes(1);
            if (bytes.Length > 0) {
                return bytes[0];
            }
            return 0;
        }
        public byte[] ReadBytes(int length) {
            string rvalue = ReadString(length);
            if (rvalue != null) {
                return Encoding.UTF8.GetBytes(rvalue);
            }
            return new byte[0];
        }

        public int ReadInt(int length) {
            string value = ReadString(length);
            if (value != null) {
                return int.Parse(value);
            }
            return 0;
        }

        public string ReadLVAR() {
            return ReadVar(1);
        }
        public string ReadLLVAR() {
            return ReadVar(2);
        }
        public string ReadLLLVAR() {
            return ReadVar(3);
        }

        public string ReadRemaining() {
            if (position < buffer.Length) {
                string rvalue = buffer.Substring(position);
                position = buffer.Length;
                return rvalue;
            }
            return "";
        }
        public byte[] ReadRemainingBytes() {
            string rvalue = ReadRemaining();
            if (rvalue != null) {
                return Encoding.ASCII.GetBytes(rvalue);
            }
            return new byte[0];
        }

        public string ReadString(int length) {
            int index = position + length;
            if (index > buffer.Length) {
                return null;
            }

            string rvalue = buffer.Substring(position, length);
            position += length;
            return rvalue;
        }
        ///
        //public TResult ReadStringConstant<TResult>(int length) where TResult : System.Enum
        //{
        //    string value = ReadString(length);
        //    return EnumConverter.FromMapping<TResult>(Target.NWS, ReadString(length));
        //}

        public string ReadToChar(char c) {
            return ReadToChar(c, true);
        }
        public string ReadToChar(char c, bool remove) {
            int index = buffer.IndexOf(c, position);
            if (index < 0) {
                return ReadRemaining();
            }

            string rvalue = buffer.Substring(position, index);
            position = index;
            if (remove) {
                position++;
            }

            return string.IsNullOrEmpty(rvalue) ? "" : rvalue;
        }

        private string ReadVar(int length) {
            int actual = ReadInt(length);
            if (actual != 0) {
                return ReadString(actual);
            }
            return null;
        }
    }
}
