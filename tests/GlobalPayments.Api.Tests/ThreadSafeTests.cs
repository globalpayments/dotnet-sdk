using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GlobalPayments.Api.Tests {
    [TestClass]
    public class ThreadSafeTests {
        [TestMethod]
        public void AddConfigThreadSafe() {
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 05,
                ExpYear = 2025,
                Cvn = "852",
                CardPresent = true
            };

            var configs = new List<GpApiConfig>();
            for(int i = 0; i < 100; i++) {
                configs.Add(new GpApiConfig {
                    AppId = "rkiYguPfTurmGcVhkDbIGKn2IJe2t09M",
                    AppKey = "6gFzVGf40S7ZpjJs",
                    Channel = Channel.CardNotPresent,
                    ChallengeNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                    MethodNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                    MerchantContactUrl = "https://enp4qhvjseljg.x.pipedream.net/",
                    DynamicHeaders = new Dictionary<string, string> {
                        ["configname"] = "config"+i,
                    }
                });
            }

            Parallel.ForEach(configs, config => {
                ServicesContainer.ConfigureService(config, config.DynamicHeaders["configname"]);
            });

            Parallel.ForEach(configs, config => {
                var transaction = card.Charge(14m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute(config.DynamicHeaders["configname"]);
                Assert.IsNotNull(transaction);
                Assert.AreEqual("SUCCESS", transaction?.ResponseCode);
                Assert.AreEqual(EnumConverter.GetMapping(Target.GP_API, TransactionStatus.Captured), transaction?.ResponseMessage);
            });
        }

        [TestMethod]
        public void AddRemoveConfigThreadSafe() {
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 05,
                ExpYear = 2025,
                Cvn = "852",
                CardPresent = true
            };

            var configs = new List<GpApiConfig>();
            for (int i = 0; i < 100; i++) {
                configs.Add(new GpApiConfig {
                    AppId = "rkiYguPfTurmGcVhkDbIGKn2IJe2t09M",
                    AppKey = "6gFzVGf40S7ZpjJs",
                    Channel = Channel.CardNotPresent,
                    ChallengeNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                    MethodNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                    MerchantContactUrl = "https://enp4qhvjseljg.x.pipedream.net/",
                    DynamicHeaders = new Dictionary<string, string> {
                        ["configname"] = "config" + i,
                    }
                });
            }

            Parallel.ForEach(configs, config => {
                ServicesContainer.ConfigureService(config, config.DynamicHeaders["configname"]);
            });

            Parallel.ForEach(configs, config => {
                var transaction = card.Charge(14m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute(config.DynamicHeaders["configname"]);
                Assert.IsNotNull(transaction);
                Assert.AreEqual("SUCCESS", transaction?.ResponseCode);
                Assert.AreEqual(EnumConverter.GetMapping(Target.GP_API, TransactionStatus.Captured), transaction?.ResponseMessage);
            });

            Parallel.ForEach(configs, config => {
                ServicesContainer.ConfigureService<GpApiConfig>(null, config.DynamicHeaders["configname"]);
            });

            Parallel.ForEach(configs, config => {
                try {
                    var transaction = card.Charge(14m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute(config.DynamicHeaders["configname"]);
                }
                catch (Exception ex) {
                    Assert.AreEqual("The specified configuration has not been configured for gateway processing.", ex.Message);
                }
            });
        }

        [TestMethod]
        public void AddConfigChargeRemoveThreadSafe() {
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 05,
                ExpYear = 2025,
                Cvn = "852",
                CardPresent = true
            };

            var configs = new List<GpApiConfig>();
            for (int i = 0; i < 100; i++) {
                configs.Add(new GpApiConfig {
                    AppId = "rkiYguPfTurmGcVhkDbIGKn2IJe2t09M",
                    AppKey = "6gFzVGf40S7ZpjJs",
                    Channel = Channel.CardNotPresent,
                    ChallengeNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                    MethodNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                    MerchantContactUrl = "https://enp4qhvjseljg.x.pipedream.net/",
                    DynamicHeaders = new Dictionary<string, string> {
                        ["configname"] = "config" + i,
                    }
                });
            }

            Parallel.ForEach(configs, config => {
                var configName = config.DynamicHeaders["configname"];
                ServicesContainer.ConfigureService(config, configName);
                var transaction = card.Charge(14m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute(config.DynamicHeaders["configname"]);
                Assert.IsNotNull(transaction);
                Assert.AreEqual("SUCCESS", transaction?.ResponseCode);
                Assert.AreEqual(EnumConverter.GetMapping(Target.GP_API, TransactionStatus.Captured), transaction?.ResponseMessage);
                ServicesContainer.ConfigureService<GpApiConfig>(null, configName);
            });
        }

        [TestMethod]
        public void AddConfigException() {
            var configs = new List<GpApiConfig>();
            for (int i = 0; i < 100; i++) {
                configs.Add(new GpApiConfig {
                    AppId = "rkiYguPfTurmGcVhkDbIGKn2IJe2t09M",
                    AppKey = "6gFzVGf40S7ZpjJs",
                    Channel = Channel.CardNotPresent,
                    ChallengeNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                    MethodNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                    MerchantContactUrl = "https://enp4qhvjseljg.x.pipedream.net/",
                    DynamicHeaders = new Dictionary<string, string> {
                        ["configname"] = "config" + i,
                    }
                });
            }

            Parallel.ForEach(configs, config => {
                try {
                ServicesContainer.ConfigureService<GpApiConfig>(config);
                }
                catch (ConfigurationException configEx) {
                    Assert.AreEqual("Failed to add configuration: default.", configEx.Message);
                }
            });
        }

        [TestMethod]
        public void AddRemoveConfigException() {
            var configs = new List<GpApiConfig>();
            for (int i = 0; i < 100; i++) {
                configs.Add(new GpApiConfig {
                    AppId = "rkiYguPfTurmGcVhkDbIGKn2IJe2t09M",
                    AppKey = "6gFzVGf40S7ZpjJs",
                    Channel = Channel.CardNotPresent,
                    ChallengeNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                    MethodNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                    MerchantContactUrl = "https://enp4qhvjseljg.x.pipedream.net/",
                    DynamicHeaders = new Dictionary<string, string> {
                        ["configname"] = "config" + i,
                    }
                });
            }

            Parallel.ForEach(configs, config => {
                try {
                    ServicesContainer.ConfigureService<GpApiConfig>(config);
                }
                catch (ConfigurationException configEx) {
                    Assert.AreEqual("Failed to add configuration: default.", configEx.Message);
                }
            });

            Parallel.ForEach(configs, config => {
                try {
                    ServicesContainer.ConfigureService<GpApiConfig>(null);
                }
                catch (ConfigurationException configEx) {
                    Assert.AreEqual("Failed to remove configuration: default.", configEx.Message);
                }
            });

        }
    }
}
