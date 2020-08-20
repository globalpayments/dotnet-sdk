using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.Portico)]
    public enum StoredCredentialInitiator {
        [Map(Target.Portico, "C")]
        CardHolder,
        [Map(Target.Portico, "M")]
        Merchant,        
        Scheduled
    }
}
