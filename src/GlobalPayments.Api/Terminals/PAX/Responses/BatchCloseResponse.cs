﻿using System.IO;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Extensions;

namespace GlobalPayments.Api.Terminals.PAX {
    public class BatchCloseResponse : PaxTerminalResponse, IBatchCloseResponse {
        private HostResponse hostResponse;

        public string TotalCount { get; set; }
        public string TotalAmount { get; set; }
        public string TimeStamp { get; set; }
        public string TID { get; set; }
        public string MID { get; set; }
        public string SequenceNumber { get; set; }

        public BatchCloseResponse(byte[] buffer)
            : base(buffer, PAX_MSG_ID.B01_RSP_BATCH_CLOSE) {
        }

        protected override void ParseResponse(BinaryReader br) {
            base.ParseResponse(br);

            this.hostResponse = new HostResponse(br);
            this.TotalCount = br.ReadToCode(ControlCodes.FS);
            this.TotalAmount = br.ReadToCode(ControlCodes.FS);
            this.TimeStamp = br.ReadToCode(ControlCodes.FS);
            this.TID = br.ReadToCode(ControlCodes.FS);
            this.MID = br.ReadToCode(ControlCodes.ETX);
        }
    }
}
