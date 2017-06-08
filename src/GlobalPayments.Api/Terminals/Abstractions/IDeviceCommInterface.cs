using System;
using GlobalPayments.Api.Terminals.PAX;

namespace GlobalPayments.Api.Terminals.Abstractions {
    interface IDeviceCommInterface {
        void Connect();
        void Disconnect();
        byte[] Send(IDeviceMessage message);
        event MessageSentEventHandler OnMessageSent;
    }
}
