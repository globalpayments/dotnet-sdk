using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests {
    [TestClass]
    public class PorticoEbtTests
    {
        EBTCardData card;
        EBTTrackData track;

        [TestInitialize]
        public void Init() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MaePAQBr-1QAqjfckFC8FTbRTT120bVQUlfVOjgCBw"
            });

            card = new EBTCardData {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123",
                PinBlock = "32539F50C245A6A93D123412324000AA"
            };

            track = new EBTTrackData {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                PinBlock = "32539F50C245A6A93D123412324000AA",
                EncryptionData = new EncryptionData {
                    Version = "01"
                }
            };
        }

        [TestMethod]
        public void EbtBalanceInquiry() {
            var response = card.BalanceInquiry().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void EbtSale() {
            var response = card.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void EbtRefund() {
            var response = card.Refund(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void EbtTrackBalanceInquiry() {
            var response = card.BalanceInquiry().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void EbtTrackSale() {
            var response = card.Charge(11m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void EbtTrackRefund() {
            var response = card.Refund(11m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod, ExpectedException(typeof(UnsupportedTransactionException))]
        public void EbtRefundFromTransactionId() {
            Transaction.FromId("1234567890", PaymentMethodType.EBT).Refund().Execute();
        }
    }
}
