using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Tests.Utils
{
    [TestClass]
    public class StringUtilsTests
    {
        [TestMethod]
        public void ToCurrencyString()
        {
            Assert.AreEqual("0.46", Extensions.ToCurrencyString(0.459m));
            Assert.AreEqual("0.40", Extensions.ToCurrencyString(0.4m));
            Assert.AreEqual("12.41", Extensions.ToCurrencyString(12.413m));
            Assert.AreEqual("12.40", Extensions.ToCurrencyString(12.4m));
        }
    }
}