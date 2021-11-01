using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Entities.PayFac
{
    public class FlashFundsPaymentCardData {
        public CreditCardData CreditCard { get; set; }
        public Address CardholderAddress { get; set; }

        public FlashFundsPaymentCardData() {
            CreditCard = new CreditCardData();
            CardholderAddress = new Address();
        }
    }
}
