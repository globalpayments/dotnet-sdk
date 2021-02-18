using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways.BillPay {
	internal sealed class GetConvenienceFeeRequest : BillPayRequestBase {
        public GetConvenienceFeeRequest(ElementTree et): base(et) { }

        public string Build(Element envelope, BillingBuilder builder, Credentials credentials) {
            var body = et.SubElement(envelope, "soapenv:Body");
            var methodElement = et.SubElement(body, "bil:GetConvenienceFee");
            var requestElement = et.SubElement(methodElement, "bil:GetConvenienceFeeRequest");

            string accountNumber = null;
            string routingNumber = null;
            string paymentMethod = null;

            if (builder.PaymentMethod is eCheck eCheck) {
                routingNumber = eCheck.RoutingNumber;
                paymentMethod = "ACH";
            } else if (builder.PaymentMethod is CreditCardData credit) {
                accountNumber = credit.Number;
            }

            BuildCredentials(requestElement, credentials);

            et.SubElement(requestElement, "bdms:BaseAmount", builder.Amount);
            if (accountNumber != null) {
                et.SubElement(requestElement, "bdms:CardNumber", accountNumber);
            }
            et.SubElement(requestElement, "bdms:CardProcessingMethod", GetCardProcessingMethod(builder.PaymentMethod.PaymentMethodType));
            if (paymentMethod != null) {
                et.SubElement(requestElement, "bdms:PaymentMethod", paymentMethod);
            }
            et.SubElement(requestElement, "bdms:RoutingNumber", routingNumber);

            return et.ToString(envelope);
        }
	}
}
