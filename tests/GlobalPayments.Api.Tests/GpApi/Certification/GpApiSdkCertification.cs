using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.GpApi.Certification {
    [TestClass]
    public class GpApiSdkCertification : BaseGpApiTests {
        #region Credit Card SUCCESS
        [TestMethod]
        public void CreditCard_Visa_Success() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "John Doe"
            };

            var response = card.Charge(14.99m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_Visa_Success")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("VISA", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("00", response.AuthorizationCode);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        [TestMethod]
        public void CreditCard_Mastercard_Success() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "5425230000004415",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "John Smith"
            };

            var response = card.Charge(4.95m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_Mastercard_Success")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("MASTERCARD", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("00", response.AuthorizationCode);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        [TestMethod]
        public void CreditCard_AmericanExpress_Success() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "374101000000608",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "1234",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Susan Jones"
            };

            var response = card.Charge(17.25m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_AmericanExpress_Success")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("AMEX", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("00", response.AuthorizationCode);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        [TestMethod]
        public void CreditCard_DinersClub_Success() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "36256000000725",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "789",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Mark Green"
            };

            var response = card.Charge(5.15m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_DinersClub_Success")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("DINERS", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("00", response.AuthorizationCode);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        [TestMethod]
        public void CreditCard_Discover_Success() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "6011000000000087",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "456",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Mark Green"
            };

            var response = card.Charge(2.14m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_Discover_Success")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("DISCOVER", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("00", response.AuthorizationCode);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        [TestMethod]
        public void CreditCard_JCB_Success() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "3566000000000000",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "223",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Mark Green"
            };

            var response = card.Charge(1.99m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_JCB_Success")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("JCB", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("00", response.AuthorizationCode);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }
        #endregion

        #region Credit Card Visa DECLINED
        [TestMethod]
        public void CreditCard_Visa_Declined_101() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "4000120000001154",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "John Doe"
            };

            var response = card.Charge(10.25m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_Visa_Declined_101")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("VISA", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("101", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_Visa_Declined_102() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "4000130000001724",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Mark Smith"
            };

            var response = card.Charge(3.75m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_Visa_Declined_102")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("VISA", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("102", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_Visa_Declined_103() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "4000160000004147",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Bob Smith"
            };

            var response = card.Charge(5.35m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_Visa_Declined_103")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("VISA", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("103", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }
        #endregion

        #region Credit Card Mastercard DECLINED
        [TestMethod]
        public void CreditCard_Mastercard_Declined_101() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "5114610000004778",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Bob Howard"
            };

            var response = card.Charge(3.25m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_Mastercard_Declined_101")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("MASTERCARD", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("101", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_Mastercard_Declined_102() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "5114630000009791",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Tom Grey"
            };

            var response = card.Charge(4.50m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_Mastercard_Declined_102")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("MASTERCARD", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("102", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_Mastercard_Declined_103() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "5121220000006921",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                CvnPresenceIndicator = CvnPresenceIndicator.Illegible,
                CardHolderName = "Marie Curie"
            };

            var response = card.Charge(5.99m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_Mastercard_Declined_103")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("MASTERCARD", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("103", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }
        #endregion

        #region Credit Card American Express DECLINED
        [TestMethod]
        public void CreditCard_AmericanExpress_Declined_101() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "376525000000010",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "1234",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "John Doe"
            };

            var response = card.Charge(7.25m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_AmericanExpress_Declined_101")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("AMEX", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("101", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_AmericanExpress_Declined_102() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "375425000000907",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "1234",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Mark Smith"
            };

            var response = card.Charge(9.75m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_AmericanExpress_Declined_102")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("AMEX", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("102", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_AmericanExpress_Declined_103() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "343452000000306",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "1234",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Bob Smith"
            };

            var response = card.Charge(1.25m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_AmericanExpress_Declined_103")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("AMEX", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("103", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }
        #endregion

        #region Credit Card Diners Club DECLINED
        [TestMethod]
        public void CreditCard_DinersClub_Declined_101() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "36256000000998",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "John Smith"
            };

            var response = card.Charge(1.25m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_DinersClub_Declined_101")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("DINERS", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("101", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_DinersClub_Declined_102() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "36256000000634",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "John Smith"
            };

            var response = card.Charge(2.25m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_DinersClub_Declined_102")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("DINERS", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("102", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_DinersClub_Declined_103() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "38865000000705",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "John Smith"
            };

            var response = card.Charge(3.25m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_DinersClub_Declined_103")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("DINERS", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("103", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }
        #endregion

        #region Credit Card Discover DECLINED
        [TestMethod]
        public void CreditCard_Discover_Declined_101() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "6011000000001010",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Rob Brown"
            };

            var response = card.Charge(1.25m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_Discover_Declined_101")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("DISCOVER", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("101", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_Discover_Declined_102() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "6011000000001028",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Rob Brown"
            };

            var response = card.Charge(2.25m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_Discover_Declined_102")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("DISCOVER", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("102", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_Discover_Declined_103() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "6011000000001036",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Rob Brown"
            };

            var response = card.Charge(3.25m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_Discover_Declined_103")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("DISCOVER", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("103", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }
        #endregion

        #region Credit Card JCB DECLINED
        [TestMethod]
        public void CreditCard_JCB_Declined_101() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "3566000000001016",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Michael Smith"
            };

            var response = card.Charge(1.25m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_JCB_Declined_101")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("JCB", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("101", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_JCB_Declined_102() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "3566000000001024",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Michael Smith"
            };

            var response = card.Charge(2.25m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_JCB_Declined_102")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("JCB", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("102", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_JCB_Declined_103() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "3566000000001032",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Michael Smith"
            };

            var response = card.Charge(3.25m)
                .WithCurrency("USD")
                .WithDescription("CreditCard_JCB_Declined_103")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("JCB", response.CardType, true, "Card brand mismatch");
            Assert.AreEqual("103", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }
        #endregion

        #region Credit Card Visa ERROR
        [TestMethod]
        public void CreditCard_Visa_Processing_Error() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "4009830000001985",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Mark Spencer"
            };

            try {
                var response = card.Charge(3.99m)
                    .WithCurrency("USD")
                    .WithDescription("CreditCard_Visa_Processing_Error")
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("50013", ex.ResponseMessage);
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
            }
        }

        [TestMethod]
        public void CreditCard_Visa_Processing_Error_Wrong_Currency() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "4009830000001985",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Mark Spencer"
            };

            try {
                var response = card.Charge(3.99m)
                    .WithCurrency("XXX")
                    .WithDescription("CreditCard_Visa_Processing_Error_Wrong_Currency")
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("50024", ex.ResponseMessage);
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
            }
        }
        #endregion

        #region Credit Card Mastercard ERROR
        [TestMethod]
        public void CreditCard_Mastercard_Processing_Error() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "5135020000005871",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Tom Brown"
            };

            try {
                var response = card.Charge(2.16m)
                    .WithCurrency("USD")
                    .WithDescription("CreditCard_Mastercard_Processing_Error")
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("50013", ex.ResponseMessage);
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
            }
        }
        #endregion

        #region Credit Card American Express ERROR
        [TestMethod]
        public void CreditCard_AmericanExpress_Processing_Error() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "372349000000852",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "1234",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Tina White"
            };

            try {
                var response = card.Charge(4.02m)
                    .WithCurrency("USD")
                    .WithDescription("CreditCard_AmericanExpress_Processing_Error")
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("50013", ex.ResponseMessage);
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
            }
        }
        #endregion

        #region Credit Card Diners Club ERROR
        [TestMethod]
        public void CreditCard_DinersClub_Processing_Error() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "30450000000985",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Ashley Brown"
            };

            try {
                var response = card.Charge(5.99m)
                    .WithCurrency("USD")
                    .WithDescription("CreditCard_DinersClub_Processing_Error")
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("50013", ex.ResponseMessage);
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
            }
        }
        #endregion

        #region Credit Card Discover ERROR
        [TestMethod]
        public void CreditCard_Discover_Processing_Error() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "6011000000002000",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Mark Spencer"
            };

            try {
                var response = card.Charge(8.99m)
                    .WithCurrency("USD")
                    .WithDescription("CreditCard_Discover_Processing_Error")
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("50013", ex.ResponseMessage);
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
            }
        }
        #endregion

        #region Credit Card JCB ERROR
        [TestMethod]
        public void CreditCard_JCB_Processing_Error() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
            });

            var card = new CreditCardData {
                Number = "3566000000002006",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = "Mark Spencer"
            };

            try {
                var response = card.Charge(4.99m)
                    .WithCurrency("USD")
                    .WithDescription("CreditCard_JCB_Processing_Error")
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("50013", ex.ResponseMessage);
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
            }
        }
        #endregion
    }
}
