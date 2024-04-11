using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Tests.TestData;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.TransIT {
    [TestClass]
    public class TransitCreditTests {
        private CreditCardData card;
        private CreditTrackData track;

        private const decimal AMOUNT = 12.35m;

        public TransitCreditTests() {
            var acceptorConfig = new AcceptorConfig {
                /* The following are the default values for the AcceptorConfig */

                //CardDataInputCapability = CardDataInputCapability.MagStripe_KeyEntry,
                //OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Attended,
                //CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.None,
                //TerminalOutputCapability = TerminalOutputCapability.Unknown,
                //PinCaptureCapability = PinCaptureCapability.Unknown,
                //CardCaptureCapability = false,
                //CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.NotAuthenticated,
                //CardDataOutputCapability = CardDataOutputCapability.None
            };

            ServicesContainer.ConfigureService(new TransitConfig {
                AcceptorConfig = acceptorConfig,
                DeveloperId = "003226G001",
                DeviceId = "88700000322601",
                MerchantId = "887000003226",
                TransactionKey = "H1O8QTS2JVA9OFMQ3FGEH6D5E28X1KS9",
                RequestLogger = new RequestConsoleLogger()
            });

            card = TestCards.VisaManual(false, false);
            card.Cvn = "999";

            track = TestCards.VisaSwipe(EntryMethod.Swipe);
        }

        [TestMethod]
        public void Sale_Manual() {
            var response = card.Charge(AMOUNT)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            Assert.AreEqual(AMOUNT, response.AuthorizedAmount);
        }

        [TestMethod]
        public void Sale_Manual_AmountBiggerThan1000()
        {
            decimal amount = 2156.56m;
            var response = card.Charge(amount)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            Assert.AreEqual(amount, response.AuthorizedAmount);
        }

        [TestMethod]
        public void Refund_Manual_AmountBiggerThan1000()
        {
            decimal amount = 2156.56m;
            var response = card.Charge(amount)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            Assert.AreEqual(amount, response.AuthorizedAmount);

            var refundResponse = Transaction.FromId(response.TransactionId)
               .Refund(amount)
               .WithCurrency("USD")
               .Execute();

            Assert.IsNotNull(refundResponse);
            Assert.AreEqual("00", refundResponse.ResponseCode);
            Assert.AreEqual(amount, response.AuthorizedAmount);
        }

        [TestMethod]
        public void Sale_Swiped() {
            var response = track.Charge(AMOUNT)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            Assert.AreEqual(AMOUNT, response.AuthorizedAmount);
        }

        [TestMethod]
        public void Auth_Manual() {
            var response = card.Authorize(AMOUNT)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            Assert.AreEqual(AMOUNT, response.AuthorizedAmount);

            var captureResponse = response.Capture()
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode, captureResponse.ResponseMessage);
            Assert.AreEqual(AMOUNT, response.AuthorizedAmount);
        }

        [TestMethod]
        public void Auth_Swiped() {
            decimal amount = 2156.56m;
            var response = track.Authorize(amount)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            Assert.AreEqual(amount, response.AuthorizedAmount);

            var captureResponse = response.Capture()
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode, captureResponse.ResponseMessage);
            Assert.AreEqual(amount, response.AuthorizedAmount);
        }

        [TestMethod]
        public void Auth_Swiped_EmvFallback() {
            var fallback = new CreditTrackData {
                Value = "5413330089010434=22122019882803290000",
                EntryMethod = EntryMethod.Swipe
            };

            var response = fallback.Authorize(AMOUNT)
                .WithCurrency("USD")
                .WithEmvFallbackData(EmvFallbackCondition.ChipReadFailure, EmvLastChipRead.Successful, "3.6.0")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            Assert.AreEqual(AMOUNT, response.AuthorizedAmount);

            var captureResponse = response.Capture()
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode, captureResponse.ResponseMessage);
        }

        [TestMethod]
        public void Auth_CaptureDifferentAmount() {
            var amount = AMOUNT;

            var response = track.Authorize(amount)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            Assert.AreEqual(AMOUNT, response.AuthorizedAmount);

            var captureResponse = response.Capture(amount + 2)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode, captureResponse.ResponseMessage);
            Assert.AreEqual(AMOUNT, response.AuthorizedAmount);
        }

        [TestMethod]
        public void Auth_AddGratuity() {
            var amount = AMOUNT;

            var response = track.Authorize(amount)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            Assert.AreEqual(AMOUNT, response.AuthorizedAmount);

            var captureResponse = response.Capture(amount + 2)
                .WithGratuity(2m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode, captureResponse.ResponseMessage);
        }

        [TestMethod]
        public void Auth_MultiCapture() {
            var response = track.Authorize(20m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            Assert.AreEqual(20m, response.AuthorizedAmount);

            var captureResponse = response.Capture(10m)
                .WithCurrency("USD")
                .WithMultiCapture(1, 2)
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode, captureResponse.ResponseMessage);

            var captureResponse2 = response.Capture(10m)
                .WithCurrency("USD")
                .WithMultiCapture(2, 2)
                .Execute();
            Assert.IsNotNull(captureResponse2);
            Assert.AreEqual("00", captureResponse2.ResponseCode, captureResponse2.ResponseMessage);
        }

        [TestMethod]
        public void Auth_MultiCapture_Mixed() {
            var response = track.Authorize(24m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            Assert.AreEqual(24m, response.AuthorizedAmount);

            var captureResponse = response.Capture(12m)
                .WithCurrency("USD")
                .WithMultiCapture(1, 2)
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode, captureResponse.ResponseMessage);

            var captureResponse2 = response.Capture(12m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(captureResponse2);
            Assert.AreEqual("00", captureResponse2.ResponseCode, captureResponse2.ResponseMessage);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void Auth_MultiCapture_OverAuth() {
            var response = track.Authorize(21m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            Assert.AreEqual(21m, response.AuthorizedAmount);

            var captureResponse = response.Capture(21m)
                .WithCurrency("USD")
                .WithMultiCapture(1, 2)
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode, captureResponse.ResponseMessage);
            Assert.AreEqual(21m, response.AuthorizedAmount);

            response.Capture(10m)
                .WithCurrency("USD")
                .WithMultiCapture(2, 2)
                .Execute();
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void Auth_MultiCapture_NoFlag() {
            var response = track.Authorize(22m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            Assert.AreEqual(22m, response.AuthorizedAmount);

            var captureResponse = response.Capture(12m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode, captureResponse.ResponseMessage);

            response.Capture(10m)
                .WithCurrency("USD")
                .Execute();
        }

        [TestMethod]
        public void BalanceInquiry_Swiped() {
            var response = track.BalanceInquiry()
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            Assert.IsNotNull(response.BalanceAmount);
        }

        [TestMethod]
        public void Verify_Manual() {
            var response = card.Verify().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void Verify_Swiped() {
            var response = track.Verify().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void Refund_WithReference() {
            var response = card.Charge(10m)
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(10m, response.AuthorizedAmount);

            var refundResponse = Transaction.FromId(response.TransactionId)
                .Refund()
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(refundResponse);
            Assert.AreEqual("00", refundResponse.ResponseCode);
            Assert.AreEqual(10m, response.AuthorizedAmount);
        }
    }
}
