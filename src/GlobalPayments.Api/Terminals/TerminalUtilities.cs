﻿using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.HPA;
using GlobalPayments.Api.Terminals.UPA;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GlobalPayments.Api.Terminals
{
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

        public static DeviceMessage BuildRequest(string messageId, params object[] elements) {
            var message = GetElementString(elements);
            return BuildMessage(messageId, message);
        }

        public static DeviceMessage BuildUpaAdminRequest(int requestId, string ecrId, string txnType, string lineItemLeft = null, string lineItemRight = null, int? displayOption = null) {
            JsonDoc doc, baseRequest;
            BuildGeneralUpaRequest(requestId, ecrId, txnType, out doc, out baseRequest);

            if ((!string.IsNullOrEmpty(lineItemLeft) || !string.IsNullOrEmpty(lineItemRight)) || displayOption.HasValue) {
                var request = baseRequest.SubElement("data");
                var requestParams = request.SubElement("params");
                requestParams.Set("lineItemLeft", lineItemLeft);
                requestParams.Set("lineItemRight", lineItemRight);
                if (displayOption.HasValue) {
                    requestParams.Set("displayOption", displayOption);
                }
            }

            return BuildUpaRequest(doc.ToString());
        }

        public static void BuildGeneralUpaRequest(int requestId, string ecrId, string txnType, out JsonDoc doc, out JsonDoc baseRequest) {
            doc = new JsonDoc();
            doc.Set("message", UpaMessageType.Msg);
            baseRequest = doc.SubElement("data");
            baseRequest.Set("command", txnType);
            baseRequest.Set("EcrId", ecrId);
            baseRequest.Set("requestId", requestId.ToString());
        }

        public static byte[] BuildRawUpaRequest(string jsonRequest) {

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
            if (!string.IsNullOrEmpty(jsonRequest)) {
                foreach (char c in jsonRequest)
                    buffer.Add((byte)c);
            }

            // End the Message
            buffer.Add((byte)ControlCodes.LF);
            buffer.Add((byte)ControlCodes.ETX);
            buffer.Add((byte)ControlCodes.LF);
            return buffer.ToArray();
        }
        public static DeviceMessage<T> BuildUpaRequest<T>(T doc) where T : IRawRequestBuilder {
            byte[] buffer;
            if (doc is JsonDoc) {
                buffer = BuildRawUpaRequest((doc as JsonDoc).ToString());
            }
            else buffer = BuildRawUpaRequest((doc as ElementTree).ToString());

            return new DeviceMessage<T>(doc, buffer);
        }
        public static DeviceMessage BuildUpaRequest(string jsonRequest) {
            var json = JsonDoc.Parse(jsonRequest);
            byte[] buffer = BuildRawUpaRequest(jsonRequest);
            return new DeviceMessage<JsonDoc>(json, buffer);
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
       
        internal static string BuildToBase64Content(string filePath, string transType, bool imageValidation = false) {   
            if(string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) {
                throw new BuilderException($"The file {filePath} does not exist.");
            }
            
            var arr = File.ReadAllBytes(filePath);
            if (imageValidation) {
                ImageValidation(arr, transType);
            }

            return Convert.ToBase64String(arr);
        }

        private static void ImageValidation(byte[] arr, string transType) {
            const int baseBit = 1024;
            const int width = 370;
            const int height = 13930;
            const int bytes = 3145728;
            switch (transType) {
                case UpaTransType.PrintData:
                    if (new byte[arr.Length].Length > bytes) {
                        throw new BuilderException($"The image must not exceed {(Math.Round((((decimal)bytes) / baseBit)) / baseBit)}MB.");
                    }
                    break;
                case UpaTransType.InjectUDDataFile:
                    if (new byte[arr.Length].Length > (bytes * 3)) {
                        throw new BuilderException($"The image must not exceed {(Math.Round((((decimal)bytes * 3) / baseBit)) / baseBit)}MB.");
                    }
                    break;
                default:
                    break;
            }

            
        }

        internal static string BuildStringFromFile(string filePath) {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) {
                throw new BuilderException($"The file {filePath} does not exist.");
            }

            var file = File.ReadAllText(filePath);
            return file;
        }
        

        public static bool IsGpApiResponse(JsonDoc root)
        {
            return !root.Has("data");
        }
    }
}
