using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum UnitOfMeasure {
        [Map(Target.NWS, "C")]
        CaseOrCarton,
        [Map(Target.NWS, "G")]
        Gallons,
        [Map(Target.NWS, "K")]
        Kilograms,
        [Map(Target.NWS, "L")]
        Liters,
        [Map(Target.NWS, "O")]
        OtherOrUnknown,
        [Map(Target.NWS, "P")]
        Pounds,
        [Map(Target.NWS, "Q")]
        Quarts,
        [Map(Target.NWS, "U")]
        Units,
        [Map(Target.NWS, "Z")]
        Ounces
    }
}
