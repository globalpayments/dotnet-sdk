using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Entities.PayFac
{
    public class GrossBillingInformation {
        public Address GrossSettleAddress { get; set; }
        public BankAccountData GrossSettleBankData { get; set; }
        public CreditCardData GrossSettleCreditCardData { get; set; }

        public GrossBillingInformation() {
            GrossSettleAddress = new Address();
            GrossSettleBankData = new BankAccountData();
            GrossSettleCreditCardData = new CreditCardData();
        }
    }
}
