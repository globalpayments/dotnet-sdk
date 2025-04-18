﻿using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.UPA;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Enums;
using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Terminals.Builders {
    public class TerminalAuthBuilder : TerminalBuilder<TerminalAuthBuilder> {
        internal Address Address { get; set; }
        internal bool AllowDuplicates { get; set; }
        internal decimal? Amount { get; set; }
        internal string AuthCode {
            get {
                if (PaymentMethod is TransactionReference)
                    return (PaymentMethod as TransactionReference).AuthCode;
                return null;
            }
        }
        internal AutoSubstantiation AutoSubstantiation { get; set; }
        internal Lodging Lodging { get; set; }
        internal decimal? CashBackAmount { get; set; }
        internal string ClientTransactionId { get; set; }
        internal CurrencyType? Currency { get; set; }
        internal string CustomerCode { get; set; }
        internal decimal? Gratuity { get; set; }
        internal string InvoiceNumber { get; set; }
        internal string PoNumber { get; set; }
        internal bool RequestMultiUseToken { get; set; }
        internal bool SignatureCapture { get; set; }
        internal decimal? TaxAmount { get; set; }
        internal string TaxExempt { get; set; }
        internal string TaxExemptId { get; set; }
        internal string TerminalRefNumber { get; set; }
        internal int? ClerkId { get; set; }
        internal string LineItemLeft { get; set; }
        internal string LineItemRight { get; set; }
        internal StoredCredentialInitiator? CardOnFileIndicator { get; set; }
        internal string CardBrandTransId { get; set; }
        internal decimal? PrescriptionAmount { get; set; }
        internal decimal? ClinicAmount { get; set; }
        internal decimal? DentalAmount { get; set; }
        internal decimal? VisionOpticalAmount { get; set; }
        internal bool? ProcessCPC { get; set; }
        internal bool? ConfirmAmount { get; set; }
        public string Token { get; set; }
        public DateTime ShippingDate { get; set; }
        public decimal? PreAuthAmount { get; set; }
        public AcquisitionType? CardAcquisition { get; set; }
        internal bool AllowPartialAuth { get; set; }
        internal bool? IsQuickChip { get; set; }
        internal bool? HasCheckLuhn { get; set; } = null;
        internal bool? HasSecurityCode { get; set; } = null;
        internal int Timeout { get; set; }
        internal DateTime? TransactionDate { get; set; }
        internal List<AcquisitionType> AcquisitionTypes { get; set; }
        internal MerchantDecision? MerchantDecision { get; set;}
        internal string Language { get; set;}
        internal HostData HostData { get; set; }
        internal string TransactionId {
            get {
                if (PaymentMethod is TransactionReference)
                    return (PaymentMethod as TransactionReference).TransactionId;
                return null;
            }
        }

        public TerminalAuthBuilder WithLineItemLeft(string lineItemLeft) {
            LineItemLeft = lineItemLeft;
            return this;
        }

        public TerminalAuthBuilder WithLineItemRight(string lineItemRight) {
            LineItemRight = lineItemRight;
            return this;
        }
        public TerminalAuthBuilder WithAllowPartialAuth(bool value) {
            AllowPartialAuth = value;
            return this;
        }

        public TerminalAuthBuilder WithAddress(Address address) {
            Address = address;
            return this;
        }

        public TerminalAuthBuilder WithEcrId(int ecrId) {
            EcrId = ecrId.ToString();
            return this;
        }
        public TerminalAuthBuilder WithEcrId(string ecrId) {
            EcrId = ecrId;
            return this;
        }
        public TerminalAuthBuilder WithAllowDuplicates(bool allowDuplicates) {
            AllowDuplicates = allowDuplicates;
            return this;
        }
        public TerminalAuthBuilder WithAmount(decimal? amount) {
            Amount = amount;
            return this;
        }

        public TerminalAuthBuilder WithPreAuthAmount(decimal? preAuthAmount) {
            PreAuthAmount = preAuthAmount;
            return this;
        }

        public TerminalAuthBuilder WithCardAcquisition(AcquisitionType cardAcquisition) {
            CardAcquisition = cardAcquisition;
            return this;
        }

        public TerminalAuthBuilder WithAuthCode(string value) {
            if (PaymentMethod == null || !(PaymentMethod is TransactionReference))
                PaymentMethod = new TransactionReference();
            (PaymentMethod as TransactionReference).AuthCode = value;
            return this;
        }

        /// <summary>
        /// Sets the auto subtantiation values for the transaction.
        /// </summary>
        /// <param name="value">The auto substantiation object</param>
        /// <returns>TerminalAuthBuilder</returns>
        public TerminalAuthBuilder WithAutoSubstantiation(AutoSubstantiation value) {
            AutoSubstantiation = value;
            return this;
        }

        public TerminalAuthBuilder WithLodging(Lodging value) {
            Lodging = value;
            return this;
        }
        public TerminalAuthBuilder WithCashBack(decimal? amount) {
            CashBackAmount = amount;
            return this;
        }
        public TerminalAuthBuilder WithClientTransactionId(string value) {
            ClientTransactionId = value;
            return this;
        }
        public TerminalAuthBuilder WithCurrency(CurrencyType? value) {
            Currency = value;
            return this;
        }
        public TerminalAuthBuilder WithCustomerCode(string customerCode) {
            CustomerCode = customerCode;
            return this;
        }
        public TerminalAuthBuilder WithGratuity(decimal? gratuity) {
            Gratuity = gratuity;
            return this;
        }
        public TerminalAuthBuilder WithInvoiceNumber(string invoiceNumber) {
            this.InvoiceNumber = invoiceNumber;
            return this;
        }
        public TerminalAuthBuilder WithPaymentMethod(IPaymentMethod method) {
            PaymentMethod = method;
            return this;
        }
        public TerminalAuthBuilder WithPoNumber(string poNumber) {
            PoNumber = poNumber;
            return this;
        }
        public TerminalAuthBuilder WithRequestMultiUseToken(bool requestMultiUseToken) {
            RequestMultiUseToken = requestMultiUseToken;
            return this;
        }
        public TerminalAuthBuilder WithSignatureCapture(bool signatureCapture) {
            SignatureCapture = signatureCapture;
            return this;
        }
        public TerminalAuthBuilder WithTaxAmount(decimal taxAmount) {
            TaxAmount = taxAmount;
            return this;
        }

        public TerminalAuthBuilder WithClerkId(short clerkId) {
            ClerkId = clerkId;
            return this;
        }

        public TerminalAuthBuilder WithTerminalRefNumber(string terminalRefNumber) {
            TerminalRefNumber = terminalRefNumber;
            return this;
        }

        public TerminalAuthBuilder WithTaxType(TaxType taxType, string taxExemptId = null) {
            TaxExempt = taxType == TaxType.TAXEXEMPT ? "1" : "0";
            TaxExemptId = taxExemptId;
            return this;
        }

        public TerminalAuthBuilder WithToken(string value) {
            if (PaymentMethod == null || !(PaymentMethod is CreditCardData))
                PaymentMethod = new CreditCardData();
            (PaymentMethod as CreditCardData).Token = value;
            return this;
        }

        public TerminalAuthBuilder WithCardOnFileIndicator(StoredCredentialInitiator value) {
            CardOnFileIndicator = value;
            return this;
        }

        public TerminalAuthBuilder WithCardBrandTransId(string value) {
            CardBrandTransId = value;
            return this;
        }

        public TerminalAuthBuilder WithTransactionId(string value) {
            if (PaymentMethod == null || !(PaymentMethod is TransactionReference))
                PaymentMethod = new TransactionReference();
            (PaymentMethod as TransactionReference).TransactionId = value;
            return this;
        }

        public TerminalAuthBuilder WithPrescriptionAmount(decimal prescriptionAmt) {
            PrescriptionAmount = prescriptionAmt;
            return this;
        }

        public TerminalAuthBuilder WithClinicAmount(decimal clinicAmt) {
            ClinicAmount = clinicAmt;
            return this;
        }

        public TerminalAuthBuilder WithDentalAmount(decimal value) {
            DentalAmount = value;
            return this;
        }

        public TerminalAuthBuilder WithVisionOpticalAmount(decimal value) {
            VisionOpticalAmount = value;
            return this;
        }

        public TerminalAuthBuilder WithConfirmationAmount(bool value) {
            ConfirmAmount = value;
            return this;
        }

        public TerminalAuthBuilder WithQuickChip(bool value) {
            IsQuickChip = value;
            return this;
        }

        public TerminalAuthBuilder WithCheckLuhn(bool value) {
            HasCheckLuhn = value;
            return this;
        }

        public TerminalAuthBuilder WithSecurityCode(bool value) {
            HasSecurityCode = value;
            return this;
        }

        public TerminalAuthBuilder WithAcquisitionTypes(List<AcquisitionType> value) {
            AcquisitionTypes = value;
            return this;
        }

        public TerminalAuthBuilder WithTransactionDate(DateTime value) {
            TransactionDate = value;
            return this;
        }

        public TerminalAuthBuilder WithTimeout(int value) {
            Timeout = value;
            return this;
        }

        public TerminalAuthBuilder WithMerchantDecision(MerchantDecision merchantDecision) {
            MerchantDecision = merchantDecision;
            return this;
        }

        public TerminalAuthBuilder WithLanguage(string language) {
            Language = language;
            return this;
        }
        
        public TerminalAuthBuilder WithHostData(HostData hostData) {
            HostData = hostData;
            return this;
        }

        public TerminalAuthBuilder WithProcessCPC(bool? value) {
            ProcessCPC = value;
            return this;
        }

        public TerminalAuthBuilder WithTransactionModifier(TransactionModifier modifier) {
            TransactionModifier = modifier;
            return this;
        }

        internal TerminalAuthBuilder(TransactionType type, PaymentMethodType paymentType) : base(type, paymentType) {
        }

        public TerminalAuthBuilder WithShippingDate(DateTime value) {
            ShippingDate = value;
            return this;
        }

        public override ITerminalResponse Execute(string configName = "default") {
            base.Execute(configName);

            var device = ServicesContainer.Instance.GetDeviceController(configName);
            return device.ProcessTransaction(this);
        }

        public override byte[] Serialize(string configName = "default") {
            base.Execute();

            var device = ServicesContainer.Instance.GetDeviceController(configName);
            return device.SerializeRequest(this);
        }

        protected override void SetupValidations() {

            #region ENUM VALIDATION WITH FLAG ATTRIBUTE            
            
            Validations.For(PaymentMethodType.Gift).Check(() => Currency).IsNotNull();
            Validations.For(PaymentMethodType.EBT).With(TransactionType.Balance)
                .When(() => Currency).IsNotNull()
                .Check(() => Currency).DoesNotEqual(CurrencyType.VOUCHER);
            
            Validations.For(PaymentMethodType.EBT).With(TransactionType.Refund).Check(() => AllowDuplicates).Equals(false);
            Validations.For(PaymentMethodType.EBT).With(TransactionType.BenefitWithdrawal).Check(() => AllowDuplicates).Equals(false);

            #endregion

            Validations.For(TransactionType.Sale).Check(() => Amount).IsNotNull();
            Validations.For(TransactionType.Auth).Check(() => Amount).IsNotNull();

            Validations.For(TransactionType.Refund).Check(() => Amount).IsNotNull();
            Validations.For(TransactionType.Refund)
                .With(PaymentMethodType.Credit)
                .When(() => TransactionId).IsNotNull()
                .Check(() => AuthCode).IsNotNull();
            Validations.For(TransactionType.AddValue).Check(() => Amount).IsNotNull();
            Validations.For(TransactionType.BenefitWithdrawal)
                .When(() => Currency).IsNotNull()
                .Check(() => Currency).Equals(CurrencyType.CASH_BENEFITS);
            
        }
    }
}
