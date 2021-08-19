using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal class GetTransactionByOrderIDRequest : BillPayRequestBase {
        public GetTransactionByOrderIDRequest(ElementTree et) : base(et) { }

        public string Build<T>(Element envelope, ReportBuilder<T> builder, Credentials credentials) where T : class {
            if (builder is TransactionReportBuilder<T> trb) {
                var body = et.SubElement(envelope, "soapenv:Body");
                var methodElement = et.SubElement(body, "bil:GetTransactionByOrderID");
                var requestElement = et.SubElement(methodElement, "bil:GetTransactionByOrderIDRequest");

                BuildCredentials(requestElement, credentials);

                et.SubElement(requestElement, "bdms:OrderID", trb.TransactionId);

                return et.ToString(envelope);
            }
            else
                throw new BuilderException("This method only supports TransactionReportBuilder");
        }
    }
}
