using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.PayFac;
using GlobalPayments.Api.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GlobalPayments.Api.Builders
{
    public class PayFacBuilder<TResult> : BaseBuilder<TResult> where TResult : class
    {
        public TransactionType TransactionType { get; set; }
        public TransactionModifier TransactionModifier { get; set; }

        /// <summary>
        /// Primary Bank Account Information - Optional. Used to add a bank account to which funds can be settled
        /// </summary>
        public BankAccountData BankAccountData { get; set; }

        /// <summary>
        /// Merchant Beneficiary Owner Information - Required for all merchants validating KYC based off of personal data
        /// </summary>
        public BeneficialOwnerData BeneficialOwnerData { get; set; }

        /// <summary>
        /// User for Portico Device Ordering. Must set TimeZone property as well when ordering Portico devices
        /// </summary>
        public DeviceData DeviceData { get; set; }

        /// <summary>
        /// Required for partners ordering Portico devices. Valid values: [ UTC, PT, MST, MT, CT, ET, HST, AT, AST, AKST, ACT, EET, EAT, MET, NET, PLT, IST, BST, VST, CTT, JST, ACT, AET, SST, NST, MIT, CNT, AGT, CAT ]
        /// </summary>
        public string TimeZone { get; set; }
       
        public PaymentMethodName? PaymentMethodName { get; set; }
      
        public PaymentMethodType? PaymentMethodType { get; set; }

        public string Currency { get; set; }

        public string ClientTransactionId{ get; set; }

        /// <summary>
        /// Business Data - Required for business validated accounts. May also be required for personal validated accounts
        /// </summary>
        public BusinessData BusinessData { get; set; }

        /// <summary>
        /// Significant Owner Information - May be required for some partners based on ProPay Risk decision
        /// </summary>
        public SignificantOwnerData SignificantOwnerData { get; set; }

        /// <summary>
        /// Threat Risk Assessment Information - May be required based on ProPay Risk decision
        /// </summary>
        public ThreatRiskData ThreatRiskData { get; set; }

        /// <summary>
        /// User/Merchant Personal Data
        /// </summary>
        public UserPersonalData UserPersonalData { get; set; }

        public CreditCardData CreditCardInformation { get; set; }
        public BankAccountData ACHInformation { get; set; }
        public Address MailingAddressInformation { get; set; }
        public BankAccountData SecondaryBankInformation { get; set; }
        public GrossBillingInformation GrossBillingInformation { get; set; }
        public string NegativeLimit { get; set; }
        public RenewAccountData RenewalAccountData { get; set; }
        public string AccountNumber { get; set; }
        public string Password { get; set; }
        public AccountPermissions AccountPermissions { get; set; }
        public BankAccountOwnershipData PrimaryBankAccountOwner { get; set; }
        public BankAccountOwnershipData SecondaryBankAccountOwner { get; set; }
        public DocumentUploadData DocumentUploadData { get; set; }
        public SSORequestData SSORequestData { get; set; }
        public OrderDevice OrderDevice { get; set; }

        public string Amount { get; set; }
        public string ReceivingAccountNumber { get; set; }
        public bool? AllowPending { get; set; }
        public string CCAmount { get; set; }
        public bool? RequireCCRefund { get; set; }
        public string TransNum { get; set; }

        public string ExternalID { get; set; }
        public string SourceEmail { get; set; }       
        public string Description { get; set; }
        public string GatewayTransactionId { get; set; }
        public string GlobaltransId { get; set; }
        public string GlobalTransSource { get; set; }
        public string CardBrandTransactionId { get; set; }

        public List<Product> ProductData { get; set; }
        public List<Person> PersonsData { get; set; }
        
        public int Page { get; set; }    
        public int PageSize { get; set; }    
        public PaymentStatistics PaymentStatistics { get; set; }    
        public StatusChangeReason? StatusChangeReason { get; set; }   
        public UserReference UserReference { get; set; }    
        public Dictionary<string, PaymentMethodFunction?> PaymentMethodsFunctions { get; set; }
        public string IdempotencyKey { get; set; }
        public Dictionary<AddressType, Address> Addresses { get; set; }

        public FlashFundsPaymentCardData FlashFundsPaymentCardData { get; set; }

        public PayFacBuilder(TransactionType type, TransactionModifier modifer = TransactionModifier.None) {
            TransactionType = type;
            TransactionModifier = modifer;
        }

        public override TResult Execute(string configName = "default") {
            base.Execute(configName);

            var client = ServicesContainer.Instance.GetPayFac(configName);           

            if (client.HasBuiltInMerchantManagementService) {
                return client.ProcessBoardingUser(this);
            }      

            return client.ProcessPayFac(this);
        }

        protected override void SetupValidations() {
           
            #region ENUM VALIDATION WITH FLAG ATTRIBUTE     
            /// TO ADD
            #endregion

            // Account Management Methods
            Validations.For(TransactionType.Create)
                .With(TransactionModifier.None)
                .Check(() => BeneficialOwnerData).IsNotNull()
                .Check(() => BusinessData).IsNotNull()
                .Check(() => UserPersonalData).IsNotNull()
                .Check(() => CreditCardInformation).IsNotNull();

            Validations.For(TransactionType.Edit)
                .With(TransactionModifier.None)
                .Check(() => AccountNumber).IsNotNull();

            Validations.For(TransactionType.ResetPassword)
                .With(TransactionModifier.None)
                .Check(() => AccountNumber).IsNotNull();

            Validations.For(TransactionType.RenewAccount)
                .With(TransactionModifier.None)
                .Check(() => AccountNumber).IsNotNull();

            Validations.For(TransactionType.UpdateBeneficialOwnership)
                .With(TransactionModifier.None)
                .Check(() => AccountNumber).IsNotNull()
                .Check(() => BeneficialOwnerData).IsNotNull();

            Validations.For(TransactionType.Deactivate)
                .With(TransactionModifier.None)
                .Check(() => AccountNumber).IsNotNull();

            Validations.For(TransactionType.UploadDocumentChargeback)
                .With(TransactionModifier.None)
                .Check(() => AccountNumber).IsNotNull()
                .Check(() => DocumentUploadData).IsNotNull();

            Validations.For(TransactionType.UploadDocument)
                .With(TransactionModifier.None)
                .Check(() => AccountNumber).IsNotNull()
                .Check(() => DocumentUploadData).IsNotNull();

            Validations.For(TransactionType.UploadDocument)
               .With(TransactionModifier.Merchant)               
               .Check(() => DocumentUploadData).IsNotNull()
               .Check(() => DocumentUploadData).PropertyOf(nameof(DocumentUploadData.DocType)).IsNotNull();


            Validations.For(TransactionType.ObtainSSOKey)
                .With(TransactionModifier.None)
                .Check(() => AccountNumber).IsNotNull()
                .Check(() => SSORequestData).IsNotNull();

            Validations.For(TransactionType.UpdateBankAccountOwnership)
                .With(TransactionModifier.None)
                .Check(() => AccountNumber).IsNotNull();

            // Funds Management Methods
            Validations.For(TransactionType.AddFunds)
                .With(TransactionModifier.None)
                .Check(() => AccountNumber).IsNotNull()
                .Check(() => Amount).IsNotNull();

            Validations.For(TransactionType.SweepFunds)
                .With(TransactionModifier.None)
                .Check(() => AccountNumber).IsNotNull()
                .Check(() => Amount).IsNotNull();

            Validations.For(TransactionType.AddCardFlashFunds)
                .With(TransactionModifier.None)
                .Check(() => AccountNumber).IsNotNull()
                .Check(() => FlashFundsPaymentCardData).IsNotNull();

            Validations.For(TransactionType.PushMoneyFlashFunds)
                .With(TransactionModifier.None)
                .Check(() => AccountNumber).IsNotNull()
                .Check(() => Amount).IsNotNull();

            // In-Network Transaction Methods
            Validations.For(TransactionType.DisburseFunds)
                .With(TransactionModifier.None)
                .Check(() => Amount).IsNotNull()
                .Check(() => ReceivingAccountNumber).IsNotNull();

            Validations.For(TransactionType.SpendBack)
                .With(TransactionModifier.None)
                .Check(() => Amount).IsNotNull()
                .Check(() => AccountNumber).IsNotNull()
                .Check(() => ReceivingAccountNumber).IsNotNull()
                .Check(() => AllowPending).IsNotNull();

            Validations.For(TransactionType.Reversal)
                .With(TransactionModifier.None)
                .Check(() => AccountNumber).IsNotNull()
                .Check(() => Amount).IsNotNull()
                .Check(() => CCAmount).IsNotNull()
                .Check(() => RequireCCRefund).IsNotNull()
                .Check(() => TransNum).IsNotNull();

            Validations.For(TransactionType.SplitFunds)
                .With(TransactionModifier.None)
                .Check(() => AccountNumber).IsNotNull()
                .Check(() => ReceivingAccountNumber).IsNotNull()
                .Check(() => Amount).IsNotNull();
                //.Check(() => TransNum).IsNotNull();

            // Get Information Methods

            // For GetAccountDetails, there are "required" 3 parameter types
            // But we can only send one type per request. If we send more than one, the method will fail
            // As a result, we need to do some more thorough validation work
            // Each .When must be paired with a .Check, hence the duplicate .When calls in each section below
            
            // Check if AccountNumber is passed in (and ExternalID/SourceEmail have not)
            Validations.For(TransactionType.GetAccountDetails)
                .With(TransactionModifier.None)
                .When(() => AccountNumber).IsNotNull()
                .Check(() => ExternalID).IsNull()
                .When(() => AccountNumber).IsNotNull()
                .Check(() => SourceEmail).IsNull();
            // Check if ExternalID has been passed in (and AccountNumber/SourceEmail have not)
            Validations.For(TransactionType.GetAccountDetails)
                .With(TransactionModifier.None)
                .When(() => ExternalID).IsNotNull()
                .Check(() => SourceEmail).IsNull()
                .When(() => ExternalID).IsNotNull()
                .Check(() => AccountNumber).IsNull();
            // Check if SourceEmail has been passed in (and AccountNumber/ExternalID have not)
            Validations.For(TransactionType.GetAccountDetails)
                .With(TransactionModifier.None)
                .When(() => SourceEmail).IsNotNull()
                .Check(() => ExternalID).IsNull()
                .When(() => SourceEmail).IsNotNull()
                .Check(() => AccountNumber).IsNull();

            Validations.For(TransactionType.GetAccountDetails)
                .With(TransactionModifier.Additional)
                .Check(() => AccountNumber).IsNotNull();

            Validations.For(TransactionType.Balance)
                .With(TransactionModifier.None)
                .Check(() => AccountNumber).IsNotNull();

            Validations.For(TransactionType.Create)
                .With(TransactionModifier.Merchant)
                .Check(() => UserPersonalData).IsNotNull();           

            Validations.For(TransactionType.Fetch)
                .With(TransactionModifier.Merchant)
                .Check(() => UserReference).PropertyOf(nameof(UserReference.UserId)).IsNotNull();

            Validations.For(TransactionType.Edit)
                .With(TransactionModifier.Merchant)
                .Check(() => UserReference).PropertyOf(nameof(UserReference.UserId)).IsNotNull();

            Validations.For(TransactionType.Edit)                
                .With(TransactionModifier.Account)
                .Check(() => AccountNumber).IsNotNull();
        }

        public PayFacBuilder<TResult> WithBankAccountData(BankAccountData bankAccountData, PaymentMethodFunction? paymentMethodFunction = null) {
            BankAccountData = bankAccountData;
            if (paymentMethodFunction != null) {
                if (PaymentMethodsFunctions == null) {
                    PaymentMethodsFunctions = new Dictionary<string, PaymentMethodFunction?>();
                }
                PaymentMethodsFunctions.Add(bankAccountData.GetType().Name, paymentMethodFunction.Value);
            }            
            return this;
        }

        public PayFacBuilder<TResult> WithBeneficialOwnerData(BeneficialOwnerData beneficialOwnerData) {
            BeneficialOwnerData = beneficialOwnerData;
            return this;
        }

        public PayFacBuilder<TResult> WithDeviceData(DeviceData deviceData) {
            DeviceData = deviceData;
            return this;
        }

        public PayFacBuilder<TResult> WithDescription(string description) {
            Description = description;
            return this;
        }

        public PayFacBuilder<TResult> WithProductData(List<Product> productData) {
            ProductData = productData;
            return this;
        }
        
        public PayFacBuilder<TResult> WithPersonsData(List<Person> personsData) {
            PersonsData = personsData;
            return this;
        }             

        public PayFacBuilder<TResult> WithUserReference(UserReference userReference) {
            UserReference = userReference;
            return this;
        }

        public PayFacBuilder<TResult> WithModifier(TransactionModifier transactionModifier) {
            TransactionModifier = transactionModifier;
            return this;
        }

        public PayFacBuilder<TResult> WithPaymentStatistics(PaymentStatistics paymentStatistics) {
            PaymentStatistics = paymentStatistics;
            return this;
        }

        public PayFacBuilder<TResult> WithStatusChangeReason(StatusChangeReason statusChangeReason) {
            StatusChangeReason = statusChangeReason;
            return this;
        }

        /// <summary>
        /// Set the gateway paging criteria for the report
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PayFacBuilder<TResult> WithPaging(int page, int pageSize) {
            Page = page;
            PageSize = pageSize;
            return this;
        }

        public PayFacBuilder<TResult> WithIdempotencyKey(string value) {
            IdempotencyKey = value;
            return this;
        }

        public PayFacBuilder<TResult> WithAddress(Address value, AddressType type = AddressType.Billing) {
            if(Addresses == null) {
                Addresses = new Dictionary<AddressType, Address>();
            }
            Addresses.Add(type, value);
            return this;
        }

        /// <summary>
        /// Required for partners ordering Portico devices. Valid values: [ UTC, PT, MST, MT, CT, ET, HST, AT, AST, AKST, ACT, EET, EAT, MET, NET, PLT, IST, BST, VST, CTT, JST, ACT, AET, SST, NST, MIT, CNT, AGT, CAT ]
        /// </summary>
        public PayFacBuilder<TResult> WithTimeZone(string timezone) {
            TimeZone = timezone;
            return this;
        }

        public PayFacBuilder<TResult> WithPaymentMethodType(PaymentMethodType? paymentMethodType) {
            PaymentMethodType = paymentMethodType;
            return this;
        }

        public PayFacBuilder<TResult> WithPaymentMethodName(PaymentMethodName? paymentMethodName) {
            PaymentMethodName = paymentMethodName;
            return this;
        }

        public PayFacBuilder<TResult> WithCurrency(string currency) {
            Currency = currency;
            return this;
        }

        public PayFacBuilder<TResult> WithClientTransactionId(string value) {
            ClientTransactionId = value;
            return this;
        }

        public PayFacBuilder<TResult> WithBusinessData(BusinessData businessData) {
            BusinessData = businessData;
            return this;
        }

        public PayFacBuilder<TResult> WithSignificantOwnerData(SignificantOwnerData significantOwnerData) {
            SignificantOwnerData = significantOwnerData;
            return this;
        }

        public PayFacBuilder<TResult> WithThreatRiskData(ThreatRiskData threatRiskData) {
            ThreatRiskData = threatRiskData;
            return this;
        }

        public PayFacBuilder<TResult> WithUserPersonalData(UserPersonalData userPersonalData) {
            UserPersonalData = userPersonalData;
            return this;
        }

        public PayFacBuilder<TResult> WithCreditCardData(CreditCardData creditCardInformation, PaymentMethodFunction? paymentMethodFunction = null) {
            CreditCardInformation = creditCardInformation;
            if (paymentMethodFunction != null) {
                if(PaymentMethodsFunctions == null) {
                    PaymentMethodsFunctions = new Dictionary<string, PaymentMethodFunction?>();
                }
                PaymentMethodsFunctions.Add(CreditCardInformation.GetType().Name, paymentMethodFunction.Value);
            }
            return this;
        }

        public PayFacBuilder<TResult> WithACHData(BankAccountData achInformation) {
            ACHInformation = achInformation;
            return this;
        }

        public PayFacBuilder<TResult> WithMailingAddress(Address mailingAddressInformation) {
            MailingAddressInformation = mailingAddressInformation;
            return this;
        }

        public PayFacBuilder<TResult> WithSecondaryBankAccountData(BankAccountData secondaryBankInformation) {
            SecondaryBankInformation = secondaryBankInformation;
            return this;
        }

        public PayFacBuilder<TResult> WithGrossBillingSettleData(GrossBillingInformation grossBillingInformation) {
            GrossBillingInformation = grossBillingInformation;
            return this;
        }

        public PayFacBuilder<TResult> WithAccountNumber(string accountNumber) {
            AccountNumber = accountNumber;
            return this;
        }

        public PayFacBuilder<TResult> WithPassword(string password) {
            Password = password;
            return this;
        }

        public PayFacBuilder<TResult> WithAccountPermissions(AccountPermissions accountPermissions) {
            AccountPermissions = accountPermissions;
            return this;
        }

        public PayFacBuilder<TResult> WithPrimaryBankAccountOwner(BankAccountOwnershipData primaryBankAccountOwner) {
            PrimaryBankAccountOwner = primaryBankAccountOwner;
            return this;
        }

        public PayFacBuilder<TResult> WithSecondaryBankAccountOwner(BankAccountOwnershipData secondaryBankAccountOwner) {
            SecondaryBankAccountOwner = secondaryBankAccountOwner;
            return this;
        }

        public PayFacBuilder<TResult> WithDocumentUploadData(DocumentUploadData docUploadData) {
            DocumentUploadData = docUploadData;

            return this;
        }

        public PayFacBuilder<TResult> WithSSORequestData(SSORequestData ssoRequestData) {
            SSORequestData = ssoRequestData;
            return this;
        }

        public PayFacBuilder<TResult> WithNegativeLimit(string negativeLimit) {
            NegativeLimit = negativeLimit;
            return this;
        }

        public PayFacBuilder<TResult> WithRenewalAccountData(RenewAccountData renewalAccountData) {
            RenewalAccountData = renewalAccountData;
            return this;
        }

        public PayFacBuilder<TResult> WithAmount(string amount) {
            Amount = amount;
            return this;
        }

        public PayFacBuilder<TResult> WithFlashFundsPaymentCardData(FlashFundsPaymentCardData cardData) {
            FlashFundsPaymentCardData = cardData;
            return this;
        }

        public PayFacBuilder<TResult> WithReceivingAccountNumber(string receivingAccountNumber) {
            ReceivingAccountNumber = receivingAccountNumber;
            return this;
        }

        public PayFacBuilder<TResult> WithAllowPending(bool allowPending) {
            AllowPending = allowPending;
            return this;
        }

        public PayFacBuilder<TResult> WithCCAmount(string ccAmount) {
            CCAmount = ccAmount;
            return this;
        }

        public PayFacBuilder<TResult> WithRequireCCRefund(bool requireCCRefund) {
            RequireCCRefund = requireCCRefund;
            return this;
        }

        public PayFacBuilder<TResult> WithTransNum(string transNum) {
            TransNum = transNum;
            return this;
        }

        public PayFacBuilder<TResult> WithExternalID(string externalId) {
            ExternalID = externalId;
            return this;
        }

        public PayFacBuilder<TResult> WithSourceEmail(string sourceEmail) {
            SourceEmail = sourceEmail;
            return this;
        }

        public PayFacBuilder<TResult> WithGatewayTransactionId(string gatewayTransactionId) {
            GatewayTransactionId = gatewayTransactionId;
            return this;
        }

        public PayFacBuilder<TResult> WithGlobaltransId(string globaltransId) {
            GlobaltransId = globaltransId;
            return this;
        }

        public PayFacBuilder<TResult> WithGlobalTransSource(string globalTransSource) {
            GlobalTransSource = globalTransSource;
            return this;
        }

        public PayFacBuilder<TResult> WithCardBrandTransactionId(string cardBrandTransactionId) {
            CardBrandTransactionId = cardBrandTransactionId;
            return this;
        }

        public PayFacBuilder<TResult> WithOrderDevice(OrderDevice orderDevice) {
            OrderDevice = orderDevice;
            return this;
        }
    }
}
