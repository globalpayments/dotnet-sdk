using System;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Utils {
    [TestClass]
    public class JsonDocTests {
        [TestMethod]
        public void GetValue_NonConvertibleValue_ReturnsDefaultInsteadOfThrowing() {
            // A JSON array of primitives is parsed into a List<string>, which is not
            // convertible to DateTime. GetValue<T> must degrade to default(T) rather
            // than re-throwing an InvalidCastException from its catch block.
            JsonDoc doc = JsonDoc.Parse("{ \"field\": [\"2026-06-11T11:29:35Z\"] }");

            DateTime value = doc.GetValue<DateTime>("field");

            Assert.AreEqual(default(DateTime), value);
        }

        [TestMethod]
        public void GetNullableValue_NonConvertibleValue_ReturnsDefaultInsteadOfThrowing() {
            JsonDoc doc = JsonDoc.Parse("{ \"field\": [\"123\"] }");

            int? value = doc.GetNullableValue<int?>("field");

            Assert.IsNull(value);
        }

        [TestMethod]
        public void GetValue_ConvertibleValue_StillMaps() {
            JsonDoc doc = JsonDoc.Parse("{ \"field\": \"2026-06-11T11:29:35Z\" }");

            DateTime value = doc.GetValue<DateTime>("field");

            Assert.AreNotEqual(default(DateTime), value);
            Assert.AreEqual(2026, value.Year);
        }
    }
}
