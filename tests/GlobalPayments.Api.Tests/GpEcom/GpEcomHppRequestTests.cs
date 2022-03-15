using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils.Logging;
using GlobalPayments.Api.Tests.Realex.Hpp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace GlobalPayments.Api.Tests.Realex {
    [TestClass]
    public class GpEcomHppRequestTests {
        HostedService _service;
        RealexHppClient _client;

        public GpEcomHppRequestTests() {
            _client = new RealexHppClient("https://pay.sandbox.realexpayments.com/pay", "secret");
            _service = new HostedService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "hpp",
                SharedSecret = "secret",
                HostedPaymentConfig = new HostedPaymentConfig {
                    Language = "GB",
                    ResponseUrl = "http://requestb.in/10q2bjb1"
                },
            });
        }

        [TestMethod]
        public void CreditAuth() {
            var json = _service.Authorize(1m)
                .WithCurrency("EUR")
                .WithCustomerId("123456")
                .WithAddress(new Address {
                    PostalCode = "123|56",
                    Country = "IRELAND"
                }).Serialize();
            Assert.IsNotNull(json);

            var response = _client.SendRequest(json);
            var parsedResponse = _service.ParseResponse(response, true);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", parsedResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditSale() {
            var json = _service.Charge(1m)
                .WithCurrency("EUR")
                .WithCustomerId("123456")
                .WithAddress(new Address {
                    PostalCode = "123|56",
                    Country = "IRELAND"
                }).Serialize();
            Assert.IsNotNull(json);

            var response = _client.SendRequest(json);
            var parsedResponse = _service.ParseResponse(response, true);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", parsedResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditVerify() {
            var json = _service.Verify()
                .WithCurrency("EUR")
                .WithCustomerId("123456")
                .WithAddress(new Address {
                    PostalCode = "123|56",
                    Country = "IRELAND"
                }).Serialize();
            Assert.IsNotNull(json);

            var response = _client.SendRequest(json);
            var parsedResponse = _service.ParseResponse(response, true);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", parsedResponse.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void AuthNoAmount() {
            _service.Authorize(null).WithCurrency("USD").Serialize();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void AuthNoCurrency() {
            _service.Authorize(10m).Serialize();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void SaleNoAmount() {
            _service.Charge(null).WithCurrency("USD").Serialize();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void SaleNoCurrency() {
            _service.Charge(10m).Serialize();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void VerifyNoCurrency() {
            _service.Verify().Serialize();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void VerifyWithAmount() {
            _service.Verify().WithAmount(10m).Serialize();
        }

        [TestMethod]
        public void BasicAuth() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2"
                },
            });

            var hppJson = service.Authorize(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"061609f85a8e0191dc7f487f8278e71898a2ee2d\", \"AUTO_SETTLE_FLAG\": \"0\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\"}";

            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void BasicCharge() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                },
            });

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"061609f85a8e0191dc7f487f8278e71898a2ee2d\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\"}";

            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        // testing COMMENT1, CUST_NUM, PROD_ID, VAR_REF, HPP_LANG, CARD_PAYMENT_BUTTON
        public void BasicHostedPaymentData() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    Language = "EN",
                    PaymentButtonText = "Place Order"
                },
            });

            var testHostedPaymentData = new HostedPaymentData {
                CustomerNumber = "a028774f-beff-47bc-bd6e-ed7e04f5d758a028774f-btefa",
                ProductId = "a0b38df5-b23c-4d82-88fe-2e9c47438972-b23c-4d82-88f",
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithHostedPaymentData(testHostedPaymentData)
                .WithDescription("Mobile Channel")
                .WithClientTransactionId("My Legal Entity")
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"061609f85a8e0191dc7f487f8278e71898a2ee2d\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"CUST_NUM\": \"a028774f-beff-47bc-bd6e-ed7e04f5d758a028774f-btefa\", \"PROD_ID\": \"a0b38df5-b23c-4d82-88fe-2e9c47438972-b23c-4d82-88f\", \"COMMENT1\": \"Mobile Channel\", \"HPP_LANG\": \"EN\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"VAR_REF\": \"My Legal Entity\"}";

            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void StoreCardNewCustomerNoRefs() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    CardStorageEnabled = true,
                },
            });

            var testHostedPaymentData = new HostedPaymentData {
                CustomerExists = false,
                OfferToSaveCard = true
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithHostedPaymentData(testHostedPaymentData)
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"7116c49826367c6513efdc0cc81e243b8095d78f\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"CARD_STORAGE_ENABLE\": \"1\", \"OFFER_SAVE_CARD\": \"1\", \"PAYER_EXIST\": \"0\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void StoreCardNewCustomerJustPayerRef() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    CardStorageEnabled = true,
                },
            });

            var testHostedPaymentData = new HostedPaymentData {
                CustomerExists = false,
                OfferToSaveCard = true,
                CustomerKey = "376a2598-412d-4805-9f47-c177d5605853"
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithHostedPaymentData(testHostedPaymentData)
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"4dcf4e5e2d43855fe31cdc097e985a895868563e\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"CARD_STORAGE_ENABLE\": \"1\", \"OFFER_SAVE_CARD\": \"1\", \"PAYER_EXIST\": \"0\", \"PAYER_REF\": \"376a2598-412d-4805-9f47-c177d5605853\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void StoreCardNewCustomerJustPaymentRef() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    CardStorageEnabled = true,
                },
            });

            var testHostedPaymentData = new HostedPaymentData {
                CustomerExists = false,
                OfferToSaveCard = true,
                PaymentKey = "ca46344d-4292-47dc-9ced-e8a42ce66977"
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithHostedPaymentData(testHostedPaymentData)
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"5fe76a45585d9793fd162ab8a3cd4a42991417df\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"CARD_STORAGE_ENABLE\": \"1\", \"OFFER_SAVE_CARD\": \"1\", \"PAYER_EXIST\": \"0\", \"PMT_REF\": \"ca46344d-4292-47dc-9ced-e8a42ce66977\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void StoreCardNewCustomerAllSuppliedRefs() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    CardStorageEnabled = true,
                },
            });

            var testHostedPaymentData = new HostedPaymentData {
                CustomerExists = false,
                OfferToSaveCard = true,
                CustomerKey = "376a2598-412d-4805-9f47-c177d5605853",
                PaymentKey = "ca46344d-4292-47dc-9ced-e8a42ce66977"
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithHostedPaymentData(testHostedPaymentData)
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"f0cf097fe769a6a5a6254eee631e51709ba34c90\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"CARD_STORAGE_ENABLE\": \"1\", \"OFFER_SAVE_CARD\": \"1\", \"PAYER_EXIST\": \"0\", \"PMT_REF\": \"ca46344d-4292-47dc-9ced-e8a42ce66977\", \"PAYER_REF\": \"376a2598-412d-4805-9f47-c177d5605853\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void StoreCardAutoNewCustomerNoRefs() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    CardStorageEnabled = true,
                },
            });

            var testHostedPaymentData = new HostedPaymentData {
                CustomerExists = false,
                OfferToSaveCard = false
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithHostedPaymentData(testHostedPaymentData)
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"7116c49826367c6513efdc0cc81e243b8095d78f\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"CARD_STORAGE_ENABLE\": \"1\", \"OFFER_SAVE_CARD\": \"0\", \"PAYER_EXIST\": \"0\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void StoreCardReturnCustomerNoPaymentRef() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    CardStorageEnabled = true,
                },
            });

            var testHostedPaymentData = new HostedPaymentData {
                CustomerExists = true,
                OfferToSaveCard = true,
                CustomerKey = "376a2598-412d-4805-9f47-c177d5605853"
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithHostedPaymentData(testHostedPaymentData)
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"4dcf4e5e2d43855fe31cdc097e985a895868563e\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"CARD_STORAGE_ENABLE\": \"1\", \"OFFER_SAVE_CARD\": \"1\", \"PAYER_EXIST\": \"1\", \"PAYER_REF\": \"376a2598-412d-4805-9f47-c177d5605853\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void StoreCardReturnCustomerWithPaymentRef() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    CardStorageEnabled = true,
                },
            });

            var testHostedPaymentData = new HostedPaymentData {
                CustomerExists = true,
                OfferToSaveCard = true,
                PaymentKey = "ca46344d-4292-47dc-9ced-e8a42ce66977"
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithHostedPaymentData(testHostedPaymentData)
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"5fe76a45585d9793fd162ab8a3cd4a42991417df\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"CARD_STORAGE_ENABLE\": \"1\", \"OFFER_SAVE_CARD\": \"1\", \"PAYER_EXIST\": \"1\", \"PMT_REF\": \"ca46344d-4292-47dc-9ced-e8a42ce66977\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void StoreCardAutoReturnCustomerAllRefs() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    CardStorageEnabled = true,
                },
            });

            var testHostedPaymentData = new HostedPaymentData {
                CustomerExists = true,
                OfferToSaveCard = false,
                CustomerKey = "376a2598-412d-4805-9f47-c177d5605853",
                PaymentKey = "ca46344d-4292-47dc-9ced-e8a42ce66977"
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithHostedPaymentData(testHostedPaymentData)
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"f0cf097fe769a6a5a6254eee631e51709ba34c90\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"CARD_STORAGE_ENABLE\": \"1\", \"OFFER_SAVE_CARD\": \"0\", \"PAYER_EXIST\": \"1\", \"PMT_REF\": \"ca46344d-4292-47dc-9ced-e8a42ce66977\", \"PAYER_REF\": \"376a2598-412d-4805-9f47-c177d5605853\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void DisplayStoredCardsOfferSaveNoPaymentRef() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    DisplaySavedCards = true
                },
            });

            var testHostedPaymentData = new HostedPaymentData {
                CustomerExists = true,
                OfferToSaveCard = true,
                CustomerKey = "376a2598-412d-4805-9f47-c177d5605853"
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithHostedPaymentData(testHostedPaymentData)
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"4dcf4e5e2d43855fe31cdc097e985a895868563e\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"HPP_SELECT_STORED_CARD\": \"376a2598-412d-4805-9f47-c177d5605853\", \"OFFER_SAVE_CARD\": \"1\", \"PAYER_EXIST\": \"1\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void DisplayStoredCardsOfferSaveWithPaymentRef() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    DisplaySavedCards = true
                },
            });

            var testHostedPaymentData = new HostedPaymentData {
                CustomerExists = true,
                OfferToSaveCard = true,
                CustomerKey = "376a2598-412d-4805-9f47-c177d5605853",
                PaymentKey = "ca46344d-4292-47dc-9ced-e8a42ce66977"
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithHostedPaymentData(testHostedPaymentData)
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"f0cf097fe769a6a5a6254eee631e51709ba34c90\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"HPP_SELECT_STORED_CARD\": \"376a2598-412d-4805-9f47-c177d5605853\", \"OFFER_SAVE_CARD\": \"1\", \"PAYER_EXIST\": \"1\", \"PMT_REF\": \"ca46344d-4292-47dc-9ced-e8a42ce66977\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void BillingData() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                },
            });

            var billingAddress = new Address {
                Country = "US",
                PostalCode = "50001"
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithAddress(billingAddress, AddressType.Billing)
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"061609f85a8e0191dc7f487f8278e71898a2ee2d\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"BILLING_CODE\": \"50001\", \"BILLING_CO\": \"US\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void ShippingData() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                },
            });

            var shippingAddress = new Address {
                Country = "US",
                PostalCode = "50001"
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithAddress(shippingAddress, AddressType.Shipping)
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"061609f85a8e0191dc7f487f8278e71898a2ee2d\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"SHIPPING_CODE\": \"50001\", \"SHIPPING_CO\": \"US\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void BillingAndShippingData() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                },
            });

            var billingAddress = new Address {
                Country = "US",
                PostalCode = "50001"
            };

            var shippingAddress = new Address {
                Country = "GB",
                PostalCode = "654|123"
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithAddress(billingAddress, AddressType.Billing)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"061609f85a8e0191dc7f487f8278e71898a2ee2d\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"SHIPPING_CODE\": \"654|123\", \"SHIPPING_CO\": \"GB\", \"BILLING_CODE\": \"50001\", \"BILLING_CO\": \"US\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void FraudFilterPassive() {

            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    FraudFilterMode = FraudFilterMode.PASSIVE
                },
            });

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"fff944d4da9a5dfd64d142448d5dcf6168b3b77f\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"HPP_FRAUDFILTER_MODE\": \"PASSIVE\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void FraudFilterWithFraudRules()
        {

            string ruleId = "853c1d37-6e9f-467e-9ffc-182210b40c6b";
            FraudFilterMode mode = FraudFilterMode.OFF;
            FraudRuleCollection fraudRuleCollection = new FraudRuleCollection();
            fraudRuleCollection.AddRule(ruleId, mode);

            var service = new HostedService(new GpEcomConfig
            {
                MerchantId = "heartlandgpsandbox",
                AccountId = "hpp",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig
                {
                    ResponseUrl = "https://www.example.com/response",
                    FraudFilterMode = FraudFilterMode.PASSIVE,
                    FraudFilterRules = fraudRuleCollection
                },
                RequestLogger = new RequestConsoleLogger()
            });

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .Serialize();

            var response = _client.SendRequest(hppJson);
            var parsedResponse = _service.ParseResponse(response, true);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", parsedResponse.ResponseCode);
            Assert.AreEqual(parsedResponse.ResponseValues["HPP_FRAUDFILTER_MODE"], FraudFilterMode.PASSIVE.ToString());
            Assert.AreEqual(parsedResponse.ResponseValues["HPP_FRAUDFILTER_RESULT"], "PASS");
            var rule = parsedResponse.ResponseValues.FirstOrDefault(x => x.Key.EndsWith(ruleId));
            Assert.IsNotNull(rule);
            Assert.AreEqual("NOT_EXECUTED", rule.Value);
        }

        [TestMethod]
        public void FraudFilterParseResponseWithoutFraudRules()
        {
            var json = "{  \"MERCHANT_ID\": \"aGVhcnRsYW5kZ3BzYW5kYm94\",  \"ACCOUNT\": \"aHBw\",  \"ORDER_ID\": \"VHdWZGVpU1ZlMHFJb05DMG1OVjJyZw==\",  \"TIMESTAMP\": \"MjAyMTA4MjYyMTU1MjU=\",  \"RESULT\": \"MDA=\",  \"PASREF\": \"MTYzMDAxMTMyNTg5NzM4OTI=\",  \"AUTHCODE\": \"MTIzNDU=\",  \"AVSPOSTCODERESULT\": \"TQ==\",  \"CVNRESULT\": \"TQ==\",  \"MERCHANT_RESPONSE_URL\": \"aHR0cHM6Ly93d3cuZXhhbXBsZS5jb20vcmVzcG9uc2U=\",  \"MESSAGE\": \"WyB0ZXN0IHN5c3RlbSBdIEF1dGhvcmlzZWQ=\",  \"SHA1HASH\": \"MjM5NGFmZjNlZDgzNTUwYWEwOTZmMGMzZWJkMjUyYzBlOGUzN2ZmYQ==\",}";
            var parsedResponse = _service.ParseResponse(json, true);
            Assert.IsNotNull(parsedResponse);
            Assert.AreEqual("00", parsedResponse.ResponseCode);
        }

        [TestMethod]
        public void FraudFilterOff() {

            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    FraudFilterMode = FraudFilterMode.OFF
                },
            });

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"2e98407a26f17dc8c7ed89df5cc69d17718bfeb2\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"HPP_FRAUDFILTER_MODE\": \"OFF\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void FraudFilterNone() {

            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    FraudFilterMode = FraudFilterMode.NONE
                },
            });

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"061609f85a8e0191dc7f487f8278e71898a2ee2d\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void DynamicCurrencyConversionOn() {

            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    DynamicCurrencyConversionEnabled = true
                },
            });

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"061609f85a8e0191dc7f487f8278e71898a2ee2d\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"DCC_ENABLE\": \"1\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void DynamicCurrencyConversionOff() {

            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    DynamicCurrencyConversionEnabled = false
                },
            });

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"061609f85a8e0191dc7f487f8278e71898a2ee2d\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"DCC_ENABLE\": \"0\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void ReturnTssOn() {

            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    RequestTransactionStabilityScore = true
                },
            });

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"061609f85a8e0191dc7f487f8278e71898a2ee2d\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"RETURN_TSS\": \"1\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void ReturnTssOff() {

            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    RequestTransactionStabilityScore = false
                },
            });

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"061609f85a8e0191dc7f487f8278e71898a2ee2d\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"RETURN_TSS\": \"0\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void RecurringInfo() {

            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2"
                },
            });

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithRecurringInfo(RecurringType.Fixed, RecurringSequence.First)
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"061609f85a8e0191dc7f487f8278e71898a2ee2d\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"RECURRING_TYPE\": \"fixed\", \"RECURRING_SEQUENCE\": \"first\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));

            hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithRecurringInfo(RecurringType.Variable, RecurringSequence.Last)
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .Serialize();

            expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"061609f85a8e0191dc7f487f8278e71898a2ee2d\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"RECURRING_TYPE\": \"variable\", \"RECURRING_SEQUENCE\": \"last\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));

            hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithRecurringInfo(RecurringType.Fixed, RecurringSequence.Subsequent)
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .Serialize();

            expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"061609f85a8e0191dc7f487f8278e71898a2ee2d\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"RECURRING_TYPE\": \"fixed\", \"RECURRING_SEQUENCE\": \"subsequent\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void HashCheckAllInputs() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    CardStorageEnabled = true,
                    FraudFilterMode = FraudFilterMode.PASSIVE
                },
            });

            var testHostedPaymentData = new HostedPaymentData {
                CustomerKey = "376a2598-412d-4805-9f47-c177d5605853",
                PaymentKey = "ca46344d-4292-47dc-9ced-e8a42ce66977"
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithHostedPaymentData(testHostedPaymentData)
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"1384392a30abbd7a1993e33c308bf9a2bd354d48\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"CARD_STORAGE_ENABLE\": \"1\", \"PMT_REF\": \"ca46344d-4292-47dc-9ced-e8a42ce66977\", \"PAYER_REF\": \"376a2598-412d-4805-9f47-c177d5605853\", \"HPP_FRAUDFILTER_MODE\": \"PASSIVE\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void HashCheckNoPaymentRef() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    CardStorageEnabled = true,
                    FraudFilterMode = FraudFilterMode.PASSIVE
                },
            });

            var testHostedPaymentData = new HostedPaymentData {
                CustomerKey = "376a2598-412d-4805-9f47-c177d5605853"
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithHostedPaymentData(testHostedPaymentData)
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"c10b55c16276366ced59174cbab20a6eeeec16c9\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"CARD_STORAGE_ENABLE\": \"1\", \"PAYER_REF\": \"376a2598-412d-4805-9f47-c177d5605853\", \"HPP_FRAUDFILTER_MODE\": \"PASSIVE\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void HashCheckNoPayerRef() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    CardStorageEnabled = true,
                    FraudFilterMode = FraudFilterMode.PASSIVE
                },
            });

            var testHostedPaymentData = new HostedPaymentData {
                PaymentKey = "ca46344d-4292-47dc-9ced-e8a42ce66977"
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithHostedPaymentData(testHostedPaymentData)
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"73236b35e253215380a9bf2f7a1f11ac23204224\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"CARD_STORAGE_ENABLE\": \"1\", \"PMT_REF\": \"ca46344d-4292-47dc-9ced-e8a42ce66977\", \"HPP_FRAUDFILTER_MODE\": \"PASSIVE\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void HashCheckFraudFilterNone() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    CardStorageEnabled = true,
                    FraudFilterMode = FraudFilterMode.NONE
                },
            });

            var testHostedPaymentData = new HostedPaymentData {
                PaymentKey = "ca46344d-4292-47dc-9ced-e8a42ce66977"
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithHostedPaymentData(testHostedPaymentData)
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"5fe76a45585d9793fd162ab8a3cd4a42991417df\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"CARD_STORAGE_ENABLE\": \"1\", \"PMT_REF\": \"ca46344d-4292-47dc-9ced-e8a42ce66977\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void AllFieldsCheck() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                    CardStorageEnabled = true,
                    DisplaySavedCards = true,
                    DynamicCurrencyConversionEnabled = true,
                    FraudFilterMode = FraudFilterMode.PASSIVE,
                    Language = "EN",
                    PaymentButtonText = "Place Order",
                    RequestTransactionStabilityScore = true
                },
            });

            var billingAddress = new Address {
                Country = "US",
                PostalCode = "50001"
            };

            var shippingAddress = new Address {
                Country = "GB",
                PostalCode = "654|123"
            };

            var testHostedPaymentData = new HostedPaymentData {
                CustomerExists = true,
                CustomerKey = "376a2598-412d-4805-9f47-c177d5605853",
                PaymentKey = "ca46344d-4292-47dc-9ced-e8a42ce66977",
                CustomerNumber = "a028774f-beff-47bc-bd6e-ed7e04f5d758a028774f-btefa",
                ProductId = "a0b38df5-b23c-4d82-88fe-2e9c47438972-b23c-4d82-88f",
                OfferToSaveCard = true
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithAddress(billingAddress, AddressType.Billing)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithHostedPaymentData(testHostedPaymentData)
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"1384392a30abbd7a1993e33c308bf9a2bd354d48\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"SHIPPING_CODE\": \"654|123\", \"SHIPPING_CO\": \"GB\", \"BILLING_CODE\": \"50001\", \"BILLING_CO\": \"US\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"CARD_STORAGE_ENABLE\": \"1\", \"OFFER_SAVE_CARD\": \"1\", \"HPP_SELECT_STORED_CARD\": \"376a2598-412d-4805-9f47-c177d5605853\", \"DCC_ENABLE\": \"1\", \"HPP_FRAUDFILTER_MODE\": \"PASSIVE\", \"HPP_LANG\": \"EN\", \"RETURN_TSS\": \"1\", \"PAYER_EXIST\": \"1\", \"PMT_REF\": \"ca46344d-4292-47dc-9ced-e8a42ce66977\", \"CUST_NUM\": \"a028774f-beff-47bc-bd6e-ed7e04f5d758a028774f-btefa\", \"PROD_ID\": \"a0b38df5-b23c-4d82-88fe-2e9c47438972-b23c-4d82-88f\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }
        
        [TestMethod]
        public void CreditAuth_MultiAutoSettle() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2"
                },
            });

            string hppJson = service.Authorize(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithMultiCapture(true)
                .Serialize();

            string expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"MULTI\",\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"061609f85a8e0191dc7f487f8278e71898a2ee2d\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }
        
        [TestMethod]
        public void BasicChargeAlternativePayment() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "hpp",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    Version = "2",
                },
            });

            var testHostedPaymentData = new HostedPaymentData {
                CustomerCountry = "DE",
                CustomerFirstName = "James",
                CustomerLastName = "Mason",
                MerchantResponseUrl = "https://www.example.com/returnUrl",
                TransactionStatusUrl = "https://www.example.com/statusUrl",
                PresetPaymentMethods = new AlternativePaymentType[]{ AlternativePaymentType.ASTROPAY_DIRECT,AlternativePaymentType.AURA,AlternativePaymentType.BALOTO_CASH,AlternativePaymentType.BANAMEX }
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithHostedPaymentData(testHostedPaymentData)
                .Serialize();

            var expectedJson = "{ \"MERCHANT_ID\": \"heartlandgpsandbox\", \"ACCOUNT\": \"hpp\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"AUTO_SETTLE_FLAG\": \"1\", \"HPP_CUSTOMER_COUNTRY\": \"DE\", \"HPP_CUSTOMER_FIRSTNAME\": \"James\", \"HPP_CUSTOMER_LASTNAME\": \"Mason\", \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/returnUrl\", \"HPP_TX_STATUS_URL\": \"https://www.example.com/statusUrl\", \"PM_METHODS\": \"ASTROPAY_DIRECT|AURA|BALOTO_CASH|BANAMEX\", \"HPP_VERSION\":\"2\", \"SHA1HASH\":\"647d071bdcb8d9da5f29688a787863a39dc51ef3\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void NetherlandsAntillesCountry()
        {
            var service = new HostedService(new GpEcomConfig
            {
                MerchantId = "heartlandgpsandbox",
                AccountId = "hpp",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig
                {
                    Version = "2",
                },
            });

            var testHostedPaymentData = new HostedPaymentData
            {
                CustomerCountry = "DE",
                CustomerFirstName = "James",
                CustomerLastName = "Mason",
                MerchantResponseUrl = "https://www.example.com/returnUrl",
                TransactionStatusUrl = "https://www.example.com/statusUrl",
                PresetPaymentMethods = new AlternativePaymentType[] { AlternativePaymentType.ASTROPAY_DIRECT, AlternativePaymentType.AURA, AlternativePaymentType.BALOTO_CASH, AlternativePaymentType.BANAMEX }
            };

            Address billingAddress = new Address
            {
                StreetAddress1 = "Flat 123",
                StreetAddress2 = "House 456",
                StreetAddress3 = "Unit 4",
                City = "Halifax",
                PostalCode = "W5 9HR",
                Country = "AN"
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithHostedPaymentData(testHostedPaymentData)
                .WithAddress(billingAddress)
                .Serialize();

            Assert.IsNotNull(hppJson);
            Assert.IsTrue(hppJson.Contains("\"HPP_BILLING_COUNTRY\":\"530\""));
            Assert.IsTrue(hppJson.Contains("\"BILLING_CO\":\"AN\""));
        }
    }
}