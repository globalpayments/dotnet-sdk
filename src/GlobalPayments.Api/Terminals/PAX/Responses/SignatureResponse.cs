using System.Collections.Generic;
using System;
using System.IO;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Extensions;
using System.Linq.Expressions;

namespace GlobalPayments.Api.Terminals.PAX {
    public class SignatureResponse : PaxTerminalResponse, ISignatureResponse {
        private DeviceType _deviceType;

        public int TotalLength { get; set; }
        public int ResponseLegth { get; set; }
        public string SigData { get; set; }
        public SignatureResponse(byte[] response, DeviceType deviceType = DeviceType.PAX_S300) : base(response, PAX_MSG_ID.A09_RSP_GET_SIGNATURE, PAX_MSG_ID.A21_RSP_DO_SIGNATURE) {
            _deviceType = deviceType;
        }
        protected override void ParseResponse(BinaryReader br)
        {
            base.ParseResponse(br);
            var b = true;
            if(base.Command == "A09" && this.DeviceResponseCode != "100003")
            {
                TotalLength = int.Parse(br.ReadToCode(ControlCodes.FS));
                ResponseLegth = int.Parse(br.ReadToCode(ControlCodes.FS));
                SigData = br.ReadToCode(ControlCodes.ETX);
            }

        }
        private byte[] GetBytes(string signatureData)
        {
            signatureData = signatureData.Replace("[COMMA]", ",").Replace("^", ",");
            string[] byteValues = signatureData.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<byte> byteList = new List<byte>();
            foreach (string value in byteValues)
            {
                if (int.Parse(value) <= 255)
                {
                    byteList.Add(Convert.ToByte(value));
                }
                else { break; }

            }
            byte[] byteArray = byteList.ToArray();
            return byteArray;
        }
    }
}
