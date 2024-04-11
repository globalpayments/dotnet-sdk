using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi.Certification {
    
    [TestClass]
    public class GpApiSdkCertificationTest : BaseGpApiTests {
        
        private const string CURRENCY = "USD";
        private const string SUCCESS_AUTH_CODE = "00";

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);
        }

        private static CreditCardData InitCreditCardData(string cardNumber, string cvn, string cardHolderName) {
            var card = new CreditCardData {
                Number = cardNumber,
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = cvn,
                CvnPresenceIndicator = CvnPresenceIndicator.Present,
                CardHolderName = cardHolderName
            };

            return card;
        }

        #region Credit Card SUCCESS

        [TestMethod]
        public void CreditCard_Visa_Success() {
            var card = InitCreditCardData("4263970000005262", "123", "John Doe");

            var response = card.Charge(14.99m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_Visa_Success")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("VISA", response.CardType);
            Assert.AreEqual(SUCCESS_AUTH_CODE, response.AuthorizationCode);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        [TestMethod]
        public void CreditCard_Mastercard_Success() {
            var card = InitCreditCardData("5425230000004415", "123", "John Smith");

            var response = card.Charge(4.95m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_Mastercard_Success")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("MASTERCARD", response.CardType);
            Assert.AreEqual(SUCCESS_AUTH_CODE, response.AuthorizationCode);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        [TestMethod]
        public void CreditCard_AmericanExpress_Success() {
            var card = InitCreditCardData("374101000000608", "1234", "Susan Jones");

            var response = card.Charge(17.25m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_AmericanExpress_Success")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("AMEX", response.CardType);
            Assert.AreEqual(SUCCESS_AUTH_CODE, response.AuthorizationCode);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        [TestMethod]
        public void CreditCard_DinersClub_Success() {
            var card = InitCreditCardData("36256000000725", "789", "Mark Green");

            var response = card.Charge(5.15m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_DinersClub_Success")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("DINERS", response.CardType);
            Assert.AreEqual(SUCCESS_AUTH_CODE, response.AuthorizationCode);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        [TestMethod]
        public void CreditCard_Discover_Success() {
            var card = InitCreditCardData("6011000000000087", "456", "Mark Green");

            var response = card.Charge(2.14m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_Discover_Success")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("DISCOVER", response.CardType);
            Assert.AreEqual(SUCCESS_AUTH_CODE, response.AuthorizationCode);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        [TestMethod]
        public void CreditCard_JCB_Success() {
            var card = InitCreditCardData("3566000000000000", "223", "Mark Green");

            var response = card.Charge(1.99m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_JCB_Success")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("JCB", response.CardType);
            Assert.AreEqual(SUCCESS_AUTH_CODE, response.AuthorizationCode);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        #endregion

        #region Credit Card Visa DECLINED

        [TestMethod]
        public void CreditCard_Visa_Declined_101() {
            var card = InitCreditCardData("4000120000001154", "123", "John Doe");

            var response = card.Charge(10.25m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_Visa_Declined_101")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("VISA", response.CardType);
            Assert.AreEqual("101", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_Visa_Declined_102() {
            var card = InitCreditCardData("4000130000001724", "123", "Mark Smith");

            var response = card.Charge(3.75m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_Visa_Declined_102")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("VISA", response.CardType);
            Assert.AreEqual("102", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_Visa_Declined_103() {
            var card = InitCreditCardData("4000160000004147", "123", "Bob Smith");

            var response = card.Charge(5.35m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_Visa_Declined_103")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("VISA", response.CardType);
            Assert.AreEqual("103", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_Visa_Declined_111() {
            var card = InitCreditCardData("4242420000000091", "123", "Bob Smith");

            var response = card.Charge(5.35m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_Visa_Declined_111")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("VISA", response.CardType);
            Assert.AreEqual("111", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        #endregion

        #region Credit Card Mastercard DECLINED

        [TestMethod]
        public void CreditCard_Mastercard_Declined_101() {
            var card = InitCreditCardData("5114610000004778", "123", "Bob Howard");

            var response = card.Charge(3.25m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_Mastercard_Declined_101")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("MASTERCARD", response.CardType);
            Assert.AreEqual("101", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_Mastercard_Declined_102() {
            var card = InitCreditCardData("5114630000009791", "123", "Tom Grey");

            var response = card.Charge(4.50m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_Mastercard_Declined_102")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("MASTERCARD", response.CardType);
            Assert.AreEqual("102", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_Mastercard_Declined_103() {
            var card = InitCreditCardData("5121220000006921", "123", "Marie Curie");
            card.CvnPresenceIndicator = CvnPresenceIndicator.Illegible;

            var response = card.Charge(5.99m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_Mastercard_Declined_103")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("MASTERCARD", response.CardType);
            Assert.AreEqual("103", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_Mastercard_Declined_111() {
            var card = InitCreditCardData("5100000000000131", "123", "Mark Spencer");
            card.CvnPresenceIndicator = CvnPresenceIndicator.Illegible;

            var response = card.Charge(5.99m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_Mastercard_Declined_111")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("MASTERCARD", response.CardType);
            Assert.AreEqual("111", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        #endregion

        #region Credit Card American Express DECLINED

        [TestMethod]
        public void CreditCard_AmericanExpress_Declined_101() {
            var card = InitCreditCardData("376525000000010", "1234", "John Doe");

            var response = card.Charge(7.25m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_AmericanExpress_Declined_101")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("AMEX", response.CardType);
            Assert.AreEqual("101", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_AmericanExpress_Declined_102() {
            var card = InitCreditCardData("375425000000907", "1234", "Mark Smith");

            var response = card.Charge(9.75m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_AmericanExpress_Declined_102")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("AMEX", response.CardType);
            Assert.AreEqual("102", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_AmericanExpress_Declined_103() {
            var card = InitCreditCardData("343452000000306", "1234", "Bob Smith");

            var response = card.Charge(1.25m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_AmericanExpress_Declined_103")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("AMEX", response.CardType);
            Assert.AreEqual("103", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_AmericanExpress_Declined_111() {
            var card = InitCreditCardData("374205502001004", "1234", "Bob Smith");

            var response = card.Charge(1.25m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_AmericanExpress_Declined_111")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("AMEX", response.CardType);
            Assert.AreEqual("111", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        #endregion

        #region Credit Card Diners Club DECLINED

        [TestMethod]
        public void CreditCard_DinersClub_Declined_101() {
            var card = InitCreditCardData("36256000000998", "123", "John Smith");

            var response = card.Charge(1.25m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_DinersClub_Declined_101")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("DINERS", response.CardType);
            Assert.AreEqual("101", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_DinersClub_Declined_102() {
            var card = InitCreditCardData("36256000000634", "123", "John Smith");

            var response = card.Charge(2.25m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_DinersClub_Declined_102")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("DINERS", response.CardType);
            Assert.AreEqual("102", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_DinersClub_Declined_103() {
            var card = InitCreditCardData("38865000000705", "123", "John Smith");

            var response = card.Charge(3.25m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_DinersClub_Declined_103")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("DINERS", response.CardType);
            Assert.AreEqual("103", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        #endregion

        #region Credit Card Discover DECLINED

        [TestMethod]
        public void CreditCard_Discover_Declined_101() {
            var card = InitCreditCardData("6011000000001010", "123", "Rob Brown");

            var response = card.Charge(1.25m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_Discover_Declined_101")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("DISCOVER", response.CardType);
            Assert.AreEqual("101", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_Discover_Declined_102() {
            var card = InitCreditCardData("6011000000001028", "123", "Rob Brown");

            var response = card.Charge(2.25m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_Discover_Declined_102")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("DISCOVER", response.CardType);
            Assert.AreEqual("102", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_Discover_Declined_103() {
            var card = InitCreditCardData("6011000000001036", "123", "Rob Brown");

            var response = card.Charge(3.25m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_Discover_Declined_103")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("DISCOVER", response.CardType);
            Assert.AreEqual("103", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        #endregion

        #region Credit Card JCB DECLINED

        [TestMethod]
        public void CreditCard_JCB_Declined_101() {
            var card = InitCreditCardData("3566000000001016", "123", "Michael Smith");

            var response = card.Charge(1.25m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_JCB_Declined_101")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("JCB", response.CardType);
            Assert.AreEqual("101", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_JCB_Declined_102() {
            var card = InitCreditCardData("3566000000001024", "123", "Michael Smith");

            var response = card.Charge(2.25m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_JCB_Declined_102")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("JCB", response.CardType);
            Assert.AreEqual("102", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        [TestMethod]
        public void CreditCard_JCB_Declined_103() {
            var card = InitCreditCardData("3566000000001032", "123", "Michael Smith");

            var response = card.Charge(3.25m)
                .WithCurrency(CURRENCY)
                .WithDescription("CreditCard_JCB_Declined_103")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("JCB", response.CardType);
            Assert.AreEqual("103", response.AuthorizationCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseCode);
        }

        #endregion

        #region Credit Card Visa ERROR

        [TestMethod]
        public void CreditCard_Visa_Processing_Error() {
            var card = InitCreditCardData("4009830000001985", "123", "Mark Spencer");

            var errorFound = false;
            try {
                card.Charge(3.99m)
                    .WithCurrency(CURRENCY)
                    .WithDescription("CreditCard_Visa_Processing_Error")
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadGateway - 200,eCom error—Developers are notified", ex.Message);
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
                Assert.AreEqual("50013", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void CreditCard_Visa_Processing_Error_Wrong_Currency() {
            var card = InitCreditCardData("4009830000001985", "123", "Mark Spencer");

            var errorFound = false;
            try {
                card.Charge(3.99m)
                    .WithCurrency("XXX")
                    .WithDescription("CreditCard_Visa_Processing_Error_Wrong_Currency")
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadGateway - currency card combination not allowed", ex.Message);
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
                Assert.AreEqual("50024", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        #endregion

        #region Credit Card Mastercard ERROR

        [TestMethod]
        public void CreditCard_Mastercard_Processing_Error() {
            var card = InitCreditCardData("5135020000005871", "123", "Tom Brown");

            var errorFound = false;
            try {
                card.Charge(2.16m)
                    .WithCurrency(CURRENCY)
                    .WithDescription("CreditCard_Mastercard_Processing_Error")
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadGateway - 200,eCom error—Developers are notified", ex.Message);
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
                Assert.AreEqual("50013", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        #endregion

        #region Credit Card American Express ERROR

        [TestMethod]
        public void CreditCard_AmericanExpress_Processing_Error() {
            var card = InitCreditCardData("372349000000852", "1234", "Tina White");

            var errorFound = false;
            try {
                card.Charge(4.02m)
                    .WithCurrency(CURRENCY)
                    .WithDescription("CreditCard_AmericanExpress_Processing_Error")
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadGateway - 200,eCom error—Developers are notified", ex.Message);
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
                Assert.AreEqual("50013", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        #endregion

        #region Credit Card Diners Club ERROR

        [TestMethod]
        public void CreditCard_DinersClub_Processing_Error() {
            var card = InitCreditCardData("30450000000985", "123", "Ashley Brown");

            var errorFound = false;
            try {
                card.Charge(5.99m)
                    .WithCurrency(CURRENCY)
                    .WithDescription("CreditCard_DinersClub_Processing_Error")
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadGateway - 200,eCom error—Developers are notified", ex.Message);
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
                Assert.AreEqual("50013", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        #endregion

        #region Credit Card Discover ERROR

        [TestMethod]
        public void CreditCard_Discover_Processing_Error() {
            var card = InitCreditCardData("6011000000002000", "123", "Mark Spencer");

            var errorFound = false;
            try {
                card.Charge(8.99m)
                    .WithCurrency(CURRENCY)
                    .WithDescription("CreditCard_Discover_Processing_Error")
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadGateway - 200,eCom error—Developers are notified", ex.Message);
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
                Assert.AreEqual("50013", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        #endregion

        #region Credit Card JCB ERROR

        [TestMethod]
        public void CreditCard_JCB_Processing_Error() {
            var card = InitCreditCardData("3566000000002006", "123", "Mark Spencer");

            var errorFound = false;
            try {
                card.Charge(4.99m)
                    .WithCurrency(CURRENCY)
                    .WithDescription("CreditCard_JCB_Processing_Error")
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadGateway - 200,eCom error—Developers are notified", ex.Message);
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
                Assert.AreEqual("50013", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        #endregion

        #region Credit Card UATP ERROR

        [TestMethod]
        public void CreditCard_UATP_Transaction_Not_Supported_Error() {
            var card = InitCreditCardData("135400000007187", "123", "Mark Spencer");

            var errorFound = false;
            try {
                card.Charge(2.16m)
                    .WithCurrency(CURRENCY)
                    .WithDescription("CreditCard_UATP_Processing_Error")
                    .Execute();
            } catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadRequest - Transaction not supported Please contact support ",
                    ex.Message);
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("50020", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        #endregion
    }
}