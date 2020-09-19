using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Tests.Network {
    [TestClass]
    public class NWSBatchTests {
        private string configName = "default";
        private BatchProvider batchProvider;

        public NWSBatchTests() {
            Address address = new Address {
                Name = "My STORE",
                StreetAddress1 = "1 MY STREET",
                City = "MYTOWN",
                PostalCode = "90210",
                State = "KY",
                Country = "USA"
            };

            AcceptorConfig acceptorConfig = new AcceptorConfig();
            acceptorConfig.Address = address;

            // data code values
            acceptorConfig.CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe_KeyEntry;
            acceptorConfig.TerminalOutputCapability = TerminalOutputCapability.Printing_Display;

            // hardware software config values
            acceptorConfig.HardwareLevel = "34";
            acceptorConfig.SoftwareLevel = "21205710";
            acceptorConfig.OperatingSystemLevel = "00";

            // pos configuration values
            acceptorConfig.SupportsPartialApproval = true;
            acceptorConfig.SupportsShutOffAmount = true;
            acceptorConfig.SupportsReturnBalance = true;
            acceptorConfig.SupportsDiscoverNetworkReferenceId = true;
            acceptorConfig.SupportsAvsCnvVoidReferrals = true;

            batchProvider = BatchProvider.GetInstance();

            // gateway config
            NetworkGatewayConfig config = new NetworkGatewayConfig(NetworkGatewayType.NWS);
            config.SecondaryEndpoint = "test.txns-c.secureexchange.net";
            config.PrimaryPort = 15031;
            config.ServiceUrl = "test.txns-e.secureexchange.net";
            config.SecondaryPort = 15031;
            config.CompanyId = "SPSA";
            config.TerminalId = "NWSDOTNET02";
            config.UniqueDeviceId = "0001";
            config.MerchantType = "5542";
            config.AcceptorConfig = acceptorConfig;
            config.EnableLogging = true;
            config.StanProvider = StanGenerator.GetInstance();
            config.BatchProvider = batchProvider;

            ServicesContainer.ConfigureService(config);

            config.BatchProvider = null;
            ServicesContainer.ConfigureService(config, "NoBatch");
        }

        [TestMethod]
        public void Test_000_batch_close() {
            BatchSummary summary = BatchService.CloseBatch();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary.IsBalanced);
        }

        //[TestMethod]
        //public void Test_001_resubmits_provider() {
        //    configName = "default";

        //    CreditSale(10);
        //    CreditAuth(20);
        //    DebitSale(30);
        //    DebitAuth(50);

        //    BatchSummary summary = BatchService.CloseBatch(1, 30m, 80m);
        //    Assert.IsNotNull(summary);
        //}

        //[TestMethod]
        //public void Test_002_resubmits_no_provider() {
        //    configName = "NoBatch";

        //    Transaction creditSale = CreditSale(10);
        //    Assert.IsNotNull(creditSale.TransactionToken);

        //    Transaction creditAuth = CreditAuth(20);
        //    Assert.IsNotNull(creditAuth.TransactionToken);

        //    Transaction debitSale = DebitSale(30);
        //    Assert.IsNotNull(debitSale.TransactionToken);

        //    Transaction debitAuth = DebitAuth(50);
        //    Assert.IsNotNull(debitAuth.TransactionToken);

        //    BatchSummary summary = BatchService.CloseBatch(
        //                batchProvider.GetBatchNumber(),
        //                batchProvider.GetSequenceNumber(),
        //                new decimal(30),
        //                new decimal(80),
        //                configName
        //        );
        //    Assert.IsNotNull(summary);
        //    Assert.IsNotNull(summary.TransactionToken);

        //    if (summary.ResponseCode.Equals("580")) {
        //        List<string> tokens = new List<string>();
        //        tokens.Add(creditSale.TransactionToken);
        //        tokens.Add(creditAuth.TransactionToken);
        //        tokens.Add(debitSale.TransactionToken);
        //        tokens.Add(debitAuth.TransactionToken);

        //        BatchSummary newSummary = summary.ResubmitTransactions(tokens, configName);
        //        Assert.IsNotNull(newSummary);
        //        Assert.AreEqual(4, newSummary.ResentTransactions.Count);
        //        foreach (Transaction re in newSummary.ResentTransactions)
        //        {
        //            Assert.AreEqual("000", re.ResponseCode);
        //        }
        //    }
        //    else {
        //        Assert.IsTrue(summary.ResponseCode.Equals("500") || summary.ResponseCode.Equals("501"));
        //    }
        //}

        [TestMethod]
        public void Test_240_batchClose_EndOfShift() {
            configName = "default";

            CreditSale(100);
            CreditSale(100);
            CreditSale(10);
            DebitSale(10);
            DebitSale(10);
            DebitSale(1);

            BatchSummary response = BatchService.CloseBatch(BatchCloseType.EndOfShift);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsBalanced);
        }

        private Transaction CreditAuth(double amount) {
            CreditTrackData track = TestCards.VisaSwipe();

            AuthorizationBuilder builder = track.Authorize(new decimal(amount))
                        .WithCurrency("USD");

            if (configName.Equals("NoBatch")) {
                builder.WithBatchNumber(batchProvider.GetBatchNumber(), batchProvider.GetSequenceNumber());
            }

            Transaction response = builder.Execute(configName);
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
            return capture;
        }

        private Transaction CreditSale(double amount) {
            CreditTrackData track = TestCards.VisaSwipe();

            AuthorizationBuilder builder = track.Charge(new decimal(amount))
                        .WithCurrency("USD");

            if (configName.Equals("NoBatch")) {
                builder.WithBatchNumber(batchProvider.GetBatchNumber(), batchProvider.GetSequenceNumber());
            }

            Transaction response = builder.Execute(configName);
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
            return response;
        }

        private Transaction DebitAuth(double amount) {
            DebitTrackData track = TestCards.AsDebit(TestCards.VisaSwipe(), "32539F50C245A6A93D123412324000AA");

            AuthorizationBuilder builder = track.Authorize(new decimal(amount))
                        .WithCurrency("USD");

            if (configName.Equals("NoBatch")) {
                builder.WithBatchNumber(batchProvider.GetBatchNumber(), batchProvider.GetSequenceNumber());
            }

            Transaction response = builder.Execute(configName);
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("000", capture.ResponseCode);
            return capture;
        }

        private Transaction DebitSale(double amount) {
            DebitTrackData track = TestCards.AsDebit(TestCards.VisaSwipe(), "32539F50C245A6A93D123412324000AA");

            AuthorizationBuilder builder = track.Charge(new decimal(amount))
                        .WithCurrency("USD");

            if (configName.Equals("NoBatch")) {
                builder.WithBatchNumber(batchProvider.GetBatchNumber(), batchProvider.GetSequenceNumber());
            }

            Transaction response = builder.Execute(configName);
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
            return response;
        }
    }
}
