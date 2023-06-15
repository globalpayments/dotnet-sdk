using GlobalPayments.Api.Utils;
using System.ComponentModel;

namespace GlobalPayments.Api.Entities
{
    public enum SdkUiType {
        [Map(Target.GP_API, "TEXT")]
        TEXT,
        [Map(Target.GP_API, "SINGLE_SELECT")]
        SINGLE_SELECT,
        [Map(Target.GP_API, "MULTI_SELECT")]
        MULTI_SELECT,
        [Map(Target.GP_API, "OUT_OF_BAND")]
        OOB,
        [Map(Target.GP_API, "HTML_OTHER")]
        HTML_OTHER
    }

}
