using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace GlobalPayments.Api.Terminals.Extensions {
    public static class NetworkStreamExtensions {
        public static int GetLength(this NetworkStream stream) {
            byte[] length_buffer = new byte[2];
            int byteCount = stream.Read(length_buffer, 0, 2);

            if (byteCount != 2)
                return 0;
            return BitConverterX.ToInt16(length_buffer, 0);
        }

        public static async Task<int> GetLengthAsync(this NetworkStream stream) {
            byte[] length_buffer = new byte[2];
            int byteCount = await stream.ReadAsync(length_buffer, 0, 2);

            if (byteCount != 2)
                return 0;
            return BitConverterX.ToInt16(length_buffer, 0);
        }
    }
}
