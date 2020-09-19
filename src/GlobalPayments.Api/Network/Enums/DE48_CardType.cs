using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE48_CardType {
        [Map(Target.NWS, "AA")]
        AmericanAirlines,
        [Map(Target.NWS, "AC")]
        AmericansCoOp,
        [Map(Target.NWS, "AD")]
        AssociatedDist,
        [Map(Target.NWS, "AE")]
        AccenteStoresInc,
        [Map(Target.NWS, "AG")]
        AssociateGold,
        [Map(Target.NWS, "AI")]
        AssocDistInc,
        [Map(Target.NWS, "AM")]
        Adamas,
        [Map(Target.NWS, "AN")]
        AMEXInternetIntern,
        [Map(Target.NWS, "AP")]
        PhhAmerPetroInst,
        [Map(Target.NWS, "AV")]
        Avcard,
        [Map(Target.NWS, "AX")]
        AmericanExpress,
        [Map(Target.NWS, "B7")]
        B1BudgetPrefixBlock,
        [Map(Target.NWS, "BB")]
        Bombay,
        [Map(Target.NWS, "BC")]
        BeaconUltramar,
        [Map(Target.NWS, "BD")]
        Budget,
        [Map(Target.NWS, "BG")]
        BigSur,
        [Map(Target.NWS, "BM")]
        BMOSShoppersCharge,
        [Map(Target.NWS, "BR")]
        BusinessRevolving_Retail,
        [Map(Target.NWS, "BS")]
        Betaseron,
        [Map(Target.NWS, "BY")]
        BombayCompany,
        [Map(Target.NWS, "C6")]
        P66ProprietaryCard,
        [Map(Target.NWS, "CA")]
        ShellCanada,
        [Map(Target.NWS, "CB")]
        CarteBlanc,
        [Map(Target.NWS, "CC")]
        Scribbles,
        [Map(Target.NWS, "CD")]
        CareCredit,
        [Map(Target.NWS, "CE")]
        CitgoFleet,
        [Map(Target.NWS, "CG")]
        CheckGuarantee,
        [Map(Target.NWS, "CI")]
        ChildrensPlace,
        [Map(Target.NWS, "CK")]
        Checking,
        [Map(Target.NWS, "CL")]
        Cardlock,
        [Map(Target.NWS, "CN")]
        Canadian,
        //[Map(Target.NWS, "CO")]
        //Conoco,
        [Map(Target.NWS, "CR")]
        Catherines,
        [Map(Target.NWS, "CS")]
        SPSCash,
        [Map(Target.NWS, "CT")]
        CreditCard,
        [Map(Target.NWS, "CU")]
        ComputerCity,
        [Map(Target.NWS, "CV")]
        CheckVerification,
        [Map(Target.NWS, "CX")]
        ConocoPrivateLabel,
        [Map(Target.NWS, "CY")]
        CompUSA,
        [Map(Target.NWS, "DB")]
        PINDebitCard,
        [Map(Target.NWS, "DC")]
        DinersClub,
        [Map(Target.NWS, "DL")]
        Dell,
        [Map(Target.NWS, "DM")]
        DmManagement,
        [Map(Target.NWS, "DN")]
        DressBarn,
        [Map(Target.NWS, "DO")]
        DriveoffCard,
        [Map(Target.NWS, "DS")]
        DiscoverCard,
        [Map(Target.NWS, "DX")]
        DiamondExteriors,
        [Map(Target.NWS, "EC")]
        EBTCash,
        [Map(Target.NWS, "EF")]
        EBTFoodStamps,
        [Map(Target.NWS, "EL")]
        ElekTekMngdBest,
        [Map(Target.NWS, "EM")]
        Email,
        [Map(Target.NWS, "EO")]
        EnglefieldOil,
        [Map(Target.NWS, "ER")]
        ECredit,
        [Map(Target.NWS, "ES")]
        Esprit,
        [Map(Target.NWS, "ET")]
        ElekTekOwnedBest,
        [Map(Target.NWS, "EW")]
        eWIC,
        [Map(Target.NWS, "EX")]
        Electrolux,
        //[Map(Target.NWS, "FC")]
        //ConocoFleet,
        [Map(Target.NWS, "FE")]
        FrysElectronics,
        [Map(Target.NWS, "FG")]
        FleetCorFleetwide,
        [Map(Target.NWS, "FJ")]
        MarksAndMorgan,
        [Map(Target.NWS, "FM")]
        FredMyer,
        [Map(Target.NWS, "FO")]
        FleetOne,
        [Map(Target.NWS, "FP")]
        FleetCorFuelmanPlus,
        [Map(Target.NWS, "FX")]
        FleetOnedeMexico,
        [Map(Target.NWS, "GA")]
        GatePetroleum,
        [Map(Target.NWS, "GB")]
        GoodyscobrandMastercard,
        [Map(Target.NWS, "GC")]
        BuckleGiftCard,
        [Map(Target.NWS, "GD")]
        GoldAndDiamonds,
        [Map(Target.NWS, "GL")]
        GalleryFurniture,
        [Map(Target.NWS, "GM")]
        GanderMountain,
        [Map(Target.NWS, "GO")]
        GoCard,
        [Map(Target.NWS, "GP")]
        GECapitalPL,
        [Map(Target.NWS, "GR")]
        GasCityCoConsumer,
        [Map(Target.NWS, "GS")]
        GeneralServiceAsc,
        [Map(Target.NWS, "GT")]
        GatewayManaged,
        [Map(Target.NWS, "GU")]
        GatewayEmployerPgm,
        [Map(Target.NWS, "GW")]
        GatewayOwned,
        [Map(Target.NWS, "GY")]
        GoodyearConsumer,
        [Map(Target.NWS, "GZ")]
        GoodyearCommercial,
        [Map(Target.NWS, "HH")]
        House2Home,
        [Map(Target.NWS, "HI")]
        Hifi,
        [Map(Target.NWS, "HP")]
        HomePlace,
        [Map(Target.NWS, "HZ")]
        Helzberg,
        [Map(Target.NWS, "IA")]
        InternatlAutomated,
        [Map(Target.NWS, "IU")]
        IncredibleUniverse,
        [Map(Target.NWS, "JC")]
        JCB,
        [Map(Target.NWS, "JJ")]
        JayJacobs,
        [Map(Target.NWS, "JK")]
        JCKeepsake,
        [Map(Target.NWS, "JL")]
        Jewelry3,
        [Map(Target.NWS, "JU")]
        Justice_SVS,
        [Map(Target.NWS, "KG")]
        KrigelJewelers,
        [Map(Target.NWS, "KM")]
        KerrMcgee,
        [Map(Target.NWS, "KS")]
        KellySpringfield,
        [Map(Target.NWS, "LA")]
        LauraAshley,
        [Map(Target.NWS, "LE")]
        Lechmere,
        [Map(Target.NWS, "LI")]
        LimitedToo,
        [Map(Target.NWS, "MA")]
        Credit,
        [Map(Target.NWS, "MB")]
        MastercardOpenSystem,
        [Map(Target.NWS, "MC")]
        Mastercard,
        [Map(Target.NWS, "MF")]
        MastercardFleet,
        [Map(Target.NWS, "MD")]
        McDuff,
        [Map(Target.NWS, "MH")]
        Centego,
        [Map(Target.NWS, "MI")]
        BMoss,
        [Map(Target.NWS, "MM")]
        MarksAndMorgan_2,
        [Map(Target.NWS, "MN")]
        MenardsCard,
        [Map(Target.NWS, "MO")]
        MastercardPurchasing,
        [Map(Target.NWS, "MP")]
        UnitedSergMedcash,
        [Map(Target.NWS, "MR")]
        Maurices,
        [Map(Target.NWS, "MS")]
        Multiservice,
        [Map(Target.NWS, "MX")]
        CircleKMidwest_MACS_GiftCardviaPaymentech,
        [Map(Target.NWS, "NT")]
        NatlTireAndBattery,
        [Map(Target.NWS, "OB")]
        OfficeDepotCommercial,
        [Map(Target.NWS, "OC")]
        OfficemaxConsumer,
        [Map(Target.NWS, "OD")]
        OfficeDepot,
        [Map(Target.NWS, "OE")]
        OfficeDepotConsumer,
        [Map(Target.NWS, "OF")]
        OfficeDepot_1,
        [Map(Target.NWS, "OH")]
        OtherCredit_ForReconciliation,
        [Map(Target.NWS, "OM")]
        OfficeMax,
        [Map(Target.NWS, "OT")]
        OfficeDepotCanada,
        [Map(Target.NWS, "OW")]
        OfficeMax_1,
        [Map(Target.NWS, "P3")]
        PrivateLabel,
        [Map(Target.NWS, "P6")]
        P66ProprietaryCard_1,
        [Map(Target.NWS, "PA")]
        Phillips66,
        [Map(Target.NWS, "PC")]
        PulseCard,
        [Map(Target.NWS, "PE")]
        StdParkingExchange,
        [Map(Target.NWS, "PG")]
        PipPrinting,
        //[Map(Target.NWS, "PH")]
        //Phillips,
        [Map(Target.NWS, "PK")]
        ParkCard,
        [Map(Target.NWS, "PL")]
        PrivateLabel_1,
        [Map(Target.NWS, "PM")]
        Primestar,
        [Map(Target.NWS, "PP")]
        PayPal,
        [Map(Target.NWS, "PR")]
        GprkPrivAndPayment,
        [Map(Target.NWS, "PS")]
        Catherines_1,
        [Map(Target.NWS, "PT")]
        PhillipsToscoFleet,
        [Map(Target.NWS, "PU")]
        PHHAmericaFleet,
        [Map(Target.NWS, "PX")]
        PhillipsPrivateLabel,
        [Map(Target.NWS, "PZ")]
        PFALShoppersCharge,
        [Map(Target.NWS, "Q1")]
        QuestPhoneCard,
        [Map(Target.NWS, "QC")]
        QuickCredit,
        [Map(Target.NWS, "RD")]
        RiddlesJewelry,
        [Map(Target.NWS, "RL")]
        ReadyLink,
        [Map(Target.NWS, "RM")]
        RadioShackMC2,
        [Map(Target.NWS, "RO")]
        Romanos,
        [Map(Target.NWS, "RP")]
        RadioShackMC1,
        [Map(Target.NWS, "RS")]
        RadioShack,
        [Map(Target.NWS, "S1")]
        ShellMastercard,
        [Map(Target.NWS, "S2")]
        SearsCoBranded,
        [Map(Target.NWS, "S3")]
        SearsCommercialOne,
        [Map(Target.NWS, "S4")]
        SamsConsumer,
        [Map(Target.NWS, "S5")]
        SamsBBC,
        [Map(Target.NWS, "SA")]
        SuperAmericaCobranded,
        [Map(Target.NWS, "SB")]
        SuperAmericaFleet,
        [Map(Target.NWS, "SC")]
        SearsCanada,
        [Map(Target.NWS, "SD")]
        StandardParking,
        [Map(Target.NWS, "SE")]
        SirSpeedyInc_1,
        [Map(Target.NWS, "SF")]
        ShellFleetPlus,
        [Map(Target.NWS, "SH")]
        ShopatHome,
        [Map(Target.NWS, "SI")]
        SirSpeedyInc_2,
        [Map(Target.NWS, "SL")]
        StaplesConsumer,
        [Map(Target.NWS, "SM")]
        SmarteCarte,
        [Map(Target.NWS, "SN")]
        PacificSun,
        [Map(Target.NWS, "SO")]
        ShopNBCValueVision,
        [Map(Target.NWS, "SP")]
        Staples,
        [Map(Target.NWS, "es")]
        spacImpdependent,
        [Map(Target.NWS, "SR")]
        SearsCharge,
        [Map(Target.NWS, "SS")]
        SeamansFurniture,
        [Map(Target.NWS, "ST")]
        Staples_1,
        [Map(Target.NWS, "SU")]
        Sunoco,
        [Map(Target.NWS, "SV")]
        SVSStoredValue,
        [Map(Target.NWS, "SW")]
        HeartlandGiftCard_Proprietary,
        [Map(Target.NWS, "SZ")]
        ShazamDebit,
        [Map(Target.NWS, "TA")]
        TechAmerica,
        [Map(Target.NWS, "TC")]
        SpsTestCard,
        [Map(Target.NWS, "TF")]
        TesoroFleetAlaska,
        [Map(Target.NWS, "TK")]
        TakeCharge,
        [Map(Target.NWS, "TO")]
        TotalCharge,
        [Map(Target.NWS, "TP")]
        TesoroConsumer,
        [Map(Target.NWS, "TR")]
        Tristar,
        [Map(Target.NWS, "TS")]
        Testing,
        [Map(Target.NWS, "TV")]
        TractorSupplyComrv,
        [Map(Target.NWS, "TW")]
        TesoroFleetHawaii,
        [Map(Target.NWS, "TX")]
        TractorSupplyComnp,
        [Map(Target.NWS, "TY")]
        TractorSupplyCo,
        [Map(Target.NWS, "TZ")]
        TransPlatinum,
        [Map(Target.NWS, "UD")]
        UnitedDairyFarmers,
        [Map(Target.NWS, "UJ")]
        UltraJewelers,
        [Map(Target.NWS, "UO")]
        USOil,
        [Map(Target.NWS, "UP")]
        DiamondProprietary,
        [Map(Target.NWS, "UR")]
        UnitedRefining,
        [Map(Target.NWS, "US")]
        UnitedSurgical,
        [Map(Target.NWS, "UT")]
        UnitedTravelCard,
        [Map(Target.NWS, "VA")]
        VirginiaSpecialties,
        [Map(Target.NWS, "VB")]
        VisaOpenSystem,
        [Map(Target.NWS, "VE")]
        ValueAmerica,
        [Map(Target.NWS, "VF")]
        Voyager,
        [Map(Target.NWS, "VG")]
        ValueLinkStoredValue,
        [Map(Target.NWS, "VI")]
        Visa,
        [Map(Target.NWS, "VL")]
        Valvoline,
        [Map(Target.NWS, "VM")]
        VistaMarketing,
        [Map(Target.NWS, "VO")]
        VisaPurchasing,
        [Map(Target.NWS, "VS")]
        VirginiaSpecialties_1,
        [Map(Target.NWS, "VT")]
        VisaFleet,
        [Map(Target.NWS, "VV")]
        ValvCoBrandedVisa,
        [Map(Target.NWS, "W1")]
        PSSPOSActivation,
        [Map(Target.NWS, "W2")]
        PSSPINRetrieval,
        [Map(Target.NWS, "W3")]
        PSSBankCardDirect,
        [Map(Target.NWS, "W4")]
        PSSComcastPrepaid,
        [Map(Target.NWS, "W5")]
        PSSBoostPrepaid,
        [Map(Target.NWS, "WA")]
        WEXGatewayPet,
        [Map(Target.NWS, "WB")]
        WalmartBRC,
        [Map(Target.NWS, "WC")]
        WexClark,
        [Map(Target.NWS, "WE")]
        WexEmro,
        [Map(Target.NWS, "WG")]
        WesGasCity,
        [Map(Target.NWS, "WL")]
        WalmartConsumer,
        [Map(Target.NWS, "WM")]
        HessWexCard,
        [Map(Target.NWS, "WN")]
        Winston,
        [Map(Target.NWS, "WO")]
        WithamOil,
        [Map(Target.NWS, "WP")]
        WEXProprietary,
        [Map(Target.NWS, "WR")]
        TesoroWEX,
        [Map(Target.NWS, "WS")]
        SSAWEX,
        [Map(Target.NWS, "WT")]
        WestMarine,
        [Map(Target.NWS, "WV")]
        WexValvoline,
        [Map(Target.NWS, "WX")]
        WEX,
        [Map(Target.NWS, "WZ")]
        WEXPLforGoGas,
        [Map(Target.NWS, "XA")]
        TemporaryAmex,
        [Map(Target.NWS, "XB")]
        TemporaryCB,
        [Map(Target.NWS, "XC")]
        TemporaryDC,
        [Map(Target.NWS, "XD")]
        TemporaryDiscover,
        [Map(Target.NWS, "XF")]
        TemporaryVoyagerFt,
        [Map(Target.NWS, "XI")]
        TemporaryIfcsFal,
        [Map(Target.NWS, "XJ")]
        TemporaryJCB,
        [Map(Target.NWS, "XX")]
        TemporaryMC,
        [Map(Target.NWS, "XY")]
        TemporaryVisa,
        [Map(Target.NWS, "ZB")]
        BaileyBanksAndBiddle,
        [Map(Target.NWS, "ZG")]
        Gordons,
        [Map(Target.NWS, "ZS")]
        Zales,
        [Map(Target.NWS, "WF")]
        WorldFuels
    }
}
