using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Tests.Logging;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Tests.GpApi
{
    [TestClass]
    public class GpApiDigitalWalletTests : BaseGpApiTests
    {
        CreditCardData card;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            ServicesContainer.ConfigureService(new GpApiConfig
            {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
                RequestLogger = new RequestFileLogger(@"C:\temp\gpapi\requestlog.txt")
            });
        }

        [TestInitialize]
        public void TestInitialize()
        {
            card = new CreditCardData
            {
                CardHolderName = "James Mason#",
            };
        }

        [TestMethod, Ignore]
        //You need a valid ApplePay token that it is valid only for 60 sec
        public void PayWithApplePayEncrypted()
        {
            card.Token = "{\"version\":\"EC_v1\",\"data\":\"7RbOpd67lcBgPFDHwpVCtz1g9DSXIEz6h7XUhsWiVS3B5WpkpgJ3Mj/EKNuuHezpPmsJ6AZb72twHiR/6Ngs29X4jKcv3XvdrEgL+5S7dKZoU0sNN3y7UWBFFklUgF+FGv9Amvytoav0mV+Pfe0UWenyb8smF5fZDF5Ta8d30WPkBaPf6IpD2sOXHoVxgqvoQNkr6rQNoG3Tm+fHzOukNTsRGxi35OZvx4SgKxZvivMiH7xs4DKnRZMiKWl+4Zym48/UQ+F/+cwM/7rCY+r7BPlki6xE50IEl2/4PPl6wzhs1AkfqVJB79J0iNHL5/CMTFi/UgUFmIRMTrujVHerhqGnFyIJ6jutsS9H6TJ6+6M9OUzfG53XNolUxJ0Nox9MA9uQxozw2tTJt/Z0RBpbTU8jnTvN9s5053xP/Hxx9dg=\",\"signature\":\"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\",\"header\":{\"ephemeralPublicKey\":\"MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEeyyM++BjGrlaodphlJUvfTx4tQwn5Ci9IGpAI3RvbYqEshGX5cdkl0j7yNEu913OgT99r/MU1wqHnXn4p7qosA==\",\"publicKeyHash\":\"rEYX/7PdO7F7xL7rH0LZVak/iXTrkeU89Ck7E9dGFO4=\",\"transactionId\":\"38bb5ca49bc54c70e6ff5996bd087f1cce27f0f84fca2f6e71871fc7a56d877e\"}}";
            card.MobileType = EncyptedMobileType.APPLE_PAY;

            var transaction = card.Charge(10m)
                .WithCurrency("EUR")
                .WithModifier(TransactionModifier.EncryptedMobile)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.IsFalse(string.IsNullOrEmpty(transaction.TransactionId));
        }

        [TestMethod]
        public void PayWithDecryptedFlow()
        {
            var encryptedProviders = new List<string>
            {
                EncyptedMobileType.APPLE_PAY,
                EncyptedMobileType.GOOGLE_PAY
            };
            foreach(var encryptedProvider in encryptedProviders)
            {
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
                Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
                Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);
                Assert.IsFalse(string.IsNullOrEmpty(transaction.TransactionId));
                Assert.IsFalse(string.IsNullOrEmpty(transaction.AuthorizationCode));
            }
        }

        [TestMethod]
        public void PayWithGooglePayEncrypted()
        {
            card.Token = "{\"signature\": \"MEUCIBIVOWdzNCngMnVzt3puh3538CpA66lRPqfEsPMEwVn2AiEAtT3NsjFMcvseKCgtTKZLoVlMYsnlgwgvbZLm9Sm0WK8=\",\"protocolVersion\": \"ECv1\",\"signedMessage\": \"{\\\"encryptedMessage\\\":\\\"jUzgICpwU5OtRSS+nDh9kEGZViqylerkdzIRbmOidf/IQXpKzFeMdCO6QoBAVCX4PLreJ+s8h/l2Czm4VxbiSWHdTOwW03iCnkxohKx00KePanypfI0zg8XO/EOauK3LNHvFFwOnCj9GbL35p21R/lEImXWiiMtXMGwlW2fNJQcB/fiOwkwX4dAVOQspQoYvBwLci/PBIGl91MJfYicTG27MhXHCjNjpwivuzhdPGDps23gLojzhKNBjfsEFwnzreAIhZcBaKsI7lGBBzBJHgR5dkB4fh6odb6Dr4AxUndqg9QgPoKy9m51FWWnycppPPmNPao2rAKndWXWvQ82YEdlyGJUG1bstsWeGmipGowXjiDIhaOdJEzXS1FtA0tSkrPB4Bt9da6ehS0HLOT0129c1Otj8bYsMMMuYczV6Co2QPK3B0+6kkqdoXaxOIc4bxZKh\\\",\\\"ephemeralPublicKey\\\":\\\"BPNqktib5dgo1tX4YWoUVqp/I/wEN5O+1exJjbSRj556mtVR3fkxQjvdIOmlm1+Vmj/cRNkuS2i3246cMTfjWuI\\\\u003d\\\",\\\"tag\\\":\\\"JDMNgNq8BGryVrt5bRMYHXjX+CVcuFYS3w3nmfkIHEA\\\\u003d\\\"}\"}";
            card.MobileType = EncyptedMobileType.GOOGLE_PAY;

            var transaction = card.Charge(10m)
                .WithCurrency("EUR")
                .WithModifier(TransactionModifier.EncryptedMobile)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);
            Assert.IsFalse(string.IsNullOrEmpty(transaction.TransactionId));
        }

        [TestMethod]
        public void GooglePayEncrypted_LinkedRefund()
        {
            card.Token = "{\"signature\": \"MEUCIBIVOWdzNCngMnVzt3puh3538CpA66lRPqfEsPMEwVn2AiEAtT3NsjFMcvseKCgtTKZLoVlMYsnlgwgvbZLm9Sm0WK8=\",\"protocolVersion\": \"ECv1\",\"signedMessage\": \"{\\\"encryptedMessage\\\":\\\"jUzgICpwU5OtRSS+nDh9kEGZViqylerkdzIRbmOidf/IQXpKzFeMdCO6QoBAVCX4PLreJ+s8h/l2Czm4VxbiSWHdTOwW03iCnkxohKx00KePanypfI0zg8XO/EOauK3LNHvFFwOnCj9GbL35p21R/lEImXWiiMtXMGwlW2fNJQcB/fiOwkwX4dAVOQspQoYvBwLci/PBIGl91MJfYicTG27MhXHCjNjpwivuzhdPGDps23gLojzhKNBjfsEFwnzreAIhZcBaKsI7lGBBzBJHgR5dkB4fh6odb6Dr4AxUndqg9QgPoKy9m51FWWnycppPPmNPao2rAKndWXWvQ82YEdlyGJUG1bstsWeGmipGowXjiDIhaOdJEzXS1FtA0tSkrPB4Bt9da6ehS0HLOT0129c1Otj8bYsMMMuYczV6Co2QPK3B0+6kkqdoXaxOIc4bxZKh\\\",\\\"ephemeralPublicKey\\\":\\\"BPNqktib5dgo1tX4YWoUVqp/I/wEN5O+1exJjbSRj556mtVR3fkxQjvdIOmlm1+Vmj/cRNkuS2i3246cMTfjWuI\\\\u003d\\\",\\\"tag\\\":\\\"JDMNgNq8BGryVrt5bRMYHXjX+CVcuFYS3w3nmfkIHEA\\\\u003d\\\"}\"}";
            card.MobileType = EncyptedMobileType.GOOGLE_PAY;

            var transaction = card.Charge(10m)
                .WithCurrency("EUR")
                .WithModifier(TransactionModifier.EncryptedMobile)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);
            Assert.IsFalse(string.IsNullOrEmpty(transaction.TransactionId));

            var refund = transaction.Refund().WithCurrency("EUR").Execute();

            Assert.IsNotNull(refund);
            Assert.AreEqual(SUCCESS, refund?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), refund?.ResponseMessage);
        }

        [TestMethod]
        public void GooglePayEncrypted_Reverse()
        {
            card.Token = "{\"signature\": \"MEUCIBIVOWdzNCngMnVzt3puh3538CpA66lRPqfEsPMEwVn2AiEAtT3NsjFMcvseKCgtTKZLoVlMYsnlgwgvbZLm9Sm0WK8=\",\"protocolVersion\": \"ECv1\",\"signedMessage\": \"{\\\"encryptedMessage\\\":\\\"jUzgICpwU5OtRSS+nDh9kEGZViqylerkdzIRbmOidf/IQXpKzFeMdCO6QoBAVCX4PLreJ+s8h/l2Czm4VxbiSWHdTOwW03iCnkxohKx00KePanypfI0zg8XO/EOauK3LNHvFFwOnCj9GbL35p21R/lEImXWiiMtXMGwlW2fNJQcB/fiOwkwX4dAVOQspQoYvBwLci/PBIGl91MJfYicTG27MhXHCjNjpwivuzhdPGDps23gLojzhKNBjfsEFwnzreAIhZcBaKsI7lGBBzBJHgR5dkB4fh6odb6Dr4AxUndqg9QgPoKy9m51FWWnycppPPmNPao2rAKndWXWvQ82YEdlyGJUG1bstsWeGmipGowXjiDIhaOdJEzXS1FtA0tSkrPB4Bt9da6ehS0HLOT0129c1Otj8bYsMMMuYczV6Co2QPK3B0+6kkqdoXaxOIc4bxZKh\\\",\\\"ephemeralPublicKey\\\":\\\"BPNqktib5dgo1tX4YWoUVqp/I/wEN5O+1exJjbSRj556mtVR3fkxQjvdIOmlm1+Vmj/cRNkuS2i3246cMTfjWuI\\\\u003d\\\",\\\"tag\\\":\\\"JDMNgNq8BGryVrt5bRMYHXjX+CVcuFYS3w3nmfkIHEA\\\\u003d\\\"}\"}";
            card.MobileType = EncyptedMobileType.GOOGLE_PAY;

                var transaction = card.Charge(10m)
                    .WithCurrency("EUR")
                    .WithModifier(TransactionModifier.EncryptedMobile)
                    .Execute();

                Assert.IsNotNull(transaction);
                Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
                Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);
                Assert.IsFalse(string.IsNullOrEmpty(transaction.TransactionId));

                var refund = transaction.Reverse().WithCurrency("EUR").Execute();

                Assert.IsNotNull(refund);
                Assert.AreEqual(SUCCESS, refund?.ResponseCode);
                Assert.AreEqual(GetMapping(TransactionStatus.Reversed), refund?.ResponseMessage);
        }
    }
}
