using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System.Linq;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal class GetTokenRequest : BillPayRequestBase {
        public GetTokenRequest(ElementTree et) : base(et) { }

        public string Build(Element envelope, AuthorizationBuilder builder, Credentials credentials) {
            var body = et.SubElement(envelope, "soapenv:Body");
            var methodElement = et.SubElement(body, "bil:GetToken");
            var requestElement = et.SubElement(methodElement, "bil:GetTokenRequest");
            var card = builder.PaymentMethod as CreditCardData;
            var customer = builder.CustomerData;

            BuildCredentials(requestElement, credentials);

            var accountHolderDataElement = et.SubElement(requestElement, "bdms:AccountHolderData");
            et.SubElement(accountHolderDataElement, "pos:FirstName", card.CardHolderName.Split(' ').First());
            et.SubElement(accountHolderDataElement, "pos:LastName", card.CardHolderName.Split(' ').Last());
            et.SubElement(accountHolderDataElement, "pos:NameOnCard", card.CardHolderName);

            if (builder.BillingAddress != null) {
                et.SubElement(accountHolderDataElement, "pos:Address", builder.BillingAddress?.StreetAddress1);
                et.SubElement(accountHolderDataElement, "pos:City", builder.BillingAddress?.City);
                et.SubElement(accountHolderDataElement, "pos:State", builder.BillingAddress?.State);
                et.SubElement(accountHolderDataElement, "pos:Zip", builder.BillingAddress?.PostalCode);
            }
            else if (customer!= null && customer.Address != null) {
                et.SubElement(accountHolderDataElement, "pos:Address", customer.Address?.StreetAddress1);
                et.SubElement(accountHolderDataElement, "pos:City", customer.Address?.City);
                et.SubElement(accountHolderDataElement, "pos:State", customer.Address?.State);
                et.SubElement(accountHolderDataElement, "pos:Zip", customer.Address?.PostalCode);
            }

            if (customer != null) {
                et.SubElement(accountHolderDataElement, "pos:BusinessName", customer?.Company);
                et.SubElement(accountHolderDataElement, "pos:Email", customer?.Email);
                et.SubElement(accountHolderDataElement, "pos:MiddleName", customer?.MiddleName);

                if (!string.IsNullOrEmpty(customer.WorkPhone)) {
                    et.SubElement(accountHolderDataElement, "pos:Phone", customer?.WorkPhone);
                }
                else if (!string.IsNullOrEmpty(customer.MobilePhone)) {
                    et.SubElement(accountHolderDataElement, "pos:Phone", customer?.MobilePhone);
                }
                else if (!string.IsNullOrEmpty(customer.HomePhone)) {
                    et.SubElement(accountHolderDataElement, "pos:Phone", customer?.HomePhone);
                }

                // PLACEHOLDER PhoneRegionCode
            }

            et.SubElement(requestElement, "bdms:AccountNumber", card.Number);
            // PLACEHOLDER ClearTrackData
            // PLACEHOLDER E3KTB
            // PLACEHOLDER e3TrackData
            // PLACEHOLDER e3TrackType
            et.SubElement(requestElement, "bdms:ExpirationMonth", card.ExpMonth);
            et.SubElement(requestElement, "bdms:ExpirationYear", card.ExpYear);
            et.SubElement(requestElement, "bdms:PaymentMethod", GetPaymentMethodType(builder.PaymentMethod.PaymentMethodType));

            return et.ToString(envelope);
        }
    }
}