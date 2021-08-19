using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal class GetTokenInformationRequest : BillPayRequestBase {
        public GetTokenInformationRequest(ElementTree et) : base(et) { }

        public string Build(Element envelope, AuthorizationBuilder builder, Credentials credentials) {
            var body = et.SubElement(envelope, "soapenv:Body");
            var methodElement = et.SubElement(body, "bil:GetTokenInformation");
            var requestElement = et.SubElement(methodElement, "bil:request");
            var tokenizablePayment = builder.PaymentMethod as ITokenizable;

            BuildCredentials(requestElement, credentials);

            et.SubElement(requestElement, "bdms:Token", tokenizablePayment.Token);

            return et.ToString(envelope);
        }
    }
}
