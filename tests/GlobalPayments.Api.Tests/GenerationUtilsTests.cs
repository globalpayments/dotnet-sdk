using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace GlobalPayments.Api.Tests
{
    [TestClass]
    public sealed class GenerationUtilsTests
    {
        private const string DataToHash = @"20211027113102.TrainlineDev.20-2000-20a1223a5d404c6492f8c0f4b330c186-12214913.00.(00)[ test system ] Authorised.16353306627169501.12345";
        private const string HashSecret = @"secret";
        private const string ExpectedHashResult = @"c54df7482cfb588f4c3bdb2688b089bced16026d";

        [TestMethod]
        public void CalculatesSha1HashCorrectly() {
            var result = GenerationUtils.GenerateHash(DataToHash, HashSecret);
            Assert.AreEqual(ExpectedHashResult, result);
        }

        [TestMethod]
        public void CalculatesSha1HashCorrectlyInMultiThreadedEnvironment() {
            Parallel.For(1, 51, (i, state) => 
            {
                var result = GenerationUtils.GenerateHash(DataToHash, HashSecret);
                Assert.AreEqual(ExpectedHashResult, result); 
            });
        }
    }
}