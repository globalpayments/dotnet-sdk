using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.PayFac;
using GlobalPayments.Api.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Builders
{
    public class PayFacBuilder : BaseBuilder<Transaction> {
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

        public string Amount { get; set; }
        public string ReceivingAccountNumber { get; set; }
        public bool? AllowPending { get; set; }
        public string CCAmount { get; set; }
        public bool? RequireCCRefund { get; set; }
        public string TransNum { get; set; }

        public string ExternalID { get; set; }
        public string SourceEmail { get; set; }

        public FlashFundsPaymentCardData FlashFundsPaymentCardData { get; set; }

        public PayFacBuilder(TransactionType type, TransactionModifier modifer = TransactionModifier.None) {
            TransactionType = type;
            TransactionModifier = modifer;
        }

        public override Transaction Execute(string configName = "default") {
            base.Execute(configName);

            var client = ServicesContainer.Instance.GetPayFac(configName);

            return client.ProcessPayFac(this);
        }

        protected override void SetupValidations() {
            // Account Management Methods
            Validations.For(TransactionType.CreateAccount)
                .With(TransactionModifier.None)
                .Check(() => BeneficialOwnerData).IsNotNull()
                .Check(() => BusinessData).IsNotNull()
                .Check(() => UserPersonalData).IsNotNull()
                .Check(() => CreditCardInformation).IsNotNull();

            Validations.For(TransactionType.EditAccount)
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

            Validations.For(TransactionType.DisownAccount)
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

            Validations.For(TransactionType.ReverseSplitPay)
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
                .Check(() => Amount).IsNotNull()
                .Check(() => TransNum).IsNotNull();

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

            Validations.For(TransactionType.GetAccountBalance)
                .With(TransactionModifier.None)
                .Check(() => AccountNumber).IsNotNull();
        }

        public PayFacBuilder WithBankAccountData(BankAccountData bankAccountData) {
            BankAccountData = bankAccountData;
            return this;
        }

        public PayFacBuilder WithBeneficialOwnerData(BeneficialOwnerData beneficialOwnerData) {
            BeneficialOwnerData = beneficialOwnerData;
            return this;
        }

        public PayFacBuilder WithBusinessData(BusinessData businessData) {
            BusinessData = businessData;
            return this;
        }

        public PayFacBuilder WithSignificantOwnerData(SignificantOwnerData significantOwnerData) {
            SignificantOwnerData = significantOwnerData;
            return this;
        }

        public PayFacBuilder WithThreatRiskData(ThreatRiskData threatRiskData) {
            ThreatRiskData = threatRiskData;
            return this;
        }

        public PayFacBuilder WithUserPersonalData(UserPersonalData userPersonalData) {
            UserPersonalData = userPersonalData;
            return this;
        }

        public PayFacBuilder WithCreditCardData(CreditCardData creditCardInformation) {
            CreditCardInformation = creditCardInformation;
            return this;
        }

        public PayFacBuilder WithACHData(BankAccountData achInformation) {
            ACHInformation = achInformation;
            return this;
        }

        public PayFacBuilder WithMailingAddress(Address mailingAddressInformation) {
            MailingAddressInformation = mailingAddressInformation;
            return this;
        }

        public PayFacBuilder WithSecondaryBankAccountData(BankAccountData secondaryBankInformation) {
            SecondaryBankInformation = secondaryBankInformation;
            return this;
        }

        public PayFacBuilder WithGrossBillingSettleData(GrossBillingInformation grossBillingInformation) {
            GrossBillingInformation = grossBillingInformation;
            return this;
        }

        public PayFacBuilder WithAccountNumber(string accountNumber) {
            AccountNumber = accountNumber;
            return this;
        }

        public PayFacBuilder WithPassword(string password) {
            Password = password;
            return this;
        }

        public PayFacBuilder WithAccountPermissions(AccountPermissions accountPermissions) {
            AccountPermissions = accountPermissions;
            return this;
        }

        public PayFacBuilder WithPrimaryBankAccountOwner(BankAccountOwnershipData primaryBankAccountOwner) {
            PrimaryBankAccountOwner = primaryBankAccountOwner;
            return this;
        }

        public PayFacBuilder WithSecondaryBankAccountOwner(BankAccountOwnershipData secondaryBankAccountOwner) {
            SecondaryBankAccountOwner = secondaryBankAccountOwner;
            return this;
        }

        public PayFacBuilder WithDocumentUploadData(DocumentUploadData docUploadData) {
            DocumentUploadData = docUploadData;

            return this;
        }

        public PayFacBuilder WithSSORequestData(SSORequestData ssoRequestData) {
            SSORequestData = ssoRequestData;
            return this;
        }

        public PayFacBuilder WithNegativeLimit(string negativeLimit) {
            NegativeLimit = negativeLimit;
            return this;
        }

        public PayFacBuilder WithRenewalAccountData(RenewAccountData renewalAccountData) {
            RenewalAccountData = renewalAccountData;
            return this;
        }

        public PayFacBuilder WithAmount(string amount) {
            Amount = amount;
            return this;
        }

        public PayFacBuilder WithFlashFundsPaymentCardData(FlashFundsPaymentCardData cardData) {
            FlashFundsPaymentCardData = cardData;
            return this;
        }

        public PayFacBuilder WithReceivingAccountNumber(string receivingAccountNumber) {
            ReceivingAccountNumber = receivingAccountNumber;
            return this;
        }

        public PayFacBuilder WithAllowPending(bool allowPending) {
            AllowPending = allowPending;
            return this;
        }

        public PayFacBuilder WithCCAmount(string ccAmount) {
            CCAmount = ccAmount;
            return this;
        }

        public PayFacBuilder WithRequireCCRefund(bool requireCCRefund) {
            RequireCCRefund = requireCCRefund;
            return this;
        }

        public PayFacBuilder WithTransNum(string transNum) {
            TransNum = transNum;
            return this;
        }

        public PayFacBuilder WithExternalID(string externalId) {
            ExternalID = externalId;
            return this;
        }

        public PayFacBuilder WithSourceEmail(string sourceEmail) {
            SourceEmail = sourceEmail;
            return this;
        }
    }
}
