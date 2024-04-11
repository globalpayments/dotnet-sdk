using System;
using System.Linq;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.GpEcom.Hpp;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpEcom {
    [TestClass]
    public class GpEcomHppRequestTest {
        HostedService _service;
        RealexHppClient _client;
        private string firstConfig = "firstConfig";
        private string secondConfig = "secondConfig";
        private const ShaHashType _shahashType = ShaHashType.SHA1;

        public GpEcomHppRequestTest() {
            _client = new RealexHppClient("https://pay.sandbox.realexpayments.com/pay", "secret", _shahashType);
            _service = new HostedService(new GpEcomConfig
            {
                MerchantId = "heartlandgpsandbox",
                AccountId = "hpp",
                SharedSecret = "secret",
                HostedPaymentConfig = new HostedPaymentConfig
                {
                    Language = "GB",
                    ResponseUrl = "http://requestb.in/10q2bjb1"
                },
                ShaHashType = _shahashType,
                RequestLogger = new RequestConsoleLogger()
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

        [TestMethod]
        public void CreditVerify_3DS()
        {
            _service = new HostedService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "3dsecure",
                SharedSecret = "secret",
                ShaHashType = _shahashType,
                HostedPaymentConfig = new HostedPaymentConfig {
                    Language = "GB",
                },
            });
            
            var shippingAddress = new Address {
                StreetAddress1 = "Apartment 852",
                StreetAddress2 = "Complex 741",
                StreetAddress3 = "no",
                City = "Chicago",
                PostalCode = "5001",
                State = "IL",
                Country = "840",
            };
            
            var billingAddress = new Address {
                StreetAddress1 = "Flat 123",
                StreetAddress2 = "House 456",
                StreetAddress3 = "Cul-De-Sac",
                City = "Halifax",
                Province = "West Yorkshire",
                State = "Yorkshire and the Humber",
                Country = "826",
                PostalCode = "E77 4QJ"
            };
            
            var testHostedPaymentData = new HostedPaymentData {
                CustomerEmail = "james.mason@example.com",
                CustomerPhoneMobile =  "44|07123456789",
                AddressesMatch = false,
                CustomerCountry = "GB",
                CustomerFirstName = "James",
                CustomerLastName = "Mason",
                MerchantResponseUrl = "http://requestb.in/10q2bjb1",
                TransactionStatusUrl = "http://requestb.in/10q2bjb123"
            };
            
            var json = _service.Verify(0)
                .WithCurrency("EUR")
                .WithCustomerId("123456")
                .WithAddress(billingAddress, AddressType.Billing)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithHostedPaymentData(testHostedPaymentData)
                .Serialize();
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

            var expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"COMMENT1\":\"Mobile Channel\",\"CUST_NUM\":\"a028774f-beff-47bc-bd6e-ed7e04f5d758a028774f-btefa\",\"PROD_ID\":\"a0b38df5-b23c-4d82-88fe-2e9c47438972-b23c-4d82-88f\",\"HPP_CHALLENGE_REQUEST_INDICATOR\": \"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\": false,\"VAR_REF\":\"My Legal Entity\",\"HPP_LANG\":\"EN\",\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"CARD_PAYMENT_BUTTON\":\"Place Order\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"061609f85a8e0191dc7f487f8278e71898a2ee2d\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }
        
        [TestMethod]
        // testing COMMENT1, CUST_NUM, PROD_ID, VAR_REF, HPP_LANG, CARD_PAYMENT_BUTTON
        public void BasicHostedPaymentData_SHA() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
                ShaHashType = ShaHashType.SHA256,
                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
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

            const string expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"COMMENT1\":\"Mobile Channel\",\"CUST_NUM\":\"a028774f-beff-47bc-bd6e-ed7e04f5d758a028774f-btefa\",\"PROD_ID\":\"a0b38df5-b23c-4d82-88fe-2e9c47438972-b23c-4d82-88f\",\"HPP_CHALLENGE_REQUEST_INDICATOR\":\"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\":false,\"VAR_REF\":\"My Legal Entity\",\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"HPP_VERSION\":\"2\",\"SHA256HASH\":\"d23f0076aea69b39dc6d268ce30bf631470969b6e20560ad352f8edd87d4481c\"}";
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

            var expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"OFFER_SAVE_CARD\":\"1\",\"PAYER_EXIST\":\"0\",\"HPP_CHALLENGE_REQUEST_INDICATOR\": \"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\": false,\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"CARD_STORAGE_ENABLE\":\"1\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"7116c49826367c6513efdc0cc81e243b8095d78f\"}";
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

            var expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"OFFER_SAVE_CARD\":\"1\",\"PAYER_EXIST\":\"0\",\"HPP_CHALLENGE_REQUEST_INDICATOR\": \"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\": false,\"PAYER_REF\":\"376a2598-412d-4805-9f47-c177d5605853\",\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"CARD_STORAGE_ENABLE\":\"1\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"4dcf4e5e2d43855fe31cdc097e985a895868563e\"}";
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

            var expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"OFFER_SAVE_CARD\":\"1\",\"PAYER_EXIST\":\"0\",\"HPP_CHALLENGE_REQUEST_INDICATOR\": \"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\": false,\"PMT_REF\":\"ca46344d-4292-47dc-9ced-e8a42ce66977\",\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"CARD_STORAGE_ENABLE\":\"1\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"5fe76a45585d9793fd162ab8a3cd4a42991417df\"}";
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

            var expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"OFFER_SAVE_CARD\":\"1\",\"PAYER_EXIST\":\"0\",\"HPP_CHALLENGE_REQUEST_INDICATOR\": \"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\": false,\"PAYER_REF\":\"376a2598-412d-4805-9f47-c177d5605853\",\"PMT_REF\":\"ca46344d-4292-47dc-9ced-e8a42ce66977\",\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"CARD_STORAGE_ENABLE\":\"1\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"f0cf097fe769a6a5a6254eee631e51709ba34c90\"}";
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

            var expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"OFFER_SAVE_CARD\":\"0\",\"PAYER_EXIST\":\"0\",\"HPP_CHALLENGE_REQUEST_INDICATOR\": \"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\": false,\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"CARD_STORAGE_ENABLE\":\"1\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"7116c49826367c6513efdc0cc81e243b8095d78f\"}";
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

            var expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"OFFER_SAVE_CARD\":\"1\",\"PAYER_EXIST\":\"1\",\"HPP_CHALLENGE_REQUEST_INDICATOR\": \"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\": false,\"PAYER_REF\":\"376a2598-412d-4805-9f47-c177d5605853\",\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"CARD_STORAGE_ENABLE\":\"1\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"4dcf4e5e2d43855fe31cdc097e985a895868563e\"}";
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

            var expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"OFFER_SAVE_CARD\":\"1\",\"PAYER_EXIST\":\"1\",\"HPP_CHALLENGE_REQUEST_INDICATOR\": \"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\": false,\"PMT_REF\":\"ca46344d-4292-47dc-9ced-e8a42ce66977\",\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"CARD_STORAGE_ENABLE\":\"1\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"5fe76a45585d9793fd162ab8a3cd4a42991417df\"}";
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

            var expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"OFFER_SAVE_CARD\":\"0\",\"PAYER_EXIST\":\"1\",\"HPP_CHALLENGE_REQUEST_INDICATOR\": \"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\": false,\"PAYER_REF\":\"376a2598-412d-4805-9f47-c177d5605853\",\"PMT_REF\":\"ca46344d-4292-47dc-9ced-e8a42ce66977\",\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"CARD_STORAGE_ENABLE\":\"1\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"f0cf097fe769a6a5a6254eee631e51709ba34c90\"}";
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

            var expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"HPP_SELECT_STORED_CARD\":\"376a2598-412d-4805-9f47-c177d5605853\",\"OFFER_SAVE_CARD\":\"1\",\"PAYER_EXIST\":\"1\",\"HPP_CHALLENGE_REQUEST_INDICATOR\": \"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\": false,\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"4dcf4e5e2d43855fe31cdc097e985a895868563e\"}";
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

            var expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"HPP_SELECT_STORED_CARD\":\"376a2598-412d-4805-9f47-c177d5605853\",\"OFFER_SAVE_CARD\":\"1\",\"PAYER_EXIST\":\"1\",\"PMT_REF\":\"ca46344d-4292-47dc-9ced-e8a42ce66977\",\"HPP_CHALLENGE_REQUEST_INDICATOR\": \"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\": false,\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"f0cf097fe769a6a5a6254eee631e51709ba34c90\"}";
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

            var expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"BILLING_CODE\":\"50001|\",\"BILLING_CO\":\"US\",\"HPP_BILLING_POSTALCODE\":\"50001\",\"HPP_BILLING_COUNTRY\":\"840\",\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"061609f85a8e0191dc7f487f8278e71898a2ee2d\"}";
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

            var expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"SHIPPING_CODE\":\"50001|\",\"SHIPPING_CO\":\"US\",\"HPP_SHIPPING_POSTALCODE\":\"50001\",\"HPP_SHIPPING_COUNTRY\":\"840\",\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"061609f85a8e0191dc7f487f8278e71898a2ee2d\"}";
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

            var expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"SHIPPING_CODE\":\"654123|\",\"SHIPPING_CO\":\"GB\",\"HPP_SHIPPING_POSTALCODE\":\"654|123\",\"HPP_SHIPPING_COUNTRY\":\"826\",\"BILLING_CODE\":\"50001|\",\"BILLING_CO\":\"US\",\"HPP_BILLING_POSTALCODE\":\"50001\",\"HPP_BILLING_COUNTRY\":\"840\",\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"061609f85a8e0191dc7f487f8278e71898a2ee2d\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void CaptureBillingAndShippingInformation()
        {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "hpp",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2",
                }
            });

            var captureAddress = true;
            var returnAddress = false;

            var testHostedPaymentData = new HostedPaymentData {
                CaptureAddress = captureAddress,
                ReturnAddress = returnAddress,                
            };

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
                .WithHostedPaymentData(testHostedPaymentData)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .Serialize();

            var jsonParse = JsonDoc.Parse(hppJson);
            
            Assert.IsTrue(hppJson.Contains("\"HPP_CAPTURE_ADDRESS\":\"TRUE\""));
            Assert.IsTrue(hppJson.Contains("\"HPP_DO_NOT_RETURN_ADDRESS\":\"FALSE\""));
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
                ShaHashType = _shahashType,
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
            var json = string.Empty;
            if (_shahashType == ShaHashType.SHA1)
            {
                json = "{  \"MERCHANT_ID\": \"aGVhcnRsYW5kZ3BzYW5kYm94\",  \"ACCOUNT\": \"aHBw\",  \"ORDER_ID\": \"VHdWZGVpU1ZlMHFJb05DMG1OVjJyZw==\",  \"TIMESTAMP\": \"MjAyMTA4MjYyMTU1MjU=\",  \"RESULT\": \"MDA=\",  \"PASREF\": \"MTYzMDAxMTMyNTg5NzM4OTI=\",  \"AUTHCODE\": \"MTIzNDU=\",  \"AVSPOSTCODERESULT\": \"TQ==\",  \"CVNRESULT\": \"TQ==\",  \"MERCHANT_RESPONSE_URL\": \"aHR0cHM6Ly93d3cuZXhhbXBsZS5jb20vcmVzcG9uc2U=\",  \"MESSAGE\": \"WyB0ZXN0IHN5c3RlbSBdIEF1dGhvcmlzZWQ=\",  \"SHA1HASH\": \"MjM5NGFmZjNlZDgzNTUwYWEwOTZmMGMzZWJkMjUyYzBlOGUzN2ZmYQ==\",}";
            }
            else
            {
                json = "{\"MERCHANT_ID\":\"aGVhcnRsYW5kZ3BzYW5kYm94\",\"ACCOUNT\":\"aHBw\",\"ORDER_ID\":\"YndQSnM1cHJ3a0dCZDB2UkUwMG5vZw==\",\"TIMESTAMP\":\"MjAyNDAyMTIxNjQ0NDQ=\",\"RESULT\":\"MDA=\",\"PASREF\":\"MTcwNzc1NjI4NDIwNzk1MDI=\",\"AUTHCODE\":\"MTIzNDU2\",\"AVSPOSTCODERESULT\":\"TQ==\",\"CVNRESULT\":\"TQ==\",\"HPP_LANG\":\"R0I=\",\"BILLING_CO\":\"SUU=\",\"MERCHANT_RESPONSE_URL\":\"aHR0cDovL3JlcXVlc3RiLmluLzEwcTJiamIx\",\"MESSAGE\":\"WyB0ZXN0IHN5c3RlbSBdIEF1dGhvcmlzZWQ=\",\"SHA256HASH\":\"NjVmODcyNjA5M2ZhZTEzMTY5YzNiODdkMGU0NTkyNzAzOGUxOGI1YjlhMTdmYzQ2YmViMjM3ODQ2YTg5MzY2Ng==\",}";
            }
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

            // var expectedJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"CURRENCY\": \"EUR\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"1384392a30abbd7a1993e33c308bf9a2bd354d48\", \"AUTO_SETTLE_FLAG\": \"1\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"HPP_VERSION\": \"2\", \"CARD_STORAGE_ENABLE\": \"1\", \"PMT_REF\": \"ca46344d-4292-47dc-9ced-e8a42ce66977\", \"PAYER_REF\": \"376a2598-412d-4805-9f47-c177d5605853\", \"HPP_FRAUDFILTER_MODE\": \"PASSIVE\"}";
            var expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"PAYER_REF\":\"376a2598-412d-4805-9f47-c177d5605853\",\"PMT_REF\":\"ca46344d-4292-47dc-9ced-e8a42ce66977\",\"HPP_CHALLENGE_REQUEST_INDICATOR\": \"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\": false,\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"CARD_STORAGE_ENABLE\":\"1\",\"HPP_FRAUDFILTER_MODE\":\"PASSIVE\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"1384392a30abbd7a1993e33c308bf9a2bd354d48\"}";
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

            var expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"PAYER_REF\":\"376a2598-412d-4805-9f47-c177d5605853\",\"HPP_CHALLENGE_REQUEST_INDICATOR\": \"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\": false,\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"CARD_STORAGE_ENABLE\":\"1\",\"HPP_FRAUDFILTER_MODE\":\"PASSIVE\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"c10b55c16276366ced59174cbab20a6eeeec16c9\"}";
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

            var expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"PMT_REF\":\"ca46344d-4292-47dc-9ced-e8a42ce66977\",\"HPP_CHALLENGE_REQUEST_INDICATOR\": \"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\": false,\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"CARD_STORAGE_ENABLE\":\"1\",\"HPP_FRAUDFILTER_MODE\":\"PASSIVE\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"73236b35e253215380a9bf2f7a1f11ac23204224\"}";
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

            var expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"PMT_REF\":\"ca46344d-4292-47dc-9ced-e8a42ce66977\",\"HPP_CHALLENGE_REQUEST_INDICATOR\": \"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\": false,\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"CARD_STORAGE_ENABLE\":\"1\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"5fe76a45585d9793fd162ab8a3cd4a42991417df\"}";
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
                PostalCode = "654",
                StreetAddress1 = "123"
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

            var expectedJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"RETURN_TSS\":\"1\",\"DCC_ENABLE\":\"1\",\"CUST_NUM\":\"a028774f-beff-47bc-bd6e-ed7e04f5d758a028774f-btefa\",\"HPP_SELECT_STORED_CARD\":\"376a2598-412d-4805-9f47-c177d5605853\",\"OFFER_SAVE_CARD\":\"1\",\"PAYER_EXIST\":\"1\",\"PMT_REF\":\"ca46344d-4292-47dc-9ced-e8a42ce66977\",\"PROD_ID\":\"a0b38df5-b23c-4d82-88fe-2e9c47438972-b23c-4d82-88f\",\"HPP_CHALLENGE_REQUEST_INDICATOR\": \"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\": false,\"SHIPPING_CODE\":\"654|123\",\"SHIPPING_CO\":\"GB\",\"HPP_SHIPPING_STREET1\": \"123\",\"HPP_SHIPPING_POSTALCODE\":\"654\",\"HPP_SHIPPING_COUNTRY\":\"826\",\"BILLING_CODE\":\"50001|\",\"BILLING_CO\":\"US\",\"HPP_BILLING_POSTALCODE\":\"50001\",\"HPP_BILLING_COUNTRY\":\"840\",\"HPP_LANG\":\"EN\",\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"CARD_PAYMENT_BUTTON\":\"Place Order\",\"CARD_STORAGE_ENABLE\":\"1\",\"HPP_FRAUDFILTER_MODE\":\"PASSIVE\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"1384392a30abbd7a1993e33c308bf9a2bd354d48\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void MultipleConfig()
        {
            #region Configuration
            var service = new HostedService(new GpEcomConfig
            {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig
                {
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
            }, firstConfig);

            var service2 = new HostedService(new GpEcomConfig
            {
                MerchantId = "MerchantIdSecondConfig",
                AccountId = "internetSecondConfig",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig
                {
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
            }, secondConfig);
            #endregion

            var billingAddress = new Address
            {
                Country = "US",
                PostalCode = "50001"
            };

            var shippingAddress = new Address
            {
                Country = "GB",
                PostalCode = "654|123"
            };

            var testHostedPaymentData = new HostedPaymentData
            {
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
                .Serialize(firstConfig);

            var expectedFirstJson = "{\"MERCHANT_ID\":\"MerchantId\",\"ACCOUNT\":\"internet\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"RETURN_TSS\":\"1\",\"DCC_ENABLE\":\"1\",\"CUST_NUM\":\"a028774f-beff-47bc-bd6e-ed7e04f5d758a028774f-btefa\",\"HPP_SELECT_STORED_CARD\":\"376a2598-412d-4805-9f47-c177d5605853\",\"OFFER_SAVE_CARD\":\"1\",\"PAYER_EXIST\":\"1\",\"PMT_REF\":\"ca46344d-4292-47dc-9ced-e8a42ce66977\",\"PROD_ID\":\"a0b38df5-b23c-4d82-88fe-2e9c47438972-b23c-4d82-88f\",\"HPP_CHALLENGE_REQUEST_INDICATOR\":\"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\":false,\"SHIPPING_CODE\":\"654123|\",\"SHIPPING_CO\":\"GB\",\"HPP_SHIPPING_POSTALCODE\":\"654|123\",\"HPP_SHIPPING_COUNTRY\":\"826\",\"BILLING_CODE\":\"50001|\",\"BILLING_CO\":\"US\",\"HPP_BILLING_POSTALCODE\":\"50001\",\"HPP_BILLING_COUNTRY\":\"840\",\"HPP_LANG\":\"EN\",\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"CARD_PAYMENT_BUTTON\":\"Place Order\",\"CARD_STORAGE_ENABLE\":\"1\",\"HPP_FRAUDFILTER_MODE\":\"PASSIVE\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"1384392a30abbd7a1993e33c308bf9a2bd354d48\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedFirstJson, hppJson));            
            
            var hppJson2 = service2.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithAddress(billingAddress, AddressType.Billing)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithHostedPaymentData(testHostedPaymentData)
                .Serialize(secondConfig);

            var expectedSecondJson = "{\"MERCHANT_ID\":\"MerchantIdSecondConfig\",\"ACCOUNT\":\"internetSecondConfig\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"RETURN_TSS\":\"1\",\"DCC_ENABLE\":\"1\",\"CUST_NUM\":\"a028774f-beff-47bc-bd6e-ed7e04f5d758a028774f-btefa\",\"HPP_SELECT_STORED_CARD\":\"376a2598-412d-4805-9f47-c177d5605853\",\"OFFER_SAVE_CARD\":\"1\",\"PAYER_EXIST\":\"1\",\"PMT_REF\":\"ca46344d-4292-47dc-9ced-e8a42ce66977\",\"PROD_ID\":\"a0b38df5-b23c-4d82-88fe-2e9c47438972-b23c-4d82-88f\",\"HPP_CHALLENGE_REQUEST_INDICATOR\":\"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\":false,\"SHIPPING_CODE\":\"654123|\",\"SHIPPING_CO\":\"GB\",\"HPP_SHIPPING_POSTALCODE\":\"654|123\",\"HPP_SHIPPING_COUNTRY\":\"826\",\"BILLING_CODE\":\"50001|\",\"BILLING_CO\":\"US\",\"HPP_BILLING_POSTALCODE\":\"50001\",\"HPP_BILLING_COUNTRY\":\"840\",\"HPP_LANG\":\"EN\",\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/response\",\"CARD_PAYMENT_BUTTON\":\"Place Order\",\"CARD_STORAGE_ENABLE\":\"1\",\"HPP_FRAUDFILTER_MODE\":\"PASSIVE\",\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"264dcaaa65ac0ab17ccddf6ef3b9015c18168f10\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedSecondJson, hppJson2));
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

            var expectedJson = "{\"MERCHANT_ID\":\"heartlandgpsandbox\",\"ACCOUNT\":\"hpp\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"HPP_CUSTOMER_COUNTRY\":\"DE\",\"HPP_CUSTOMER_FIRSTNAME\":\"James\",\"HPP_CUSTOMER_LASTNAME\":\"Mason\",\"HPP_NAME\":\"James Mason\",\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/returnUrl\",\"HPP_TX_STATUS_URL\":\"https://www.example.com/statusUrl\",\"PM_METHODS\":\"ASTROPAY_DIRECT|AURA|BALOTO_CASH|BANAMEX\",\"HPP_VERSION\":\"2\",\"HPP_CHALLENGE_REQUEST_INDICATOR\": \"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\": false,\"SHA1HASH\":\"647d071bdcb8d9da5f29688a787863a39dc51ef3\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }
        
        [TestMethod]
        public void BasicChargeCardAndAlternativePayment() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "hpp",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig {
                    Version = "2",
                },
                RequestLogger = new RequestConsoleLogger()
            });

            var testHostedPaymentData = new HostedPaymentData {
                CustomerCountry = "DE",
                CustomerFirstName = "James",
                CustomerLastName = "Mason",
                MerchantResponseUrl = "https://www.example.com/returnUrl",
                TransactionStatusUrl = "https://www.example.com/statusUrl",
                HostedPaymentMethods = new HostedPaymentMethods[] { HostedPaymentMethods.CARDS },
                PresetPaymentMethods = new AlternativePaymentType[]{ AlternativePaymentType.ASTROPAY_DIRECT,AlternativePaymentType.AURA,AlternativePaymentType.BALOTO_CASH,AlternativePaymentType.BANAMEX },
            };

            var hppJson = service.Charge(19.99m)
                .WithCurrency("EUR")
                .WithTimestamp("20170725154824")
                .WithOrderId("GTI5Yxb0SumL_TkDMCAxQA")
                .WithHostedPaymentData(testHostedPaymentData)
                .Serialize();

            var expectedJson = "{\"MERCHANT_ID\":\"heartlandgpsandbox\",\"ACCOUNT\":\"hpp\",\"ORDER_ID\":\"GTI5Yxb0SumL_TkDMCAxQA\",\"AMOUNT\":\"1999\",\"CURRENCY\":\"EUR\",\"TIMESTAMP\":\"20170725154824\",\"AUTO_SETTLE_FLAG\":\"1\",\"HPP_CUSTOMER_COUNTRY\":\"DE\",\"HPP_CUSTOMER_FIRSTNAME\":\"James\",\"HPP_CUSTOMER_LASTNAME\":\"Mason\",\"HPP_NAME\":\"James Mason\",\"MERCHANT_RESPONSE_URL\":\"https://www.example.com/returnUrl\",\"HPP_TX_STATUS_URL\":\"https://www.example.com/statusUrl\",\"PM_METHODS\":\"CARDS|ASTROPAY_DIRECT|AURA|BALOTO_CASH|BANAMEX\",\"HPP_CHALLENGE_REQUEST_INDICATOR\":\"NO_PREFERENCE\",\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\":false,\"HPP_VERSION\":\"2\",\"SHA1HASH\":\"647d071bdcb8d9da5f29688a787863a39dc51ef3\"}";
            Assert.AreEqual(true, JsonComparator.AreEqual(expectedJson, hppJson));
        }

        [TestMethod]
        public void OpenBankingInitiate()
        {
            var config = new GpEcomConfig();
            config.MerchantId = "openbankingsandbox";
            config.SharedSecret = "sharedsecret";
            config.AccountId = "internet";
            config.ServiceUrl = "https://pay.sandbox.realexpayments.com/pay";
            config.EnableBankPayment = true;
            config.HostedPaymentConfig = new HostedPaymentConfig
            {                
                Version = HppVersion.VERSION_1
            };                        
            config.RequestLogger = new RequestConsoleLogger();

            var hostedPaymentData = new HostedPaymentData();
            hostedPaymentData.CustomerCountry = "DE";
            hostedPaymentData.CustomerFirstName = "James";
            hostedPaymentData.CustomerLastName = "Mason";
            hostedPaymentData.TransactionStatusUrl = "https://www.example.com/statusUrl";
            hostedPaymentData.MerchantResponseUrl = "https://www.example.com/responseUrl";

            //Validate AlternativePaymentType
            //hostedPaymentData.PresetPaymentMethods = new AlternativePaymentType[] { AlternativePaymentType.SOFORT, AlternativePaymentType.BANAMEX };
            hostedPaymentData.HostedPaymentMethods = new HostedPaymentMethods[] { HostedPaymentMethods.OB };

            var bankPayment = new BankPayment();
            bankPayment.AccountNumber = "12345678";
            bankPayment.SortCode = "406650";
            bankPayment.AccountName = "AccountName";

            var client = new RealexHppClient(config.ServiceUrl, config.SharedSecret, ShaHashType.SHA1);
            var service = new HostedService(config);

            var json = service.Charge((decimal)10.99)
                .WithCurrency("GBP")
                .WithPaymentMethod(bankPayment)
                .WithHostedPaymentData(hostedPaymentData)
                .WithRemittanceReference(RemittanceReferenceType.TEXT, "Nike Bounce Shoes")
                .Serialize();
            Assert.IsNotNull(json);
            var response = client.SendRequest(json);
            Assert.IsNotNull(response);

            var parsedResponse = service.ParseResponse(response, true);
            Assert.AreEqual("PAYMENT_INITIATED", parsedResponse.ResponseMessage);
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

        [TestMethod]
        public void CardBlockingPayment()
        {
            var config = new GpEcomConfig();
            config.MerchantId = "heartlandgpsandbox";
            config.AccountId = "hpp";
            config.SharedSecret = "secret";
            config.ServiceUrl = "https://pay.sandbox.realexpayments.com/pay";

            config.HostedPaymentConfig = new HostedPaymentConfig();
            config.HostedPaymentConfig.Version = HppVersion.VERSION_1;

            var client = new RealexHppClient(config.ServiceUrl, config.SharedSecret);
            var service = new HostedService(config);

            var hostedPaymentData = new HostedPaymentData();
            hostedPaymentData.CustomerCountry = "DE";
            hostedPaymentData.CustomerFirstName = "James";
            hostedPaymentData.CustomerLastName = "Mason";
            hostedPaymentData.MerchantResponseUrl = "https://www.example.com/returnUrl";
            hostedPaymentData.TransactionStatusUrl = "https://www.example.com/statusUrl";
            var blockCardTypes = new BlockCardType[]{ BlockCardType.COMMERCIAL_CREDIT, BlockCardType.COMMERCIAL_DEBIT};
            hostedPaymentData.BlockCardTypes = blockCardTypes;

            var blockCardTypesToValidate = string.Join("|", new[] { EnumConverter.GetDescription(BlockCardType.COMMERCIAL_CREDIT), EnumConverter.GetDescription(BlockCardType.COMMERCIAL_DEBIT) });

            var json = service.Charge(10.01m)
                .WithCurrency("EUR")
                .WithHostedPaymentData(hostedPaymentData)
                .Serialize();           
        
            var response = client.SendRequest(json);
            var parsedResponse = service.ParseResponse(response, true);
            Assert.AreEqual(blockCardTypesToValidate, parsedResponse.ResponseValues["BLOCK_CARD_TYPE"]);

            Assert.AreEqual("00", parsedResponse.ResponseCode);
        }

        [TestMethod]
        public void CardBlockingPayment_AllCardTypes()
        {
            var config = new GpEcomConfig();
            config.MerchantId = "heartlandgpsandbox";
            config.AccountId = "hpp";
            config.SharedSecret = "secret";
            config.ServiceUrl = "https://pay.sandbox.realexpayments.com/pay";

            config.HostedPaymentConfig = new HostedPaymentConfig();
            config.HostedPaymentConfig.Version = HppVersion.VERSION_1;

            var client = new RealexHppClient(config.ServiceUrl, config.SharedSecret);
            var service = new HostedService(config);

            var hostedPaymentData = new HostedPaymentData();
            hostedPaymentData.CustomerCountry = "DE";
            hostedPaymentData.CustomerFirstName = "James";
            hostedPaymentData.CustomerLastName = "Mason";
            hostedPaymentData.MerchantResponseUrl = "https://www.example.com/returnUrl";
            hostedPaymentData.TransactionStatusUrl = "https://www.example.com/statusUrl";

            var blockCardTypes = new BlockCardType[] { BlockCardType.CONSUMER_CREDIT, BlockCardType.CONSUMER_DEBIT, BlockCardType.COMMERCIAL_CREDIT, BlockCardType.COMMERCIAL_DEBIT };
            hostedPaymentData.BlockCardTypes = blockCardTypes;

            var blockCardTypesToValidate = string.Join("|", new[] { EnumConverter.GetDescription(BlockCardType.CONSUMER_CREDIT), EnumConverter.GetDescription(BlockCardType.CONSUMER_DEBIT),
                EnumConverter.GetDescription(BlockCardType.COMMERCIAL_CREDIT), EnumConverter.GetDescription(BlockCardType.COMMERCIAL_DEBIT) });


            var json = service.Charge(10.01m)
                .WithCurrency("EUR")
                .WithHostedPaymentData(hostedPaymentData)
                .Serialize();   

            var exceptionCaught = false;
            try
            {
                var response = client.SendRequest(json);
            }
            catch (Exception e) {
                exceptionCaught = true;
                Assert.AreEqual("Unexpected Gateway Response: 561 - All card types are blocked, invalid request", e.Message);
            } finally
            {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void ThreeDSExemption() {

            var service = new HostedService(new GpEcomConfig
            {
                MerchantId = "heartlandgpsandbox",
                AccountId = "3dsecure",
                SharedSecret = "secret",

                HostedPaymentConfig = new HostedPaymentConfig
                {
                    Language = "GB",
                    ResponseUrl = "https://www.example.com/response",
                    Version = "2"                   
                },
            });

            // data to be passed to the HPP along with transaction level settings
            var hostedPaymentData = new HostedPaymentData();
            hostedPaymentData.EnableExemptionOptimization = true;
            hostedPaymentData.ChallengeRequestIndicator = ChallengeRequestIndicator.NO_CHALLENGE_REQUESTED;

            var shippingAddress = new Address
            {
                StreetAddress1 = "Apartment 852",
                StreetAddress2 = "Complex 741",
                StreetAddress3 = "no",
                City = "Chicago",
                PostalCode = "5001",
                State = "IL",
                Country = "840",
            };

            var billingAddress = new Address
            {
                StreetAddress1 = "Flat 123",
                StreetAddress2 = "House 456",
                StreetAddress3 = "Cul-De-Sac",
                City = "Halifax",
                Province = "West Yorkshire",
                State = "Yorkshire and the Humber",
                Country = "826",
                PostalCode = "E77 4QJ"
            };

            //serialize the request
            var json = service.Charge((decimal)10.01)
            .WithCurrency("EUR")
            .WithAddress(billingAddress, AddressType.Billing)
            .WithAddress(shippingAddress, AddressType.Shipping)
            .WithHostedPaymentData(hostedPaymentData)
            .Serialize();
        
            Assert.IsNotNull(json);
        
            Assert.IsTrue(json.Contains("\"HPP_ENABLE_EXEMPTION_OPTIMIZATION\":true"));       
        }
    }
}