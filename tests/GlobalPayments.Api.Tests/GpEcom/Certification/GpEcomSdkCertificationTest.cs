using System;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpEcom.Certification {
    [TestClass]
    public partial class GpEcomSdkCertificationTest {
        [TestCleanup]
        public void Throttle() { Thread.Sleep(1500); }

        protected static readonly int expYear = DateTime.Now.Year + 1;
        
        [TestMethod]
        public void DotNet_Auth_006a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-006a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_006b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-006b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_006c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-006c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_006d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-006d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_006e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-006e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_006f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-006f")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_006g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-006g")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_006h() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-006h")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_006i() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-006i")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_006j() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-006j")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_006k() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-006k")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_007a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-007a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_007b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-007b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_007c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-007c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_007d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-007d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_007e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-007e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_008a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-008a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_008b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-008b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_008c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-008c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_008d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-008d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_008e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-008e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_009a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-009a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_009b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "E"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-009b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_009c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOMMERCE"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-009c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_009d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-009d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_010a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-010a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_010b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-010b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_010c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-010c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_010d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-010d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_010e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-010e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_011a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-011a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_011b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-011b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_011c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-011c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Auth_011d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge()
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-011d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_012a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-012a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_012b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EURO")
                .WithDescription("DotNet_Auth-012b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_012c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("�UR")
                .WithDescription("DotNet_Auth-012c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Auth_012d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithDescription("DotNet_Auth-012d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_013a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-013a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_013b1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "424242000000000000000",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-013b1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_013b2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "42424242424",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-013b2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_013c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262#",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-013c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_014a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-014a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_014b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-014b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_014c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-014c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_014d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James~Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-014d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_015a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-015a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_015b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-015b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_015c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 20,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-015c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_015d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-015d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_016a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-016a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_016b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-016b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_016c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-016c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_017a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-017a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_018a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-018a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_019a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-019a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_019b1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-019b1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_019b2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-019b2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_019c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "12345",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-019c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_019d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "374101000000608",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-019d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_020a1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-020a1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_020a2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "",
                CvnPresenceIndicator = (CvnPresenceIndicator)2,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-020a2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_020a3() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "",
                CvnPresenceIndicator = (CvnPresenceIndicator)3,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-020a3")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_020a4() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "",
                CvnPresenceIndicator = (CvnPresenceIndicator)4,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-020a4")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_020b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)5,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-020b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_020c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)0,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-020c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_021a1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-021a1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_021a2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Authorize(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-021a2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_021a3() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Authorize(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-021a3")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_021b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Authorize(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-021b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_021c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Authorize(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-021c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_022a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-022a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_022b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-022b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_022c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-022c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_022d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-022d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_022e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-022e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_023a1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-023a1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_023a2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-023a2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_023b1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-023b1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_023c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-023c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_024a1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-024a1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_024a2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-024a2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_024a3() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-024a3")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_024b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-024b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_024c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-024c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_025() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-025")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_026a1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-026a1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_026a2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_026b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_026c1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIep3uviSnW9XEB3a4wpIW9XEB3a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_026c2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-026c2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_027a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithCustomerId("123456")
                .WithDescription("DotNet_Auth-027a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_028a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("123456")
                .WithDescription("DotNet_Auth-028a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_028b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-028b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_028c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithCustomerId("3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep33a4wpQQQQQQQQQ1")
                .WithDescription("DotNet_Auth-028c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_028d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("123456~")
                .WithDescription("DotNet_Auth-028d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_029a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithProductId("123456")
                .WithDescription("DotNet_Auth-029a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_029b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Auth-029b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_029c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithProductId("3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep33a4wpQQQQQQQQQ1")
                .WithDescription("DotNet_Auth-029c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_029d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithProductId("123456~")
                .WithDescription("DotNet_Auth-029d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_030a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithClientTransactionId("123456")
                .WithDescription("DotNet_Auth-030a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_030b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-030b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_030c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithClientTransactionId("3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep33a4wpQQQQQQQQQ1")
                .WithDescription("DotNet_Auth-030c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_030d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithClientTransactionId("123456~")
                .WithDescription("DotNet_Auth-030d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_031a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerIpAddress("123.123.123.123")
                .WithDescription("DotNet_Auth-031a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_031b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-031b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_031c1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithCustomerIpAddress("1234.123.123.123")
                .WithDescription("DotNet_Auth-031c1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_031c2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerIpAddress("123~.123.123.123")
                .WithDescription("DotNet_Auth-031c2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_032a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "E77 4QJ",
                Country = "United Kingdom"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "Z76 PO9",
                Country = "France"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Auth-032a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_033a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "774|10",
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "769|52",
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Auth-033a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_033b1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "774|10",
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithAddress(billingAddress)
                .WithDescription("DotNet_Auth-033b1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_033b2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "769|52",
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Auth-033b2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_033c1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwep4wpIwep3u111",
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithAddress(billingAddress)
                .WithDescription("DotNet_Auth-033c1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_033c2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwep4wpIwep3u111",
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Auth-033c2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_034a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // billing address
            var billingAddress = new Address {
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Auth-034a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_034b1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // shipping address
            var shippingAddress = new Address {
                Country = "GB"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Auth-034b1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_034b2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // billing address
            var billingAddress = new Address {
                Country = "GB"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithAddress(billingAddress)
                .WithDescription("DotNet_Auth-034b2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_034c1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // billing address
            var billingAddress = new Address {
                Country = "3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwep4wpIwep3u111"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithAddress(billingAddress)
                .WithDescription("DotNet_Auth-034c1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Auth_034c2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // shipping address
            var shippingAddress = new Address {
                Country = "3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwep4wpIwep3u111"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Auth-034c2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_035a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Auth-035a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_035b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Auth-035b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Auth_055a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "774|10",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "769|52",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithCustomerId("12345")
                .WithProductId("654321")
                .WithClientTransactionId("987654")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Auth-055a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_002a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-002a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_002b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-002b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_002c1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-002c1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_002c2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-002c2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_002d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "V002625938386848",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-002d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_002e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-002e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_002f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = " 4002 6259 3838 6848",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-002f")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_002g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-002g")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_002h() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-002h")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_003a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-003a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_003b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-003b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_003c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 20,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-003c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_003d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-003d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_003e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 11,
                ExpYear = 5,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-003e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_003f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-003f")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_003g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-003g")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_003h() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-003h")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_003i() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-003i")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_004a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-004a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_004b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-004b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_004c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "12345",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-004c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_004d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "374101000000608",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-004d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_004e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "374101000000608",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-004e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_004f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)0,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-004f")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_004g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)2,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-004g")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_004h() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)3,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-004h")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_004i() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)4,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-004i")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_005a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-005a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_005b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-005b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_005c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-005c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_005d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-005d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_005e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-005e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_005f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-005f")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_005g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-005g")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_005h() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-005h")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_006a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-006a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_006b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-006b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_006c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 11,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-006c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_006d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 11,
                ExpYear = 5,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-006d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_006e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-006e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_007a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "5425230000004415",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-007a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_007b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "5425230000004415",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-007b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_007d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "5425230000004415",
                ExpMonth = 11,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-007d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_007e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "5425230000004415",
                ExpMonth = 11,
                ExpYear = 5,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-007e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_007f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "5425230000004415",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-007f")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Validation_008b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "374101000000608",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-008b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_008c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "374101000000608",
                ExpMonth = 11,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-008c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_008d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "374101000000608",
                ExpMonth = 11,
                ExpYear = 5,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-008d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_009b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "30384800000000",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-009b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_009c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "30450100000000",
                ExpMonth = 11,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-009c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Validation_009d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "779|102",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "658|325",
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "30450100000000",
                ExpMonth = 11,
                ExpYear = 5,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Validation-009d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_AVS_001a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                StreetAddress1 = "Flat 123 House 456",
                PostalCode = "E77 4QJ",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "1",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_AVS-001a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_AVS_001b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                StreetAddress1 = "Flat #123 House No. 456",
                PostalCode = "E77 #4QJ",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "2",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_AVS-001b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_AVS_001c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                StreetAddress1 = "# Flat #123 House No. #456",
                PostalCode = "# E77 @~4 Q # J",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "3",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_AVS-001c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_AVS_001d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "4",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_AVS-001d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_AVS_001e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                StreetAddress1 = "Lorem ipsum dolor sit 1amet; consectetur adipiscing elit. Aenean ali2quam tellus in elit hendrerit; non 3porttE77 4QJitor lorem venenatis. Pellentesque dictum eu nunc ac fringilla. In vitae quam eu odio sollicitudin rhoncus. Praesent ullamcorper eros vitae consequat tempus. In gravida viverra iaculis. Morbi dignissim orci et ipsum accumsan",
                PostalCode = "Lorem ipsum dolo1r sit amet; consectetur adipiscing elit. Aenean aliquam tellus in elit hendrerit; non porttE77 4QJitor lorem venenatis. Pellentesque dictum eu2 nunc ac fringilla. In vitae quam eu 3odio sollicitudin rhoncus. Praesent ullamcorper eros vitae consequat tempus. In gravida viverra iaculis. Morbi dignissim orci et ipsum accumsan",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "5",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_AVS-001e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_AVS_001f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                StreetAddress1 = "ABCDEFGHIJ",
                PostalCode = "ABCDEFGHIJ",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "6",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_AVS-001f")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_AVS_001g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                StreetAddress1 = "Lorem ipsum dolor sit amet; consectetur adipiscing elit. Aenean aliquam tellus in elit hendrerit; non porttE77 4QJitor lorem venenatis. Pellentesque dictum eu nunc ac fringilla. In vitae quam eu odio sollicitudin rhoncus. Praesent ullamcorper eros vitae consequat tempus. In gravida viverra iaculis. Morbi dignissim orci et ipsum accumsan",
                PostalCode = "Lorem ipsum dolor sit amet; consectetur adipiscing elit. Aenean aliquam tellus in elit hendrerit; non porttE77 4QJitor lorem venenatis. Pellentesque dictum eu nunc ac fringilla. In vitae quam eu odio sollicitudin rhoncus. Praesent ullamcorper eros vitae consequat tempus. In gravida viverra iaculis. Morbi dignissim orci et ipsum accumsan",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "7",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_AVS-001g")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_AVS_003a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                StreetAddress1 = "Flat 123 House 456",
                PostalCode = "E77 4QJ",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "8",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_AVS-003a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_AVS_003b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                StreetAddress1 = "Flat 123 House 456",
                PostalCode = "E77 4QJ",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "9",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_AVS-003b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_AVS_003c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                StreetAddress1 = "Flat 123 House 456",
                PostalCode = "E77 4QJ",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "10",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_AVS-003c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_AVS_003d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                StreetAddress1 = "Flat 123 House 456",
                PostalCode = "E77 4QJ",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "11",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_AVS-003d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_AVS_003e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                StreetAddress1 = "Flat 123 House 456",
                PostalCode = "E77 4QJ",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "12",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_AVS-003e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_AVS_003f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                StreetAddress1 = "Flat 123 House 456",
                PostalCode = "E77 4QJ",
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "13",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_AVS-003f")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_AVS_003g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "14",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_AVS-003e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_AVS_003h() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // billing address
            var billingAddress = new Address {
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "15",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("100")
                .WithProductId("999")
                .WithClientTransactionId("test")
                .WithCustomerIpAddress("123.123.123.123")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_AVS-003f")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_Sample() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_006a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_006b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_006c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_006d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_006e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_006f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_006g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_006h() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_006i() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_006k() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_007a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_007b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_007c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_007d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_007e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_008a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_008b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_008c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_008d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_008e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_009a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_009b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Settle_009c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOm"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Settle_009d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECO#"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_010c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_010d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_010e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_011a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_011b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_011c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_011d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_012a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_012b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1.005m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1.005m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Settle_012c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture()
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Settle_012d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture()
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_012e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1000m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1000m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Settle_012f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture()
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_013a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Settle_013b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EURO")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EURO")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Settle_013c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EU#")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EU#")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_013d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_015a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_014a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_014b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_014c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_014d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_016a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_016b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_016c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_SettleAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_016d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle###")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Settle_017a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Settle")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Settle_017b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secreto",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Authorize(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Capture(1m)
                .WithCurrency("EUR")
                .WithDescription("SDK-DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_Sample() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_006a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_006b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_006c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_006d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_006e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_006f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_006g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_006h() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_006i() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_006k() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_007a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_007b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_007c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_007d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_007e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_008a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_008b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_008c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_008d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Void_009e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "EC"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_009a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_009b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Void_009c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOm"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Void_009d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECO#"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_010c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_010d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_010e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_011a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_011b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_011c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_011d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_012a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_012b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_012c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_012d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_013a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_014a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_014b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_014c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("akshdfgakjdshfgjdshgfkjdsahgfjshagdfjshdagfkjdshgfjshdgfjdshgfkjhdsagfjdsgfdskjgfdsjkhgfdsjhgfkdsjgfkjdshgfkjdsahgfjdskhgfjhdsgfjkhsdgfjkhsdgfjhsdgfjhsdgfkjhgsadjfhgsakjdhgfsajdhgfkjsadgfjhsadgfjkhdsgafjhdsgfjhdsgfjhdsgfkjhdgsafjkhgsfjhsdagfkjsgdafjhsgdfjhgdskjfgdsjfhgjdskhgfjhdsgfjhdsgfkjhgdsfkjhgsdkjfgsdkjhgfkjsahgdfkjgdsajfhgdsjkgfjdshgfjkdsagfjkhdsgfjsdhgfjkdshgfkjhgdsfkjhgdskjfgdskjgfkjdsahgfjhgdsakjfgdsafjhgdsjkhgfkjdshgfakjadshgfjhdsagfjhgdsfjhgsdakjfgdsakjhgfjsdhgfjhdsgfjhdsgfkjgdsajkfhgjdshgfjdsahgfjkhdsagfjhdsgfjkgdsfjhdsgfjhgdsjfhgdsjhfgjdshgfkjdsgfkjsadgfjkgdsfkjhgdsajfkhgdsjkgfkjdsagfkjgdsakjfhgdsfjkhgdsafkjgsadkjgfdkjsahgfkjsagfkjdshgfkjshdgfjgdsfkjgsadkjhgfdsjhgfkjdsagfjhdsgfjhgdsakjfgdsakjhgfjsdahgfjkgdsfjhgdsajkhfgjhdsagfkjhsgdakjf")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_014d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("SDK#####")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_015a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Void_015b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Void()
                .WithDescription("DotNet_Void")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_Sample() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_006a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_006b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_006c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_006d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_006e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_006f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_006g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_006h() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_007a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_007b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_007c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_007d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_007e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_008a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_008b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_008c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_008d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_008e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_009a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Rebate_009b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "EC"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Rebate_009c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOm"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Rebate_009d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECO#"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_010c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_010d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_010e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_011a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Rebate_011b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EURO")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EURO")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Rebate_011c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EU##")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EU##")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Rebate_011d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_012a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_012b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1.005m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1.005m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Rebate_012c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund()
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Rebate_012d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund()
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_012e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100000m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(100000m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Rebate_012f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund()
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_013a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_013b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_013c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_013d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_014a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_014b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_014c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_014d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_015a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_016a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_016b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_016c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("akshdfgakjdshfgjdshgfkjdsahgfjshagdfjshdagfkjdshgfjshdgfjdshgfkjhdsagfjdsgfdskjgfdsjkhgfdsjhgfkdsjgfkjdshgfkjdsahgfjdskhgfjhdsgfjkhsdgfjkhsdgfjhsdgfjhsdgfkjhgsadjfhgsakjdhgfsajdhgfkjsadgfjhsadgfjkhdsgafjhdsgfjhdsgfjhdsgfkjhdgsafjkhgsfjhsdagfkjsgdafjhsgdfjhgdskjfgdsjfhgjdskhgfjhdsgfjhdsgfkjhgdsfkjhgsdkjfgsdkjhgfkjsahgdfkjgdsajfhgdsjkgfjdshgfjkdsagfjkhdsgfjsdhgfjkdshgfkjhgdsfkjhgdskjfgdskjgfkjdsahgfjhgdsakjfgdsafjhgdsjkhgfkjdshgfakjadshgfjhdsagfjhgdsfjhgsdakjfgdsakjhgfjsdhgfjhdsgfjhdsgfkjgdsajkfhgjdshgfjdsahgfjkhdsagfjhdsgfjkgdsfjhdsgfjhgdsjfhgdsjhfgjdshgfkjdsgfkjsadgfjkgdsfkjhgdsajfkhgdsjkgfkjdsagfkjgdsakjfhgdsfjkhgdsafkjgsadkjgfdkjsahgfkjsagfkjdshgfkjshdgfjgdsfkjgsadkjhgfdsjhgfkjdsagfjhdsgfjhgdsakjfgdsakjhgfjsdahgfjkgdsfjhgdsajkhfgjhdsagfkjhsgdakjf")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_016d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("SDK#####")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Rebate_017a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Rebate_017b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secreto",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(1m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Rebate")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_Sample() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_006a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_006b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_006c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_006d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_006e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_006f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_006g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_006h() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_006i() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_006k() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Charge(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_006l() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_007a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_007b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_007c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_007d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_007e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_008a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_008b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_008c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_008d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_008e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_009a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_009b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "EC"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_009c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_009d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_010c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_010d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_010e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_012a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_012b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_012c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_012d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_012e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_013a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_OTB_013b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_OTB_013c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermeloooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_013d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_014a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_014b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_OTB_014c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 18,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_OTB_014d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_015a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_015b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_015c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_016a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_017a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_OTB_017b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_OTB_017c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "1.23457E+18",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_017d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "374101000000608",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_OTB_017f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "7",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_OTB_017g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "7",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_OTB_018a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_OTB_018b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secreto",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_OTB")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_Sample() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_006a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_006b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_006c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_006d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_006e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_006f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_006g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_006h() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_007a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_007b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_007c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_007d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_007e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_008a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_008b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_008c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_008d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_008e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_009a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_009b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "EC"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_009c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_009d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_010c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_010d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_010e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_011a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_012a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_012b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_012c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_013a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Credit_013b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Credit_013c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_014a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_014b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = 1813,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_014c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 18,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Credit_014d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_015a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_015b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_015c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_016a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_017a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_017b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_Credit_017c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123456789",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_017d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "374101000000608",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_Credit_017f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4242424242424240",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "7",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Credit_017g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "12#",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_018a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_018b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_018c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_018d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_019a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_019b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_019c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_020a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Credit_020b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secreto",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_021a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_021b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1.005m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Credit_021c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund()
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Credit_021d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund()
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_021e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(100000m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Credit_021f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund()
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Credit_022a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Credit_022b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EURO")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Credit_022c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithCurrency("EU#")
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Credit_022d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "Peter Watermelon"
            };

            // request
            var response = card.Refund(1m)
                .WithDescription("DotNet_Credit")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_Sample() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_006a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_006b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_006c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_006d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_006e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_006f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_006g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_006h() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_006i() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_006k() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_006l() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_007a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_007b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_007c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Hold_007d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
            });

            var saleResponse = Transaction.FromId(null);

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Hold_007e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
            });

            var saleResponse = Transaction.FromId(null);

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_008a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_008b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Hold_008c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
            });

            var saleResponse = Transaction.FromId(null);

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Hold_008d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
            });

            var saleResponse = Transaction.FromId(null);

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Hold_008e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
            });

            var saleResponse = Transaction.FromId(null);

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_009c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Hold_009d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            var saleResponse = Transaction.FromId(null);

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("SDK-DotNet_Query")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Hold_009e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            var saleResponse = Transaction.FromId(null);

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("SDK-DotNet_Query")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_010a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_010b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Hold_010c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            var saleResponse = Transaction.FromId(null);

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_010d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_011a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_011b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("PIUM"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_011c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_011d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("#####"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_012a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Hold_012b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secreto",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("SDK-DotNet_Query")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Hold_013a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Hold_013b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "EC"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("SDK-DotNet_Query")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Hold_013c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOOOOOOOOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("SDK-DotNet_Query")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_Sample() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // hold it first
            var holdResponse = saleResponse.Hold()
                .WithReasonCode(ReasonCode.OUTOFSTOCK)
                .Execute();
            Assert.IsNotNull(holdResponse);
            Assert.AreEqual("00", holdResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_006a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // hold it first
            var holdResponse = saleResponse.Hold()
                .WithReasonCode(ReasonCode.OUTOFSTOCK)
                .Execute();
            Assert.IsNotNull(holdResponse);
            Assert.AreEqual("00", holdResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_006b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // hold it first
            var holdResponse = saleResponse.Hold()
                .WithReasonCode(ReasonCode.OUTOFSTOCK)
                .Execute();
            Assert.IsNotNull(holdResponse);
            Assert.AreEqual("00", holdResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_006c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // hold it first
            var holdResponse = saleResponse.Hold()
                .WithReasonCode(ReasonCode.OUTOFSTOCK)
                .Execute();
            Assert.IsNotNull(holdResponse);
            Assert.AreEqual("00", holdResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_006d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // hold it first
            var holdResponse = saleResponse.Hold()
                .WithReasonCode(ReasonCode.OUTOFSTOCK)
                .Execute();
            Assert.IsNotNull(holdResponse);
            Assert.AreEqual("00", holdResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_006e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // hold it first
            var holdResponse = saleResponse.Hold()
                .WithReasonCode(ReasonCode.OUTOFSTOCK)
                .Execute();
            Assert.IsNotNull(holdResponse);
            Assert.AreEqual("00", holdResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_006f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // hold it first
            var holdResponse = saleResponse.Hold()
                .WithReasonCode(ReasonCode.OUTOFSTOCK)
                .Execute();
            Assert.IsNotNull(holdResponse);
            Assert.AreEqual("00", holdResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_006g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // hold it first
            var holdResponse = saleResponse.Hold()
                .WithReasonCode(ReasonCode.OUTOFSTOCK)
                .Execute();
            Assert.IsNotNull(holdResponse);
            Assert.AreEqual("00", holdResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_006h() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // hold it first
            var holdResponse = saleResponse.Hold()
                .WithReasonCode(ReasonCode.OUTOFSTOCK)
                .Execute();
            Assert.IsNotNull(holdResponse);
            Assert.AreEqual("00", holdResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_006i() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // hold it first
            var holdResponse = saleResponse.Hold()
                .WithReasonCode(ReasonCode.OUTOFSTOCK)
                .Execute();
            Assert.IsNotNull(holdResponse);
            Assert.AreEqual("00", holdResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_006k() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // hold it first
            var holdResponse = saleResponse.Hold()
                .WithReasonCode(ReasonCode.OUTOFSTOCK)
                .Execute();
            Assert.IsNotNull(holdResponse);
            Assert.AreEqual("00", holdResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_006l() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // hold it first
            var holdResponse = saleResponse.Hold()
                .WithReasonCode(ReasonCode.OUTOFSTOCK)
                .Execute();
            Assert.IsNotNull(holdResponse);
            Assert.AreEqual("00", holdResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_007a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // hold it first
            var holdResponse = saleResponse.Hold()
                .WithReasonCode(ReasonCode.OUTOFSTOCK)
                .Execute();
            Assert.IsNotNull(holdResponse);
            Assert.AreEqual("00", holdResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_007b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // hold it first
            var holdResponse = saleResponse.Hold()
                .WithReasonCode(ReasonCode.OUTOFSTOCK)
                .Execute();
            Assert.IsNotNull(holdResponse);
            Assert.AreEqual("00", holdResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_007c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // hold it first
            var holdResponse = saleResponse.Hold()
                .WithReasonCode(ReasonCode.OUTOFSTOCK)
                .Execute();
            Assert.IsNotNull(holdResponse);
            Assert.AreEqual("00", holdResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Release_007d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
            });

            var saleResponse = Transaction.FromId(null);

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Release_007e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
            });

            var saleResponse = Transaction.FromId(null);

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Release_008c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
            });

            var saleResponse = Transaction.FromId(null);

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Release_008d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
            });

            var saleResponse = Transaction.FromId(null);

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Release_008e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
            });

            var saleResponse = Transaction.FromId(null);

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Release_009d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            var saleResponse = Transaction.FromId(null);

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .WithDescription("DotNet_Query")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Release_009e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            var saleResponse = Transaction.FromId(null);

            // request
            var response = saleResponse.Release()
                .WithReasonCode(ParseReasonCode("INSTOCK"))
                .WithDescription("DotNet_Query")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_010a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("OTHER"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_010b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Release_010c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            var saleResponse = Transaction.FromId(null);

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("FRAUD"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_010d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("OTHER"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_011a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("OTHER"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_011b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("PIUM"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_011c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_011d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("#####"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_012a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("OTHER"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Release_012b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secreto",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("OTHER"))
                .WithDescription("DotNet_Query")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Release_013a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("OTHER"))
                .WithDescription("DotNet_Hold")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Release_013b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "EC"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("OTHER"))
                .WithDescription("DotNet_Query")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Release_013c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 5000,
                Channel = "ECOOOOOOOOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = saleResponse.Hold()
                .WithReasonCode(ParseReasonCode("OTHER"))
                .WithDescription("DotNet_Query")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_006a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-006a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_006b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-006b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_006c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-006c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_006d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-006d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_006e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-006e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_006f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-006f")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_006g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-006g")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_006h() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-006h")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_006i() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-006i")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_006j() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-006j")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_006k() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-006k")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_007a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-007a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_007b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-007b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_007c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-007c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_007d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-007d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_007e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-007e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_008a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-008a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_008b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-008b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_008c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-008c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_008d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-008d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_008e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-008e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_009a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOM"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-009a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_009b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "E"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-009b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_009c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
                Channel = "ECOMMERCE"
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-009c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_009d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-009d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_010a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-010a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_010b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-010b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_010c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-010c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_010d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-010d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_010e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-010e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_011a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-011a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_011b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-011b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_011c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-011c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Manual_011d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(10m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge()
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-011d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_012a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-012a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_012b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EURO")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EURO")
                .WithDescription("DotNet_Manual-012b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_012c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("�UR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("�UR")
                .WithDescription("DotNet_Manual-012c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DotNet_Manual_012d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Charge(100.01m)
                .WithDescription("DotNet_Manual-012d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_013a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-013a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_013b1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-013b1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_013b2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-013b2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_013c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-013c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_014a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-014a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_014b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-014b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_014c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-014c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_014d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James~Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-014d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_015a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-015a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_015b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-015b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_015c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 20,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-015c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_015d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-015d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_016a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-016a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_016b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-016b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_016c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-016c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_017a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-017a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_018a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-018a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_019a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-019a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_019b1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-019b1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_019b2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "374101000000608",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-019b2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_019c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "12345",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-019c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_019d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "374101000000608",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-019d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_020a1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-020a1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_020a2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "",
                CvnPresenceIndicator = (CvnPresenceIndicator)2,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-020a2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_020a3() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "",
                CvnPresenceIndicator = (CvnPresenceIndicator)3,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-020a3")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_020a4() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "",
                CvnPresenceIndicator = (CvnPresenceIndicator)4,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-020a4")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_020b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)5,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-020b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_020c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)0,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-020c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_021a1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-021a1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_021a2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Authorize(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-021a2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_021a3() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Authorize(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-021a3")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_021b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Authorize(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-021b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_021c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Authorize(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-021c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_022a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-022a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_022b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-022b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_022c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-022c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_022d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-022d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_022e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-022e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_023a1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-023a1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_023a2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-023a2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_023b1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-023b1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_023c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-023c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_024a1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-024a1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_024a2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-024a2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_024a3() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-024a3")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_024b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-024b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_024c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-024c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_025() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-025")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_026a1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-026a1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_026a2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_026b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_026c1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIep3uviSnW9XEB3a4wpIW9XEB3a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_026c2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-026c2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_027a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithCustomerId("123456")
                .WithDescription("DotNet_Manual-027a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_028a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("123456")
                .WithDescription("DotNet_Manual-028a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_028b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-028b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_028c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithCustomerId("3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep33a4wpQQQQQQQQQ1")
                .WithDescription("DotNet_Manual-028c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_028d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerId("123456~")
                .WithDescription("DotNet_Manual-028d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_029a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithProductId("123456")
                .WithDescription("DotNet_Manual-029a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_029b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithDescription("DotNet_Manual-029b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_029c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithProductId("3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep33a4wpQQQQQQQQQ1")
                .WithDescription("DotNet_Manual-029c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_029d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithProductId("123456~")
                .WithDescription("DotNet_Manual-029d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_030a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithClientTransactionId("123456")
                .WithDescription("DotNet_Manual-030a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_030b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-030b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_030c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithClientTransactionId("3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep33a4wpQQQQQQQQQ1")
                .WithDescription("DotNet_Manual-030c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_030d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithClientTransactionId("123456~")
                .WithDescription("DotNet_Manual-030d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_031a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerIpAddress("123.123.123.123")
                .WithDescription("DotNet_Manual-031a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_031b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-031b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_031c1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithCustomerIpAddress("1234.123.123.123")
                .WithDescription("DotNet_Manual-031c1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_031c2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithCustomerIpAddress("123~.123.123.123")
                .WithDescription("DotNet_Manual-031c2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_032a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "E77 4QJ",
                Country = "United Kingdom"
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "Z76 PO9",
                Country = "France"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Manual-032a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_033a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "774|10",
            };

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "769|52",
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Manual-033a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_033b1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "774|10",
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithAddress(billingAddress)
                .WithDescription("DotNet_Manual-033b1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_033b2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "769|52",
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Manual-033b2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_033c1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // billing address
            var billingAddress = new Address {
                PostalCode = "3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwep4wpIwep3u111",
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithAddress(billingAddress)
                .WithDescription("DotNet_Manual-033c1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_033c2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // shipping address
            var shippingAddress = new Address {
                PostalCode = "3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwep4wpIwep3u111",
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Manual-033c2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_034a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // billing address
            var billingAddress = new Address {
                Country = "GB"
            };

            // shipping address
            var shippingAddress = new Address {
                Country = "FR"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithAddress(billingAddress)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Manual-034a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_034b1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // shipping address
            var shippingAddress = new Address {
                Country = "GB"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Manual-034b1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_034b2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // billing address
            var billingAddress = new Address {
                Country = "GB"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithAddress(billingAddress)
                .WithDescription("DotNet_Manual-034b2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_034c1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // billing address
            var billingAddress = new Address {
                Country = "3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwep4wpIwep3u111"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithAddress(billingAddress)
                .WithDescription("DotNet_Manual-034c1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_Manual_034c2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // shipping address
            var shippingAddress = new Address {
                Country = "3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwep4wpIwep3u111"
            };

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("USD")
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithDescription("DotNet_Manual-034c2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_035a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("GBP")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("GBP")
                .WithDescription("DotNet_Manual-035a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_Manual_035b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 20000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // build transaction
            var saleResponse = card.Charge(100.01m)
                .WithCurrency("EUR")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            Throttle();

            // request
            var response = card.Charge(100.01m)
                .WithCurrency("EUR")
                .WithDescription("DotNet_Manual-035a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_014a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001038443335",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-014a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_014b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001038488884",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-014b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_014c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001036298889",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-014c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_014d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001036853337",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-014d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_014e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-014e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_014f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037167778",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-014f")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_014g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-014g")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_014h() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037484447",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-014h")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_014i() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037490006",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-014i")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_verifyenrolled_015a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "5100000000000198�",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-015a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void DotNet_verifyenrolled_015b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "5100000000000149",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-015b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void DotNet_verifyenrolled_015c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "5100000000000172",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-015c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void DotNet_verifyenrolled_015d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "5100000000000297",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-015d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void DotNet_verifyenrolled_015e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "5100000000000131",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-015e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void DotNet_verifyenrolled_015f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "5100000000000206",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-015f")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void DotNet_verifyenrolled_015g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "5100000000000131",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-015g")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void DotNet_verifyenrolled_015h() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "5100000000000214",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-015h")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void DotNet_verifyenrolled_015i() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "5100000000000164",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-015i")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_verifyenrolled_016a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "370537726695896�",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-016a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void DotNet_verifyenrolled_016b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "344598846104303",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-016b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void DotNet_verifyenrolled_016c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "342911579886552",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-016c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void DotNet_verifyenrolled_016d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "377775599797356",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-016d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void DotNet_verifyenrolled_016e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "371810438025523",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-016e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void DotNet_verifyenrolled_016f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "374973180958759",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-016f")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void DotNet_verifyenrolled_016g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "371810438025523",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-016g")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void DotNet_verifyenrolled_016h() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "376515222233960",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-016h")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void DotNet_verifyenrolled_016i() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "372749236937027",
                ExpMonth = 10,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-016i")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_017a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-017a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_017b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-017b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_017c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("USD")
                .WithDescription("DotNet_verifyenrolled-017c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_017d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-017d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_017e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-017e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_017f() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("USD")
                .WithDescription("DotNet_verifyenrolled-017f")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_017g() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-017g")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_017h() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-017h")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_017i() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("USD")
                .WithDescription("DotNet_verifyenrolled-017i")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_017j() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-017j")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_017k() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-017k")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_018a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-018a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_018b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-018b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_018c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("USD")
                .WithDescription("DotNet_verifyenrolled-018c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_018d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-018d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_018e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-018e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_019a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("USD")
                .WithDescription("DotNet_verifyenrolled-019a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_019b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-019b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_019c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-019c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_019d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("USD")
                .WithDescription("DotNet_verifyenrolled-019d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_019e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-019e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_020b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-020b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_020c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-020c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_020d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("USD")
                .WithDescription("DotNet_verifyenrolled-020d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_020e() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-020e")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_021a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-021a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_021b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("USD")
                .WithDescription("DotNet_verifyenrolled-021b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_021c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-021c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_021d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-021d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_022a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("USD")
                .WithDescription("DotNet_verifyenrolled-022a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_022b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EURO")
                .WithDescription("DotNet_verifyenrolled-022b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_022c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("�UR")
                .WithDescription("DotNet_verifyenrolled-022c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_022d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithDescription("DotNet_verifyenrolled-022d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_023a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-023a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_023b1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-023b1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_verifyenrolled_023b2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "42424242424",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("USD")
                .WithDescription("DotNet_verifyenrolled-023b2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_verifyenrolled_023c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4263970000005262#",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-023c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_024a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-024a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_verifyenrolled_024b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
            };

            // request
            var response = card.Verify()
                .WithCurrency("USD")
                .WithDescription("DotNet_verifyenrolled-024b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_verifyenrolled_024c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep3a4wpIwep3uviSnW9XEB3a4wpIwep3uviSnW9XEB3a4wpIwepeep"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-024c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_024d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James~Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-024d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_025a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("USD")
                .WithDescription("DotNet_verifyenrolled-025a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_025b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-025b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_verifyenrolled_025c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 20,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-025c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_verifyenrolled_025d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("USD")
                .WithDescription("DotNet_verifyenrolled-025d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_026a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-026a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_026b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-026b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_026c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("USD")
                .WithDescription("DotNet_verifyenrolled-026c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_027a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-027a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_028a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-028a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_029a() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("USD")
                .WithDescription("DotNet_verifyenrolled-029a")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_verifyenrolled_029b1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-029b1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_verifyenrolled_029b2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "371810438025523",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-029b2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayException))]
        public void DotNet_verifyenrolled_029c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "12345",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-029c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void DotNet_verifyenrolled_029d() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "371810438025523",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "1234",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("USD")
                .WithDescription("DotNet_verifyenrolled-029d")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_030a1() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)1,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-030a1")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_030a2() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "",
                CvnPresenceIndicator = (CvnPresenceIndicator)2,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-030a2")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_030a3() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "",
                CvnPresenceIndicator = (CvnPresenceIndicator)3,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("USD")
                .WithDescription("DotNet_verifyenrolled-030a3")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_030a4() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "",
                CvnPresenceIndicator = (CvnPresenceIndicator)4,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("GBP")
                .WithDescription("DotNet_verifyenrolled-030a4")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void DotNet_verifyenrolled_030b() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)5,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("EUR")
                .WithDescription("DotNet_verifyenrolled-030b")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DotNet_verifyenrolled_030c() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RefundPassword = "refund",
                RebatePassword = "rebate",
                Timeout = 60000,
            });

            // create card
            var card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = expYear,
                Cvn = "123",
                CvnPresenceIndicator = (CvnPresenceIndicator)0,
                CardHolderName = "James Mason"
            };

            // request
            var response = card.Verify()
                .WithCurrency("USD")
                .WithDescription("DotNet_verifyenrolled-030c")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        private ReasonCode? ParseReasonCode(string value) {
            ReasonCode code;
            if (Enum.TryParse("", out code)) {
                return (ReasonCode?)code;
            }
            return null;
        }
    }
}
