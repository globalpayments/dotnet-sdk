using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal sealed class MakeQuickPayBlindPaymentReturnTokenRequest : BillPayRequestBase {
        public MakeQuickPayBlindPaymentReturnTokenRequest(ElementTree et) : base(et) { }

        public string Build(Element envelope, AuthorizationBuilder builder, Credentials credentials) {
            var body = et.SubElement(envelope, "soapenv:Body");
            var methodElement = et.SubElement(body, "bil:MakeQuickPayBlindPaymentReturnToken");
            var requestElement = et.SubElement(methodElement, "bil:request");

            bool hasToken = (builder.PaymentMethod is ITokenizable tokenData && !string.IsNullOrWhiteSpace(tokenData.Token));
            // Would EntryMethod.Manual be clear Swipe?
            bool hasCardData = (builder.PaymentMethod is ICardData cardData && !string.IsNullOrWhiteSpace(cardData.Number));
            bool hasACHData = (builder.PaymentMethod is eCheck && !string.IsNullOrWhiteSpace(((eCheck)builder.PaymentMethod).AccountNumber));

            // Quick Pay token MUST be present
            if (!hasToken) {
                throw new UnsupportedTransactionException("Quick Pay token must be provided for this transaction");
            }

            ValidateTransaction(builder);

            BuildCredentials(requestElement, credentials);

            if (builder.PaymentMethod is eCheck eCheck) {
                if (!string.IsNullOrWhiteSpace(eCheck.Token)) {
                    BuildQuickPayACHAccount(requestElement, eCheck, builder.Amount ?? 0, builder.ConvenienceAmount);
                }
                else {
                    throw new UnsupportedTransactionException("Quick Pay token must be provided for this transaction");
                }
            }

            var billTransactions = et.SubElement(requestElement, "bdms:BillTransactions");
            BuildBillTransactions(billTransactions, builder.Bills, "bdms:BillTransaction", "bdms:AmountToApplyToBill");
            // PLACEHOLDER: ClearSwipe

            if (builder.PaymentMethod is CreditCardData creditCard) {
                if (!string.IsNullOrWhiteSpace(creditCard.Token)) {
                    BuildQuickPayCardToCharge(requestElement, creditCard, builder.Amount ?? 0, builder.ConvenienceAmount, builder.BillingAddress);
                }
                else {
                    throw new UnsupportedTransactionException("Quick Pay token must be provided for this transaction");
                }
            }

            // PLACEHOLDER: E3Credit
            // PLACEHOLDER: E3DebitWithPIN
            et.SubElement(requestElement, "bdms:EndUserBrowserType", browserType);
            et.SubElement(requestElement, "bdms:EndUserIPAddress", builder.CustomerIpAddress);
            et.SubElement(requestElement, "bdms:OrderID", builder.OrderId);
            // PLACEHOLDER: PAXDevices
            // PLACEHOLDER: TimeoutInSeconds

            BuildTransaction(requestElement, builder);

            return et.ToString(envelope);
        }
    }
}
