using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiAuthenticationTests {
        [TestMethod]
        public void GenerateAccessTokenManual() {
            var environment = Entities.Environment.TEST;
            string appId = "JF2GQpeCrOivkBGsTRiqkpkdKp67Gxi0";
            string appKey = "y7vALnUtFulORlTV";
            
            AccessTokenInfo info = GpApiService.GenerateTransactionKey(environment, appId, appKey);

            Assert.IsNotNull(info);
            Assert.IsNotNull(info.Token);
            Assert.IsNotNull(info.DataAccountName);
            Assert.IsNotNull(info.DisputeManagementAccountName);
            Assert.IsNotNull(info.TokenizationAccountName);
            Assert.IsNotNull(info.TransactionProcessingAccountName);
        }
    }
}
