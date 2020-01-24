using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests {
    [TestClass]
    public class BuilderValidationTests {
        CreditCardData card;

        [TestInitialize]
        public void Init() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w"
            });

            card = new CreditCardData {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123",
                CardHolderName = "Joe Smith"
            };
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditAuthNoAmount() {
            card.Authorize().Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditAuthNoCurrency() {
            card.Authorize(14m).Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditAuthNoPaymentMethod() {
            card.Authorize(14m).WithCurrency("USD").WithPaymentMethod(null).Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditSaleNoAmount() {
            card.Charge().Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditSaleNoCurrency() {
            card.Charge(14m).Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditSaleNoPaymentMethod() {
            card.Charge(14m).WithCurrency("USD").WithPaymentMethod(null).Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditOfflineNoAmount() {
            card.Authorize().WithOfflineAuthCode("12345").Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditOfflineNoCurrency() {
            card.Authorize(14m).WithOfflineAuthCode("12345").Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditOfflineNoAuthCode() {
            card.Authorize(14m).WithCurrency("USD").WithOfflineAuthCode(null).Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void GiftNoReplacementCard() {
            var gift = new GiftCard { Alias = "1234567890" };
            gift.ReplaceWith(null).Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CheckNoAddress() {
            new eCheck().Charge(14m).WithCurrency("USD").Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void BenefitNoCurrency() {
            new EBTTrackData().BenefitWithdrawal(10m).Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void BenefitNoAmount() {
            new EBTTrackData().BenefitWithdrawal().WithCurrency("USD").Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void BenefitNoPaymentMethod() {
            new EBTTrackData().BenefitWithdrawal().WithCurrency("USD").WithPaymentMethod(null).Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void ReportTransactionDetailNoTransactionId() {
            ReportingService.TransactionDetail(null).Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void ReportActivityWithTransactionId() {
            ReportingService.Activity()
                .WithTransactionId("1234567890")
                .Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void RecurringOneTimeWithShippingAmount() {
            new RecurringPaymentMethod().Charge(10m).WithCurrency("USD").WithShippingAmt(3m).Execute();
        }
    }
}
