using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Gateways {
    public interface ISecure3dProvider {
        Secure3dVersion Version { get; }

        Transaction ProcessSecure3d(Secure3dBuilder builder);        
    }
}
