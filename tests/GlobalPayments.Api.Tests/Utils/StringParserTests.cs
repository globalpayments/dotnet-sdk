using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Utils {
    [TestClass]
    public class StringParserTests {
        [TestMethod]
        public void ReadToChar_SecondFieldStopsAtTheDelimiter() {
            StringParser parser = new StringParser("12\\345\\6789");

            Assert.AreEqual("12", parser.ReadToChar('\\'));
            Assert.AreEqual("345", parser.ReadToChar('\\'));
        }
    }
}
