using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal sealed class PreloadBillsRequest : BillPayRequestBase {
        public PreloadBillsRequest(ElementTree et) : base(et) { }

        public string Build(Element envelope, BillingBuilder builder, Credentials credentials) {
            var body = et.SubElement(envelope, "soapenv:Body");
            var methodElement = et.SubElement(body, "bil:PreloadBills");
            var requestElement = et.SubElement(methodElement, "bil:PreloadBillsRequest");

            ValidateBills(builder.Bills);
            BuildCredentials(requestElement, credentials);
            var bills = et.SubElement(requestElement, "bdms:Bills");

            foreach (var bill in builder.Bills) {
                var billElement = et.SubElement(bills, "bdms:Bill");
                var billIdentifierExtended = et.SubElement(billElement, "bdms:BillIdentifierExtended");

                et.SubElement(billIdentifierExtended, "bdms:BillType", bill.BillType);
                et.SubElement(billIdentifierExtended, "bdms:ID1", bill.Identifier1);
                et.SubElement(billIdentifierExtended, "bdms:ID2", bill.Identifier2);
                et.SubElement(billIdentifierExtended, "bdms:ID3", bill.Identifier3);
                et.SubElement(billIdentifierExtended, "bdms:ID4", bill.Identifier4);
                et.SubElement(billIdentifierExtended, "bdms:DueDate", bill.DueDate.ToString("o"));

                et.SubElement(billElement, "bdms:BillPresentment", GetBillPresentmentType(bill.BillPresentment));

                if (bill.Customer != null && bill.Customer.Address != null) {
                    var customerAddress = et.SubElement(billElement, "bdms:CustomerAddress");
                    et.SubElement(customerAddress, "bdms:AddressLineOne", bill.Customer.Address.StreetAddress1);
                    et.SubElement(customerAddress, "bdms:City", bill.Customer.Address.City);
                    et.SubElement(customerAddress, "bdms:Country", bill.Customer.Address.Country);
                    et.SubElement(customerAddress, "bdms:PostalCode", bill.Customer.Address.PostalCode);
                    et.SubElement(customerAddress, "bdms:State", bill.Customer.Address.State);
                }

                et.SubElement(billElement, "bdms:MerchantCustomerId", bill.Customer?.Id);
                et.SubElement(billElement, "bdms:ObligorEmailAddress", bill.Customer?.Email);
                et.SubElement(billElement, "bdms:ObligorFirstName", bill.Customer?.FirstName);
                et.SubElement(billElement, "bdms:ObligorLastName", bill.Customer?.LastName);
                et.SubElement(billElement, "bdms:ObligorPhoneNumber", bill.Customer?.HomePhone);

                et.SubElement(billElement, "bdms:RequiredAmount", bill.Amount);
            }

            return et.ToString(envelope);
        }
    }
}
