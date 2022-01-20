using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils.Logging;
using GlobalPayments.Api.Tests.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Tests.Certifications.TransIT {
    [TestClass]
    public abstract class TransitBaseTestClass {
        protected string _firstTransactionId;
        private IncrementalNumberProvider _generator;
        protected string _lastTransactionId;
        private static RequestFileLogger _logger;

        protected Address Address { get; private set; }
        protected CreditCardData AmexManual { get; private set; }
        protected CreditCardData AmexToken { get; private set; }
        protected string ClientTransactionId {
            get {
                if (_generator == null) {
                    _generator = IncrementalNumberProvider.GetInstance();
                }
                return _generator.GetRequestId().ToString();
            }
        }
        protected CreditCardData DinersManual { get; private set; }
        protected CreditCardData DinersToken { get; private set; }
        protected CreditCardData DiscoverCupManual { get; private set; }
        protected CreditCardData DiscoverCupToken { get; private set; }
        protected CreditCardData DiscoverManual { get; private set; }
        protected CreditCardData DiscoverToken { get; private set; }
        protected CreditCardData JcbManual { get; private set; }
        protected CreditCardData JcbToken { get; private set; }
        protected static RequestFileLogger Logger {
            get {
                if (_logger == null) {
                    _logger = new RequestFileLogger(@"C:\temp\transit\requestlog.txt");
                }
                return _logger;
            }
        }
        protected CreditCardData MasterCardBin2Manual { get; private set; }
        protected CreditCardData MasterCardManual { get; private set; }
        protected CreditCardData MasterCardToken { get; private set; }
        protected CreditCardData MasterCardBin2Token { get; private set; }
        protected CreditCardData VisaManual { get; private set; }
        protected CreditCardData VisaToken { get; private set; }

        protected virtual AcceptorConfig AcceptorConfig {
            get {
                return new AcceptorConfig {
                    /* The following are the default values for the AcceptorConfig */

                    //CardDataInputCapability = CardDataInputCapability.MagStripe_KeyEntry,
                    //OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Attended,
                    //CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.None,
                    //TerminalOutputCapability = TerminalOutputCapability.Unknown,
                    //PinCaptureCapability = PinCaptureCapability.Unknown,
                    //CardCaptureCapability = false,
                    //CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.NotAuthenticated,
                    //CardDataOutputCapability = CardDataOutputCapability.None
                };
            }
        }

        protected Transaction Response { get; set; }

        public TransitBaseTestClass() {
            ServicesContainer.ConfigureService(new TransitConfig {
                AcceptorConfig = AcceptorConfig,
                DeveloperId = "003282G001",
                DeviceId = "88700000328201",
                MerchantId = "887000003282",
                TransactionKey = "VI476HZHVXWBNYA1JL4C6K1S6LDHVST5",
                RequestLogger = Logger
            });

            Address = new Address {
                StreetAddress1 = "8320",
                PostalCode = "85284"
            };

            AmexManual = new CreditCardData {
                Number = "371449635392376",
                ExpMonth = 12,
                ExpYear = 2020,
                Cvn = "9997"
            };

            DinersManual = new CreditCardData {
                Number = "3055155515160018",
                ExpMonth = 12,
                ExpYear = 2020,
                Cvn = "996"
            };

            DiscoverCupManual = new CreditCardData {
                Number = "6282000123842342",
                ExpMonth = 12,
                ExpYear = 2020,
                Cvn = "996"
            };

            DiscoverManual = new CreditCardData {
                Number = "6011000993026909",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "996"
            };

            JcbManual = new CreditCardData {
                Number = "3530142019945859",
                ExpMonth = 12,
                ExpYear = 2020,
                Cvn = "996"
            };

            MasterCardBin2Manual = new CreditCardData {
                Number = "2223000048400011",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn= "998"
            };

            MasterCardManual = new CreditCardData {
                Number = "5146315000000055",
                ExpMonth = 12,
                ExpYear = 2020,
                Cvn = "998"
            };

            VisaManual = new CreditCardData {
                Number = "4012000098765439",
                ExpMonth = 12, 
                ExpYear = 2020,
                Cvn = "999"
            };

            VisaToken = new CreditCardData {
                Token = "JYzDjhCWzImG5439",
                ExpMonth = 12,
                ExpYear = 2020
            };

            MasterCardToken = new CreditCardData {
                Token = "LRzFCAe4zzc50055",
                ExpMonth = 12,
                ExpYear = 2020
            };

            MasterCardBin2Token = new CreditCardData {
                Token = "YEZQD4GQTjj0001",
                ExpMonth = 12,
                ExpYear = 2025
            };

            DiscoverToken = new CreditCardData {
                Token = "eJ4QTv9ewZ506909",
                ExpMonth = 12,
                ExpYear = 2020
            };

            AmexToken = new CreditCardData {
                Token = "CtU9Fabp9E12376",
                ExpMonth = 12,
                ExpYear = 2020
            };

            JcbToken = new CreditCardData {
                Token = "FA1U0MrpYDsp5859",
                ExpMonth = 12,
                ExpYear = 2020
            };

            DiscoverCupToken = new CreditCardData {
                Token = "vd8F2vAQv03p2342",
                ExpMonth = 12,
                ExpYear = 2020
            };

            DinersToken = new CreditCardData {
                Token = "W8Ng8PDpetxr0018",
                ExpMonth = 12,
                ExpYear = 2020
            };
        }

        [TestCleanup]
        public void TestCleanup() {
            if (Response != null) {
                Assert.IsNotNull(Response);

                var acceptedCodes = new List<string>() { "00", "10" };
                Assert.IsTrue(acceptedCodes.Contains(Response.ResponseCode));

                if (_firstTransactionId == null) {
                    Logger.AppendText("Authorization Start Date/Time: {0}", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));
                    _firstTransactionId = Response.TransactionId;
                }
                else {
                    _lastTransactionId = Response.TransactionId;
                }

                Response = null;
            }
        }
    }
}
