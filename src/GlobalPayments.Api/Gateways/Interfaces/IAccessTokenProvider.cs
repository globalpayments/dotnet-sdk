using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.GpApi;

namespace GlobalPayments.Api.Gateways {
    public interface IAccessTokenProvider {
        Request SignIn(string appId, string appKey, int? secondsToExpire = null, IntervalToExpire? intervalToExpire = null, string[] permissions = null, PorticoTokenConfig porticoTokenConfig = null);
        Request SignOut();
    }
}
