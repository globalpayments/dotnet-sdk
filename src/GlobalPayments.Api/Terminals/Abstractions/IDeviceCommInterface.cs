using GlobalPayments.Api.Terminals.Messaging;

namespace GlobalPayments.Api.Terminals.Abstractions {
    interface IDeviceCommInterface {
        void Connect();
        void Disconnect();
        byte[] Send(IDeviceMessage message);
        event MessageSentEventHandler OnMessageSent;
    }
}
