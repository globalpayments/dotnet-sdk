using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.GpEcom.Hpp;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpEcom {
    [TestClass]
    public class GpEcomHppResponseTest {
        HostedService _service;
        RealexHppClient _client;

        public GpEcomHppResponseTest() {
            _client = new RealexHppClient("https://pay.sandbox.realexpayments.com/pay", "secret");
            _service = new HostedService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "hpp",
                SharedSecret = "secret",
                HostedPaymentConfig = new HostedPaymentConfig {
                    Language = "GB",
                    ResponseUrl = "http://requestb.in/10q2bjb1"
                }
            });
        }

        [TestMethod]
        public void BasicResponse() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
            });

            var responseJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"843680654f377bfa845387fdbace35acc9d95778\", \"RESULT\": \"00\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"AUTHCODE\": \"12345\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"AVSADDRESSRESULT\": \"M\", \"AVSPOSTCODERESULT\": \"M\", \"BATCHID\": \"445196\", \"MESSAGE\": \"[ test system ] Authorised\", \"PASREF\": \"15011597872195765\", \"CVNRESULT\": \"M\"}";
            var response = service.ParseResponse(responseJson, false);

            Assert.AreEqual("12345", response.AuthorizationCode);
            Assert.AreEqual(1999, response.AuthorizedAmount.Value);
            Assert.AreEqual("M", response.AvsResponseCode);
            Assert.AreEqual("GTI5Yxb0SumL_TkDMCAxQA", response.OrderId);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("[ test system ] Authorised", response.ResponseMessage);
            Assert.AreEqual("15011597872195765", response.TransactionId);
            Assert.AreEqual("M", response.CvnResponseCode);
        }

        [TestMethod]
        public void StandardResponse() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
            });

            var responseJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"843680654f377bfa845387fdbace35acc9d95778\", \"RESULT\": \"00\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"AUTHCODE\": \"12345\", \"SHIPPING_CODE\": \"654|123\", \"SHIPPING_CO\": \"GB\", \"BILLING_CODE\": \"50001\", \"BILLING_CO\": \"US\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"AVSADDRESSRESULT\": \"M\", \"AVSPOSTCODERESULT\": \"M\", \"BATCHID\": \"445196\", \"DCC_ENABLE\": \"1\", \"HPP_FRAUDFILTER_MODE\": \"PASSIVE\", \"HPP_LANG\": \"EN\", \"MESSAGE\": \"[ test system ] Authorised\", \"PASREF\": \"15011597872195765\", \"CVNRESULT\": \"M\", \"HPP_FRAUDFILTER_RESULT\": \"PASS\", \"COMMENT1\": \"Mobile Channel\", \"COMMENT2\": \"Down Payment\", \"ECI\": \"5\", \"XID\": \"vJ9NXpFueXsAqeb4iAbJJbe+66s=\", \"CAVV\": \"AAACBUGDZYYYIgGFGYNlAAAAAAA=\", \"CARDDIGITS\": \"424242xxxx4242\", \"CARDTYPE\": \"VISA\", \"EXPDATE\": \"1025\", \"CHNAME\": \"James Mason\"}";
            var response = service.ParseResponse(responseJson, false);

            Assert.AreEqual("12345", response.AuthorizationCode);
            Assert.AreEqual(1999, response.AuthorizedAmount.Value);
            Assert.AreEqual("M", response.AvsResponseCode);
            Assert.AreEqual("GTI5Yxb0SumL_TkDMCAxQA", response.OrderId);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("[ test system ] Authorised", response.ResponseMessage);
            Assert.AreEqual("15011597872195765", response.TransactionId);
            Assert.AreEqual("M", response.CvnResponseCode);
        }

        [TestMethod]
        public void ParseResponseForFinalStatus()
        {
            var service = new HostedService(new GpEcomConfig
            {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
            });

            var responseJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"PAYMENTMETHOD\": \"testpay\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"f91ddc74d0abb0ff8743b9fa8ac988a19aca2b40\", \"RESULT\": \"00\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"AUTHCODE\": \"12345\", \"SHIPPING_CODE\": \"654|123\", \"SHIPPING_CO\": \"GB\", \"BILLING_CODE\": \"50001\", \"BILLING_CO\": \"US\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"AVSADDRESSRESULT\": \"M\", \"AVSPOSTCODERESULT\": \"M\", \"BATCHID\": \"445196\", \"DCC_ENABLE\": \"1\", \"HPP_FRAUDFILTER_MODE\": \"PASSIVE\", \"HPP_LANG\": \"EN\", \"MESSAGE\": \"[ test system ] Authorised\", \"PASREF\": \"15011597872195765\", \"CVNRESULT\": \"M\", \"HPP_FRAUDFILTER_RESULT\": \"PASS\", \"COMMENT1\": \"Mobile Channel\", \"COMMENT2\": \"Down Payment\", \"ECI\": \"5\", \"XID\": \"vJ9NXpFueXsAqeb4iAbJJbe+66s=\", \"CAVV\": \"AAACBUGDZYYYIgGFGYNlAAAAAAA=\", \"CARDDIGITS\": \"424242xxxx4242\", \"CARDTYPE\": \"VISA\", \"EXPDATE\": \"1025\", \"CHNAME\": \"James Mason\"}";
            var response = service.ParseResponse(responseJson, false);

            Assert.AreEqual("12345", response.AuthorizationCode);
            Assert.AreEqual(1999, response.AuthorizedAmount.Value);
            Assert.AreEqual("M", response.AvsResponseCode);
            Assert.AreEqual("GTI5Yxb0SumL_TkDMCAxQA", response.OrderId);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("[ test system ] Authorised", response.ResponseMessage);
            Assert.AreEqual("15011597872195765", response.TransactionId);
            Assert.AreEqual("M", response.CvnResponseCode);
        }

        [TestMethod, ExpectedException(typeof(ApiException))]
        public void IncorrectHashAuthCode() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
            });

            var responseJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"841680654f377bfa845387fdbace35acc9d95778\", \"RESULT\": \"00\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"AUTHCODE\": \"54321\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"AVSADDRESSRESULT\": \"M\", \"AVSPOSTCODERESULT\": \"M\", \"BATCHID\": \"445196\", \"MESSAGE\": \"[ test system ] Authorised\", \"PASREF\": \"15011597872195765\", \"CVNRESULT\": \"M\"}";
            var response = service.ParseResponse(responseJson, false);
        }

        [TestMethod, ExpectedException(typeof(ApiException))]
        public void IncorrectSharedSecret() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "SharedSecret",
            });

            var responseJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"841680654f377bfa845387fdbace35acc9d95778\", \"RESULT\": \"00\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"AUTHCODE\": \"12345\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"AVSADDRESSRESULT\": \"M\", \"AVSPOSTCODERESULT\": \"M\", \"BATCHID\": \"445196\", \"MESSAGE\": \"[ test system ] Authorised\", \"PASREF\": \"15011597872195765\", \"CVNRESULT\": \"M\"}";
            var response = service.ParseResponse(responseJson, false);
        }

        [TestMethod]
        public void FraudCheckBlock() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
            });

            var responseJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"7a2fff26deac60f470b5de22c63151f530a22805\", \"RESULT\": \"107\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"AUTHCODE\": \"\", \"SHIPPING_CODE\": \"654|123\", \"SHIPPING_CO\": \"GB\", \"BILLING_CODE\": \"50001\", \"BILLING_CO\": \"US\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"AVSADDRESSRESULT\": \"U\", \"AVSPOSTCODERESULT\": \"U\", \"BATCHID\": \"-1\", \"MESSAGE\": \"Fails Fraud Checks\", \"PASREF\": \"15016893697197771\", \"CVNRESULT\": \"U\", \"HPP_FRAUDFILTER_RESULT\": \"BLOCK\", \"HPP_FRAUDFILTER_RULE_56257838-4590-4227-b946-11e061fb15fe\": \"BLOCK\", \"HPP_FRAUDFILTER_RULE_NAME\": \"Cardholder Name Check\"}";
            var response = service.ParseResponse(responseJson, false);

            Assert.AreEqual("107", response.ResponseCode);
            Assert.AreEqual("Fails Fraud Checks", response.ResponseMessage);
            Assert.AreEqual("", response.AuthorizationCode);
            Assert.AreEqual(1999, response.AuthorizedAmount.Value);
            Assert.AreEqual("U", response.AvsResponseCode);
            Assert.AreEqual("GTI5Yxb0SumL_TkDMCAxQA", response.OrderId);
            Assert.AreEqual("15016893697197771", response.TransactionId);
            Assert.AreEqual("U", response.CvnResponseCode);
        }

        [TestMethod]
        public void DeclinedTransaction() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
            });

            var responseJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"41a73ae4f563c60a0da840af36f078fde1beb4e0\", \"RESULT\": \"101\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"AUTHCODE\": \"\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"AVSADDRESSRESULT\": \"M\", \"AVSPOSTCODERESULT\": \"M\", \"BATCHID\": \"-1\", \"MESSAGE\": \"[ test system ] DECLINED\", \"PASREF\": \"15016900517792053\", \"CVNRESULT\": \"N\"}";
            var response = service.ParseResponse(responseJson, false);

            Assert.AreEqual("101", response.ResponseCode);
            Assert.AreEqual("[ test system ] DECLINED", response.ResponseMessage);
            Assert.AreEqual("", response.AuthorizationCode);
            Assert.AreEqual(1999, response.AuthorizedAmount.Value);
            Assert.AreEqual("M", response.AvsResponseCode);
            Assert.AreEqual("GTI5Yxb0SumL_TkDMCAxQA", response.OrderId);
            Assert.AreEqual("15016900517792053", response.TransactionId);
            Assert.AreEqual("N", response.CvnResponseCode);
        }

        [TestMethod]
        public void ReferralBTransaction() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
            });

            var responseJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"1de7ac3d4128719fe1d7a9217b9a1cce02e2b1c9\", \"RESULT\": \"102\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"AUTHCODE\": \"\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"AVSADDRESSRESULT\": \"M\", \"AVSPOSTCODERESULT\": \"M\", \"BATCHID\": \"-1\", \"MESSAGE\": \"[ test system ] REFERRAL B\", \"PASREF\": \"15017567624469248\", \"CVNRESULT\": \"M\"}";
            var response = service.ParseResponse(responseJson, false);

            Assert.AreEqual("102", response.ResponseCode);
            Assert.AreEqual("[ test system ] REFERRAL B", response.ResponseMessage);
            Assert.AreEqual("", response.AuthorizationCode);
            Assert.AreEqual(1999, response.AuthorizedAmount.Value);
            Assert.AreEqual("GTI5Yxb0SumL_TkDMCAxQA", response.OrderId);
            Assert.AreEqual("15017567624469248", response.TransactionId);
        }

        [TestMethod]
        public void ReferralATransaction() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
            });

            var responseJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"f6ef9e8d2c463ae94e07009954dc83527125bb7e\", \"RESULT\": \"103\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"AUTHCODE\": \"\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"AVSADDRESSRESULT\": \"M\", \"AVSPOSTCODERESULT\": \"M\", \"BATCHID\": \"-1\", \"MESSAGE\": \"[ test system ] REFERRAL A\", \"PASREF\": \"15017567624469248\", \"CVNRESULT\": \"M\"}";
            var response = service.ParseResponse(responseJson, false);

            Assert.AreEqual("103", response.ResponseCode);
            Assert.AreEqual("[ test system ] REFERRAL A", response.ResponseMessage);
            Assert.AreEqual("", response.AuthorizationCode);
            Assert.AreEqual(1999, response.AuthorizedAmount.Value);
            Assert.AreEqual("GTI5Yxb0SumL_TkDMCAxQA", response.OrderId);
            Assert.AreEqual("15017567624469248", response.TransactionId);
        }

        [TestMethod, ExpectedException(typeof(ApiException))]
        public void IncorrectResponseHash() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
            });

            var responseJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"841680654f377bfa845387fdbace35acc9d95778\", \"RESULT\": \"00\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"AUTHCODE\": \"12345\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"AVSADDRESSRESULT\": \"M\", \"AVSPOSTCODERESULT\": \"M\", \"BATCHID\": \"445196\", \"MESSAGE\": \"[ test system ] Authorised\", \"PASREF\": \"15011597872195765\", \"CVNRESULT\": \"M\"}";
            var response = service.ParseResponse(responseJson, false);
        }

        [TestMethod, ExpectedException(typeof(ApiException))]
        public void IncorrectResultCode() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
            });

            var responseJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"841680654f377bfa845387fdbace35acc9d95778\", \"RESULT\": \"101\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"AUTHCODE\": \"54321\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"AVSADDRESSRESULT\": \"M\", \"AVSPOSTCODERESULT\": \"M\", \"BATCHID\": \"445196\", \"MESSAGE\": \"[ test system ] Authorised\", \"PASREF\": \"15011597872195765\", \"CVNRESULT\": \"M\"}";
            var response = service.ParseResponse(responseJson, false);
        }

        [TestMethod, ExpectedException(typeof(ApiException))]
        public void IncorrectMerchantId() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
            });

            var responseJson = "{ \"MERCHANT_ID\": \"Merchant Id\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"841680654f377bfa845387fdbace35acc9d95778\", \"RESULT\": \"00\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"AUTHCODE\": \"54321\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"AVSADDRESSRESULT\": \"M\", \"AVSPOSTCODERESULT\": \"M\", \"BATCHID\": \"445196\", \"MESSAGE\": \"[ test system ] Authorised\", \"PASREF\": \"15011597872195765\", \"CVNRESULT\": \"M\"}";
            var response = service.ParseResponse(responseJson, false);
        }

        [TestMethod, ExpectedException(typeof(ApiException))]
        public void IncorrectTimestamp() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
            });

            var responseJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"TIMESTAMP\": \"20170803151925\", \"SHA1HASH\": \"841680654f377bfa845387fdbace35acc9d95778\", \"RESULT\": \"00\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"AUTHCODE\": \"54321\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"AVSADDRESSRESULT\": \"M\", \"AVSPOSTCODERESULT\": \"M\", \"BATCHID\": \"445196\", \"MESSAGE\": \"[ test system ] Authorised\", \"PASREF\": \"15011597872195765\", \"CVNRESULT\": \"M\"}";
            var response = service.ParseResponse(responseJson, false);
        }

        [TestMethod, ExpectedException(typeof(ApiException))]
        public void IncorrectOrderId() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
            });

            var responseJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"N6qsk4kYRZihmPrTXWYS6g\", \"AMOUNT\": \"1999\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"841680654f377bfa845387fdbace35acc9d95778\", \"RESULT\": \"00\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"AUTHCODE\": \"54321\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"AVSADDRESSRESULT\": \"M\", \"AVSPOSTCODERESULT\": \"M\", \"BATCHID\": \"445196\", \"MESSAGE\": \"[ test system ] Authorised\", \"PASREF\": \"15011597872195765\", \"CVNRESULT\": \"M\"}";
            var response = service.ParseResponse(responseJson, false);
        }

        [TestMethod, ExpectedException(typeof(ApiException))]
        public void IncorrectMessage() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
            });

            var responseJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"841680654f377bfa845387fdbace35acc9d95778\", \"RESULT\": \"00\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"AUTHCODE\": \"54321\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"AVSADDRESSRESULT\": \"M\", \"AVSPOSTCODERESULT\": \"M\", \"BATCHID\": \"445196\", \"MESSAGE\": \"[ test system ] DECLINED\", \"PASREF\": \"15011597872195765\", \"CVNRESULT\": \"M\"}";
            var response = service.ParseResponse(responseJson, false);
        }

        [TestMethod, ExpectedException(typeof(ApiException))]
        public void IncorrectPasref() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
            });

            var responseJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"841680654f377bfa845387fdbace35acc9d95778\", \"RESULT\": \"00\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"AUTHCODE\": \"54321\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"AVSADDRESSRESULT\": \"M\", \"AVSPOSTCODERESULT\": \"M\", \"BATCHID\": \"445196\", \"MESSAGE\": \"[ test system ] Authorised\", \"PASREF\": \"15011596872195765\", \"CVNRESULT\": \"M\"}";
            var response = service.ParseResponse(responseJson, false);
        }

        [TestMethod]
        public void BasicEncodedResponse() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
            });

            var responseJson = "{ \"MERCHANT_ID\": \"TWVyY2hhbnRJZA==\", \"ACCOUNT\": \"aW50ZXJuZXQ=\", \"ORDER_ID\": \"R1RJNVl4YjBTdW1MX1RrRE1DQXhRQQ==\", \"AMOUNT\": \"MTk5OQ==\", \"TIMESTAMP\": \"MjAxNzA3MjUxNTQ4MjQ=\", \"SHA1HASH\": \"ODQzNjgwNjU0ZjM3N2JmYTg0NTM4N2ZkYmFjZTM1YWNjOWQ5NTc3OA==\", \"RESULT\": \"MDA=\",  \"MERCHANT_RESPONSE_URL\": \"aHR0cHM6Ly93d3cuZXhhbXBsZS5jb20vcmVzcG9uc2U=\", \"AUTHCODE\": \"MTIzNDU=\", \"CARD_PAYMENT_BUTTON\": \"UGxhY2UgT3JkZXI=\", \"AVSADDRESSRESULT\": \"TQ==\", \"AVSPOSTCODERESULT\": \"TQ==\", \"BATCHID\": \"NDQ1MTk2\", \"MESSAGE\": \"WyB0ZXN0IHN5c3RlbSBdIEF1dGhvcmlzZWQ=\", \"PASREF\": \"MTUwMTE1OTc4NzIxOTU3NjU=\", \"CVNRESULT\": \"TQ==\"}";
            var response = service.ParseResponse(responseJson, true);

            Assert.AreEqual("12345", response.AuthorizationCode);
            Assert.AreEqual(1999, response.AuthorizedAmount.Value);
            Assert.AreEqual("M", response.AvsResponseCode);
            Assert.AreEqual("GTI5Yxb0SumL_TkDMCAxQA", response.OrderId);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("[ test system ] Authorised", response.ResponseMessage);
            Assert.AreEqual("15011597872195765", response.TransactionId);
            Assert.AreEqual("M", response.CvnResponseCode);
        }

        [TestMethod, ExpectedException(typeof(ApiException))]
        public void IncorrectEncodedResponse() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
            });

            var responseJson = "{ \"MERCHANT_ID\": \"TWVyY2hhbnRJZA==\", \"ACCOUNT\": \"aW50ZXJuZXQ=\", \"ORDER_ID\": \"R1RJNVl4YjBTdW1MX1RrRE1DQXhRQQ==\", \"AMOUNT\": \"MTk5OQ==\", \"TIMESTAMP\": \"MjAxNzA3MjUxNTQ4MjQ=\", \"SHA1HASH\": \"ODQzNjgwNjU0ZjM3N2JmYTg0NTM4N2ZkYmFjZTM1YWNjOWQ5NTc3OA==\", \"RESULT\": \"MDA=\",  \"MERCHANT_RESPONSE_URL\": \"aHR0cHM6Ly93d3cuZXhhbXBsZS5jb20vcmVzcG9uc2U=\", \"AUTHCODE\": \"MTIzNDU=\", \"CARD_PAYMENT_BUTTON\": \"UGxhY2UgT3JkZXI=\", \"AVSADDRESSRESULT\": \"TQ==\", \"AVSPOSTCODERESULT\": \"TQ==\", \"BATCHID\": \"NDQ1MTk2\", \"MESSAGE\": \"WyB0ZXN0IHN5c3RlbSBdIEF1dGhvcmlzZWQ=\", \"PASREF\": \"MTUwMTE1OTc4NzIxOTU3NjU=\", \"CVNRESULT\": \"TQ==\"}";
            var response = service.ParseResponse(responseJson, false);
        }

        [TestMethod, ExpectedException(typeof(ApiException))]
        public void IncorrectNonEncodedResponse() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
            });

            var responseJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"843680654f377bfa845387fdbace35acc9d95778\", \"RESULT\": \"00\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"AUTHCODE\": \"12345\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"AVSADDRESSRESULT\": \"M\", \"AVSPOSTCODERESULT\": \"M\", \"BATCHID\": \"445196\", \"MESSAGE\": \"[ test system ] Authorised\", \"PASREF\": \"15011597872195765\", \"CVNRESULT\": \"M\"}";
            var response = service.ParseResponse(responseJson, true);
        }

        [TestMethod]
        public void VerifyResponseValues() {
            var service = new HostedService(new GpEcomConfig {
                MerchantId = "MerchantId",
                AccountId = "internet",
                SharedSecret = "secret",
            });

            var responseJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"843680654f377bfa845387fdbace35acc9d95778\", \"RESULT\": \"00\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"AUTHCODE\": \"12345\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"AVSADDRESSRESULT\": \"M\", \"AVSPOSTCODERESULT\": \"M\", \"BATCHID\": \"445196\", \"MESSAGE\": \"[ test system ] Authorised\", \"PASREF\": \"15011597872195765\", \"CVNRESULT\": \"M\"}";
            var response = service.ParseResponse(responseJson, false);

            var doc = JsonDoc.Parse(responseJson);
            Assert.IsNotNull(response.ResponseValues);
            Assert.AreEqual(doc.Keys.Count, response.ResponseValues.Count);
            foreach (var key in doc.Keys) {
                var strValue = doc.GetValue<string>(key);
                Assert.AreEqual(strValue, response.ResponseValues[key]);
            }
        }
        [TestMethod]
        public void StandardResponseWithMultiAutoSettle() {
            string responseJson = "{ \"MERCHANT_ID\": \"MerchantId\", \"ACCOUNT\": \"internet\", \"ORDER_ID\": \"GTI5Yxb0SumL_TkDMCAxQA\", \"AMOUNT\": \"1999\", \"TIMESTAMP\": \"20170725154824\", \"SHA1HASH\": \"843680654f377bfa845387fdbace35acc9d95778\", \"RESULT\": \"00\",  \"MERCHANT_RESPONSE_URL\": \"https://www.example.com/response\", \"AUTHCODE\": \"12345\", \"SHIPPING_CODE\": \"654|123\", \"SHIPPING_CO\": \"GB\", \"BILLING_CODE\": \"50001\", \"BILLING_CO\": \"US\", \"CARD_PAYMENT_BUTTON\": \"Place Order\", \"AVSADDRESSRESULT\": \"M\", \"AVSPOSTCODERESULT\": \"M\", \"BATCHID\": \"445196\", \"DCC_ENABLE\": \"1\", \"HPP_FRAUDFILTER_MODE\": \"PASSIVE\", \"HPP_LANG\": \"EN\", \"MESSAGE\": \"[ test system ] Authorised\", \"PASREF\": \"15011597872195765\", \"CVNRESULT\": \"M\", \"HPP_FRAUDFILTER_RESULT\": \"PASS\", \"COMMENT1\": \"Mobile Channel\", \"COMMENT2\": \"Down Payment\", \"ECI\": \"5\", \"XID\": \"vJ9NXpFueXsAqeb4iAbJJbe+66s=\", \"CAVV\": \"AAACBUGDZYYYIgGFGYNlAAAAAAA=\", \"CARDDIGITS\": \"424242xxxx4242\", \"CARDTYPE\": \"VISA\", \"EXPDATE\": \"1025\", \"CHNAME\": \"James Mason\", \"AUTO_SETTLE_FLAG\": \"MULTI\"}";
            Transaction response = _service.ParseResponse(responseJson, false);

            Assert.AreEqual("12345", response.AuthorizationCode);
            Assert.AreEqual("MULTI", response.AutoSettleFlag);
            Assert.AreEqual(1999, response.AuthorizedAmount.Value);
            Assert.AreEqual("M", response.AvsResponseCode);
            Assert.AreEqual("GTI5Yxb0SumL_TkDMCAxQA", response.OrderId);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("[ test system ] Authorised", response.ResponseMessage);
            Assert.AreEqual("15011597872195765", response.TransactionId);
            Assert.AreEqual("M", response.CvnResponseCode);
        }
    }
}