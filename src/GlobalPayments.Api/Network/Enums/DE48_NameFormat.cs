using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE48_NameFormat {
        [Map(Target.NWS, "0")]
        FreeFormat,
        [Map(Target.NWS, "1")]
        Delimited_FirstMiddleLast,
        [Map(Target.NWS, "2")]
        Delimited_Title

    }
}
