using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum AuthorizerCode {
        [Map(Target.NWS, " ")]
        Interchange_Authorized,
        [Map(Target.NWS, "B")]
        Host_Authorized,
        [Map(Target.NWS, "T")]
        Terminal_Authorized,
        [Map(Target.NWS, "V")]
        Voice_Authorized,
        [Map(Target.NWS, "P")]
        PassThrough,
        [Map(Target.NWS, "N")]
        NegativeFile,
        [Map(Target.NWS, "L")]
        LocalNegativeFile,
        [Map(Target.NWS, "A")]
        AuthTable
    }
}
