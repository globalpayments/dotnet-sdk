using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GlobalPayments.Api.Entities.OnlineBoarding {
    public class BoardingApplication {
        public MerchantInfo MerchantInfo { get; set; }
        public LegalInfo LegalInfo { get; set; }
        public Headquarters Headquarters { get; set; }
        public BusinessInfo BusinessInfo { get; set; }
        public SaleMethods SalesMethods { get; set; }
        public ProcessingMethod ProcessingMethod { get; set; }
        public FutureDeliveryInfo FutureDeliveryInfo { get; set; }
        public GolfIndustry GolfIndustry { get; set; }
        public SalonIndustry SalonIndustry { get; set; }
        public LodgingResportIndustry LodgingResortInfo { get; set; }
        public TransactionInfo TransactionInfo { get; set; }
        public EquipmentInfo EquipmentInfo { get; set; }
        public ShippingOptions ShippingOptions { get; set; }
        public DepositOptions DepositOptions { get; set; }
        public StatementOptions StatementOptions { get; set; }
        public DisputeOptions DisputeOptions { get; set; }
        public List<OwnerOfficer> OwnerOfficers { get; set; }
        public BankingInfo BankingInfo { get; set; }

        // helpers
        public bool LegalInfoSameAsMerchant { get; set; }

        internal BoardingApplication() {
            OwnerOfficers = new List<OwnerOfficer>();
        }

        internal MultipartForm BuildForm() {
            var form = new MultipartForm(false);

            PopulateForm(form, MerchantInfo);
            if (LegalInfoSameAsMerchant) {
                LegalInfo = new LegalInfo {
                    CorporateName = MerchantInfo?.MerchantDbaName,
                    CorporateStreet = MerchantInfo?.MerchantStreet,
                    CorporateCity = MerchantInfo?.MerchantCity,
                    CorporateStatesSelect = MerchantInfo?.MerchantStatesSelect,
                    CorporateZip = MerchantInfo?.MerchantZip
                };
            }
            PopulateForm(form, LegalInfo);
            PopulateForm(form, BusinessInfo);

            // OPTIONALS
            Headquarters?.PopulateForm(form);
            SalesMethods?.PopulateForm(form);
            ProcessingMethod?.PopulateForm(form);
            FutureDeliveryInfo?.PopulateForm(form);
            GolfIndustry?.PopulateForm(form);
            SalonIndustry?.PopulateForm(form);
            LodgingResortInfo?.PopulateForm(form);
            TransactionInfo?.PopulateForm(form);
            EquipmentInfo?.PopulateForm(form);
            ShippingOptions?.PopulateForm(form);
            DepositOptions?.PopulateForm(form);
            StatementOptions?.PopulateForm(form);
            DisputeOptions?.PopulateForm(form);

            // owners/officers
            for (int i = 0; i < 10; i++) {
                OwnerOfficer owner = new OwnerOfficer();
                if(i < OwnerOfficers.Count)
                    owner = OwnerOfficers[i];

                owner.Prefix = string.Format("OwnerOfficer{0}_", i + 1);
                PopulateForm(form, owner);

                // signers
                form.Set(string.Format("Signer{0}Email", i + 1), owner.EmailAddress ?? " ");
                form.Set(string.Format("Signer{0}FullName", i + 1), owner.FullName ?? " ");
            }

            // banking info
            PopulateForm(form, BankingInfo);
            if (BankingInfo != null) {
                foreach (var account in BankingInfo.BankAccounts) {
                    account.Prefix = string.Format("MerchantAccount{0}_", BankingInfo.BankAccounts.IndexOf(account) + 1);
                    account.PopulateForm(form);
                }
            }

            return form;
        }

        internal List<string> ProcessValidationResult(string json) {
            var response = JsonDoc.Parse(json);
            var validationErrors = new List<string>();

            MerchantInfo?.ProcessValidation(response, validationErrors);
            LegalInfo?.ProcessValidation(response, validationErrors);
            BusinessInfo?.ProcessValidation(response, validationErrors);

            // OPTIONALS
            Headquarters?.ProcessValidation(response, validationErrors);
            SalesMethods?.ProcessValidation(response, validationErrors);
            ProcessingMethod?.ProcessValidation(response, validationErrors);
            FutureDeliveryInfo?.ProcessValidation(response, validationErrors);
            GolfIndustry?.ProcessValidation(response, validationErrors);
            SalonIndustry?.ProcessValidation(response, validationErrors);
            LodgingResortInfo?.ProcessValidation(response, validationErrors);
            TransactionInfo?.ProcessValidation(response, validationErrors);
            EquipmentInfo?.ProcessValidation(response, validationErrors);
            ShippingOptions?.ProcessValidation(response, validationErrors);
            DepositOptions?.ProcessValidation(response, validationErrors);
            StatementOptions?.ProcessValidation(response, validationErrors);
            DisputeOptions?.ProcessValidation(response, validationErrors);

            // owners/officers
            for (int i = 0; i < 10; i++) {
                if (i < OwnerOfficers.Count) {
                    var owner = OwnerOfficers[i];
                    owner.Prefix = string.Format("OwnerOfficer{0}_", i + 1);
                    owner.ProcessValidation(response, validationErrors);
                }
                else continue;
            }

            // banking info
            BankingInfo?.ProcessValidation(response, validationErrors);
            if (BankingInfo != null) {
                foreach (var account in BankingInfo.BankAccounts) {
                    account.Prefix = string.Format("MerchantAccount{0}_", BankingInfo.BankAccounts.IndexOf(account) + 1);
                    account.ProcessValidation(response, validationErrors);
                }
            }

            return validationErrors;
        }

        private void PopulateForm<T>(MultipartForm form, T element) where T : FormElement {
            if (element == null)
                element = Activator.CreateInstance<T>();
            element.PopulateForm(form);
        }
    }

    public abstract class FormElement {
        internal string Prefix = string.Empty;
        
        public virtual void PopulateForm(MultipartForm form) {
            foreach (var propInfo in GetType().GetRuntimeProperties()) {
                var fieldName = string.Format("{0}{1}", Prefix, propInfo.Name);
                
                var value = propInfo.GetValue(this);
                if (value == null)
                    form.Set(fieldName, DefaultForType(propInfo));
                else {
                    if(value is Enum) {
                        var description = value.GetType().GetRuntimeField(value.ToString()).GetCustomAttribute<DescriptionAttribute>();
                        form.Set(fieldName, description?.Description);
                    }
                    else form.Set(fieldName, value.ToString());
                }
            }
        }

        public virtual void ProcessValidation(JsonDoc doc, List<string> validationErrors) {
            foreach (var propInfo in GetType().GetRuntimeProperties()) {
                var fieldName = string.Format("{0}{1}", Prefix, propInfo.Name);

                var validations = doc.GetArray<string>(fieldName) as List<string>;
                if (validations != null) {
                    if (validations.Contains(string.Format("Application configuration field {0} within the application configuration is not set to be editable (shown).", fieldName)))
                        continue;

                    if (validations.Contains("This field is required."))
                        validationErrors.Add(string.Format("{0} is required.", fieldName));
                    else {
                        var propValue = propInfo.GetValue(this)?.ToString();
                        if (!string.IsNullOrWhiteSpace(propValue)) {
                            continue;
                            // handle validations on a per field basis?
                        }
                        continue;
                    }
                }
            }
        }

        private string DefaultForType(PropertyInfo prop) {
            if (prop.PropertyType == typeof(DateTime?))
                return "__/__/____";
            else if (prop.Name.EndsWith("Select"))
                return "(select)";
            else return " ";
        }
    }

    // Business (DBA) Info
    public class MerchantInfo : FormElement {
        public string AffiliatePartnerId { get; set; }
        public string RelationshipManagerName { get; set; }
        public string RelationshipManagerPhone { get; set; }
        public string MerchantDbaName { get; set; }
        public string MerchantStreet { get; set; }
        public string MerchantCity { get; set; }
        public States? MerchantStatesSelect { get; set; }
        public string MerchantZip { get; set; }
        public string MerchantEmail { get; set; }
        public string MerchantEmailFirstName { get; set; }
        public string MerchantEmailLastName { get; set; }
        public string MerchantPrimaryContactName { get; set; }
        public string MerchantPrimaryContactPhone { get; set; }
        public PhoneTypeSelect? PrimaryContactPhoneTypeSelect { get; set; }
        public string MerchantSecondaryContactName { get; set; }
        public string MerchantSecondaryContactPhone { get; set; }
        public PhoneTypeSelect? SecondaryContactPhoneTypeSelect { get; set; }
        public string MerchantStoreNumber { get; set; }
        public int? MerchantNumberOfLocations { get; set; }
        public string MerchantPhone { get; set; }
        public string MerchantFax { get; set; }
        public string MerchantCustomerServiceNumber { get; set; }
        public string MerchantWebsiteAddress { get; set; }
        public string FederalTaxId { get; set; }
    }

    // Legal Info
    public class LegalInfo : FormElement {
        public string CorporateName { get; set; }
        public string CorporateStreet { get; set; }
        public string CorporateCity { get; set; }
        public States? CorporateStatesSelect { get; set; }
        public string CorporateZip { get; set; }
        public string CorporatePhone { get; set; }
        public string CorporateFax { get; set; }
    }

    // Headquarters
    public class Headquarters : FormElement {
        public string HeadquartersName { get; set; }
        public string HeadquartersStreet { get; set; }
        public string HeadquartersCity { get; set; }
        public States? HeadquartersStatesSelect { get; set; }
        public string HeadquartersZip { get; set; }
        public string HeadquartersPhone { get; set; }
        public string HeadquartersFax { get; set; }
    }

    // About Your Business
    public class BusinessInfo : FormElement {
        public PrivatePublic? BusinessTypeSelect { get; set; }
        public TypeofOwnershipSelect? OwnershipTypeSelect { get; set; }
        public CorporationDisregardedEn? IrsReportingClassificationForLlcOptionSelect { get; set; }
        public bool IsFederalIdSignersSsn { get; set; }
        public bool DataCompromiseOrComplianceInvestigation { get; set; }
        public DateTime? DateOfCompromise { get; set; }
        public string IsBusinessPCICompliant { get; set; }
        public string ProcessOrTransmitThirdPartyCardData { get; set; }
        public string PaymentFacilitatorOrServiceProvider { get; set; }
        public string HomeBasedBusiness { get; set; }
        public string IsSignatureObtainedForReceipt { get; set; }
        public string ContractWithThirdPartyLender { get; set; }
        public string ThirdPartyContractStartDate { get; set; }
        public string ThirdPartyLengthOfContract { get; set; }
        public string ThirdPartyLoanBalance { get; set; }
        public bool EverFiledBankrupt { get; set; }
        public DateTime? DateOfBankruptcy { get; set; }
        public BusinessPersonal? BankruptcyTypeSelect { get; set; }
        public DateTime? DateBusinessStarted { get; set; }
        public DateTime? DateBusinessAcquired { get; set; }
        public string PercentSalesReturned { get; set; }
        public bool AcceptCreditCardsOnline { get; set; }
        public string OnlineTransactionsProcessedByHPS { get; set; }
        public string NameOfPaymentProcessorIfNotHPS { get; set; }
        public bool DataStorageOrMerchantServicer { get; set; }
        public string PercentOfBusinessDirectlyWithCustomers { get; set; }
        public string PercentOfBusinessDirectlyWithBusiness { get; set; }
        public string ProductsServicesProvided { get; set; }
        public string UseFulfillmentHouse { get; set; }
        public string RefundPolicy { get; set; }
        public string TimeUntilCardIsCharged { get; set; }
        public string ProcessForAgeRestrictionPurchases { get; set; }
        public bool AcceptCreditCards { get; set; }

        // hidden fields dependent on AcceptCreditCards
        public DateTime? DateAcceptingCreditCardsStarted { get; set; }
        public string PercentSalesChargebacks { get; set; }
        public string CurrentProcessor { get; set; }
        public string CurrentMID { get; set; }
        public bool SeasonalMerchant { get; set; }
        public bool OperatesInJanuary { get; set; }
        public bool OperatesInMay { get; set; }
        public bool OperatesInSeptember { get; set; }
        public bool OperatesInFebruary { get; set; }
        public bool OperatesInJune { get; set; }
        public bool OperatesInOctober { get; set; }
        public bool OperatesInMarch { get; set; }
        public bool OperatesInJuly { get; set; }
        public bool OperatesInNovember { get; set; }
        public bool OperatesInApril { get; set; }
        public bool OperatesInAugust { get; set; }
        public bool OperatesInDecember { get; set; }
    }

    // Sales Method
    public class SaleMethods : FormElement {
        public string SalesMethodOnPremiseFaceToFaceSales { get; set; }
        public string SalesMethodOffPremiseFaceToFaceSales { get; set; }
        public string SalesMethodMailOrderSales { get; set; }
        public string SalesMethodRealTimeInternetSales { get; set; }
        public string SalesMethodInboundTelephoneSales { get; set; }
        public string SalesMethodOutboundTelephoneSales { get; set; }
        public string SalesMethodInternetKeyed { get; set; }
        public string SalesMethodRecurringBilling { get; set; }
    }

    // Processing Method
    public class ProcessingMethod : FormElement {
        public string ProcessingMethodCardSwiped { get; set; }
        public string ProcessingMethodKeyedWithImprint { get; set; }
        public string ProcessingMethodKeyedWithoutImprint { get; set; }
        public string ProcessingMethodMotoDomesticTransactions { get; set; }
        public string ProcessingMethodMotoForeignTransactions { get; set; }
        public string ProcessingMethodPercentOfGiftCardSales { get; set; }
    }

    // Future Delivery
    public class FutureDeliveryInfo : FormElement {
        public string FutureDelivery { get; set; }
        public string FutureDeliveryTwoFive { get; set; }
        public string FutureDeliverySixTen { get; set; }
        public string FutureDeliveryElevenThirty { get; set; }
        public string FutureDeliveryThirtyOneSixty { get; set; }
        public string FutureDeliverSixtyOneNinety { get; set; }
        public string FutureDeliveryNinetyOneHundredTwenty { get; set; }
        public string FutureDeliveryMoreThanHundredTwenty { get; set; }
        public string PercentBankCardVolumeFutureDeliver { get; set; }
    }

    // Golf Industry
    public class GolfIndustry : FormElement {
        public string GolfPercentPublic { get; set; }
        public string GolfPercentPrivate { get; set; }
        public string GolfPercentMembership { get; set; }
        public string GolfPercentProShop { get; set; }
        public string GolfPercentRestaurantOther { get; set; }
    }

    // Salon Industry
    public class SalonIndustry : FormElement {
        public string SalonPercentHairNailsFacials { get; set; }
        public string SalonPercentMassages { get; set; }
        public string SalonPercentSpaPackages { get; set; }
    }

    // Lodging/Resort Industry
    public class LodgingResportIndustry : FormElement {
        public string LodgingPercentLodging { get; set; }
        public string LodgingPercentMeetinCatering { get; set; }
        public string LodgingPercentOther { get; set; }
    }

    // Transaction Info
    public class TransactionInfo : FormElement {
        public string AnnualVolume { get; set; }
        public string AverageTicket { get; set; }
        public string HighTicket { get; set; }
        public string HighTicketFrequency { get; set; }
        public string AchAnnualVolume { get; set; }
        public string AchAverageTicket { get; set; }
        public string AmexOptOut { get; set; }
        public string AmexMerchantNumber { get; set; }
        public string AmexOptBlue { get; set; }
        public string AmexAnnualVolume { get; set; }
        public string AmexAverageTicket { get; set; }
        public string AmexMarketingMaterialOptOut { get; set; }
        public string DiscoverOptOut { get; set; }
        public string PaypalOptOut { get; set; }
        public string OnePoint { get; set; }
        //@Model.RenderField(WellKnownFieldNames.ProcessingStatement)
    }

    // Equipment Setup Info
    public class EquipmentInfo : FormElement {
        public string InfoCentralEmailAddress { get; set; }
        public string GatewayIndustrySelect { get; set; }
        public string EquipmentIndustrySelect { get; set; }
        public YesNo? GatewayDeviceTypeSelect { get; set; }
        public string GatewayVersionSelect { get; set; }
        public string GatewayTimeZoneSelect { get; set; }
        public YesNo? GatewayAutoCloseSelect { get; set; }
        public YesNoDataSecurity? GatewayAutoCloseTimeHour { get; set; }
        public AMPM? GatewayAutoCloseAmPmSelect { get; set; }
        public string DeveloperName { get; set; }
        public string DeveloperEmail { get; set; }
        //@Model.RenderField(WellKnownFieldNames.ShoppingCartPlugin)
        //@Model.RenderField(WellKnownFieldNames.ShoppingCartPlugin_Other)
        //@Model.RenderField(WellKnownFieldNames.SecureCustomSoftware)
        //@Model.RenderField(WellKnownFieldNames.SecureCustomSoftware_Other)
        //@Model.RenderField(WellKnownFieldNames.MobilePaymentsOptIn)
        //@Model.RenderField(WellKnownFieldNames.MobuyleDeviceTypeSelect)
        //@Model.RenderField(WellKnownFieldNames.MobuylePhoneModel)
        //@Model.RenderField(WellKnownFieldNames.MobuyleOsVersion)
        public string FraudScreeningOptOut { get; set; }
        public string PaypalThroughGlobalPaymentsOptOut { get; set; }
        public string MasterpassThroughGlobalPaymentsOptOut { get; set; }
        public string AVSDeclineAll { get; set; }
        public string MerchantEquipmentQuantity1 { get; set; }
    }

    // Shipping options
    public class ShippingOptions : FormElement {
        public string GatewayAddPrinter { get; set; }
        public string EquipmentGrandTotalDueToHps { get; set; }
    }

    // Deposit Options
    public class DepositOptions : FormElement {
        public DepositMethodOptions? DepositMethodTypeSelect { get; set; }
        public SettlementTypeSelect? SettlementOptionSelect { get; set; }
    }

    // Statement Options
    public class StatementOptions : FormElement {
        public DBALegalElectronic? StatementMailDestinationOptionSelect { get; set; }
        public string StatementInfoCentralOrPreferredEmail { get; set; }
        public string StatementPreferredEmailAddress { get; set; }
    }

    // Dispute Letters
    public class DisputeOptions : FormElement {
        public DBALegal? MailingOptionSelect { get; set; }
        public EmailFax? ElectronicOptionSelect { get; set; }
    }

    class UnknownFields {
        /*These do not have a header yet - if they become visible, will deal with that then. */
        public ByBatchByCardTypeStand? StatementOptionSelect { get; set; }
        public AutomatedFuelAFDConve? InterchangeQualificationTypeSelect { get; set; }
        public AllCardsAcceptedConsume? CardAcceptanceTypeSelect { get; set; }
        public string RecurringMinimumDiscountFee { get; set; }
        public string RecurringServiceRegulatoryFee { get; set; }
        public string RecurringAnnualFee { get; set; }
        public string RecurringChargebackFee { get; set; }
        public string ReccuringVoiceAuthFee { get; set; }
    }

    // Owners/Officers
    public class OwnerOfficer : FormElement {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName {
            get {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }
        public string Title { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public OwnerOfficerSelect? OwnershipTypeSelect { get; set; }
        public string EquityOwnership { get; set; }
        public string EmailAddress { get; set; }
        public string LengthAtHomeAddress { get; set; }
        public string DriversLicenseNumber { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }
        public string SSN { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public States? StateSelect { get; set; }
        public string Zip { get; set; }
    }

    // Banking Info
    public class BankingInfo : FormElement {
        public string FuelSupplierCompany { get; set; }
        public string BankName { get; set; }
        public string BankPhone { get; set; }
        public string BankStreet { get; set; }
        public string BankCity { get; set; }
        public States? BankStatesSelect { get; set; }
        public string BankZip { get; set; }
        public List<BankAccount> BankAccounts { get; set; }

        public BankingInfo() {
            BankAccounts = new List<BankAccount>();
        }
    }

    // Bank Account
    public class BankAccount : FormElement {
        public string AccountNumber { get; set; }
        public BankAccountTypeSelect? AccountTypeSelect { get; set; }
        public string TransitRouterAbaNumber { get; set; }
        public FundsTransferMethodSelect? TransferMethodTypeSelect { get; set; }
        // TODO: Check Upload
    }

    #region enums
    public class DescriptionAttribute : Attribute {
        public string Description { get; set; }

        public DescriptionAttribute(string description) {
            Description = description;
        }
    }

    public enum OwnerOfficerSelect {
        [Description("Owner")]
        Owner,
        [Description("Officer")]
        Officer,
        [Description("AuthorizedSigner")]
        AuthorizedSigner
    }

    public enum PrivatePublic {
        [Description("Private")]
        Private,
        [Description("Public")]
        Public
    }

    public enum TypeofOwnershipSelect {
        [Description("Sole Proprietorship")]
        SoleProprietorship,
        [Description("Government")]
        GovernmentMunicipality,
        [Description("Partnership")]
        Partnership,
        [Description("Corporation")]
        Corporation,
        [Description("LLC")]
        LLC,
        [Description("Non-Profit")]
        NonProfit
    }

    public enum CorporationDisregardedEn {
        [Description("Corporation")]
        Corporation,
        [Description("Disregarded Entity")]
        DisregardedEntitySingleMemberLLC,
        [Description("Partnership")]
        Partnership
    }

    public enum SettlementTypeSelect {
        [Description("DailySplit")]
        DailySplit,
        [Description("Monthly")]
        MonthlyBilling,
        [Description("DailyNet")]
        DailyNet
    }

    public enum AllCardsAcceptedConsume {
        [Description("1")]
        AllCardsAccepted,
        [Description("Consumer Prepaid/Debit (Check Cards) Only")]
        ConsumerPrepaidDebitCheckCardsOnly,
        [Description("Credit/Business Cards Only")]
        CreditBusinessCardsOnly
    }

    public enum AutomatedFuelAFDConve {
        [Description("Automated Fuel (AFD)")]
        AutomatedFuelAFD,
        [Description("Convenience")]
        Convenience,
        [Description("DialPay/TT")]
        DialPayTT,
        [Description("Emerging Market")]
        EmergingMarket,
        [Description("GSA Large Ticket")]
        GSALargeTicket,
        [Description("Interchange Plus")]
        InterchangePlus,
        [Description("Lodging/Car Rental")]
        LodgingCarRental,
        [Description("MOTO / Internet")]
        MOTOInternet,
        [Description("Purchase Card Level 1")]
        PurchaseCardLevel1,
        [Description("Purchase Card Level 2")]
        PurchaseCardLevel2,
        [Description("Purchase Card Level 3")]
        PurchaseCardLevel3,
        [Description("Restaurant")]
        Restaurant,
        [Description("Retail")]
        Retail,
        [Description("Service Station (NFD)")]
        ServiceStationNFD,
        [Description("Small Ticket/Convenience Purchase")]
        SmallTicketConveniencePurchase,
        [Description("Small Ticket/M3")]
        SmallTicketM
    }

    public enum ByBatchByCardTypeStand {
        [Description("By Batch")]
        ByBatch,
        [Description("By Card Type")]
        ByCardType,
        [Description("Chain Recap Summary")]
        ChainRecapSummary,
        [Description("Non-Qual Breakout")]
        NonQualBreakout,
        [Description("Standard")]
        Standard
    }

    public enum AllElectronicCommunicat {
        [Description("All Electronic ")]
        AllElectronicCommunications,
        [Description("DBA")]
        DBA,
        [Description("Legal")]
        Legal,
        [Description("Supressed Statements")]
        SupressedStatements
    }

    public enum EmailFax {
        [Description("Email")]
        Email,
        [Description("Fax")]
        Fax
    }

    public enum PhoneTypeSelect {
        [Description("DBA")]
        DBA,
        [Description("Home")]
        Home,
        [Description("Cell")]
        Cell
    }

    public enum BusinessPersonal {
        [Description("Business")]
        Business,
        [Description("Personal")]
        Personal
    }

    public enum ECommerceNormalSelfServ {
        [Description("E-commerce")]
        ECommerce,
        [Description("Normal")]
        Normal,
        [Description("Self Service")]
        SelfService,
        [Description("Self Service Limited")]
        SelfServiceLimitedAmount
    }

    public enum Timezones {
        [Description("Alaska")]
        Alaska,
        [Description("Arizona")]
        Arizona,
        [Description("Central")]
        Central,
        [Description("Eastern")]
        Eastern,
        [Description("Hawaii")]
        Hawaii,
        [Description("Mountain")]
        Mountain,
        [Description("Pacific")]
        Pacific
    }

    public enum YesNo {
        [Description("Yes")]
        Yes,
        [Description("No")]
        No
    }

    public enum AMPM {
        [Description("AM")]
        AM,
        [Description("PM")]
        PM
    }

    public enum BankAccountTypeSelect {
        [Description("Checking")]
        Checking,
        [Description("Savings")]
        Savings,
        [Description("GL")]
        Other
    }

    public enum AndroidiOS {
        [Description("Android")]
        Android,
        [Description("iOS")]
        iOS
    }

    public enum FundsTransferMethodSelect {
        [Description("Deposits & Fees")]
        DepositsAndFees,
        [Description("Deposits Only - (Split*)")]
        DepositsOnlySplit,
        [Description("Fees")]
        Fees
    }

    public enum States {
        [Description("AL")]
        Alabama,
        [Description("AK")]
        Alaska,
        [Description("AZ")]
        Arizona,
        [Description("AR")]
        Arkansas,
        [Description("CA")]
        California,
        [Description("CO")]
        Colorado,
        [Description("CT")]
        Connecticut,
        [Description("DE")]
        Delaware,
        [Description("FL")]
        Florida,
        [Description("GA")]
        Georgia,
        [Description("HI")]
        Hawaii,
        [Description("ID")]
        Idaho,
        [Description("IL")]
        Illinois,
        [Description("IN")]
        Indiana,
        [Description("IA")]
        Iowa,
        [Description("KS")]
        Kansas,
        [Description("KY")]
        Kentucky,
        [Description("LA")]
        Louisiana,
        [Description("ME")]
        Maine,
        [Description("MD")]
        Maryland,
        [Description("MA")]
        Massachussetts,
        [Description("MI")]
        Michigan,
        [Description("MN")]
        Minnesota,
        [Description("MS")]
        Mississippi,
        [Description("MO")]
        Missouri,
        [Description("MT")]
        Montana,
        [Description("NE")]
        Nebraska,
        [Description("NV")]
        Nevada,
        [Description("NH")]
        NewHampshire,
        [Description("NJ")]
        NewJersey,
        [Description("NM")]
        NewMexico,
        [Description("NY")]
        NewYork,
        [Description("NC")]
        NorthCarolina,
        [Description("ND")]
        NorthDakota,
        [Description("OH")]
        Ohio,
        [Description("OK")]
        Oklahoma,
        [Description("OR")]
        Oregon,
        [Description("PA")]
        Pennsylvania,
        [Description("RI")]
        RhodeIsland,
        [Description("SC")]
        SouthCarolina,
        [Description("SD")]
        SouthDakota,
        [Description("TN")]
        Tennessee,
        [Description("TX")]
        Texas,
        [Description("UT")]
        Utah,
        [Description("VT")]
        Vermont,
        [Description("VA")]
        Virginia,
        [Description("WA")]
        Washington,
        [Description("DC")]
        WashingtonDistrictofColumbia,
        [Description("WV")]
        WestVirginia,
        [Description("WI")]
        Wisconsin,
        [Description("WY")]
        Wyoming
    }

    public enum DBALegalElectronic {
        [Description("All Electronic ")]
        AllElectronicCommunications,
        [Description("DBA")]
        DBA,
        [Description("Legal")]
        Legal
    }

    public enum DBALegal {
        [Description("DBA")]
        DBA,
        [Description("Legal")]
        Legal
    }

    public enum DepositMethodOptions {
        [Description("Standard")]
        Standard,
        [Description("By Batch")]
        ByBatch,
        [Description("By Card Type")]
        ByCardType
    }

    public enum InfocentralOrPreferred {
        [Description("PreferredEmail")]
        EnterPreferredEmail,
        [Description("InfoCentral")]
        UseInfoCentralEmail
    }

    public enum AmexOptBlueSelect {
        [Description("AXP")]
        Yes,
        [Description("No")]
        No
    }

    public enum AmexMarketingOptOut {
        [Description("No")]
        No,
        [Description("OptOut")]
        Yes
    }

    public enum HighTicketFrequency {
        [Description("1-3 Monthly")]
        OneToThreeMonthly,
        [Description("4-6 Monthly")]
        FourToSixMonthly,
        [Description("7-10 Monthly")]
        SevenToTenMonthly,
        [Description("4-6 Quarterly")]
        FourToSixQuarterly,
        [Description("7-10 Quarterly")]
        SevenToTenQuarterly,
        [Description("1-3 Annually")]
        OneToThreeAnnually,
        [Description("4-6 Annually")]
        FourToSixAnnually,
        [Description("7-10 Annually")]
        SevenToTenAnnually
    }

    public enum ShoppingCartPlugin {
        [Description("Magento")]
        Magento,
        [Description("WooCommerce")]
        WooCommerce,
        [Description("WordPress")]
        WordPress,
        [Description("Bigcommerce")]
        Bigcommerce,
        [Description("Shopify")]
        Shopify,
        [Description("osCommerce")]
        osCommerce,
        [Description("X-Cart")]
        XCart,
        [Description("Gravity Forms")]
        GravityForms,
        [Description("Other")]
        Other
    }

    public enum SecureCustomSoftware {
        [Description("PHP")]
        PHP,
        [Description(".Net")]
        Net,
        [Description("Java")]
        Java,
        [Description("Ruby")]
        Ruby,
        [Description("Python")]
        Python,
        [Description("nodeJS")]
        nodeJS,
        [Description("Other")]
        Other
    }

    public enum EBT {
        [Description("FoodAndCash")]
        FoodstampsAndcashbenefits,
        [Description("FoodOnly")]
        Foodstampsonly,
        [Description("CashOnly")]
        Cashbenefitsonly
    }

    public enum YesNoDataSecurity {
        [Description("Yes")]
        Yes,
        [Description("No")]
        No,
        [Description("NeverAcceptedPaymentCards")]
        Ihaveneveracceptedpaymentcards,
        [Description("N/A")]
        NA
    }
    #endregion
}
