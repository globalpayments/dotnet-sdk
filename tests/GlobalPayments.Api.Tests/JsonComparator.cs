using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Tests {
    public class JsonComparator {
        public static bool AreEqual(string expectedString, string compareString) {
            var expected = JsonDoc.Parse(expectedString);
            var compare = JsonDoc.Parse(compareString);

            return AreEqual(expected, compare);
        }

        public static bool AreEqual(JsonDoc expected, JsonDoc compare) {
            // initial compare
            foreach (var key in expected.Keys) {
                if (!compare.Has(key))
                    return false;

                var expObj = expected.GetValue(key);
                var compObj = compare.GetValue(key);

                if (compObj == null || compObj.GetType() != expObj.GetType())
                    return false;

                if (expObj is JsonDoc) {
                    if (!AreEqual((JsonDoc)expObj, (JsonDoc)compObj))
                        return false;
                }
                else {
                    if (!expObj.Equals(compObj))
                        return false;
                }
            }

            // extra property check
            foreach (var key in compare.Keys) {
                if (!expected.Has(key))
                    return false;
            }

            return true;
        }
    }
}
