using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.Enums {
    public enum OperationType {
        [Map(Target.NWS, "1")]
        Reserved,
        [Map(Target.NWS, "2")]
        Decrypt
    }
}
