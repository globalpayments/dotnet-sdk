
namespace GlobalPayments.Api.Gateways.Events {
    public enum GatewayEventType {
        Connection,
        Disconnected,
        RequestSent,
        ResponseReceived,
        Timeout,
        TimeoutFailOver
    }
}
