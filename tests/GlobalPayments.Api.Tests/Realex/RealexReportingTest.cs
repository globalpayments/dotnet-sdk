using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.Realex {
    [TestClass]
    public class CathalReportingTest {
        public CathalReportingTest() {
            ServicesContainer.ConfigureService(new GatewayConfig {
                DataClientId = "2413abd8-ea1f-4f0e-a4b5-eb5ca682efe2",
                DataClientSecret = "nQ3eO3gV1sV7qC7vX8vY3nB1qR4oQ0dH6wI6wN4aA1oA3sP3aL",
                DataClientUserId = "INTAPIUK",
                ServiceUrl = "https://api.sandbox.realexpayments.com/epage-remote.cgi",
                Timeout = 240000
            });
        }

        [TestMethod]
        public void GetTransactions() {
            var response = ReportingService.FindTransactions()
                .Where(DataServiceCriteria.StartDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.EndDepositDate, DateTime.Parse("02/01/2018"))
                .Execute();
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void GetTransactionByHierarchy() {
            var response = ReportingService.FindTransactions()
                .Where(DataServiceCriteria.StartDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.EndDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.Hierarchy, "052-03-001-001-BTE")
                .Execute();
            Assert.IsNotNull(response);

            foreach (var trans in response)
                Assert.AreEqual("052-03-001-001-BTE", trans.MerchantHierarchy);
        }

        [TestMethod]
        public void GetTransactionByMerchantId() {
            var response = ReportingService.FindTransactions()
                .Where(DataServiceCriteria.StartDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.EndDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.MerchantId, "1474961")
                .Execute();
            Assert.IsNotNull(response);

            foreach (var trans in response)
                Assert.AreEqual("1474961", trans.MerchantId);
        }

        [TestMethod]
        public void GetTransactionByDepositReference() {
            var response = ReportingService.FindTransactions()
                .Where(DataServiceCriteria.StartDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.EndDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.DepositReference, "20180201E034974-001")
                .Execute();
            Assert.IsNotNull(response);

            foreach (var trans in response)
                Assert.AreEqual("20180201E034974-001", trans.DepositReference);
        }

        [TestMethod]
        public void GetTransactionByOrderId() {
            var response = ReportingService.FindTransactions()
                .Where(DataServiceCriteria.StartDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.EndDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.OrderId, "1886b4049dd4")
                .Execute();
            Assert.IsNotNull(response);

            foreach (var trans in response)
                Assert.AreEqual("1886b4049dd4", trans.OrderId);
        }

        [TestMethod]
        public void GetTransactionByLocalDate() {
            var response = ReportingService.FindTransactions()
                .Where(DataServiceCriteria.StartDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.EndDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.LocalTransactionStartTime, DateTime.Parse("1/31/2018 5:29:52 PM"))
                .Execute();
            Assert.IsNotNull(response);

            foreach (var trans in response)
                Assert.AreEqual("1/31/2018 5:29:52 PM", trans.TransactionLocalDate);
        }

        [TestMethod]
        public void GetDeposits() {
            var response = ReportingService.FindDeposits()
                .Where(DataServiceCriteria.StartDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.EndDepositDate, DateTime.Parse("02/28/2018"))
                .Execute();
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void GetDepositByAmount() {
            var response = ReportingService.FindDeposits()
                .Where(DataServiceCriteria.StartDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.EndDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.Amount, -12.67m)
                .Execute();
            Assert.IsNotNull(response);

            foreach (var deposit in response)
                Assert.AreEqual(-12.67m, deposit.Amount);
        }

        [TestMethod]
        public void GetDepositByAccountNumber() {
            var response = ReportingService.FindDeposits()
                .Where(DataServiceCriteria.StartDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.EndDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.BankAccountNumber, "21672479")
                .Execute();
            Assert.IsNotNull(response);

            foreach (var deposit in response)
                Assert.AreEqual("21672479", deposit.AccountNumber);
        }

        [TestMethod]
        public void GetDisputes() {
            var response = ReportingService.FindDisputes()
                .Where(DataServiceCriteria.StartDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.EndDepositDate, DateTime.Parse("02/01/2018"))
                .Execute();
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void GetDisputeByCaseId() {
            var response = ReportingService.FindDisputes()
                .Where(DataServiceCriteria.StartDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.EndDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.CaseId, "D-4991109")
                .Execute();
            Assert.IsNotNull(response);

            foreach (var dispute in response)
                Assert.AreEqual("D-4991109", dispute.CaseId);
        }

        [TestMethod]
        public void GetDisputeByCaseNumber() {
            var response = ReportingService.FindDisputes()
                .Where(DataServiceCriteria.StartDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.EndDepositDate, DateTime.Parse("02/01/2018"))
                .And(DataServiceCriteria.CaseNumber, "6803102066")
                .Execute();
            Assert.IsNotNull(response);

            foreach (var dispute in response)
                Assert.AreEqual("6803102066", dispute.CaseNumber);
        }
    }
}
