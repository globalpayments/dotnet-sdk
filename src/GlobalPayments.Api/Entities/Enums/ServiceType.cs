using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.Enums {
    public enum ServiceType {
        [Map(Target.NWS, "G")]
        GPN_API,
    }
}
