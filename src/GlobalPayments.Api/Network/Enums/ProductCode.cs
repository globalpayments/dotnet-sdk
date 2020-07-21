using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum ProductCode {
        [Map(Target.NWS, "01")]
        Unleaded_Gas,
        [Map(Target.NWS, "02")]
        Unleaded_Premium_Gas,
        [Map(Target.NWS, "03")]
        Super_Premium_Gas,
        [Map(Target.NWS, "04")]
        Ethanol_Unleaded_Reg,
        [Map(Target.NWS, "05")]
        Ethanol_Unleaded_Mid_Grade,
        [Map(Target.NWS, "06")]
        Ethanol_Unleaded_Premium,
        [Map(Target.NWS, "07")]
        Ethanol_Unleaded_Super,
        [Map(Target.NWS, "08")]
        Ethanol_Regular_Leaded,
        [Map(Target.NWS, "09")]
        Methanol_Unleaded_Mid_Grade,
        [Map(Target.NWS, "10")]
        Methanol_Unleaded_Premium,
        [Map(Target.NWS, "11")]
        Methanol_Unleaded_Super,
        [Map(Target.NWS, "12")]
        Methanol_Regular_Leaded,
        [Map(Target.NWS, "13")]
        Methanol_Unleaded_Regular,
        [Map(Target.NWS, "14")]
        Regular_Leaded,
        [Map(Target.NWS, "15")]
        Mid_Grade_Gas,
        [Map(Target.NWS, "16")]
        No_2_Diesel,
        [Map(Target.NWS, "17")]
        Kerosene,
        [Map(Target.NWS, "18")]
        Propane,
        [Map(Target.NWS, "19")]
        CNG_Gas,
        [Map(Target.NWS, "20")]
        Jet_Fuel,
        [Map(Target.NWS, "21")]
        Unleaded_Reformulated,
        [Map(Target.NWS, "22")]
        Unleaded_Mid_Grade_Reformulated,
        [Map(Target.NWS, "23")]
        Unleaded_Premium_Gas_Reformulated,
        [Map(Target.NWS, "24")]
        Unleaded_Super_Reformulated,
        [Map(Target.NWS, "25")]
        Natural_Gas,
        [Map(Target.NWS, "26")]
        Gasohol_Gas_10_Percent,
        [Map(Target.NWS, "27")]
        Gasohol_Gas_7_Point_7_Percent,
        [Map(Target.NWS, "28")]
        Gasohol_Gas_5_Point_7_Percent,
        [Map(Target.NWS, "29")]
        White_Gas,
        [Map(Target.NWS, "30")]
        Dual_Propane_Unleaded,
        [Map(Target.NWS, "31")]
        Wide_nozzle_unleaded,
        [Map(Target.NWS, "32")]
        Marine_Fuel,
        [Map(Target.NWS, "33")]
        Motor_Fuel,
        [Map(Target.NWS, "34")]
        Methanol_85,
        [Map(Target.NWS, "35")]
        Ethanol_85,
        [Map(Target.NWS, "36")]
        No_1_Diesel,
        [Map(Target.NWS, "37")]
        Aviation_Gas,
        [Map(Target.NWS, "38")]
        Military_Fuel,
        [Map(Target.NWS, "39")]
        Other_Fuel,
        [Map(Target.NWS, "50")]
        Motor_Oils,
        [Map(Target.NWS, "51")]
        Oil_Change,
        [Map(Target.NWS, "60")]
        Automotive_Products,
        [Map(Target.NWS, "61")]
        Automotive_Glass,
        [Map(Target.NWS, "62")]
        Car_Wash,
        [Map(Target.NWS, "63")]
        Lamps,
        [Map(Target.NWS, "64")]
        Wipers,
        [Map(Target.NWS, "65")]
        Fluids_and_Coolant,
        [Map(Target.NWS, "66")]
        Hoses_and_Belts,
        [Map(Target.NWS, "67")]
        Tires,
        [Map(Target.NWS, "68")]
        Filters,
        [Map(Target.NWS, "69")]
        Batteries,
        [Map(Target.NWS, "70")]
        Repairs_Services,
        [Map(Target.NWS, "71")]
        Engine_Service,
        [Map(Target.NWS, "72")]
        Transmission_Service,
        [Map(Target.NWS, "73")]
        Brake_Service,
        [Map(Target.NWS, "74")]
        Towing,
        [Map(Target.NWS, "75")]
        Tuneup,
        [Map(Target.NWS, "76")]
        Inspection,
        [Map(Target.NWS, "77")]
        Storage,
        [Map(Target.NWS, "78")]
        Labor,
        [Map(Target.NWS, "79")]
        Reserved,
        [Map(Target.NWS, "80")]
        Groceries,
        [Map(Target.NWS, "81")]
        Cigarettes_Tobacco,
        [Map(Target.NWS, "82")]
        Soda,
        [Map(Target.NWS, "83")]
        Health_Beauty_Aid,
        [Map(Target.NWS, "84")]
        Milk_Juice,
        [Map(Target.NWS, "85")]
        Misc_Beverage,
        [Map(Target.NWS, "86")]
        Restaurant,
        [Map(Target.NWS, "87")]
        Beer_and_Wine,
        [Map(Target.NWS, "90")]
        Miscellaneous,
        [Map(Target.NWS, "97")]
        Federal_excise_tax_on_tire_lube,
        [Map(Target.NWS, "98")]
        Sales_tax,
        [Map(Target.NWS, "99")]
        Discount
    }
}
