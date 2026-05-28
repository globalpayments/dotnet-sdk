using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {

    /// <summary>
    /// APAC eCommerce endpoint connectivity tests for GP-API.
    /// Covers all transaction flows required by AH-2131 / AH-2146 using Visa and
    /// Mastercard test cards against the Production Sandbox (GP_API_TEST).
    ///
    /// Transaction types covered:
    ///   Sale, Auth, Pre-Auth, Capture, Void, Auth Reversal, Refund,
    ///   Partial Capture ,  MOTO.
    /// </summary>
    [TestClass]
    public class GpApiApacEcommTest : BaseGpApiTests {

        // APAC merchant credentials — Production Sandbox
        private const string ApacAppId = "16Br1RfjChBrsFnWlu7NGIp9LKm2MWWFyGg3SU3UfEl3voA2";
        private const string ApacAppKey = "xV9wnRLmi8qPqvMZoxAH9S0RtoQlodCYuvCboVYUohW6DObtcrYL1uj4YOZilKyu";

        // APAC currencies
        private const string CurrencySGD = "SGD";
        private const string CurrencyHKD = "HKD";

        // Standard test amount
        private const decimal Amount = 10.00m;

        // Test card numbers (sandbox test cards only — not real PANs)
        private const string VisaCardNumber = "4263970000005262";
        private const string MastercardCardNumber = "5425230000004415";

        private CreditCardData _visaCard;
        private CreditCardData _mastercardCard;
        private CreditCardData _motoVisaCard;
        private CreditCardData _motoMastercardCard;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            var config = GpApiConfigSetup(ApacAppId, ApacAppKey, Channel.CardNotPresent);
            config.Country = "SG";
            ServicesContainer.ConfigureService(config);
        }

        [TestInitialize]
        public void TestInitialize() {
            _visaCard = new CreditCardData {
                Number = VisaCardNumber,
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "123",
                CardHolderName = "John Smith"
            };

            _mastercardCard = new CreditCardData {
                Number = MastercardCardNumber,
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "123",
                CardHolderName = "Jane Doe"
            };

            _motoVisaCard = new CreditCardData {
                Number = VisaCardNumber,
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "123",
                CardHolderName = "John Smith",
                EntryMethod = ManualEntryMethod.Moto
            };

            _motoMastercardCard = new CreditCardData {
                Number = MastercardCardNumber,
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "123",
                CardHolderName = "Jane Doe",
                EntryMethod = ManualEntryMethod.Moto
            };
        }

        #region Sale Tests

        [TestMethod]
        public void CreditSale_Visa_SGD() {
            var response = _visaCard.Charge(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CreditSale_Mastercard_SGD() {
            var response = _mastercardCard.Charge(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CreditSale_Visa_HKD() {
            var response = _visaCard.Charge(Amount)
                .WithCurrency(CurrencyHKD)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CreditSale_Mastercard_HKD() {
            var response = _mastercardCard.Charge(Amount)
                .WithCurrency(CurrencyHKD)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        #endregion

        #region Authorization Tests

        [TestMethod]
        public void CreditAuthorization_Visa() {
            var response = _visaCard.Authorize(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Preauthorized);
        }

        [TestMethod]
        public void CreditAuthorization_Mastercard() {
            var response = _mastercardCard.Authorize(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Preauthorized);
        }

        #endregion

        #region Pre-Auth Tests
        // Pre-Auth is an Authorize call — funds are reserved but not settled.

        [TestMethod]
        public void CreditPreAuth_Visa() {
            var response = _visaCard.Authorize(Amount)
                .WithCurrency(CurrencyHKD)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Preauthorized);
        }

        [TestMethod]
        public void CreditPreAuth_Mastercard() {
            var response = _mastercardCard.Authorize(Amount)
                .WithCurrency(CurrencyHKD)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Preauthorized);
        }

        #endregion

        #region Capture Tests

        [TestMethod]
        public void CreditCapture_Visa() {
            var authResponse = _visaCard.Authorize(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(authResponse, TransactionStatus.Preauthorized);

            var captureResponse = authResponse.Capture(Amount)
                .Execute();
            AssertTransactionResponse(captureResponse, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CreditCapture_Mastercard() {
            var authResponse = _mastercardCard.Authorize(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(authResponse, TransactionStatus.Preauthorized);

            var captureResponse = authResponse.Capture(Amount)
                .Execute();
            AssertTransactionResponse(captureResponse, TransactionStatus.Captured);
        }

        #endregion

        #region Void/Reverse Tests
        // Void reverses a captured (Charge) transaction before settlement.

        [TestMethod]
        public void CreditVoid_Visa() {
            var saleResponse = _visaCard.Charge(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(saleResponse, TransactionStatus.Captured);

            var voidResponse = saleResponse.Reverse(Amount)
                .Execute();
            AssertTransactionResponse(voidResponse, TransactionStatus.Reversed);
        }

        [TestMethod]
        public void CreditVoid_Mastercard() {
            var saleResponse = _mastercardCard.Charge(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(saleResponse, TransactionStatus.Captured);

            var voidResponse = saleResponse.Reverse(Amount)
                .Execute();
            AssertTransactionResponse(voidResponse, TransactionStatus.Reversed);
        }

        #endregion

        #region Auth Reversal Tests
        // Auth Reversal cancels a Preauthorized (Authorize) transaction.

        [TestMethod]
        public void CreditAuthReversal_Visa() {
            var authResponse = _visaCard.Authorize(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(authResponse, TransactionStatus.Preauthorized);

            var reversalResponse = authResponse.Reverse(Amount)
                .Execute();
            AssertTransactionResponse(reversalResponse, TransactionStatus.Reversed);
        }

        [TestMethod]
        public void CreditAuthReversal_Mastercard() {
            var authResponse = _mastercardCard.Authorize(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(authResponse, TransactionStatus.Preauthorized);

            var reversalResponse = authResponse.Reverse(Amount)
                .Execute();
            AssertTransactionResponse(reversalResponse, TransactionStatus.Reversed);
        }

        #endregion

        #region Refund Tests

        [TestMethod]
        public void CreditRefund_Visa_LinkedRefund() {
            var saleResponse = _visaCard.Charge(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(saleResponse, TransactionStatus.Captured);

            var refundResponse = saleResponse.Refund(1m)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(refundResponse, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CreditRefund_Mastercard_LinkedRefund() {
            var saleResponse = _mastercardCard.Charge(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(saleResponse, TransactionStatus.Captured);

            var refundResponse = saleResponse.Refund(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(refundResponse, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CreditRefund_Visa_StandaloneRefund() {
            var refundResponse = _visaCard.Refund(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(refundResponse, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CreditRefund_Mastercard_StandaloneRefund() {
            var refundResponse = _mastercardCard.Refund(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(refundResponse, TransactionStatus.Captured);
        }

        #endregion

        #region Partial Capture Tests (via Virtual Terminal)
        // Partial Capture: authorize for a higher amount, capture for a lower amount.

        [TestMethod]
        public void CreditPartialCapture_Visa() {
            var authResponse = _visaCard.Authorize(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(authResponse, TransactionStatus.Preauthorized);

            var captureResponse = authResponse.Capture(5.00m)
                .WithGratuity(1.00m)
                .Execute();
            AssertTransactionResponse(captureResponse, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CreditPartialCapture_Mastercard() {
            var authResponse = _mastercardCard.Authorize(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(authResponse, TransactionStatus.Preauthorized);

            var captureResponse = authResponse.Capture(5.00m)
                .WithGratuity(1.00m)
                .Execute();
            AssertTransactionResponse(captureResponse, TransactionStatus.Captured);
        }

        #endregion

        #region MOTO Tests (Manual Key-In)
        // MOTO = Mail Order / Telephone Order. Card details are manually entered;
        // no card is physically present. Channel remains CardNotPresent.

        [TestMethod]
        public void CreditMoto_Visa_Sale() {
            var response = _motoVisaCard.Charge(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CreditMoto_Mastercard_Sale() {
            var response = _motoMastercardCard.Charge(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CreditMoto_Visa_Authorization() {
            var response = _motoVisaCard.Authorize(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Preauthorized);
        }

        [TestMethod]
        public void CreditMoto_Mastercard_Authorization() {
            var response = _motoMastercardCard.Authorize(Amount)
                .WithCurrency(CurrencySGD)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Preauthorized);
        }

        #endregion

        #region Helper Methods

        private void AssertTransactionResponse(Transaction transaction, TransactionStatus transactionStatus) {
            Assert.IsNotNull(transaction);
            Assert.AreEqual("00", transaction.ResponseCode);
            Assert.AreEqual(GetMapping(transactionStatus), transaction.ResponseMessage);
        }

        #endregion
    }
}
