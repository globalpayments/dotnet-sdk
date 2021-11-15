using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal class GetTokenRequest : BillPayRequestBase {
        public GetTokenRequest(ElementTree et) : base(et) { }

        public string Build(Element envelope, AuthorizationBuilder builder, Credentials credentials) {
            var body = et.SubElement(envelope, "soapenv:Body");
            var methodElement = et.SubElement(body, "bil:GetToken");
            var requestElement = et.SubElement(methodElement, "bil:GetTokenRequest");
            var card = builder.PaymentMethod as CreditCardData;

            BuildCredentials(requestElement, credentials);

            var accountHolderData = et.SubElement(requestElement, "bdms:AccountHolderData");
            et.SubElement(accountHolderData, "pos:Zip", builder.BillingAddress?.PostalCode);
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