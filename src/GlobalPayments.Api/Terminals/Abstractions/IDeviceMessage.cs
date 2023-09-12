using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.Abstractions {
    public interface IDeviceMessage {
        bool KeepAlive { get; set; }
        bool AwaitResponse { get; set; }

        byte[] GetSendBuffer();

        IRawRequestBuilder GetRequestBuilder();
        T GetRequestField<T>(string keyName);
    }
}
