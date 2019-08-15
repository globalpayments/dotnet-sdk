using System;

namespace GlobalPayments.Api.Terminals.Abstractions {
    interface IDeviceMessage {
        bool KeepAlive { get; set; }
        bool AwaitResponse { get; set; }

        byte[] GetSendBuffer();        
    }
}
