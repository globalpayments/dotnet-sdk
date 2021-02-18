using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System.Linq;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal sealed class ReversePaymentRequest : BillPayRequestBase {
        public ReversePaymentRequest(ElementTree et) : base(et) { }

        public string Build(Element envelope, ManagementBuilder builder, Credentials credentials) {
            var body = et.SubElement(envelope, "soapenv:Body");
            var methodElement = et.SubElement(body, "bil:ReversePayment");
            var requestElement = et.SubElement(methodElement, "bil:ReversePaymentRequest");

            ValidateReversal(builder);

            BuildCredentials(requestElement, credentials);

            if (builder.Amount > 0M) {
                et.SubElement(requestElement, "bdms:BaseAmountToRefund", builder.Amount);
            }

            var billsToReverse = et.SubElement(requestElement, "bdms:BillsToReverse");

            if (builder.Bills != null && builder.Bills.Count() > 0) {
                BuildBillTransactions(billsToReverse, builder.Bills, "bdms:ReversalBillTransaction", "bdms:AmountToReverse");
            }

            et.SubElement(requestElement, "bdms:EndUserBrowserType", browserType);
            et.SubElement(requestElement, "bdms:EndUserIPAddress", builder.CustomerIpAddress);
            et.SubElement(requestElement, "bdms:ExpectedFeeAmountToRefund", builder.ConvenienceAmount);
            et.SubElement(requestElement, "bdms:OrderIDOfReversal", builder.OrderId);
            // PLACEHOLDER ReversalReason
            et.SubElement(requestElement, "bdms:Transaction_ID", (builder.PaymentMethod as TransactionReference)?.TransactionId);

            return et.ToString(envelope);
        }
    }
}