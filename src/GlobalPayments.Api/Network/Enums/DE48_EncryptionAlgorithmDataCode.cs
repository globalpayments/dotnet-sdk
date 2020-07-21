using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE48_EncryptionAlgorithmDataCode {
        [Map(Target.NWS, "0")]
        NoEncryption,
        [Map(Target.NWS, "1")]
        DES,
        [Map(Target.NWS, "2")]
        TripleDES_2Keys,
        [Map(Target.NWS, "3")]
        TripleDES_3Keys

    }
}
