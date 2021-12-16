using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals;
using System.Globalization;
using System.Security.Cryptography;
using System.Linq;

namespace GlobalPayments.Api.Utils {
    public static class Extensions {
        public static string FormatWith(this string pattern, params object[] values) {
            return string.Format(pattern, values);
        }

        public static string ToNumeric(this string str) {
            return Regex.Replace(str, "[^0-9]", "");
        }

        public static string ToNumericCurrencyString(this decimal dec) {
            return Regex.Replace(string.Format("{0:c}", dec), "[^0-9]", "");
        }

        public static string ToNumericCurrencyString(this decimal? dec) {
            return dec?.ToNumericCurrencyString();
        }

        public static string ToCurrencyString(this decimal? dec) {
            if (dec != null) {
                return Regex.Replace(string.Format("{0:c}", dec), "[^0-9.,]", "");
            }
            return null;
        }

        public static decimal? ToAmount(this string str) {
            if (string.IsNullOrEmpty(str))
                return null;

            decimal amount = 0m;
            if (decimal.TryParse(str, out amount)) {
                return amount / 100;
            }
            return null;
        }

        public static decimal? ToDecimal(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            decimal amount = 0m;
            if (decimal.TryParse(str, out amount))
            {
                return amount;
            }
            return null;
        }

        public static string ToInitialCase(this Enum value) {
            var initial = value.ToString();
            return initial.Substring(0, 1).ToUpper() + initial.Substring(1).ToLower();
        }

        public static byte[] GetTerminalResponse(this NetworkStream stream) {
            var buffer = new byte[4096];
            int bytesReceived = stream.ReadAsync(buffer, 0, buffer.Length).Result;
            if (bytesReceived <= 0) {
                bytesReceived = stream.ReadAsync(buffer, 0, buffer.Length).Result;
            }

            if (bytesReceived > 0) {
                byte[] readBuffer = new byte[bytesReceived];
                Array.Copy(buffer, readBuffer, bytesReceived);

                var code = (ControlCodes)readBuffer[0];
                if (code == ControlCodes.NAK)
                    return null;
                else if (code == ControlCodes.EOT)
                    throw new MessageException("Terminal returned EOT for the current message.");
                else if (code == ControlCodes.ACK) {
                    return stream.GetTerminalResponse();
                }
                else if (code == ControlCodes.STX) {
                    var queue = new Queue<byte>(readBuffer);

                    // break off only one message
                    var rec_buffer = new List<byte>();
                    do {
                        rec_buffer.Add(queue.Dequeue());
                        if (rec_buffer[rec_buffer.Count - 1] == (byte)ControlCodes.ETX)
                            break;
                    }
                    while (queue.Count > 0);

                    // Should be the LRC
                    if (queue.Count > 0) {
                        rec_buffer.Add(queue.Dequeue());
                    }
                    return rec_buffer.ToArray();
                }
                else throw new MessageException(string.Format("Unknown message received: {0}", code));
            }
            return null;
        }

        public static byte[] GetTerminalResponseAsync(this NetworkStream stream) {
            var buffer = new byte[4096];
            int bytesReceived = stream.ReadAsync(buffer, 0, buffer.Length).Result;
            
            if (bytesReceived > 0) {
                byte[] readBuffer = new byte[bytesReceived];
                Array.Copy(buffer, readBuffer, bytesReceived);

                var code = (ControlCodes)readBuffer[0];
                if (code == ControlCodes.NAK) {
                    return null;
                }
                else if (code == ControlCodes.EOT) {
                    throw new MessageException("Terminal returned EOT for the current message.");
                }
                else if (code == ControlCodes.ACK) {
                    return stream.GetTerminalResponse();
                }
                else if (code == ControlCodes.STX) {
                    var queue = new Queue<byte>(readBuffer);

                    // break off only one message
                    var rec_buffer = new List<byte>();
                    do {
                        rec_buffer.Add(queue.Dequeue());
                        if (rec_buffer[rec_buffer.Count - 1] == (byte)ControlCodes.ETX)
                            break;
                    }
                    while (queue.Count > 0);

                    // Should be the LRC
                    if (queue.Count > 0) {
                        rec_buffer.Add(queue.Dequeue());
                    }
                    return rec_buffer.ToArray();
                }
                else throw new MessageException(string.Format("Unknown message received: {0}", code));
            }
            return null;
        }

        public static int? ToInt32(this string str) {
            if (string.IsNullOrEmpty(str))
                return null;

            int rvalue = default(int);
            if (Int32.TryParse(str, out rvalue))
                return rvalue;
            return null;
        }

        public static DateTime? ToDateTime(this string str) {
            if (string.IsNullOrEmpty(str))
                return null;

            DateTime rvalue;
            if (DateTime.TryParseExact(str, "yyyyMMddhhmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out rvalue))
                return rvalue;
            return null;
        }

        public static DateTime? ToDateTime(this string str, string format)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            DateTime rvalue;
            if (DateTime.TryParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out rvalue))
                return rvalue;
            return null;
        }

        public static byte[] GetKey(this Rfc2898DeriveBytes bytes) {
            return bytes.GetBytes(32);
        }

        public static byte[] GetVector(this Rfc2898DeriveBytes bytes) {
            return bytes.GetBytes(16);
        }

        public static T GetValue<T>(this Dictionary<string, string> dict, string key) {
            try {
                return (T)Convert.ChangeType(dict[key], typeof(T));
            }
            catch (KeyNotFoundException) {
                return default(T);
            }
        }
        public static decimal? GetAmount(this Dictionary<string, string> dict, string key) {
            try {
                return dict[key].ToAmount();
            }
            catch (KeyNotFoundException) {
                return null;
            }
        }

        public static bool? GetBoolean(this Dictionary<string, string> dict, string key) {
            try {
                return bool.TryParse(dict[key], out bool result);
            }
            catch (KeyNotFoundException) {
                return null;
            }
        }

        public static IEnumerable<string> SplitInMaxDataSize(this string str, int maxDataSize) {
            if (string.IsNullOrEmpty(str)) {
                yield return string.Empty;
            }

            for (var i = 0; i < str.Length; i += maxDataSize) {
                yield return str.Substring(i, Math.Min(maxDataSize, str.Length - i));
            }
        }

        public static string TrimEnd(this string str, string trimString) {
            string rvalue = str;
            if (rvalue.EndsWith(trimString)) {
                int trimLength = trimString.Length;
                rvalue = rvalue.Substring(0, rvalue.Length - trimLength);
            }
            return rvalue;
        }

        public static string ExtractDigits(this string str) {
            return string.IsNullOrEmpty(str) ? str : new string(str.Where(char.IsDigit).ToArray());
        }
    }
}
