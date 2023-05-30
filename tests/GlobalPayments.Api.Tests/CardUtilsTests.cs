using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests {
    [TestClass]
    public class CardUtilsTests {
        [TestMethod]
        public void ParseTrackData() {
            //%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?
            var track = new CreditTrackData {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?"
            };
        }
        [TestMethod]
        public void MapCardTypeTest()
        {
            var track = new CreditTrackData
            {
                Value = "%B5806740000004316^MC TEST CARD^251210199998888777766665555444433332?;5806740000004316=25121019999888877776?",
            };
            Assert.AreEqual("MC", TestCards.MasterCardSwipeEncryptedV2());
            Assert.AreEqual("MC", CardUtils.MapCardType(track.Pan));
        }
    }
}
