using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal sealed class UpdateCustomerAccountRequest : BillPayRequestBase {
        public UpdateCustomerAccountRequest(ElementTree et) : base(et) { }

        public string Build(Element envelope, Credentials credentials, RecurringPaymentMethod paymentMethod) {
            var body = et.SubElement(envelope, "soapenv:Body");
            var methodElement = et.SubElement(body, "bil:UpdateCustomerAccount");
            var requestElement = et.SubElement(methodElement, "bil:UpdateCustomerAccountRequest");

            BuildCredentials(requestElement, credentials);

            string bankName = "";
            int? expMonth = null;
            int? expYear = null;
            if (paymentMethod.PaymentMethod is eCheck eCheck) {
                et.SubElement(requestElement, "bdms:ACHAccountType", GetDepositType(eCheck.CheckType.Value));
                et.SubElement(requestElement, "bdms:ACHDepositType", GetACHAccountType(eCheck.AccountType.Value));
                bankName = eCheck.BankName;
            }

            var accountHolderElement = et.SubElement(requestElement, "bdms:AccountHolderData");
            BuildAccountHolderData(accountHolderElement, paymentMethod.Address, paymentMethod.NameOnAccount);

            if (paymentMethod.PaymentMethod is CreditCardData credit) {
                expMonth = credit.ExpMonth;
                expYear = credit.ExpYear;
                bankName = credit.BankName;
            }
            if (string.IsNullOrWhiteSpace(bankName)) {
                // Need to explicity set the empty value
                et.SubElement(requestElement, "bdms:BankName");
            } else {
                et.SubElement(requestElement, "bdms:BankName", bankName);
            }
            et.SubElement(requestElement, "bdms:ExpirationMonth", expMonth);
            et.SubElement(requestElement, "bdms:ExpirationYear", expYear);
            et.SubElement(requestElement, "bdms:IsCustomerDefaultAccount", SerializeBooleanValues(paymentMethod.PreferredPayment));
            et.SubElement(requestElement, "bdms:MerchantCustomerID", paymentMethod.CustomerKey);
            et.SubElement(requestElement, "bdms:NewCustomerAccountName", paymentMethod.Id);
            et.SubElement(requestElement, "bdms:OldCustomerAccountName", paymentMethod.Id);
            et.SubElement(requestElement, "bdms:PaymentMethod", GetPaymentMethodType(paymentMethod.PaymentMethod.PaymentMethodType));

            return et.ToString(envelope);
        }
    }
}
