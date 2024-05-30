using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Gateways {
    public interface IAccessTokenProvider {
        Request SignIn(string appId, string appKey, int? secondsToExpire = null, IntervalToExpire? intervalToExpire = null, string[] permissions = null);
        Request SignOut();
    }
}
