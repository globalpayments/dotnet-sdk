using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpEcom.Certification {
    [TestClass]
    public class JsonComparatorTest {
        private const string KNOWN_GOOD_JSON = "{ \"MERCHANT_ID\": \"TWVyY2hhbnRJZA==\", \"ACCOUNT\": \"aW50ZXJuZXQ=\", \"ORDER_ID\": \"R1RJNVl4YjBTdW1MX1RrRE1DQXhRQQ==\", \"AMOUNT\": \"MTk5OQ==\", \"CURRENCY\": \"RVVS\", \"TIMESTAMP\": \"MjAxNzA3MTMxNTUzNDM=\", \"SHA1HASH\": \"NjhlMTgyZDIzNTg1ZTJlNDNlMDIwODFhNTA1ODYyM2Y2ODg2MjQyZQ==\", \"AUTO_SETTLE_FLAG\": \"MA==\", \"SHIPPING_CODE\": \"NjU0fDEyMw==\", \"SHIPPING_CO\": \"R0I=\", \"BILLING_CODE\": \"OTg3fDY1NA==\", \"BILLING_CO\": \"SUU=\", \"CUST_NUM\": \"Q1JNUkVGMTIzNDU2Nzg5\", \"PROD_ID\": \"U0tVMTIzNDU2Nzg5\", \"HPP_LANG\": \"RU4=\", \"CARD_PAYMENT_BUTTON\": \"Q29tcGxldGUgUGF5bWVudA==\"}";

        [TestMethod]
        public void CompareSameStrings() {
            Assert.AreEqual(true, JsonComparator.AreEqual(KNOWN_GOOD_JSON, KNOWN_GOOD_JSON));
        }

        [TestMethod]
        public void CompareWithMissingField() {
            var testString = "{ \"ACCOUNT\": \"aW50ZXJuZXQ=\", \"ORDER_ID\": \"R1RJNVl4YjBTdW1MX1RrRE1DQXhRQQ==\", \"AMOUNT\": \"MTk5OQ==\", \"CURRENCY\": \"RVVS\", \"TIMESTAMP\": \"MjAxNzA3MTMxNTUzNDM=\", \"SHA1HASH\": \"NjhlMTgyZDIzNTg1ZTJlNDNlMDIwODFhNTA1ODYyM2Y2ODg2MjQyZQ==\", \"AUTO_SETTLE_FLAG\": \"MA==\", \"SHIPPING_CODE\": \"NjU0fDEyMw==\", \"SHIPPING_CO\": \"R0I=\", \"BILLING_CODE\": \"OTg3fDY1NA==\", \"BILLING_CO\": \"SUU=\", \"CUST_NUM\": \"Q1JNUkVGMTIzNDU2Nzg5\", \"PROD_ID\": \"U0tVMTIzNDU2Nzg5\", \"HPP_LANG\": \"RU4=\", \"CARD_PAYMENT_BUTTON\": \"Q29tcGxldGUgUGF5bWVudA==\"}";
            Assert.AreEqual(false, JsonComparator.AreEqual(KNOWN_GOOD_JSON, testString));
        }

        [TestMethod]
        public void CompareWithDifferentValue() {
            var testString = "{ \"MERCHANT_ID\": \"merchant_id\", \"ACCOUNT\": \"aW50ZXJuZXQ=\", \"ORDER_ID\": \"R1RJNVl4YjBTdW1MX1RrRE1DQXhRQQ==\", \"AMOUNT\": \"MTk5OQ==\", \"CURRENCY\": \"RVVS\", \"TIMESTAMP\": \"MjAxNzA3MTMxNTUzNDM=\", \"SHA1HASH\": \"NjhlMTgyZDIzNTg1ZTJlNDNlMDIwODFhNTA1ODYyM2Y2ODg2MjQyZQ==\", \"AUTO_SETTLE_FLAG\": \"MA==\", \"SHIPPING_CODE\": \"NjU0fDEyMw==\", \"SHIPPING_CO\": \"R0I=\", \"BILLING_CODE\": \"OTg3fDY1NA==\", \"BILLING_CO\": \"SUU=\", \"CUST_NUM\": \"Q1JNUkVGMTIzNDU2Nzg5\", \"PROD_ID\": \"U0tVMTIzNDU2Nzg5\", \"HPP_LANG\": \"RU4=\", \"CARD_PAYMENT_BUTTON\": \"Q29tcGxldGUgUGF5bWVudA==\"}";
            Assert.AreEqual(false, JsonComparator.AreEqual(KNOWN_GOOD_JSON, testString));
        }

        [TestMethod]
        public void CompareWithExtraData() {
            var testString = "{ \"NEW_FIELD\": \"new_field_data\", \"MERCHANT_ID\": \"TWVyY2hhbnRJZA==\", \"ACCOUNT\": \"aW50ZXJuZXQ=\", \"ORDER_ID\": \"R1RJNVl4YjBTdW1MX1RrRE1DQXhRQQ==\", \"AMOUNT\": \"MTk5OQ==\", \"CURRENCY\": \"RVVS\", \"TIMESTAMP\": \"MjAxNzA3MTMxNTUzNDM=\", \"SHA1HASH\": \"NjhlMTgyZDIzNTg1ZTJlNDNlMDIwODFhNTA1ODYyM2Y2ODg2MjQyZQ==\", \"AUTO_SETTLE_FLAG\": \"MA==\", \"SHIPPING_CODE\": \"NjU0fDEyMw==\", \"SHIPPING_CO\": \"R0I=\", \"BILLING_CODE\": \"OTg3fDY1NA==\", \"BILLING_CO\": \"SUU=\", \"CUST_NUM\": \"Q1JNUkVGMTIzNDU2Nzg5\", \"PROD_ID\": \"U0tVMTIzNDU2Nzg5\", \"HPP_LANG\": \"RU4=\", \"CARD_PAYMENT_BUTTON\": \"Q29tcGxldGUgUGF5bWVudA==\"}";
            Assert.AreEqual(false, JsonComparator.AreEqual(KNOWN_GOOD_JSON, testString));
        }
    }
}
