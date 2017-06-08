using System;
using System.Collections.Generic;
using System.Text;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.PAX;

namespace GlobalPayments.Api.Terminals {
    internal class TerminalUtilities {
        const string _version = "1.35";

        private static string GetElementString(object[] elements) {
            var sb = new StringBuilder();
            foreach (var element in elements) {
                if (element is ControlCodes)
                    sb.Append((char)((ControlCodes)element));
                else if (element is IRequestSubGroup)
                    sb.Append(((IRequestSubGroup)element).GetElementString());
                else if (element is string[])
                    foreach (var sub_element in element as string[]) {
                        sb.Append((char)ControlCodes.FS);
                        sb.Append(sub_element);
                    }
                else sb.Append(element);
            }

            return sb.ToString();
        }

        private static DeviceMessage BuildMessage(string messageId, string message) {
            var buffer = new List<byte>();

            // Begin Message
            buffer.Add((byte)ControlCodes.STX);
            
            // Add Message ID
            foreach (char c in messageId)
                buffer.Add((byte)c);
            buffer.Add((byte)ControlCodes.FS);

            // Add Version
            foreach (char c in _version)
                buffer.Add((byte)c);
            buffer.Add((byte)ControlCodes.FS);

            // Add the Message
            if (!string.IsNullOrEmpty(message)) {
                foreach (char c in message)
                    buffer.Add((byte)c);
            }

            // End the Message
            buffer.Add((byte)ControlCodes.ETX);

            byte lrc = CalculateLRC(buffer.ToArray());
            buffer.Add(lrc);

            return new DeviceMessage(buffer.ToArray());
        }

        public static DeviceMessage BuildRequest(string messageId, params object[] elements) {
            var message = GetElementString(elements);
            return BuildMessage(messageId, message);
        }

        public static byte CalculateLRC(byte[] buffer) {
            byte lrc = new byte();
            for (int i = 1; i < buffer.Length; i++)
                lrc = (byte)(lrc ^ buffer[i]);
            return lrc;
        }
    }
}
