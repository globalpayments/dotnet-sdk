using System;

namespace GlobalPayments.Api.Terminals.Abstractions {
    interface IDeviceMessage {
        byte[] GetSendBuffer();
    }
}
