using GlobalPayments.Api.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Utils {
    public class MessageWriter {
        List<byte> buffer;
        public MessageWriter() {
            buffer = new List<byte>();
        }
        public MessageWriter(byte[] bytes) {
            buffer = new List<byte>();
            foreach (byte b in bytes)
                buffer.Add(b);
        }
        public void Add(byte b) { buffer.Add(b); }
        public void AddRange(byte[] bytes) {
            foreach (byte b in bytes)
                buffer.Add(b);
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
    }
}
