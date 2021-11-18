using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.HPA;
using System.IO;
using System.Drawing.Imaging;
using GlobalPayments.Api.Terminals.UPA;
using GlobalPayments.Api.Utils;

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

        public static DeviceMessage BuildUpaAdminRequest(int requestId, string ecrId, string txnType, string lineItemLeft = null, string lineItemRight = null, int? displayOption = null)
        {
            var doc = new JsonDoc();
            doc.Set("message", UpaMessageType.Msg);
            var data = doc.SubElement("data");
            data.Set("command", txnType);
            data.Set("EcrId", ecrId);
            data.Set("requestId", requestId);

            if ((!string.IsNullOrEmpty(lineItemLeft) || !string.IsNullOrEmpty(lineItemRight)) || displayOption.HasValue) {
                var request = data.SubElement("data");
                var requestParams = request.SubElement("params");
                requestParams.Set("lineItemLeft", lineItemLeft);
                requestParams.Set("lineItemRight", lineItemRight);
                if (displayOption.HasValue) {
                    requestParams.Set("displayOption", displayOption);
                }
            }

            return BuildUpaRequest(doc.ToString());
        }

        public static DeviceMessage BuildRequest(string messageId, params object[] elements) {
            var message = GetElementString(elements);
            return BuildMessage(messageId, message);
        }

        public static DeviceMessage BuildUpaRequest(string jsonRequest)
        {
            jsonRequest = jsonRequest.Replace("ecrId", "EcrId");

            jsonRequest = jsonRequest.Replace("<LF>", "\r\n");
            jsonRequest = jsonRequest.Replace("{", "{\r\n");
            jsonRequest = jsonRequest.Replace("}", "}\r\n");
            jsonRequest = jsonRequest.Replace(",", ",\r\n");
            var buffer = new List<byte>();

            // Begin Message
            buffer.Add((byte)ControlCodes.STX);
            buffer.Add(0x0A);

            // Add the Message
            if (!string.IsNullOrEmpty(jsonRequest))
            {
                foreach (char c in jsonRequest)
                    buffer.Add((byte)c);
            }

            // End the Message
            buffer.Add((byte)ControlCodes.LF);
            buffer.Add((byte)ControlCodes.ETX);
            buffer.Add((byte)ControlCodes.LF);
            return new DeviceMessage(buffer.ToArray());
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
    }
}
