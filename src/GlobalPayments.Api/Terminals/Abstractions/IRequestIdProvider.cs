using GlobalPayments.Api.Terminals.Builders;

namespace GlobalPayments.Api.Terminals.Abstractions {
    public interface IRequestIdProvider {
        int GetRequestId();
    }
}
