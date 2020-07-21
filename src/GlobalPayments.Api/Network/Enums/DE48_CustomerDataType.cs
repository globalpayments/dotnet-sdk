using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE48_CustomerDataType {
        [Map(Target.NWS, "0")]
        UnencryptedIdNumber,
        [Map(Target.NWS, "1")]
        Vehicle_Trailer_Number,
        [Map(Target.NWS, "2")]
        VehicleTag,
        [Map(Target.NWS, "3")]
        DriverId_EmployeeNumber,
        [Map(Target.NWS, "4")]
        Odometer_Hub_Reading,
        [Map(Target.NWS, "5")]
        DriverLicense_Number,
        [Map(Target.NWS, "6")]
        DriverLicense_State_Province,
        [Map(Target.NWS, "7")]
        DriverLicense_Name,
        [Map(Target.NWS, "8")]
        WorkOrder_PoNumber,
        [Map(Target.NWS, "9")]
        InvoiceNumber,
        [Map(Target.NWS, "A")]
        TripNumber,
        [Map(Target.NWS, "B")]
        UnitNumber,
        [Map(Target.NWS, "C")]
        TrailerHours_ReferHours,
        [Map(Target.NWS, "D")]
        DateofBirth,
        [Map(Target.NWS, "E")]
        PostalCode,
        [Map(Target.NWS, "F")]
        EnteredData_Numeric,
        [Map(Target.NWS, "G")]
        EnteredData_AlphaNumeric,
        [Map(Target.NWS, "Q")]
        SocialSecurityNumber,
        [Map(Target.NWS, "R")]
        CardPresentSecurityCode,
        [Map(Target.NWS, "S")]
        ServicePrompt,
        [Map(Target.NWS, "T")]
        PassportNumber,
        [Map(Target.NWS, "U")]
        JobNumber,
        [Map(Target.NWS, "V")]
        Department,
        [Map(Target.NWS, "W")]
        LoyaltyInformation,
        [Map(Target.NWS, "Z")]
        Merchant_Order_Number
    }
}
