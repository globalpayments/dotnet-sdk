using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.Portico)]
    public enum StoredCredentialInitiator {
        [Map(Target.Portico, "C")]
        [Map(Target.GP_API, "PAYER")]
        CardHolder,
        [Map(Target.Portico, "M")]
        [Map(Target.GP_API, "MERCHANT")]
        Merchant,        
        Scheduled
    }
}
