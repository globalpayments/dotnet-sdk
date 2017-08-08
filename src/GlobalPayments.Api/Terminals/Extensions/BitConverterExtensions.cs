using System;

namespace System {
    public static partial class BitConverterX {
        public static short ToInt16(byte[] buffer, int startIndex) {
            byte[] lengthBuffer = new byte[2];
            Array.Copy(buffer, 0, lengthBuffer, 0, 2);
            Array.Reverse(lengthBuffer);
            return BitConverter.ToInt16(lengthBuffer, 0);
        }
    }
}
