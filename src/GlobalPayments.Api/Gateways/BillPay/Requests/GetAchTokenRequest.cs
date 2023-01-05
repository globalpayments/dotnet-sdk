using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
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
            var customer = builder.CustomerData;

            BuildCredentials(requestElement, credentials);

            et.SubElement(requestElement, "bdms:ACHAccountType", GetDepositType(ach.CheckType.Value));
            et.SubElement(requestElement, "bdms:ACHDepositType", GetACHAccountType(ach.AccountType.Value));
            et.SubElement(requestElement, "bdms:ACHStandardEntryClass", ach.SecCode);

            var accountHolderDataElement = et.SubElement(requestElement, "bdms:AccountHolderData");
            et.SubElement(accountHolderDataElement, "pos:FirstName", ach.CheckHolderName.Split(' ').First());
            et.SubElement(accountHolderDataElement, "pos:LastName", ach.CheckHolderName.Split(' ').Last());
            et.SubElement(accountHolderDataElement, "pos:NameOnCard", ach.CheckHolderName);

            // ACH Tokenization fails with address information present
            //if (builder.BillingAddress != null) {
            //    et.SubElement(accountHolderDataElement, "pos:Address", builder.BillingAddress?.StreetAddress1);
            //    et.SubElement(accountHolderDataElement, "pos:City", builder.BillingAddress?.City);
            //    et.SubElement(accountHolderDataElement, "pos:State", builder.BillingAddress?.State);
            //    et.SubElement(accountHolderDataElement, "pos:Zip", builder.BillingAddress?.PostalCode);
            //}
            //else if (customer != null && customer.Address != null) {
            //    et.SubElement(accountHolderDataElement, "pos:Address", customer.Address?.StreetAddress1);
            //    et.SubElement(accountHolderDataElement, "pos:City", customer.Address?.City);
            //    et.SubElement(accountHolderDataElement, "pos:State", customer.Address?.State);
            //    et.SubElement(accountHolderDataElement, "pos:Zip", customer.Address?.PostalCode);
            //}

            if (customer != null) {
                et.SubElement(accountHolderDataElement, "pos:BusinessName", customer?.Company);
                et.SubElement(accountHolderDataElement, "pos:Email", customer?.Email);
                // FIRST NAME (Taken from ACH info earlier in method)
                // LAST NAME (Taken from ACH info earlier in method)
                et.SubElement(accountHolderDataElement, "pos:MiddleName", customer?.MiddleName);

                // NAME ON CARD (Taken from ACH info earlier in method)

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

            et.SubElement(requestElement, "bdms:AccountNumber", ach.AccountNumber);
            et.SubElement(requestElement, "bdms:PaymentMethod", GetPaymentMethodType(builder.PaymentMethod.PaymentMethodType));
            et.SubElement(requestElement, "bdms:RoutingNumber", ach.RoutingNumber);

            return et.ToString(envelope);
        }
    }
}
