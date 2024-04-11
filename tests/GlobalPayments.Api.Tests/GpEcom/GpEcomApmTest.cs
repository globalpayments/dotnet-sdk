using System;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpEcom
{
    [TestClass]
    public class GpEcomApmTest
    {
        [TestInitialize]
        public void Init()
        {
            ServicesContainer.ConfigureService(new GpEcomConfig
            {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                //Channel = "ECOM",
                RebatePassword = "rebate",
                RefundPassword = "refund",
                RequestLogger = new RequestConsoleLogger()
            });
        }

        [TestMethod]
        public void APMForCharge()
        {
            var PaymentMethodDetails = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.SOFORT,
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
        public void TestAlternativePaymentMethodForCharge_withAlternativePaymentMethodResponse()
        {
            var PaymentMethodDetails = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.TESTPAY,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                Descriptor = "Test Transaction",
                Country = "DE",
                AccountHolderName = "James Mason"
            };

            var response = PaymentMethodDetails.Charge(10m)
                .WithCurrency("EUR")
                .WithDescription("New APM")
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsNotNull(response?.AlternativePaymentResponse.AccountHolderName);
            Assert.IsNotNull(response?.AlternativePaymentResponse.Country);
            Assert.IsNotNull(response?.AlternativePaymentResponse.PaymentPurpose);
            Assert.IsNotNull(response?.AlternativePaymentResponse.RedirectUrl);
            Assert.AreEqual("01", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void APMWithoutAmount()
        {
            var PaymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.SOFORTUBERWEISUNG,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                Descriptor = "Test Transaction",
                Country = "DE",
                AccountHolderName = "James Mason"
            };
            try
            {
                PaymentMethod.Charge()
                    .WithCurrency("EUR")
                    .WithDescription("New APM")
                    .Execute();
            }
            catch (BuilderException ex)
            {
                Assert.AreEqual("Amount cannot be null for this transaction type.", ex.Message);
            }
        }

        [TestMethod]
        public void APMWithoutCurrency()
        {
            var PaymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.SOFORTUBERWEISUNG,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                Descriptor = "Test Transaction",
                Country = "DE",
                AccountHolderName = "James Mason"
            };
            try
            {
                PaymentMethod.Charge(10m)
                    .WithDescription("New APM")
                    .Execute();
            }
            catch (BuilderException ex)
            {
                Assert.AreEqual("Currency cannot be null for this transaction type.", ex.Message);
            }
        }

        [TestMethod]
        public void APMWithoutReturnUrl()
        {
            var PaymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.SOFORTUBERWEISUNG,
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                Descriptor = "Test Transaction",
                Country = "DE",
                AccountHolderName = "James Mason"
            };
            try
            {
                PaymentMethod.Charge(10m)
                    .WithCurrency("EUR")
                    .WithDescription("New APM")
                    .Execute();
            }
            catch (BuilderException ex)
            {
                Assert.AreEqual("ReturnUrl cannot be null for this transaction type.", ex.Message);
            }
        }

        [TestMethod]
        public void APMWithoutStatusUpdateUrl()
        {
            var PaymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.SOFORTUBERWEISUNG,
                ReturnUrl = "https://www.example.com/returnUrl",
                Descriptor = "Test Transaction",
                Country = "DE",
                AccountHolderName = "James Mason"
            };
            try
            {
                PaymentMethod.Charge(10m)
                    .WithCurrency("EUR")
                    .WithDescription("New APM")
                    .Execute();
            }
            catch (BuilderException ex)
            {
                Assert.AreEqual("StatusUpdateUrl cannot be null for this transaction type.", ex.Message);
            }
        }

        [TestMethod]
        public void ApmWithoutDescriptor()
        {
            var PaymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.SOFORTUBERWEISUNG,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/returnUrl",
                Country = "DE",
                AccountHolderName = "James Mason"
            };
            try
            {
                PaymentMethod.Charge(10m)
                    .WithCurrency("EUR")
                    .WithDescription("New APM")
                    .Execute();
            }
            catch (BuilderException ex)
            {
                Assert.AreEqual(
                    "PaymentMethod, ReturnUrl, StatusUpdateUrl, AccountHolderName, Country, Descriptor can not be null ",
                    ex.Message);
            }
        }

        [TestMethod]
        public void ApmWithoutAccountHolderName()
        {
            var PaymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.SOFORTUBERWEISUNG,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/returnUrl",
                Descriptor = "Test Transaction",
                Country = "DE",
            };
            try
            {
                PaymentMethod.Charge(10m)
                    .WithCurrency("EUR")
                    .WithDescription("New APM")
                    .Execute();
            }
            catch (BuilderException ex)
            {
                Assert.AreEqual("AccountHolderName cannot be null for this transaction type.", ex.Message);
            }
        }

        [TestMethod]
        public void ApmWithoutCountry()
        {
            var PaymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.SOFORTUBERWEISUNG,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/returnUrl",
                Descriptor = "Test Transaction",
                AccountHolderName = "James Mason"
            };
            try
            {
                PaymentMethod.Charge(10m)
                    .WithCurrency("EUR")
                    .WithDescription("New APM")
                    .Execute();
            }
            catch (BuilderException ex)
            {
                Assert.AreEqual(
                    "PaymentMethod, ReturnUrl, StatusUpdateUrl, AccountHolderName, Country, Descriptor can not be null ",
                    ex.Message);
            }
        }

        [TestMethod]
        public void Apm_AlternativePaymentMethodType_NotAllow()
        {
            var PaymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.SOFORTUBERWEISUNG,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/returnUrl",
                Descriptor = "Test Transaction",
                Country = "DE",
                AccountHolderName = "James Mason"
            };
            try
            {
                PaymentMethod.Charge(10m)
                    .WithCurrency("EUR")
                    .WithDescription("New APM")
                    .Execute();
            }
            catch (GatewayException ex)
            {
                Assert.AreEqual(
                    "Unexpected Gateway Response: 503 - Payment method type [SOFORTUBERWEISUNG] is not allowed",
                    ex.Message);
                Assert.AreEqual("503", ex.ResponseCode);
            }
        }

        [TestMethod]
        public void APMRefundPendingTransaction()
        {
            var PaymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.TESTPAY,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                Descriptor = "Test Transaction",
                Country = "DE",
                AccountHolderName = "James Mason"
            };

            try
            {
                var response = PaymentMethod.Charge(10m)
                    .WithCurrency("EUR")
                    .WithDescription("New APM")
                    .Execute();

                Assert.IsNotNull(response);
                Assert.AreEqual("01", response.ResponseCode);

                var refund = response.Refund(10m)
                    .WithCurrency("EUR")
                    .WithAlternativePaymentType(AlternativePaymentType.TESTPAY)
                    .Execute();
            }
            catch (GatewayException ex)
            {
                Assert.AreEqual("FAILED", ex.ResponseMessage);
            }
        }

        [TestMethod]
        public void APMPayByBankApp()
        {
            var paymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.PAYBYBANKAPP,
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                Descriptor = "Test Transaction",
                Country = "GB",
                AccountHolderName = "James Mason"
            };

            var response = paymentMethod.Charge(10m)
                .WithCurrency("GBP")
                .WithDescription("New APM")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("01", response.ResponseCode);
        }

        [TestMethod]
        public void APMPayPal()
        {
            var paymentMethod = new AlternativePaymentMethod
            {
                AlternativePaymentMethodType = AlternativePaymentType.PAYPAL,
                ReturnUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net",
                StatusUpdateUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net",
                CancelUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net",
                Descriptor = "Test Transaction",
                Country = "US",
                AccountHolderName = "James Mason"
            };

            const decimal amount = 10m;
            const string currency = "USD";

            var transaction = paymentMethod.Charge(amount)
                .WithCurrency(currency)
                .WithDescription("New APM")
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual("00", transaction.ResponseCode);
            Assert.IsNotNull(transaction.AlternativePaymentResponse.SessionToken);

            Console.WriteLine("https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_express-checkout&token=" +
                              transaction.AlternativePaymentResponse.SessionToken);

            Thread.Sleep(30000);

            transaction.AlternativePaymentResponse.ProviderReference = "SMKGK7K2BLEUA";

            try
            {
                var response = transaction.Confirm(amount)
                    .WithCurrency(currency)
                    .WithAlternativePaymentType(AlternativePaymentType.PAYPAL)
                    .Execute();

                Assert.IsNotNull(response);
                Assert.AreEqual("00", response.ResponseCode);
            }
            catch (GatewayException e)
            {
                Assert.AreEqual("Unexpected Gateway Response: 101 - Payment has not been authorized by the user.",
                    e.Message);
            }
        }

        [TestMethod]
        public void APMForRefund()
        {
            // a refund request requires the original order id
            string orderId = "20180614113445-5b2252d5aa6f9";
            // and the payments reference (pasref) from the authorization response
            string paymentsReference = "15289760863411679";
            // create the refund transaction object
            Transaction transaction = Transaction.FromId(paymentsReference, orderId);

            try
            {
                var response = transaction.Refund(10m)
                    .WithCurrency("EUR")
                    .WithAlternativePaymentType(AlternativePaymentType.TESTPAY) // set the APM method 
                    .Execute();

                Assert.IsNotNull(response);
                Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            }
            catch (GatewayException ex)
            {
                Assert.AreEqual("FAILED", ex.ResponseMessage);
            }
            // send the refund request for payment-credit an APM, we must specify the amount,currency and alternative payment method
        }
    }
}