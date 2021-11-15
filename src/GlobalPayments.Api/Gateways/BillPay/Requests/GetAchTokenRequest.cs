using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System.Linq;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal sealed class GetAchTokenRequest : BillPayRequestBase {
        public GetAchTokenRequest(ElementTree et) : base(et) { }

        public string Build(Element envelope, AuthorizationBuilder builder, Credentials credentials) {
            var body = et.SubElement(envelope, "soapenv:Body");
            var methodElement = et.SubElement(body, "bil:GetToken");
            var requestElement = et.SubElement(methodElement, "bil:GetTokenRequest");
            var ach = builder.PaymentMethod as eCheck;

            BuildCredentials(requestElement, credentials);

            et.SubElement(requestElement, "bdms:ACHAccountType", GetDepositType(ach.CheckType.Value));
            et.SubElement(requestElement, "bdms:ACHDepositType", GetACHAccountType(ach.AccountType.Value));
            et.SubElement(requestElement, "bdms:ACHStandardEntryClass", ach.SecCode);

            var accountHolderDataElement = et.SubElement(requestElement, "bdms:AccountHolderData");
            et.SubElement(accountHolderDataElement, "pos:LastName", ach.CheckHolderName.Split(' ').Last());

            et.SubElement(requestElement, "bdms:AccountNumber", ach.AccountNumber);
            et.SubElement(requestElement, "bdms:PaymentMethod", GetPaymentMethodType(builder.PaymentMethod.PaymentMethodType));
            et.SubElement(requestElement, "bdms:RoutingNumber", ach.RoutingNumber);

            return et.ToString(envelope);
        }
    }
}
