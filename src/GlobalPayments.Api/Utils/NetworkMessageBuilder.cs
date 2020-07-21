using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Utils {
    public class NetworkMessageBuilder {
        private List<byte> buffer;
        public NetworkMessageBuilder() {
            buffer = new List<byte>();
        }
        public NetworkMessageBuilder(byte[] bytes) {
            buffer = new List<byte>();
            foreach (byte b in bytes)
                buffer.Add(b);
        }
        public NetworkMessageBuilder Append(byte b) {
            buffer.Add(b);
            return this;
        }
        public NetworkMessageBuilder Append(int value) {
            return Append(value, 1);
        }
        public NetworkMessageBuilder Append(int value, int length) {
            byte[] bytes = FormatInteger(value, length);
            return Append(bytes);
        }
        public NetworkMessageBuilder Append(long value) {
            return Append(value, 1);
        }
        public NetworkMessageBuilder Append(long value, int length) {
            byte[] bytes = FormatInteger(value, length);
            return Append(bytes);
        }        
        public NetworkMessageBuilder Append(string value) {
            return Append(Encoding.UTF8.GetBytes(value));
        }
        public NetworkMessageBuilder Append(byte[] bytes) {
            foreach (byte b in bytes)
                buffer.Add(b);
            return this;
        }
        public void Pop() {
            buffer.RemoveAt(buffer.Count - 1);
        }
        public byte[] ToArray() {
            byte[] b = new byte[buffer.Count];

            byte[] b2 = buffer.ToArray();
            for (int i = 0; i < buffer.Count; i++)
                b[i] = (byte)b2[i];

            return b;
        }
        private byte[] FormatInteger(long value, int length) {
            int[] offsets = { 0, 8, 16, 32, 64, 128, 256, 512, 1024, 2048 };

            if (length == 1) {
                return new byte[] { (byte)(value & 0xFF) };
            }
            else {
                int byteCount = Math.Abs(CountSetBits(value) / 8) + 1;
                int baseLength = byteCount * 2;
                if (baseLength > length) { baseLength = length; }

                MessageWriter buffer = new MessageWriter();
                for (int i = 0; i < baseLength; i++) {
                    int offset = offsets[baseLength - 1 - i];
                    buffer.Add((byte)((uint)value >> offset));
                }
                byte[] input = buffer.ToArray();

                byte[] output = new byte[length];
                Array.Copy(input, 0, output, length - baseLength, baseLength);

                return output;
            }
        }

        public new string ToString() {
            char[] HEX_CHARS = "0123456789abcdef".ToCharArray();
            char[] chars = new char[2 * buffer.Count];
            for (int i = 0; i < buffer.Count; ++i) {
                chars[2 * i] = HEX_CHARS[(uint)(buffer[i] & 0xF0) >> 4];
                chars[2 * i + 1] = HEX_CHARS[buffer[i] & 0x0F];
            }
            return new string(chars);
        }

        public int Length() {
            return buffer.Count;
        }
        public int CountSetBits(long n) {
            if (n == 0)
                return 0;
            else
                return Convert.ToInt32((n & 1) + CountSetBits(n >> 1));
        }
    }

}