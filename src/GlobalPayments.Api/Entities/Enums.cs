using GlobalPayments.Api.Utils;
using System;
using System.Runtime.Serialization;

namespace GlobalPayments.Api.Entities {
    internal enum AliasAction {
        CREATE,
        ADD,
        DELETE
    }

    /// <summary>
    /// Indicates an address type.
    /// </summary>
    public enum AddressType {
        /// <summary>
        /// Indicates a billing address.
        /// </summary>
        [Description("BILLING")]
        Billing,

        /// <summary>
        /// Indicates a shipping address.
        /// </summary>
        [Description("SHIPPING")]
        Shipping,

        [Description("BUSINESS")]
        Business,
    }

    /// <summary>
    /// Indicates a device type for out of scope / semi-integrated devices.
    /// </summary>
    public enum DeviceType
    {        
        /// <summary>
        /// Indicates PAX device.
        /// </summary>
        PAX_DEVICE,
        /// <summary>
        /// Indicates PAX D200 device.
        /// </summary>
        PAX_D200,
        /// <summary>
        /// INdicates PAX D210 device.
        /// </summary>
        PAX_D210,
        /// <summary>
        /// Indicates a Pax S300 device.
        /// </summary>
        PAX_S300,
        /// <summary>
        /// Indicates PAX PX5 device.
        /// </summary>
        PAX_PX5,
        /// <summary>
        /// Indicates PAX PX7 device.
        /// </summary>
        PAX_PX7,

        /// <summary>
        /// Indicates a HeeartSIP iSC250 device.
        /// </summary>
        HPA_ISC250,

        /// <summary>
        /// Indicates a HeartSIP Lane 3000 device
        /// </summary>
        HPA_LANE3000,
        /// <summary>
        /// Indicates a UPA device
        /// </summary>
        UPA_DEVICE,
        PAX_ARIES8,
        PAX_A80,
        PAX_A35,
        PAX_A920,
        PAX_A77,
        NEXGO_N5,
        /// <summary>
        /// Indicates a genius terminal
        /// </summary>
        GENIUS,

        /// <summary>
        /// Indicates a Nucleus terminal
        /// </summary>
        NUCLEUS_SATURN_1000,
        /// <summary>
        /// Indicates a genius verifone P400
        /// </summary>
        GENIUS_VERIFONE_P400,

    }

    /// <summary>
    /// Indicates an inquiry type.
    /// </summary>
    public enum InquiryType {
        /// <summary>
        /// Indicates a foodstamp inquiry.
        /// </summary>
        FOODSTAMP,

        /// <summary>
        /// Indicates a cash inquiry.
        /// </summary>
        CASH
    }

    /// <summary>
    /// Indicates a payment method type.
    /// </summary>
    [Flags]
    public enum PaymentMethodType {
        /// <summary>
        /// Indicates a payment method reference.
        /// </summary>
        /// <remarks>
        /// Should be accompanied by a gateway transaction ID.
        /// </remarks>
        Reference = 0,

        /// <summary>
        /// Indicates a credit or PIN-less debit account.
        /// </summary>
        /// <remarks>
        /// Should be accompanied by a token, card number, or track data.
        /// </remarks>
        Credit = 1 << 1,

        /// <summary>
        /// Indicates a PIN debit account.
        /// </summary>
        /// <remarks>
        /// Should be accompanied by track data and a PIN block.
        /// </remarks>
        Debit = 1 << 2,

        /// <summary>
        /// Indicates an EBT account.
        /// </summary>
        /// <remarks>
        /// Should be accompanied by track data and a PIN block.
        /// </remarks>
        EBT = 1 << 3,

        /// <summary>
        /// Indicates cash as the payment method.
        /// </summary>
        Cash = 1 << 4,

        /// <summary>
        /// Indicates an ACH/eCheck account.
        /// </summary>
        /// <remarks>
        /// Should be accompanied by a token or an account number and routing number.
        /// </remarks>
        ACH = 1 << 5,

        /// <summary>
        /// Indicates a gift/loyalty/stored value account.
        /// </summary>
        /// <remarks>
        /// Should be accompanied by a token, card number, alias, or track data.
        /// </remarks>
        Gift = 1 << 6,

        /// <summary>
        /// Indicates a recurring payment method.
        /// </summary>
        /// <remarks>
        /// Should be accompanied by a payment method key.
        /// </remarks>
        Recurring = 1 << 7,

        Other = 1 << 8,

        APM = 1 << 9,

        Ewic = 1 << 10,

        BankPayment = 1 << 11,

        BNPL = 1 << 12,

        Account_Funds = 1 << 13
    }

    /// <summary>
    /// Indicates how the payment method data was obtained.
    /// </summary>
    public enum EntryMethod {
        /// <summary>
        /// Indicates manual entry.
        /// </summary>
        Manual,

        /// <summary>
        /// Indicates swipe entry.
        /// </summary>
        Swipe,

        /// <summary>
        /// Indicates proximity/contactless entry.
        /// </summary>
        Proximity,

        /// <summary>
        /// Indicates contact entry
        /// </summary>
        Chip
    }

    /// <summary>
    /// Indicates how the payment method data was obtained.
    /// </summary>
    public enum ManualEntryMethod
    {
        /// <summary>
        /// Indicates manual entry.
        /// </summary>
        Moto,

        /// <summary>
        /// Indicates swipe entry.
        /// </summary>
        Mail,

        /// <summary>
        /// Indicates proximity/contactless entry.
        /// </summary>
        Phone
    }

    /// <summary>
    /// Indicates how the gift/loyalty/stored value account data was obtained.
    /// </summary>
    public enum GiftEntryMethod {
        /// <summary>
        /// Indicates swipe entry.
        /// </summary>
        Swipe,

        /// <summary>
        /// Indicates an alias was entered.
        /// </summary>
        Alias,

        /// <summary>
        /// Indicates manual entry.
        /// </summary>
        Manual
    }    

    /// <summary>
    /// Indicates if a transaction should be specialized.
    /// </summary>
    public enum TransactionModifier {
        /// <summary>
        /// Indicates no specialization.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates an incremental transaction.
        /// </summary>
        Incremental = 1 << 1,

        /// <summary>
        /// Indicates an additional transaction.
        /// </summary>
        Additional = 1 << 2,

        /// <summary>
        /// Indicates an offline transaction.
        /// </summary>
        Offline = 1 << 3,

        /// <summary>
        /// Indicates a commercial request transaction.
        /// </summary>
        Level_II = 1 << 4,

        /// <summary>
        /// Indicates a fraud decline transaction.
        /// </summary>
        FraudDecline = 1 << 5,

        /// <summary>
        /// Indicates a chip decline transaction.
        /// </summary>
        ChipDecline = 1 << 6,

        /// <summary>
        /// Indicates a cash back transaction.
        /// </summary>
        CashBack = 1 << 7,

        /// <summary>
        /// Indicates a voucher transaction.
        /// </summary>
        Voucher = 1 << 8,

        /// <summary>
        /// Indicates a consumer authentication (3DSecure) transaction.
        /// </summary>
        Secure3D = 1 << 9,

        /// <summary>
        /// Indicates a hosted payment transaction.
        /// </summary>
        HostedRequest = 1 << 10,

        /// <summary>
        /// Indicates a recurring transaction.
        /// </summary>
        Recurring = 1 << 11,

        /// <summary>
        /// Indicates a mobile transaction.
        /// </summary>
        EncryptedMobile = 1 << 12,

        /// <summary>
        /// Indicates a level II transaction
        /// </summary>
        Level_III = 1 << 13,

        /// <summary>
        /// Indicates a mobile transaction.
        /// </summary>
        DecryptedMobile = 1 << 14,

        /// <summary>
        /// Indicates an alternative payment method transaction.
        /// </summary>
        AlternativePaymentMethod = 1 << 15,

        /// <summary>
        /// Indicates a DeletePreAuth transaction
        /// </summary>
        DeletePreAuth = 1 << 16,

        /// <summary>
        /// Indicates a OpenBanking transaction
        /// </summary>
        BankPayment = 1 << 17,

        [Description("MERCHANT")]
        Merchant = 1 << 18,

        /// <summary>
        /// Indicates a Buy Now Pay Later transaction
        /// </summary>
        BuyNowPayLater = 1 << 19,

        Account = 1 << 20
    }

    /// <summary>
    /// Indicates the tax type.
    /// </summary>
    public enum TaxType {
        /// <summary>
        /// Indicates tax was not used.
        /// </summary>
        NOTUSED,

        /// <summary>
        /// Indicates sales tax was applied.
        /// </summary>
        SALESTAX,

        /// <summary>
        /// Indicates tax exemption.
        /// </summary>
        TAXEXEMPT
    }

    /// <summary>
    /// Indicates the currency type.
    /// </summary>
    public enum CurrencyType {
        /// <summary>
        /// Indicates a true currency.
        /// </summary>
        CURRENCY,

        /// <summary>
        /// Indicates loyalty points.
        /// </summary>
        POINTS,

        CASH_BENEFITS,

        FOODSTAMPS,

        VOUCHER
    }

    /// <summary>
    /// Indicates the account type for ACH/eCheck transactions.
    /// </summary>
    public enum AccountType {
        /// <summary>
        /// Indicates a checking account.
        /// </summary>
        [Map(Target.GP_API, "CHECKING")] 
        [Description("Checking")]
        
        [Map(Target.TransactionApi, "Checking")]
        CHECKING,

        /// <summary>
        /// Indicates a savings account.
        /// </summary>
        [Map(Target.GP_API, "SAVING")]
        [Description("Savings")]
        SAVINGS,

        /// <summary>
        /// Indicates a credit account.
        /// </summary>
        [Map(Target.GP_API, "CREDIT")]
        [Description("Credit")]
        CREDIT
    }

    /// <summary>
    /// Indicates the check type for ACH/eCheck transactions.
    /// </summary>
    public enum CheckType {
        /// <summary>
        /// Indicates a personal check.
        /// </summary>
        [Map(Target.NWS, "0")]
        [Description("Personal")]
        PERSONAL,

        /// <summary>
        /// Indicates a business check.
        /// </summary>
        [Map(Target.NWS, "3")]
        [Description("Business")]
        BUSINESS,

        /// <summary>
        /// Indicates a payroll check.
        /// </summary>
        [Map(Target.NWS, "1")]
        [Description("Payroll")]
        PAYROLL
    }

    /// <summary>
    /// Indicates the NACHA standard entry class (SEC) code for ACH/eCheck transactions.
    /// </summary>
    public abstract class SecCode {
        /// <summary>
        /// Indicates prearranged payment and deposit (PPD).
        /// </summary>
        public const string PPD = "PPD";

        /// <summary>
        /// Indicates cash concentration or disbursement (CCD).
        /// </summary>
        public const string CCD = "CCD";

        /// <summary>
        /// Indicates point of purchase entry (POP).
        /// </summary>
        public const string POP = "POP";

        /// <summary>
        /// Indicates internet initiated entry (WEB).
        /// </summary>
        public const string WEB = "WEB";

        /// <summary>
        /// Indicates telephone initiated entry (TEL).
        /// </summary>
        public const string TEL = "TEL";

        /// <summary>
        /// Indicates verification only.
        /// </summary>
        public const string EBRONZE = "EBRONZE";
    }

    /// <summary>
    /// Indicates the report type.
    /// </summary>
    [Flags]
    public enum ReportType {
        /// <summary>
        /// Indicates a FindTransactions report.
        /// </summary>
        FindTransactions = 1,

        /// <summary>
        /// Indicates an Activity report.
        /// </summary>
        Activity = 1 << 1,

        /// <summary>
        /// Indicates a BatchDetail report.
        /// </summary>
        BatchDetail = 1 << 2,

        /// <summary>
        /// Indicates a BatchHistory report.
        /// </summary>
        BatchHistory = 1 << 3,

        /// <summary>
        /// Indicates a BatchSummary report.
        /// </summary>
        BatchSummary = 1 << 4,

        /// <summary>
        /// Indicates an OpenAuths report.
        /// </summary>
        OpenAuths = 1 << 5,

        /// <summary>
        /// Indicates a Search report.
        /// </summary>
        Search = 1 << 6,

        /// <summary>
        /// Indicates a TransactionDetail report.
        /// </summary>
        TransactionDetail = 1 << 7,

        /// <summary>
        /// Indicates a deposit report
        /// </summary>
        FindDeposits = 1 << 8,

        /// <summary>
        /// Indicates a dispute report
        /// </summary>
        FindDisputes = 1 << 9,

        /// <summary>
        /// Indicates a Deposit Details report.
        /// </summary>
        DepositDetail = 1 << 10,

        /// <summary>
        /// Indicates a Dispute Details report.
        /// </summary>
        DisputeDetail = 1 << 11,

        /// <summary>
        /// Indicates a Settlement Dispute Details report.
        /// </summary>
        SettlementDisputeDetail = 1 << 12,

        /// <summary>
        /// Indicates a Transactions report paged
        /// </summary>
        FindTransactionsPaged = 1 << 13,

        /// <summary>
        /// Indicates a Settlement Transactions report paged
        /// </summary>
        FindSettlementTransactionsPaged = 1 << 14,

        /// <summary>
        /// Indicates a Deposit report paged
        /// </summary>
        FindDepositsPaged = 1 << 15,

        /// <summary>
        /// Indicates a Dispute report paged
        /// </summary>
        FindDisputesPaged = 1 << 16,

        /// <summary>
        /// Indicates a Settlement Dispute report paged
        /// </summary>
        FindSettlementDisputesPaged = 1 << 17,

        /// <summary>
        /// Indicates a Stored Payment Method details report
        /// </summary>
        StoredPaymentMethodDetail = 1 << 18,

        /// <summary>
        /// Indicates a Stored Payment Methods report paged
        /// </summary>
        FindStoredPaymentMethodsPaged = 1 << 19,

        /// <summary>
        /// Indcates an action details report
        /// </summary>
        ActionDetail = 1 << 20,

        /// <summary>
        /// Indicates an actions report paged
        /// </summary>
        FindActionsPaged = 1 << 21,

        /// <summary>
        /// Indicates a Dispute Document Details report.
        /// </summary>
        DocumentDisputeDetail = 1 << 22,

        FindBankPayment = 1 << 23,

        PayByLinkDetail = 1 << 24,

        FindPayByLinkPaged = 1 << 25,

        FindMerchantsPaged = 1 << 26,

        FindAccountsPaged = 1 << 27,

        FindAccountDetail = 1 << 28
    }

    /// <summary>
    /// Indicates how timezones should be handled.
    /// </summary>
    public enum TimeZoneConversion {
        /// <summary>
        /// Indicates time is in coordinated universal time (UTC).
        /// </summary>
        UTC,

        /// <summary>
        /// Indicates the merchant is responsible for timezone conversions.
        /// </summary>
        Merchant,

        /// <summary>
        /// Indicates the datacenter, gateway, or processor is responsible
        /// for timezone conversions.
        /// </summary>
        Datacenter
    }

    /// <summary>
    /// Indicates the type of recurring schedule.
    /// </summary>
    public enum RecurringType {
        /// <summary>
        /// Indicates a fix number of payments.
        /// </summary>
        Fixed,

        /// <summary>
        /// Indicates a variable number of payments.
        /// </summary>
        Variable
    }

    /// <summary>
    /// Indicates when a transaction is ran in a recurring schedule.
    /// </summary>
    public enum RecurringSequence {
        /// <summary>
        /// Indicates the transaction is the first of a recurring schedule.
        /// </summary>
        First,

        /// <summary>
        /// Indicates the transaction is a subsequent payment of a recurring schedule.
        /// </summary>
        Subsequent,

        /// <summary>
        /// Indicates the transaction is the last of a recurring schedule.
        /// </summary>
        Last
    }

    /// <summary>
    /// Indicates when an email receipt should be sent for the transaction.
    /// </summary>
    /// <remarks>
    /// Currently only used in recurring schedules.
    /// </remarks>
    public enum EmailReceipt {
        /// <summary>
        /// Indicates an email receipt should never be sent.
        /// </summary>
        Never,

        /// <summary>
        /// Indicates an email receipt should always be sent.
        /// </summary>
        All,

        /// <summary>
        /// Indicates an email receipt should only be sent on approvals.
        /// </summary>
        Approvals,

        /// <summary>
        /// Indicates an email receipt should only be sent on declines.
        /// </summary>
        Declines
    }

    /// <summary>
    /// Indicates when in the month a recurring schedule should run.
    /// </summary>
    public enum PaymentSchedule {
        /// <summary>
        /// Indicates a specified date.
        /// </summary>
        Dynamic,

        /// <summary>
        /// Indicates the first of the month.
        /// </summary>
        FirstDayOfTheMonth,

        /// <summary>
        /// Indicates the last of the month.
        /// </summary>
        LastDayOfTheMonth
    }

    /// <summary>
    /// Indicates the frequency of a recurring schedule.
    /// </summary>
    public static class ScheduleFrequency {
        /// <summary>
        /// Indicates a schedule should process payments weekly.
        /// </summary>
        public const string WEEKLY = "Weekly";

        /// <summary>
        /// Indicates a schedule should process payments bi-weekly
        /// (every other week).
        /// </summary>
        public const string BI_WEEKLY = "Bi-Weekly";

        /// <summary>
        /// Indicates a schedule should process payments bi-monthly
        /// (twice a month).
        /// </summary>
        public const string BI_MONTHLY = "Bi-Monthly";

        /// <summary>
        /// Indicates a schedule should process payments semi-monthly
        /// (every other month).
        /// </summary>
        public const string SEMI_MONTHLY = "Semi-Monthly";

        /// <summary>
        /// Indicates a schedule should process payments monthly.
        /// </summary>
        public const string MONTHLY = "Monthly";

        /// <summary>
        /// Indicates a schedule should process payments quarterly.
        /// </summary>
        public const string QUARTERLY = "Quarterly";

        /// <summary>
        /// Indicates a schedule should process payments semi-annually
        /// (twice a year).
        /// </summary>
        public const string SEMI_ANNUALLY = "Semi-Annually";

        /// <summary>
        /// Indicates a schedule should process payments annually.
        /// </summary>
        public const string ANNUALLY = "Annually";
    }

    /// <summary>
    /// Indicates the GooglePay and ApplePay.
    /// </summary>
    public static class MobilePaymentMethodType {
        public const string APPLEPAY = "apple-pay";
        public const string GOOGLEPAY = "pay-with-google";
    }

    /// <summary>
    /// Indicates the API or source of the cryptogram.
    /// </summary>
    public static class PaymentDataSourceType
    {
        public const string APPLEPAY = "ApplePay";
        public const string APPLEPAYAPP = "ApplePayApp";
        public const string APPLEPAYWEB = "ApplePayWeb";
        public const string GOOGLEPAYAPP = "GooglePayApp";
        public const string GOOGLEPAYWEB = "GooglePayWeb";
        public const string DISCOVER3DSECURE = "Discover 3DSecure";
    }

    /// <summary>
    /// Indicates the GooglePay and ApplePay.
    /// </summary>
    public static class EncyptedMobileType
    {
        public const string APPLE_PAY = "APPLEPAY";
        public const string GOOGLE_PAY = "PAY_BY_GOOGLE";
        public const string CLICK_TO_PAY = "CLICK_TO_PAY";
    }

    public static class DigitalWalletTokenFormat{
        public const string CARD_NUMBER = "CARD_NUMBER";
        public const string CARD_TOKEN = "CARD_TOKEN";
    }
/// <summary>
/// Indicates a reason for the transaction.
/// </summary>
/// <remarks>
/// This is typically used for returns/reversals.
/// </remarks>
public enum ReasonCode {
        /// <summary>
        /// Indicates fraud.
        /// </summary>
        [Map(Target.GP_API, "FRAUD")]
        FRAUD,

        /// <summary>
        /// Indicates a false positive.
        /// </summary>
        [Map(Target.GP_API, "FALSE_POSITIVE")]
        FALSEPOSITIVE,

        /// <summary>
        /// Indicates desired good is out of stock.
        /// </summary>
        [Map(Target.GP_API, "OUT_OF_STOCK")]
        OUTOFSTOCK,

        /// <summary>
        /// Indicates desired good is in of stock.
        /// </summary>
        [Map(Target.GP_API, "IN_STOCK")]
        INSTOCK,

        /// <summary>
        /// Indicates another reason.
        /// </summary>
        [Map(Target.GP_API, "OTHER")]
        OTHER,

        /// <summary>
        /// Indicates reason was not given.
        /// </summary>
        [Map(Target.GP_API, "NOT_GIVEN")]
        NOTGIVEN
    }

    public enum ReversalReasonCode {
        CUSTOMERCANCELLATION,
        TERMINALERROR,
        TIMEOUT
    }

    public enum HostedPaymentMethods {
        OB,
        CARDS
    }

    public enum AlternativePaymentType {
        ASTROPAY_DIRECT,
        AURA,
        BALOTO_CASH,
        BANAMEX,
        BANCA_AV_VILLAS,
        BANCA_CAJA_SOCIAL,
        BANCO_GNB_SUDAMERIS,
        BANCO_CONSORCIO,
        BANCO_COOPERATIVO_COOPCENTRAL,
        BANCO_CORPBANCA,
        BANCO_DE_BOGOTA,
        BANCO_DE_CHILE_EDWARDS_CITI,
        BANCO_DE_CHILE_CASH,
        BANCO_DE_OCCIDENTE,
        BANCO_DE_OCCIDENTE_CASH,
        BANCO_DO_BRASIL,
        BANCO_FALABELLA_Chile,
        BANCO_FALABELLA_Columbia,
        BANCO_INTERNATIONAL,
        BANCO_PICHINCHA,
        BANCO_POPULAR,
        BANCO_PROCREDIT,
        BANCO_RIPLEY,
        BANCO_SANTANDER,
        BANCO_SANTANDER_BANEFE,
        BANCO_SECURITY,
        BANCOBICE,
        BANCOESTADO,
        BANCOLOMBIA,
        BANCOMER,
        BANCONTACT_MR_CASH,
        BANCOOMEVA,
        BANK_ISLAM,
        BANK_TRANSFER,
        BBVA_Chile,
        BBVA_Columbia,
        BCI_TBANC,
        BITPAY,
        BOLETO_BANCARIO,
        BRADESCO,
        CABAL,
        CARTAO_MERCADOLIVRE,
        CARULLA,
        CENCOSUD,
        CHINA_UNION_PAY,
        CIMB_CLICKS,
        CITIBANK,
        CMR,
        COLPATRIA,
        COOPEUCH,
        CORPBANCA,
        DANSKE_BANK,
        DAVIVIENDA,
        DRAGONPAY,
        EASYPAY,
        EFECTY,
        ELO,
        EMPRESA_DE_ENERGIA_DEL_QUINDIO,
        ENETS,
        ENTERCASH,
        E_PAY_PETRONAS,
        EPS,
        ESTONIAN_ONLINE_BANK_TRANSFER,
        FINLAND_ONLINE_BANK_TRANSFER,
        GIROPAY,
        HANDELSBANKEN,
        HELM_BANK,
        HIPERCARD,
        HONG_LEONG_BANK,
        IDEAL,
        INDONESIA_ATM,
        INSTANT_TRANSFER,
        INTERNATIONAL_PAY_OUT,
        ITAU_BRAZIL,
        ITAU_CHILE,
        O,
        LINK,
        LITHUANIAN_ONLINE_BANK_TRANSFER,
        MAGNA,
        MAXIMA,
        MAYBANK2U,
        MULTIBANCO,
        MYBANK,
        MYCLEAR_FPX,
        NARANJA,
        NARVESEN_LIETUVOS_SPAUDA,
        NATIVA,
        NORDEA,
        OSUUSPANKKI,
        OXXO,
        PAGO_FACIL,
        PAYPAL,
        PAYPOST_LIETUVOS_PASTAS,
        PAYSAFECARD,
        PAYSBUY_CASH,
        PAYSERA,
        PAYU,
        PERLAS,
        POLI,
        POLISH_PAYOUT,
        POP_PANKKI,
        POSTFINANCE,
        PRESTO,
        PROVINCIA_NET,
        PRZELEWY24,
        PSE,
        QIWI,
        QIWI_PAYOUT,
        RAPI_PAGO,
        REDPAGOS,
        RHB_BANK,
        SAASTOPANKKI,
        SAFETYPAY,
        SANTANDER_BRAZIL,
        SANTANDER_MEXICO,
        SANTANDER_RIO,
        SCOTIABANK,
        SEPA_DIRECTDEBIT_MERCHANT_MANDATE_MODEL_C,
        SEPA_DIRECTDEBIT_PPPRO_MANDATE_MODEL_A,
        SEPA_PAYOUT,
        SERVIPAG,
        SINGPOST,
        SKRILL,
        SOFORTUBERWEISUNG,
        SOFORT,
        S_PANKKI,
        SURTIMAX,
        TARJETA_SHOPPING,
        TELEINGRESO,
        TESTPAY,
        TRUSTLY,
        TRUSTPAY,
        WEBMONEY,
        WEBPAY,
        WECHAT_PAY,
        ZIMPLER,
        UK_DIRECT_DEBIT,
        PAYBYBANKAPP,
        ALIPAY
    }

    public enum CardType {
        VISA,
        MC,
        DISC,
        AMEX,
        GIFTCARD,
        PAYPALECOMMERCE
    }

    /// <summary>
    /// Specifies the reservation service provider
    /// </summary>
    public enum TableServiceProviders {
        FreshTxt
    }

    public enum PayGroupFrequency {
        Annually = 1,
        Quarterly = 4,
        Monthly = 12,
        SemiMonthly = 24,
        BiWeekly = 26,
        Weekly = 52
    }

    internal class DataServiceReportTypes {
        public const string TRANSACTION = "transaction";
        public const string DEPOSIT = "deposit";
        public const string DISPUTE = "dispute";
    }

    public enum ApplicationCryptogramType {
        TC,
        ARQC
    }

    internal static class SAFReportType {
        public const string APPROVED = "APPROVED SAF SUMMARY";
        public const string PENDING = "PENDING SAF SUMMARY";
        public const string DECLINED = "DECLINED SAF SUMMARY";
        public const string OFFLINE_APPROVED = "OFFLINE APPROVED SAF SUMMARY";
        public const string PARTIALLY_APPROVED = "PARTIALLY APPROVED  SAF SUMMARY";
        public const string APPROVED_VOID = "APPROVED SAF VOID SUMMARY";
        public const string PENDING_VOID = "PENDING SAF VOID SUMMARY";
        public const string DECLINED_VOID = "DECLINED SAF VOID SUMMARY";
        public const string PROVISIONAL = "PROVISIONAL SAF SUMMARY";                    
        public const string DISCARDED = "DISCARDED SAF SUMMARY";                        
        public const string REVERSAL = "REVERSAL SUMMARY";                              
        public const string EMV_DECLINED = "EMV OFFLINE DECLINE SUMMARY";               
        public const string ATTACHMENT = "ATTACHMENT SUMMARY";                          
        public const string PROVISIONAL_VOID = "PROVISIONAL SAF VOID SUMMARY";          
        public const string DISCARDED_VOID = "DISCARDED SAF VOID SUMMARY";              

    }

    internal static class EODCommandType {
        public const string END_OF_DAY = "EOD";
        public const string REVERSAL = "Reversal";
        public const string EMV_OFFLINE_DECLINE = "EMVOfflineDecline";
        public const string TRANSACTION_CERTIFICATE = "TransactionCertificate";
        public const string ATTACHMENT = "Attachment";
        public const string SENDSAF = "SendSAF";
        public const string BATCH_CLOSE = "BatchClose";
        public const string HEARTBEAT = "Heartbeat";
        public const string EMV_PARAMETER_DOWNLOAD = "EMVPDL";
        public const string EMV_CRYPTOGRAM_TYPE = "EMVTC";
        public const string GET_BATCH_REPORT = "GetBatchReport";
        public const string GET_SAF_REPORT = "GetSAFReport";
    }

    internal static class CardSummaryType {
        public const string VISA = "VISA CARD SUMMARY";
        public const string MC = "MASTERCARD CARD SUMMARY";
        public const string AMEX = "AMERICAN EXPRESS CARD SUMMARY";
        public const string DISCOVER = "DISCOVER CARD SUMMARY";
        public const string PAYPAL = "PAYPAL CARD SUMMARY";
    }

    public enum SendFileType {
        Banner,
        Logo
    }

    public enum AuthenticationRequestType {
        PAYMENT_TRANSACTION,
        RECURRING_TRANSACTION,
        INSTALLMENT_TRANSACTION,
        ADD_CARD,
        MAINTAIN_CARD,
        CARDHOLDER_VERIFICATION,
    }

    public enum ChallengeWindowSize {
        WINDOWED_250X400,
        WINDOWED_390X400,
        WINDOWED_500X600,
        WINDOWED_600X400,
        FULL_SCREEN
    }

    public enum ColorDepth {
        ONE_BIT,
        TWO_BITS,
        FOUR_BITS,
        EIGHT_BITS,
        FIFTEEN_BITS,
        SIXTEEN_BITS,
        TWENTY_FOUR_BITS,
        THIRTY_TWO_BITS,
        FORTY_EIGHT_BITS
    }

    public enum Environment {
        QA,
        TEST,
        PRODUCTION
    }

    public enum MessageCategory {
        PAYMENT_AUTHENTICATION,
        NON_PAYMENT_AUTHENTICATION
    }

    public static class MessageVersion {
        public const string VERSION_210 = "2.1.0";
    }

    public enum MethodUrlCompletion {
        YES,
        NO,
        UNAVAILABLE
    }

    public enum Secure3dVersion {
        None,
        One,
        Two,
        Any
    }
    
    public enum EbtCardType {
        CashBenefit,	
        FoodStamp	
    }	

    public enum BatchCloseType {	
        Forced,	
        EndOfShift,	
        //EndOfDay	
    }

    public enum ReportOutput {
        Print,
        ReturnData
    }

    public enum LanguageEnum {
        [Description("en-US")]
        En_Us,
        [Description("en-CA")]
        En_Ca,
        [Description("fr-CA")]
        Fr_Ca,
        [Description("en-AU")]
        En_Au,
        [Description("en-NZ")]
        En_Nz,
        [Description("en-GB")]
        En_Gb
    }

    public enum CountryCode {
        [Description("840")]
        Country_840,
        [Description("124")]
        Country_124,
        [Description("036")]
        Country_036,
        [Description("554")]
        Country_554,
        [Description("826")]
        Country_826
    }

    public enum EcomIndicator {
        [Description("1")]
        Indicator1,
        [Description("2")]
        Indicator2,
        [Description("3")]
        Indicator3,
        [Description("5")]
        Indicator5,
        [Description("7")]
        Indicator7
    }
}
