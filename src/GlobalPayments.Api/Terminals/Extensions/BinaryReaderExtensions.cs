using System;
using System.IO;
using System.Linq;

namespace GlobalPayments.Api.Terminals.Extensions {
    internal static class BinaryReaderExtensions {
        public static int GetInt32(this BinaryReader br, int length) {
            string value = string.Empty;
            br.ReadChars(length).ToList().ForEach(x => value += x);
            return Int32.Parse(value);
        }

        public static string GetString(this BinaryReader br, int length) {
            string value = string.Empty;
            br.ReadChars(length).ToList().ForEach(x => {
                if (x == (byte)0x00)
                    value += ' ';
                else value += x;
            });
            return value;
        }

        public static string ReadToCode(this BinaryReader br, ControlCodes code, bool removeCode = true) {
            string value = string.Empty;

            try {
                while (br.PeekChar() != (int)code && br.PeekChar() != (int)ControlCodes.ETX)
                    value += br.ReadChar();
            }
            catch (EndOfStreamException) { removeCode = false; }

            // Remove the code so the next read is ready
            if (removeCode)
                br.ReadByte();

            return value;
        }
    }
}
