using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests {
    [TestClass]
    public class PorticoGiftTests
    {
        GiftCard card;
        GiftCard track;

        [TestInitialize]
        public void Init() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MaePAQBr-1QAqjfckFC8FTbRTT120bVQUlfVOjgCBw"
            });

            card = new GiftCard { Number = "5022440000000000007" };
            track = new GiftCard { TrackData = "%B5022440000000000098^^391200081613?;5022440000000000098=391200081613?" };
        }

        [TestMethod]
        public void GiftCreate() {
            var newCard = GiftCard.Create("2145550199");
            Assert.IsNotNull(newCard);
            Assert.IsNotNull(newCard.Number);
            Assert.IsNotNull(newCard.Alias);
            Assert.IsNotNull(newCard.Pin);
        }

        [TestMethod]
        public void GiftAddAlias() {
            var response = card.AddAlias("2145550199").Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void GiftAddValue() {
            var response = card.AddValue(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void GiftBalanceInquiry() {
            var response = card.BalanceInquiry().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void GiftSale() {
            var response = card.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void GiftDeactivate() {
            var response = card.Deactivate().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void GiftRemoveAlias() {
            var response = card.RemoveAlias("2145550199").Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void GiftReplace() {
            var response = card.ReplaceWith(track).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void GiftReverse() {
            var response = card.Reverse(10m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void GiftRewards() {
            var response = card.Rewards(10m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void GiftTrackAddAlias() {
            var response = track.AddAlias("2145550199").Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void GiftTrackAddValue() {
            var response = track.AddValue(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void GiftTrackBalanceInquiry() {
            var response = track.BalanceInquiry().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void GiftTrackSale() {
            var response = track.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void GiftTrackDeactivate() {
            var response = track.Deactivate().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void GiftTrackRemoveAlias() {
            var response = track.RemoveAlias("2145550199").Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void GiftTrackReplace() {
            var response = track.ReplaceWith(card).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void GiftTrackReverse() {
            var response = track.Reverse(10m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void GiftTrackRewards() {
            var response = track.Rewards(10m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void GiftReverseWithTransactionId() {
            var response = card.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);

            var reverseResponse = Transaction.FromId(response.TransactionId, PaymentMethodType.Gift)
                .Reverse(10m)
                .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("00", reverseResponse.ResponseCode, reverseResponse.ResponseMessage);
        }
    }
}
