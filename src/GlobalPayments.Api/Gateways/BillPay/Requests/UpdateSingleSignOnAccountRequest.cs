using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal sealed class UpdateSingleSignOnAccountRequest : BillPayRequestBase {
        public UpdateSingleSignOnAccountRequest(ElementTree et) : base(et) { }

        public string Build(Element envelope, Credentials credentials, Customer customer) {
            var body = et.SubElement(envelope, "soapenv:Body");
            var methodElement = et.SubElement(body, "bil:UpdateSingleSignOnAccount");
            var requestElement = et.SubElement(methodElement, "bil:request");

            BuildCredentials(requestElement, credentials);

            var customerElement = et.SubElement(requestElement, "bdms:Customer");

            BuildCustomer(customerElement, customer);

            et.SubElement(requestElement, "bdms:MerchantCustomerIDToUpdate", customer.Id);

            return et.ToString(envelope);
        }
    }
}
