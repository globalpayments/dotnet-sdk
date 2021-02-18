using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal sealed class CreateCustomerAccountRequest: BillPayRequestBase {
        public CreateCustomerAccountRequest(ElementTree et) : base(et) { }

        public string Build(Element envelope, Credentials credentials, RecurringPaymentMethod paymentMethod) {
            var body = et.SubElement(envelope, "soapenv:Body");
            var methodElement = et.SubElement(body, "bil:SaveCustomerAccount");
            var requestElement = et.SubElement(methodElement, "bil:SaveCustomerAccountRequest");

            BuildCredentials(requestElement, credentials);

            var customerAccountElement = et.SubElement(requestElement, "bdms:CustomerAccount");

            string accountNumber = "";
            string routingNumber = "";
            string bankName = "";
            int? expMonth = null;
            int? expYear = null;
            if (paymentMethod.PaymentMethod is eCheck eCheck) {
                et.SubElement(customerAccountElement, "bdms:ACHAccountType", GetDepositType(eCheck.CheckType.Value));
                et.SubElement(customerAccountElement, "bdms:ACHDepositType", GetACHAccountType(eCheck.AccountType.Value));
                accountNumber = eCheck.AccountNumber;
                routingNumber = eCheck.RoutingNumber;
                bankName = eCheck.BankName;
            }

            var accountHolder = et.SubElement(customerAccountElement, "bdms:AccountHolderData");
            BuildAccountHolderData(accountHolder, paymentMethod.Address, paymentMethod.NameOnAccount);

            if (paymentMethod.PaymentMethod is CreditCardData credit) {
                accountNumber = credit.Number;
                expMonth = credit.ExpMonth;
                expYear = credit.ExpYear;
                bankName = credit.BankName;
            }
            et.SubElement(customerAccountElement, "bdms:AccountNumber", accountNumber);
            if (string.IsNullOrWhiteSpace(bankName)) {
                // Need to explicity set the empty value
                et.SubElement(customerAccountElement, "bdms:BankName");
            } else {
                et.SubElement(customerAccountElement, "bdms:BankName", bankName);
            }
            et.SubElement(customerAccountElement, "bdms:CustomerAccountName", paymentMethod.Id);
            et.SubElement(customerAccountElement, "bdms:ExpirationMonth", expMonth);
            et.SubElement(customerAccountElement, "bdms:ExpirationYear", expYear);
            et.SubElement(customerAccountElement, "bdms:IsCustomerDefaultAccount", SerializeBooleanValues(paymentMethod.PreferredPayment));
            et.SubElement(customerAccountElement, "bdms:RoutingNumber", routingNumber);
            //   <bdms:Token>?</bdms:Token>
            et.SubElement(customerAccountElement, "bdms:TokenPaymentMethod", GetPaymentMethodType(paymentMethod.PaymentMethod.PaymentMethodType));

            et.SubElement(requestElement, "bdms:MerchantCustomerID", paymentMethod.CustomerKey);

            return et.ToString(envelope);
        }
    }
}
