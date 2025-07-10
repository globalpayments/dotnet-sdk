using System.Collections.Generic;
using System;
using System.IO;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Extensions;
using System.Linq.Expressions;

namespace GlobalPayments.Api.Terminals.PAX.Responses
{
    public class ShowTextBoxResponse : PaxTerminalResponse, IShowTextBoxResponse
    {
        private DeviceType _deviceType;
        public string ButtonNumberSelected { get; set; }
        public bool SignStatus { get; set; }
        public string SigData { get; set; }
        public string Text { get; set; }
        public ShowTextBoxResponse(byte[] buffer)
            : base(buffer, PAX_MSG_ID.A57_RSP_SHOW_TEXTBOX)
        {
        }
        protected override void ParseResponse(BinaryReader br)
        {
            base.ParseResponse(br);
            ButtonNumberSelected = br.ReadToCode(ControlCodes.FS);
            SignStatus = br.ReadToCode(ControlCodes.FS) == "1" ? true: false;
            SigData = br.ReadToCode(ControlCodes.FS);
            Text = br.ReadToCode(ControlCodes.ETX);
        }
    }
}