using System;
using System.IO;
using GlobalPayments.Api.Terminals.Extensions;
using GlobalPayments.Api.Terminals.Abstractions;

namespace GlobalPayments.Api.Terminals.PAX {
    internal class HostResponse : IResponseSubGroup {
        public string HostResponseCode { get; private set; }
        public string HostResponseMessage { get; private set; }
        public string AuthCode { get; private set; }
        public string HostRefereceNumber { get; private set; }
        public string TraceNumber { get; private set; }
        public string BatchNumber { get; private set; }

        public HostResponse(BinaryReader br) {
            var values = br.ReadToCode(ControlCodes.FS);
            if (string.IsNullOrEmpty(values))
                return;

            var data = values.Split((char)ControlCodes.US);
            try {
                this.HostResponseCode = data[0];
                this.HostResponseMessage = data[1];
                this.AuthCode = data[2];
                this.HostRefereceNumber = data[3];
                this.TraceNumber = data[4];
                this.BatchNumber = data[5];
            }
            catch (IndexOutOfRangeException) {
            }
        }
    }
}
