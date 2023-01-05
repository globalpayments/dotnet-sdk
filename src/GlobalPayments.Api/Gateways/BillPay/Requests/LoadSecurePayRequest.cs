using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal sealed class LoadSecurePayRequest : BillPayRequestBase {
        public LoadSecurePayRequest(ElementTree et) : base(et) { }

        public string Build(Element envelope, BillingBuilder builder, Credentials credentials) {
            var body = et.SubElement(envelope, "soapenv:Body");
            var methodElement = et.SubElement(body, "bil:LoadSecurePayDataExtended");
            var requestElement = et.SubElement(methodElement, "bil:request");
            var customerIsEditable = SerializeBooleanValues(builder.HostedPaymentData.CustomerIsEditable);

            ValidateLoadSecurePay(builder.HostedPaymentData);

            BuildCredentials(requestElement, credentials);

            var billsElement = et.SubElement(requestElement, "bdms:BillData");

            foreach (var bill in builder.HostedPaymentData.Bills) {
                var billElement = et.SubElement(billsElement, "bdms:SecurePayBill");

                et.SubElement(billElement, "bdms:Amount", bill.Amount);
                et.SubElement(billElement, "bdms:BillTypeName", bill.BillType);
                et.SubElement(billElement, "bdms:Identifier1", bill.Identifier1);
                et.SubElement(billElement, "bdms:Identifier2", bill.Identifier2);
                et.SubElement(billElement, "bdms:Identifier3", bill.Identifier3);
                et.SubElement(billElement, "bdms:Identifier4", bill.Identifier4);
            }

            et.SubElement(requestElement, "bdms:CancelURL", builder.HostedPaymentData.CancelUrl);
            et.SubElement(requestElement, "bdms:MerchantCustomerID", builder.HostedPaymentData.CustomerKey ?? builder.HostedPaymentData.CustomerNumber ?? null);
            et.SubElement(requestElement, "bdms:OrderID", builder.OrderID);
            et.SubElement(requestElement, "bdms:PayorAddress", builder.HostedPaymentData.CustomerAddress.StreetAddress1);
            et.SubElement(requestElement, "bdms:PayorAddressIsEditable", customerIsEditable);
            et.SubElement(requestElement, "bdms:PayorBusinessNameIsEditable", customerIsEditable);
            et.SubElement(requestElement, "bdms:PayorCity", builder.HostedPaymentData.CustomerAddress.City);
            et.SubElement(requestElement, "bdms:PayorCityIsEditable", customerIsEditable);
            et.SubElement(requestElement, "bdms:PayorCountry", builder.HostedPaymentData.CustomerAddress.CountryCode);
            et.SubElement(requestElement, "bdms:PayorCountryIsEditable", customerIsEditable);
            et.SubElement(requestElement, "bdms:PayorEmailAddress", builder.HostedPaymentData.CustomerEmail);
            et.SubElement(requestElement, "bdms:PayorEmailAddressIsEditable", customerIsEditable);
            et.SubElement(requestElement, "bdms:PayorFirstName", builder.HostedPaymentData.CustomerFirstName);
            et.SubElement(requestElement, "bdms:PayorFirstNameIsEditable", customerIsEditable);
            et.SubElement(requestElement, "bdms:PayorLastName", builder.HostedPaymentData.CustomerLastName);
            et.SubElement(requestElement, "bdms:PayorLastNameIsEditable", customerIsEditable);
            et.SubElement(requestElement, "bdms:PayorMiddleNameIsEditable", customerIsEditable);
            et.SubElement(requestElement, "bdms:PayorPhoneNumber", builder.HostedPaymentData.CustomerPhoneMobile);
            et.SubElement(requestElement, "bdms:PayorPhoneNumberIsEditable", customerIsEditable);
            et.SubElement(requestElement, "bdms:PayorPostalCode", builder.HostedPaymentData.CustomerAddress.PostalCode);
            et.SubElement(requestElement, "bdms:PayorPostalCodeIsEditable", customerIsEditable);
            et.SubElement(requestElement, "bdms:PayorState", builder.HostedPaymentData.CustomerAddress.State);
            et.SubElement(requestElement, "bdms:PayorStateIsEditable", customerIsEditable);
            et.SubElement(requestElement, "bdms:ReturnURL", builder.HostedPaymentData.MerchantResponseUrl);
            et.SubElement(requestElement, "bdms:SecurePayPaymentType_ID", (int)builder.HostedPaymentData.HostedPaymentType);

            return et.ToString(envelope);
        }
    }
}
