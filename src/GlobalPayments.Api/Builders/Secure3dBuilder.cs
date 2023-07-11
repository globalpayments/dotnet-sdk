using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Builders {
    public class Secure3dBuilder : SecureBuilder<ThreeDSecure> {        
        internal string ApplicationId { get; set; }        
        internal AuthenticationRequestType AuthenticationRequestType { get; set; }        
        internal MobileData MobileData { get; set; }
        internal ChallengeRequestIndicator? ChallengeRequestIndicator { get; set; }        
        internal string CustomerEmail { get; set; }
        internal bool? DecoupledFlowRequest { get; set; }
        internal int? DecoupledFlowTimeout { get; set; }
        internal string DecoupledNotificationUrl { get; set; }        
        internal bool? EnableExemptionOptimization { get; set; }
        internal string EncodedData { get; set; }
        internal JsonDoc EphemeralPublicKey { get; set; }        
        internal string IdempotencyKey { get; set; }        
        internal int? MaximumTimeout { get; set; }
        internal MerchantDataCollection MerchantData { get; set; }
        internal MessageCategory MessageCategory { get; set; }
        internal MerchantInitiatedRequestType? MerchantInitiatedRequestType { get; set; }
        internal string MessageVersion { get; set; }
        internal MethodUrlCompletion MethodUrlCompletion { get; set; }        
        internal string PayerAuthenticationResponse { get; set; }        
        internal SdkInterface? SdkInterface { get; set; }
        internal string SdkTransactionId { get; set; }
        internal SdkUiType[] SdkUiTypes { get; set; }
        internal string ServerTransactionId {
            get {
                if (ThreeDSecure != null) {
                    return ThreeDSecure.ServerTransactionId;
                }
                return null;
            }
        }        
        internal StoredCredential StoredCredential { get; set; }
        internal ThreeDSecure ThreeDSecure { get; set; }        
        internal Secure3dVersion? Version {
            get {
                if (ThreeDSecure != null) {
                    return ThreeDSecure.Version;
                }
                return null;
            }
        }
        internal bool? WhitelistStatus { get; set; }        
        public Secure3dBuilder WithAddress(Address address)
        {
            return WithAddress(address, AddressType.Billing);
        }
        public Secure3dBuilder WithAddress(Address address, AddressType type)
        {
            if (type.Equals(AddressType.Billing)) {
                BillingAddress = address;
            }
            else {
                ShippingAddress = address;
            }
            return this;
        }
        public Secure3dBuilder WithAccountAgeIndicator(AgeIndicator ageIndicator)
        {
            AccountAgeIndicator = ageIndicator;
            return this;
        }
        public Secure3dBuilder WithAccountChangeDate(DateTime accountChangeDate)
        {
            AccountChangeDate = accountChangeDate;
            return this;
        }
        public Secure3dBuilder WithAccountCreateDate(DateTime accountCreateDate)
        {
            AccountCreateDate = accountCreateDate;
            return this;
        }
        public Secure3dBuilder WithAccountChangeIndicator(AgeIndicator accountChangeIndicator)
        {
            AccountChangeIndicator = accountChangeIndicator;
            return this;
        }
        public Secure3dBuilder WithAddressMatchIndicator(bool value)
        {
            AddressMatchIndicator = value;
            return this;
        }
        public Secure3dBuilder WithAmount(decimal value)
        {
            Amount = value;
            return this;
        }
        public Secure3dBuilder WithApplicationId(string applicationId)
        {
            ApplicationId = applicationId;
            return this;
        }
        public Secure3dBuilder WithAuthenticationSource(AuthenticationSource value)
        {
            AuthenticationSource = value;
            return this;
        }
        public Secure3dBuilder WithAuthenticationRequestType(AuthenticationRequestType value)
        {
            AuthenticationRequestType = value;
            return this;
        }
        public Secure3dBuilder WithBrowserData(BrowserData value)
        {
            BrowserData = value;
            return this;
        }
        public Secure3dBuilder WithMobileData(MobileData value)
        {
            MobileData = value;
            return this;
        }
        public Secure3dBuilder WithChallengeRequestIndicator(ChallengeRequestIndicator value)
        {
            ChallengeRequestIndicator = value;
            return this;
        }
        public Secure3dBuilder WithCurrency(string value)
        {
            Currency = value;
            return this;
        }
        public Secure3dBuilder WithCustomerAccountId(string customerAccountId)
        {
            CustomerAccountId = customerAccountId;
            return this;
        }
        public Secure3dBuilder WithCustomerAuthenticationData(string customerAuthenticationData)
        {
            CustomerAuthenticationData = customerAuthenticationData;
            return this;
        }
        public Secure3dBuilder WithCustomerAuthenticationMethod(CustomerAuthenticationMethod customerAuthenticationMethod)
        {
            CustomerAuthenticationMethod = customerAuthenticationMethod;
            return this;
        }
        public Secure3dBuilder WithCustomerAuthenticationTimestamp(DateTime customerAuthenticationTimestamp)
        {
            CustomerAuthenticationTimestamp = customerAuthenticationTimestamp;
            return this;
        }
        public Secure3dBuilder WithCustomerEmail(string value)
        {
            CustomerEmail = value;
            return this;
        }
        public Secure3dBuilder WithDecoupledFlowRequest(bool value)
        {
            DecoupledFlowRequest = value;
            return this;
        }
        public Secure3dBuilder WithDecoupledFlowTimeout(int value)
        {
            DecoupledFlowTimeout = value;
            return this;
        }
        public Secure3dBuilder WithDecoupledNotificationUrl(string value)
        {
            DecoupledNotificationUrl = value;
            return this;
        }
        public Secure3dBuilder WithDeliveryEmail(string deliveryEmail)
        {
            DeliveryEmail = deliveryEmail;
            return this;
        }
        public Secure3dBuilder WithDeliveryTimeFrame(DeliveryTimeFrame deliveryTimeframe)
        {
            DeliveryTimeframe = deliveryTimeframe;
            return this;
        }
        public Secure3dBuilder WithEnableExemptionOptimization(bool enableExemptionOptimization)
        {
            EnableExemptionOptimization = enableExemptionOptimization;
            return this;
        }
        public Secure3dBuilder WithEncodedData(string encodedData)
        {
            EncodedData = encodedData;
            return this;
        }
        public Secure3dBuilder WithEphemeralPublicKey(string ephemeralPublicKey)
        {
            EphemeralPublicKey = JsonDoc.Parse(ephemeralPublicKey);
            return this;
        }
        public Secure3dBuilder WithGiftCardCount(int? giftCardCount)
        {
            GiftCardCount = giftCardCount;
            return this;
        }
        public Secure3dBuilder WithGiftCardCurrency(string giftCardCurrency)
        {
            GiftCardCurrency = giftCardCurrency;
            return this;
        }
        public Secure3dBuilder WithGiftCardAmount(decimal giftCardAmount)
        {
            GiftCardAmount = giftCardAmount;
            return this;
        }
        public Secure3dBuilder WithHomeNumber(string countryCode, string number)
        {
            HomeCountryCode = countryCode;
            HomeNumber = number;
            return this;
        }
        public Secure3dBuilder WithIdempotencyKey(string value)
        {
            IdempotencyKey = value;
            return this;
        }
        public Secure3dBuilder WithMaxNumberOfInstallments(int? maxNumberOfInstallments)
        {
            MaxNumberOfInstallments = maxNumberOfInstallments;
            return this;
        }
        public Secure3dBuilder WithMaximumTimeout(int? maximumTimeout)
        {
            MaximumTimeout = maximumTimeout;
            return this;
        }
        public Secure3dBuilder WithMerchantData(MerchantDataCollection value)
        {
            MerchantData = value;
            if (MerchantData != null) {
                if (ThreeDSecure == null) {
                    ThreeDSecure = new ThreeDSecure();
                }
                ThreeDSecure.MerchantData = value;
            }

            return this;
        }
        public Secure3dBuilder WithMessageCategory(MessageCategory value)
        {
            MessageCategory = value;
            return this;
        }
        public Secure3dBuilder WithMerchantInitiatedRequestType(MerchantInitiatedRequestType merchantInitiatedRequestType)
        {
            MerchantInitiatedRequestType = merchantInitiatedRequestType;
            return this;
        }
        public Secure3dBuilder WithMessageVersion(string value)
        {
            MessageVersion = value;
            return this;
        }
        public Secure3dBuilder WithMethodUrlCompletion(MethodUrlCompletion value)
        {
            MethodUrlCompletion = value;
            return this;
        }
        public Secure3dBuilder WithMobileNumber(string countryCode, string number)
        {
            MobileCountryCode = countryCode;
            MobileNumber = number;
            return this;
        }
        public Secure3dBuilder WithNumberOfAddCardAttemptsInLast24Hours(int? numberOfAddCardAttemptsInLast24Hours)
        {
            NumberOfAddCardAttemptsInLast24Hours = numberOfAddCardAttemptsInLast24Hours;
            return this;
        }
        public Secure3dBuilder WithNumberOfPurchasesInLastSixMonths(int? numberOfPurchasesInLastSixMonths)
        {
            NumberOfPurchasesInLastSixMonths = numberOfPurchasesInLastSixMonths;
            return this;
        }
        public Secure3dBuilder WithNumberOfTransactionsInLast24Hours(int? numberOfTransactionsInLast24Hours)
        {
            NumberOfTransactionsInLast24Hours = numberOfTransactionsInLast24Hours;
            return this;
        }
        public Secure3dBuilder WithNumberOfTransactionsInLastYear(int? numberOfTransactionsInLastYear)
        {
            NumberOfTransactionsInLastYear = numberOfTransactionsInLastYear;
            return this;
        }
        public Secure3dBuilder WithOrderCreateDate(DateTime value)
        {
            OrderCreateDate = value;
            return this;
        }
        public Secure3dBuilder WithOrderId(string value)
        {
            OrderId = value;
            return this;
        }
        public Secure3dBuilder WithOrderTransactionType(OrderTransactionType orderTransactionType)
        {
            OrderTransactionType = orderTransactionType;
            return this;
        }
        public Secure3dBuilder WithPasswordChangeDate(DateTime passwordChangeDate)
        {
            PasswordChangeDate = passwordChangeDate;
            return this;
        }
        public Secure3dBuilder WithPasswordChangeIndicator(AgeIndicator passwordChangeIndicator)
        {
            PasswordChangeIndicator = passwordChangeIndicator;
            return this;
        }
        public Secure3dBuilder WithPaymentAccountCreateDate(DateTime paymentAccountCreateDate)
        {
            PaymentAccountCreateDate = paymentAccountCreateDate;
            return this;
        }
        public Secure3dBuilder WithPaymentAccountAgeIndicator(AgeIndicator paymentAgeIndicator)
        {
            PaymentAgeIndicator = paymentAgeIndicator;
            return this;
        }
        public Secure3dBuilder WithPayerAuthenticationResponse(string value)
        {
            PayerAuthenticationResponse = value;
            return this;
        }
        public Secure3dBuilder WithPaymentMethod(IPaymentMethod value)
        {
            PaymentMethod = value;
            if (value is ISecure3d) {
                var secureEcom = ((ISecure3d)value).ThreeDSecure;
                if (secureEcom != null) {
                    ThreeDSecure = secureEcom;
                }
            }
            return this;
        }
        public Secure3dBuilder WithPreOrderAvailabilityDate(DateTime preOrderAvailabilityDate)
        {
            PreOrderAvailabilityDate = preOrderAvailabilityDate;
            return this;
        }
        public Secure3dBuilder WithPreOrderIndicator(PreOrderIndicator preOrderIndicator)
        {
            PreOrderIndicator = preOrderIndicator;
            return this;
        }
        public Secure3dBuilder WithPreviousSuspiciousActivity(bool? previousSuspiciousActivity)
        {
            PreviousSuspiciousActivity = previousSuspiciousActivity;
            return this;
        }
        public Secure3dBuilder WithPriorAuthenticationData(string priorAuthenticationData)
        {
            PriorAuthenticationData = priorAuthenticationData;
            return this;
        }
        public Secure3dBuilder WithPriorAuthenticationMethod(PriorAuthenticationMethod priorAuthenticationMethod)
        {
            PriorAuthenticationMethod = priorAuthenticationMethod;
            return this;
        }
        public Secure3dBuilder WithPriorAuthenticationTransactionId(string priorAuthencitationTransactionId)
        {
            PriorAuthenticationTransactionId = priorAuthencitationTransactionId;
            return this;
        }
        public Secure3dBuilder WithPriorAuthenticationTimestamp(DateTime priorAuthenticationTimestamp)
        {
            PriorAuthenticationTimestamp = priorAuthenticationTimestamp;
            return this;
        }
        public Secure3dBuilder WithRecurringAuthorizationExpiryDate(DateTime recurringAuthorizationExpiryDate)
        {
            RecurringAuthorizationExpiryDate = recurringAuthorizationExpiryDate;
            return this;
        }
        public Secure3dBuilder WithRecurringAuthorizationFrequency(int? recurringAuthorizationFrequency)
        {
            RecurringAuthorizationFrequency = recurringAuthorizationFrequency;
            return this;
        }
        public Secure3dBuilder WithReferenceNumber(string referenceNumber)
        {
            ReferenceNumber = referenceNumber;
            return this;
        }
        public Secure3dBuilder WithReorderIndicator(ReorderIndicator reorderIndicator)
        {
            ReorderIndicator = reorderIndicator;
            return this;
        }
        public Secure3dBuilder WithSdkInterface(SdkInterface sdkInterface)
        {
            SdkInterface = sdkInterface;
            return this;
        }
        public Secure3dBuilder WithSdkTransactionId(string sdkTransactionId)
        {
            SdkTransactionId = sdkTransactionId;
            return this;
        }
        public Secure3dBuilder WithSdkUiTypes(params SdkUiType[] sdkUiTypes)
        {
            SdkUiTypes = sdkUiTypes;
            return this;
        }
        public Secure3dBuilder WithServerTransactionId(string value)
        {
            if (ThreeDSecure == null) {
                ThreeDSecure = new ThreeDSecure();
            }
            ThreeDSecure.ServerTransactionId = value;
            return this;
        }
        public Secure3dBuilder WithShippingAddressCreateDate(DateTime shippingAddressCreateDate)
        {
            ShippingAddressCreateDate = shippingAddressCreateDate;
            return this;
        }
        public Secure3dBuilder WithShippingAddressUsageIndicator(AgeIndicator shippingAddressUsageIndicator)
        {
            ShippingAddressUsageIndicator = shippingAddressUsageIndicator;
            return this;
        }
        public Secure3dBuilder WithShippingMethod(ShippingMethod shippingMethod)
        {
            ShippingMethod = shippingMethod;
            return this;
        }
        public Secure3dBuilder WithShippingNameMatchesCardHolderName(bool? shippingNameMatchesCardHolderName)
        {
            ShippingNameMatchesCardHolderName = shippingNameMatchesCardHolderName;
            return this;
        }
        public Secure3dBuilder WithStoredCredential(StoredCredential storedCredential)
        {
            StoredCredential = storedCredential;
            return this;
        }
        public Secure3dBuilder WithThreeDSecure(ThreeDSecure threeDSecure)
        {
            ThreeDSecure = threeDSecure;
            return this;
        }
        public Secure3dBuilder WithTransactionType(TransactionType transactionType)
        {
            TransactionType = transactionType;
            return this;
        }
        public Secure3dBuilder WithWhitelistStatus(bool whitelistStatus)
        {
            WhitelistStatus = whitelistStatus;
            return this;
        }
        public Secure3dBuilder WithWorkNumber(string countryCode, string number)
        {
            WorkCountryCode = countryCode;
            WorkNumber = number;
            return this;
        }

        public Secure3dBuilder(TransactionType transactionType) {
            AuthenticationSource = Entities.AuthenticationSource.BROWSER;
            AuthenticationRequestType = AuthenticationRequestType.PAYMENT_TRANSACTION;
            MessageCategory = MessageCategory.PAYMENT_AUTHENTICATION;

            TransactionType = transactionType;
        }

        // HELPER METHOD FOR THE CONNECTOR
        public bool HasMobileFields {
            get {
                return (
                        !string.IsNullOrEmpty(ApplicationId) ||
                        EphemeralPublicKey != null ||
                        MaximumTimeout != null ||
                        ReferenceNumber != null ||
                        !string.IsNullOrEmpty(SdkTransactionId) ||
                        !string.IsNullOrEmpty(EncodedData) ||
                        SdkInterface != null ||
                        SdkUiTypes != null
                );
            }
        }
        public bool HasPriorAuthenticationData {
            get {
                return (
                        PriorAuthenticationMethod != null ||
                        !string.IsNullOrEmpty(PriorAuthenticationTransactionId) ||
                        PriorAuthenticationTimestamp != null ||
                        !string.IsNullOrEmpty(PriorAuthenticationData)
                );
            }
        }
        public bool HasRecurringAuthData {
            get {
                return (
                        MaxNumberOfInstallments != null ||
                        RecurringAuthorizationFrequency != null ||
                        RecurringAuthorizationExpiryDate != null
                );
            }
        }
        public bool HasPayerLoginData {
            get {
                return (
                        !string.IsNullOrEmpty(CustomerAuthenticationData) ||
                        CustomerAuthenticationTimestamp != null ||
                        CustomerAuthenticationMethod != null
                );
            }
        }

        public override ThreeDSecure Execute(string configName = "default") {
            return Execute(Secure3dVersion.Any, configName);
        }
        public override ThreeDSecure Execute(Secure3dVersion version, string configName = "default") {
            Validations.Validate(this);

            // setup return object
            ThreeDSecure rvalue = ThreeDSecure;
            if (rvalue == null) {
                rvalue = new ThreeDSecure();
                rvalue.Version = version;
            }

            // working version
            if (rvalue.Version != null) {
                version = rvalue.Version.Value;
            }

            // get the provider
            ISecure3dProvider provider = ServicesContainer.Instance.GetSecure3d(configName, version);
            if (version == Secure3dVersion.One && (provider is GpApiConnector || provider is GpEcomConnector)) {
                throw new BuilderException($"3D Secure {version} is no longer supported!");
            }
            if (provider != null) {
                bool canDowngrade = false;
                if (provider.Version.Equals(Secure3dVersion.Two) && version.Equals(Secure3dVersion.Any) &&
                (!(provider is GpEcomConnector) && !(provider is GpApiConnector))) {
                    try {
                        var oneProvider = ServicesContainer.Instance.GetSecure3d(configName, Secure3dVersion.One);
                        canDowngrade = (oneProvider != null);
                    }
                    catch (ConfigurationException) { /* NOT CONFIGURED */ }
                }

                /* process the request, capture any exceptions which might have been thrown */
                Transaction response = null;
                try {
                    response = provider.ProcessSecure3d(this);
                    if (response == null && canDowngrade) {
                        return Execute(Secure3dVersion.One, configName);
                    }
                }
                catch (GatewayException exc) {
                    // check for not enrolled
                    if ("110".Equals(exc.ResponseCode) && provider.Version.Equals(Secure3dVersion.One)) {
                        return rvalue;
                    }
                    // check if we can downgrade
                    else if (canDowngrade && TransactionType.Equals(TransactionType.VerifyEnrolled)) {
                        return Execute(Secure3dVersion.One, configName);
                    }
                    // throw exception
                    else throw exc;
                }

                // check the response
                if (response != null) {
                    switch (TransactionType) {
                        case TransactionType.VerifyEnrolled: {
                                if (response.ThreeDSecure != null) {
                                    rvalue = response.ThreeDSecure;
                                    if (new List<string>() { "True", "Y" }.Contains(rvalue.Enrolled)) {
                                        rvalue.Amount = Amount;
                                        rvalue.Currency = Currency;
                                        rvalue.OrderId = response.OrderId;
                                        rvalue.Version = provider.Version;
                                    }
                                    else if (canDowngrade) {
                                        return Execute(Secure3dVersion.One, configName);
                                    }
                                }
                                else if (canDowngrade) {
                                    return Execute(Secure3dVersion.One, configName);
                                }
                            }
                            break;
                        case TransactionType.InitiateAuthentication:
                        case TransactionType.VerifySignature: {
                                rvalue.Merge(response.ThreeDSecure);
                            }
                            break;
                    }
                }
            }

            return rvalue;
        }

        public void setupValidations() {
           
            #region ENUM VALIDATION WITH FLAG ATTRIBUTE     
            /// TO ADD
            #endregion

            Validations.For(TransactionType.VerifyEnrolled)
                .Check(() => PaymentMethod).IsNotNull();

            Validations.For(TransactionType.VerifyEnrolled)
                .When(() => PaymentMethod).IsNotNull()
                .Check(() => PaymentMethod).Is<ISecure3d>();

            Validations.For(TransactionType.VerifySignature)
                .When(() => Version).Equals(Secure3dVersion.One)
                .Check(() => ThreeDSecure).IsNotNull()
                .When(() => Version).Equals(Secure3dVersion.One)
                .Check(() => PayerAuthenticationResponse).IsNotNull();

            Validations.For(TransactionType.VerifySignature)
                .When(() => Version).Equals(Secure3dVersion.Two)
                .Check(() => ServerTransactionId).IsNotNull();

            Validations.For(TransactionType.InitiateAuthentication)
                .Check(() => ThreeDSecure).IsNotNull();

            Validations.For(TransactionType.InitiateAuthentication)
                .When(() => PaymentMethod).IsNotNull()
                .Check(() => PaymentMethod).Is<ISecure3d>();

            Validations.For(TransactionType.InitiateAuthentication)
                .When(() => MerchantInitiatedRequestType).IsNotNull()
                .Check(() => MerchantInitiatedRequestType).DoesNotEqual(AuthenticationRequestType.PAYMENT_TRANSACTION);

            Validations.For(TransactionType.InitiateAuthentication)
                .When(() => AccountAgeIndicator).IsNotNull()
                .Check(() => AccountAgeIndicator).DoesNotEqual(AgeIndicator.NO_CHANGE);

            Validations.For(TransactionType.InitiateAuthentication)
                .When(() => PasswordChangeIndicator).IsNotNull()
                .Check(() => PasswordChangeIndicator).DoesNotEqual(AgeIndicator.NO_ACCOUNT);

            Validations.For(TransactionType.InitiateAuthentication)
                .When(() => ShippingAddressUsageIndicator).IsNotNull()
                .Check(() => ShippingAddressUsageIndicator).DoesNotEqual(AgeIndicator.NO_CHANGE)
                .When(() => ShippingAddressUsageIndicator).IsNotNull()
                .Check(() => ShippingAddressUsageIndicator).DoesNotEqual(AgeIndicator.NO_ACCOUNT);
        }        
    }
}
