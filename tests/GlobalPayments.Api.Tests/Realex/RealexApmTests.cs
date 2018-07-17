using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests {
    [TestClass]
    public class RealexApmTests {

        [TestInitialize]
        public void Init() {
            ServicesContainer.ConfigureService(new GatewayConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "hpp",
                SharedSecret = "secret",
                RebatePassword = "rebate",
                RefundPassword = "refund",
                ServiceUrl = "https://api.sandbox.realexpayments.com/epage-remote.cgi"
            });
        }

        [TestMethod]
        public void TestAlternativePaymentMethodForCharge() {
                var PaymentMethodDetails = new AlternatePaymentMethod {
                    AlternativePaymentMethodType = AlternativePaymentType.TESTPAY,
                    ReturnUrl = "https://www.example.com/returnUrl",
                    StatusUpdateUrl = "https://www.example.com/statusUrl",
                    Descriptor = "Test Transaction",
                    Country = "DE",
                    AccountHolderName = "James Mason"
                };

                var response = PaymentMethodDetails.Charge(15m)
                     .WithCurrency("EUR")
                     .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("01", response.ResponseCode, response.ResponseMessage);  
        }

        [TestMethod]
        public void TestAlternativePaymentMethodForRefund() {
            // a refund request requires the original order id
            string orderId = "20180614113445-5b2252d5aa6f9";
            // and the payments reference (pasref) from the authorization response
            string paymentsReference = "15289760863411679";
            // create the refund transaction object
            Transaction transaction = Transaction.FromId(paymentsReference, orderId);

            // send the refund request for payment-credit an APM, we must specify the amount,currency and alternative payment method
            var response = transaction.Refund(10m)
                        .WithCurrency("EUR")
                        .WithAlternativePaymentType(AlternativePaymentType.TESTPAY) // set the APM method 
                        .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }
    }
}
