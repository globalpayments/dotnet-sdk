using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi
{
    [TestClass]
    public class GpApiDigitalWalletTest : BaseGpApiTests {
        private CreditCardData card;
        private const string ClickToPayToken = "8144735251653223601";

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
            card.Token = ClickToPayToken;
            card.MobileType = EncyptedMobileType.CLICK_TO_PAY;

            var response = card.Charge(10m)
                .WithCurrency("EUR")
                .WithModifier(TransactionModifier.EncryptedMobile)
                .WithMaskedDataResponse(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
            Assert.AreEqual(Success, response?.ResponseCode);
            Assert.IsFalse(string.IsNullOrEmpty(response.TransactionId));
            AssertClickToPayPayerDetails(response);
        }

        [TestMethod]
        public void ClickToPayEncryptedChargeThenSplitAmount()
        {
            card.Token = ClickToPayToken;
            card.MobileType = EncyptedMobileType.CLICK_TO_PAY;

            var response = card.Charge(10m)
                .WithCurrency("EUR")
                .WithModifier(TransactionModifier.EncryptedMobile)
                .WithMaskedDataResponse(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
            Assert.AreEqual(Success, response?.ResponseCode);
            Assert.IsFalse(string.IsNullOrEmpty(response.TransactionId));
            AssertClickToPayPayerDetails(response);

            var split = response.Split(5m)
                .WithDescription("Split CTP")
                .Execute();
        }

        [TestMethod]
        public void ClickToPayEncryptedChargeThenRefund()
        {
            card.Token = ClickToPayToken;
            card.MobileType = EncyptedMobileType.CLICK_TO_PAY;

            var response = card.Charge(10m)
                .WithCurrency("EUR")
                .WithModifier(TransactionModifier.EncryptedMobile)
                .WithMaskedDataResponse(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
            AssertClickToPayPayerDetails(response);

            var refund = response.Refund()
                .WithCurrency("EUR")
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(refund);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), refund?.ResponseMessage);
            AssertClickToPayPayerDetails(response);
        }

        [TestMethod]
        public void ClickToPayEncryptedChargeThenReverse()
        {
            card.Token = ClickToPayToken;
            card.MobileType = EncyptedMobileType.CLICK_TO_PAY;

            var response = card.Charge(10m)
                .WithCurrency("EUR")
                .WithModifier(TransactionModifier.EncryptedMobile)
                .WithMaskedDataResponse(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
            AssertClickToPayPayerDetails(response);

            var reverse = response.Reverse()
                   .WithCurrency("EUR")
                   .WithAllowDuplicates(true)
                   .Execute();

            Assert.IsNotNull(reverse);
            Assert.AreEqual(GetMapping(TransactionStatus.Reversed), reverse?.ResponseMessage);
            AssertClickToPayPayerDetails(response);
        }
        
        [TestMethod]
        public void ClickToPayEncryptedAuthorize() {            
            card.Token = ClickToPayToken;              
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
            card.Token = ClickToPayToken;              
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
            card.Token = "{\"version\":\"EC_v1\",\"data\":\"7RbOpd67lcBgPFDHwpVCtz1g9DSXIEz6h7XUhsWiVS3B5WpkpgJ3Mj/EKNuuHezpPmsJ6AZb72twHiR/6Ngs29X4jKcv3XvdrEgL+5S7dKZoU0sNN3y7UWBFFklUgF+FGv9Amvytoav0mV+Pfe0UWenyb8smF5fZDF5Ta8d30WPkBaPf6IpD2sOXHoVxgqvoQNkr6rQNoG3Tm+fHzOukNTsRGxi35OZvx4SgKxZvivMiH7xs4DKnRZMiKWl+4Zym48/UQ+F/+cwM/7rCY+r7BPlki6xE50IEl2/4PPl6wzhs1AkfqVJB79J0iNHL5/CMTFi/UgUFmIRMTrujVHerhqGnFyIJ6jutsS9H6TJ6+6M9OUzfG53XNolUxJ0Nox9MA9uQxozw2tTJt/Z0RBpbTU8jnTvN9s5053xP/Hxx9dg=\",\"signature\":\"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\",\"header\":{\"ephemeralPublicKey\":\"MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEeyyM++BjGrlaodphlJUvfTx4tQwn5Ci9IGpAI3RvbYqEshGX5cdkl0j7yNEu913OgT99r/MU1wqHnXn4p7qosA==\",\"publicKeyHash\":\"rEYX/7PdO7F7xL7rH0LZVak/iXTrkeU89Ck7E9dGFO4=\",\"transactionId\":\"38bb5ca49bc54c70e6ff5996bd087f1cce27f0f84fca2f6e71871fc7a56d877e\"}}";
            card.MobileType = EncyptedMobileType.APPLE_PAY;

            var transaction = card.Charge(10m)
                .WithCurrency("EUR")
                .WithModifier(TransactionModifier.EncryptedMobile)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);
            Assert.AreEqual(Success, transaction?.ResponseCode);
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
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);
            Assert.AreEqual(Success, transaction?.ResponseCode);
            Assert.IsFalse(string.IsNullOrEmpty(transaction.TransactionId));

            var refund = transaction.Refund().WithCurrency("USD").Execute();

            Assert.IsNotNull(refund);
            Assert.AreEqual(Success, refund?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), refund?.ResponseMessage);
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
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);
            Assert.AreEqual(Success, transaction?.ResponseCode);
            Assert.IsFalse(string.IsNullOrEmpty(transaction.TransactionId));

            var reverse = transaction.Reverse().WithCurrency("USD").Execute();

            Assert.IsNotNull(reverse);
            Assert.AreEqual(Success, reverse?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Reversed), reverse?.ResponseMessage);
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
                Assert.AreEqual(Success, transaction?.ResponseCode);
                Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);
                Assert.IsFalse(string.IsNullOrEmpty(transaction.TransactionId));
                Assert.IsFalse(string.IsNullOrEmpty(transaction.AuthorizationCode));
            }
        }

        [TestMethod]
        public void PayWithGooglePayEncrypted() {
            card.Token = "{\n" +
                         "    \"signature\": \"MEQCIDoMJc0jcHAzk846Y7BT6XVN/YrwypNtShE+nF/XWMiAAiAd0nhStaWT0Fw3mWjJXPRy4J+GXhdm5WOjlgEu9DS7pQ\\u003d\\u003d\",\n" +
                         "    \"protocolVersion\": \"ECv1\",\n" +
                         "    \"signedMessage\": \"{\\\"encryptedMessage\\\":\\\"c9G2ljhM/OK3IrJapYrVYCi/HUQOX9IZfPE7mSLhxe26R/fpCkXF8i0j9DhM7kttoQBa1KmzNwAP4kaIR9XQB/IZsnZ0YllS+Jo0KYmOoLfcW/OUFYzRXZGbvJZRts7PP4U/NPXDiSDQM7ACna7Nca9o5UBIrY4ZL8MLiIbdBI8BSlCdJYqw4kyXibY2C5QMG+2lzF3fn4kSsz2odC8c9BuW/2D2Rz2Lau9awUMtQUpIHI4W6VU8t1FtDWppWzPUbD0CBw88ZlBOdHTgu2OJE5bR/+NXz99Rn7h3hmqnnMu/0JNxryvZr+Ed1cmLqoVbRf+3ik5WVUsyOcQWrouH8vjOTVq208o8MbSA1wX0LK4gAWVnyRpPsUkz5KJOKvKWIzT3VlV7m9BxlVc7E4RQe7/QJO5LKVcV5cotKXdGK9YINLSyIwkLRcKR9qQ92Q4z0h4g\\\",\\\"ephemeralPublicKey\\\":\\\"BPUg4L7vjs5ucmOzR+GNmLxWp1LnEsFV/Bc6WQY6qgtyxT4HFnziN/3nkbspF0l4auZX1UjCV1scYjE910Lp2TU\\\\u003d\\\",\\\"tag\\\":\\\"x0md5a9GwLxoCWrxQYS4jJfPjuQv9ajrJMd5zNjbd+A\\\\u003d\\\"}\"\n" +
                         "}";
            card.MobileType = EncyptedMobileType.GOOGLE_PAY;

            var transaction = card.Charge(10m)
                .WithCurrency("EUR")
                .WithModifier(TransactionModifier.EncryptedMobile)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);
            Assert.IsFalse(string.IsNullOrEmpty(transaction.TransactionId));
        }

        [TestMethod]
        public void GooglePayEncrypted_LinkedRefund() {
            card.Token = "{\n" +
                         "    \"signature\": \"MEQCIDoMJc0jcHAzk846Y7BT6XVN/YrwypNtShE+nF/XWMiAAiAd0nhStaWT0Fw3mWjJXPRy4J+GXhdm5WOjlgEu9DS7pQ\\u003d\\u003d\",\n" +
                         "    \"protocolVersion\": \"ECv1\",\n" +
                         "    \"signedMessage\": \"{\\\"encryptedMessage\\\":\\\"c9G2ljhM/OK3IrJapYrVYCi/HUQOX9IZfPE7mSLhxe26R/fpCkXF8i0j9DhM7kttoQBa1KmzNwAP4kaIR9XQB/IZsnZ0YllS+Jo0KYmOoLfcW/OUFYzRXZGbvJZRts7PP4U/NPXDiSDQM7ACna7Nca9o5UBIrY4ZL8MLiIbdBI8BSlCdJYqw4kyXibY2C5QMG+2lzF3fn4kSsz2odC8c9BuW/2D2Rz2Lau9awUMtQUpIHI4W6VU8t1FtDWppWzPUbD0CBw88ZlBOdHTgu2OJE5bR/+NXz99Rn7h3hmqnnMu/0JNxryvZr+Ed1cmLqoVbRf+3ik5WVUsyOcQWrouH8vjOTVq208o8MbSA1wX0LK4gAWVnyRpPsUkz5KJOKvKWIzT3VlV7m9BxlVc7E4RQe7/QJO5LKVcV5cotKXdGK9YINLSyIwkLRcKR9qQ92Q4z0h4g\\\",\\\"ephemeralPublicKey\\\":\\\"BPUg4L7vjs5ucmOzR+GNmLxWp1LnEsFV/Bc6WQY6qgtyxT4HFnziN/3nkbspF0l4auZX1UjCV1scYjE910Lp2TU\\\\u003d\\\",\\\"tag\\\":\\\"x0md5a9GwLxoCWrxQYS4jJfPjuQv9ajrJMd5zNjbd+A\\\\u003d\\\"}\"\n" +
                         "}";
            card.MobileType = EncyptedMobileType.GOOGLE_PAY;

            var transaction = card.Charge(10m)
                .WithCurrency("EUR")
                .WithModifier(TransactionModifier.EncryptedMobile)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);
            Assert.IsFalse(string.IsNullOrEmpty(transaction.TransactionId));

            var refund = transaction.Refund().WithCurrency("EUR").Execute();

            Assert.IsNotNull(refund);
            Assert.AreEqual(Success, refund?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), refund?.ResponseMessage);
        }

        [TestMethod]
        public void GooglePayEncrypted_Reverse() {
            card.Token = "{\n" +
                         "    \"signature\": \"MEQCIDoMJc0jcHAzk846Y7BT6XVN/YrwypNtShE+nF/XWMiAAiAd0nhStaWT0Fw3mWjJXPRy4J+GXhdm5WOjlgEu9DS7pQ\\u003d\\u003d\",\n" +
                         "    \"protocolVersion\": \"ECv1\",\n" +
                         "    \"signedMessage\": \"{\\\"encryptedMessage\\\":\\\"c9G2ljhM/OK3IrJapYrVYCi/HUQOX9IZfPE7mSLhxe26R/fpCkXF8i0j9DhM7kttoQBa1KmzNwAP4kaIR9XQB/IZsnZ0YllS+Jo0KYmOoLfcW/OUFYzRXZGbvJZRts7PP4U/NPXDiSDQM7ACna7Nca9o5UBIrY4ZL8MLiIbdBI8BSlCdJYqw4kyXibY2C5QMG+2lzF3fn4kSsz2odC8c9BuW/2D2Rz2Lau9awUMtQUpIHI4W6VU8t1FtDWppWzPUbD0CBw88ZlBOdHTgu2OJE5bR/+NXz99Rn7h3hmqnnMu/0JNxryvZr+Ed1cmLqoVbRf+3ik5WVUsyOcQWrouH8vjOTVq208o8MbSA1wX0LK4gAWVnyRpPsUkz5KJOKvKWIzT3VlV7m9BxlVc7E4RQe7/QJO5LKVcV5cotKXdGK9YINLSyIwkLRcKR9qQ92Q4z0h4g\\\",\\\"ephemeralPublicKey\\\":\\\"BPUg4L7vjs5ucmOzR+GNmLxWp1LnEsFV/Bc6WQY6qgtyxT4HFnziN/3nkbspF0l4auZX1UjCV1scYjE910Lp2TU\\\\u003d\\\",\\\"tag\\\":\\\"x0md5a9GwLxoCWrxQYS4jJfPjuQv9ajrJMd5zNjbd+A\\\\u003d\\\"}\"\n" +
                         "}";
            card.MobileType = EncyptedMobileType.GOOGLE_PAY;

            var transaction = card.Charge(10m)
                    .WithCurrency("EUR")
                    .WithModifier(TransactionModifier.EncryptedMobile)
                    .Execute();

                Assert.IsNotNull(transaction);
                Assert.AreEqual(Success, transaction?.ResponseCode);
                Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);
                Assert.IsFalse(string.IsNullOrEmpty(transaction.TransactionId));

                var reverse = transaction.Reverse().WithCurrency("EUR").Execute();

                Assert.IsNotNull(reverse);
                Assert.AreEqual(Success, reverse?.ResponseCode);
                Assert.AreEqual(GetMapping(TransactionStatus.Reversed), reverse?.ResponseMessage);
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
