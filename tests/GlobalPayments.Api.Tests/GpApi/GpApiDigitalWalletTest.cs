using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi
{
    [TestClass]
    public class GpApiDigitalWalletTest : BaseGpApiTests {
        private CreditCardData card;
        private const string CLICK_TO_PAY_TOKEN = "8144735251653223601";

        private const string GOOGLE_PAY_TOKEN =
            "{\n\"signature\": \"MEUCICd0LX9L1tHEf0I037gVTPS/DSFfKSHmChCkzGH629atAiEA2FbMt8aOuI+8p7QDEJIz2CetUWqHn+wpudVkEmc436A=\"," +
            "\n\"protocolVersion\": \"ECv1\"," +
            "\n\"signedMessage\": \"{\\\"encryptedMessage\\\":\\\"keMtGQ03Kz+ydkxvv3Wy8XtUoHIGDvaKdLmIT3Czi7gY4wNe7o6UQ4gkjxH2qyYvRyoFqSfUyXAN+++AEEAlaJUsJysFJkM6G3GOcAvt7mxrNRJQhj60JbvX3iJ/NBDTlInPZVO5jbDh314igbV/tKhehztLIjFF4+Rn0bwh+ZE8DvFqt+hH/piw4vvDSVPXLdMiCddfmFKYEgHAxeiovHjnlb6PcA6UXRIWanu0etLmX0Wj4Kkz15MD6rIjPaKTP8VJr5El13/4SaRpbWZ8pAEULZuJYNUQbhwEas+5YjsskGDPJQKn8L2zMAOF9YOCA9+C/wBMRvDPvJ4X4hobkk7E/QsHViUtKFlEYHN73ojTsymrJxq9kDQY2rZQtgalzDq0gRWJYxDyvCX3979X8FGNitbV7rzL2rPyt0TA\\\",\\\"ephemeralPublicKey\\\":\\\"BMEZDnSw5a1OgsZlMJU9mrml/FWfKzRRNgtV/2P7uzpb0j5/MuqXH4gFgan6u1qlVC8E6nPCsago7yu2tMNJBAA\\\\u003d\\\",\\\"tag\\\":\\\"YGvjeXWGRW1jfLkHzm6vDJKu3p+ccCeUF/xARa5NQuk\\\\u003d\\\"}" +
            "\"\n}";

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);
        }

        [TestInitialize]
        public void TestInitialize() {
            card = new CreditCardData {
                CardHolderName = "James Mason",
            };
        }

        [TestMethod]
        public void ClickToPayEncrypted() {
            card.Token = CLICK_TO_PAY_TOKEN;
            card.MobileType = EncyptedMobileType.CLICK_TO_PAY;

            var response = card.Charge(10m)
                .WithCurrency("EUR")
                .WithModifier(TransactionModifier.EncryptedMobile)
                .WithMaskedDataResponse(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual("123456", response.AuthorizationCode);
            Assert.IsFalse(string.IsNullOrEmpty(response.TransactionId));
            AssertClickToPayPayerDetails(response);
        }

        [TestMethod]
        public void ClickToPayEncryptedChargeThenSplitAmount()
        {
            card.Token = CLICK_TO_PAY_TOKEN;
            card.MobileType = EncyptedMobileType.CLICK_TO_PAY;

            var response = card.Charge(10m)
                .WithCurrency("EUR")
                .WithModifier(TransactionModifier.EncryptedMobile)
                .WithMaskedDataResponse(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual("123456", response.AuthorizationCode);
            Assert.IsFalse(string.IsNullOrEmpty(response.TransactionId));
            AssertClickToPayPayerDetails(response);

            var split = response.Split(5m)
                .WithDescription("Split CTP")
                .Execute();
            
            Assert.IsNotNull(split);
        }

        [TestMethod]
        public void ClickToPayEncryptedChargeThenRefund()
        {
            card.Token = CLICK_TO_PAY_TOKEN;
            card.MobileType = EncyptedMobileType.CLICK_TO_PAY;

            var response = card.Charge(10m)
                .WithCurrency("EUR")
                .WithModifier(TransactionModifier.EncryptedMobile)
                .WithMaskedDataResponse(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
            Assert.AreEqual("123456", response.AuthorizationCode);
            AssertClickToPayPayerDetails(response);

            var refund = response.Refund()
                .WithCurrency("EUR")
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(refund);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), refund.ResponseMessage);
            AssertClickToPayPayerDetails(response);
        }

        [TestMethod]
        public void ClickToPayEncryptedChargeThenReverse()
        {
            card.Token = CLICK_TO_PAY_TOKEN;
            card.MobileType = EncyptedMobileType.CLICK_TO_PAY;

            var response = card.Charge(10m)
                .WithCurrency("EUR")
                .WithModifier(TransactionModifier.EncryptedMobile)
                .WithMaskedDataResponse(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
            Assert.AreEqual("123456", response.AuthorizationCode);
            AssertClickToPayPayerDetails(response);

            var reverse = response.Reverse()
                   .WithCurrency("EUR")
                   .WithAllowDuplicates(true)
                   .Execute();

            Assert.IsNotNull(reverse);
            Assert.AreEqual(GetMapping(TransactionStatus.Reversed), reverse.ResponseMessage);
            Assert.AreEqual("000000", reverse.AuthorizationCode);
            AssertClickToPayPayerDetails(response);
        }
        
        [TestMethod]
        public void ClickToPayEncryptedAuthorize() {            
            card.Token = CLICK_TO_PAY_TOKEN;              
            card.MobileType = EncyptedMobileType.CLICK_TO_PAY;

            var errorFound = false;
            try {
                card.Authorize(10m)
                    .WithCurrency("EUR")
                    .WithModifier(TransactionModifier.EncryptedMobile)
                    .WithMaskedDataResponse(true)
                    .Execute();
            }
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadRequest - capture_mode contains unexpected data.", e.Message);
                Assert.AreEqual("40213", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }
        
        [TestMethod]
        public void ClickToPayEncryptedRefundStandalone() {            
            card.Token = CLICK_TO_PAY_TOKEN;              
            card.MobileType = EncyptedMobileType.CLICK_TO_PAY;

            var errorFound = false;
            try {
                card.Refund(10m)
                    .WithCurrency("EUR")
                    .WithModifier(TransactionModifier.EncryptedMobile)
                    .WithMaskedDataResponse(true)
                    .Execute();
            }
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadRequest - Mandatory Fields missing [ request card number] See Developers Guide", e.Message);
                Assert.AreEqual("50021", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod, Ignore]
        //You need a valid ApplePay token that it is valid only for 60 sec
        public void PayWithApplePayEncrypted() {
            card.Token = "{\"version\":\"EC_v1\",\"data\":\"NICZ32trHZ49D9WR0x4hHkegnKzaUM1xyJFutL0GejUUaF0jmojY+QmXqRvz0vcjrK3yr6smDWHpO4ik9gvHEsrPdIo/utv47ouX7wKVe99vZC8XgNl/AG4hDp93SnqnxjZZiVC5BjbdI6g34WVHxL8P8CbNFU7K+2jyU97ZZ+q0u0VfxvjnaK7GvmcLdv+cNyeB9vbfoGvEAlIOdQvQJrX8n+9ykSTzAnCERQbyDiZZYjG3Q1j0UXMH8wrtdzu45svjGh2USyc5Ms32oqiPCVF7lvU0nzJ2aVgbjSKqlalWUil5T32lKK4YkHm3Funv7f1tOJGtgBHLaQjl0VO9p0ZfC1b+FxX1OwlFh43DvCeLeGufhJHy/nhfU8QZW/56vDIKUmYEU1YQcfm6NiZT0grNDPljckUjhcAuEI6pkW4=\",\"signature\":\"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\",\"header\":{\"ephemeralPublicKey\":\"MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEPosEkIYUU0ZfcTrr0Ru2sVA6JeAC9B+2ItJ6y94UNs+KYUlvhf+Hfw9hK9JTc8dAoi1ZJ7Z8dzpDKyNASDDGOw==\",\"publicKeyHash\":\"YKX3vVU8AgaDqrDyn3iCDAWzmJaYr6xMEiBRrTwGcAI=\",\"transactionId\":\"a9f4b8d321798ba82544a1e6256e4395b44a5e5cc30a36f850e3c76a8af5cb12\"}}";
            card.MobileType = EncyptedMobileType.APPLE_PAY;

            var transaction = card.Charge(10m)
                .WithCurrency("USD")
                .WithModifier(TransactionModifier.EncryptedMobile)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual("123456", transaction.AuthorizationCode);
            Assert.IsFalse(string.IsNullOrEmpty(transaction.TransactionId));
        }

        [TestMethod, Ignore]
        //You need a valid ApplePay token that it is valid only for 60 sec
        public void PayWithApplePayEncryptedChargedThenRefund()
        {
            card.Token = "{\"version\":\"EC_v1\",\"data\":\"u4gZ7Qvq9jF8DN13aWOB8FkQ4coB5nurVfuG7DOv2JAbswT8KrcMqcIK2KdtFOzncmrKEaB3w8oysY0ZKfYxLS2KrK0mnQhY3qAbmNb3HfJ433CfsO1hTSEZgCDNsWFLkcOql4Do2wk4IKVlEq105CjRGfNeYn0gkWjGh9T/yT85WYElMB216nKH3WoutefbEt/uoN2aKUSImaciFPy6qDwFbtX1pOQ8kT//n7MvUfl7aUR83MTgktpH9VEU19k6K+H6D8xvecAlXiYt4zNCiw2XkYKWR0cg+4GxBqTl1RI1DV0bU0ZR4Qyz2FAmadvXohC7qZnOrh0FX/4w7D5DSP+O4BF3uCst4XIRJsQaz9zyr1GncE3qsePV4Q0WRfxoARvzBF0MQhnNfbR9cBmqgAFCMPlk0Qv0UJg+rbwgYGQ=\",\"signature\":\"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\",\"header\":{\"ephemeralPublicKey\":\"MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEuhKOdDaqF0EjVE0qN4Cb1+d+hdXkoy4LIKbGzymTbIiMBj6tU2OZIDGes21fgEbIsXn92nkuwpalEsAoGueVjg==\",\"publicKeyHash\":\"rEYX/7PdO7F7xL7rH0LZVak/iXTrkeU89Ck7E9dGFO4=\",\"transactionId\":\"e9cfc9cc29bf17945a53fea1d8f65265ee2962708a46d7696ffc90090ea364aa\"}}";
            card.MobileType = EncyptedMobileType.APPLE_PAY;

            var transaction = card.Charge(10m)
                .WithCurrency("USD")
                .WithModifier(TransactionModifier.EncryptedMobile)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual("123456", transaction.AuthorizationCode);
            Assert.IsFalse(string.IsNullOrEmpty(transaction.TransactionId));

            var refund = transaction.Refund().WithCurrency("USD").Execute();

            Assert.IsNotNull(refund);
            Assert.AreEqual(Success, refund.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), refund.ResponseMessage);
        }

        [TestMethod, Ignore]
        //You need a valid ApplePay token that it is valid only for 60 sec
        public void PayWithApplePayEncryptedChargedThenReverse()
        {
            card.Token = "{\"version\":\"EC_v1\",\"data\":\"EXcXc7RAa3/fyG30V1xmTRZZDBOwQMmURCNsE4gutV6apykQlbR7uAJEqbpI4hURpKXUc+g5uGCx2Ugp4XPH+CvnYBdeNiy6dVW9aGqY3fJubm0m1ye9xVTBCZtNF3+DsccYu08ER8H2eweiOa/ugOdcfvCKc86QIWdvFuQHHGYQG6GZl8krQOYT81x1cMfmNqisjCoS29F9p+MxvH7kvxGPewJ2cPvTOSyZx4wKRcKv+pblR/sktRBdkJGloGxUnxtCGM3DUE+CAKoFftfZramvlOCVY+1dbXnqDdf5P3ROTAGoP3F21Sgw43eQvKktaJ+99CAXB5ITtChEnEPMj24CDsvD+DyjRYjLytYzrv5gHAcTnySHdyeRldiYTyRHMJtN1PwnhFl/tlLOpNKgTr4I9TIHDwJpp4ujczilKIc=\",\"signature\":\"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\",\"header\":{\"ephemeralPublicKey\":\"MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEDt8/QYDs4S7NjtJmOgzU71QuryCjBSYgPfdcP+Wr0KPQn2poE1X6gAk+702JNJ0pA0yqX+8OC93CNrB2DQ55cA==\",\"publicKeyHash\":\"rEYX/7PdO7F7xL7rH0LZVak/iXTrkeU89Ck7E9dGFO4=\",\"transactionId\":\"bd4bfd340914027b54c5aec21993d3537133228311b57defc36f1a107acd5662\"}}";
            card.MobileType = EncyptedMobileType.APPLE_PAY;

            var transaction = card.Charge(10m)
                .WithCurrency("USD")
                .WithModifier(TransactionModifier.EncryptedMobile)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual("123456", transaction.AuthorizationCode);
            Assert.IsFalse(string.IsNullOrEmpty(transaction.TransactionId));

            var reverse = transaction.Reverse().WithCurrency("USD").Execute();

            Assert.IsNotNull(reverse);
            Assert.AreEqual(Success, reverse.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Reversed), reverse.ResponseMessage);
        }

        [TestMethod]
        public void PayWithDecryptedFlow() {
            var encryptedProviders = new List<string> {
                EncyptedMobileType.APPLE_PAY,
                EncyptedMobileType.GOOGLE_PAY
            };
            foreach(var encryptedProvider in encryptedProviders) {
                card.Token = "5167300431085507";
                card.MobileType = encryptedProvider;
                card.ExpMonth = 05;
                card.ExpYear = 2025;
                card.Cryptogram = "234234234";

                var transaction = card.Charge(5m)
                    .WithCurrency("EUR")
                    .WithModifier(TransactionModifier.DecryptedMobile)
                    .Execute();

                Assert.IsNotNull(transaction);
                Assert.AreEqual(Success, transaction.ResponseCode);
                Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);
                Assert.AreEqual("123456", transaction.AuthorizationCode);
                Assert.IsFalse(string.IsNullOrEmpty(transaction.TransactionId));
                Assert.IsFalse(string.IsNullOrEmpty(transaction.AuthorizationCode));
            }
        }

        [TestMethod]
        public void PayWithGooglePayEncrypted() {
            card.Token = GOOGLE_PAY_TOKEN;
            card.MobileType = EncyptedMobileType.GOOGLE_PAY;

            var transaction = card.Charge(10m)
                .WithCurrency("EUR")
                .WithModifier(TransactionModifier.EncryptedMobile)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);
            Assert.AreEqual("123456", transaction.AuthorizationCode);
            Assert.AreEqual("VISA", transaction.CardDetails.Brand);
            Assert.IsNotNull(transaction.CardBrandTransactionId);
            Assert.IsFalse(string.IsNullOrEmpty(transaction.TransactionId));
        }

        [TestMethod]
        public void GooglePayEncrypted_LinkedRefund() {
            card.Token = GOOGLE_PAY_TOKEN;
            card.MobileType = EncyptedMobileType.GOOGLE_PAY;

            var transaction = card.Charge(10m)
                .WithCurrency("EUR")
                .WithModifier(TransactionModifier.EncryptedMobile)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);
            Assert.AreEqual("123456", transaction.AuthorizationCode);
            Assert.IsFalse(string.IsNullOrEmpty(transaction.TransactionId));

            var refund = transaction.Refund().WithCurrency("EUR").Execute();

            Assert.IsNotNull(refund);
            Assert.AreEqual(Success, refund.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), refund.ResponseMessage);
        }

        [TestMethod]
        public void GooglePayEncrypted_Reverse() {
            card.Token = GOOGLE_PAY_TOKEN;
            card.MobileType = EncyptedMobileType.GOOGLE_PAY;

            var transaction = card.Charge(10m)
                    .WithCurrency("EUR")
                    .WithModifier(TransactionModifier.EncryptedMobile)
                    .Execute();

                Assert.IsNotNull(transaction);
                Assert.AreEqual(Success, transaction.ResponseCode);
                Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);
                Assert.AreEqual("123456", transaction.AuthorizationCode);
                Assert.IsFalse(string.IsNullOrEmpty(transaction.TransactionId));

                var reverse = transaction.Reverse().WithCurrency("EUR").Execute();

                Assert.IsNotNull(reverse);
                Assert.AreEqual(Success, reverse.ResponseCode);
                Assert.AreEqual(GetMapping(TransactionStatus.Reversed), reverse.ResponseMessage);
        }

        private void AssertClickToPayPayerDetails(Transaction response) {
            Assert.IsNotNull(response.PayerDetails);
            Assert.IsNotNull(response.PayerDetails.Email);
            Assert.IsNotNull(response.PayerDetails.BillingAddress);
            Assert.IsNotNull(response.PayerDetails.ShippingAddress);
            Assert.IsNotNull(response.PayerDetails.FirstName);
            Assert.IsNotNull(response.PayerDetails.LastName);
        }
    }
}
