using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.Enums {
    public enum EncryptedFieldMatrix {
        [Map(Target.NWS, "1")]
        Track1,
        [Map(Target.NWS, "2")]
        Track2,
        [Map(Target.NWS, "3")]
        Pan,
        [Map(Target.NWS, "03")]
        CustomerData,
        [Map(Target.NWS, "04")]
        CustomerDataCSV
    }
}
