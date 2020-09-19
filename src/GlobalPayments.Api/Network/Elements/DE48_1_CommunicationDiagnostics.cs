using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE48_1_CommunicationDiagnostics : IDataElement<DE48_1_CommunicationDiagnostics> {
        public int CommunicationAttempts { get; set; }
        public DE48_ConnectionResult ConnectionResult { get; set; }
        public DE48_HostConnected HostConnected { get; set; }

        public DE48_1_CommunicationDiagnostics FromByteArray(byte[] buffer) {
        StringParser sp = new StringParser(buffer);
        CommunicationAttempts = sp.ReadInt(1);
        //ConnectionResult = sp.ReadStringConstant<DE48_ConnectionResult>(2);
        //HostConnected = sp.ReadStringConstant<DE48_HostConnected>(1);
        ConnectionResult = EnumConverter.FromMapping<DE48_ConnectionResult>(Target.NWS, sp.ReadString(2));
        HostConnected = EnumConverter.FromMapping<DE48_HostConnected>(Target.NWS, sp.ReadString(1));
        return this;
        }

        public byte[] ToByteArray() {
        string rvalue = CommunicationAttempts + "";
        return Encoding.ASCII.GetBytes(string.Concat(rvalue, EnumConverter.GetMapping(Target.NWS, ConnectionResult),
        EnumConverter.GetMapping(Target.NWS, HostConnected)));
        }
    }
}
