using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal sealed class MakePaymentReturnTokenRequest : BillPayRequestBase {
        public MakePaymentReturnTokenRequest(ElementTree et) : base(et) { }

        public string Build(Element envelope, AuthorizationBuilder builder, Credentials credentials) {
            var body = et.SubElement(envelope, "soapenv:Body");
            var methodElement = et.SubElement(body, "bil:MakePaymentReturnToken");
            var requestElement = et.SubElement(methodElement, "bil:MakePaymentReturnTokenRequest");

            bool hasToken = (builder.PaymentMethod is ITokenizable tokenData && !string.IsNullOrWhiteSpace(tokenData.Token));
            // Would EntryMethod.Manual be clear Swipe?
            bool hasCardData = (builder.PaymentMethod is ICardData cardData && !string.IsNullOrWhiteSpace(cardData.Number));
            bool hasACHData = (builder.PaymentMethod is eCheck && !string.IsNullOrWhiteSpace(((eCheck)builder.PaymentMethod).AccountNumber));

            // Only allow token, card, and ACH data at this time
            if (!hasToken && !hasCardData && !hasACHData) {
                throw new UnsupportedTransactionException("Payment method not accepted");
            }

            ValidateTransaction(builder);

            BuildCredentials(requestElement, credentials);

            if (!hasToken && builder.PaymentMethod is eCheck eCheck) {
                BuildACHAccount(requestElement, eCheck, builder.Amount ?? 0, builder.ConvenienceAmount);
            }

            var billTransactions = et.SubElement(requestElement, "bdms:BillTransactions");
            BuildBillTransactions(billTransactions, builder.Bills, "bdms:BillTransaction", "bdms:AmountToApplyToBill");
            // PLACEHOLDER: ClearSwipe

            // ClearTextCredit
            if (hasCardData && builder.PaymentMethod is CreditCardData creditCard) {
                BuildClearTextCredit(requestElement, creditCard, builder.Amount ?? 0, builder.ConvenienceAmount, builder.EmvFallbackCondition, builder.EmvLastChipRead, builder.BillingAddress);
            }

            // PLACEHOLDER: E3Credit
            // PLACEHOLDER: E3DebitWithPIN
            et.SubElement(requestElement, "bdms:EndUserBrowserType", browserType);
            et.SubElement(requestElement, "bdms:EndUserIPAddress", builder.CustomerIpAddress);
            et.SubElement(requestElement, "bdms:OrderID", builder.OrderId);
            // PLACEHOLDER: PAXDevices
            // PLACEHOLDER: TimeoutInSeconds
            if (hasToken) {
                BuildTokenToCharge(requestElement, builder.PaymentMethod, builder.Amount ?? 0, builder.ConvenienceAmount);
            }

            BuildTransaction(requestElement, builder);

            return et.ToString(envelope);
        }
    }
}
