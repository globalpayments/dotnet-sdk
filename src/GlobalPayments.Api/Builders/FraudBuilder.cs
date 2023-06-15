using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Builders
{
    public class FraudBuilder<TResult> : SecureBuilder<TResult> where TResult : class
    {
        public FraudBuilder(TransactionType type) {
            TransactionType = type;
        }
               

        public override TResult Execute(string configName = "default")
        {
            base.Execute(configName);

            var client = ServicesContainer.Instance.GetFraudCheckClient(configName);
            return client.ProcessFraud(this);
        }
        
        protected override void SetupValidations()
        {
           
            #region ENUM VALIDATION WITH FLAG ATTRIBUTE            
            /// TO ADD
            #endregion

            Validations.For(TransactionType.RiskAssess)               
               .Check(() => PaymentMethod).IsNotNull();       
        }

        public FraudBuilder<TResult> WithAmount(decimal? value)
        {
            Amount = value;
            return this;
        }

        public FraudBuilder<TResult> WithCurrency(string value)
        {
            Currency = value;
            return this;
        }

        public FraudBuilder<TResult> WithAuthenticationSource(AuthenticationSource? value)
        {
            AuthenticationSource = value;
            return this;
        }

        public FraudBuilder<TResult> WithOrderCreateDate(DateTime value)
        {
            OrderCreateDate = value;
            return this;
        }

        public FraudBuilder<TResult> WithReferenceNumber(string referenceNumber)
        {
            ReferenceNumber = referenceNumber;
            return this;
        }

        public FraudBuilder<TResult> WithAddressMatchIndicator(bool value)
        {
            AddressMatchIndicator = value;
            return this;
        }

        public FraudBuilder<TResult> WithAddress(Address address)
        {
            return WithAddress(address, AddressType.Billing);
        }
        public FraudBuilder<TResult> WithAddress(Address address, AddressType type)
        {
            if (type.Equals(AddressType.Billing)) {
                BillingAddress = address;
            }
            else {
                ShippingAddress = address;
            }
            return this;
        }

        public FraudBuilder<TResult> WithGiftCardAmount(decimal giftCardAmount)
        {
            GiftCardAmount = giftCardAmount;
            return this;
        }

        public FraudBuilder<TResult> WithGiftCardCount(int? giftCardCount)
        {
            GiftCardCount = giftCardCount;
            return this;
        }

        public FraudBuilder<TResult> WithGiftCardCurrency(string giftCardCurrency)
        {
            GiftCardCurrency = giftCardCurrency;
            return this;
        }

        public FraudBuilder<TResult> WithDeliveryEmail(string deliveryEmail)
        {
            DeliveryEmail = deliveryEmail;
            return this;
        }

        public FraudBuilder<TResult> WithDeliveryTimeFrame(DeliveryTimeFrame deliveryTimeframe)
        {
            DeliveryTimeframe = deliveryTimeframe;
            return this;
        }

        public FraudBuilder<TResult> WithShippingMethod(ShippingMethod shippingMethod)
        {
            ShippingMethod = shippingMethod;
            return this;
        }

        public FraudBuilder<TResult> WithShippingNameMatchesCardHolderName(bool? shippingNameMatchesCardHolderName)
        {
            ShippingNameMatchesCardHolderName = shippingNameMatchesCardHolderName;
            return this;
        }

        public FraudBuilder<TResult> WithPreOrderIndicator(PreOrderIndicator preOrderIndicator)
        {
            PreOrderIndicator = preOrderIndicator;
            return this;
        }

        public FraudBuilder<TResult> WithPreOrderAvailabilityDate(DateTime preOrderAvailabilityDate)
        {
            PreOrderAvailabilityDate = preOrderAvailabilityDate;
            return this;
        }

        public FraudBuilder<TResult> WithReorderIndicator(ReorderIndicator reorderIndicator)
        {
            ReorderIndicator = reorderIndicator;
            return this;
        }

        public FraudBuilder<TResult> WithOrderTransactionType(OrderTransactionType orderTransactionType)
        {
            OrderTransactionType = orderTransactionType;
            return this;
        }

        public FraudBuilder<TResult> WithCustomerAccountId(string customerAccountId)
        {
            CustomerAccountId = customerAccountId;
            return this;
        }

        public FraudBuilder<TResult> WithAccountAgeIndicator(AgeIndicator ageIndicator)
        {
            AccountAgeIndicator = ageIndicator;
            return this;
        }

        public FraudBuilder<TResult> WithAccountCreateDate(DateTime accountCreateDate)
        {
            AccountCreateDate = accountCreateDate;
            return this;
        }

        public FraudBuilder<TResult> WithAccountChangeDate(DateTime accountChangeDate)
        {
            AccountChangeDate = accountChangeDate;
            return this;
        }

        public FraudBuilder<TResult> WithAccountChangeIndicator(AgeIndicator accountChangeIndicator)
        {
            AccountChangeIndicator = accountChangeIndicator;
            return this;
        }

        public FraudBuilder<TResult> WithPasswordChangeDate(DateTime passwordChangeDate)
        {
            PasswordChangeDate = passwordChangeDate;
            return this;
        }

        public FraudBuilder<TResult> WithPasswordChangeIndicator(AgeIndicator passwordChangeIndicator)
        {
            PasswordChangeIndicator = passwordChangeIndicator;
            return this;
        }

        public FraudBuilder<TResult> WithPhoneNumber(string phoneCountryCode, string number, PhoneNumberType type)
        {
            var phoneNumber = new PhoneNumber();
            phoneNumber.CountryCode = phoneCountryCode;
            phoneNumber.Number = number;            
            switch (type) {
                case PhoneNumberType.Home:
                    HomeNumber = number;
                    HomeCountryCode = phoneCountryCode;
                    break;
                case PhoneNumberType.Work:
                    WorkNumber = number;
                    WorkCountryCode = phoneCountryCode;
                    break;
                case PhoneNumberType.Mobile:
                    MobileNumber = number;
                    MobileCountryCode = phoneCountryCode;
                    break;
                default:
                    break;
            }

            return this;
        }

        public FraudBuilder<TResult> WithPaymentAccountCreateDate(DateTime paymentAccountCreateDate)
        {
            PaymentAccountCreateDate = paymentAccountCreateDate;
            return this;
        }

        public FraudBuilder<TResult> WithPaymentAccountAgeIndicator(AgeIndicator paymentAgeIndicator)
        {
            PaymentAgeIndicator = paymentAgeIndicator;
            return this;
        }

        public FraudBuilder<TResult> WithPreviousSuspiciousActivity(bool? previousSuspiciousActivity)
        {
            PreviousSuspiciousActivity = previousSuspiciousActivity;
            return this;
        }

        public FraudBuilder<TResult> WithNumberOfPurchasesInLastSixMonths(int? numberOfPurchasesInLastSixMonths)
        {
            NumberOfPurchasesInLastSixMonths = numberOfPurchasesInLastSixMonths;
            return this;
        }

        public FraudBuilder<TResult> WithNumberOfTransactionsInLast24Hours(int? numberOfTransactionsInLast24Hours)
        {
            NumberOfTransactionsInLast24Hours = numberOfTransactionsInLast24Hours;
            return this;
        }

        public FraudBuilder<TResult> WithNumberOfTransactionsInLastYear(int? numberOfTransactionsInLastYear)
        {
            NumberOfTransactionsInLastYear = numberOfTransactionsInLastYear;
            return this;
        }

        public FraudBuilder<TResult> WithNumberOfAddCardAttemptsInLast24Hours(int? numberOfAddCardAttemptsInLast24Hours)
        {
            NumberOfAddCardAttemptsInLast24Hours = numberOfAddCardAttemptsInLast24Hours;
            return this;
        }

        public FraudBuilder<TResult> WithShippingAddressCreateDate(DateTime shippingAddressCreateDate)
        {
            ShippingAddressCreateDate = shippingAddressCreateDate;
            return this;
        }

        public FraudBuilder<TResult> WithShippingAddressUsageIndicator(AgeIndicator shippingAddressUsageIndicator)
        {
            ShippingAddressUsageIndicator = shippingAddressUsageIndicator;
            return this;
        }

        public FraudBuilder<TResult> WithPriorAuthenticationMethod(PriorAuthenticationMethod priorAuthenticationMethod)
        {
            PriorAuthenticationMethod = priorAuthenticationMethod;
            return this;
        }

        public FraudBuilder<TResult> WithPriorAuthenticationTransactionId(string priorAuthencitationTransactionId)
        {
            PriorAuthenticationTransactionId = priorAuthencitationTransactionId;
            return this;
        }

        public FraudBuilder<TResult> WithPriorAuthenticationTimestamp(DateTime priorAuthenticationTimestamp)
        {
            PriorAuthenticationTimestamp = priorAuthenticationTimestamp;
            return this;
        }

        public FraudBuilder<TResult> WithPriorAuthenticationData(string priorAuthenticationData)
        {
            PriorAuthenticationData = priorAuthenticationData;
            return this;
        }

        public FraudBuilder<TResult> WithMaxNumberOfInstallments(int? maxNumberOfInstallments)
        {
            MaxNumberOfInstallments = maxNumberOfInstallments;
            return this;
        }

        public FraudBuilder<TResult> WithRecurringAuthorizationFrequency(int? recurringAuthorizationFrequency)
        {
            RecurringAuthorizationFrequency = recurringAuthorizationFrequency;
            return this;
        }

        public FraudBuilder<TResult> WithRecurringAuthorizationExpiryDate(DateTime recurringAuthorizationExpiryDate)
        {
            RecurringAuthorizationExpiryDate = recurringAuthorizationExpiryDate;
            return this;
        }

        public FraudBuilder<TResult> WithCustomerAuthenticationData(string customerAuthenticationData)
        {
            CustomerAuthenticationData = customerAuthenticationData;
            return this;
        }

        public FraudBuilder<TResult> WithCustomerAuthenticationTimestamp(DateTime customerAuthenticationTimestamp)
        {
            CustomerAuthenticationTimestamp = customerAuthenticationTimestamp;
            return this;
        }

        public FraudBuilder<TResult> WithCustomerAuthenticationMethod(CustomerAuthenticationMethod customerAuthenticationMethod)
        {
            CustomerAuthenticationMethod = customerAuthenticationMethod;
            return this;
        }
       
        public FraudBuilder<TResult> WithIdempotencyKey(string value)
        {
            IdempotencyKey = value;
            return this;
        }

        public FraudBuilder<TResult> WithBrowserData(BrowserData value)
        {
            BrowserData = value;
            return this;
        }

        public FraudBuilder<TResult> WithPaymentMethod<T>(IPaymentMethod value)
        {
            PaymentMethod = value;
            return this;
        }
    }
}
