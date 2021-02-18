using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal sealed class CommitPreloadedBillsRequest : BillPayRequestBase {
        public CommitPreloadedBillsRequest(ElementTree et) : base(et) { }

        public string Build(Element envelope, Credentials credentials) {
            var body = et.SubElement(envelope, "soapenv:Body");
            var methodElement = et.SubElement(body, "bil:CommitPreloadedBills");
            var requestElement = et.SubElement(methodElement, "bil:CommitPreloadedBillsRequest");

            BuildCredentials(requestElement, credentials);

            return et.ToString(envelope);
        }
    }
}
