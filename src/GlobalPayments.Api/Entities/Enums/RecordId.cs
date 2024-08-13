using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.Enums {
    public enum RecordId {
        [Map(Target.GP_API, "E3")]
        E3_Encryption,
        [Map(Target.NWS, "3D")]
        [Map(Target.GP_API, "3D")]
        Encryption_3DE,
        [Map(Target.NWS, "TD")]
        [Map(Target.GP_API, "TD")]
        Tokenization_TD
    }
}
