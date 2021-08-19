using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal abstract class BillPayRequestBase {
        const int version = 3092;
        const int applicationId = 3;
        public const string browserType = ".Net SDK";
        internal readonly ElementTree et;

        public BillPayRequestBase(ElementTree et) {
            this.et = et;
        }

        /// <summary>
        /// Builds the credentials element
        /// </summary>
        /// <param name="parent">The element to add children elements under</param>
        /// <param name="credentials">The credential object containing merchant credentials to authenticate the request</param>
        protected void BuildCredentials(Element parent, Credentials credentials) {
            et.SubElement(parent, "bdms:BollettaVersion", version);
            var credential = et.SubElement(parent, "bdms:Credential");
            et.SubElement(credential, "bdms:ApiKey", credentials.ApiKey);
            et.SubElement(credential, "bdms:ApplicationID", applicationId);
            et.SubElement(credential, "bdms:Password", credentials.Password);
            et.SubElement(credential, "bdms:UserName", credentials.UserName);
            et.SubElement(credential, "bdms:MerchantName", credentials.MerchantName);
        }

        /// <summary>
        /// Builds the ACH Account section of the request
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="eCheck"></param>
        /// <param name="amountToCharge"></param>
        /// <param name="feeAmount"></param>
        protected void BuildACHAccount(Element parent, eCheck eCheck, decimal amountToCharge, decimal? feeAmount) {
            var achAccounts = et.SubElement(parent, "bdms:ACHAccountsToCharge");
            var achAccount = et.SubElement(achAccounts, "bdms:ACHAccountToCharge");
            et.SubElement(achAccount, "bdms:Amount", amountToCharge);
            et.SubElement(achAccount, "bdms:ExpectedFeeAmount", feeAmount ?? 0M);
            // PLACEHOLDER: ACHReturnEmailAddress
            et.SubElement(achAccount, "bdms:ACHStandardEntryClass", eCheck.SecCode);
            et.SubElement(achAccount, "bdms:AccountNumber", eCheck.AccountNumber);
            et.SubElement(achAccount, "bdms:AccountType", GetDepositType(eCheck.CheckType.Value));
            et.SubElement(achAccount, "bdms:DepositType", GetACHAccountType(eCheck.AccountType.Value));
            // PLACEHOLDER: DocumentID
            // PLACEHOLDER: InternalAccountNumber
            et.SubElement(achAccount, "bdms:PayorName", eCheck.CheckHolderName);
            et.SubElement(achAccount, "bdms:RoutingNumber", eCheck.RoutingNumber);
            // PLACEHOLDER: SendEmailOnReturn
            // PLACEHOLDER: SubmitDate
            // PLACEHOLDER: TrackingNumber
        }

        /// <summary>
        /// Builds the Quick Pay ACH Account section of the request
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="eCheck"></param>
        /// <param name="amountToCharge"></param>
        /// <param name="feeAmount"></param>
        protected void BuildQuickPayACHAccount(Element parent, eCheck eCheck, decimal amountToCharge, decimal? feeAmount) {
            var achAccount = et.SubElement(parent, "bdms:QuickPayACHAccountToCharge");
            et.SubElement(achAccount, "bdms:Amount", amountToCharge);
            et.SubElement(achAccount, "bdms:ExpectedFeeAmount", feeAmount ?? 0M);
            // PLACEHOLDER: ACHReturnEmailAddress
            et.SubElement(achAccount, "bdms:ACHStandardEntryClass", eCheck.SecCode);
            et.SubElement(achAccount, "bdms:AccountNumber", eCheck.AccountNumber);
            et.SubElement(achAccount, "bdms:AccountType", GetDepositType(eCheck.CheckType.Value));
            et.SubElement(achAccount, "bdms:DepositType", GetACHAccountType(eCheck.AccountType.Value));
            // PLACEHOLDER: DocumentID
            // PLACEHOLDER: InternalAccountNumber
            et.SubElement(achAccount, "bdms:PayorName", eCheck.CheckHolderName);
            et.SubElement(achAccount, "bdms:RoutingNumber", eCheck.RoutingNumber);
            // PLACEHOLDER: SendEmailOnReturn
            // PLACEHOLDER: SubmitDate
            // PLACEHOLDER: TrackingNumber
            et.SubElement(achAccount, "bdms:QuickPayToken", eCheck.Token);
        }

        /// <summary>
        /// Builds a list of BillPay Bill Transactions from a list of Bills
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="bills"></param>
        protected void BuildBillTransactions(Element parent, IEnumerable<Bill> bills, string billLabel, string amountLabel) {
            foreach (var bill in bills) {
                var billTransaction = et.SubElement(parent, billLabel);
                et.SubElement(billTransaction, "bdms:BillType", bill.BillType);
                et.SubElement(billTransaction, "bdms:ID1", bill.Identifier1);
                et.SubElement(billTransaction, "bdms:ID2", bill.Identifier2);
                et.SubElement(billTransaction, "bdms:ID3", bill.Identifier3);
                et.SubElement(billTransaction, "bdms:ID4", bill.Identifier4);
                et.SubElement(billTransaction, amountLabel, bill.Amount);
            }
        }

        /// <summary>
        /// Builds a BillPay ClearTextCredit card from CreditCardData
        /// </summary>
        /// <param name="et"></param>
        /// <param name="parent"></param>
        /// <param name="card"></param>
        /// <param name="amountToCharge"></param>
        protected void BuildClearTextCredit(Element parent, CreditCardData card, decimal amountToCharge, decimal? feeAmount, EmvFallbackCondition? condition, EmvLastChipRead? lastRead, Address address) {
            bool isEmvFallback = condition == EmvFallbackCondition.ChipReadFailure;
            bool isPreviousEmvFallback = lastRead == EmvLastChipRead.Failed;

            var clearTextCards = et.SubElement(parent, "bdms:ClearTextCreditCardsToCharge");
            var clearTextCard = et.SubElement(clearTextCards, "bdms:ClearTextCardToCharge");
            et.SubElement(clearTextCard, "bdms:Amount", amountToCharge);
            et.SubElement(clearTextCard, "bdms:CardProcessingMethod", "Credit");
            et.SubElement(clearTextCard, "bdms:ExpectedFeeAmount", feeAmount ?? 0);

            var clearTextCredit = et.SubElement(clearTextCard, "bdms:ClearTextCreditCard");

            var cardHolder = et.SubElement(clearTextCredit, "pos:CardHolderData");
            BuildAccountHolderData(cardHolder,
                address,
                card.CardHolderName);

            et.SubElement(clearTextCredit, "pos:CardNumber", card.Number);
            et.SubElement(clearTextCredit, "pos:ExpirationMonth", card.ExpMonth);
            et.SubElement(clearTextCredit, "pos:ExpirationYear", card.ExpYear);
            et.SubElement(clearTextCredit, "pos:IsEmvFallback", isEmvFallback);
            et.SubElement(clearTextCredit, "pos:PreviousEmvAlsoFallback", isPreviousEmvFallback);
            et.SubElement(clearTextCredit, "pos:VerificationCode", card.Cvn);
        }

        /// <summary>
        /// Builds a Quick Pay BillPay ClearTextCredit card from CreditCardData
        /// </summary>
        /// <param name="et"></param>
        /// <param name="parent"></param>
        /// <param name="card"></param>
        /// <param name="amountToCharge"></param>
        protected void BuildQuickPayCardToCharge(Element parent, CreditCardData card, decimal amountToCharge, decimal? feeAmount, Address address) {
            var cardToCharge = et.SubElement(parent, "bdms:QuickPayCardToCharge");
            et.SubElement(cardToCharge, "bdms:Amount", amountToCharge);
            et.SubElement(cardToCharge, "bdms:CardProcessingMethod", "Credit");
            et.SubElement(cardToCharge, "bdms:ExpectedFeeAmount", feeAmount ?? 0);

            var cardHolder = et.SubElement(cardToCharge, "pos:CardHolderData");
            BuildAccountHolderData(cardHolder,
                address,
                card.CardHolderName);

            et.SubElement(cardToCharge, "bdms:ExpirationMonth", card.ExpMonth);
            et.SubElement(cardToCharge, "bdms:ExpirationYear", card.ExpYear);
            et.SubElement(cardToCharge, "bdms:VerificationCode", card.Cvn);
            et.SubElement(cardToCharge, "bdms:QuickPayToken", card.Token);
        }

        /// <summary>
        /// Builds the account billing information
        /// </summary>
        /// <param name="parent">The XML element to attatch to</param>
        /// <param name="address">The billing address of the customer</param>
        /// <param name="nameOnAccount">The name on the payment account</param>
        // private void BuildAccountHolderData(Element parent, RecurringPaymentMethod recurringPaymentMethod)
        protected void BuildAccountHolderData(Element parent, Address address, string nameOnAccount) {
            et.SubElement(parent, "pos:Address", address?.StreetAddress1);
            et.SubElement(parent, "pos:City", address?.City);
            et.SubElement(parent, "pos:NameOnCard", nameOnAccount);
            et.SubElement(parent, "pos:State", address?.State);
            et.SubElement(parent, "pos:Zip", address?.PostalCode);
        }

        /// <summary>
        /// Builds a BillPay token to charge from any payment method
        /// </summary>
        /// <param name="parent">The parent XML element to attatch to</param>
        /// <param name="paymentMethod">The token to pay</param>
        /// <param name="amount">The amount to charge</param>
        /// <param name="feeAmount">The expected fee amount to charge</param>
        protected void BuildTokenToCharge(Element parent, IPaymentMethod paymentMethod, decimal amount, decimal? feeAmount) {
            var tokensToCharge = et.SubElement(parent, "bdms:TokensToCharge");
            var tokenToCharge = et.SubElement(tokensToCharge, "bdms:TokenToCharge");

            et.SubElement(tokenToCharge, "bdms:Amount", amount);
            et.SubElement(tokenToCharge, "bdms:CardProcessingMethod", GetCardProcessingMethod(paymentMethod.PaymentMethodType));
            et.SubElement(tokenToCharge, "bdms:ExpectedFeeAmount", feeAmount);
            if (paymentMethod is eCheck ach) {
                et.SubElement(tokenToCharge, "bdms:ACHStandardEntryClass", ach.SecCode);
            }
            et.SubElement(tokenToCharge, "bdms:Token", (paymentMethod as ITokenizable).Token);
        }

        /// <summary>
        /// Builds the BillPay transaction object
        /// </summary>
        /// <param name="parent"></param>
        protected void BuildTransaction(Element parent, AuthorizationBuilder builder) {
            var transaction = et.SubElement(parent, "bdms:Transaction");
            et.SubElement(transaction, "bdms:Amount", builder.Amount);
            et.SubElement(transaction, "bdms:FeeAmount", builder.ConvenienceAmount);
            et.SubElement(transaction, "bdms:MerchantInvoiceNumber", builder.InvoiceNumber);
            et.SubElement(transaction, "bdms:MerchantTransactionDescription", builder.Description);
            et.SubElement(transaction, "bdms:MerchantTransactionID", builder.ClientTransactionId);
            et.SubElement(transaction, "bdms:PayorAddress", builder.Customer?.Address?.StreetAddress1);
            et.SubElement(transaction, "bdms:PayorCity", builder.Customer?.Address?.City);
            et.SubElement(transaction, "bdms:PayorCountry", builder.Customer?.Address?.Country);
            et.SubElement(transaction, "bdms:PayorEmailAddress", builder.Customer?.Email);
            et.SubElement(transaction, "bdms:PayorFirstName", builder.Customer?.FirstName);
            et.SubElement(transaction, "bdms:PayorLastName", builder.Customer?.LastName);
            et.SubElement(transaction, "bdms:PayorPhoneNumber", builder.Customer?.HomePhone);
            et.SubElement(transaction, "bdms:PayorPostalCode", builder.Customer?.Address?.PostalCode);
            et.SubElement(transaction, "bdms:PayorState", builder.Customer?.Address?.State);
        }

        protected void BuildCustomer(Element parent, Customer customer) {
            et.SubElement(parent, "bdms:Address", customer.Address?.StreetAddress1);
            et.SubElement(parent, "bdms:City", customer.Address?.City);
            et.SubElement(parent, "bdms:Country", customer.Address?.Country);
            et.SubElement(parent, "bdms:EmailAddress", customer.Email);
            et.SubElement(parent, "bdms:FirstName", customer.FirstName);
            //et.SubElement(parent, "bdms:FullAddress");
            //et.SubElement(parent, "bdms:FullName")
            et.SubElement(parent, "bdms:LastName", customer.LastName);
            et.SubElement(parent, "bdms:MerchantCustomerID", customer.Id); // Should we create the guid or throw an error?
                                                                                                        //et.SubElement(parent, "bdms:MiddleName");
            et.SubElement(parent, "bdms:MobilePhone", customer.MobilePhone);
            et.SubElement(parent, "bdms:MobilePhoneCountry");
            et.SubElement(parent, "bdms:Phone", customer.HomePhone);
            et.SubElement(parent, "bdms:PhoneCountry");
            et.SubElement(parent, "bdms:Postal", customer.Address?.PostalCode);
            //<bdms:PreferredContactMethod>?</bdms:PreferredContactMethod>
            et.SubElement(parent, "bdms:State", customer.Address?.State);
        }

        /// <summary>
        /// Validates that the AuthorizationBuilder is configured correctly for a Bill Payment
        /// </summary>
        protected void ValidateTransaction(AuthorizationBuilder builder) {
            var validationErrors = new List<string>();

            if (builder.Bills == null || builder.Bills.Count() < 1) {
                validationErrors.Add("Bill Payments must have at least one bill to pay.");
            } else {
                if (builder.Amount != builder.Bills.Sum(x => x.Amount)) {
                    validationErrors.Add("The sum of the bill amounts must match the amount charged.");
                }
            }

            if (builder.Currency != "USD") {
                validationErrors.Add("Bill Pay only supports currency USD.");
            }

            if (validationErrors.Count > 0) {
                throw new ValidationException(validationErrors);
            }
        }

        protected void ValidateBills(IEnumerable<Bill> bills) {
            var validationErrors = new List<string>();

            if (bills == null || bills.Count() < 1) {
                validationErrors.Add("At least one Bill required to Load Bills.");
            } else {
                if (bills.Any(x => x.Amount <= 0)) {
                    validationErrors.Add("Bills require an amount greater than zero.");
                }
            }

            if (validationErrors.Count > 0) {
                throw new ValidationException(validationErrors);
            }
        }

        protected void ValidateReversal(ManagementBuilder builder) {
            var validationErrors = new List<string>();

            if (!(builder.PaymentMethod is TransactionReference reference) || string.IsNullOrWhiteSpace(reference.TransactionId)) {
                validationErrors.Add("A transaction to reverse must be provided.");
            } else {
                if (!int.TryParse(reference.TransactionId, out int transactionId) || transactionId < 1) {
                    validationErrors.Add("The transaction id to reverse must be a positive integer.");
                }
            }

            if (builder.Bills != null && builder.Bills.Count() > 0) {
                if (builder.Amount != builder.Bills.Sum(x => x.Amount)) {
                    validationErrors.Add("The sum of the bill amounts must match the amount to reverse.");
                }
            }

            if (validationErrors.Count > 0) {
                throw new ValidationException(validationErrors);
            }
        }

        protected void ValidateLoadSecurePay(HostedPaymentData hostedPaymentData) {
            var validationErrors = new List<string>();

            if (hostedPaymentData == null) {
                validationErrors.Add("HostedPaymentData Required");
            } else {
                if (hostedPaymentData.Bills == null || hostedPaymentData.Bills.Count() < 1) {
                    validationErrors.Add("At least one Bill required to Load Bills.");
                } else {
                    if (hostedPaymentData.Bills.Any(x => x.Amount <= 0)) {
                        validationErrors.Add("Bills require an amount greater than zero.");
                    }
                }

                if (hostedPaymentData.HostedPaymentType == HostedPaymentType.None) {
                    validationErrors.Add("You must set a valid HostedPaymentType.");
                }
            }

            if (validationErrors.Count > 0) {
                throw new ValidationException(validationErrors);
            }
        }

        /// <summary>
        /// These methods are here to convert SDK enums
        /// Into values that BillPay will recognize
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        protected string GetBillPresentmentType(BillPresentment billPresentment) {
            switch (billPresentment) {
                case BillPresentment.Full:
                    return "Full";
                default:
                    throw new UnsupportedTransactionException($"Bill Presentment Type of {billPresentment} is not supported");
            }
        }

        protected string GetDepositType(CheckType deposit) {
            switch (deposit) {
                case CheckType.BUSINESS:
                    return "Business";
                case CheckType.PERSONAL:
                    return "Personal";
                case CheckType.PAYROLL:
                default:
                    throw new UnsupportedTransactionException($"eCheck Deposit Type of {deposit} is not supported.");
            }
        }

        protected string GetACHAccountType(AccountType account) {
            switch (account) {
                case AccountType.CHECKING:
                    return "Checking";
                case AccountType.SAVINGS:
                    return "Savings";
                default:
                    throw new UnsupportedTransactionException($"eCheck Account Type of {account} is not supported");
            }
        }

        internal string GetCardProcessingMethod(PaymentMethodType paymentMethod) {
            switch (paymentMethod) {
                case PaymentMethodType.Credit:
                    return "Credit";
                case PaymentMethodType.Debit:
                    return "Debit";
                // Need to differentiate PINDebit
                default:
                    return "Unassigned";
            }
        }

        internal string GetPaymentMethodType(PaymentMethodType paymentMethod) {
            switch (paymentMethod) {
                case PaymentMethodType.Credit:
                    return "Credit";
                case PaymentMethodType.Debit:
                    return "Debit";
                case PaymentMethodType.ACH:
                    return "ACH";
                default:
                    throw new UnsupportedTransactionException();
            }
        }

        protected string SerializeBooleanValues(bool value) {
            return value.ToString().ToLower();
        }
    }
}
