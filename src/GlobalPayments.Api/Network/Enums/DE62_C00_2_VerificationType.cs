using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Network.Entities
{
    public enum DE62_C00_2_VerificationType
    {
        [Map(Target.NWS, "1")]
        RawMICRData,
        [Map(Target.NWS, "2")]
        FormattedMICRData,
        [Map(Target.NWS, "3")]
        DriversLicense,
        [Map(Target.NWS, "4")]
        FormattedMICR_DriversLicense,
        [Map(Target.NWS, "5")]
        FormattedMICR_AlternateID,
        [Map(Target.NWS, "6")]
        FormattedMICR_DriversLicense_AlternateID,
        [Map(Target.NWS, "7")]
        RawMICR_DriversLicense,
        [Map(Target.NWS, "8")]
        RawMICR_AlternateID,
        [Map(Target.NWS, "9")]
        RawMICR_DriversLicense_AlternateID,
    }
}
