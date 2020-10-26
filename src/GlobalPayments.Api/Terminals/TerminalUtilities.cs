using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.HPA;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Terminals {
    public class TerminalUtilities {
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

        public static DeviceMessage BuildRequest(string message, ConnectionModes settings) {
            var buffer = new List<byte>();
            byte[] lrc;

            switch (settings) {
                case ConnectionModes.SERIAL:
                case ConnectionModes.PAY_AT_TABLE:
                    buffer.Add((byte)ControlCodes.STX);
                    foreach (char c in message)
                        buffer.Add((byte)c);
                    buffer.Add((byte)ControlCodes.ETX);
                    lrc = CalculateLRC(message);
                    buffer.Add(lrc[0]);
                    break;
                case ConnectionModes.TCP_IP_SERVER:
                    var _msg = CalculateHeader(Encoding.UTF8.GetBytes(message)) + message;

                    foreach (char c in _msg)
                        buffer.Add((byte)c);

                    break;
                default:
                    throw new BuilderException("Failed to build request message. Unknown Connection mode.");
            }

            return new DeviceMessage(buffer.ToArray());
        }

        public static DeviceMessage BuildRequest(string message, MessageFormat format) {
            var buffer = new List<byte>();

            if (format == MessageFormat.Visa2nd)
                buffer.Add((byte)ControlCodes.STX);
            else {
                var length_bytes = BitConverter.GetBytes(message.Length);
                buffer.Add(length_bytes[1]);
                buffer.Add(length_bytes[0]);
            }

            foreach (char c in message)
                buffer.Add((byte)c);

            if (format == MessageFormat.Visa2nd) {
                // End the Message
                buffer.Add((byte)ControlCodes.ETX);

                byte lrc = CalculateLRC(buffer.ToArray());
                buffer.Add(lrc);
            }

            return new DeviceMessage(buffer.ToArray());
        }

        public static DeviceMessage BuildRequest(string messageId, params object[] elements) {
            var message = GetElementString(elements);
            return BuildMessage(messageId, message);
        }

        public static byte CalculateLRC(byte[] buffer) {
            // account for LRC still being attached
            var length = buffer.Length;
            if (buffer[buffer.Length - 1] != (byte)ControlCodes.ETX)
                length--;

            byte lrc = new byte();
            for (int i = 1; i < length; i++)
                lrc = (byte)(lrc ^ buffer[i]);
            return lrc;
        }

        public static byte[] BuildSignatureImage(string pathData, int width = 150) {
            Func<string, Point> toPoint = (coord) => {
                var xy = coord.Split(',');
                return new Point {
                    X = int.Parse(xy[0]),
                    Y = int.Parse(xy[1])
                };
            };

            // parse instructions
            var coordinates = pathData.Split('^');

            Bitmap bmp = new Bitmap(width, 100);

            var gfx = Graphics.FromImage(bmp);
            gfx.Clear(Color.White);

            var index = 0;
            var coordinate = coordinates[index++];
            do {
                if(coordinate == "0,65535")
                    coordinate = coordinates[index++];
                var start = toPoint(coordinate);

                coordinate = coordinates[index++];
                if (coordinate == "0,65535")
                    gfx.FillRectangle(Brushes.Black, start.X, start.Y, 1, 1);
                else {
                    var end = toPoint(coordinate);
                    gfx.DrawLine(Pens.Black, start, end);
                }
            }
            while (coordinates[index] != "~");

            using (var ms = new MemoryStream()) {
                bmp.Save(ms, ImageFormat.Bmp);
                return ms.ToArray();
            }
        }

        public static string CalculateHeader(byte[] buffer) {
            //The Header contains the data length in hexadecimal format on two digits
            var hex = buffer.Length.ToString("X4");
            hex = hex.PadLeft(4, '0');

            // Get total value per two char.
            var fDigit = hex[0].ToString() + hex[1];
            var sDigit = hex[2].ToString() + hex[3];

            return string.Format("{0}{1}", Convert.ToChar(Convert.ToUInt32(fDigit, 16)),
               Convert.ToChar(Convert.ToUInt32(sDigit, 16)));
        }

        public static int HeaderLength(byte[] buffer) {
            // Conversion from decimal to hex value
            var fHex = Convert.ToInt64(buffer[0]).ToString("X2");
            var sHex = Convert.ToInt64(buffer[1]).ToString("X2");

            // Concat two hex value
            var _hex = fHex + sHex;

            // Get decimal value of concatenated hex
            return int.Parse(_hex, System.Globalization.NumberStyles.HexNumber);
        }

        public static byte[] CalculateLRC(string requestMessage) {
            byte[] bytes = Encoding.ASCII.GetBytes((requestMessage + (char)ControlCodes.ETX));
            byte lrc = 0;
            for (int i = 0; i < bytes.Length; i++) {
                lrc ^= bytes[i];
            }
            bytes = new byte[] { lrc };
            return bytes;
        }

        public static string GetTextContent(string filePath) {
            try {
                if (!filePath.Contains(".xml")) {
                    throw new BuilderException("File must be in XML Document");
                }

                return File.ReadAllText(filePath);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}
